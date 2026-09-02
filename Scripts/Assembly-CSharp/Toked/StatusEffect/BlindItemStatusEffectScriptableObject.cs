using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Toked.StatusEffect;

[CreateAssetMenu(fileName = "BlindItemStatusEffectScriptableObject", menuName = "WMO/ScriptableObjects/StatusEffect/BlindItemStatusEffectScriptableObject", order = 0)]
public class BlindItemStatusEffectScriptableObject : CurseItemStatusEffectScriptableObjectBase
{
	[SerializeField]
	private float _vignetteIntensity = 1f;

	[SerializeField]
	private Vector2 _centerPosition = new Vector2(0.5f, 0.6f);

	[SerializeField]
	private float _smoothness = 1f;

	[SerializeField]
	private bool _rounded = true;

	private CameraGame _cameraGame;

	private GameObject _mapUI;

	private bool _fistMapCondition;

	private Tweener _vignetteTweener;

	public override void ApplyEffect(PlayerController playerController, StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		if (statusEffectController.PlayerController.network.isLocalPlayer && !statusEffect.HasAntiStatusEffect)
		{
			if (_cameraGame == null)
			{
				SetBlindEffect();
			}
			if (_mapUI == null)
			{
				SetMapUI(setActive: true, CheckMapUIChange());
			}
		}
	}

	public override IEnumerator OnApplyEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		yield return base.OnApplyEffect(statusEffectController, statusEffect);
		if (statusEffectController.PlayerController.network.isLocalPlayer && !statusEffect.HasAntiStatusEffect && !statusEffectController.CheckContainEffectStatus(_statusEffectData.BaseName, _statusEffectData.Name))
		{
			SetBlindEffect();
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

	private void SetBlindEffect()
	{
		SetCameraEffect();
		SetMapUI(setActive: false, CheckMapUIChange());
	}

	private void RemoveBlindEffect()
	{
		RemoveCameraEffect();
		SetMapUI(setActive: true, CheckMapUIChange());
	}

	private void SetCameraEffect()
	{
		if (_cameraGame == null || _cameraGame != CameraGame.Instance)
		{
			_cameraGame = CameraGame.Instance;
		}
		SetVignetteEffect(_vignetteIntensity, _centerPosition, _smoothness, _rounded);
	}

	private void RemoveCameraEffect()
	{
		ResetVignetteEffect();
		_cameraGame = null;
	}

	private void ResetVignetteEffect()
	{
		if (_cameraGame != null)
		{
			_cameraGame.ResetVignetteEffect();
		}
	}

	private void SetVignetteEffect(float intensity, Vector2 centerPosition, float smoothness, bool rounded)
	{
		Vignette vignette = _cameraGame?.VignetteEffect;
		if ((bool)vignette)
		{
			SetVignetteIntensityValue(intensity, withAnimation: true, vignette.intensity.value);
			vignette.intensity.value = intensity;
			vignette.center.value = centerPosition;
			vignette.smoothness.value = smoothness;
			vignette.rounded.value = rounded;
		}
	}

	private void SetVignetteIntensityValue(float value, bool withAnimation = false, float from = 0.45f, float duration = 0.6f)
	{
		Vignette vignette = _cameraGame?.VignetteEffect;
		if (!vignette)
		{
			return;
		}
		_vignetteTweener?.Kill(complete: true);
		if (withAnimation)
		{
			_vignetteTweener = DOTween.To((float intensityValue) =>
			{
				vignette.intensity.value = intensityValue;
			}, from, value, duration);
		}
		else
		{
			vignette.intensity.value = value;
		}
	}

	private void SetMapUI(bool setActive, bool fistMapCondition = true)
	{
		if (!(_mapUI == null))
		{
			_mapUI.transform.parent.gameObject.SetActive(setActive);
			if (setActive)
			{
				_mapUI = null;
			}
		}
	}

	private bool CheckMapUIChange()
	{
		if (_mapUI == null || _mapUI != UIGameManager.Instance.gameObject)
		{
			_mapUI = UIGameManager.Instance.mapUI;
			_fistMapCondition = (bool)_mapUI && _mapUI.activeSelf;
		}
		return _fistMapCondition;
	}

	public override void CheckHaveAntiStatusEffect(StatusEffectController.StatusEffect statusEffect)
	{
		base.CheckHaveAntiStatusEffect(statusEffect);
		if (statusEffect.statusEffectController.PlayerController.network.isLocalPlayer)
		{
			if (statusEffect.HasAntiStatusEffect)
			{
				RemoveBlindEffect();
			}
			else
			{
				SetBlindEffect();
			}
		}
	}
}
