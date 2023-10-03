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
    {
        if (col.gameObject.tag == "WeaponBow")
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Bow weapon_script = gameObject.AddComponent<Bow>();
                AttackBow weapon_attack_script = gameObject.AddComponent<AttackBow>();
                weapon_script.bulletSpawnPoint = gameObject.transform;
                GameObject bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
                weapon_script.bulletPrefab = bulletPrefab;
                weapon_attack_script.b = weapon_script;
                Destroy(col.gameObject);
            }
        }
        if (col.gameObject.tag == "WeaponCrossBow")
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                CrossBow weapon_script = gameObject.AddComponent<CrossBow>();
                CrossBowAttack weapon_attack_script = gameObject.AddComponent<CrossBowAttack>();
                weapon_script.bulletSpawnPoint = gameObject.transform;
                GameObject bulletPrefab = Resources.Load<GameObject>("Prefabs/CrossBow");
                weapon_script.bulletPrefab = bulletPrefab;
                weapon_attack_script.c = weapon_script;
                Destroy(col.gameObject);
            }
        }
        if (col.gameObject.tag == "WeaponSpear")
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Spear weapon_script = gameObject.AddComponent<Spear>();
                SpearAttack weapon_attack_script = gameObject.AddComponent<SpearAttack>();
                weapon_script.bulletSpawnPoint = gameObject.transform;
                GameObject bulletPrefab = Resources.Load<GameObject>("Prefabs/Spear");
                weapon_script.bulletPrefab = bulletPrefab;
                weapon_attack_script.s = weapon_script;
                Destroy(col.gameObject);
            }
        }
        if (col.gameObject.tag == "WeaponGun")
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Gun weapon_script = gameObject.AddComponent<Gun>();
                Attack weapon_attack_script = gameObject.AddComponent<Attack>();
                weapon_script.bulletSpawnPoint = gameObject.transform;
                GameObject bulletPrefab = Resources.Load<GameObject>("Prefabs/Gun");
                weapon_script.bulletPrefab = bulletPrefab;
                weapon_attack_script.g = weapon_script;
                Destroy(col.gameObject);
            }
        }
        if (col.gameObject.tag == "WeaponKnife")
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Knife weapon_script = gameObject.AddComponent<Knife>();
                KnifeAttack weapon_attack_script = gameObject.AddComponent<KnifeAttack>();
                weapon_script.weaponSpawnPoint = gameObject.transform;
                GameObject bulletPrefab = Resources.Load<GameObject>("Prefabs/Knife");
                weapon_script.weaponPrefab = bulletPrefab;
                weapon_attack_script.k = weapon_script;
                Destroy(col.gameObject);
            }
        }
        if (col.gameObject.tag == "WeaponDagger")
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Dagger weapon_script = gameObject.AddComponent<Dagger>();
                DaggerAttack weapon_attack_script = gameObject.AddComponent<DaggerAttack>();
                weapon_script.weaponSpawnPoint = gameObject.transform;
                GameObject bulletPrefab = Resources.Load<GameObject>("Prefabs/Dagger");
                weapon_script.weaponPrefab = bulletPrefab;
                weapon_attack_script.d = weapon_script;
                Destroy(col.gameObject);
            }
        }
        if (col.gameObject.tag == "WeaponSword")
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Sword weapon_script = gameObject.AddComponent<Sword>();
                SwordAttack weapon_attack_script = gameObject.AddComponent<SwordAttack>();
                weapon_script.weaponSpawnPoint = gameObject.transform;
                GameObject bulletPrefab = Resources.Load<GameObject>("Prefabs/Sword");
                weapon_script.weaponPrefab = bulletPrefab;
                weapon_attack_script.s = weapon_script;
                Destroy(col.gameObject);
            }
        }
        if (col.gameObject.tag == "WeaponHalberd")
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Halberd weapon_script = gameObject.AddComponent<Halberd>();
                HalberdAttack weapon_attack_script = gameObject.AddComponent<HalberdAttack>();
                weapon_script.weaponSpawnPoint = gameObject.transform;
                GameObject bulletPrefab = Resources.Load<GameObject>("Prefabs/Halberd");
                weapon_script.weaponPrefab = bulletPrefab;
                weapon_attack_script.h = weapon_script;
                Destroy(col.gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal") * 5;
        float v = Input.GetAxis("Vertical") * 5;

        Vector3 vel = rb.velocity;
        vel.x = h;
        vel.z = v;
        rb.velocity = vel;

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
                Destroy(GetComponent<AttackBow>());
            }
            else if (GetComponent<Knife>() != null)
            {
                GameObject knifePrefab = Resources.Load<GameObject>("Prefabs/Knife");
                knifePrefab.transform.position = player_pos;
                Instantiate(knifePrefab);
                Destroy(GetComponent<Knife>());
                Destroy(GetComponent<KnifeAttack>());
            }
            else if (GetComponent<CrossBow>() != null)
            {
                GameObject CrossBowPrefab = Resources.Load<GameObject>("Prefabs/CrossBow");
                CrossBowPrefab.transform.position = player_pos;
                Instantiate(CrossBowPrefab);
                Destroy(GetComponent<CrossBow>());
                Destroy(GetComponent<CrossBowAttack>());
            }
            else if (GetComponent<Gun>() != null)
            {
                GameObject GunPrefab = Resources.Load<GameObject>("Prefabs/Gun");
                GunPrefab.transform.position = player_pos;
                Instantiate(GunPrefab);
                Destroy(GetComponent<Gun>());
                Destroy(GetComponent<Attack>());
            }
            else if (GetComponent<Spear>() != null)
            {
                GameObject SpearPrefab = Resources.Load<GameObject>("Prefabs/Spear");
                SpearPrefab.transform.position = player_pos;
                Instantiate(SpearPrefab);
                Destroy(GetComponent<Spear>());
                Destroy(GetComponent<SpearAttack>());
            }
            else if (GetComponent<Dagger>() != null)
            {
                GameObject DaggerPrefab = Resources.Load<GameObject>("Prefabs/Dagger");
                DaggerPrefab.transform.position = player_pos;
                Instantiate(DaggerPrefab);
                Destroy(GetComponent<Dagger>());
                Destroy(GetComponent<DaggerAttack>());
            }
            else if (GetComponent<Halberd>() != null)
            {
                GameObject HalberdPrefab = Resources.Load<GameObject>("Prefabs/Halberd");
                HalberdPrefab.transform.position = player_pos;
                Instantiate(HalberdPrefab);
                Destroy(GetComponent<Halberd>());
                Destroy(GetComponent<HalberdAttack>());
            }
            else if (GetComponent<Sword>() != null)
            {
                GameObject SwordPrefab = Resources.Load<GameObject>("Prefabs/Sword");
                SwordPrefab.transform.position = player_pos;
                Instantiate(SwordPrefab);
                Destroy(GetComponent<Sword>());
                Destroy(GetComponent<SwordAttack>());
            }
        }
    }
}
