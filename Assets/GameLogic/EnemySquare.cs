using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class EnemySquare : MonoBehaviour
{
     [SerializeField] float health, maxHealth = 10f;
    public float speed = 1f;
    public float minDist = 3f;
    public float maxDist = 10f;
    public Transform target;
    public Inventory inventory;
    public bool WeaponChoice = false;


    public Transform[] points;
    int current;
    private void Start()
    {
        health = maxHealth;

        if (target == null) {
            if (GameObject.FindWithTag("Player")!=null)
            {
                target = GameObject.FindWithTag("Player").GetComponent<Transform>();
            }
        }
        List<IInventoryItem> items = new List<IInventoryItem>();
        Halberd halberd = new Halberd();
        halberd._Image = Resources.Load("Sprites/halberd") as Sprite;
        halberd.weaponPrefab = Resources.Load("Prefabs/WeaponProx") as GameObject;
        halberd.weaponSpawnPoint = gameObject.transform;
        Debug.Log("is inventory still alive" + inventory);
        halberd.IsPlayer = false;
        items.Add(halberd);
        Sword sword = new Sword();
        // sword._Image = Resources.Load("Sprites/sword") as Sprite;
        sword.weaponPrefab = Resources.Load("Prefabs/WeaponProx") as GameObject;
        sword.weaponSpawnPoint = gameObject.transform;
        sword.IsPlayer = false;
        items.Add(sword);
        inventory = new Inventory(9, items, false);
        Debug.Log("in start enemy weapons count = " + inventory.mItems.Count);

        current = 0;
    }

    void Update() 
    {
        if (target == null)
        {
            if (GameObject.FindWithTag("Player") != null)
            {
                target = GameObject.FindWithTag("Player").GetComponent<Transform>();
            }
            else
            {
                return;
            }
        }

        transform.LookAt(target);
        float distance = Vector3.Distance(transform.position,target.position);

        if (distance > minDist && distance < maxDist)	
        {
            transform.position += transform.forward * speed * Time.deltaTime;
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
        } else {
            if (transform.position != points[current].position)
            {
                transform.position = Vector3.MoveTowards(transform.position, points[current].position, speed * Time.deltaTime);
            }
            else 
            {
                current=(current+1)%points.Length;
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public int TakeDamage(float damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            PlayerMove player = GameObject.FindObjectOfType<PlayerMove>();
            player.UpdateXp(10);
            GameObject LootPrefab = Resources.Load<GameObject>("Prefabs/Loot");
            var loot = Instantiate(LootPrefab, target.position, target.rotation);
            loot.GetComponentInChildren<Inventory>().Initialize(9, inventory.mItems, true);
            Destroy(gameObject);
            return 1;
        }
        return 0;
    }
}