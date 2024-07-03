using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using UnityEngine;

public class DisplayStats : MonoBehaviour
{
    public Text textComponent;
    // Start is called before the first frame update
    void Start()
    {
        if (textComponent != null)
        {
            textComponent.text = "Hello, World!"; // Set the text
        }
        else
        {
            Debug.LogError("Text component is not assigned.");
        }
    }
    void Update()
    {
        
    }
}
