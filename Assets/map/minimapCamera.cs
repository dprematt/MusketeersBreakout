using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class minimapCamera : MonoBehaviourPunCallbacks
{
    public Camera camera; // Minimap camera
    public Camera playerCamera; // Référence à la caméra principale du joueur
    public SpriteRenderer playerIcon; // L'icône sur la minimap représentant le joueur
    public float cameraHeight = 50f; // Hauteur de la caméra minimap au-dessus du joueur

    void Start()
    {
        if (!photonView.IsMine)
        {
            camera.gameObject.SetActive(false);
            
            // L'icône des autres joueurs est mise en rouge (ou autre couleur si souhaité)
            playerIcon.color = new Color(1f, 0f, 0f); // Rouge pour les autres joueurs
            return;
        } 
        else 
        {
            // Si c'est le joueur local, l'icône est verte
            playerIcon.color = new Color(0f, 1f, 0f); // Vert pour le joueur local
        }

        if (camera == null)
        {
            Debug.LogError("MinimapCamera n'est pas assignée.");
        }
        else
        {
            Debug.Log("MinimapCamera assignée pour " + gameObject.name);
        }

        // Cherche la caméra principale du joueur si elle n'est pas déjà assignée
        if (playerCamera == null)
        {
            playerCamera = Camera.main; // Assigne automatiquement la caméra principale
        }

        // Activer la caméra minimap pour le joueur local
        camera.gameObject.SetActive(true);
        Debug.Log("MinimapCamera activée pour " + gameObject.name);
    }

    void LateUpdate()
    {
        if (!photonView.IsMine)
            return;

        if (camera != null)
        {
            // Met à jour la position de la minimap caméra directement au-dessus du joueur
            Vector3 newPos = transform.position;
            newPos.y = transform.position.y + cameraHeight;
            camera.transform.position = newPos;

            // Applique la rotation de la caméra principale à la minimap caméra
            if (playerCamera != null)
            {
                camera.transform.rotation = Quaternion.Euler(90f, playerCamera.transform.eulerAngles.y, 0f);
            }
            else
            {
                Debug.LogError("PlayerCamera n'est pas assignée.");
            }
        }
    }
}
