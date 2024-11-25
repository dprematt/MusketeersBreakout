using UnityEngine;
using TMPro;
using System.Collections;
using Photon.Pun;

public class IsometricAiming : MonoBehaviourPun
{
    [Header("Aim")]
    [SerializeField] private bool aim;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool ignoreHeight;
    [SerializeField] private Transform aimedTransform;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] public Camera ViewCamera;

    [Header("Laser")]
    [SerializeField] private LineRenderer laserRenderer;
    [SerializeField] private LayerMask laserMask;
    [SerializeField] private float laserLength;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform prefabSpawn;

    [Header("Gizmos")]
    [SerializeField] private bool gizmo_cameraRay = false;
    [SerializeField] private bool gizmo_ground = false;
    [SerializeField] private bool gizmo_target = false;
    [SerializeField] private bool gizmo_ignoredHeightTarget = false;

    [Header("Shoot")]
    [SerializeField] public int maxAmmo = 1;
    [SerializeField] private int currentAmmo;
    [SerializeField] public float reloadTime = 2.03f;
    [SerializeField] private bool isReloading = false;
    [SerializeField] public AudioClip attackSound;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public float damages = 10f;

    [SerializeField] public Animator anim;

    [Header("UI")]
    [SerializeField] private TMP_Text ammoCounter;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        currentAmmo = maxAmmo;
        anim = GetComponentInChildren<Animator>();

        if (laserRenderer != null)
        {
            laserRenderer.SetPositions(new Vector3[]{
                Vector3.zero,
                Vector3.zero
            });
        }

        laserRenderer.enabled = false;

        UpdateAmmoUI();
    }

    private void Update()
    {
    }

    public void UpdateAmmoUI()
    {
        if (ammoCounter != null)
        {
            ammoCounter.text = maxAmmo.ToString();
        }
    }

    private void OnEnable()
    {
        isReloading = false;
        anim.SetBool("isReloading", false);
    }

    public void Aiming()
    {
        Aim();
        RefreshLaser();
        Shoot();
        ChangeTargetMode();
        GizmoSettings();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
        {
            return;
        }

        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, float.MaxValue, groundMask))
        {
            if (gizmo_cameraRay)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(ray.origin, hitInfo.point);
                Gizmos.DrawWireSphere(ray.origin, 0.5f);
            }

            var hitPosition = hitInfo.point;
            var hitGroundHeight = Vector3.Scale(hitInfo.point, new Vector3(1, 0, 1)); ;
            var hitPositionIngoredHeight = new Vector3(hitInfo.point.x, aimedTransform.position.y, hitInfo.point.z);

            if (gizmo_ground)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawWireSphere(hitGroundHeight, 0.5f);
                Gizmos.DrawLine(hitGroundHeight, hitPosition);
            }

            if (gizmo_target)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(hitInfo.point, 0.5f);
                Gizmos.DrawLine(aimedTransform.position, hitPosition);
            }

            if (gizmo_ignoredHeightTarget)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(hitPositionIngoredHeight, 0.5f);
                Gizmos.DrawLine(aimedTransform.position, hitPositionIngoredHeight);
            }
        }
    }

    private void Aim()
    {
        if (aim == false)
        {
            return;
        }

        var (success, position) = GetMousePosition();

        if (success)
        {
            var direction = position - aimedTransform.position;

            if (ignoreHeight)
            {
                direction.y = 0;
            }

            float verticalAngle = Mathf.Atan2(direction.y, direction.magnitude) * Mathf.Rad2Deg;
            float horizontalAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            bodyTransform.localRotation = Quaternion.Euler(0f, horizontalAngle, 0f);
            aimedTransform.localRotation = Quaternion.Euler(-1 * verticalAngle, horizontalAngle, 0f);
        }
    }

    private (bool success, Vector3 position) GetMousePosition()
    {
        var ray = ViewCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            return (success: true, position: hitInfo.point);
        }
        else
        {
            return (success: false, position: Vector3.zero);
        }
    }

    private void Shoot()
    {
        if (isReloading)
            return;

        if (currentAmmo <= 0)
        {
            if (maxAmmo > 0)
            {
                maxAmmo--;
                StartCoroutine(Reload());
                return;
            }
            else
                return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            currentAmmo--;

            Weapon equippedWeapon = GetComponent<Player>().EquippedWeapon;
            if (equippedWeapon.isLongRange)
            {
                audioSource = equippedWeapon.gameObject.GetComponent<AudioSource>();
                audioSource.PlayOneShot(attackSound);
            }

            /*RaycastHit hit;
            if (Physics.Raycast(prefabSpawn.position, prefabSpawn.forward, out hit, Mathf.Infinity, laserMask))
            {
                Debug.Log(hit.transform.name);

                Enemy enemy = hit.transform.GetComponentInParent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damages);
                }
            }*/

            var projectile = PhotonNetwork.Instantiate(projectilePrefab.name, prefabSpawn.position, Quaternion.identity);
            projectile.transform.forward = aimedTransform.forward;
            if (projectile.GetComponent<Bullet>() != null)
            {
                int shooterID = photonView.Owner.ActorNumber;
                projectile.GetComponent<Bullet>().photonView.RPC("SetShooter", RpcTarget.AllBuffered, shooterID);
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        anim.SetBool("isReloading", true);

        yield return new WaitForSeconds(reloadTime);

        anim.SetBool("isReloading", false);

        currentAmmo = 1;
        isReloading = false;

        UpdateAmmoUI();
    }

    private void RefreshLaser()
    {
        if (laserRenderer == null)
        {
            return;
        }

        Vector3 lineEnd;

        if (Physics.Raycast(prefabSpawn.position, prefabSpawn.forward, out var hitinfo, laserLength, laserMask))
        {
            lineEnd = hitinfo.point;
        }
        else
        {
            lineEnd = prefabSpawn.position + aimedTransform.forward * laserLength;
        }
        laserRenderer.SetPosition(1, aimedTransform.InverseTransformPoint(lineEnd));
    }

    private void ChangeTargetMode()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ignoreHeight = !ignoreHeight;
        }
    }

    private void GizmoSettings()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gizmo_cameraRay = !gizmo_cameraRay;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            gizmo_ground = !gizmo_ground;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            gizmo_target = !gizmo_target;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            gizmo_ignoredHeightTarget = !gizmo_ignoredHeightTarget;
        }
    }

    public void laserStartAndStop()
    {
        laserRenderer.enabled = !laserRenderer.enabled;
    }
}