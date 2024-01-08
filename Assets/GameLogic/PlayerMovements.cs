using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMovements : MonoBehaviourPunCallbacks
{
    float playerHeight = 2f;
    public int xp;
    public bool Demo = false;

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

    [SerializeField] float health, maxHealth = 10f;

    float Max_Health_ = 100;

    [SerializeField] private float maxStamina = 100f;
    private float stamina;

    private bool staminaFullUsed;
    private HealthManager HealthManager;
    private PlayFabInventory PFInventory_;

    public Inventory inventory;
    public GameObject HUD;

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
        HealthManager = GetComponent<HealthManager>();
        PFInventory_ = GetComponent<PlayFabInventory>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        health = maxHealth;
        xp = 0;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<Inventory>();
        Debug.Log(inventory);
        HUD = GameObject.FindGameObjectWithTag("InventoryHUD");
        HUD.GetComponent<HUD>().init();

        cylinderTransform = transform;
        originalHeight = cylinderTransform.localScale.y;
    }

    public void TakeDamage(float Damage)
    {
        if (photonView.IsMine)
        {
            if (HealthManager.GetHealth() <= Damage)
            {
                PFInventory_.PlayerLose();
            }

            HealthManager.Take_Damage((int)Damage, gameObject);
        }
    }

    public int UpdateXp(int new_xp)
    {
        xp += new_xp;
        return xp;
    }

    void OnCollisionEnter(Collision col)
    {
        Inventory loot = col.gameObject.GetComponent<Inventory>();
        if (loot != null)
        {
            loot.DisplayLoot(inventory);
            return;
        }
    }
    void OnTriggerEnter(Collider col)
    {
        Inventory loot = col.GetComponent<Inventory>();
        IInventoryItem item = col.GetComponent<IInventoryItem>();
        if (item != null)
        {
            Debug.Log("item CATCH !");
            Debug.Log(item);
            inventory.AddItem(item);
        }

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
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("acccccccelere tmr");
            moveSpeed = 7f;
        }
        else
        {
            moveSpeed = 6f;
        }
        Debug.Log("vitesse apres");
        Debug.Log(moveSpeed);

        //Vector3 forward = cylinderTransform.forward;
        //Vector3 right = cylinderTransform.right;

        //forward.y = 0f;
        //right.y = 0f;

        //Vector3 desiredMoveDirection = forward * v + right * h;
        bool accelerateKeyPressed = Input.GetKey(KeyCode.B);
        if (accelerateKeyPressed && !staminaFullUsed)
        {
            if (stamina > 0)
            {
                stamina -= 30 * Time.deltaTime;
                moveSpeed = 15f;
            }
            else
            {
                staminaFullUsed = true;
            }
        }
        else
        {
            if (stamina < maxStamina)
            {
                stamina += 30 * Time.deltaTime;
                moveSpeed = 6f;
            }
            else
            {
                staminaFullUsed = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log(HUD);
            if (HUD.activeSelf)
            {
                HUD.SetActive(false);
                // Debug.Log("close hud");
            }
            else
            {
                //  Debug.Log("activate hud");
                HUD.SetActive(true);
            }
        }
        //rb.velocity = desiredMoveDirection.normalized * currentMoveSpeed;
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
