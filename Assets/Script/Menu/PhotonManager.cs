using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviour
{

    private int Nb_max;
    public Text Players;

    private void Start()
    {
        Nb_max = 2;
        Players.text = Nb_max.ToString();
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
        if (Name.Length >= 1)
        {
            PhotonNetwork.CreateRoom(Name, roomOptions);
        }
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }






}
