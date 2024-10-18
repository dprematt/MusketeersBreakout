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

    public void AddEnemyWeapon(Weapon weapon, GameObject enemy)
    {
        if (enemy != null)
        {
            Player player = enemy.GetComponent<Player>();
            if (player == null)
                return;
        }
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

    public void EquipMainWeapon(string weaponName)
    {
        Debug.Log("EquipMainWeapon func");
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
        Destroy(weaponPrefab);
        if (weaponItem == null)
        {
            Destroy(weaponObject);
            return;
        }

        Debug.Log("call to equipMainWeapon end and call to equipWeapon in player");
        playerScript.SetWeaponEvents(weaponItem);
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
        Debug.Log("CheckOldWeapon");
        Player player = gameObject.GetComponent<Player>();

        Transform weapon = player.FindDeepChild(gameObject.transform, name);
        if (weapon == null)
        {
            Debug.Log("CheckOldWeapon: weapon == null");
            return false;
        }
        Debug.Log("CheckOldWeapon: transform = " + weapon);
        Debug.Log("CheckOldWeapon: obj name = " + weapon.name);
        weapon.gameObject.SetActive(true);
        return true;
    }

    public void RemoveAnimR()
    {
        Debug.Log("RemoveAnimR");
        Player player = gameObject.GetComponent<Player>();

        Transform weapons = player.FindDeepChild(gameObject.transform, "jointItemR");
        if (weapons == null)
        {
            Debug.Log("RemoveAnim: weapons == null");
            return;
        }
        foreach (Transform child in weapons.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void RemoveAnimL()
    {
        Debug.Log("RemoveAnimL");
        Player player = gameObject.GetComponent<Player>();

        Transform weapons = player.FindDeepChild(gameObject.transform, "jointItemL");
        if (weapons == null)
        {
            Debug.Log("RemoveAnim: weapons == null");
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
            if (gameObject.GetComponent<Player>() != null)
            {
                if (id == 0 && item != null)
                {
                    Debug.Log("checkoldweapon = " + CheckOldWeapon(item.Name));
                    if (CheckOldWeapon(item.Name) == false)
                    {
                        ReleaseLeftArm();
                        RemoveAnimR();
                        RemoveAnimL();
                        Debug.Log("INSERT AT: " + item.GameObject);
                        EquipMainWeapon(item.Name);
                    }
                }
                else if (id == 0 && item == null)
                {
                    Debug.Log("INSERT AT: DESEQUIP WEAPON");
                    ReleaseLeftArm();
                    EquipMainWeapon(null);
                    RemoveAnimR();
                    RemoveAnimL();
                }
                else if (id != 0 && item == null)
                {
                    return;
                }
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
        Debug.Log("SwapItems");
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
        Debug.Log("SwapItemsLoot");
        Debug.Log("index 1 " + index1);
        Debug.Log("index 2 " + index2);
        Debug.Log("lootinventory " + lootInventory.transform.name);
        IInventoryItem temp = mItems[index1];
        Debug.Log("temp = " + temp.Name);
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
        Debug.Log("InsertItem");
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
            Debug.Log("call to equipMainWeapon");
            EquipMainWeapon(item.Name);
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
                Debug.Log("call to equipMainWeapon");
                EquipMainWeapon(item.Name);
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
            GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
            /*if ((playerGo != null) && (gameObject.GetComponent<PhotonView>().ViewID !=
                0))
            {
                Player player = playerGo.GetComponent<Player>();
                player.UpdateXp(5);
                player.CheckXp();
            }
            else
            {
                Debug.Log("player == null");
            }*/
            //mItems.Clear();
            //GameObject player = GameObject.FindGameObjectWithTag("Player");
            //player.GetComponent<Player>().DeactivateLoot();
            view = gameObject.GetComponent<PhotonView>();
            if (view != null)
            {
                if (view.ViewID == 0)
                    return;
                Debug.Log("before loot destroyed");
                view.RPC("DestroyObject", RpcTarget.All);
                Debug.Log("after loot destroyed");
            }
            else
            {
                Destroy(gameObject);
            }
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
                GameObject playerGo = GameObject.FindGameObjectWithTag("Player");

                if (playerGo == null)
                {
                    Debug.Log("player == null");
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
            Debug.Log("item is loot");
            GameObject weaponPrefab = GameObject.FindGameObjectWithTag("TempObjTag");
            if (weaponPrefab == null)
            {
                Debug.Log("INVENTORY: Weapon Prefab == null");
                return;
            }
            //Destroy(weaponPrefab);
            if (weaponPrefab.TryGetComponent(out Weapon weapon))
            {
                Debug.Log("weapon added");
                if (mItems == null)
                    mItems = new IInventoryItem[9];
                Add(weapon);
                ItemAdded?.Invoke(this, new InventoryEventArgs(weapon));
                Debug.Log("update items: weapon added");
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
            Debug.Log("apply network update: view is mine");
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
    
    public void DropWeapons(string name)
    {
        Debug.Log("DropTOTO 1");
        
        GameObject newItem = PhotonNetwork.Instantiate(name, transform.position, transform.rotation);
        // AddItem(newItem.GetComponent<Weapon>());
        Debug.Log("DropTOTO 2");

        // Debug.Log("photon view state = " + loot.GetComponentInChildren<PhotonView>().ViewID);

        newItem.GetComponent<Weapon>().RequestTagChange("TempObjTag");
        Debug.Log("DropTOTO 3");

        ApplyNetworkUpdate("TempObjTag");
        Debug.Log("DropTOTO 4");

    }

    [PunRPC]
    public void DestroyObject()
    {

        Debug.Log("INVENTORY: in DESTROY OBJECT via punRPC");
        PhotonNetwork.Destroy(this.gameObject);
    }
}