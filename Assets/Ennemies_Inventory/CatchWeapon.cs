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
    private float moveSpeed = 5.0f; // Vitesse de d�placement du cylindre
    private Vector3 entityScale = new Vector3(0.5f, 0.5f, 0.5f); // �chelle de l'entit� (ajustez selon vos besoins)

    private void Update()
    {
        // Gestion du mouvement
        //float moveHorizontal = Input.GetAxis("Horizontal"); // Obtenez l'entr�e horizontale (gauche/droite)
        //float moveVertical = Input.GetAxis("Vertical"); // Obtenez l'entr�e verticale (haut/bas)

        //Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical); // Cr�ez un vecteur de mouvement en fonction des entr�es

        Vector3 randomPosition = new Vector3(5, 0, 0);

        
        if (Input.GetKeyDown(KeyCode.N))
        {
            // Spawnez uniquement s'il n'y a pas d�j� d'entit�
            //float randomX = UnityEngine.Random.Range(-spawnRange, spawnRange);
            //float randomY = 0.0f; // Vous pouvez sp�cifier une hauteur Y fixe si n�cessaire.
            //float randomZ = UnityEngine.Random.Range(-spawnRange, spawnRange);
            spawnedEntity = Instantiate(entityPrefab, randomPosition, Quaternion.identity);
            spawnedEntity.transform.localScale = entityScale;

            // Changez le mat�riau de l'entit� spawnee en marron
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
