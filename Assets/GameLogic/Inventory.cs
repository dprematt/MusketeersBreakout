using System.Collections.Generic;
using UnityEngine;
using System;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using PlayFab.CloudScriptModels;

public class Inventory : MonoBehaviourPunCallbacks
{
    public int SLOTS;
    PhotonView view;
    public IInventoryItem[] mItems;
    public bool loot = false;
    public bool test = false;
    public string LastItemName;

    public event EventHandler<InventoryEventArgs> ItemAdded;
    public event EventHandler<InventoryEventArgs> ItemInsertedAt;
    public event EventHandler<InventoryEventArgs> ItemRemoved;

    public void AddEnemyWeapon(Weapon weapon)
    {
        //GameObject weaponPrefab = Resources.Load<GameObject>(weaponName);
        // if (weaponPrefab == null)
        // {
        //     Debug.LogError("ENEMY Weapon not found in folder Resources : " + weaponName);
        //     return;
        // }
        // Vector3 pos;
        // pos.z = 0;
        // pos.y = 0;
        // pos.x = 0;
        //GameObject weaponObject = PhotonNetwork.Instantiate(weaponPrefab.name, pos, Quaternion.identity);
        //Weapon weaponItem = weaponObject.GetComponent<Weapon>();
        //Destroy(weaponPrefab);
        if (weapon == null)
        {
            //Debug.LogError("ENEMY Weapon doesn't implement IInventoryItem interface: " + weapon);
            // Destroy(weaponObject);
            return;
        }
        AddItem(weapon);
        //weaponObject.SetActive(false);
    }
    public void AddWeapon(string weaponName)
    {
        GameObject weaponPrefab = Resources.Load<GameObject>(weaponName);
        if (weaponPrefab == null)
        {
            return;
        }
        Vector3 pos;
        pos.z = 0;
        pos.y = 0;
        pos.x = 0;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        Player playerScript = playerObj.GetComponent<Player>();
        GameObject weaponObject = PhotonNetwork.Instantiate(weaponPrefab.name, pos, Quaternion.identity);
        Weapon weaponItem = weaponObject.GetComponent<Weapon>();
        //Destroy(weaponObject);
        Destroy(weaponPrefab);
        if (weaponItem == null)
        {
            Destroy(weaponObject);
            return;
        }

        //AddItem(weaponItem);
        playerScript.EquipWeapon(weaponItem, weaponObject, true);
    }

    public int Count()
    {
        int res = 0;

        if (mItems == null)
            return 0;
        for (int i = 0; i < mItems.Length; i++)
        {
            if (mItems[i] != null)
                res += 1;
        }
        return res;
    }


    public void InsertAt(IInventoryItem item, int id)
    {
        mItems[id] = item;
    }
    public int Add(IInventoryItem item)
    {
        for (int i = 0; i < mItems.Length; i++)
        {
            if (mItems[i] == null)
            {
                mItems[i] = item;
                return 0;
            }
        }
        return -1;
    }

    public int PocketCount()
    {
        int res = 0;

        if (mItems[0] != null)
            res += 1;
        if (mItems[1] != null)
            res += 1;
        return res;
    }

    public void RemoveAt(int index)
    {
        mItems[index] = null;
    }

    public void Initialize(int slots, IInventoryItem[] items, bool isLoot)
    {
        SLOTS = slots;
        mItems = items;
        loot = isLoot;
    }

    public Inventory(int slots, IInventoryItem[] items, bool isLoot)
    {
        SLOTS = slots;
        mItems = items;
        loot = isLoot;
    }
    public void Start()
    {
        SLOTS = 9;
        view = GetComponent<PhotonView>();
        if (mItems == null)
        {
            mItems = new IInventoryItem[9];
        }
    }

    public IInventoryItem[] GetInventory()
    {
        return mItems;
    }

    public void SwapItems(int index1, int index2)
    {
        if (index1 >= 0 && index1 < mItems.Length)
        {
            IInventoryItem item1 = mItems[index1];
            if (item1 != null && item1.GameObject != null)
            {
                item1.GameObject.SetActive(false);
            }
        }

        if (index2 >= 0 && index2 < mItems.Length)
        {
            IInventoryItem item2 = mItems[index2];
            if (item2 != null && item2.GameObject != null)
            {
                item2.GameObject.SetActive(true);
            }
        }

        IInventoryItem temp = mItems[index1];
        InsertAt(mItems[index2], index1);
        InsertAt(temp, index2);
    }
    public void SwapItemsLoot(int index1, int index2, Inventory lootInventory)
    {
        IInventoryItem temp = mItems[index1];
        InsertAt(lootInventory.mItems[index2], index1);
        lootInventory.InsertAt(temp, index2);
    }
    public void Print_Inventory()
    {
        Debug.Log("PRINT INVENTORY:");
        IInventoryItem[] Item = GetInventory();

        for (int i = 0; i < mItems.Length; i++)
        {
            if (mItems[i] != null)
                Debug.Log("Item n�" + i + " = " + mItems[i].Name);
            else
            {
                Debug.Log("Item n�" + i + " = Null");
            }
        }
        Debug.Log("end of print inventory");
    }

    public void InsertItem(IInventoryItem item, int index)
    {
        if (Count() == 0)
            mItems = new IInventoryItem[9];
        InsertAt(item, index);
        Print_Inventory();
        if (ItemAdded != null)
        {
            ItemInsertedAt(this, new InventoryEventArgs(item, index));
        }
    }
    public void AddItem(IInventoryItem item)
    {
        if (loot == true)
        {
            GameObject newItem = PhotonNetwork.Instantiate(item.Name, gameObject.transform.position, gameObject.transform.rotation);
            if (mItems == null)
                mItems = new IInventoryItem[9];
            Weapon weapon = newItem.GetComponent<Weapon>();
            Add(weapon);
            if (ItemAdded != null)
            {
                ItemAdded(this, new InventoryEventArgs(weapon));
            }
            Destroy(newItem);
            return;
        }
        if (Count() == 0)
        {
            mItems = new IInventoryItem[9];
            Add(item);
            Print_Inventory();
            //item.OnPickup();
            if (ItemAdded != null)
            {
                ItemAdded(this, new InventoryEventArgs(item));
            }
            return;
        }
        if (Count() < SLOTS)
        {
            Add(item);
            //item.OnPickup();
            if (ItemAdded != null)
            {
                ItemAdded(this, new InventoryEventArgs(item));
            }
        }
    }

    public void Update()
    {
        if (Count() == 0 && loot == true)
        {
            Debug.Log("INVENTORY/ IN DESTROY LOOT");
            //mItems.Clear();
            //GameObject player = GameObject.FindGameObjectWithTag("Player");
            //player.GetComponent<Player>().DeactivateLoot();
            view = gameObject.GetComponent<PhotonView>();
            view.RPC("DestroyObject", RpcTarget.All);
        }
    }

    public void DisplayLoot(Inventory playerInventory)
    {
        for (int i = 0; i < 9; i++)
        {
            if (mItems[i] != null)
            {
                if (loot == true)
                {
                    Debug.Log("ITEM IN LOOT id = " + i + " name = " + mItems[i].Name);
                }
                ItemInsertedAt(this, new InventoryEventArgs(mItems[i], i));
            }
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        /*if (loot == true)
        {
            Inventory playerInventory = collision.collider.GetComponent<Inventory>();
            if (playerInventory != null)
            {
                //Debug.Log("in if 2 inventory");
                if (Input.GetKeyDown(KeyCode.E))
                {
                    //Debug.Log("in if 3 inventory");
                    DisplayLoot(playerInventory);
                }
            }
        }*/
    }

    [PunRPC]
    public void UpdateItems(string name)
    {
        if (loot)
        {
            GameObject weaponPrefab = GameObject.FindGameObjectWithTag("TempObjTag");
            if (weaponPrefab == null)
            {
                Debug.Log("INVENTORY: Weapon Prefab == null");
                return;
            }
            //Destroy(weaponPrefab);
            if (weaponPrefab.TryGetComponent(out Weapon weapon))
            {
                if (mItems == null)
                    mItems = new IInventoryItem[9];
                Add(weapon);
                ItemAdded?.Invoke(this, new InventoryEventArgs(weapon));
                weaponPrefab.SetActive(false);
                //Destroy(weaponPrefab);
                //weapon.DeactivateAllObjects();
            }
            else
            {
                Debug.LogWarning("New item does not have a Weapon component.");
            }

            Debug.Log("IN UPDATE ITEMS: items count = " + Count());
            //PhotonNetwork.Destroy(newItem);
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(LastItemName);
        }
        else
        {
            LastItemName = (string)stream.ReceiveNext();
        }
    }

    public void ApplyNetworkUpdate(string name)
    {
        Debug.Log(gameObject);
        Debug.Log(gameObject.GetComponent<PhotonView>());
        view = gameObject.GetComponent<PhotonView>();
        if (view.IsMine)
        {
            view.RPC("UpdateItems", RpcTarget.All, name);
        }
    }
    public void DropItem(int id)
    {
        GameObject LootPrefab = Resources.Load<GameObject>("Prefabs/Loot");
        Vector3 newPos = gameObject.transform.position;
        newPos.x += 2;
        GameObject loot = PhotonNetwork.Instantiate("Prefabs/Loot", newPos, gameObject.transform.rotation);
        loot.GetComponentInChildren<Inventory>().loot = true;
        Inventory lootInventory = loot.GetComponentInChildren<Inventory>();
        //loot.GetComponentInChildren<Inventory>().AddItem(mItems[id]);
        Debug.Log("photon view state = " + loot.GetComponentInChildren<PhotonView>().ViewID);
        LastItemName = mItems[id].Name;
        GameObject newItem = PhotonNetwork.Instantiate(mItems[id].Name, transform.position, transform.rotation);
        newItem.GetComponent<Weapon>().RequestTagChange("TempObjTag");
        lootInventory.ApplyNetworkUpdate("TempObjTag");
        RemoveAt(id);
    }

    [PunRPC]
    public void DestroyObject()
    {

        Debug.Log("INVENTORY: in DESTROY OBJECT via punRPC");
        PhotonNetwork.Destroy(this.gameObject);
    }
}
