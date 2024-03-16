using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Bow : Weapons
{
    public Transform BulletSpawnPoint_;
    public GameObject BulletPrefab_;
    public float bulletSpeed = 10;

    public Sprite _Image = null;

    public AudioClip shootingSound;
    public AudioSource audioSource;

    public override Sprite Image
    {
        get { return _Image; }
    }

    private void Start()
    {
        LifeTime = 10;
        audioSource = GetComponent<AudioSource>();
    }

    public override void OnPickup()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        transform.parent = player.transform;
        transform.localPosition = new Vector3(0.3f, 1f, 0.0f);
        BulletSpawnPoint_ = player.transform;
        //Debug.Log("Bow onPickup");
        photonView.RPC("DisableObject", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void DisableObject()
    {
        gameObject.SetActive(false);
    }

    public override string Name
    {
        get { return "Bow"; }
    }

    public override void Attack()
    {
        audioSource.PlayOneShot(shootingSound);

        SpawnBullet();
        Debug.Log("Attack");
    }

    public void SpawnBullet()
    {
        if (LifeTime > 0)
        {
            float offset = 1f;
            Player pm = gameObject.GetComponentInParent<Player>();

            // Use the forward vector to determine the spawn position
            var NewPos = BulletSpawnPoint_.position + pm.characterModel.rotation * Vector3.forward * offset;
            NewPos.y += 1.5f;
            NewPos.z -= 1;
            Damage = 4;

            GameObject Bullet = PhotonNetwork.Instantiate(BulletPrefab_.name, NewPos, Quaternion.identity);
            //Damage = 2;
            //var bullet = Instantiate(bulletPrefab, NewPos, bulletSpawnPoint.rotation);
            //bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;
            //bullet.GetComponent<Bullet>().Initialize(bulletSpawnPoint, Range, Damage);
            //Debug.Log("spawn bullet end");
        }
    }


    public void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnBullet();
            if (UpdateLifeTime(LifeTime--))
                Destroy(gameObject); // destruction de l'arc si la durabilit? atteint 0;
        }*/
    }
}
