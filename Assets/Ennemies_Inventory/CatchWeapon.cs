using System;
using UnityEngine;

public class CatchWeapon : MonoBehaviour
{
    private GameObject spawnedEntity;
    public GameObject entityPrefab;
    public Material brownMaterial;
    public float spawnRange = 5.0f;
    private bool entitySpawned = false;
    private int life = 100;
    private float moveSpeed = 5.0f; // Vitesse de déplacement du cylindre
    private Vector3 entityScale = new Vector3(0.5f, 0.5f, 0.5f); // Échelle de l'entité (ajustez selon vos besoins)

    private void Update()
    {
        // Gestion du mouvement
        //float moveHorizontal = Input.GetAxis("Horizontal"); // Obtenez l'entrée horizontale (gauche/droite)
        //float moveVertical = Input.GetAxis("Vertical"); // Obtenez l'entrée verticale (haut/bas)

        //Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical); // Créez un vecteur de mouvement en fonction des entrées

        Vector3 randomPosition = new Vector3(5, 0, 0);

        
        if (Input.GetKeyDown(KeyCode.N))
        {
            // Spawnez uniquement s'il n'y a pas déjà d'entité
            //float randomX = UnityEngine.Random.Range(-spawnRange, spawnRange);
            //float randomY = 0.0f; // Vous pouvez spécifier une hauteur Y fixe si nécessaire.
            //float randomZ = UnityEngine.Random.Range(-spawnRange, spawnRange);
            spawnedEntity = Instantiate(entityPrefab, randomPosition, Quaternion.identity);
            spawnedEntity.transform.localScale = entityScale;

            // Changez le matériau de l'entité spawnee en marron
            Renderer renderer = spawnedEntity.GetComponent<Renderer>();
            entitySpawned = true;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (spawnedEntity != null)
            {
                Destroy(spawnedEntity);
                spawnedEntity = null;
                entitySpawned = false;
            }
        }
    }
}
