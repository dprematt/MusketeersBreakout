using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class PlayFabManager : MonoBehaviourPunCallbacks
{

    public InputField Email_, Username_, Password_, RoomName_;
    public Text Room_;
    public GameObject LobbyPanel_, RoomPanel_;
    public RoomItem roomItemPrefab;
    List<RoomItem> roomItemList = new List<RoomItem>();
    public Transform contentObject;


    // Start de la classe
    void Start()
    {

    }

    //Fonction qui permet de register un user
    public void Register()
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = Email_.text,
            Username = Username_.text,
            Password = Password_.text,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, result =>
        {
            Debug.Log("Register success");
            PhotonNetwork.NickName = Username_.text;
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("After Connect");

        }, error =>
        {
            Debug.Log("Error while registering : " + error.ErrorMessage);
        });
    }

    //Fonction qui permet de login un user
    public void Login()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = Email_.text,
            Password = Password_.text,
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, result =>
        {
            Debug.Log("Login success");
        }, error =>
        {
            Debug.Log("Error while Loging : " + error.ErrorMessage);
        });
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("ConnectedToMaster Photon");
        PhotonNetwork.JoinLobby();
        LobbyPanel_.SetActive(true);
    }



    public void OnClickCreate()
    {
        if (RoomName_.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(RoomName_.text);
        }
    }

    public override void OnJoinedRoom()
    {
        LobbyPanel_.SetActive(false);
        RoomPanel_.SetActive(true);
        Room_.text = "Room name : " + PhotonNetwork.CurrentRoom.Name;
        //PhotonNetwork.LoadLevel("Delivery");
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
    }



    public override void OnRoomListUpdate(List<RoomInfo> roomlist)
    {
        foreach (RoomItem item in roomItemList)
        {
            Destroy(item.gameObject);
        }
        roomItemList.Clear();
        foreach (RoomInfo room in roomlist)
        {
            RoomItem newroom = Instantiate(roomItemPrefab, contentObject);
            newroom.SetRoomName(room.Name); // Utilisez la propriété Name pour obtenir le nom de la salle.
            roomItemList.Add(newroom);
        }
    }








}
