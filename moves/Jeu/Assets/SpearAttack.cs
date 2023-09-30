using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpearAttack : MonoBehaviour
{

    public Spear s;

    // Start is called before the first frame update
    public void Start()
    {
        s = new Spear();
    }


    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            s.SpawnBullet();
            s.updateLifeTime(s.Lifetime--);
        }
    }
}
