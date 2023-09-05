using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Gun
{
    void OnCollisionEnter(Collision col) 
        {
            if (col.gameObject.TryGetComponent<Enemy>(out Enemy enemyComponent))
                {
                    enemyComponent.TakeDamage(Damage);
                    Destroy(gameObject);
                }
        }
}
