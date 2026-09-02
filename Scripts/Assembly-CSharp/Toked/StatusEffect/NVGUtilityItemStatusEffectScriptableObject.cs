using System.Collections;
using DarkTonic.MasterAudio;
using UnityEngine;

namespace Toked.StatusEffect;

[CreateAssetMenu(fileName = "NVGUtilityItemStatusEffectScriptableObject", menuName = "WMO/ScriptableObjects/StatusEffect/NVGUtilityItemStatusEffectScriptableObject", order = 0)]
public class NVGUtilityItemStatusEffectScriptableObject : UtilityItemStatusEffectScriptableObjectBase
{
	[SerializeField]
	private Color _filterColor = new Color32(50, byte.MaxValue, 100, byte.MaxValue);

	[SerializeField]
	private float _postExposureValue = 2f;

	[SoundGroup]
	[SerializeField]
	private string _nvgSfx;

	private CameraGame _cameraGame;

	private Coroutine _cameraEffect;

	public override void ApplyEffect(PlayerController playerController, StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		if (statusEffectController.PlayerController.network.isLocalPlayer && !statusEffect.HasAntiStatusEffect && _cameraGame == null)
		{
			SetNvgEffect(playSfx: false);
		}
	}

	public override IEnumerator OnApplyEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		yield return base.OnApplyEffect(statusEffectController, statusEffect);
		if (statusEffectController.PlayerController.network.isLocalPlayer && !statusEffect.HasAntiStatusEffect && !statusEffectController.CheckContainEffectStatus(_statusEffectData.BaseName, _statusEffectData.Name))
		{
			SetNvgEffect();
		}
	}

	public override void RemoveEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		base.RemoveEffect(statusEffectController, statusEffect);
		if (statusEffectController.PlayerController.network.isLocalPlayer && !statusEffectController.CheckContainEffectStatus(_statusEffectData.BaseName))
		{
			RemoveBlindEffect();
		}
	}

	private void SetNvgEffect(bool playSfx = true)
	{
		SetCameraEffect(playSfx);
	}

	private void RemoveBlindEffect()
	{
		RemoveCameraEffect();
	}

	private void SetCameraEffect(bool playSfx = true)
	{
		if (_cameraGame == null || _cameraGame != CameraGame.Instance)
		{
			_cameraGame = CameraGame.Instance;
		}
		StopCameraEffectCoroutine();
		_cameraEffect = _cameraGame.StartCoroutine(DoSetCameraEffect(playSfx));
	}

	private void StopCameraEffectCoroutine()
	{
		if (_cameraEffect != null)
		{
			_cameraGame?.StopCoroutine(_cameraEffect);
			_cameraEffect = null;
		}
	}

	private IEnumerator DoSetCameraEffect(bool playSfx = true)
	{
		if (playSfx)
		{
			AudioManager.PlaySFX(_nvgSfx);
		}
		SetColorFilterEffect();
		yield return null;
	}

	private void RemoveCameraEffect()
	{
		RemoveColorFilterEffect();
		RemoveostExposureEffect();
		_cameraGame = null;
	}

	private void SetColorFilterEffect()
	{
		_cameraGame?.SetColorFilter(Color.white, 0.6f);
		_cameraGame?.SetNVGFilter(_filterColor, _postExposureValue, 0.6f);
	}

	private void RemoveColorFilterEffect()
	{
		_cameraGame?.ResetColorFilter();
		_cameraGame?.RemoveNVGFilter();
	}

	private void SetPostExposureEffect()
	{
		_cameraGame?.SetPostExposure(_postExposureValue, 1f);
	}

	private void RemoveostExposureEffect()
	{
		_cameraGame?.ResetPostExposure();
	}
}
