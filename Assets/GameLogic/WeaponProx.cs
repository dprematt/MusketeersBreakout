    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProx : MonoBehaviour
{
    private Transform weaponSpawnPoint;
    private float Range;
    private float Damage;
    private Rigidbody rb;
    public bool isEnnemy;

    public void Initialize(Transform spawnPoint, float range, float damage, bool type = false)
    {
        weaponSpawnPoint = spawnPoint;
        weaponSpawnPoint.localPosition += new Vector3(0, 0, -1);
        Range = range;
        Damage = damage;
        gameObject.transform.localScale += new Vector3(0, 0, range);
        isEnnemy = type;
        Debug.Log("weap prox init");
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("in weapon prox colision");
        if (col.gameObject.TryGetComponent(out Enemy enemyComponent) && isEnnemy != true)
        {
            enemyComponent.TakeDamage(Damage);
            Destroy(gameObject);
        }
        else if (col.gameObject.TryGetComponent(out EnemySquare enemyComponentSquare) && isEnnemy != true)
        {
            enemyComponentSquare.TakeDamage(Damage);
            Destroy(gameObject);
        }
        else if (col.gameObject.TryGetComponent(out EnemyShape enemyComponentShape) && isEnnemy != true)
        {
            enemyComponentShape.TakeDamage(Damage);
            Destroy(gameObject);
        }
        if (col.gameObject.TryGetComponent(out Player player))
        {
            Debug.Log("PLAYER TAKE DMG !!!");
            player.TakeDamage(Damage);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
