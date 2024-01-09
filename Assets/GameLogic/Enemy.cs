using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class Enemy : MonoBehaviour
{
    [SerializeField] float health, maxHealth = 10f;
    public float speed = 5f;
    public float minDist = 3f;
    public Transform target;
    public Inventory inventory;
    public bool WeaponChoice = false;
    public Animator anim;
    public ParticleSystem bloodParticles;

    public float detectionRadius = 10f;

    public float rotationSpeed = 5f;

    public Transform[] points;
    int current;
    private void Start()
    {
        health = maxHealth;

        List<IInventoryItem> items = new List<IInventoryItem>();
        Halberd halberd = new Halberd();
        halberd._Image = Resources.Load("Sprites/halberd") as Sprite;
        halberd.weaponPrefab = Resources.Load("Prefabs/WeaponProx") as GameObject;
        halberd.weaponSpawnPoint = gameObject.transform;
        Debug.Log("is inventory still alive" + inventory);
        halberd.IsPlayer = false;
        halberd.audioSource = GetComponent<AudioSource>();
        items.Add(halberd);
        Sword sword = new Sword();
        // sword._Image = Resources.Load("Sprites/sword") as Sprite;
        sword.weaponPrefab = Resources.Load("Prefabs/WeaponProx") as GameObject;
        sword.weaponSpawnPoint = gameObject.transform;
        sword.IsPlayer = false;
        sword.audioSource = GetComponent<AudioSource>();
        items.Add(sword);
        inventory = new Inventory(9, items, false);
        Debug.Log("in start enemy weapons count = " + inventory.mItems.Count);

        current = 0;
    }

    void Update() 
    {
        if (target == null)
        {
            anim.SetBool("isWalking", false);
            return;
        }

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= detectionRadius && dist >= minDist)
        {
            anim.SetBool("isWalking", true);

            Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);

            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else
        {
            Debug.Log("ATTACK in enemy ! -1");
            anim.SetBool("isWalking", false);
            if (WeaponChoice == false)
            {
                Debug.Log("ATTACK in enemy ! 0");
                inventory.mItems[0].Attack();
                WeaponChoice = true;
            }
            else
            {
                Debug.Log("ATTACK in enemy ! 0Bis");
                inventory.mItems[inventory.mItems.Count - 1].Attack();
                WeaponChoice = false;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target = other.transform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target = null;
        }
    }

    public int TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        bloodParticles.Play();

        if (health <= 0)
        {
            GameObject player = GameObject.FindWithTag("Player");
            PlayerMovements playerMov = player.GetComponent<PlayerMovements>();
            playerMov.UpdateXp(10);
            GameObject LootPrefab = Resources.Load<GameObject>("Prefabs/Loot");
            var loot = Instantiate(LootPrefab, gameObject.transform.position, gameObject.transform.rotation);
            loot.GetComponentInChildren<Inventory>().Initialize(9, inventory.mItems, true);
            Destroy(gameObject);
            return 1;
        }
        return 0;
    }
}