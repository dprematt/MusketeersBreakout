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

        PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest
        {
            Email = Email_.text,
            Password = Password_.text
        }, OnLoginSuccess, OnLoginFailure);
        //var request = new LoginWithEmailAddressRequest
        //{
        //    Email = Email_.text,
        //    Password = Password_.text,
        //};

        //PlayFabClientAPI.LoginWithEmailAddress(request, result =>
        //{
        //    //PhotonNetwork.NickName = Username_.text;
        //    //PhotonNetwork.ConnectUsingSettings();
        //    Debug.Log("Login success");
        //}, error =>
        //{
        //    Debug.Log("Error while Loging : " + error.ErrorMessage);
        //});
    }

    public void OnLoginSuccess(LoginResult result)
    {
        // Access the PlayFabID from the authentication result
        string playFabId = result.PlayFabId;
        Debug.Log("PlayFab ID: " + playFabId);

        // After getting the PlayFab ID, you can proceed to get the player's username
        GetPlayerUsername(playFabId);
    }

    public void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Login failed: " + error.GenerateErrorReport());
    }

    // Get the player's username using the PlayFabID
    public void GetPlayerUsername(string playFabId)
    {
        GetAccountInfoRequest request = new GetAccountInfoRequest
        {
            PlayFabId = playFabId
        };

        PlayFabClientAPI.GetAccountInfo(request, OnGetAccountInfoSuccess, OnGetAccountInfoFailure);
    }

    private void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        if (result.AccountInfo != null && result.AccountInfo.Username != null)
        {
            string username = result.AccountInfo.Username;
            Debug.Log("Username: " + username);
        }
    }

    private void OnGetAccountInfoFailure(PlayFabError error)
    {
        Debug.LogError("GetAccountInfo request failed: " + error.GenerateErrorReport());
    }
}

