using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    private Transform bulletSpawnPoint;

    private float Range;
    private float Damage = 10;
    private float PosActual;
    private float PosInit;



    //IEnumerator DestroyBullet()
    //{
    //    yield return new WaitForSeconds(2f);
    //    this.GetComponent<PhotonView>().RPC("Destroy", RpcTarget.AllBuffered);
    //}

    //private void Start()
    //{
    //    StartCoroutine(DestroyBullet());
    //}

    public void Initialize(Transform spawnPoint, float range, float damage)
    {
        bulletSpawnPoint = spawnPoint;
        Range = range;
        Damage = damage;
    }
    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * 10f);
    }

    [PunRPC]
    public void Destroy()
    {
        Destroy(this.gameObject);

    }

    void checkRange()
    {
        //Debug.Log("Check bullet range");
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
        else if (col.gameObject.TryGetComponent(out PlayerMove playerComponent))
        {
            playerComponent.TakeDamage(Damage);
            Debug.Log("Collide Call InflictDamage");
            this.GetComponent<PhotonView>().RPC("Destroy", RpcTarget.AllBuffered);
        }
    }
}