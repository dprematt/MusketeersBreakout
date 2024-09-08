using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkManager : MonoBehaviour
{
    public GameObject shark;
    private GameObject upperShark;
    private GameObject lowerShark;
    private GameObject rightShark;
    private GameObject leftShark;
    public float speed = 15.0f;
    private bool movingUpperShark = true;
    private bool movingLowerShark = true;
    private bool movingRightShark = true;
    private bool movingLeftShark = true;


    void Start()
    {
        upperShark = Instantiate(shark, new Vector3(1, 0.1f, 580), Quaternion.identity);
        lowerShark = Instantiate(shark, new Vector3(1, 0.1f, -580), Quaternion.identity);
        rightShark = Instantiate(shark, new Vector3(580, 0.1f, 1), Quaternion.identity);
        leftShark = Instantiate(shark, new Vector3(-580, 0.1f, 1), Quaternion.identity);
        leftShark.transform.Rotate(0.0f, -90.0f, 0.0f);
        rightShark.transform.Rotate(0.0f, -90.0f, 0.0f);
       }

    void Update()
    {
        updateUpperShark();
        updateLowerShark();
        updateRightShark();
        updateLeftShark();
    }

    void updateUpperShark() {
        if (movingUpperShark)
        {
            upperShark.transform.Translate(Vector3.left * speed * Time.deltaTime);

            if (upperShark.transform.position.x <= -580)
            {
                movingUpperShark = false;
                FlipUpperShark();
            }
        }
        else
        {
            upperShark.transform.Translate(Vector3.right * speed * Time.deltaTime);

            if (upperShark.transform.position.x >= 580)
            {
                movingUpperShark = true;
                FlipUpperShark();
            }
        }
    }

    void updateLowerShark() {
        if (movingLowerShark)
        {
            lowerShark.transform.Translate(Vector3.left * speed * Time.deltaTime);

            if (lowerShark.transform.position.x <= -580)
            {
                movingLowerShark = false;
                FlipLowerShark();
            }
        }
        else
        {
            lowerShark.transform.Translate(Vector3.right * speed * Time.deltaTime);

            if (lowerShark.transform.position.x >= 580)
            {
                movingLowerShark = true;
                FlipLowerShark();
            }
        }
    }

    void updateLeftShark()
    {
        if (movingLeftShark)
        {
            leftShark.transform.Translate(Vector3.left * speed * Time.deltaTime);

            if (leftShark.transform.position.z <= -580)
            {
                movingLeftShark = false;
                FlipLeftShark();
            }
        }
        else
        {
            leftShark.transform.Translate(Vector3.right * speed * Time.deltaTime);

            if (leftShark.transform.position.z >= 580)
            {
                movingLeftShark = true;
                FlipLeftShark();
            }
        }
    }

    void updateRightShark()
    {
        if (movingRightShark)
        {
            rightShark.transform.Translate(Vector3.left * speed * Time.deltaTime);

            if (rightShark.transform.position.z <= -580)
            {
                movingRightShark = false;
                FlipRightShark();
            }
        }
        else
        {
            rightShark.transform.Translate(Vector3.right * speed * Time.deltaTime);

            if (rightShark.transform.position.z >= 580)
            {
                movingRightShark = true;
                FlipRightShark();
            }
        }
    }

    void FlipRightShark()
    {
        Vector3 scale = rightShark.transform.localScale;
        scale.z *= -1;
        rightShark.transform.localScale = scale;
        // rightShark.transform.Rotate(Vector3.up, 180.0f);
    }

    void FlipLeftShark()
    {
        Vector3 scale = leftShark.transform.localScale;
        scale.z *= -1;
        leftShark.transform.localScale = scale;
        // leftShark.transform.Rotate(Vector3.up, 180.0f);
    }

    void FlipUpperShark()
    {
        Vector3 scale = upperShark.transform.localScale;
        scale.x *= -1;
        upperShark.transform.localScale = scale;
    }

    void FlipLowerShark()
    {
        Vector3 scale = lowerShark.transform.localScale;
        scale.x *= -1;
        lowerShark.transform.localScale = scale;
    }
}
