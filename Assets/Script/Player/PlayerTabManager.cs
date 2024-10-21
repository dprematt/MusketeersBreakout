using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class PlayerTabManager : MonoBehaviourPun
{
    public Text PlayerName_;
    //public Text PlayerHealth_;

    

    //HealthManager HealthManager_;

    private void Start()
    {
        //HealthManager_ = GetComponent<HealthManager>();
    }


    public void SetPlayerName(string PlayerName)
    {
        PlayerName_.text = PlayerName;
    }

    public void SetPlayerHP()
    {
        //PlayerHealth_.text = HealthManager_.GetHealth().ToString() + "HP";
    }

    //[PunRPC]
    //public void TakeDamage(float damageAmount)
    //{
    //    Debug.Log("Dmg : " + damageAmount + "   Health : " + Health_);
    //    Debug.Log("Joueur : " + PhotonNetwork.LocalPlayer.NickName);
    //    Health_ -= damageAmount;
    //    SetPlayerHP();
    //    Debug.Log("\n\n\nInTakeDamage\n\n\n");
    //    if (Health_ <= 0)
    //    {
    //        Destroy(gameObject);
    //        if (photonView.IsMine)
    //        {
    //            PhotonNetwork.LoadLevel("Menu");
    //        }
    //    }

    //}
}