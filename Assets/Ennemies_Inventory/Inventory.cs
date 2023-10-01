using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<string> items;
    private int inventoryLimit = 10;
    private bool isInventoryOpen = false;


    private void Start()
    {
        items = new List<string>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (isInventoryOpen)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            CloseInventory();
        }
    }

    public void OpenInventory()
    {
        isInventoryOpen = true;
        Debug.Log("Inventory opened.");
        // Add any additional logic or UI changes to show the inventory
    }

    public void CloseInventory()
    {
        isInventoryOpen = false;
        Debug.Log("Inventory closed.");
        // Add any additional logic or UI changes to hide the inventory
    }
    public void AddItem(string item)
    {
        if (items.Count >= inventoryLimit)
        {
            Debug.Log("The inventory is full. Unable to add a new item.");
            return;
        }

        items.Add(item);
        Debug.Log("New item added to the inventory: " + item);
    }

    public void RemoveItem(string item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            Debug.Log("Item removed from the inventory: " + item);
        }
        else
        {
            Debug.Log("The item does not exist in the inventory.");
        }
    }

    public void DisplayInventory()
    {
        Debug.Log("Inventory:");
        foreach (string item in items)
        {
            Debug.Log("- " + item);
        }
    }
}
