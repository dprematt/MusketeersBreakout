using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CrossBowAttack : MonoBehaviour
{
    public CrossBow c;

    // Start is called before the first frame update
    public void Start()
    {
    }


    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            c.SpawnBullet();
            c.updateLifeTime(c.Lifetime--);
         }
    }
}
