using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class RoomManager : MonoBehaviour
{
    public Text Room_;
    PhotonManager Manager_;
    public InputField RoomName_;

    private void Start()
    {
        Manager_ = FindObjectOfType<PhotonManager>();

        if (Manager_ == null)
        {
            Debug.LogError("PhotonManager not found in the scene.");
        }
        else
        {
            Debug.Log("PhotonManager found.");
        }
    }

    public void SetRoomName(string RoomName)
    {
        Room_.text = RoomName;
    }

    public void OnClickRoomItem()
    {
        Manager_.JoinRoom(Room_.text);
    }

    public void OnClickCreateRoom()
    {
        Manager_.CreateRoom(RoomName_.text);
    }

    public void OnClickLeaveRoom()
    {
        Manager_.LeaveRoom();
    }
}