using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    public int coinsCollected = 0;

    PhotonView view;

    // Début de la classe
    private void Start()
    {
        view = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Acutalisation de la classe
    private void Update()
    {
        if (view.IsMine)
        {
            // Déplacement du joueur
            float horizontalMovement = Input.GetAxis("Horizontal");
            float verticalMovement = Input.GetAxis("Vertical");
            Vector2 moveDirection = new Vector2(horizontalMovement, verticalMovement).normalized;
            rb.velocity = moveDirection * moveSpeed;

            // Détecter la collision avec les pièces
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Coin"))
                {
                    CollectCoin(collider.gameObject);
                    UpdateCoinsOnPlayFab();
                }
            }
        }
        
    }

    //Fonction qui permet de récolter un "Coin", en incrémentant la variable "CoinsCollected" et en supprimant l'objet touché
    public void CollectCoin(GameObject coin)
    {
        Destroy(coin);
        coinsCollected++;
        Debug.Log("okok : " + coinsCollected.ToString());
    }

    //Fonction qui permet de mettre sur le compte playfab du join le nombre de "Coin" récupérés 
    private void UpdateCoinsOnPlayFab()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"Coins", coinsCollected.ToString()}
            }
        };

        PlayFabClientAPI.UpdateUserData(request, result =>
        {
            Debug.Log("Coins updated on PlayFab");
        }, error =>
        {
            Debug.LogError("Error updating coins on PlayFab: " + error.ErrorMessage);
        });
    }
}
