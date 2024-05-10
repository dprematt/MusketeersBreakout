using System.Collections.Generic;
using UnityEngine;
using System;
using PlayFab;
using PlayFab.ClientModels;
public class Inventory : MonoBehaviour
{
    public int SLOTS;
    public IInventoryItem[] mItems;
    public bool loot = false;
    public bool test = false;

    public event EventHandler<InventoryEventArgs> ItemAdded;
    public event EventHandler<InventoryEventArgs> ItemRemoved;
    public void AddWeapon(string weaponName)
    {
        GameObject weaponPrefab = Resources.Load<GameObject>(weaponName);
        if (weaponPrefab == null)
        {
            Debug.LogError("Weapon not found in folder Resources : " + weaponName);
            return;
        }
        GameObject weaponObject = Instantiate(weaponPrefab);
        IInventoryItem weaponItem = weaponObject.GetComponent<IInventoryItem>();
        Destroy(weaponObject);
        Destroy(weaponPrefab);
        if (weaponItem == null)
        {
            Debug.LogError("Weapon doesn't implement IInventoryItem interface: " + weaponName);
            Destroy(weaponObject);
            return;
        }
        AddItem(weaponItem);
    }

    public int Count()
    {
        int res = 0;

        for (int i = 0; i < mItems.Length; i++)
        {
            if (mItems[i] != null)
                res += 1;
        }
        return res;
    }

    public int Add(IInventoryItem item)
    {
        Debug.Log("inventory: in function Add item name = " + item.Name);
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
        //Debug.Log("in inventory start");
        if (mItems == null)
        {
            //Debug.Log("in inventory start item == null");
            mItems = new IInventoryItem[9];
        }
        if (loot == true)
        {
            if (Count() == 0)
            {
                GameObject go = Resources.Load<GameObject>("Prefabs/Sword");
                IInventoryItem item = go.GetComponent<IInventoryItem>();
                Add(item);
            }
        }
    }

    public IInventoryItem[] GetInventory()
    {
        return mItems;
    }

    public void SwapItems(int index1, int index2)
    {
        /*// Ensure that both indices are valid
        if (index1 < 0 || index1 >= Count() || index2 < 0 || index2 >= Count()
        {
            Debug.LogError("Invalid indices for swapping items.");
            return;
        }
        */
        // Swap the items at the specified indices

        IInventoryItem temp = mItems[index1];
        mItems[index1] = mItems[index2];
        mItems[index2] = temp;
        Print_Inventory();
    }
    public void SwapItemsLoot(int index1, int index2, Inventory lootInventory)
    {
        // Ensure that both indices are valid
        /*if (index2 >= lootInventory.mItems.Count && index2 <= 9)
        {
            lootInventory.AddItem(mItems[index1]);
            //Debug.LogError("Invalid indices for swapping items.");
            mItems.RemoveAt(index1);
            return;
        }*/

        // Swap the items at the specified indices
        IInventoryItem temp = mItems[index1];
        mItems[index1] = lootInventory.mItems[index2];
        lootInventory.mItems[index2] = temp;
        Print_Inventory();
    }
    public void Print_Inventory()
    {
        IInventoryItem[] Item = GetInventory();

        for (int i = 0; i < mItems.Length; i++)
        {
            if (mItems[i] != null)
                Debug.Log("Item in inventory : " + mItems[i].Name);
        }
        Debug.Log("end of print inventory");
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
            Destroy(gameObject);
        }
    }

    public void DisplayLoot(Inventory playerInventory)
    {
        foreach (IInventoryItem lootItem in mItems)
        {
            //   Debug.Log(lootItem.Name);
            ItemAdded(this, new InventoryEventArgs(lootItem));
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
        loot.GetComponentInChildren<Inventory>().AddItem(mItems[id]);
        Debug.Log("after add item in drop item");
        RemoveAt(id);
        Debug.Log(Count());
    }
}
