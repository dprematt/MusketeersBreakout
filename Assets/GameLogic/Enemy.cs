using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class Enemy : MonoBehaviour
{
    [SerializeField] float health, maxHealth = 10f;
    public float speed = 1f;
    public float minDist = 3f;
    public float maxDist = 10f;
    public Transform target;
    public Inventory inventory;

    // Ajout de variables pour le mouvement en carré
    private Vector3[] squarePath;
    private int currentPathIndex = 0;

    private void Start()
    {
        health = maxHealth;

        // Initialisation de la cible si elle n'est pas définie
        if (target == null)
        {
            if (GameObject.FindWithTag("Player") != null)
            {
                target = GameObject.FindWithTag("Player").GetComponent<Transform>();
            }
        }

        // Initialisation de l'inventaire
        inventory = new Inventory(9, new List<IInventoryItem>(), false);
        Halberd halberd = new Halberd();
        halberd._Image = Resources.Load("Sprites/halberd") as Sprite;
        halberd.weaponPrefab = Resources.Load("Prefabs/WeaponProx") as GameObject;
        halberd.weaponSpawnPoint = gameObject.transform;
        inventory.AddItem(halberd);

        // Initialisation du chemin en carré
        Vector3 initialPosition = transform.position;
        squarePath = new Vector3[]
        {
            initialPosition + new Vector3(0, 0, 20),
            initialPosition + new Vector3(20, 0, 20),
            initialPosition + new Vector3(20, 0, 0),
            initialPosition
        };
    }

    void Update()
    {
        if (target == null)
            return;

        transform.LookAt(target);
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > minDist && distance < maxDist)
        {
            // Exécutez votre logique existante ici
            transform.position += transform.forward * speed * Time.deltaTime;
            inventory.mItems[0].Attack();
        }
        else if (squarePath != null) // Vérifiez si squarePath est défini
        {
            // Déplacement automatique en carré
            Vector3 nextPosition = squarePath[currentPathIndex];
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);

            if (transform.position == nextPosition)
            {
                // Passer à l'étape suivante du carré
                currentPathIndex = (currentPathIndex + 1) % squarePath.Length;
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
