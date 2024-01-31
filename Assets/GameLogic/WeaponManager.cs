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
    public float coolDown = 3f;

    public bool isLooted = false;

    public bool isAttacking = false;

    public Animator anim;

    GameObject holder;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        anim.SetBool("isAttacking", false);
        if (isLooted && Input.GetMouseButton(0) && Time.time - lastAttackTime > coolDown)
        {
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
            isLooted = true;
        }

        if (isAttacking && other.gameObject != holder)
        {
            other.gameObject.GetComponent<Enemy>().TakeDamage(2);
            isAttacking = false;
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
        isAttacking = true;
        anim.SetBool("isAttacking", true);
        audioSource.PlayOneShot(attackSound);
        lastAttackTime = Time.time;
    }
}
