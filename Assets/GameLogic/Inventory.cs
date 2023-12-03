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

    private void DisplayLoot(Inventory playerInventory)
    {
        GameObject lootHUD = Resources.Load<GameObject>("Prefabs/LootHUD");
        lootHUD.GetComponent<LootHUD>().InventoryFill(mItems);
        var hud = Instantiate(lootHUD);
        if (Input.GetKeyDown(KeyCode.E))
        {
            Destroy(lootHUD);
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (loot == true)
        {
            Inventory playerInventory = collision.collider.GetComponent<Inventory>();
            if (playerInventory != null)
            {
                if (Input.GetKeyDown(KeyCode.E))
                    {
                    DisplayLoot(playerInventory);
                }
            }
        }
    }
}
