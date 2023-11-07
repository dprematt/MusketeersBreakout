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
        inventory = new Inventory(9, new List<IInventoryItem>(), false);
        Halberd halberd = new Halberd();
        halberd._Image = Resources.Load("Sprites/halberd") as Sprite;
        Debug.Log("is inventory still alive" + inventory);
        halberd.weaponPrefab = Resources.Load("Prefabs/WeaponProx") as GameObject;
        halberd.weaponSpawnPoint = gameObject.transform;
        inventory.AddItem(halberd);

        current = 0;
    }

    void Update() 
    {
        if (target == null)
            return;
        
        transform.LookAt(target);
        float distance = Vector3.Distance(transform.position,target.position);

        if (distance > minDist && distance < maxDist)	
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            //inventory.mItems[0].Attack();
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
            loot.GetComponent<Inventory>().Initialize(9, inventory.mItems, true);
            Destroy(gameObject);
            return 1;
        }
        return 0;
    }
}