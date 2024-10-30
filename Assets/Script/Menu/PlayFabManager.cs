using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using System;


public class PlayFabManager : MonoBehaviourPunCallbacks
{

    public InputField Email_, Username_, Password_;

    public GameObject BoutonPlay_, BoutonInscription_, BoutonConnexion_, ObjectBienvenue_, BoutonDeconnexion_, ErrorGO_;

    public GameObject Login_, Register_, Welcome_;
    public Text Bienvenue_;

    public Text Error_;

    //Fonction qui permet de register un user

    void Start()
    {
        // S'abonner à l'événement onValueChanged
        Username_.onValueChanged.AddListener(OnTextChanged);
        
    }

    void Update()
    {
        // Vérifie si la touche Entrée est pressée et si un bouton est actuellement sélectionné
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (Login_.activeSelf) {
                Login();
            } else if (Register_.activeSelf) {
                Register();
            }
        }
    }

    public void Register()
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = Email_.text,
            Username = Username_.text.ToLower(),
            Password = Password_.text,
            RequireBothUsernameAndEmail = false
        };

        Username_.text = "";
        Password_.text = "";
        Email_.text = "";

        PlayFabClientAPI.RegisterPlayFabUser(request, result =>
        {
            Debug.Log("Register success");
            GetPlayerUsername(result.PlayFabId);
            Register_.SetActive(false);
            Welcome_.SetActive(true);
        }, error =>
        {
            Debug.Log("Error while registering : " + error.ErrorMessage);
            ErrorGO_.SetActive(true);
            Error_.text = error.ErrorMessage;
        });
    }

    //Fonction qui permet de login un user
    public void Login()
    {
        String email = Email_.text;
        String password = Password_.text;

        Email_.text = "";
        Password_.text = "";

        PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        }, OnLoginSuccess, OnLoginFailure);
        
    }

    public void Login_Admin()
    {
        PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest
        {
            Email = "okayokay999@gmail.com",
            Password = "okayokay999"
        }, OnLoginSuccess, OnLoginFailure);
    }

    public void OnLoginSuccess(LoginResult result)
    {
        // Access the PlayFabID from the authentication result
        string playFabId = result.PlayFabId;
        Debug.Log("PlayFab ID: " + playFabId);
        Login_.SetActive(false);
        Welcome_.SetActive(true);

        // After getting the PlayFab ID, you can proceed to get the player's username
        GetPlayerUsername(playFabId);
    }

    public void OnLoginFailure(PlayFabError error)
    {
        ErrorGO_.SetActive(true);
        Error_.text = error.ErrorMessage;
        Debug.LogError("Login failed: " + error.GenerateErrorReport());
    }

    // Get the player's username using the PlayFabID
    public void GetPlayerUsername(string playFabId)
    {
        PlayerPrefs.SetString("playfabID", playFabId);
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
            PhotonNetwork.NickName = username;
            PhotonNetwork.ConnectUsingSettings();
            ObjectBienvenue_.SetActive(true);
            BoutonConnexion_.SetActive(false);
            BoutonInscription_.SetActive(false);
            BoutonPlay_.SetActive(true);
            BoutonDeconnexion_.SetActive(true);
            Bienvenue_.text = "Welcome " + username + " !!!";
            Debug.Log("Username: " + username);
            SaveStatus();
        }
    }

    private void OnGetAccountInfoFailure(PlayFabError error)
    {
        Debug.LogError("GetAccountInfo request failed: " + error.GenerateErrorReport());
        ErrorGO_.SetActive(true);
        Error_.text = error.GenerateErrorReport();
    }

    public void OnClickDeconnexion()
    {
        PhotonNetwork.Disconnect();
        PlayFabClientAPI.ForgetAllCredentials();
        BoutonDeconnexion_.SetActive(false);
        BoutonInscription_.SetActive(true);
        BoutonConnexion_.SetActive(true);
        ObjectBienvenue_.SetActive(false);
        BoutonPlay_.SetActive(false);
    }

    public void SaveStatus()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new System.Collections.Generic.Dictionary<string, string>
            {
                { "Status", "online" },

            },
            Permission = UserDataPermission.Public
        };

        PlayFabClientAPI.UpdateUserData(request, OnVariableEnregistree, OnPlayFabError);
    }

    private void OnVariableEnregistree(UpdateUserDataResult result)
    {
    }

    private void OnPlayFabError(PlayFabError error)
    {
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        // Expression régulière pour vérifier une adresse e-mail
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }

    public void SendPasswordRecovery()
    {
        if (IsValidEmail(Email_.text))
        {

            var request = new SendAccountRecoveryEmailRequest
            {
                Email = Email_.text,
                TitleId = PlayFabSettings.TitleId // Assurez-vous d'avoir configuré votre Title ID
            };

            PlayFabClientAPI.SendAccountRecoveryEmail(request, OnRecoveryEmailSent, OnError);
        } else {
            ErrorGO_.SetActive(true);
            Error_.text = "Error sending mail, verify that you correctly put the email address";
        }
    }

    private void OnRecoveryEmailSent(SendAccountRecoveryEmailResult result)
    {
        Debug.Log("Email de récupération envoyé avec succès !");
    }

    private void OnError(PlayFabError error)
    {
        ErrorGO_.SetActive(true);
        Error_.text = error.GenerateErrorReport();
        Debug.LogError("Login failed: " + error.GenerateErrorReport());
    }

    void OnTextChanged(string text)
    {
        Username_.text = text.ToLower();
    }

    void OnApplicationQuit()
    {

        var request = new UpdateUserDataRequest
        {
            Data = new System.Collections.Generic.Dictionary<string, string>
            {
                { "Status", "offline" },

            },
            Permission = UserDataPermission.Public
        };

        PlayFabClientAPI.UpdateUserData(request, OnVariableEnregistree, OnPlayFabError);
        
    }

}

