using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeAttack : MonoBehaviour
{

    public Knife k;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
           k.SpawnWeaponProx();
        }
    }
}
