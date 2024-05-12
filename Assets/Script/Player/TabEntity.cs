using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TabEntity : MonoBehaviour
{

    public GameObject Tabulation;


    // Start is called before the first frame update
    void Start()
    {

    }

    void OnKeyNPressed()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Vérifie si le GameObject est actif
            if (Tabulation.activeSelf)
            {
                // Désactive le GameObject s'il est actif
                Tabulation.SetActive(false);
            }
            else
            {
                // Active le GameObject s'il est désactivé
                Tabulation.SetActive(true);
            }
        }
    }

    void Update()
    {
        OnKeyNPressed();
    }
}