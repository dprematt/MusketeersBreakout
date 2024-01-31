using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    public List<AudioClip> grassSound, woodSound;
    public AudioClip curSound;
    public float volMin, volMax, pitchMin, pitchMax;
    AudioSource audioS;

    void Start()
    {
        audioS = GetComponent<AudioSource>();
    }

    private void OnCollisionStay(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "grass":
                curSound = grassSound[Random.Range(0, grassSound.Count)];
                break;
            case "wood":
                curSound = woodSound[Random.Range(0, woodSound.Count)];
                break;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        curSound = null;
    }

    void Update()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (!audioS.isPlaying)
            {
                audioS.volume = Random.Range(volMin, volMax);
                audioS.pitch = Random.Range(pitchMin, pitchMax);
                if (curSound != null)
                {
                    audioS.PlayOneShot(curSound);
                }
            }
        }
    }
}
