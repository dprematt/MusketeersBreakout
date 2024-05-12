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
        weaponComp.CheckAttackPhase();
    }

    public void HitEvent()
    {
        weaponComp.IsAttacking = true;
    }
}
