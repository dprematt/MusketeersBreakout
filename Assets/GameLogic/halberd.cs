using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Halberd : Weapons
{
    public Transform weaponSpawnPoint;
    public GameObject weaponPrefab;

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
    public void buildHalberd()
    {
        _Image = Resources.Load("Sprites/halberd") as Sprite;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        weaponSpawnPoint = player.gameObject.transform;
        weaponPrefab = Resources.Load("Prefabs/WeaponProx") as GameObject;
        IsPlayer = true;
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
        get { return "Halberd"; }
    }

    public void SpawnEnemyWeaponProx()
    {
        //float offset = 2.8f;
        //GameObject pm = gameObject;
        //Debug.Log(gameObject);

        Debug.Log("ATTACK in enemy ! 4");
        //audioSource.PlayOneShot(shootingSound);
        // Use the forward vector to determine the spawn position
        //var NewPos = weaponSpawnPoint.position + pm.transform.rotation * Vector3.forward * offset;
        var NewPos = weaponSpawnPoint.position;
        NewPos.y += 1.5f;
        NewPos.z -= 1;
        var halberd = Instantiate(weaponPrefab, NewPos, weaponSpawnPoint.rotation);
        Debug.Log("ATTACK in enemy ! 5");
        if (IsPlayer == true)
            halberd.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 4, 4);
        else
        {
            halberd.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 4, 4, true);
        }
        Debug.Log("ATTACK in enemy ! 6");
    }
    public void SpawnWeaponProx()
    {
        float offset = 2.8f;
        try
        {
            Player pm1 = gameObject.GetComponentInParent<Player>();
        }
        catch
        {
            Debug.Log("ATTACK in enemy ! 3");
            SpawnEnemyWeaponProx();
            return;
        }

        Player pm = gameObject.GetComponentInParent<Player>();
        Debug.Log("ATTACK in enemy ! 2");
        // Use the forward vector to determine the spawn position
        var NewPos = weaponSpawnPoint.position + pm.characterModel.rotation * Vector3.forward * offset;
        NewPos.y += 1.5f;
        NewPos.z -= 1;
        var halberd = Instantiate(weaponPrefab, NewPos, pm.characterModel.rotation);
        if (IsPlayer == true)
            halberd.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 4, 4);
        else
        {
            halberd.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 4, 4, true);
        }
    }

    public override void Attack()
    {
        Debug.Log("ATTACK in enemy ! 1");
        if (shootingSound != null)
            audioSource.PlayOneShot(shootingSound);

        SpawnWeaponProx();
    }

    public void Update()
    {
        /* if (Input.GetKeyDown(KeyCode.L))
         {
             SpawnWeaponProx();
             if (UpdateLifeTime(LifeTime--))
                 Destroy(gameObject); // destruction de l'arme si la durabilit� atteint 0;
         }*/
    }
}
