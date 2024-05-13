using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class EnemyShape : MonoBehaviour
{
     [SerializeField] float health, maxHealth = 10f;
    public float speed = 5f;
    public float minDist = 3f;
    public Transform target;
    public Inventory inventory;
    public bool WeaponChoice = false;
    public Animator anim;
    public ParticleSystem bloodParticles;
    public List<Vector3> biomesPositions = new List<Vector3>();


    public float detectionRadius = 10f;

    public float rotationSpeed = 5f;

    public Transform[] points;
    int current;

    bool isPlaced = false;

    private void Start()
    {
        health = maxHealth;

        current = 0;
    }

    void Update() 
    {
        if (isPlaced == false)
        {
            if (biomesPositions.Count != 0)
            {
                if (biomesPositions == null)
                {
                    Debug.Log("biome pos is null..");
                }
                float randomBiome = Random.Range(0f, 5.0001f);
                float randomNumber = Random.Range(0f, 5.0001f);
                gameObject.transform.position = biomesPositions[(int)randomBiome];
                // inventory = new Inventory(9, null, false);
                // inventory.AddEnemyWeapon("Sword");
                // inventory.AddEnemyWeapon("Gun");
                if (randomNumber < 2)
                {
                    health = 100;
                }
                else if (randomNumber < 4)
                {
                    Debug.Log("BIOME jungle");
                    speed = 15;
                }
                else if (randomNumber < 6)
                {
                    Debug.Log("BIOME neige");
                    speed = 10;
                    health = 30;
                }
                isPlaced = true;
            }
        }
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
            GameObject LootPrefab = Resources.Load<GameObject>("Prefabs/Loot");
            var loot = Instantiate(LootPrefab, gameObject.transform.position, gameObject.transform.rotation);
            loot.GetComponentInChildren<Inventory>().Initialize(9, inventory.mItems, true);
            Destroy(gameObject);
            return 1;
        }
        return 0;
    }
}