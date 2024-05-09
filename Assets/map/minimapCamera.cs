using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minimapCamera : MonoBehaviour
{
    public GameObject player;
    

    void Start()
    {
        player = gameObject.transform.parent.gameObject;

        if (player == null)
        {
            Debug.LogError("Impossible de trouver le joueur.s");
        }
        Debug.Log("PAYER = " + player.name);
    }

    void LateUpdate()
    {
        player = gameObject.transform.parent.gameObject;

        if (player == null)
            Debug.LogError("JOUEUR NUL");
        else if (player != null)
        {
            Vector3 newPos = player.transform.position;
            newPos.y = transform.position.y;
            transform.position = newPos;
            transform.rotation = Quaternion.Euler(90f, player.transform.eulerAngles.y, 0f);
        }
    }
}
