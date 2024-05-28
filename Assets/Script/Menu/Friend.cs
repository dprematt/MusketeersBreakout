using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Friend : MonoBehaviour
{
    // Start is called before the first frame update
    public Text usernameText;

    public Text state_;

    public void SetUsername(string username)
    {
        usernameText.text = username;
    }

    public void SetState(string state)
    {
        state_.text = state;
    }
}
