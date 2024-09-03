using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnWeapons : MonoBehaviour
{
    public GameObject BowPrefab_;
    public GameObject CrossBowPrefab_;
    public GameObject GunPrefab_;
    public GameObject SwordPrefab;
    public GameObject ShieldPrefab_;

    public GameObject Ennemy_;

    // Start is called before the first frame update
    public void InstanciateWeapons(Vector3 position)
    {
        if (PhotonNetwork.IsMasterClient)
        {  
            position.y -= 10;
            position.x += 10;
            PhotonNetwork.Instantiate(SwordPrefab.name, position, Quaternion.identity);
            position.z += 10;
            PhotonNetwork.Instantiate(SwordPrefab.name, position, Quaternion.identity);
            position.z += 10;
            PhotonNetwork.Instantiate(GunPrefab_.name, position, Quaternion.identity);
            position.z += 10;
            PhotonNetwork.Instantiate(GunPrefab_.name, position, Quaternion.identity);
            position.z += 10;
            PhotonNetwork.Instantiate(ShieldPrefab_.name, position, Quaternion.identity);
            position.z += 10;
            position.y += 10;
            PhotonNetwork.Instantiate(Ennemy_.name, position, Quaternion.identity);
            position.z += 10;
            // PhotonNetwork.Instantiate(Ennemy_.name, position, Quaternion.identity);
            // position.z += 10;
            // PhotonNetwork.Instantiate(Ennemy_.name, position, Quaternion.identity);
            //Bow_Object_.name = "Bow";
        }
    }

    // Update is called once per frame
}
