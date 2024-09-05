using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    // Start is called before the first frame update

    public Text Message;
    public Text Name;
    void Start()
    {
        GetComponent<RectTransform>().SetAsFirstSibling();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
