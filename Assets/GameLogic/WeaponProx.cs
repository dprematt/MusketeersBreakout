using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProx : MonoBehaviour
{
    private Transform weaponSpawnPoint;
    private float Range;
    private float Damage;
    private Rigidbody rb;

    public void Initialize(Transform spawnPoint, float range, float damage)
    {
        weaponSpawnPoint = spawnPoint;
        Range = range;
        Damage = damage;
        gameObject.transform.localScale += new Vector3(0, 0, range);
    }

    void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.TryGetComponent<Enemy>(out Enemy enemyComponent))
        {
            enemyComponent.TakeDamage(Damage);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
