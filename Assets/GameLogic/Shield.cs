using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shield : MonoBehaviourPun, IInventoryItem
{
    public virtual string Name { get; protected set; }
    public virtual bool IsPlayer { get; set; }

    public Sprite _Image;

    public bool isLooted = false;

    public GameObject holder;

    public float positionX;
    public float positionY;
    public float positionZ;

    public float rotationX;
    public float rotationY;
    public float rotationZ;

    private bool isPlayer;

    private bool isProtecting = false;

    public Animator anim;

    private void Start()
    {
    }

    public void Update()
    {
    }

    public virtual bool SetIsPlayer(bool type)
    {
        IsPlayer = type;
        return IsPlayer;
    }

    public virtual void OnPickup()
    {

    }

    public virtual void Attack()
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
        if (isProtecting)
        {
            if (other.CompareTag("Bullet"))
            {
                Destroy(other.gameObject);
            }

            Weapon weaponComp = other.gameObject.GetComponent<Weapon>();
            if (weaponComp != null)
            {
                weaponComp.ignoreAttack = true;
            }
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

    public void whenPickUp(GameObject newHolder, Transform hand)
    {
        holder = newHolder;
        isPlayer = holder.CompareTag("Player") ? true : false;
        transform.parent = hand;
        transform.localPosition = new Vector3(positionX, positionY, positionZ);
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
        anim = holder.GetComponentInChildren<Animator>();
        isLooted = true;
    }

    public void setProtectionMode(bool mode)
    {
        isProtecting = mode;
        if (mode)
        {
            anim.SetBool("isProtecting", true);
        }
        else
        {
            anim.SetBool("isProtecting", false);
        }
    }
}
