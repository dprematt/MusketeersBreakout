using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class OnEventCallBacksPlayer : MonoBehaviourPunCallbacks
{
    public Transform ContentObject_;
    public PlayerTabManager PlayerItemPrefab_;

    List<PlayerTabManager> PlayerItemList_ = new List<PlayerTabManager>();

    public float TimeUpdate_ = 1f;
    float NextUpdateTime_;

    private void Start()
    {
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
    }
}