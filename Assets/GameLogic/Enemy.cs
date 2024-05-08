using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class Enemy : MonoBehaviour
{
    [SerializeField]
    public float health, maxHealth = 10f;
    public float speed = 5f;
    public float minDist = 3f;
    public Transform target;
    public Inventory inventory;
    public bool WeaponChoice = false;
    public Animator anim;
    public ParticleSystem bloodParticles;

    public float detectionRadius = 10f;

    public float rotationSpeed = 5f;

    public List<Weapon> weaponList;
    private int currentWeapon = 0;

    public float nextAttack = 0f;
    private float delay = 1.5f;

    public EventListener eventListener;

    private void Start()
    {
        health = maxHealth;
        List<IInventoryItem> items = new List<IInventoryItem>();
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
        inventory = new Inventory(9, items, false);

        eventListener = GetComponent<EventListener>();

        Transform hand = FindDeepChild(transform, "hand.R");
        foreach (Weapon weapon in weaponList)
        {
            weapon.whenPickUp(gameObject, hand);
        }
        if (weaponList.Count > 1)
        {
            weaponList[1].gameObject.SetActive(false);
        }
        weaponList[0].setAnim(gameObject);
        eventListener.weaponComp = weaponList[0];
    }

    void Update()
    {
/*        if (weaponList.Count > 0 &&  weaponList[currentWeapon].anim.GetCurrentAnimatorStateInfo(0).IsName("hit1"))
        {
            weaponList[currentWeapon].anim.SetBool("hit1", false);
        }*/

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
            anim.SetBool("isWalking", false);
            if (weaponList.Count > 0)
            {
                if (Time.time > nextAttack)
                {
                    weaponList[currentWeapon].Attack();
                    weaponList[currentWeapon].gameObject.SetActive(false);

                    currentWeapon = currentWeapon == 0 ? 1 : 0;

                    weaponList[currentWeapon].gameObject.SetActive(true);
                    weaponList[currentWeapon].setAnim(gameObject);
                    eventListener.weaponComp = weaponList[currentWeapon];

                    nextAttack = Time.time + delay;
                }
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
    Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}