using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float healt, maxHealt = 10f;
    // Start is called before the first frame update
    private void Start()
    {
        healt = maxHealt;
    }

    public void TakeDamage(float damageAmount) 
    {
        healt -= damageAmount;

        if(healt <= 0)
        {
            Destroy(gameObject);
        }
    }
}
