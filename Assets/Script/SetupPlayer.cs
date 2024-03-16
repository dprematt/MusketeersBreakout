using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupPlayer : MonoBehaviour
{
    public Player Move_;
    public GameObject Camera_;
    // Start is called before the first frame update

    public void IsLocalPlayer()
    {
        Move_.enabled = true;
        Camera_.SetActive(true);
    }
}
