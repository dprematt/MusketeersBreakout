using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
        Transform spawnInHand = transform.Find("spawnInHand");
        Transform hand = FindDeepChild(player.transform, "jointItemR");

        transform.parent = hand;
        transform.localPosition = new Vector3(0.02f, 0.15f, 0);
        transform.localRotation = Quaternion.Euler(90f, 90f, 0);
        //gameObject.SetActive(false);
    }

    Transform FindDeepChild(Transform parent, string nom)
    {
        foreach (Transform child in parent)
        {
            if (child.name == nom)
                return child;

            Transform result = FindDeepChild(child, nom);
            if (result != null)
                return result;
        }
        return null;
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
        if (PhotonNetwork.IsMasterClient)
        {
            var halberd = PhotonNetwork.Instantiate(weaponPrefab.name, weaponSpawnPoint.position, weaponSpawnPoint.rotation);
        
            if (IsPlayer == true)
                halberd.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 4, 4);
            else
            {
                halberd.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 4, 4, true);
            }
        }
    }
    public void SpawnWeaponProx()
    {
        float offset = 2.2f;
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
        if (PhotonNetwork.IsMasterClient)
        {
            Player pm = gameObject.GetComponentInParent<Player>();

            // Calcul de la position de spawn de l'épée
            var NewPos = weaponSpawnPoint.position + pm.characterModel.rotation * Vector3.forward * offset;
            NewPos.y += 1.5f;
            NewPos.z -= 1;

            // Instanciation de l'épée sur le réseau
            var sword = PhotonNetwork.Instantiate(weaponPrefab.name, NewPos, pm.characterModel.rotation);

            // Initialisation de l'épée
            sword.GetComponent<WeaponProx>().Initialize(weaponSpawnPoint, 3, 5);
        }
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
                Destroy(gameObject); // destruction de l'arme si la durabilit� atteint 0;
        }*/
    }
}
