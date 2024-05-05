using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class Player : MonoBehaviourPunCallbacks
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

    [SerializeField] private float maxStamina = 100f;
    public float stamina;
    public static event System.Action<float, float> OnStaminaChanged;

    private bool staminaFullUsed;
    private HealthManager HealthManager;
    private PlayFabInventory PFInventory_;

    public Inventory inventory;
    public GameObject HUD;
    public GameObject LootHUD;


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

    public ParticleSystem bloodParticles;

    public List<Weapon> weaponList;
    private int currentWeapon;

    private Vector3 aimTarget;
    LineRenderer lineRenderer;

    private bool hasShield = false;
    private GameObject shield;
    public Shield shieldComp;

    public EventListener eventListener;

    private void Start()
    {
        weaponList = new List<Weapon>();
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
        LootHUD = GameObject.FindGameObjectWithTag("LootHUD");
        GameObject xpProgressBarExperience = GameObject.FindWithTag("ExperienceBarDefaultTag");
        GameObject xpProgressBarLevel = GameObject.FindWithTag("ExperienceBarLevelTag");
        GameObject xpProgressBarXp = GameObject.FindWithTag("ExperienceBarXpTag");
        XpProgressBar = xpProgressBarExperience.GetComponent<Image>();
        levelText2D = xpProgressBarLevel.GetComponent<Text>();
        xpText2D = xpProgressBarXp.GetComponent<Text>();
        cylinderTransform = transform;
        originalHeight = cylinderTransform.localScale.y;
        eventListener = GameObject.Find("PlayerBody").GetComponent<EventListener>();
        //StartCoroutine(DamageOverTime());
    }
    private void Update()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.enabled = false;
        }

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

        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if ((scrollDelta > 0f || scrollDelta < 0f) && weaponList.Count > 1)
        {
            int layer = weaponList[currentWeapon].isLongRange ? 2 : 1;

            anim.SetLayerWeight(layer, 0f);

            weaponList[currentWeapon].gameObject.SetActive(false);
            currentWeapon = currentWeapon == 0 ? 1 : 0;
            weaponList[currentWeapon].gameObject.SetActive(true);
            weaponList[currentWeapon].setAnim();
            eventListener.weaponComp = weaponList[currentWeapon];
        }

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
        OnStaminaChanged?.Invoke(stamina, maxStamina);
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            LootHUD.SetActive(false);
        }

        if (hasShield && Input.GetMouseButton(1))
        {
            shieldComp.setProtectionMode(true);
        } else if (hasShield && Input.GetMouseButtonUp(1))
        {
            shieldComp.setProtectionMode(false);
        }

        if (weaponList.Count > 0)
        {
            if (!weaponList[currentWeapon].isLongRange && Input.GetMouseButtonDown(0))
            {
                weaponList[currentWeapon].Attack();
            }

            if (weaponList[currentWeapon].isLongRange && !Input.GetKey(KeyCode.LeftShift))
            {
                IsometricAiming aim = gameObject.GetComponent<IsometricAiming>();
                if (Input.GetMouseButtonDown(1))
                    aim.laserStartAndStop();

                if (Input.GetMouseButton(1))
                {
                    anim.SetLayerWeight(2, Mathf.Lerp(anim.GetLayerWeight(2), 1f, Time.deltaTime * 10f));
                    aim.Aiming();
                }
                else
                    anim.SetLayerWeight(2, Mathf.Lerp(anim.GetLayerWeight(2), 0f, Time.deltaTime * 10f));

                if (Input.GetMouseButtonUp(1))
                    aim.laserStartAndStop();
            }
        }
    }
    private IEnumerator DamageOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            TakeDamage(1);
        }
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
            bloodParticles.Play();
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
        if (xp >= max_xp)
        {
            UpdateLevel();
        }
    }
    void OnCollisionEnter(Collision col)
    {
        Debug.Log("on trigger enter");
        Inventory loot = col.gameObject.GetComponent<Inventory>();
        if (loot.loot == true)
        {
            Debug.Log("do we need to activate ?");
            if (!LootHUD.activeSelf)
            {
                Debug.Log("activated");
                LootHUD.SetActive(true);
            }
            else
            {
                Debug.Log("not activated");
                return;
            }
            Debug.Log("size before init");
            Debug.Log(loot.mItems.Count);
            LootHUD.GetComponent<LootHUD>().init(ref loot);
            Debug.Log("size after init");
            Debug.Log(loot.mItems.Count);
            loot.DisplayLoot(inventory);

        }
        /*Inventory loot = col.gameObject.GetComponent<Inventory>();
        if (loot != null)
        {
            loot.DisplayLoot(inventory);
            return;
        }*/
    }
    void OnTriggerEnter(Collider col)
    {
        
        IInventoryItem item = col.GetComponent<IInventoryItem>();
        if (item != null)
        {
            inventory.AddItem(item);
        }

        Weapon weaponComp = col.GetComponent<Weapon>();
        if (weaponComp != null)
        {
            if (hasShield && (weaponComp.tag == "WeaponSpear"
            || weaponComp.tag == "WeaponHalberd"
            || weaponComp.tag == "WeaponCrossBow"))
            {
                return;
            }

            if (weaponList.Count < 2)
            {
                GameObject weapon = col.gameObject;
                if (!weaponComp.isLooted)
                {
                    weaponList.Add(weaponComp);
                    Transform hand = FindDeepChild(transform, "jointItemR");
                    weaponComp.whenPickUp(gameObject, hand);
                    if (weaponList.Count == 2)
                    {
                        weapon.SetActive(false);
                    }
                    else
                    {
                        weaponComp.setAnim();
                        currentWeapon = 0;
                        eventListener.weaponComp = weaponComp;
                    }
                }
            }
        }

        if (!hasShield)
        {
            if (weaponList.Count > 0 && (weaponList[currentWeapon].tag == "WeaponSpear"
            || weaponList[currentWeapon].tag == "WeaponHalberd"
            || weaponList[currentWeapon].tag == "WeaponCrossBow"))
            {
                return;
            }

            shieldComp = col.GetComponent<Shield>();
            if (shieldComp != null)
            {
                shield = col.gameObject;
                if (!shieldComp.isLooted)
                {
                    hasShield = true;
                    Transform hand = FindDeepChild(transform, "jointItemL");
                    shieldComp.whenPickUp(gameObject, hand);
                }
            }
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
            //transform.rotation = Quaternion.Slerp(characterModel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
    Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
            Transform result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}