using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using PlayFab;
using PlayFab.ClientModels;

public class PhotonManager : MonoBehaviour
{

    private int Nb_max;
    public Text Players;
    public Text Time;

    public GameObject ObjectBienvenue_;
    public GameObject BoutonConnexion_;
    public GameObject BoutonInscription_;
    public GameObject BoutonPlay_;
    public GameObject BoutonDeconnexion_;

    public GameObject LobbyPannel_;

    public GameObject Welcome_;
    public Text Bienvenue_;

    private void Start()
    {
        Nb_max = 2;
        Players.text = Nb_max.ToString();

        if (PlayFabClientAPI.IsClientLoggedIn() && PhotonNetwork.IsConnected) {
            Debug.Log("Letsgooooo    " + PhotonNetwork.NickName);
            PhotonNetwork.LeaveRoom();
            ObjectBienvenue_.SetActive(true);
            BoutonConnexion_.SetActive(false);
            BoutonInscription_.SetActive(false);
            BoutonPlay_.SetActive(true);
            BoutonDeconnexion_.SetActive(true);
            Bienvenue_.text = "Welcome " + PhotonNetwork.NickName + " !!!";
            Welcome_.SetActive(false);
            LobbyPannel_.SetActive(true);
            Debug.Log("Username: " + PhotonNetwork.NickName);
        }
    }

    public void Plus()
    {
        Nb_max++;
        Players.text = Nb_max.ToString();
    }

    public void Moins()
    {
        Nb_max--;
        Players.text = Nb_max.ToString();
    }

    public void JoinRoom(string Name)
    {
        PhotonNetwork.JoinRoom(Name);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

public void CreateRoom(string Name)
{
    RoomOptions roomOptions = new RoomOptions { MaxPlayers = (byte)Nb_max };
    roomOptions.CleanupCacheOnLeave = false;

    if (Name.Length >= 1)
    {   
        int seed = new System.Random().Next();
        Hashtable options = new Hashtable()
        {
            { "mapSeed", seed },
            { "Time", 1800 },
            { "GameState", "en attente" },
            { "MaxPlayers", Nb_max } // Ajout de MaxPlayers comme propriété custom
        };

        roomOptions.CustomRoomProperties = options;
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "mapSeed", "Time", "GameState", "MaxPlayers" };

        PhotonNetwork.CreateRoom(Name, roomOptions);
    }
}


public void JoinRandomRoom()
{
    Hashtable expectedCustomRoomProperties = new Hashtable()
    {
        { "MaxPlayers", Nb_max },
        { "GameState", "en attente" } // Filtrer uniquement les rooms en attente
    };

    PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, (byte)Nb_max);
}







}
