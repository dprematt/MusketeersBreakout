using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpearAttack : MonoBehaviour
{

    public Spear s;
    //public CrossBow c;
    //public Gun g;

    // Start is called before the first frame update
    public void Start()
    {
        //c = new CrossBow();
        s = new Spear();
        //g = new Gun();
    }


    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            //      g.SpawnBullet();
            s.SpawnBullet();
            //c.SpawnBullet();
            //c.updateLifeTime(c.Lifetime--);
            //    g.updateLifeTime(g.Lifetime--);
            s.updateLifeTime(s.Lifetime--);
        }
    }
}
