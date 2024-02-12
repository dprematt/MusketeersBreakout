using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PhotonManager : MonoBehaviour
{
    private void Start()
    {
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
        if (Name.Length >= 1)
        {
            PhotonNetwork.CreateRoom(Name);
        }
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }






}
