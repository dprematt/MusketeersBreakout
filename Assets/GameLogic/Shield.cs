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

    public bool isProtecting = false;

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
        if (mode != isProtecting && anim.GetInteger("intAttackPhase") == 0)
        {
            isProtecting = mode;
            if (mode)
            {
                Vector3 targetRotation = new Vector3(35.86f, 90f, 104f);
                Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
                StartCoroutine(TransitionAnimation(1f));
                StartCoroutine(TransitionRotation(targetQuaternion));
            }
            else
            {
                Vector3 targetRotation = new Vector3(rotationX, rotationY, rotationZ);
                Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
                StartCoroutine(TransitionAnimation(0f));
                StartCoroutine(TransitionRotation(targetQuaternion));
            }
        }
    }

    IEnumerator TransitionAnimation(float targetWeight)
    {
        float currentWeight = anim.GetLayerWeight(3);
        float duration = 0.2f;

        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            anim.SetLayerWeight(3, Mathf.Lerp(currentWeight, targetWeight, t));
            yield return null;
        }

        anim.SetLayerWeight(3, targetWeight);
    }

    IEnumerator TransitionRotation(Quaternion targetQuaternion)
    {
        Quaternion startRotation = transform.localRotation;
        float startTime = Time.time;
        float duration = 0.2f;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            transform.localRotation = Quaternion.Slerp(startRotation, targetQuaternion, t);
            yield return null;
        }

        transform.localRotation = targetQuaternion; // Assure que la rotation finale est correcte
    }
}
