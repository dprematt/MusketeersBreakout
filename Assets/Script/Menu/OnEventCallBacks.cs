using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class OnEventCallBacks : MonoBehaviourPunCallbacks
{
    public GameObject LobbyPanel_, RoomPanel_;
    public Text Room_;

    public RoomManager RoomItemPrefab_;
    List<RoomManager> RoomItemList_ = new List<RoomManager>();
    public Transform ContentObject_;
    public float TimeUpdate_ = 5f;
    float NextUpdateTime_;

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        LobbyPanel_.SetActive(true);
        Debug.Log("ConnectedToMaster Photon");
    }

    public override void OnJoinedRoom()
    {
        LobbyPanel_.SetActive(false);
        RoomPanel_.SetActive(true);
        Room_.text = "Room name : " + PhotonNetwork.CurrentRoom.Name;
        //PhotonNetwork.LoadLevel("Delivery");
        Debug.Log("Room joined : " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnRoomListUpdate(List<RoomInfo> RoomList)
    {
        if (Time.time >= NextUpdateTime_)
        {
            foreach (RoomManager item in RoomItemList_)
            {
                Destroy(item.gameObject);
            }
            RoomItemList_.Clear();

            foreach (RoomInfo room in RoomList)
            {
                if (room.RemovedFromList)
                {

                }
                else
                {
                    RoomManager newroom = Instantiate(RoomItemPrefab_, ContentObject_);
                    newroom.SetRoomName(room.Name);
                    RoomItemList_.Add(newroom);
                }
            }
            NextUpdateTime_ = Time.time + TimeUpdate_;
        }
        Debug.Log("Room List Update");
    }

    public override void OnLeftRoom()
    {
        RoomPanel_.SetActive(false);
        LobbyPanel_.SetActive(true);
    }
}
