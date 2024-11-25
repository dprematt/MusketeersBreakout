using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Enemy : MonoBehaviourPun
{
    [SerializeField]
    public float health, maxHealth = 10f;
    public float speed = 5f;
    public float moveSpeed = 5f;
    private float distanceMoved = 0f;
    private Vector3 randomDirection;
    public float minDist = 3f;
    public Transform target;
    public Inventory inventory;
    public bool WeaponChoice = false;
    public Animator anim;
    public ParticleSystem bloodParticles;
    public List<Vector3> biomesPositions = new List<Vector3>();
    private Vector3 moveDirection;

    public Text Health_;

    public float detectionRadius = 10f;

    public float rotationSpeed = 5f;

    public List<Weapon> weaponList;
    private int currentWeapon = 0;

    public float nextAttack = 0f;
    private float delay = 1.5f;

    public GameObject Sword;
    public GameObject Knife;


    public EventListener eventListener;

    bool isPlaced = false;

    private void Start()
    {
        health = maxHealth;
        weaponList.Add(PhotonNetwork.Instantiate(Sword.name, transform.position, Quaternion.identity).GetComponent<Weapon>());
        weaponList.Add(PhotonNetwork.Instantiate(Knife.name, transform.position, Quaternion.identity).GetComponent<Weapon>());

        weaponList[0].positionX = 0.00086f;
        weaponList[0].positionY = 0.00059f;
        weaponList[0].positionZ = -0.00067f;
        weaponList[0].rotationX = 0f;
        weaponList[0].rotationY = -90f;
        weaponList[0].rotationZ = 0f;

        weaponList[1].positionX = 0.00108f;
        weaponList[1].positionY = 0.00139f;
        weaponList[1].positionZ = -0.0004f;
        weaponList[1].rotationX = 0f;
        weaponList[1].rotationY = 90f;
        weaponList[1].rotationZ = 180f;

        inventory = new Inventory(9, null, false);
        inventory.AddEnemyWeapon(weaponList[0]);
        inventory.AddEnemyWeapon(weaponList[1]);

        eventListener = GetComponent<EventListener>();
        if (eventListener == null)
        {
            Debug.Log("Ennemy: eventlistener == null");
        }
        Transform hand = FindDeepChild(transform, "hand.R");
        foreach (Weapon weapon in weaponList)
        {
            weapon.whenPickUp(gameObject);
        }
        if (weaponList.Count > 1)
        {
            weaponList[1].gameObject.SetActive(false);
        }
        weaponList[0].setAnim(gameObject);
        eventListener.weaponComp = weaponList[0];
        Health_.text = health.ToString() + "HP";
    }

    void Update()
    {
        // if (isPlaced == false)
        // {
        //     if (biomesPositions.Count != 0)
        //     {
        //         float randomBiome = Random.Range(0f, 5.0001f);
        //         float randomNumber = Random.Range(0f, 5.0001f);
        //         gameObject.transform.position = biomesPositions[(int)randomBiome];
        //         Vector3 pos = biomesPositions[(int)randomBiome];
        //         pos.y += 300;
        //         gameObject.transform.position = pos;
        //         if (randomNumber < 2)
        //         {
        //             Debug.Log("BIOME DESERT");
        //             health = 100;
        //         }
        //         else if (randomNumber < 4)
        //         {
        //             Debug.Log("BIOME JUNGLE");
        //             speed = 15;
        //         }
        //         else if (randomNumber < 6)
        //         {
        //             Debug.Log("BIOME NEIGE");
        //             speed = 10;
        //             health = 30;
        //         }
        //         isPlaced = true;
        //     }
        // }
        /*        if (weaponList.Count > 0 &&  weaponList[currentWeapon].anim.GetCurrentAnimatorStateInfo(0).IsName("hit1"))
                {
                    weaponList[currentWeapon].anim.SetBool("hit1", false);
                }*/

        if (target == null)
        {
            MoveRandomly();
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
                    if (eventListener == null)
                    {
                        eventListener = GetComponent<EventListener>();
                        if (eventListener == null)
                        {
                            Debug.Log("Ennemy: after set: eventlistener == null");
                        }
                    }
                    eventListener.weaponComp = weaponList[currentWeapon];

                    nextAttack = Time.time + delay;
                }
            }
        }
    }

    private void MoveRandomly()
    {
        if (moveDirection == Vector3.zero)
        {
            moveDirection = Random.onUnitSphere;
            moveDirection.y = 0f;
            distanceMoved = 0f;

            anim.SetBool("isWalking", true);
        }

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        distanceMoved += moveSpeed * Time.deltaTime;

        if (distanceMoved >= 5f)
        {
            moveDirection = Vector3.zero;
            anim.SetBool("isWalking", false);
        }
        if (moveDirection != Vector3.zero)
        {
            transform.LookAt(transform.position + moveDirection);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Weapon weaponComp = other.GetComponent<Weapon>();
        if (weaponComp != null)
        {
            if (weaponComp.isLooted && weaponComp.holder != gameObject && weaponComp.IsAttacking && weaponComp.holder.CompareTag("Player"))
            {
                TakeDamage(weaponComp.damages);
            }
        }

        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet != null)
        {
            TakeDamage(10);
            bullet.GetComponent<PhotonView>().RPC("Destroy", RpcTarget.AllBuffered);
        }

        if (other.gameObject.CompareTag("Player"))
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
        bloodParticles.Play();
        photonView.RPC("Damage_instance", RpcTarget.All, damageAmount);
        if (health <= 0)
        {
            GameObject player = GameObject.FindWithTag("Player");
            Player playerMov = player.GetComponent<Player>();
            playerMov.UpdateXp(10);
            Vector3 newPos = gameObject.transform.position;
            newPos.x += 2;
            GameObject loot = PhotonNetwork.Instantiate("Prefabs/Loot", newPos, gameObject.transform.rotation);
            Inventory lootInventory = loot.GetComponentInChildren<Inventory>();
            loot.GetComponentInChildren<Inventory>().loot = true;
            GameObject newItem = PhotonNetwork.Instantiate(weaponList[0].Name, transform.position, transform.rotation);
            newItem.GetComponent<Weapon>().RequestTagChange("TempObjTag");
            lootInventory.ApplyNetworkUpdate("TempObjTag");
            GameObject newItem2 = PhotonNetwork.Instantiate(weaponList[1].Name, transform.position, transform.rotation);
            newItem2.GetComponent<Weapon>().RequestTagChange("TempObjTag");
            lootInventory.ApplyNetworkUpdate("TempObjTag");
            PhotonNetwork.Destroy(gameObject);
            photonView.RPC("Particles", RpcTarget.All);
            return 1;
        }
        return 0;
    }

    [PunRPC]
    public void Damage_instance(float damageAmount) // ADD
    {
        health -= damageAmount;
        Health_.text = health.ToString() + "HP";
        if (health <= 0)
            Destroy(gameObject);
    }

    [PunRPC]
    public void Particles() // ADD
    {
        bloodParticles.Play();
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