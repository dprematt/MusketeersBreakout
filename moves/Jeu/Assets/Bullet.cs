using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Gun
{
    void OnCollisionEnter(Collision col) 
        {
            if (posActual <= posInit + Range){
            if (col.gameObject.TryGetComponent<Enemy>(out Enemy enemyComponent))
                {
                    enemyComponent.TakeDamage(Damage);
                    Destroy(gameObject);
                }
            } else
                Destroy(gameObject);
        }
}
