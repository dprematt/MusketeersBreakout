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
    public int kills = 0;
    public int lootedChests = 0;
    public int dmgTaken = 0;
    public int dmgDone = 0;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float movementMultiplier = 10f;
    [SerializeField] float airMultiplier = 0.4f;

    [Header("Jumping")]
    public float jumpForce = 7f;
    private bool hasDoubleJumped = false;

    [Header("Keybinds")]
    [SerializeField] public KeyCode jumpKey = KeyCode.Space;
    [SerializeField] public KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] public KeyCode HUDKey = KeyCode.U;
    [SerializeField] public KeyCode dodgeKey = KeyCode.F;

    [Header("Drag")]
    [SerializeField] float groundDrag = 6f;
    [SerializeField] float airDrag = 2f;

    [SerializeField] public float health, maxHealth = 10f;

    [SerializeField] private float maxStamina = 100f;
    public float stamina;
    public static event System.Action<float, float> OnStaminaChanged;

    private bool staminaFullUsed;
    public HealthManager HealthManager;
    private PlayFabInventory PFInventory_;

    public Inventory inventory;
    public GameObject HUD;
    public GameObject HUDFixe;
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

    private int currentWeapon;

    private Vector3 aimTarget;
    LineRenderer lineRenderer;

    public bool hasShield = false;
    private GameObject shield;
    private GameObject extractionZone;
    public Shield shieldComp;
    public Weapon EquippedWeapon;

    public EventListener eventListener;

    private void Start()
    {
        HealthManager = GetComponent<HealthManager>();
        PFInventory_ = GetComponent<PlayFabInventory>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        health = maxHealth;
        xp = 1;
        level = 1;
        max_xp = 20;
        health_up = 30;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<Inventory>();
        HUD = GameObject.FindGameObjectWithTag("InventoryHUD");
        HUD.GetComponent<HUD>().init();
        HUDFixe = GameObject.FindGameObjectWithTag("InventoryHUDFixe");
        HUDFixe.GetComponent<HUDFixe>().init();
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
        HUDFixe hudfixe2 = HUDFixe.GetComponent<HUDFixe>();
        hudfixe2.Clean();
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

        if (Input.GetKeyDown(jumpKey) && !staminaFullUsed)
        {
            Jump();
        }

        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if ((scrollDelta > 0f || scrollDelta < 0f) && inventory.PocketCount() > 1 && anim.GetInteger("intAttackPhase") == 0)
        {
            inventory.SwapItems(0, 1);
            if (inventory.mItems[0] == null)
            {
                EquippedWeapon = null;
            }
            EquippedWeapon = inventory.mItems[0].GameObject.GetComponent<Weapon>();
            EquippedWeapon.setAnim();
            eventListener.weaponComp = EquippedWeapon;

            HUD.GetComponent<HUD>().Clean();
            HUDFixe.GetComponent<HUDFixe>().Clean();
        }

        if (Input.GetKey(sprintKey) && !staminaFullUsed)
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
        else if (Input.GetKeyDown(jumpKey) && !staminaFullUsed && (isGrounded || !hasDoubleJumped))
        {
            if (stamina > 0)
            {
                stamina -= 20;
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
        if (Input.GetKeyDown(HUDKey))
        {
            if (HUD.activeSelf)
            {
                HUD.SetActive(false);
            }
            else
            {
                HUD.SetActive(true);
                HUD hud = HUD.GetComponent<HUD>();
                hud.Clean();
                HUDFixe hudfixe = HUDFixe.GetComponent<HUDFixe>();
                hudfixe.Clean();
            }
        }

        /*if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventory.SwapItems(0, 1);
            if (inventory.mItems[0] == null)
            {
                EquippedWeapon = null;
            }
            EquippedWeapon = inventory.mItems[0].GameObject.GetComponent<Weapon>();
            HUD.GetComponent<HUD>().Clean();
            HUDFixe.GetComponent<HUDFixe>().Clean();
        }*/

        if (Input.GetKeyDown(KeyCode.E))
        {
            LootHUD.SetActive(false);
        }

        if (hasShield && Input.GetMouseButton(1))
        {
            shieldComp.setProtectionMode(true);
        }
        else if (hasShield && Input.GetMouseButtonUp(1))
        {
            shieldComp.setProtectionMode(false);
        }

        if (inventory.PocketCount() > 0)
        {
            if (!EquippedWeapon.isLongRange && Input.GetMouseButtonDown(0))
            {
                EquippedWeapon.Attack();
            }

            if (EquippedWeapon.isLongRange && !Input.GetKey(sprintKey))
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

        if (inventory.mItems != null)
        {
            if (inventory.mItems[0] == null)
            {
                EquippedWeapon = null;

            }

            if (inventory.mItems[0] != null && inventory.mItems[0].GameObject != EquippedWeapon.gameObject)
            {
                EquippedWeapon = inventory.mItems[0].GameObject.GetComponent<Weapon>();
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
            HealthManager.Take_Damage((int)Damage);
            bloodParticles.Play();
        }
    }
    public int UpdateXp(int new_xp)
    {
        if (photonView.IsMine)
        {
            xp += new_xp;
            Debug.Log("xp = " + xp);
            Debug.Log(new_xp);
            if (new_xp != 5)
                kills += 1;
            XpProgressBar.fillAmount = (float)(xp / max_xp);
            xpText2D.text = "XP " + xp.ToString() + " / " + max_xp.ToString();
            return xp;
        }
        return xp;
    }
    public void UpdateLevel()
    {
        if (photonView.IsMine)
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
    }
    public void CheckXp()
    {
        if (photonView.IsMine)
        {
            if (xp >= max_xp)
            {
                UpdateLevel();
            }
        }
    }

    public void DeactivateLoot()
    {
        if (LootHUD)
        {
            LootHUD.SetActive(false);
        }
    }
    void OnCollisionEnter(Collision col)
    {
        Inventory loot = col.gameObject.GetComponent<Inventory>();
        if (loot != null)
        {
            if (loot.loot == true)
            {
                if (LootHUD == null)
                    return;
                Debug.Log("do we need to activate ?");
                if (!LootHUD.activeSelf)
                {
                    LootHUD.SetActive(true);
                }
                else
                {
                    return;
                }
                LootHUD.GetComponent<LootHUD>().init(ref loot);
                LootHUD.GetComponent<LootHUD>().Clean();
                loot.DisplayLoot(inventory);

            }
        }
    }

    public void UnequipWeapon()
    {

    }
    public void EquipWeapon(Weapon weaponComp, GameObject weaponObject, bool toAdd)
    {
        if (weaponComp != null && toAdd == true)
            inventory.AddItem(weaponComp);
        Transform hand = FindDeepChild(transform, "jointItemR");
        weaponComp.whenPickUp(gameObject);
        if (inventory.PocketCount() > 1 && toAdd == true)
        {
            weaponObject.SetActive(false);
        }
        else
        {
            weaponComp.setAnim();
            currentWeapon = 0;
            eventListener.weaponComp = weaponComp;
        }
        EquippedWeapon = weaponComp;
    }
    void OnTriggerEnter(Collider col)
    {
        Bullet bullet = col.GetComponent<Bullet>();
        if (bullet != null && bullet.shooter != null && bullet.shooter.ActorNumber != photonView.Owner.ActorNumber)
        {
            if (hasShield && shieldComp.isProtecting)
            {
                return;
            }
            TakeDamage(10);
            bullet.GetComponent<PhotonView>().RPC("Destroy", RpcTarget.AllBuffered);
        }

        if (col.CompareTag("ExtractionZone"))
        {
            PFInventory_.PlayerWin();
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel("Menu");
        }

        Weapon weaponComp = col.GetComponent<Weapon>();
        if (weaponComp != null)
        {
            if (weaponComp.isLooted && weaponComp.holder != gameObject && weaponComp.IsAttacking)
            {
                if (hasShield && shieldComp.isProtecting)
                {
                    return;
                }
                TakeDamage(weaponComp.damages);
            }

            if (hasShield && (weaponComp.tag == "WeaponSpear"
            || weaponComp.tag == "WeaponHalberd"
            || weaponComp.tag == "WeaponCrossBow"))
            {
                return;
            }
            if (inventory == null)
                return;
            if (inventory.PocketCount() < 2)
            {
                GameObject weapon = col.gameObject;
                if (!weaponComp.isLooted)
                {
                    Debug.Log(weaponComp.Name);
                    EquipWeapon(weaponComp, weapon, true);
                }
            }
            else
            {
                GameObject weapon = col.gameObject;
                if (inventory.Count() < 9 && (!weaponComp.isLooted))
                {
                    inventory.AddItem(weaponComp);
                    weapon.SetActive(false);
                }
            }
        }

        if (!hasShield)
        {
            if (inventory.PocketCount() > 0 && (inventory.mItems[currentWeapon].Name == "WeaponSpear"
            || inventory.mItems[currentWeapon].Name == "WeaponHalberd"
            || inventory.mItems[currentWeapon].Name == "WeaponCrossBow"))
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
                    shieldComp.whenPickUp(gameObject);
                    //EquippedWeapon = weaponComp; a demander a mathis pr le  shield;
                    if (inventory.PocketCount() > 0)
                    {
                        anim.SetLayerWeight(1, 0f);
                        anim.SetLayerWeight(4, 1f);
                    }
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

    public void SetJumpKey(KeyCode newJumpKey)
    {
        jumpKey = newJumpKey;
    }
    public void SetSprintKey(KeyCode newSprintKey)
    {
        sprintKey = newSprintKey;
    }
    public void SetHUDKey(KeyCode newHUDKey)
    {
        HUDKey = newHUDKey;
    }
    public void SetDodgeKey(KeyCode newDodgeKey)
    {
        dodgeKey = newDodgeKey;
    }
    void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            hasDoubleJumped = false;
        }
        else if (!hasDoubleJumped)
        {
            rb.AddForce(transform.up * 7f, ForceMode.Impulse);
            hasDoubleJumped = true;
        }
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
        if (Input.GetKeyDown(dodgeKey) && isGrounded && !anim.GetBool("isDodging"))
        {
            Vector3 dodgeDirection = characterModel.forward;
            rb.AddForce(dodgeDirection.normalized * 50f * movementMultiplier, ForceMode.Acceleration);
            anim.SetBool("isDodging", true);
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0) && anim.GetBool("isDodging"))
        {
            anim.SetBool("isDodging", false);
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
    public Transform FindDeepChild(Transform parent, string name)
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