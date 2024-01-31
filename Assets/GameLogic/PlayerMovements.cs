using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMovements : MonoBehaviourPunCallbacks
{
    public int xp;
    public int level;
    public Text levelText2D;
    public Text xpText2D;
    [SerializeField] private Image XpProgressBar;
    public int health_up;
    public int max_xp;
    public bool Demo = false;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float movementMultiplier = 10f;
    [SerializeField] float airMultiplier = 0.4f;

    [Header("Jumping")]
    public float jumpForce = 6f;

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

    public Vector3 moveDirection;

    Rigidbody rb;

    private float originalHeight;
    public float heightModifier = 0.5f;
    public float minHeight = 0.5f;
    public Transform cylinderTransform;

    public float rotationSpeed = 50.0f;

    public Transform characterModel;

    public Animator anim;

    public bool isAttacking = false;

    private void Start()
    {
        HealthManager = GetComponent<HealthManager>();
        PFInventory_ = GetComponent<PlayFabInventory>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        health = maxHealth;
        xp = 0;
        level = 1;
        max_xp = 20;
        health_up = 30;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<Inventory>();
        HUD = GameObject.FindGameObjectWithTag("InventoryHUD");
        HUD.GetComponent<HUD>().init();
        GameObject xpProgressBarExperience = GameObject.FindWithTag("ExperienceBarDefaultTag");
        GameObject xpProgressBarLevel = GameObject.FindWithTag("ExperienceBarLevelTag");
        GameObject xpProgressBarXp = GameObject.FindWithTag("ExperienceBarXpTag");
        XpProgressBar = xpProgressBarExperience.GetComponent<Image>();
        levelText2D = xpProgressBarLevel.GetComponent<Text>();
        xpText2D = xpProgressBarXp.GetComponent<Text>();

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
        XpProgressBar.fillAmount = (float)(xp / max_xp);
        xpText2D.text = "XP " + xp.ToString() + " / " + max_xp.ToString();
        return xp;
    }

    public void UpdateLevel()
    {
        level = level + 1;
        xp = xp - max_xp;
        HealthManager.HealthUp(health_up);
        health_up = health_up + 5;
        max_xp = max_xp + 10;
        XpProgressBar.fillAmount = (float)(xp / max_xp);
        xpText2D.text = "XP " + xp.ToString() + " / " + max_xp.ToString();
        levelText2D.text = "LEVEL " + level.ToString();
    }

    public void CheckXp()
    {
        if (xp >= max_xp) {
            UpdateLevel();
        }
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
            inventory.AddItem(item);
        }

    }

    private void Update()
    {
        Vector3 temp = transform.position;
        temp.y += 0.1f;
        isGrounded = Physics.Raycast(temp, Vector3.down, 0.2f);

        MyInput();
        ControlDrag();

        CheckXp();

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }

        //Vector3 forward = cylinderTransform.forward;
        //Vector3 right = cylinderTransform.right;

        //forward.y = 0f;
        //right.y = 0f;

        //Vector3 desiredMoveDirection = forward * v + right * h;
        if (Input.GetKey(KeyCode.LeftShift) && !staminaFullUsed)
        {
            if (stamina > 0)
            {
                stamina -= 30 * Time.deltaTime;
                moveSpeed = 8f;
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
            if (HUD.activeSelf)
            {
                HUD.SetActive(false);
            }
            else
            {
                HUD.SetActive(true);
            }
        }

        if (anim.GetBool("isAttacking") == true)
            moveSpeed = 0f;
        else
            moveSpeed = 6f;
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

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDirection = cameraForward * verticalMovement + cameraRight * horizontalMovement;

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

        if (Input.GetKey(KeyCode.LeftControl) && isGrounded)
        {
            float newHeight = originalHeight * heightModifier;
            newHeight = Mathf.Max(newHeight, minHeight);
            Vector3 newScale = cylinderTransform.localScale;
            newScale.y = newHeight;
            cylinderTransform.localScale = newScale;
        }
        else
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
