using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeactivateCamera : MonoBehaviour//PunCallBacks
{
    PhotonView view;
    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        if (!view.IsMine)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
