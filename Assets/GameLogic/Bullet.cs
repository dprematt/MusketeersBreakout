using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform bulletSpawnPoint;
    private float Range;
    private float Damage;
    private float PosActual;
    private float PosInit;


    public void Initialize(Transform spawnPoint, float range, float damage)
    {
        bulletSpawnPoint = spawnPoint;
        Range = range;
        Damage = damage;
    }
    void Update()
    {
        checkRange();
    }
    
    void checkRange()
    {
        PosInit = bulletSpawnPoint.transform.position.z;
        PosActual = gameObject.transform.position.z;
        if (PosActual <= PosInit + Range) {
                return;
            } else
                Destroy(gameObject);
    }
    void OnCollisionEnter(Collision col) 
    {
            
        if (col.gameObject.TryGetComponent<Enemy>(out Enemy enemyComponent))
            {
                enemyComponent.TakeDamage(Damage);
                Destroy(gameObject);
            }
    }

}
