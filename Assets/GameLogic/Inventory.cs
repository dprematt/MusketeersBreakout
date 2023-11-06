using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour
{
    private int SLOTS;
    private List<IInventoryItem> mItems;

    public event EventHandler<InventoryEventArgs> ItemAdded;
    public Inventory(int slots) 
        {
        SLOTS = slots;
        }
    public void Start()
    {
        Debug.Log("in inventory start");
        mItems = new List<IInventoryItem>();
        SLOTS = 9;
    }
    public void AddItem(IInventoryItem item)
    {
        Debug.Log("in add item");
        Debug.Log(mItems.Count);
        if (mItems.Count < SLOTS)
        {
            Debug.Log("in add item 2");
            Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
            if (collider.enabled)
            {
                Debug.Log("in inventory add item -> collider found!");
                collider.enabled = false;
                mItems.Add(item);
                item.OnPickup();

                //if (ItemAdded != null)
                //{
                Debug.Log("Event created");
                ItemAdded(this, new InventoryEventArgs(item));
                //}
            }
        }
    }
}
