using UnityEngine;
using UnityEngine.SceneManagement;

namespace DestroyIt;

[DisallowMultipleComponent]
public class InputManager : MonoBehaviour
{
	public GameObject cannonballPrefab;

	public float cannonballVelocity = 75f;

	public GameObject rocketPrefab;

	public GameObject bulletPrefab;

	public ParticleSystem muzzleFlash;

	public ParticleSystem cannonFire;

	public ParticleSystem rocketFire;

	public Light muzzleLight;

	public GameObject launcherRocket;

	public int bulletDamage = 15;

	public float bulletForcePerSecond = 25f;

	public float bulletForceFrequency = 10f;

	[Range(1f, 30f)]
	public int gunShotsPerSecond = 8;

	public float startDistance = 1.5f;

	public WeaponType startingWeapon = WeaponType.Rocket;

	public GameObject nukePrefab;

	public float shockwaveSpeed = 800f;

	public GameObject shockWallPrefab;

	public GameObject dustWallPrefab;

	public int dustWallDistance = 120;

	public GameObject groundChurnPrefab;

	public int nukeDistance = 2500;

	public int groundChurnDistance = 90;

	[Range(0.1f, 0.5f)]
	public float timeSlowSpeed = 0.25f;

	public GameObject windZone;

	private bool timeSlowed;

	private bool timeStopped;

	private float timeBetweenShots;

	private float meleeAttackDelay;

	private float lastShotTime;

	private float lastMeleeTime;

	private int playerPrefShowReticle = -1;

	private int playerPrefShowHud = -1;

	private float rocketTimer;

	private float nukeTimer;

	private CharacterController firstPersonController;

	private Transform gunTransform;

	private Transform cannonTransform;

	private Transform rocketTransform;

	private Transform nukeTransform;

	private Transform axeTransform;

	private Transform repairWrenchTransform;

	public WeaponType SelectedWeapon { get; set; }

	public static InputManager Instance { get; private set; }

	private InputManager()
	{
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		if (Camera.main == null || Camera.main.transform.parent == null)
		{
			return;
		}
		firstPersonController = Camera.main.transform.parent.GetComponent<CharacterController>();
		if (firstPersonController == null)
		{
			Debug.LogError("InputManager: Could not find Character Controller on Main Camera parent.");
		}
		foreach (Transform item in Camera.main.transform)
		{
			switch (item.name)
			{
			case "WeaponPosition-Nuke":
				nukeTransform = item;
				break;
			case "WeaponPosition-Gun":
				gunTransform = item;
				break;
			case "WeaponPosition-Axe":
				axeTransform = item;
				break;
			case "WeaponPosition-Cannon":
				cannonTransform = item;
				break;
			case "WeaponPosition-Rocket":
				rocketTransform = item;
				break;
			case "WeaponPosition-RepairWrench":
				repairWrenchTransform = item;
				break;
			default:
				gunTransform = item;
				break;
			}
		}
		if (muzzleLight != null && muzzleLight.enabled)
		{
			muzzleLight.enabled = false;
		}
		timeBetweenShots = 1f / (float)gunShotsPerSecond;
		meleeAttackDelay = 0.6f;
		lastShotTime = 0f;
		lastMeleeTime = 0f;
		SetTimeScale();
		int num = PlayerPrefs.GetInt("SelectedWeapon", -1);
		if (num == -1)
		{
			SelectedWeapon = startingWeapon;
		}
		else
		{
			SelectedWeapon = (WeaponType)num;
		}
		playerPrefShowHud = PlayerPrefs.GetInt("ShowHud", -1);
		playerPrefShowReticle = PlayerPrefs.GetInt("ShowReticle", -1);
		SetActiveWeapon();
	}

	private void Update()
	{
		if (nukeTimer > 0f)
		{
			nukeTimer -= Time.deltaTime;
		}
		if (nukeTimer < 0f)
		{
			nukeTimer = 0f;
		}
		if (rocketTimer > 0f)
		{
			rocketTimer -= Time.deltaTime;
		}
		if (rocketTimer <= 0f)
		{
			if (launcherRocket != null)
			{
				launcherRocket.SetActive(value: true);
			}
			rocketTimer = 0f;
		}
		if (Input.GetButtonDown("Fire1"))
		{
			switch (SelectedWeapon)
			{
			case WeaponType.Cannonball:
				if (cannonFire != null)
				{
					cannonFire.GetComponent<ParticleSystem>().Clear(withChildren: true);
					cannonFire.Play(withChildren: true);
				}
				WeaponHelper.Launch(cannonballPrefab, cannonTransform, startDistance, cannonballVelocity, randomRotation: true);
				break;
			case WeaponType.Rocket:
				if (rocketTimer <= 0f)
				{
					if (launcherRocket != null)
					{
						launcherRocket.SetActive(value: false);
					}
					if (rocketFire != null)
					{
						rocketFire.GetComponent<ParticleSystem>().Clear(withChildren: true);
						rocketFire.Play(withChildren: true);
					}
					WeaponHelper.Launch(rocketPrefab, rocketTransform, startDistance + 0.1f, 6f, randomRotation: false);
					RocketLoading componentInChildren = launcherRocket.GetComponentInChildren<RocketLoading>();
					if (componentInChildren != null)
					{
						componentInChildren.isLoaded = false;
					}
					rocketTimer = 1f;
				}
				break;
			case WeaponType.Nuke:
				if (nukeTimer <= 0f)
				{
					FadeIn fadeIn = base.gameObject.AddComponent<FadeIn>();
					fadeIn.startColor = Color.white;
					fadeIn.fadeLength = 5f;
					Transform transform = GameObject.FindGameObjectWithTag("Player").transform;
					Vector3 position = transform.position + transform.forward * nukeDistance;
					Vector3 vector = new Vector3(position.x, 0f, position.z);
					if (groundChurnPrefab != null)
					{
						Follow follow = Object.Instantiate(groundChurnPrefab, vector, Quaternion.identity).AddComponent<Follow>();
						follow.isPositionFixed = true;
						follow.objectToFollow = transform;
						follow.facingDirection = FacingDirection.FixedPosition;
						follow.fixedFromPosition = vector;
						follow.fixedDistance = groundChurnDistance;
					}
					Object.Instantiate(nukePrefab, vector, Quaternion.Euler(Vector3.zero)).transform.LookAt(transform);
					if (windZone != null)
					{
						windZone.transform.position = position;
						windZone.transform.LookAt(transform);
						Invoke("EnableWindZone", 5f);
						DisableAfter obj = windZone.GetComponent<DisableAfter>() ?? windZone.AddComponent<DisableAfter>();
						obj.seconds = 25f;
						obj.removeScript = true;
					}
					if (dustWallPrefab != null)
					{
						GameObject gameObject = Object.Instantiate(dustWallPrefab, position, Quaternion.Euler(Vector3.zero));
						gameObject.transform.LookAt(transform);
						gameObject.transform.position += gameObject.transform.forward * dustWallDistance;
						gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * shockwaveSpeed, ForceMode.Force);
						gameObject.GetComponent<DustWall>().fixedFromPosition = vector;
					}
					if (shockWallPrefab != null)
					{
						GameObject gameObject2 = Object.Instantiate(shockWallPrefab, position, Quaternion.Euler(Vector3.zero));
						gameObject2.transform.LookAt(transform);
						gameObject2.GetComponent<Rigidbody>().AddForce(gameObject2.transform.forward * shockwaveSpeed, ForceMode.Force);
						gameObject2.GetComponent<ShockWall>().origin = vector;
					}
					Invoke("BroadcastNukeStart", 0.1f);
					Invoke("BroadcastNukeEnd", 25f);
					nukeTimer = 30f;
				}
				break;
			case WeaponType.Gun:
				FireGun();
				break;
			case WeaponType.Melee:
				if (Time.time >= lastMeleeTime + meleeAttackDelay)
				{
					MeleeAttack();
				}
				break;
			case WeaponType.RepairWrench:
				if (Time.time >= lastMeleeTime + meleeAttackDelay)
				{
					RepairByHand();
				}
				break;
			}
		}
		if (Input.GetButton("Fire1") && SelectedWeapon == WeaponType.Gun && Time.time >= lastShotTime + timeBetweenShots)
		{
			FireGun();
		}
		if (Input.GetButton("Fire1") && SelectedWeapon == WeaponType.Melee && Time.time >= lastMeleeTime + meleeAttackDelay)
		{
			MeleeAttack();
		}
		if (Input.GetButton("Fire1") && SelectedWeapon == WeaponType.RepairWrench && Time.time >= lastMeleeTime + meleeAttackDelay)
		{
			RepairByHand();
		}
		if (Input.GetKeyUp("t"))
		{
			timeSlowed = !timeSlowed;
			SetTimeScale();
		}
		if (Input.GetKeyUp("y"))
		{
			timeStopped = !timeStopped;
			SetTimeScale();
		}
		if (timeSlowed)
		{
			Object[] array = Object.FindObjectsOfType(typeof(GameObject));
			for (int i = 0; i < array.Length; i++)
			{
				Rigidbody[] componentsInChildren = ((GameObject)array[i]).GetComponentsInChildren<Rigidbody>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].interpolation = RigidbodyInterpolation.Interpolate;
				}
			}
		}
		if (Input.GetKey("r"))
		{
			TreeManager instance = TreeManager.Instance;
			if (instance != null)
			{
				instance.RestoreTrees();
			}
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
		if (Input.GetKeyUp("q"))
		{
			SelectedWeapon = WeaponHelper.GetPrevious(SelectedWeapon);
			PlayerPrefs.SetInt("SelectedWeapon", (int)SelectedWeapon);
			SetActiveWeapon();
		}
		if (Input.GetKeyUp("e"))
		{
			SelectedWeapon = WeaponHelper.GetNext(SelectedWeapon);
			PlayerPrefs.SetInt("SelectedWeapon", (int)SelectedWeapon);
			SetActiveWeapon();
		}
		if (Input.GetKeyUp("o"))
		{
			if (playerPrefShowReticle == -1)
			{
				playerPrefShowReticle = 0;
			}
			else
			{
				playerPrefShowReticle = -1;
			}
			PlayerPrefs.SetInt("ShowReticle", playerPrefShowReticle);
		}
		if (Input.GetKeyUp("h"))
		{
			if (playerPrefShowHud == -1)
			{
				playerPrefShowHud = 0;
			}
			else
			{
				playerPrefShowHud = -1;
			}
			PlayerPrefs.SetInt("ShowHud", playerPrefShowHud);
		}
		if (Input.GetKeyUp("m"))
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			TreeManager instance2 = TreeManager.Instance;
			if (instance2 != null)
			{
				instance2.RestoreTrees();
			}
			SceneManager.LoadScene("Choose Demo");
		}
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis > 0f)
		{
			SelectedWeapon = WeaponHelper.GetNext(SelectedWeapon);
			PlayerPrefs.SetInt("SelectedWeapon", (int)SelectedWeapon);
			SetActiveWeapon();
		}
		if (axis < 0f)
		{
			SelectedWeapon = WeaponHelper.GetPrevious(SelectedWeapon);
			PlayerPrefs.SetInt("SelectedWeapon", (int)SelectedWeapon);
			SetActiveWeapon();
		}
	}

	private void EnableWindZone()
	{
		if (windZone != null)
		{
			windZone.SetActive(value: true);
		}
	}

	private void SetActiveWeapon()
	{
		gunTransform.gameObject.SetActive(SelectedWeapon == WeaponType.Gun);
		cannonTransform.gameObject.SetActive(SelectedWeapon == WeaponType.Cannonball);
		rocketTransform.gameObject.SetActive(SelectedWeapon == WeaponType.Rocket);
		nukeTransform.gameObject.SetActive(SelectedWeapon == WeaponType.Nuke);
		axeTransform.gameObject.SetActive(SelectedWeapon == WeaponType.Melee);
		repairWrenchTransform.gameObject.SetActive(SelectedWeapon == WeaponType.RepairWrench);
	}

	private void MeleeAttack()
	{
		axeTransform.GetComponentInChildren<Animation>().Play("Axe Swinging");
		lastMeleeTime = Time.time;
		Invoke("BroadcastMeleeDamage", 0.2f);
	}

	private void RepairByHand()
	{
		repairWrenchTransform.GetComponentInChildren<Animation>().Play("Wrench Turn");
		lastMeleeTime = Time.time;
		Invoke("BroadcastRepairDamage", 0.2f);
	}

	private void BroadcastMeleeDamage()
	{
		firstPersonController.BroadcastMessage("OnMeleeDamage");
	}

	private void BroadcastRepairDamage()
	{
		firstPersonController.BroadcastMessage("OnMeleeRepair");
	}

	private void BroadcastNukeStart()
	{
		firstPersonController.BroadcastMessage("OnNukeStart");
	}

	private void BroadcastNukeEnd()
	{
		firstPersonController.BroadcastMessage("OnNukeEnd");
	}

	private void FireGun()
	{
		if (muzzleFlash != null)
		{
			muzzleFlash.Emit(1);
		}
		if (muzzleLight != null && !muzzleLight.enabled)
		{
			muzzleLight.enabled = true;
			Invoke("DisableMuzzleLight", 0.1f);
		}
		WeaponHelper.Launch(bulletPrefab, gunTransform, 0f, randomRotation: false);
		lastShotTime = Time.time;
	}

	private void DisableMuzzleLight()
	{
		if (muzzleLight != null && muzzleLight.enabled)
		{
			muzzleLight.enabled = false;
		}
	}

	private void SetTimeScale()
	{
		if (timeStopped)
		{
			Time.timeScale = 0f;
			return;
		}
		Object[] array;
		if (timeSlowed)
		{
			Time.timeScale = timeSlowSpeed;
			array = Object.FindObjectsOfType(typeof(GameObject));
			for (int i = 0; i < array.Length; i++)
			{
				Rigidbody[] componentsInChildren = ((GameObject)array[i]).GetComponentsInChildren<Rigidbody>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].interpolation = RigidbodyInterpolation.Interpolate;
				}
			}
			return;
		}
		Time.timeScale = 1f;
		array = Object.FindObjectsOfType(typeof(GameObject));
		for (int i = 0; i < array.Length; i++)
		{
			Rigidbody[] componentsInChildren = ((GameObject)array[i]).GetComponentsInChildren<Rigidbody>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].interpolation = RigidbodyInterpolation.None;
			}
		}
	}
}
