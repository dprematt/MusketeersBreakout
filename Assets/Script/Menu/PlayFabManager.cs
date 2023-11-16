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

    public InputField Email_, Username_, Password_;

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
            PhotonNetwork.NickName = Username_.text;
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Register success");

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


    

    






}
