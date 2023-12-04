using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isGrounded = true;
    public Vector3 jump;
    public float jumpForce = 2.0f;
    public float heightModifier = 0.5f;
    public float minHeight = 0.5f;
    public int xp;
    /// <summary>
    /// private 
    /// </summary>
    private float originalHeight;
    private Transform cylinderTransform;
    private Rigidbody rb;
    //private int inventory_size = 20;
    public Inventory inventory;
    public GameObject HUD;
    
    [SerializeField] float health, maxHealth = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        jump = new Vector3(0.0f, 2.0f, 0.0f);
        cylinderTransform = transform;
        originalHeight = cylinderTransform.localScale.y;
        health = maxHealth;
        xp = 0;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<Inventory>();
        Debug.Log(inventory);
        HUD = GameObject.FindGameObjectWithTag("InventoryHUD");
        HUD.GetComponent<HUD>().init();
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public int UpdateXp(int new_xp)
    {
        xp += new_xp;
        return xp;
    }

    void OnCollisionStay()
    {
        isGrounded = true;
    }

    void OnCollisionEnter(Collision col)
    {//here
        Debug.Log("COLLISION WITH PLAYER !");
        Debug.Log(col.gameObject.name);
        Inventory loot = col.collider.GetComponent<Inventory>();
        if (loot != null)
        {
                Debug.Log("in if 2 inventory");
                loot.DisplayLoot(inventory);
            return;
        }
        IInventoryItem item = col.collider.GetComponent<IInventoryItem>();
        if (item != null)
        {
            Debug.Log("item CATCH !");
            Debug.Log(item);
            inventory.AddItem(item);
        }

    }

    // Update is called once per frame
    void Update()
    {
        float moveSpeed = 3f;
        float acceleratedMoveSpeed = 10f;
        float rotationAngle = 90f;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.T) && isGrounded)
        {
            cylinderTransform.Rotate(Vector3.up * rotationAngle);
        }

        if (Input.GetKeyDown(KeyCode.Y) && isGrounded)
        {
            cylinderTransform.Rotate(Vector3.up * -rotationAngle);
        }

        /*if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("acccccccelere tmr");
            moveSpeed = 7f;
        } else {
            moveSpeed = 3f;
        }*/

        Vector3 forward = cylinderTransform.forward;
        Vector3 right = cylinderTransform.right;

        forward.y = 0f;
        right.y = 0f;

        Vector3 desiredMoveDirection = forward * v + right * h;
        bool accelerateKeyPressed = Input.GetKey(KeyCode.M);
        float currentMoveSpeed = accelerateKeyPressed ? acceleratedMoveSpeed : moveSpeed;
        rb.velocity = desiredMoveDirection.normalized * currentMoveSpeed;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {

            rb.AddForce(jump * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

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
        /*if (Input.GetKeyDown(KeyCode.L))
        {
        }*/
        if (Input.GetKeyDown(KeyCode.V))
        {
            Vector3 player_pos = gameObject.transform.position;
            player_pos.z += 2;
            if (GetComponent<Bow>() != null)
            {
                GameObject bowPrefab = Resources.Load<GameObject>("Prefabs/Bow");
                bowPrefab.transform.position = player_pos;
                Instantiate(bowPrefab);
                Destroy(GetComponent<Bow>());
            }
            else if (GetComponent<Knife>() != null)
            {
                GameObject knifePrefab = Resources.Load<GameObject>("Prefabs/Knife");
                knifePrefab.transform.position = player_pos;
                Instantiate(knifePrefab);
                Destroy(GetComponent<Knife>());
            }
            else if (GetComponent<CrossBow>() != null)
            {
                GameObject CrossBowPrefab = Resources.Load<GameObject>("Prefabs/CrossBow");
                CrossBowPrefab.transform.position = player_pos;
                Instantiate(CrossBowPrefab);
                Destroy(GetComponent<CrossBow>());
            }
            else if (GetComponent<Gun>() != null)
            {
                GameObject GunPrefab = Resources.Load<GameObject>("Prefabs/Gun");
                GunPrefab.transform.position = player_pos;
                Instantiate(GunPrefab);
                Destroy(GetComponent<Gun>());
            }
            else if (GetComponent<Spear>() != null)
            {
                GameObject SpearPrefab = Resources.Load<GameObject>("Prefabs/Spear");
                SpearPrefab.transform.position = player_pos;
                Instantiate(SpearPrefab);
                Destroy(GetComponent<Spear>());
            }
            else if (GetComponent<Dagger>() != null)
            {
                GameObject DaggerPrefab = Resources.Load<GameObject>("Prefabs/Dagger");
                DaggerPrefab.transform.position = player_pos;
                Instantiate(DaggerPrefab);
                Destroy(GetComponent<Dagger>());
            }
            else if (GetComponent<Halberd>() != null)
            {
                GameObject HalberdPrefab = Resources.Load<GameObject>("Prefabs/Halberd");
                HalberdPrefab.transform.position = player_pos;
                Instantiate(HalberdPrefab);
                Destroy(GetComponent<Halberd>());
            }
            else if (GetComponent<Sword>() != null)
            {
                GameObject SwordPrefab = Resources.Load<GameObject>("Prefabs/Sword");
                SwordPrefab.transform.position = player_pos;
                Instantiate(SwordPrefab);
                Destroy(GetComponent<Sword>());
            }
        }
    }
}
