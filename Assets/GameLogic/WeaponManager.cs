using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponManager : MonoBehaviourPun, IInventoryItem
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

    private GameObject holder;

    private PlayerMovements holderMovements;

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
        if (!isLooted && other.gameObject.CompareTag("Player")) {
            holder = other.gameObject;
            Transform hand = FindDeepChild(holder.transform, "jointItemR");

            transform.parent = hand;
            transform.localPosition = new Vector3(positionX, positionY, positionZ);
            transform.localRotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
            anim = holder.GetComponentInChildren<Animator>();
            anim.runtimeAnimatorController = animOverride;
            anim.SetInteger("weaponType", weaponType);
            holderMovements = holder.GetComponent<PlayerMovements>();
            isLooted = true;
        }

        if (anim.GetBool("hit" + currentAttack) && other.gameObject != holder && !damageDealt)
        {
            other.gameObject.GetComponent<Enemy>().TakeDamage(damages);
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

            if (Input.GetMouseButtonDown(0) && timeSinceAttack > resetTime && isLooted && holderMovements.stamina >= 30)
            {
                currentAttack++;
                damageDealt = false;
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
}
