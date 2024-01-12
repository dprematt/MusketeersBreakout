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
            Vector3 position = new Vector3(-244, 1, 12);
            PhotonNetwork.Instantiate(BowPrefab_.name, position, Quaternion.identity);
            position = new Vector3(-252, 1, 12);
            PhotonNetwork.Instantiate(SpearPrefab_.name, position, Quaternion.identity);
            position = new Vector3(-247, 1, 12);
            PhotonNetwork.Instantiate(GunPrefab_.name, position, Quaternion.identity);
            position = new Vector3(-245, 1, 12);
            PhotonNetwork.Instantiate(CrossBowPrefab_.name, position, Quaternion.identity);
            //Bow_Object_.name = "Bow";
        }

    }

    // Update is called once per frame
}
