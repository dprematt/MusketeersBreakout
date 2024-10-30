using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonManager : MonoBehaviour
{

    private int Nb_max;
    public Text Players;
    public Text Time;

    private void Start()
    {
        Nb_max = 2;
        Players.text = Nb_max.ToString();
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Plus()
    {
        Nb_max++;
        Players.text = Nb_max.ToString();
    }

    public void Moins()
    {
        Nb_max--;
        Players.text = Nb_max.ToString();
    }

    public void JoinRoom(string Name)
    {
        PhotonNetwork.JoinRoom(Name);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void CreateRoom(string Name)
    {
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = (byte)Nb_max };
        roomOptions.CleanupCacheOnLeave = false;
        if (Name.Length >= 1)
        {   
            int seed = new System.Random().Next();
            Hashtable options = new Hashtable()
            {
                { "mapSeed", seed },
                { "Time", 1800 },
            };


            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "mapSeed", seed }, {"Time", 1800}};
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "mapSeed", "Time" };

            PhotonNetwork.CreateRoom(Name, roomOptions);
        }
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }






}
