using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TabSounds : MonoBehaviour
{
        public GameObject Tabulation;
        public GameObject Tabulation2;
        public GameObject Tabulation3;


        // Start is called before the first frame update
        void Start()
        {

        }

        void OnKeyNPressed()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                // Vérifie si le GameObject est actif
                if (Tabulation.activeSelf)
                {
                    // Désactive le GameObject s'il est actif
                    Tabulation.SetActive(false);
                    Tabulation2.SetActive(false);
                    Tabulation3.SetActive(false);
                }
                else
                {
                    // Active le GameObject s'il est désactivé
                    Tabulation.SetActive(true);
                    Tabulation2.SetActive(true);
                    Tabulation3.SetActive(true);
                }
            }
        }

        void Update()
        {
            OnKeyNPressed();
        }
}
