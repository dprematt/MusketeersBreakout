using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProx : MonoBehaviour
{
    private Transform weaponSpawnPoint;
    private float Range;
    private float Damage;

    public void Initialize(Transform spawnPoint, float range, float damage)
    {
        weaponSpawnPoint = spawnPoint;
        Range = range;
        Damage = damage;
        weaponSpawnPoint.GetComponent<Rigidbody>().x = range;
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
