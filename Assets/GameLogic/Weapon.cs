using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Weapon : MonoBehaviourPun, IInventoryItem
{
    public virtual string Name { get; protected set; }
    public virtual bool IsPlayer { get; set; }

    public int weaponType;

    public AudioClip attackSound;
    public AudioSource audioSource;

    public Sprite _Image;

    public bool isLooted = false;
    private bool damageDealt = false;

    public Animator anim;
    public AnimatorOverrideController animOverride;

    public GameObject holder;

    public PlayerMovements holderMovements;

    private float timeSinceAttack;
    private int currentAttack = 0;

    public int damages;

    public float resetTime;

    public float positionX;
    public float positionY;
    public float positionZ;

    public float rotationX;
    public float rotationY;
    public float rotationZ;

    private bool isPlayer;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (isLooted)
        {
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
        if (anim.GetBool("hit" + currentAttack) && other.gameObject != holder && !damageDealt)
        {
            if (holder.CompareTag("Player"))
                other.gameObject.GetComponent<Enemy>().TakeDamage(damages);
            else if (holder.CompareTag("Enemy"))
                other.gameObject.GetComponent<PlayerMovements>().TakeDamage(damages);
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
        if (timeSinceAttack > resetTime)
        {
            anim.SetBool("hit" + currentAttack, false);

            if (Input.GetMouseButtonDown(0) && holderMovements.stamina >= 30 && timeSinceAttack > resetTime && isLooted)
            {
                currentAttack++;
                damageDealt = false;
                if (isPlayer)
                    holderMovements.stamina -= 30;

                if (currentAttack > 3)
                {
                    currentAttack = 1;
                }

                anim.SetBool("hit" + currentAttack, true);

                audioSource.PlayOneShot(attackSound);

                timeSinceAttack = 0;
            }

            if (timeSinceAttack > (resetTime + 0.2f))
                currentAttack = 0;
        }
    }

    public void BotAttack()
    {
        currentAttack = 1;
        anim.SetBool("hit" + currentAttack, true);
        damageDealt = false;
        audioSource.PlayOneShot(attackSound);
    }

    public void whenPickUp(GameObject newHolder, Transform hand)
    {
        holder = newHolder;
        isPlayer = holder.CompareTag("Player") ? true : false;
        transform.parent = hand;
        transform.localPosition = new Vector3(positionX, positionY, positionZ);
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
        if (isPlayer)
            holderMovements = holder.GetComponent<PlayerMovements>();
        isLooted = true;
    }

    public void setAnim()
    {
        anim = holder.GetComponentInChildren<Animator>();
        anim.runtimeAnimatorController = animOverride;
        anim.SetInteger("weaponType", weaponType);
    }
}
