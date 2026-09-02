using MoreMountains.Feedbacks;
using UnityEngine;

public class ObjectImpactPool : MonoBehaviour
{
	public enum ImpactType
	{
		Brick = 0,
		Metal = 1,
		Glass = 2,
		Wood = 3,
		Grenade = 4,
		Blood = 5,
		BloodOmni = 6,
		GLauncher = 7
	}

	public float timeDespawn;

	private float timePassed;

	private bool despawned;

	public ImpactType typeImpact;

	public GameObject brickImpact;

	public GameObject metalImpact;

	public GameObject glassImpact;

	public GameObject woodImpact;

	public GameObject grenadeImpact;

	public GameObject bloodImpact;

	public GameObject bloodImpactOmni;

	public GameObject gLauncher;

	private int rdm;

	[SerializeField]
	private MMF_Player mmfGrenade;

	private void OnEnable()
	{
		despawned = false;
		timePassed = 0f;
	}

	public void Init(ImpactType impactType, float despawnTime)
	{
		timeDespawn = despawnTime;
		typeImpact = impactType;
		initType();
	}

	public void initType()
	{
		brickImpact.SetActive(value: false);
		metalImpact.SetActive(value: false);
		glassImpact.SetActive(value: false);
		woodImpact.SetActive(value: false);
		grenadeImpact.SetActive(value: false);
		bloodImpact.SetActive(value: false);
		bloodImpactOmni.SetActive(value: false);
		gLauncher.SetActive(value: false);
		switch (typeImpact)
		{
		case ImpactType.Brick:
			brickImpact.SetActive(value: true);
			break;
		case ImpactType.Metal:
			metalImpact.SetActive(value: true);
			break;
		case ImpactType.Glass:
			glassImpact.SetActive(value: true);
			break;
		case ImpactType.Wood:
			woodImpact.SetActive(value: true);
			break;
		case ImpactType.Grenade:
			mmfGrenade.Initialization();
			grenadeImpact.SetActive(value: true);
			break;
		case ImpactType.Blood:
			bloodImpact.SetActive(value: true);
			break;
		case ImpactType.BloodOmni:
			bloodImpactOmni.SetActive(value: true);
			break;
		case ImpactType.GLauncher:
			gLauncher.SetActive(value: true);
			break;
		}
	}

	private void FixedUpdate()
	{
		timePassed += Time.deltaTime;
		if (!despawned && timePassed > timeDespawn)
		{
			despawned = true;
			brickImpact.SetActive(value: false);
			metalImpact.SetActive(value: false);
			glassImpact.SetActive(value: false);
			woodImpact.SetActive(value: false);
			grenadeImpact.SetActive(value: false);
			bloodImpact.SetActive(value: false);
			bloodImpactOmni.SetActive(value: false);
			gLauncher.SetActive(value: false);
			ImpactSpawner.Instance.Release(this);
		}
	}
}
