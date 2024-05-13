using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HealthManager : MonoBehaviourPunCallbacks
{

    float Health_ = 100;
    public Text Health_Text_;
    PlayFabInventory PFInventory_;

    public ParticleSystem bloodParticles;

    private void Start()
    {

        PFInventory_ = gameObject.GetComponent<PlayFabInventory>();
        if (PFInventory_ == null)
            Debug.Log("PFInventory_ null");
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(HealthManager))]
    public class SubtractTenButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            HealthManager subtractTenButton = (HealthManager)target;

            if (GUILayout.Button("Subtract 10"))
            {
                subtractTenButton.SubtractTen();
            }
        }
    }
#endif

    void SubtractTen()
    {
        if (10 < Health_)
            {
                photonView.RPC("DamageInstance", RpcTarget.All, 10);
                //photonView.RPC("SetPlayerHP", RpcTarget.All);
            }
            else
                DestroyPlayer();
    }


    public void Take_Damage(int Damage)
    {
        if (photonView.IsMine)
        {
            if (Damage < Health_)
            {
                photonView.RPC("DamageInstance", RpcTarget.All, Damage);
            }
            else
                DestroyPlayer();
        }
    }

    void DestroyPlayer()
    {
        PFInventory_.PlayerLose();
        PhotonNetwork.Destroy(gameObject);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Menu");
    }

    public float GetHealth()
    {
        return Health_;
    }


    [PunRPC]
    public void DamageInstance(int Damage)
    {
        bloodParticles.Play();
        Health_ -= Damage;
    }

    public void HealthUp(int HealtAdd)
    {
        photonView.RPC("HealthUpInstance", RpcTarget.All, HealtAdd);
    }

        [PunRPC]
    public void HealthUpInstance(int Health)
    {
        Health_ += Health;
    }
    private void FixedUpdate()
    {
        Health_Text_.text = Health_.ToString() + "HP";
    }

    public virtual void OnPhotonSerializeView(PhotonStream Stream, PhotonMessageInfo Info)
    {
        if (Stream.IsWriting)
        {
            Stream.SendNext(Health_);
        }
        else if (Stream.IsReading)
        {
            Health_ = (float)Stream.ReceiveNext();
        }
    }
}