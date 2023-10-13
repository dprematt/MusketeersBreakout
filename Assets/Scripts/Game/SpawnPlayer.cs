using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject Prefab;

    public float minx;
    public float maxx;
    public float miny;
    public float maxy;


    private void Start()
    {
        Vector2 radom = new Vector2(Random.Range(minx, maxx), Random.Range(miny, maxy));
        PhotonNetwork.Instantiate(Prefab.name, radom, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
