using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    private GameObject spawnedEntity;
    private GameObject spawnedEntity2;
    private GameObject spawnedEntity3;
    private List<int> weaponstypeList = new List<int>(); 
    private GameObject weapon1;
    public GameObject entityPrefab;
    public Material brownMaterial;
    public float spawnRange = 5.0f;
    private bool entitySpawned = false;
    private int life = 100;
    private float moveSpeed = 5.0f;
    private Vector3 entityScale = new Vector3(0.2f, 0.2f, 0.5f); 
    private Vector3 entityScale2 = new Vector3(0.1f, 0.1f, 0.2f); 
    private Vector3 entityScale3 = new Vector3(0.5f, 0.5f, 0.5f);
    private float WeaponType = 2;
    private int changeweapon = 0;
    private Vector3 randomPosition = new Vector3(0, 0, 0);
    public Vector3 randomPosition2 = new Vector3(1, 0, 0);
    public Vector3 randomPosition4 = new Vector3(10, 0, 0);
    public Vector3 WeapPos = new Vector3(0, 0, 0);
    public Vector3 WeapPos2 = new Vector3(0, 0, 0);
    public Vector3 randomPosition3 = new Vector3(999, 999, 999);
    private List<int> randomIntList = new List<int>();
    public bool WithWeapon = false;

    int x, y, z, x2, y2, z2 , x3 , y3 , z3  = 0;
    

    private bool there = false;
    private int eaten = 1;
    private int eaten2 = 1;
    private bool weap1, weap2 = false;

    private void Awake()
    {

        for (int i = 0; i < 3; i++)
            randomIntList.Add(UnityEngine.Random.Range(1, 3));
    }

    private void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);


        if (spawnedEntity != null)
        {
            spawnedEntity.transform.Translate(movement * moveSpeed * Time.deltaTime);
            Vector3 entityPosition = spawnedEntity.transform.position;
            Vector3 weaponPosition = spawnedEntity2.transform.position;
            Vector3 weaponPosition2 = spawnedEntity3.transform.position;

            // Convertir les coordonnées x, y et z en int.
            x = Mathf.RoundToInt(entityPosition.x);
            y = Mathf.RoundToInt(entityPosition.y);
            z = Mathf.RoundToInt(entityPosition.z);

            x2 = Mathf.RoundToInt(weaponPosition.x);
            y2 = Mathf.RoundToInt(weaponPosition.y);
            z2 = Mathf.RoundToInt(weaponPosition.z);

            x3 = Mathf.RoundToInt(weaponPosition2.x);
            y3 = Mathf.RoundToInt(weaponPosition2.y);
            z3 = Mathf.RoundToInt(weaponPosition2.z);


            WeapPos = new Vector3((float)x2, (float)y2, (float)z2);
            WeapPos2 = new Vector3((float)x3, (float)y3, (float)z3);

            if (weap1 == false && weap2 == true)
            {
                x2 = Mathf.RoundToInt(weaponPosition2.x);
                y2 = Mathf.RoundToInt(weaponPosition2.y);
                z2 = Mathf.RoundToInt(weaponPosition2.z);

                WeapPos = new Vector3((float)x3, (float)y3, (float)z3);

            }

            //UnityEngine.Debug.Log($"arme  x y z {x} {y} {z}");
            //UnityEngine.Debug.Log($"enemie x y z {x} {y} {z}");

            if (Vector3.Distance(entityPosition, weaponPosition) <= 1.0f && Input.GetKeyDown(KeyCode.X))
            {
                // Les positions sont proches.
                UnityEngine.Debug.Log("L'arme 1 a été ramasé.");
                weap1 = true;
                eaten = 0;
                if (weap2 == true)
                    eaten2 = -1;
            }

            if (Vector3.Distance(entityPosition, weaponPosition2) <= 1.0f && Input.GetKeyDown(KeyCode.X))
            {
                // Les positions sont proches.
                UnityEngine.Debug.Log($"111 L'arme 2 a été ramasé. {eaten}");
                eaten2 = 0;
                weap2 = true;
            }

            //UnityEngine.Debug.Log($"L'arme caca. {eaten}");

            //UnityEngine.Debug.Log($"caca eaten {eaten}");

            //else
            //{
            // Les positions ne sont pas proches.
            //    UnityEngine.Debug.Log("L'objet n'est pas proche de Vector3(1, 0, 0).");
            //}

            if (eaten == 0  && weap1 == true)
                spawnedEntity2.transform.Translate(movement * moveSpeed * Time.deltaTime);

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            //UnityEngine.Debug.Log($"R weap 1 R weap 2. {weap1} {weap2}");
            if (weap1 == true || weap2 == true) 
            {
                if (changeweapon >= randomIntList.Count - 1)
                    changeweapon = 0;
                else
                {
                    changeweapon = changeweapon + 1;
                }
            }
        }

        if (life > 0)
        {
            randomPosition = new Vector3(0, 0, 0);
        }
        else
        {
            randomPosition = new Vector3(999, 999, 999);
        }

        if ((!entitySpawned && (Input.GetKeyDown(KeyCode.P)) || ((Input.GetKeyDown(KeyCode.P) && life <= 0))))
        {
            float randomX = UnityEngine.Random.Range(-spawnRange, spawnRange);
            float randomY = 0.0f;
            float randomZ = UnityEngine.Random.Range(-spawnRange, spawnRange);


            Vector3 randomPosition2 = new Vector3(1, 0, 0);

            if (there == false)
                spawnedEntity = Instantiate(entityPrefab, randomPosition, Quaternion.identity);
                spawnedEntity2 = Instantiate(entityPrefab, randomPosition2, Quaternion.identity);
                spawnedEntity3 = Instantiate(entityPrefab, randomPosition4, Quaternion.identity);


            there = true;

            
            entitySpawned = true;
            life = 100;
        }


        if (weap1 == true)
        {
            if (randomIntList[changeweapon] == 1 && there == true)
                spawnedEntity2.transform.localScale = entityScale;
            else if (randomIntList[changeweapon] == 2 && there == true)
                spawnedEntity2.transform.localScale = entityScale2;

        }
        else
        {

            if (there == true)
                spawnedEntity2.transform.localScale = entityScale3;
        }

        if (weap2 == true)
        {
            if (randomIntList[changeweapon] == 1 && there == true)
                spawnedEntity3.transform.localScale = entityScale;
            else if (randomIntList[changeweapon] == 2 && there == true)
                spawnedEntity3.transform.localScale = entityScale2;
        }

        else
        {
            if (there == true) 
                spawnedEntity3.transform.localScale = entityScale3;
        }

        if (weap1 == true && weap2 == true)
        {
            eaten = 0;
            eaten2 = -1;
        }
        //spawnedEntity2.transform.position = WeapPos;

        if (weap1 == true && weap2 == false)
        {
            Vector3 entityPosition = spawnedEntity.transform.position;
            spawnedEntity2.transform.position = new Vector3(entityPosition.x + 1, entityPosition.y, entityPosition.z);
        }

        if (weap2 == true && weap1 == false)
        {
            Vector3 entityPosition = spawnedEntity.transform.position;
            spawnedEntity3.transform.position = new Vector3(entityPosition.x + 1, entityPosition.y, entityPosition.z);
        }

        if (eaten == -1)
            spawnedEntity2.transform.position = randomPosition3;
        
        if (eaten2 == -1)
            spawnedEntity3.transform.position = randomPosition3;

        // Sa supprime les 2

        //if (Input.GetKeyDown(KeyCode.Alpha9))
        //{
        //    life = 0;
        //    changeweapon = 0;

        //    if (spawnedEntity != null)
        //    {
        //        randomPosition = new Vector3(999, 999, 999);
        //        spawnedEntity.transform.position = randomPosition;
        //        spawnedEntity2.transform.position = randomPosition;
        //        spawnedEntity3.transform.position = randomPosition;


        //    }
        //}

        // Sa supprime que l'arme

        //if (Input.GetKeyDown(KeyCode.Alpha7))
        //{
        //    changeweapon = 0;

        //    if (spawnedEntity != null)
        //    {
        //        spawnedEntity2.transform.position = randomPosition;
            
        //    }
        //}

        // Sa supprime le perso

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            life = 0;
            changeweapon = 0;
            weap1 = false;
            eaten2 = 1;
            weap2 = false;

            if (spawnedEntity != null)
            {
                randomPosition = new Vector3(999, 999, 999);
                spawnedEntity.transform.position = randomPosition;
                eaten = 1;
            }
        }


        
        if (Input.GetKeyDown(KeyCode.V))
        {
            changeweapon = 0;
            eaten = 1;
            weap1 = false;
            eaten2 = 1;
            weap2 = false;
        }

        // Sa réafiche les 2

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {   
            if (spawnedEntity != null)
            {
                
                randomPosition = new Vector3(UnityEngine.Random.Range(-10, 10), 0,0);
                spawnedEntity.transform.position = randomPosition;
                spawnedEntity2.transform.position = WeapPos;

            }
        }
    }
}
