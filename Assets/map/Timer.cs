using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;  // Nécessaire pour les méthodes sur Room
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI time;
    public bool count;
    public int Time;

    ExitGames.Client.Photon.Hashtable setTime = new ExitGames.Client.Photon.Hashtable();
    private PhotonView photonView;

    void Start()
    {
        count = true;
    }

    void Update()
    {
            Time = (int)PhotonNetwork.CurrentRoom.CustomProperties["Time"];

            float minutes = Mathf.FloorToInt(Time / 60);
            float seconds = Mathf.FloorToInt(Time % 60);

            time.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (count)
            {
                count = false;
                StartCoroutine(timer());
            }
    }

    IEnumerator timer()
    {
        yield return new WaitForSeconds(1);
        if (PhotonNetwork.IsMasterClient)
        {
            int nextTime = Time - 1;

            setTime["Time"] = nextTime;
            PhotonNetwork.CurrentRoom.SetCustomProperties(setTime);
        }
        count = true;
    }
}