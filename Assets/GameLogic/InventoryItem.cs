using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IInventoryItem
{
    // Start is called before the first frame update
    string Name { get; }
    Sprite Image { get; }
    float Damage { get; }
    float ReloadingTime { get; }
    float XpLimit { get; } // limite d'xp d'un item avant de level up
    float Xp { get; } // xp actuel 
    int Level { get; } // niveau actuel
    float LifeTime { get; } // durée de vie en cours
    Vector3 ImpactZone { get; } // zone d'impact si collision

    int Mag { get; } // nombre de munitions dans le chargeur
    bool Echo { get; } // echo actif ou non de l'arme
    int Range { get; } // range de l'arme
    int UpgradeLevel(int levels);
    int UpdateMag(int bullet = 1);
    int UpdateXp(float xp);
    bool UpdateLifeTime(float lifetime);
    void OnPickup();
}
public class InventoryEventArgs : EventArgs
{
    public InventoryEventArgs(IInventoryItem item)
    {
        Item = item;
    }

    public IInventoryItem Item;
 }