using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : Weapons
{
    public Transform weaponSpawnPoint;
    public GameObject weaponPrefab;

    public Sprite _Image = null;

    public AudioClip shootingSound;
    private AudioSource audioSource;

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
        weaponSpawnPoint = player.transform;
        //gameObject.SetActive(false);
    }
    public override string Name
    {
        get { return "Dagger"; }
    }
    public void SpawnWeaponProx()
    { 
        var NewPos = weaponSpawnPoint.position;
        NewPos.z += 1/2;
        NewPos.y += 1;
        var dagger = Instantiate(weaponPrefab, NewPos, weaponSpawnPoint.rotation);
        dagger.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 1, 3);
    }

    public override void Attack()
    {
        audioSource.PlayOneShot(shootingSound);

        SpawnWeaponProx();
    }
    public void Update()
    {
       /* if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnWeaponProx();
            if (UpdateLifeTime(LifeTime--))
                Destroy(gameObject); // destruction de l'arme si la durabilité atteint 0;
        }*/
    }
}
