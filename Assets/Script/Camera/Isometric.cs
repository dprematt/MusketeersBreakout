using UnityEngine;

public class Isometric : MonoBehaviour
{
    public Transform target; // Le personnage à suivre
    public float targetAngle = 45f; // Angle cible de la caméra
    public float currentAngle = 0f; // Angle actuel de la caméra
    public float mouseSensitivity = 2f; // Sensibilité de la souris
    public float rotationSpeed = 5f; // Vitesse de rotation
    public float distance = 10f; // Distance de la caméra par rapport au personnage

    private Vector3 offset; // Décalage initial entre la caméra et le personnage

    void Start()
    {
        // Initialiser l'offset basé sur la position actuelle de la caméra
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

        // Calculer la nouvelle position de la caméra
        Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        // Déplacer la caméra vers la nouvelle position
        transform.position = desiredPosition;

        // Toujours regarder le personnage
        transform.LookAt(target.position);
    }
}
