using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class EnemySquare : MonoBehaviour
{
    [SerializeField]
    float health, maxHealth = 10f;
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

        /*List<IInventoryItem> items = new List<IInventoryItem>();
        Halberd halberd = new Halberd();
        halberd._Image = Resources.Load("Sprites/halberd") as Sprite;
        halberd.weaponPrefab = Resources.Load("Prefabs/WeaponProx") as GameObject;
        halberd.weaponSpawnPoint = gameObject.transform;
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
        inventory = new Inventory(9, items, false);*/

        current = 0;
    }

    void Update() 
    {
        if (target == null)
        {
            anim.SetBool("isWalking", true);
            if (transform.position.x != points[current].position.x && transform.position.z != points[current].position.z)
            {
                Vector3 targetPosition = new Vector3(points[current].position.x, transform.position.y, points[current].position.z);

                Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            }
            else
            {
                current = (current + 1) % points.Length;
            }
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
        /*else
        {
            anim.SetBool("isWalking", false);
            if (WeaponChoice == false)
            {
                inventory.mItems[0].Attack();
                WeaponChoice = true;
            }
            else
            {
                inventory.mItems[inventory.mItems.Count - 1].Attack();
                WeaponChoice = false;
            }
        }*/
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
            Player playerMov = player.GetComponent<Player>();
            playerMov.UpdateXp(10);
            /*GameObject LootPrefab = Resources.Load<GameObject>("Prefabs/Loot");
            if (target == null)
            {
                target = gameObject.transform;
            }
            if (inventory == null)
            {
                inventory = null;
            }
            var loot = Instantiate(LootPrefab, target.position, target.rotation);
            loot.GetComponent<Inventory>().Initialize(9, inventory.mItems, true);*/
            Destroy(gameObject);
            return 1;
        }
        return 0;
    }
}