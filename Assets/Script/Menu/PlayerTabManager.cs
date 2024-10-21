using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerTabManager : MonoBehaviourPunCallbacks
{
    public Text PlayerName_;
    public GameObject ExcludeButton;

    public GameObject Crown_;

    private void Start()
    {
        PhotonNetwork.EnableCloseConnection = true;
    }

    public void SetPlayerName(string PlayerName)
    {
        PlayerName_.text = PlayerName;
    }

    public void DisplayPlayerManager()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (ExcludeButton.activeSelf)
            {
                ExcludeButton.SetActive(false);
            }
            else
            {
                ExcludeButton.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Not able to display PlayerManager because not MasterClient !");
        }
    }
    public void Master()
    {
        Crown_.SetActive(true);
    }

    public void ExcludePlayer()
    {
        PhotonNetwork.CloseConnection(GetPlayerByName());
    }

    public Player GetPlayerByName()
    {

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == PlayerName_.text)
            {
                return player;
            }
        }
        Debug.Log("Problème avec GetPlayerByName");
        return null;
    }

}
