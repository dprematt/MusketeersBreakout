using System.Collections.Generic;
using UnityEngine;
using System;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using PlayFab.CloudScriptModels;
public class Inventory : MonoBehaviour
{
    public int SLOTS;
    public IInventoryItem[] mItems;
    public bool loot = false;
    public bool test = false;

    public event EventHandler<InventoryEventArgs> ItemAdded;
    public event EventHandler<InventoryEventArgs> ItemInsertedAt;
    public event EventHandler<InventoryEventArgs> ItemRemoved;
    public void AddWeapon(string weaponName)
    {
        GameObject weaponPrefab = Resources.Load<GameObject>(weaponName);
        if (weaponPrefab == null)
        {
            Debug.LogError("Weapon not found in folder Resources : " + weaponName);
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
            Debug.LogError("Weapon doesn't implement IInventoryItem interface: " + weaponName);
            Destroy(weaponObject);
            return;
        }
        Debug.Log("INVENTORY ADD WEAPON: item name = " + weaponItem.name);
        
        //AddItem(weaponItem);
        if (Count() == 0)
        {
            Debug.Log("CALL TO EQUI WEAPON FROM INVENTORY");
            playerScript.EquipWeapon(weaponItem, weaponObject, true);
        }
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
        Debug.Log("inventory ADD: in function Add item name = " + item.Name);
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
        Debug.Log("SWAP ITEMS");
        IInventoryItem temp = mItems[index1];
        InsertAt(mItems[index2], index1);
        InsertAt(temp, index2);
    }
    public void SwapItemsLoot(int index1, int index2, Inventory lootInventory)
    {
        Debug.Log("SWAP ITEMS LOOT: item 1 id = " + index1 + "item 2 id = " + index2);
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
        Debug.Log("inventory: in function AddItem item name = " + item.Name);
        if (Count() == 0)
        {
            Debug.Log("inventory: AddItem count == 0");
            mItems = new IInventoryItem[9];
            Debug.Log("on call l'event add");
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
            Debug.Log("ADD ITEM: item count = " + Count());
            Add(item);
            //item.OnPickup();
            ItemAdded(this, new InventoryEventArgs(item));
        }
    }

    public void Update()
    {
        if (Count() == 0 && loot == true)
        {
            //mItems.Clear();
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<Player>().DeactivateLoot();
            Destroy(gameObject);
        }
    }

    public void DisplayLoot(Inventory playerInventory)
    {
        Debug.Log("DISPLAY LOOT");
        Debug.Log(Count());
        for (int i = 0; i < 9; i++)
        {
            if (mItems[i] != null)
            {
                Debug.Log(mItems[i].Name);
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
    public void DropItem(int id)
    {
        GameObject LootPrefab = Resources.Load<GameObject>("Prefabs/Loot");
        Vector3 newPos = gameObject.transform.position;
        newPos.x += 2;
        var loot = Instantiate(LootPrefab, newPos, gameObject.transform.rotation);
        loot.GetComponentInChildren<Inventory>().loot = true;
        Debug.Log("INVENTORY DROP ITEM: print inventory call");
        Print_Inventory();
        loot.GetComponentInChildren<Inventory>().AddItem(mItems[id]);
        Debug.Log("after add item in drop item");
        RemoveAt(id);
        Debug.Log(Count());
    }
}
