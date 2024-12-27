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
    
    private bool movingDown = true;
    private bool movingDownBis = true;

    void Start()
    {
        upperShark = Instantiate(shark, new Vector3(1, 0.1f, 340), Quaternion.identity);
        lowerShark = Instantiate(shark, new Vector3(1, 0.1f, -580), Quaternion.identity);
        rightShark = Instantiate(shark, new Vector3(340, 0.1f, 1), Quaternion.identity);
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

    private void RotateShark(GameObject shark)
        {
            shark.transform.rotation = Quaternion.Euler(0, shark.transform.rotation.eulerAngles.y + 180, 0);
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

            if (upperShark.transform.position.x >= 325)
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

            if (lowerShark.transform.position.x >= 325)
            {
                movingLowerShark = true;
                FlipLowerShark();
            }
        }
    }

    void updateLeftShark()
    {
        if (movingDown)
        {
            leftShark.transform.position += Vector3.back * speed * Time.deltaTime;
            if (leftShark.transform.position.z <= -580)
            {
                movingDown = false;
                RotateShark(leftShark);
            }
        }
        else
        {
            leftShark.transform.position += Vector3.forward * speed * Time.deltaTime;
            if (leftShark.transform.position.z >= 325)
            {
                movingDown = true;
                RotateShark(leftShark);
            }
        }
    }

    void updateRightShark()
    {
        if (movingDownBis)
        {
            rightShark.transform.position += Vector3.back * speed * Time.deltaTime;
            if (rightShark.transform.position.z <= -580)
            {
                movingDownBis = false;
                RotateShark(rightShark);
            }
        }
        else
        {
            rightShark.transform.position += Vector3.forward * speed * Time.deltaTime;
            if (rightShark.transform.position.z >= 325)
            {
                movingDownBis = true;
                RotateShark(rightShark);
            }
        }
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
