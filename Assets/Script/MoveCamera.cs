using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float moveSpeed = 5f;

    public float moveHorizontal;
    public float moveVertical;
    public Vector3 movement;

    public void UpdateMovement()
    {
        movement = new Vector3(moveHorizontal, 0f, moveVertical).normalized;
        movement = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * movement;
    }

    void Update()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        UpdateMovement();

        transform.position += movement * moveSpeed * Time.deltaTime;
    }
}