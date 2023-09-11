using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float health, maxHealth = 10f;
    // Start is called before the first frame update
    private void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float damageAmount) 
    {
        health -= damageAmount;

        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
