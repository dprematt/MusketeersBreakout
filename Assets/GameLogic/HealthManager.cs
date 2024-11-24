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

    public float Health_ = 100;
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
        Debug.Log("take damage in health manager before is mine check");
        if (photonView.IsMine)
        {
            Debug.Log("take damage in health manager");
            if (Damage < Health_)
            {
                Debug.Log("damage < health");
                Player player = gameObject.GetComponent<Player>();
                if (player != null)
                {
                    Debug.Log("player != null");
                    player.dmgTaken += Damage;
                }
                Debug.Log("call to rpc damage instance");
                photonView.RPC("DamageInstance", RpcTarget.All, Damage);
            }
            else {
                Debug.Log("transfer to host");
                TransferToNextPlayer();
                DestroyPlayer();
            }
        }
    }

    void DestroyPlayer()
    {
        PFInventory_.PlayerLose();
        PhotonNetwork.Destroy(gameObject);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Menu");
    }


    public void TransferMasterClient(Photon.Realtime.Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Vérifie si le joueur cible est bien connecté
            if (newMasterClient != null && newMasterClient != PhotonNetwork.MasterClient)
            {
                PhotonNetwork.SetMasterClient(newMasterClient);
                Debug.Log($"Le nouveau Master Client est : {newMasterClient.NickName}");
            }
            else
            {
                Debug.LogWarning("Le joueur spécifié n'est pas valide ou est déjà Master Client.");
            }
        }
        else
        {
            Debug.LogWarning("Seul le Master Client actuel peut transférer ce rôle.");
        }
    }

    // Exemple d'utilisation pour transférer au prochain joueur de la liste
    public void TransferToNextPlayer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
            int currentMasterIndex = System.Array.IndexOf(players, PhotonNetwork.MasterClient);
            int nextPlayerIndex = (currentMasterIndex + 1) % players.Length;
            TransferMasterClient(players[nextPlayerIndex]);
        }
    }


    public float GetHealth()
    {
        return Health_;
    }


    [PunRPC]
    public void DamageInstance(int Damage)
    {
        Debug.Log("in damage instance");
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