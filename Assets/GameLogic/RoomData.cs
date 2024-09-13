using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;


public class RoomData : MonoBehaviour
{
    // Start is called before the first frame update
    public string roomName;
    void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            // Get the current room name
            roomName = PhotonNetwork.CurrentRoom.Name;
            Debug.Log("Room Name: " + roomName);
        }
        else
        {
            Debug.Log("Not in a room");
        }
    }

    // Update is called once per frame

    public void DisplayPlayers(Photon.Realtime.Player[] players)
    {
        Debug.Log("Display Players");
        Transform Players = transform.Find("Players");

        if (Players != null)
        {
            Debug.Log("Display Players Players found");
            Transform RoomName = Players.Find("RoomName");
            if (RoomName != null)
            {
                Debug.Log("Display Players Room name != null");
                TextMeshProUGUI textComp = RoomName.GetComponent<TextMeshProUGUI>();
                if (textComp != null)
                {
                    Debug.Log("room name set");
                    textComp.text = roomName;
                }
                else
                {
                    Debug.Log("HUD: textComp == null");
                }
            }

            Transform playersUi = Players.Find("Players");
            if (playersUi != null)
            {
                Debug.Log("Display Players playersui != null");
                TextMeshProUGUI textComp = playersUi.GetComponent<TextMeshProUGUI>();
                if (textComp != null)
                {
                    Debug.Log("Display Players playersui textcomp != null");
                    foreach (Photon.Realtime.Player player in players)
                    {
                        Debug.Log("Display Players nickname = " + player.NickName);
                        textComp.text += ", " + player.NickName;
                    }
                }
                else
                {
                    Debug.Log("HUD: textComp == null");
                }
            }
        }
    }
    void Update()
    {
        // Get a list of all players in the room
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        // Loop through the players and print their Nicknames
        foreach (Photon.Realtime.Player player in players)
        {
            Debug.Log("Player: " + player.NickName);
        }
        DisplayPlayers(players);
    }
}
