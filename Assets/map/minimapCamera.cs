using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MinimapCamera : MonoBehaviourPunCallbacks
{
    public Camera minimapCamera;

    void Start()
    {
        if (!photonView.IsMine)
        {
            minimapCamera.gameObject.SetActive(false);
            return;
        }

        if (minimapCamera == null)
        {
            Debug.LogError("MinimapCamera n'est pas assignée.");
        }
        else
        {
            Debug.Log("MinimapCamera assignée pour " + gameObject.name);
        }
    }

    void LateUpdate()
    {
        if (!photonView.IsMine)
            return;

        if (minimapCamera != null)
        {
            Vector3 newPos = transform.position;
            newPos.y = minimapCamera.transform.position.y;
            minimapCamera.transform.position = newPos;
            minimapCamera.transform.rotation = Quaternion.Euler(90f, transform.eulerAngles.y, 0f);
        }
    }
}
