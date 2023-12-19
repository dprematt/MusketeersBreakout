using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnWeapons : MonoBehaviour
{
    public GameObject BowPrefab_;
    public GameObject CrossBowPrefab_;
    public GameObject GunPrefab_;
    public GameObject SpearPrefab_;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Vector3 position = new Vector3(-259, 9, 15);
            PhotonNetwork.Instantiate(BowPrefab_.name, position, Quaternion.identity);
            position = new Vector3(-254, 9, 15);
            PhotonNetwork.Instantiate(SpearPrefab_.name, position, Quaternion.identity);
            position = new Vector3(-261, 9, 15);
            PhotonNetwork.Instantiate(GunPrefab_.name, position, Quaternion.identity);
            position = new Vector3(-257, 9, 15);
            PhotonNetwork.Instantiate(CrossBowPrefab_.name, position, Quaternion.identity);
            //Bow_Object_.name = "Bow";
        }

    }

    // Update is called once per frame
}
