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
        if (mItems == null)
            mItems = new IInventoryItem[9];
        Add(weapon);
        if (ItemAdded != null)
        {
            ItemAdded(this, new InventoryEventArgs(weapon));
        }  
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

        GameObject playerObj = gameObject;
        Player playerScript = playerObj.GetComponent<Player>();
        GameObject weaponObject = PhotonNetwork.Instantiate(weaponPrefab.name, pos, Quaternion.identity);
        Weapon weaponItem = weaponObject.GetComponent<Weapon>();
        //Destroy(weaponObject);
        if (weaponItem == null)
        {
            Destroy(weaponObject);
            return;
        }

        //AddItem(weaponItem);
        playerScript.EquipWeapon(weaponItem, weaponObject, true);
    }

    public void EquipMainWeapon(string weaponName, int index)
    {
        if (weaponName == null)
            return;

        GameObject weaponPrefab = Resources.Load<GameObject>(weaponName);
        if (weaponPrefab == null)
        {
            return;
        }

        Vector3 pos;
        pos.z = 0;
        pos.y = 0;
        pos.x = 0;

        Player playerScript = transform.GetComponentInParent<Player>();
        GameObject weaponObject = PhotonNetwork.Instantiate(weaponPrefab.name, pos, Quaternion.identity);
        Weapon weaponItem = weaponObject.GetComponent<Weapon>();

        //Destroy(weaponObject);

        if (weaponItem == null)
        {
            Destroy(weaponObject);
            return;
        }

        if (index == 0)
        {
            playerScript.SetWeaponEvents(weaponItem);
            weaponObject.SetActive(true);
        }
        else
        {
            weaponObject.SetActive(false);
        }
        weaponItem.whenPickUp(playerScript.gameObject);
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

    public bool CheckOldWeapon(string name)
    {
        Player player = gameObject.GetComponent<Player>();

        Transform weapon = player.FindDeepChild(gameObject.transform, name);
        if (weapon == null)
        {
            return false;
        }
        weapon.gameObject.SetActive(true);
        return true;
    }

    public void RemoveAnimR()
    {
        Player player = gameObject.GetComponent<Player>();

        Transform weapons = player.FindDeepChild(gameObject.transform, "jointItemR");
        if (weapons == null)
        {
            return;
        }
        foreach (Transform child in weapons.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void RemoveAnimL()
    {
        Player player = gameObject.GetComponent<Player>();

        Transform weapons = player.FindDeepChild(gameObject.transform, "jointItemL");
        if (weapons == null)
        {
            return;
        }
        foreach (Transform child in weapons.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void ReleaseLeftArm()
    {
        Player player = gameObject.GetComponent<Player>();

        if (player != null)
        {
            Weapon mainWeapon = player.RetrieveEquippedWeapon("jointItemL");
            if (mainWeapon != null)
            {
                mainWeapon.resetAnim(player.gameObject);
            }
        }
    }

    public void InsertAt(IInventoryItem item, int id)
    {
        mItems[id] = item;
        if (!loot)
        {
            if (gameObject.GetComponent<Player>() != null && item != null)
            {
                Player playerScript = transform.GetComponentInParent<Player>();
                GameObject weaponObject = item.GameObject;
                Weapon weaponItem = weaponObject.GetComponent<Weapon>();

                if (weaponItem == null)
                {
                    Destroy(weaponObject);
                    return;
                }

                if (id == 0)
                {
                    playerScript.SetWeaponEvents(weaponItem);
                    UpdateActiveWeapon(weaponObject, true);
                }
                else
                {
                    UpdateActiveWeapon(weaponObject, false);
                }
                weaponItem.whenPickUp(playerScript.gameObject);
            }
        }
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
        if (items == null)
            Debug.Log("in init, items == null");
        SLOTS = slots;
        mItems = items;
        loot = isLoot;
        DisplayCurrentLoot();
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
        if (index1 == 0 && index2 == 1)
        {
            IInventoryItem item1 = mItems[index1];
            if (item1 != null && item1.GameObject != null)
            {
                UpdateActiveWeapon(item1.GameObject, false);
            }

            IInventoryItem item2 = mItems[index2];
            if (item2 != null && item2.GameObject != null)
            {
                UpdateActiveWeapon(item2.GameObject, true);
            }
        }

        IInventoryItem temp = mItems[index1];
        mItems[index1] = mItems[index2];
        mItems[index2] = temp;
    }

    public void SwapItemsLoot(int index1, int index2, Inventory lootInventory)
    {
        IInventoryItem temp = mItems[index1];
        InsertAt(lootInventory.mItems[index2], index1);
        lootInventory.InsertAt(temp, index2);
    }

    public void Print_Inventory()
    {
        /*IInventoryItem[] Item = GetInventory();

        for (int i = 0; i < mItems.Length; i++)
        {
            if (mItems[i] != null)
            else
            {
            }
        }*/
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
        if (!loot && index == 0)
        {
            EquipMainWeapon(item.Name, index);
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
            if (!loot)
            {
                EquipMainWeapon(item.Name, 0);
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
            view = gameObject.GetComponent<PhotonView>();
            if (view != null)
            {
                if (view.ViewID == 0)
                {
                    Debug.Log("destroy loot, view id == 0");
                    Debug.Log("other view id == " + gameObject.GetComponentInChildren<PhotonView>().ViewID);
                    return;
                }
                view.RPC("DestroyObject", RpcTarget.All);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void DisplayCurrentLoot()
    {
        if (mItems == null)
        {
            return;
        }
        for (int i = 0; i < 9; i++)
        {
            if (mItems[i] != null)
            {
                if (loot == true)
                {
                    Debug.Log("ITEM IN LOOT id = " + i + " name = " + mItems[i].Name);
                }
            }
        }
    }

    public void DisplayLoot(Inventory playerInventory)
    {
        for (int i = 0; i < 9; i++)
        {
            if (mItems[i] != null)
            {
                ItemInsertedAt(this, new InventoryEventArgs(mItems[i], i));
                GameObject playerGo = gameObject;

                if (playerGo == null)
                {
                    return;
                }

                Player player = playerGo.GetComponent<Player>();
                player.lootedChests += 1;
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
                if (Input.GetKeyDown(KeyCode.E))
                {
                    DisplayLoot(playerInventory);
                }
            }
        }*/
    }

    public void UpdateActiveWeapon(GameObject item, bool state)
    {
        PhotonView view = item.GetComponent<PhotonView>();
        photonView.RPC("SetActiveWeapon", RpcTarget.All, view.ViewID, state);             
    }

    [PunRPC]
    public void SetActiveWeapon(int viewID, bool state)
    {
        PhotonView view = PhotonView.Find(viewID);
        view.gameObject.SetActive(state);
    }

    [PunRPC]
    public void UpdateItems(string name)
    {
        if (loot)
        {
            GameObject weaponPrefab = GameObject.FindGameObjectWithTag("TempObjTag");
            if (weaponPrefab == null)
            {
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
        LastItemName = mItems[id].Name;
        GameObject newItem = PhotonNetwork.Instantiate(mItems[id].Name, transform.position, transform.rotation);
        newItem.GetComponent<Weapon>().RequestTagChange("TempObjTag");
        lootInventory.ApplyNetworkUpdate("TempObjTag");
        RemoveAt(id);
    }
    
    public void DropWeapons(string name)
    {
        GameObject newItem = PhotonNetwork.Instantiate(name, transform.position, transform.rotation);
        // AddItem(newItem.GetComponent<Weapon>());

        newItem.GetComponent<Weapon>().RequestTagChange("TempObjTag");

        ApplyNetworkUpdate("TempObjTag");
    }

    [PunRPC]
    public void DestroyObject()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }
}