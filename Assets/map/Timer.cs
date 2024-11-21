using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;  // Nécessaire pour les méthodes sur Room
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
        try
        {
            if (PhotonNetwork.CurrentRoom == null)
            {
                Debug.LogWarning("Aucune salle active (PhotonNetwork.CurrentRoom est null).");
                return;
            }

            if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Time"))
            {
                Debug.LogWarning("La propriété 'Time' n'est pas définie dans CustomProperties.");
                return;
            }

            Time = (int)PhotonNetwork.CurrentRoom.CustomProperties["Time"];

            if (Time <= 0)
            {
                Debug.Log("Le temps est écoulé, déconnexion de la salle...");
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LoadLevel("Menu");
                return;
            }

            float minutes = Mathf.FloorToInt(Time / 60);
            float seconds = Mathf.FloorToInt(Time % 60);

            time.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (count)
            {
                count = false;
                StartCoroutine(TimerCoroutine());
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Erreur dans Update: {ex.Message}");
        }
    }

    IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(1);
        HandleTimerUpdate();
        count = true;
    }

    void HandleTimerUpdate()
    {
        try
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int nextTime = Time - 1;

                setTime["Time"] = nextTime;
                PhotonNetwork.CurrentRoom.SetCustomProperties(setTime);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Erreur dans HandleTimerUpdate: {ex.Message}");
        }
    }
}
