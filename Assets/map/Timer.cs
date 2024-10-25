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

    void Start()
    {
        count = true;
    }

    void Update()
    {
            // Time = (int)PhotonNetwork.CurrentRoom.CustomProperties["Time"];
        // Vérifiez si la salle existe pour éviter les erreurs null
        // if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Time"))
        // {
            // Récupérer la valeur actuelle de "Time" dans les propriétés de la salle
            Time = (int)PhotonNetwork.CurrentRoom.CustomProperties["Time"];
            Debug.Log("ZIZI " + Time);

            // Calculer les minutes et secondes pour l'affichage
            float minutes = Mathf.FloorToInt(Time / 60);
            float seconds = Mathf.FloorToInt(Time % 60);

            // Mettre à jour le texte
            time.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Démarrer le timer s'il n'est pas déjà en cours
            if (count)
            {
                count = false;
                StartCoroutine(timer());
            }
        // }
    }

    IEnumerator timer()
    {
        yield return new WaitForSeconds(1);
        int nextTime = Time - 1;

        setTime["Time"] = nextTime;
        PhotonNetwork.CurrentRoom.SetCustomProperties(setTime);

        count = true;
    }
}
