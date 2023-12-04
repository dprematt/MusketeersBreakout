using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour
{
    public int SLOTS;
    public List<IInventoryItem> mItems;
    public bool loot = false;

    public event EventHandler<InventoryEventArgs> ItemAdded;
    public void Initialize(int slots, List<IInventoryItem> items, bool isLoot)
    {
        SLOTS = slots;
        mItems = items;
        loot = isLoot;
    }

    public Inventory(int slots, List<IInventoryItem> items, bool isLoot)
    {
        SLOTS = slots;
        mItems = items;
        loot = isLoot;
    }
    public void Start()
    {
        Debug.Log("in inventory start");
        if (mItems == null)
        {
            Debug.Log("in inventory start item == null");
            mItems = new List<IInventoryItem>();
            SLOTS = 9;
        }
    }
    public void AddItem(IInventoryItem item)
    {
        // Debug.Log("in add item");
        Debug.Log(mItems.Count);
        if (mItems.Count < SLOTS)
        {
            // Debug.Log("in add item 2");
            //Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
            //if (collider.enabled)
            //{
            Debug.Log("in inventory add item -> collider found!");
            //  collider.enabled = false;
            Debug.Log(item.Name);
            mItems.Add(item);
            item.OnPickup();

            //if (ItemAdded != null)
            //{
            Debug.Log("Event created");
            ItemAdded(this, new InventoryEventArgs(item));
            //}
            //}
        }
    }

    public void Update()
    {
        if (loot == true)
        {
            Debug.Log("Loot update & count !");
            Debug.Log("is a loot = " + loot);
            Debug.Log("how many slots = " + SLOTS);
            Debug.Log("how many items = " + mItems.Count);
        }
        if (mItems.Count == 0 && loot == true)
        {
            Debug.Log("Delete loot");
            Destroy(gameObject);
        }
    }

    public void DisplayLoot(Inventory playerInventory)
    {
        Debug.Log("in display loot");
        Debug.Log(mItems.Count);
        int free_slots = 9 - playerInventory.mItems.Count;
        foreach (IInventoryItem lootItem in mItems) 
        {
            if (free_slots == 0)
                return;
            Debug.Log(lootItem.Name);
            if (lootItem.Name == "Halberd")
            {
                GameObject go = Resources.Load<GameObject>("Prefabs/Halberd");
                var halberd = Instantiate(go, playerInventory.gameObject.transform.position,
                    playerInventory.gameObject.transform.rotation);
                halberd.GetComponent<Halberd>().buildHalberd();
                IInventoryItem newHalberd = go.GetComponent<IInventoryItem>();
                playerInventory.AddItem(newHalberd);
            }
            else if (lootItem.Name == "Sword")
            {
                Debug.Log("sword creation");
                GameObject go = Resources.Load<GameObject>("Prefabs/Sword");
                var sword = Instantiate(go, playerInventory.gameObject.transform.position,
                    playerInventory.gameObject.transform.rotation);
                sword.GetComponent<Sword>().buildSword();
                IInventoryItem newSword = go.GetComponent<IInventoryItem>();
                playerInventory.AddItem(newSword);
            }
            Debug.Log("item added");
            mItems.Remove(lootItem);
            Debug.Log(mItems.Count);
            if (mItems.Count == 0)
                Destroy(gameObject);
            free_slots--;
        }
    }
        public void OnCollisionEnter(Collision collision)
        {
        if (loot == true)
        {
            Inventory playerInventory = collision.collider.GetComponent<Inventory>();
            if (playerInventory != null)
            {
                Debug.Log("in if 2 inventory");
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("in if 3 inventory");
                    DisplayLoot(playerInventory);
                }
            }
        }
    }
}
