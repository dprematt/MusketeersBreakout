using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public Sword s;
    // Start is called before the first frame update
    void Start()
    {
        s = new Sword();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
           s.SpawnWeaponProx();
        }
    }
}
