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

    public int countAttackClick;

    public bool isAttacking;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        countAttackClick = 0;

        isAttacking = false;
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
        if (anim.GetInteger("intAttackPhase") > 0 && other.CompareTag("Shield"))
        {
            Shield shieldComp = other.gameObject.GetComponent<Shield>();
            if (shieldComp.isProtecting)
            {
                return;
            }
        }

        if (anim.GetInteger("intAttackPhase") > 0 && other.gameObject != holder && isAttacking)
        {
            if (IsPlayer && other.CompareTag("EnemyBody"))
            {
                other.gameObject.GetComponentInParent<Enemy>().TakeDamage(damages);
                Debug.Log("Hit !!!");
            }
            else if (holder.CompareTag("Enemy") && other.CompareTag("Player"))
            {
                other.gameObject.GetComponent<Player>().TakeDamage(damages);
            }
            isAttacking = false;
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
        if (isPlayer)
        {
            if (playerComp.hasShield && playerComp.shieldComp.isProtecting)
            {
                return;
            }
        }

        countAttackClick += 1;

        if (countAttackClick == 1)
        {
            anim.SetInteger("intAttackPhase", 1);
            audioSource.PlayOneShot(attackSound);
            if (isPlayer) {
                playerComp.stamina -= 30;
            }
        }
    }

    public void CheckAttackPhase()
    {
        if (anim.GetCurrentAnimatorStateInfo(1).IsName("Attack 1"))
        {
            if (countAttackClick > 1)
            {
                anim.SetInteger("intAttackPhase", 2);
                audioSource.PlayOneShot(attackSound);
                if (isPlayer)
                {
                    playerComp.stamina -= 30;
                }
                isAttacking = false;
            }
            else
            {
                ResetAttackPhase();
            }
        }
        else if (anim.GetCurrentAnimatorStateInfo(1).IsName("Attack 2"))
        {
            if (countAttackClick > 2)
            {
                anim.SetInteger("intAttackPhase", 3);
                audioSource.PlayOneShot(attackSound);
                if (isPlayer)
                {
                    playerComp.stamina -= 30;
                }
                isAttacking = false;
            }
            else
            {
                ResetAttackPhase();
            }
        }
        else if (anim.GetCurrentAnimatorStateInfo(1).IsName("Attack 3"))
        {
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
        isAttacking = false;
    }

    public void whenPickUp(GameObject newHolder, Transform hand)
    {
        Debug.Log("Arme mise dans les main du joueur");
        holder = newHolder;
        isPlayer = holder.CompareTag("Player") ? true : false;
        int holderID = newHolder.GetPhotonView().ViewID;
        Vector3 relativePosition = hand.position;
        photonView.RPC("SyncPickUp", RpcTarget.All, holderID, relativePosition, rotationX, rotationY, rotationZ);
    }

     [PunRPC]
    private void SyncPickUp(int holderID, Vector3 relativePosition, float rotX, float rotY, float rotZ)
    {
        GameObject holderObject = PhotonView.Find(holderID).gameObject;
        holder = holderObject;
        isPlayer = holder.CompareTag("Player");
        transform.parent = holder.transform;
        transform.localPosition = new Vector3(positionX, positionY, positionZ);
        transform.localRotation = Quaternion.Euler(rotX, rotY, rotZ);

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
            if (isPlayer)
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
            else
            {
                anim.SetLayerWeight(1, 1f);
            }
        }
    }
}
