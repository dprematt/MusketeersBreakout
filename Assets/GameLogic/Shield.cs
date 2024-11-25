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

    public GameObject GameObject
    {
        get { return gameObject; }
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

    public void OnTriggerEnter(Collider col)
    {
        /*if (isProtecting)
        {
            Bullet bullet = col.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.GetComponent<PhotonView>().RPC("Destroy", RpcTarget.AllBuffered);
            }
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

/*    public void whenPickUp(GameObject newHolder, Transform hand)
    {
        holder = newHolder;
        isPlayer = holder.CompareTag("Player") ? true : false;
        transform.parent = hand;
        transform.localPosition = new Vector3(positionX, positionY, positionZ);
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
        anim = holder.GetComponentInChildren<Animator>();
        isLooted = true;
    }*/

    public void whenPickUp(GameObject newHolder)
    {
        holder = newHolder;
        isPlayer = holder.CompareTag("Player") ? true : false;
        int holderID = newHolder.GetPhotonView().ViewID;
        photonView.RPC("SyncPickUp", RpcTarget.All, holderID, rotationX, rotationY, rotationZ);
    }

    [PunRPC]
    private void SyncPickUp(int holderID, float rotX, float rotY, float rotZ)
    {
        GameObject holderObject = PhotonView.Find(holderID).gameObject;
        holder = holderObject;
        anim = holder.GetComponentInChildren<Animator>();
        isPlayer = holder.CompareTag("Player");
        transform.parent = FindDeepChild(holderObject.transform, "jointItemL");
        transform.localPosition = new Vector3(positionX, positionY, positionZ);
        transform.localRotation = Quaternion.Euler(rotX, rotY, rotZ);
        isLooted = true;
    }

    public void setProtectionMode(bool mode)
    {
        photonView.RPC("setProtectionModeSync", RpcTarget.All, mode);
    }

    [PunRPC]
    public void setProtectionModeSync(bool mode)
    {
        if (mode != isProtecting)
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

        transform.localRotation = targetQuaternion;
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

    private void SetTag(string newTag)
    {
        this.gameObject.tag = newTag;
    }
}
