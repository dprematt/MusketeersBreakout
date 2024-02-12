using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class MasterClientManager : MonoBehaviourPun
{
    public Transform ContentObject_;
    public PlayerTabManager PlayerItemPrefab_;
    public Text Waiting;

    List<PlayerTabManager> PlayerItemList_ = new List<PlayerTabManager>();

    public float TimeUpdate_ = 1f;
    float NextUpdateTime_;

    private void Start()
    {
        UpdateWaitingMessage();
    }

    private void Update()
    {
        if (Time.time >= NextUpdateTime_)
        {
            DeletePlayerList();
            NextUpdateTime_ = Time.time + TimeUpdate_;
        }
    }

    public void DeletePlayerList()
    {
        foreach (PlayerTabManager Item in PlayerItemList_)
        {
            Destroy(Item.gameObject);
        }

        PlayerItemList_.Clear();
        AddPlayerList();
    }

    public void AddPlayerList()
    {

        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        float tmp;

        // Parcours la liste et imprime le nom de chaque joueur
        foreach (Photon.Realtime.Player player in players)
        {
            //tmp = Playertest.GetHP();
            PlayerTabManager NewPlayer_ = Instantiate(PlayerItemPrefab_, ContentObject_);
            NewPlayer_.SetPlayerName(player.NickName);
            PlayerItemList_.Add(NewPlayer_);
        }
        UpdateWaitingMessage();
    }

    public void UpdateWaitingMessage()
    {
        Waiting.text = "Waiting for " + (GetMaxPlayers() - GetCurrentPlayers()).ToString() + " players ...";
    }

    public int GetCurrentPlayers()
    {
        if (PhotonNetwork.InRoom)
        {
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            Debug.Log("Le nombre de joueurs dans la salle actuelle est : " + playerCount);
            return playerCount;
        }
        return -1;
    }

    public int GetMaxPlayers()
    {
        if (PhotonNetwork.InRoom)
        {
            int maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
            Debug.Log("Le nombre maximum de joueurs dans la salle est : " + maxPlayers);
            return maxPlayers;
        }
        return -1;
    }

}
