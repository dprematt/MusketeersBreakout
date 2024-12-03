using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ChatManager : MonoBehaviour
{

    public GameObject Friend;
    public GameObject Chat;

    public InputField inputField;

    public GameObject LMessage;
    public GameObject MyLMessage; 
    
    public GameObject MMessage;
    public GameObject MyMMessage; 

    public GameObject BMessage;
    public GameObject MyBMessage; 

    public GameObject Content;
    public ScrollRect scrollRect;
    // Start is called before the first frame update

    void Update()
    {
        // Vérifie si la touche Entrée est pressée et si un bouton est actuellement sélectionné
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (gameObject.activeSelf) {
                SendMessage();
            }
        }
        if (!PhotonNetwork.InRoom) {
            if (Chat.activeSelf)
                Chat.SetActive(false);
        }

    }
    public void SendMessage()
    {
        if (inputField.text.Length <= 0)
            return;
        GetComponent<PhotonView>().RPC("GetMessage", RpcTarget.All, PhotonNetwork.NickName, inputField.text);
        inputField.text = "";
    }

    [PunRPC]
    public void GetMessage(string name, string ReceiveMessage)
    {
        // Vérifie si le message provient de l'utilisateur local
        GameObject M = null;

        if (name == PhotonNetwork.NickName)
        {
            // Instancier "MyMessage" si le message est envoyé par l'utilisateur local
            if (ReceiveMessage.Length <= 30) {
                M = Instantiate(MyLMessage, Vector3.zero, Quaternion.identity, Content.transform);
            }
            else if (ReceiveMessage.Length <= 90) {
                M = Instantiate(MyMMessage, Vector3.zero, Quaternion.identity, Content.transform);
            }
            else {
                M = Instantiate(MyBMessage, Vector3.zero, Quaternion.identity, Content.transform);
            }
        }
        else
        {
            if (ReceiveMessage.Length <= 23) {
                M = Instantiate(LMessage, Vector3.zero, Quaternion.identity, Content.transform);
            }
            else if (ReceiveMessage.Length <= 90) {
                M = Instantiate(MMessage, Vector3.zero, Quaternion.identity, Content.transform);
            }
            else {
                M = Instantiate(BMessage, Vector3.zero, Quaternion.identity, Content.transform);
            }
        }
        M.transform.SetParent(Content.transform, false);
        M.transform.SetAsFirstSibling();
        M.GetComponent<Chat>().Message.text = ReceiveMessage;
        M.GetComponent<Chat>().Name.text = name;
    }

    public void OnClick()
    {
        if (Friend.activeSelf)
            Friend.SetActive(false);
        if (Chat.activeSelf)
            Chat.SetActive(false);
        else
            Chat.SetActive(true);
    }
}
