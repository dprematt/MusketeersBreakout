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

	[Header("Audio Clips")]
	public Slider hudSlider;
	public Slider dodgeSlider;
	public Slider jumpSlider;
	public AudioClip openHUDClip;
	public AudioClip closeHUDClip;
	public AudioClip dodgeClip;
	public AudioClip jumpClip;
	public AudioClip lootHUDClip;
	private AudioSource hudSource;
	private AudioSource audioSource;
	private AudioSource dodgeSource;
	private AudioSource jumpSource;


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

	private Coroutine attackSlowdownCoroutine;

	[SerializeField] public AudioClip blockingSound;

	private void Start()
	{
		if (photonView.IsMine) {
		HealthManager = GetComponent<HealthManager>();
		PFInventory_ = GetComponent<PlayFabInventory>();
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;
		health = maxHealth;
		xp = 1;
		level = 1;
		max_xp = 20;
		health_up = 30;
		inventory = gameObject.GetComponent<Inventory>();
		HUD = transform.Find("HUD").gameObject;
		HUD.GetComponent<HUD>().init();
		HUDFixe = FindDeepChild(transform, "HUDFixe").gameObject;
		HUDFixe.GetComponent<HUDFixe>().init();
		LootHUD = transform.Find("LootHUD").gameObject;
		GameObject xpProgressBarExperience = GameObject.FindWithTag("ExperienceBarDefaultTag");
		GameObject xpProgressBarLevel = GameObject.FindWithTag("ExperienceBarLevelTag");
		GameObject xpProgressBarXp = GameObject.FindWithTag("ExperienceBarXpTag");
		XpProgressBar = xpProgressBarExperience.GetComponent<Image>();
		levelText2D = xpProgressBarLevel.GetComponent<Text>();
		xpText2D = xpProgressBarXp.GetComponent<Text>();
		cylinderTransform = transform;
		originalHeight = cylinderTransform.localScale.y;
		eventListener = transform.Find("PlayerBody").GetComponent<EventListener>();
		audioSource = GetComponent<AudioSource>();
		hudSource = gameObject.AddComponent<AudioSource>();
		audioSource = gameObject.AddComponent<AudioSource>();
		dodgeSource = gameObject.AddComponent<AudioSource>();
		jumpSource = gameObject.AddComponent<AudioSource>();
		dodgeSource.clip = dodgeClip;
		jumpSource.clip = jumpClip;
		if (hudSlider != null)
		{
			hudSlider.onValueChanged.AddListener(SetHUDVolume);
			hudSlider.value = hudSource.volume;
		}

		if (dodgeSlider != null)
		{
			dodgeSlider.onValueChanged.AddListener(SetDodgeVolume);
			dodgeSlider.value = dodgeSource.volume;
		}

		if (jumpSlider != null)
		{
			jumpSlider.onValueChanged.AddListener(SetJumpVolume);
			jumpSlider.value = jumpSource.volume;
		}
		//StartCoroutine(DamageOverTime());
		}
	}
	public Weapon RetrieveEquippedWeapon(string hand)
	{
		Transform weapons = FindDeepChild(gameObject.transform, hand);
		if (weapons != null)
		{
			foreach (Transform child in weapons.transform)
			{
				if (child.gameObject.active == true)
				{
					Weapon weapon = child.gameObject.GetComponent<Weapon>();
					if (weapon == null)
					{
						return null;
					}
					return weapon;
				}
			}
		}
		return null;
	}

	private void Update()
	{
		if (photonView.IsMine) {
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
			PlayJumpSound();
			Jump();
		}

		float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
		if ((scrollDelta > 0f || scrollDelta < 0f) && inventory.PocketCount() > 1 && anim.GetInteger("intAttackPhase") == 0)
		{
			inventory.SwapItems(0, 1);
			if (inventory.mItems[0].GameObject == null)
			{
				EquippedWeapon = null;
			}
			else if (inventory.mItems[0].GameObject.GetComponent<Weapon>() != null)
            {
				EquippedWeapon = inventory.mItems[0].GameObject.GetComponent<Weapon>();
				EquippedWeapon.setAnim(gameObject);
				eventListener.weaponComp = EquippedWeapon;
				EquippedWeapon.whenPickUp(gameObject);
			}
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
				PlayHUDSound(closeHUDClip);
			}
			else
			{
				HUD.SetActive(true);
				PlayHUDSound(openHUDClip);
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
			PlayHUDSound(lootHUDClip);
			LootHUD.SetActive(false);
		}

		if (hasShield && Input.GetMouseButton(1))
		{
			if (shieldComp != null)
				shieldComp.setProtectionMode(true);
		}
		else if (hasShield && Input.GetMouseButtonUp(1))
		{
			if (shieldComp != null)
				shieldComp.setProtectionMode(false);
		}

		//Weapon MainWeapon = RetrieveEquippedWeapon("jointItemR");
		if (EquippedWeapon != null)
		{
			if (Input.GetMouseButtonDown(0) && EquippedWeapon.isLongRange == false)
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
		/*
		if (inventory.PocketCount() > 0)
		{
			if (EquippedWeapon != null)
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
		}
		*/
		}  
	}

	public void SetHUDVolume(float volume)
	{
		hudSource.volume = volume;
	}

	public void SetDodgeVolume(float volume)
	{
		dodgeSource.volume = volume;
	}

	public void SetJumpVolume(float volume)
	{
		jumpSource.volume = volume;
	}

	private void PlayDodgeSound()
	{
		dodgeSource.PlayOneShot(dodgeSource.clip);
	}

	private void PlayJumpSound()
	{
		jumpSource.PlayOneShot(jumpSource.clip);
	}

	private void PlayHUDSound(AudioClip clip)
	{
		if (clip != null)
		{
			hudSource.PlayOneShot(clip);
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

	public void SetWeaponEvents(Weapon weaponComp)
	{
		weaponComp.setAnim(gameObject);
		currentWeapon = 0;
		EquippedWeapon = weaponComp;
		eventListener.weaponComp = weaponComp;
	}

	public void EquipWeapon(Weapon weaponComp, GameObject weaponObject, bool toAdd)
	{
		/*
		if (weaponComp != null && toAdd == true)
			inventory.AddItem(weaponComp);
		Transform hand = FindDeepChild(transform, "jointItemR");
		if (weaponComp == null || weaponObject == null)
		{
			weaponComp.whenPickUp(null);
			return;
		}   
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
		EquippedWeapon = weaponComp;*/
	}

	void OnTriggerEnter(Collider col)
	{
		Bullet bullet = col.GetComponent<Bullet>();
		if (bullet != null && bullet.shooter != null && bullet.shooter.ActorNumber != photonView.Owner.ActorNumber)
		{
			if (hasShield && shieldComp.isProtecting)
			{
				shield.GetComponent<ParticleSystem>().Play();
				audioSource.PlayOneShot(blockingSound);
				anim.SetTrigger("hitted");
				return;
			}
			TakeDamage(10);
			bullet.GetComponent<PhotonView>().RPC("Destroy", RpcTarget.AllBuffered);
		}

		if (col.CompareTag("ExtractionZone"))
		{
			HealthManager.TransferToNextPlayer();
			PFInventory_.PlayerWin();
			PhotonNetwork.Destroy(gameObject);
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
					shieldComp.gameObject.GetComponent<ParticleSystem>().Play();
					audioSource.PlayOneShot(blockingSound);
					anim.SetTrigger("hitted");
					return;
				}
				TakeDamage(weaponComp.damages);
			}
		}

		if (col.gameObject.name == "AmmoBox")
		{
			IsometricAiming aim = gameObject.GetComponent<IsometricAiming>();
			if (aim.maxAmmo < 5)
			{
				aim.maxAmmo++;
				aim.UpdateAmmoUI();
			}
		}
	}

	void MyInput()
	{
		if (photonView.IsMine) {
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
		if (photonView.IsMine) {
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
			PlayDodgeSound();
			Vector3 dodgeDirection = characterModel.forward;
			rb.AddForce(dodgeDirection.normalized * 50f * movementMultiplier, ForceMode.Acceleration);
			anim.SetBool("isDodging", true);
		}
		else if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0) && anim.GetBool("isDodging"))
		{
			anim.SetBool("isDodging", false);
		}
		}
	}

	void MovePlayer()
	{
		if (photonView.IsMine) {
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

	public void StartAttackSlowdown(float duration, float speedMultiplier)
	{
		if (attackSlowdownCoroutine != null)
		{
			StopCoroutine(attackSlowdownCoroutine);
		}
		attackSlowdownCoroutine = StartCoroutine(SlowdownCoroutine(duration, speedMultiplier));
	}

	private IEnumerator SlowdownCoroutine(float duration, float speedMultiplier)
	{
		isAttacking = true;
		float originalSpeed = moveSpeed;
		moveSpeed *= speedMultiplier;
		yield return new WaitForSeconds(duration);
		moveSpeed = originalSpeed;
		isAttacking = false;
		attackSlowdownCoroutine = null;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(gameObject.transform.position);
			stream.SendNext(gameObject.transform.rotation);
		}
		else
		{
			gameObject.transform.position = (Vector3)stream.ReceiveNext();
			gameObject.transform.rotation = (Quaternion)stream.ReceiveNext();
		}
	}
}