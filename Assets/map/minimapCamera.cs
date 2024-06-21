using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class minimapCamera : MonoBehaviourPunCallbacks
{
    public Camera camera;
    public float cameraHeight = 50f; // The height at which the camera will be placed above the player

    void Start()
    {
        if (!photonView.IsMine)
        {
            camera.gameObject.SetActive(false);
            return;
        }

        if (camera == null)
        {
            Debug.LogError("MinimapCamera n'est pas assignée.");
        }
        else
        {
            Debug.Log("MinimapCamera assignée pour " + gameObject.name);
        }

        // Ensure the camera is enabled for the local player
        camera.gameObject.SetActive(true);
        Debug.Log("MinimapCamera activée pour " + gameObject.name);
    }

    void LateUpdate()
    {
        if (!photonView.IsMine)
            return;

        if (camera != null)
        {
            // Set the camera position directly above the player
            Vector3 newPos = transform.position;
            newPos.y = transform.position.y + cameraHeight;
            camera.transform.position = newPos;
            camera.transform.rotation = Quaternion.Euler(90f, transform.eulerAngles.y, 0f);
            Debug.Log("MinimapCamera position mise à jour pour " + gameObject.name + " à " + newPos);
        }
    }
}
