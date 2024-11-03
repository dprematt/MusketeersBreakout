using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TabStats : MonoBehaviour
{
    // Start is called before the first frame updatepublic GameObject Tabulation;
    public GameObject Tabulation;


    // Start is called before the first frame update
    void Start()
    {

    }

    void OnKeyPressed()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            // V�rifie si le GameObject est actif
            if (Tabulation.activeSelf)
            {
                // D�sactive le GameObject s'il est actif
                Tabulation.SetActive(false);
            }
            else
            {
                // Active le GameObject s'il est d�sactiv�
                Tabulation.SetActive(true);
            }
        }
    }

    void Update()
    {
        OnKeyPressed();
    }
}
