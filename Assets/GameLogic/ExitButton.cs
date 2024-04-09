using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class ExitButton : MonoBehaviourPun
{
    private PlayFabInventory Inventory_;

    public GameObject VoiceManager;

    void Start()
    {
    }

    public void ToggleMute()
    {
        if(VoiceManager.activeSelf)
            VoiceManager.SetActive(false);
        else
            VoiceManager.SetActive(true);
    }
    public void Exit()
    {
        Inventory_ = GetComponent<PlayFabInventory>();
        Debug.Log("Player Exit");
        Inventory_.PlayerWin();
        // Quitter la room
        PhotonNetwork.LeaveRoom();

        // Charger la nouvelle scène
        PhotonNetwork.LoadLevel("Menu");
    }
}
