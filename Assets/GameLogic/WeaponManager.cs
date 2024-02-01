using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponManager : MonoBehaviourPun, IInventoryItem
{
    public virtual string Name { get; protected set; }
    public virtual bool IsPlayer { get; set; }

    public AudioClip attackSound;
    public AudioSource audioSource;

    public Sprite _Image;

    private float lastAttackTime;

    public float coolDown = 2f;
    private float nextFireTime = 0f;
    public static int noOfClicks = 0;
    float lastClickedTime = 0;
    float maxComboDelay = 1;

    public bool isLooted = false;
    private bool damageDealt = false;

    public Animator anim;

    private GameObject holder;

    private PlayerMovements holderMovements;

    public bool isAttacking;
    private float timeSinceAttack;
    public int currentAttack = 0;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (isLooted)
        {
            Debug.Log(currentAttack);
            timeSinceAttack += Time.deltaTime;
            Attack();
        }
    }

    public virtual bool SetIsPlayer(bool type)
    {
        IsPlayer = type;
        return IsPlayer;
    }

    public virtual void OnPickup()
    {

    }

    public virtual void SelectItem(bool state)
    {
        gameObject.SetActive(state);
    }

    public Sprite Image
    {
        get { return _Image; }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!isLooted && other.gameObject.CompareTag("Player")) {
            holder = other.gameObject;
            Transform hand = FindDeepChild(holder.transform, "jointItemR");

            transform.parent = hand;
            transform.localPosition = new Vector3(0.02f, 0.15f, 0);
            transform.localRotation = Quaternion.Euler(90f, 90f, 0);
            anim = holder.GetComponentInChildren<Animator>();
            holderMovements = holder.GetComponent<PlayerMovements>();
            isLooted = true;
        }

        if (anim.GetBool("hit" + currentAttack) && other.gameObject != holder && !damageDealt)
        {
            other.gameObject.GetComponent<Enemy>().TakeDamage(2);
            damageDealt = true;
            return;
        }
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

    public virtual void Attack()
    {
        if (timeSinceAttack > 0.8)
        {
            anim.SetBool("hit" + currentAttack, false);

            if (Input.GetMouseButtonDown(0) && timeSinceAttack > 0.8f && isLooted)
            {
                currentAttack++;
                damageDealt = false;

                if (currentAttack > 3)
                {
                    currentAttack = 1;
                }

                anim.SetBool("hit" + currentAttack, true);

                audioSource.PlayOneShot(attackSound);

                timeSinceAttack = 0;
            }

            if (timeSinceAttack > 1.0f)
                currentAttack = 0;
        }
    }
}
