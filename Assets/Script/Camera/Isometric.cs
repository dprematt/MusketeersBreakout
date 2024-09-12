using UnityEngine;

public class Isometric : MonoBehaviour
{
    public Transform target; // Le personnage � suivre
    public float targetAngle = 45f; // Angle cible de la cam�ra
    public float currentAngle = 0f; // Angle actuel de la cam�ra
    public float mouseSensitivity = 2f; // Sensibilit� de la souris
    public float rotationSpeed = 5f; // Vitesse de rotation
    public float distance = 10f; // Distance de la cam�ra par rapport au personnage

    private Vector3 offset; // D�calage initial entre la cam�ra et le personnage

    void Start()
    {
        // Initialiser l'offset bas� sur la position actuelle de la cam�ra
        offset = transform.position - target.position;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");

        if (Input.GetMouseButton(2))
        {
            targetAngle += mouseX * mouseSensitivity;
        }

        // Interpolation pour une rotation fluide
        currentAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, rotationSpeed * Time.deltaTime);

        // Calculer la nouvelle position de la cam�ra
        Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        // D�placer la cam�ra vers la nouvelle position
        transform.position = desiredPosition;

        // Toujours regarder le personnage
        transform.LookAt(target.position);
    }
}
