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


    public void Take_Damage(int Damage)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("DamageInstance", RpcTarget.All, Damage);
        }
    }


    [PunRPC]
    public void DamageInstance(int Damage)
    {
        Health_ -= 10;

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