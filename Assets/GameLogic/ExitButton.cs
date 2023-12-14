using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ExitButton : MonoBehaviour
{
    private PlayFabInventory Inventory_;

    private void Start()
    { 
        
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
