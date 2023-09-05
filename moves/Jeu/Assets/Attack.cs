using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Attack : MonoBehaviour
{
   
   public Gun g;
  
    // Start is called before the first frame update
    public void Start()
    { 
        g = new Gun();
    }

    
    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) {
            g.SpawnBullet();
            //g.updateLifeTime(g.Lifetime--);
        }
    }
}
