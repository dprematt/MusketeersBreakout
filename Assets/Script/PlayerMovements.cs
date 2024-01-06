using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    float playerHeight = 2f;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float movementMultiplier = 10f;
    [SerializeField] float airMultiplier = 0.4f;

    [Header("Jumping")]
    public float jumpForce = 5f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;

    [Header("Drag")]
    [SerializeField] float groundDrag = 6f;
    [SerializeField] float airDrag = 2f;

    float horizontalMovement;
    float verticalMovement;

    bool isGrounded;

    Vector3 moveDirection;

    Rigidbody rb;

    private float originalHeight;
    public float heightModifier = 0.5f;
    public float minHeight = 0.5f;
    private Transform cylinderTransform;

    public float rotationSpeed = 50.0f;

    public Transform characterModel;

    public Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        cylinderTransform = transform;
        originalHeight = cylinderTransform.localScale.y;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.1f);

        MyInput();
        ControlDrag();

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        if (horizontalMovement != 0 || verticalMovement != 0)
            anim.SetBool("isWalking", true);
        else
            anim.SetBool("isWalking", false);

        // Utilisez la direction de la caméra pour le déplacement
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDirection = cameraForward * verticalMovement + cameraRight * horizontalMovement;

        // Calculez la rotation de l'objet "CharacterModel" pour qu'il fasse face à la direction de déplacement
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            characterModel.rotation = Quaternion.Slerp(characterModel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();

        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            float newHeight = originalHeight * heightModifier;
            newHeight = Mathf.Max(newHeight, minHeight);
            Vector3 newScale = cylinderTransform.localScale;
            newScale.y = newHeight;
            cylinderTransform.localScale = newScale;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && isGrounded)
        {
            float newHeight = originalHeight;
            Vector3 newScale = cylinderTransform.localScale;
            newScale.y = newHeight;
            cylinderTransform.localScale = newScale;
        }
        if (Input.GetKeyDown(KeyCode.F) && isGrounded)
        {
            Vector3 newPos = cylinderTransform.localPosition;
            newPos.z -= 1;
            cylinderTransform.localPosition = newPos;
        }
    }

    void MovePlayer()
    {
        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);

        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
    }
}
