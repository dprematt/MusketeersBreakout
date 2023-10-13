using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using Photon.Pun;

public class PlayFabManager : MonoBehaviourPunCallbacks
{

    public InputField Email_, Username_, Password_;


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

    public void create()
    {
        
        PhotonNetwork.CreateRoom("ok");
        Debug.Log("Create room");
    }

    public void join()
    {
        PhotonNetwork.JoinRoom("ok");
        Debug.Log("Join room");
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
        //
        //PhotonNetwork.JoinRoom("ok");
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby joined");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created !");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Delivery");
        Debug.Log("Load level");
    }
}
