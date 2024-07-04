using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener : MonoBehaviour
{
    public Weapon weaponComp;

    void Start()
    {
    }

    public void AnimEvent()
    {
        if (weaponComp != null)
            weaponComp.CheckAttackPhase();
    }

    public void HitEvent()
    {
        if (weaponComp != null)
            weaponComp.IsAttacking = true;
    }
}
