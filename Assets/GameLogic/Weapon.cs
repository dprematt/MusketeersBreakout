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
    public bool damageDealt;

    public Animator anim;
    public AnimatorOverrideController animOverride;

    public GameObject holder;

    public Player playerComp;

    public int damages;

    public float positionX;
    public float positionY;
    public float positionZ;

    public float rotationX;
    public float rotationY;
    public float rotationZ;

    private bool isPlayer;

    public bool isLongRange;

    public bool ignoreAttack;

    public int countAttackClick;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        countAttackClick = 0;

        damageDealt = false;
        ignoreAttack = false;
    }

    public void Update()
    {
        if (gameObject.name == "Sword")
        {
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

        if (anim.GetInteger("intAttackPhase") > 0 && other.gameObject != holder && damageDealt == false && ignoreAttack == false)
        {
            if (holder.CompareTag("Player") && other.CompareTag("EnemyBody"))
            {
                other.gameObject.GetComponentInParent<Enemy>().TakeDamage(damages);
                Debug.Log("Hit !!!");
            }
            else if (holder.CompareTag("Enemy"))
            {
                other.gameObject.GetComponent<Player>().TakeDamage(damages);
            }
            damageDealt = true;
            return;
        }
        else if (ignoreAttack)
        {
            ignoreAttack = false;
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
        if (playerComp.hasShield && playerComp.shieldComp.isProtecting)
        {
            return;
        }

        countAttackClick += 1;

        if (countAttackClick == 1)
        {
            anim.SetInteger("intAttackPhase", 1);
            audioSource.PlayOneShot(attackSound);
            if (holder.CompareTag("Player")) {
                playerComp.stamina -= 30;
            }
            damageDealt = false;
        }
    }

    public void CheckAttackPhase()
    {
        //Debug.Log("Checking Attack Phase...");
        if (anim.GetCurrentAnimatorStateInfo(1).IsName("Attack 1"))
        {
            //Debug.Log("Current State : attack 1");
            if (countAttackClick > 1)
            {
                anim.SetInteger("intAttackPhase", 2);
                audioSource.PlayOneShot(attackSound);
                playerComp.stamina -= 30;
                damageDealt = false;
            }
            else
            {
                ResetAttackPhase();
            }
        }
        else if (anim.GetCurrentAnimatorStateInfo(1).IsName("Attack 2"))
        {
            //Debug.Log("Current State : attack 2");
            if (countAttackClick > 2)
            {
                anim.SetInteger("intAttackPhase", 3);
                audioSource.PlayOneShot(attackSound);
                playerComp.stamina -= 30;
                damageDealt = false;
            }
            else
            {
                ResetAttackPhase();
            }
        }
        else if (anim.GetCurrentAnimatorStateInfo(1).IsName("Attack 3"))
        {
            //Debug.Log("Current State : attack 3");
            if (countAttackClick >= 3)
            {
                ResetAttackPhase();
            }
        }
    }

    private void ResetAttackPhase()
    {
        countAttackClick = 0;
        anim.SetInteger("intAttackPhase", 0);
    }

    public void whenPickUp(GameObject newHolder, Transform hand)
    {
        holder = newHolder;
        isPlayer = holder.CompareTag("Player") ? true : false;
        transform.parent = hand;
        transform.localPosition = new Vector3(positionX, positionY, positionZ);
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
        if (isPlayer)
            playerComp = holder.GetComponent<Player>();
        isLooted = true;
    }

    public void setAnim()
    {
        anim = holder.GetComponentInChildren<Animator>();
        if(animOverride != null)
            anim.runtimeAnimatorController = animOverride;
        if (!isLongRange)
        {
            if (playerComp.hasShield)
            {
                anim.SetLayerWeight(4, 1f);
            }
            else
            {
                anim.SetLayerWeight(1, 1f);
            }
        }
    }
}
