using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class Friend : MonoBehaviour
{
    // Start is called before the first frame update
    public Text usernameText;

    FriendsManager manager;
    public GameObject Disconnected;
    public GameObject Connected;

    public void Start()
    {
        manager = FindObjectOfType<FriendsManager>();
    }

    

    public void OnClickDelete()
    {
        manager.OnClickRemoveButton(usernameText.text);
    }

    public void SetUsername(string username)
    {
        usernameText.text = username;
    }

    public void SetState(string state)
    {
        if (state == "online") {
            Disconnected.SetActive(false);
            Connected.SetActive(true);
        } else {
            Disconnected.SetActive(true);
            Connected.SetActive(false);
        }
    }
}
