using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class Weapon : MonoBehaviourPun, IInventoryItem
{
    [SerializeField] private string itemName;
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

    public bool _isAttacking = false;

    public string Name
    {
        get { return itemName; }
        set { itemName = value; }
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        countAttackClick = 0;
    }

    public void Update()
    {
        if (gameObject.name == "Sword")
        {
        }
    }

    public GameObject GameObject
    {
        get { return gameObject; }
    }

    public bool IsAttacking
    {
        get { return _isAttacking; }
        set
        {
            _isAttacking = value;
            photonView.RPC("SyncIsAttacking", RpcTarget.All, value);
        }
    }

    [PunRPC]
    private void SyncIsAttacking(bool value)
    {
        _isAttacking = value;
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
/*        if (anim.GetInteger("intAttackPhase") > 0 && other.CompareTag("Shield"))
        {
            Shield shieldComp = other.gameObject.GetComponent<Shield>();
            if (shieldComp.isProtecting)
            {
                return;
            }
        }*/

/*        if (*//*anim.GetInteger("intAttackPhase") > 0 && *//*other.gameObject != holder && IsAttacking)
        {
            if (IsPlayer && other.CompareTag("Player"))
            {
                Debug.Log("HIT IN WEAPON");
                other.gameObject.GetComponentInParent<Player>().TakeDamage(damages);
                Player player = gameObject.GetComponentInParent<Player>();
                if (player != null)
                {
                    player.dmgDone += player.EquippedWeapon.damages;
                }
            }
            else if (holder.CompareTag("Enemy") && other.CompareTag("Player"))
            {
                other.gameObject.GetComponent<Player>().TakeDamage(damages);
            }
            else
            {
                Player player = gameObject.GetComponentInParent<Player>();
                if (player != null)
                {
                    player.dmgDone += player.EquippedWeapon.damages;
                }
                other.gameObject.GetComponentInParent<Enemy>().TakeDamage(damages);
            }
            IsAttacking = false;
        }*/
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
        if (isPlayer)
        {
            if (countAttackClick > 4)
                ResetAttackPhase();
        }
        
        if (countAttackClick == 1)
        {
            anim.SetInteger("intAttackPhase", 1);
            audioSource.PlayOneShot(attackSound);
            if (isPlayer) {
                playerComp.stamina -= 30;
                playerComp.StartAttackSlowdown(1f, 0.5f);
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
                    playerComp.StartAttackSlowdown(1f, 0.5f);
                }
                IsAttacking = false;
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
                    playerComp.StartAttackSlowdown(1f, 0.5f);
                }
                IsAttacking = false;
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
        IsAttacking = false;
    }

    public void whenPickUp(GameObject newHolder)
    {
        if (newHolder == null)
        {
            holder = null;
            photonView.RPC("SyncPickUp", RpcTarget.All, 0, rotationX, rotationY, rotationZ, "null");
        }
        else
        {
            holder = newHolder;
            isPlayer = holder.CompareTag("Player") ? true : false;
            String hand = isPlayer ? "jointItemR" : "hand.R";
            int holderID = newHolder.GetPhotonView().ViewID;
            photonView.RPC("SyncPickUp", RpcTarget.All, holderID, rotationX, rotationY, rotationZ, hand);
        }
    }

     [PunRPC]
    private void SyncPickUp(int holderID, float rotX, float rotY, float rotZ, String hand)
    {
        if (holderID == 0)
        {
            holder = null;
            anim = null;
            transform.parent = null;
        }
        GameObject holderObject = PhotonView.Find(holderID).gameObject;
        holder = holderObject;
        anim = holder.GetComponentInChildren<Animator>();
        isPlayer = holder.CompareTag("Player");
        transform.parent = FindDeepChild(holderObject.transform, hand);
        transform.localPosition = new Vector3(positionX, positionY, positionZ);
        transform.localRotation = Quaternion.Euler(rotX, rotY, rotZ);

        if (isPlayer)
            playerComp = holder.GetComponent<Player>();
        isLooted = true;
    }

    public void resetAnim(GameObject newHolder)
    {
        if (newHolder != null)
            holder = newHolder;
        anim = holder.GetComponentInChildren<Animator>();
        anim.SetLayerWeight(1, 1f);

    }
    public void setAnim(GameObject newHolder)
    {
        if (newHolder != null)
            holder = newHolder;

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

    public void DeactivateAllObjects()
    {
        if (gameObject != null)
        {
            // Ensure the current client is the owner or a master client
            if (photonView.IsMine || PhotonNetwork.IsMasterClient)
                photonView.RPC("DeactivateObject", RpcTarget.All);
        }
    }

    [PunRPC]
    void DeactivateObject()
    {
        gameObject.SetActive(false);
    }

    private void SetTag(string newTag)
    {
        this.gameObject.tag = newTag;
    }

    // Public method to request tag change, called by any client
    public void RequestTagChange(string newTag)
    {
        // Ensure the request is only made by the owner
        if (photonView.IsMine)
        {
            photonView.RPC("SyncTag", RpcTarget.All, newTag);
        }
    }

    // RPC method to synchronize the tag across all clients
    [PunRPC]
    public void SyncTag(string newTag)
    {
        SetTag(newTag);
    }
}
