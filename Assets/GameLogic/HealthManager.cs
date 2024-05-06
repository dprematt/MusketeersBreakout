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
    Player player_;
    PlayFabInventory PFInventory_;

    public GameObject Player_;


    //public ParticleSystem bloodParticles;

    private void Start()
    {
        Player_ = GameObject.FindGameObjectWithTag("Player");
        PFInventory_ = GetComponent<PlayFabInventory>();
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
        Health_ -= 10; // Soustraire 10 à la valeur
        if (Health_ == 0) {
            DestroyPlayer(Player_);
        }
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
                DestroyPlayer(Player_);
        }
    }

    void DestroyPlayer(GameObject Player)
    {
        PFInventory_.PlayerLose();
        Destroy(Player);
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
    }

    public void HealthUp(int HealtAdd)
    {
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
        }
    }
}