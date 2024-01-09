using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapons
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

    public void buildSword()
    {
        weaponPrefab = Resources.Load("Prefabs/WeaponProx") as GameObject;
        _Image = Resources.Load("Sprites/sword") as Sprite;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        weaponSpawnPoint = player.gameObject.transform;
        IsPlayer = true;
    }
    private void Start()
    {
        LifeTime = 10;
        audioSource = GetComponent<AudioSource>();
    }
    public override void OnPickup()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        weaponSpawnPoint = player.transform;
        transform.localPosition = new Vector3(0.3f, 1f, 0.0f);
        transform.parent = player.transform;
        //gameObject.SetActive(false);
    }
    public override string Name
    {
        get { return "Sword"; }
    }
    public void SpawnEnemyWeaponProx()
    {
        //float offset = 2.8f;
        //Enemy pm = gameObject.GetComponentInParent<Enemy>();

        //audioSource.PlayOneShot(shootingSound);
        // Use the forward vector to determine the spawn position
        //var NewPos = weaponSpawnPoint.position + pm.transform.rotation * Vector3.forward * offset;
        Vector3 NewPos = weaponSpawnPoint.position;
        NewPos.y += 1.5f;
        NewPos.z -= 1;
        var halberd = Instantiate(weaponPrefab, NewPos, weaponSpawnPoint.rotation);
        if (IsPlayer == true)
            halberd.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 4, 4);
        else
        {
            halberd.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 4, 4, true);
        }
    }
    public void SpawnWeaponProx()
    {
        float offset = 2.2f;
        try
        {
            PlayerMovements pm1 = gameObject.GetComponentInParent<PlayerMovements>();
        }
        catch
        {
            Debug.Log("ATTACK in enemy ! 3");
            SpawnEnemyWeaponProx();
            return;
        }
        PlayerMovements pm = gameObject.GetComponentInParent<PlayerMovements>();
        // Use the forward vector to determine the spawn position
        var NewPos = weaponSpawnPoint.position + pm.characterModel.rotation * Vector3.forward * offset;
        NewPos.y += 1.5f;
        NewPos.z -= 1;
        var sword = Instantiate(weaponPrefab, NewPos, pm.characterModel.rotation);
        sword.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 3, 5);
    }

    public override void Attack()
    {
        if (shootingSound != null)
            audioSource.PlayOneShot(shootingSound);
        SpawnWeaponProx();
    }
    public void Update()
    {
      /*  if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnWeaponProx();
            if (UpdateLifeTime(LifeTime--))
                Destroy(gameObject); // destruction de l'arme si la durabilité atteint 0;
        }*/
    }
}
