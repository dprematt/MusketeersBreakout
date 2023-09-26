using UnityEngine;

public class Isometric : MonoBehaviour
{
    public float targetAngle = 45f;
    public float currentAngle = 0f;
    public float mouseSensitivity = 2f;
    public float rotationSpeed = 5f;

    public void UpdateTargetAngle()
    {
        if (targetAngle < 0)
            targetAngle += 360;

        if (targetAngle > 360)
            targetAngle -= 360;

        //targetAngle = Mathf.Round(targetAngle / 45) * 45;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");

        if (Input.GetMouseButton(0))
            targetAngle += mouseX * mouseSensitivity;
        else
        {
            UpdateTargetAngle();
        }

        currentAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(30, currentAngle, 0);
    }
}