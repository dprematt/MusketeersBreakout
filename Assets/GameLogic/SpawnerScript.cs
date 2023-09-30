using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject cylinderPrefab;
    private int life { get; set; }
    private float pos_x { get; set; }
    private float pos_y { get; set; }
    private float pos_z { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        life = 10;
        pos_x = 10;
        pos_y = 20;
        pos_z = 10;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Instantiate(cylinderPrefab, new Vector3(pos_x, pos_y, pos_z), Quaternion.identity);
        }
    }
}
