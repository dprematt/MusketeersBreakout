using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;


public class ExitButton : MonoBehaviourPun
{
    private PlayFabInventory Inventory_;

    public Text MuteButton;
    public GameObject VoiceManager;

    void Start()
    {
        MuteButton.text = "Mute";
    }

    public void ToggleMute()
    {
        if(VoiceManager.activeSelf) {
            VoiceManager.SetActive(false);
            MuteButton.text = "Sound";
        }
        else {
            VoiceManager.SetActive(true);
            MuteButton.text = "Mute";
        }
    }
}
