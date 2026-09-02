using UnityEngine;
using UnityEngine.UI;

namespace DestroyIt;

public class HeadsUpDisplay : MonoBehaviour
{
	public RectTransform hud;

	[Tooltip("The number of times per second the UI updates.")]
	public float updateRate = 15f;

	public Text destroyedPrefabsText;

	public Text destroyedParticlesText;

	public Text debrisCountText;

	public Image reticleImage;

	public Image selectedWeapon;

	public Sprite unknownWeapon;

	public Sprite assaultRifle;

	public Sprite rocketLauncher;

	public Sprite fireAxe;

	public Sprite cannon;

	public Sprite nuke;

	public Sprite wrench;

	private float nextUpdate;

	private void Start()
	{
		if (DestructionManager.Instance != null)
		{
			DestructionManager.Instance.DestroyedPrefabCounterChangedEvent += OnDestroyedPrefabCounterChanged;
			DestructionManager.Instance.ActiveDebrisCounterChangedEvent += OnActiveDebrisCounterChanged;
		}
		if (ParticleManager.Instance != null)
		{
			ParticleManager.Instance.ActiveParticlesCounterChangedEvent += OnActiveParticlesCounterChanged;
		}
		OnDestroyedPrefabCounterChanged();
		OnActiveDebrisCounterChanged();
		OnActiveParticlesCounterChanged();
	}

	private void OnDisable()
	{
		if (DestructionManager.Instance != null)
		{
			DestructionManager.Instance.DestroyedPrefabCounterChangedEvent -= OnDestroyedPrefabCounterChanged;
			DestructionManager.Instance.ActiveDebrisCounterChangedEvent -= OnActiveDebrisCounterChanged;
		}
		if (ParticleManager.Instance != null)
		{
			ParticleManager.Instance.ActiveParticlesCounterChangedEvent -= OnActiveParticlesCounterChanged;
		}
	}

	private void OnDestroyedPrefabCounterChanged()
	{
		destroyedPrefabsText.text = "Destroyed Prefabs (last " + DestructionManager.Instance.withinSeconds + "s): " + DestructionManager.Instance.DestroyedPrefabCounter.Count;
	}

	private void OnActiveParticlesCounterChanged()
	{
		destroyedParticlesText.text = "Destroyed Particles (last " + ParticleManager.Instance.withinSeconds + "s): " + ParticleManager.Instance.ActiveParticles.Length;
	}

	private void OnActiveDebrisCounterChanged()
	{
		debrisCountText.text = "Debris Count: " + DestructionManager.Instance.ActiveDebrisCount;
	}

	private void Update()
	{
		if (!(Time.time > nextUpdate))
		{
			return;
		}
		nextUpdate = Time.time + 1f / updateRate;
		int num = PlayerPrefs.GetInt("ShowHud", -1);
		hud.gameObject.SetActive(num == -1);
		int num2 = PlayerPrefs.GetInt("ShowReticle", -1);
		reticleImage.gameObject.SetActive(num2 == -1);
		if (InputManager.Instance != null)
		{
			switch (InputManager.Instance.SelectedWeapon)
			{
			case WeaponType.Gun:
				selectedWeapon.sprite = assaultRifle;
				break;
			case WeaponType.Rocket:
				selectedWeapon.sprite = rocketLauncher;
				break;
			case WeaponType.Melee:
				selectedWeapon.sprite = fireAxe;
				break;
			case WeaponType.Cannonball:
				selectedWeapon.sprite = cannon;
				break;
			case WeaponType.Nuke:
				selectedWeapon.sprite = nuke;
				break;
			case WeaponType.RepairWrench:
				selectedWeapon.sprite = wrench;
				break;
			default:
				selectedWeapon.sprite = unknownWeapon;
				break;
			}
		}
	}
}
