using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class HealthManager : MonoBehaviourPunCallbacks
{

    float Health_ = 100;
    public Text Health_Text_;

    //public ParticleSystem bloodParticles;

    private void Start()
    {
    }


    public void Take_Damage(int Damage, GameObject Player)
    {
        if (photonView.IsMine)
        {
            if (Damage < Health_)
            {
                photonView.RPC("DamageInstance", RpcTarget.All, Damage);
                //photonView.RPC("SetPlayerHP", RpcTarget.All);
            }
            else
                DestroyPlayer(Player);
        }
    }

    void DestroyPlayer(GameObject Player)
    {
        // Détruire le GameObject localement
        Destroy(Player);

        // Détruire le GameObject de manière synchronisée sur le réseau
        PhotonNetwork.Destroy(Player);
    }

    public float GetHealth()
    {
        return Health_;
    }


    [PunRPC]
    public void DamageInstance(int Damage)
    {
        //bloodParticles.Play();
        Health_ -= Damage;
        Debug.Log("Damage Taken : " + Damage);
    }

    public void HealthUp(int HealtAdd)
    {
        Debug.Log(HealtAdd);
        Health_ += HealtAdd;
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
            Debug.Log("Received HP Other: " + Health_.ToString());


        }
    }
}