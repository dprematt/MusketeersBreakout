using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{

    public Text room;

    public void SetRoomName(string roomname)
    {
        room.text = roomname;
    }
}