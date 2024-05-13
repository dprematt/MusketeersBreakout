using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Weapons : MonoBehaviourPun, IInventoryItem
{
    public virtual string Name { get; protected set; }
    public virtual Sprite Image { get; protected set; }
    public virtual float Damage { get; protected set; }
    public virtual float ReloadingTime { get; protected set; }
    public virtual float XpLimit { get; protected set; }
    public virtual float Xp { get; protected set; }
    public virtual int Level { get; protected set; }
    public virtual float LifeTime { get; protected set; }
    public virtual Vector3 ImpactZone { get; protected set; }
    public virtual int Mag { get; protected set; }
    public virtual bool Echo { get; protected set; }
    public virtual int Range { get; protected set; }
    public virtual bool IsPlayer { get; set; }

    public virtual bool SetIsPlayer(bool type)
    {
        IsPlayer = type;
        return IsPlayer;
    }
    public virtual void Attack()
    {
    }

    public GameObject GameObject
    {
        get { return gameObject; }
    }
    public virtual int UpgradeLevel(int levels)
    {
        Level += levels;
        return Level;
    }

    public virtual void SelectItem(bool state)
    {
        gameObject.SetActive(state);
    }
    public virtual int UpdateMag(int mag)
    {
        Mag -= mag;
        return Mag;
    }

    public virtual int UpdateXp(float new_xp)
    {
        if (Level < 100)
        {
            float buffer = Xp + new_xp;
            if (buffer >= XpLimit)
            {
                UpgradeLevel(1);
                if (new_xp - XpLimit >= XpLimit)
                {
                    Xp = 0;
                    UpdateXp(new_xp - XpLimit);
                }
                else
                {
                    Xp = buffer - XpLimit;
                }
            }
            else
            {
                Xp += new_xp;
            }
        }
        return Level;
    }

    public virtual bool UpdateLifeTime(float lifetime)
    {
        LifeTime -= lifetime;
        return LifeTime < 0;
    }
    public virtual void OnPickup()
    {

    }
}