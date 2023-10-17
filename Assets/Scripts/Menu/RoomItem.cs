using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{

    public Text Rooom;
    PlayFabManager manager;

    private void Start()
    {
        manager = FindObjectOfType<PlayFabManager>();
    }

    public void SetRoomName(string roomname)
    {
        Rooom.text = roomname;
    }

    public void OnClickItem()
    {
        manager.JoinRoom(Rooom.text);
    }
}