using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IInventoryItem
{
    string Name { get; }
    Sprite Image { get; }
    bool IsPlayer { get; }

    bool SetIsPlayer(bool type = true);
    void OnPickup();
    void SelectItem(bool state);
    void Attack();
}
public class InventoryEventArgs : EventArgs
{
    public InventoryEventArgs(IInventoryItem item)
    {
        Item = item;
    }

    public IInventoryItem Item;
}