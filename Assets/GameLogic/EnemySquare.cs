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
    public List<Vector3> biomesPositions = new List<Vector3>();
    int current;

    private void Start()
    {
        Debug.Log("ENEMY SQUARE: in enemy square start");
        inventory.AddEnemyWeapon("Sword");
        inventory.AddEnemyWeapon("Gun");
        health = maxHealth;
        if (biomesPositions == null)
        {
            Debug.Log("biome pos is null..");
        }
        for (int i = 0; i < 6; ++i)
        {
            Debug.Log("Biome Pos = " + biomesPositions[i]);
        }
        //_generator = FindObjectOfType<generator>();


        //current = 0;
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
            if (inventory.mItems[0].Name == "Sword")
            {
                inventory.SwapItems(0, 1);
            }
            inventory.mItems[0].Attack();
            Debug.Log("enemy range attack");
        }
        else
        {
            anim.SetBool("isWalking", false);
            if (inventory.mItems[0].Name == "Gun")
            {
                inventory.SwapItems(0, 1);
            }
            Debug.Log("enemy melee attack");
            inventory.mItems[0].Attack();
            /*if (WeaponChoice == false)
            {
                inventory.mItems[0].Attack();
                WeaponChoice = true;
            }
            else
            {
                inventory.mItems[inventory.mItems.Count - 1].Attack();
                WeaponChoice = false;
            }*/
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