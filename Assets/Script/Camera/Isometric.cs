using UnityEngine;

public class Isometric : MonoBehaviour
{
    public float targetAngle = 45f;
    public float currentAngle = 0f;
    public float mouseSensitivity = 2f;
    public float rotationSpeed = 5f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");

        if (Input.GetMouseButton(2))
            targetAngle += mouseX * mouseSensitivity;

        currentAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(45, currentAngle, 0);
    }
}