using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Gun : Weapons
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
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
        bulletSpawnPoint = player.transform;

        //gameObject.SetActive(false);
        photonView.RPC("DisableObject", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void DisableObject()
    {
        gameObject.SetActive(false);
    }
    
    public override string Name
    {
        get { return "Gun"; }
    }

    public override void Attack()
    {
        audioSource.PlayOneShot(shootingSound);

        SpawnBullet();
        Debug.Log("Attack");
    }

    bool IsLookingDownward(Quaternion characterRot, Vector3 forwardVector)
    {
        // Get the character's current rotation

        // Get the forward vector of the character
        forwardVector = characterRot * Vector3.forward;

        // Check if the y-component of the forward vector is negative
        return forwardVector.y < 0;
    }
    public void SpawnBullet()
    {
        if (LifeTime > 0) {
            float offset = 1f;
            Player pm = gameObject.GetComponentInParent<Player>();

            // Use the forward vector to determine the spawn position
            var NewPos = bulletSpawnPoint.position + pm.characterModel.rotation * Vector3.forward * offset;
            NewPos.y += 1.5f;
            NewPos.z -= 1;
            Damage = 6;
            var bullet = Instantiate(bulletPrefab, NewPos, pm.characterModel.rotation);
            bullet.GetComponent<Rigidbody>().velocity = pm.characterModel.forward * bulletSpeed;
            //bullet.GetComponent<Bullet>().Initialize(pm.characterModel, Range, Damage);
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
