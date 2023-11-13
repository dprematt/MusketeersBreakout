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
        Debug.Log("Check bullet range");
        /*PosInit = bulletSpawnPoint.transform.position.z;
        PosActual = gameObject.transform.position.z;
        if (PosActual <= PosInit + Range) {
                return;
            } else
                Destroy(gameObject);*/
    }
    void OnCollisionEnter(Collision col) 
    {
            
        if (col.gameObject.TryGetComponent(out Enemy enemyComponent))
            {
                enemyComponent.TakeDamage(Damage);
                Destroy(gameObject);
            }
        else if (col.gameObject.TryGetComponent(out EnemySquare enemyComponentSquare))
            {
                enemyComponentSquare.TakeDamage(Damage);
                Destroy(gameObject);
        }
        else if (col.gameObject.TryGetComponent(out EnemyShape enemyComponentCircle))
            {
                enemyComponentCircle.TakeDamage(Damage);
                Destroy(gameObject);
        }
    }

}
