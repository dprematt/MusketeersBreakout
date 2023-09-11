using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackBow : MonoBehaviour
{
   
   public Bow b;
  
    // Start is called before the first frame update
    public void Start()
    { 
        b = new Bow();
    }

    
    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) {
            b.SpawnBullet();
            b.updateLifeTime(b.Lifetime--);
        }
    }
}
