using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Photon.Pun;

public interface IInventoryItem
{
    string Name { get; }
    Sprite Image { get; }
    bool IsPlayer { get; }
    GameObject GameObject { get; }

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

    public InventoryEventArgs(IInventoryItem item, int index)
    {
        Index = index;
        Item = item;
    }

    public InventoryEventArgs(int index)
    {
        Index = index;
    }

    public IInventoryItem Item;
    public int Index;
}