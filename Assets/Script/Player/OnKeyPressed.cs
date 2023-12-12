using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnKeyPressed : MonoBehaviour
{

    public GameObject Tabulation;
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        // Vérifie si la touche "F" est enfoncée
        if (Input.GetKeyDown(KeyCode.N))
        {
            // Vérifie si le GameObject est actif
            if (Tabulation.activeSelf)
            {
                // Désactive le GameObject s'il est actif
                Tabulation.SetActive(false);
                Debug.Log("GameObject désactivé.");
            }
            else
            {
                // Active le GameObject s'il est désactivé
                Tabulation.SetActive(true);
                Debug.Log("GameObject activé.");
            }
        }
    }
}