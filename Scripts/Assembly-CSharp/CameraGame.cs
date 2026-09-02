using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using MoreMountains.Feedbacks;
using StylizedWater;
using Toked;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraGame : MonoBehaviour
{
	public Camera mainCam;

	public Camera cameraMap;

	public CinemachineVirtualCamera CinemachineCam;

	public CinemachineTargetGroup CinemachineTarget;

	public CinemachineFramingTransposer camTransposer;

	private float shakeTimer;

	public int CamRotationPerClick = 90;

	public int camRotate = 45;

	public int dummy;

	public float tiltCam = 30f;

	public CutoutObject cutOut;

	[SerializeField]
	private MMF_Player feedbackWeapon;

	private MMF_CameraShake feedbackShakeWeapon;

	[SerializeField]
	private MMF_Player feedbackHurt;

	public List<Transform> object2DCam = new List<Transform>();

	[SerializeField]
	private Volume volume;

	public ColorAdjustments colorA;

	public Transform targetCursor;

	public Transform oclusionLightCollider;

	public LayerMask layerPlanar;

	public PlanarReflections planar;

	public float rotateCamSpeed = 0.6f;

	public int maxZoomOutCam;

	public int minZoomOutCam;

	public Blit nvgFeature;

	public Blit stonedFeature;

	private float _defaultVignetteIntensity;

	private Vector2 _defaultCenterValue;

	private float _defaultSmoothnessValue;

	private bool _defaultRoundedValue;

	private Vignette _vignette;

	private ChromaticAberration _chromaticAberration;

	private Bloom _bloom;

	private LensDistortion _lensDistortion;

	private ColorAdjustments _colorAdjustment;

	private Tweener _vignetteTweener;

	private bool _isUtilityUsed;

	private float _chromaticAberrationValue = 1f;

	private Tweener _chromaticAberrationTweener;

	private float _lensDistortionValue = 0.6f;

	private Tweener _lensDistortionTweener;

	private Tweener _lensPostExposureTweener;

	private Coroutine nvgTransitionCoroutine;

	private Coroutine stonedTransitionCoroutine;

	private float _stonedValue;

	public static CameraGame Instance { get; private set; }

	public Volume Volume => volume;

	public Vignette VignetteEffect
	{
		get
		{
			if (_vignette == null)
			{
				volume?.profile.TryGet<Vignette>(out _vignette);
			}
			return _vignette;
		}
	}

	public ChromaticAberration ChromaticAberration
	{
		get
		{
			if (_chromaticAberration == null)
			{
				volume?.profile.TryGet<ChromaticAberration>(out _chromaticAberration);
			}
			return _chromaticAberration;
		}
	}

	public Bloom Bloom => _bloom;

	public LensDistortion LensDistortion
	{
		get
		{
			if (_lensDistortion == null)
			{
				volume?.profile.TryGet<LensDistortion>(out _lensDistortion);
			}
			return _lensDistortion;
		}
	}

	private ColorAdjustments ColorAdjustmentEffect
	{
		get
		{
			if (_colorAdjustment == null)
			{
				volume?.profile.TryGet<ColorAdjustments>(out _colorAdjustment);
			}
			return _colorAdjustment;
		}
	}

	public event Action<int> OnCameraRotateEvent;

	private void Start()
	{
		if (volume != null)
		{
			volume.profile.TryGet<ColorAdjustments>(out colorA);
			InitDefaultVignette();
			volume.profile.TryGet<Bloom>(out _bloom);
			if (GlobalSaveData.instance.optionData.graphic == 2)
			{
				if (_bloom != null)
				{
					_bloom.active = true;
				}
				Shader.EnableKeyword("BACKLIGHT_ENABLED");
			}
			else
			{
				Shader.DisableKeyword("BACKLIGHT_ENABLED");
			}
		}
		if (planar != null)
		{
			planar.reflectionLayer = layerPlanar;
		}
		if (LobbyManager.Instance != null)
		{
			camRotate = 45;
			RotateCamera(0);
		}
		nvgFeature.SetActive(active: false);
		stonedFeature.SetActive(active: false);
	}

	private void InitDefaultVignette()
	{
		if (VignetteEffect != null)
		{
			_defaultVignetteIntensity = VignetteEffect.intensity.value;
			_defaultCenterValue = VignetteEffect.center.value;
			_defaultSmoothnessValue = VignetteEffect.smoothness.value;
			_defaultRoundedValue = VignetteEffect.rounded.value;
		}
		else
		{
			_defaultVignetteIntensity = 0.45f;
			_defaultCenterValue = new Vector2(0.5f, 0.5f);
			_defaultSmoothnessValue = 0.45f;
			_defaultRoundedValue = false;
		}
	}

	private void Awake()
	{
		mainCam = Camera.main;
		camRotate = 45;
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
		camTransposer = CinemachineCam.GetCinemachineComponent<CinemachineFramingTransposer>();
		feedbackShakeWeapon = feedbackWeapon.GetFeedbackOfType<MMF_CameraShake>();
	}

	private void OnDestroy()
	{
		nvgFeature.SetActive(active: false);
		stonedFeature.SetActive(active: false);
		Shader.SetGlobalFloat("_NVGBlendFactor", 0f);
	}

	public void RotateCamera(int value, bool isInit = false)
	{
		if (!UIGameManager.Instance.UIMenuMap.isHidden && !(LobbyManager.Instance != null))
		{
			return;
		}
		camRotate -= value;
		if (camRotate < 0)
		{
			camRotate = 315;
		}
		if (camRotate > 360)
		{
			camRotate = 45;
		}
		CinemachineCam.transform.DOLocalRotate(new Vector3(tiltCam, camRotate, 0f), rotateCamSpeed).SetEase(Ease.OutQuad);
		if (!isInit && CameraMiniMap.Instance != null)
		{
			if (GlobalSaveData.instance.optionData.autoMinimap == 1)
			{
				CameraMiniMap.Instance.transform.DOLocalRotate(new Vector3(90f, camRotate, 0f), rotateCamSpeed).SetEase(Ease.OutQuad);
			}
			foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerNetworkController)
			{
				if (item != null && !item.network.isLocalPlayer && GlobalSaveData.instance.optionData.autoMinimap == 1)
				{
					item.iconCharMap.DORotate(new Vector3(90f, 0f, -camRotate), 0f);
				}
			}
		}
		foreach (PlayerController item2 in NetworkGameManager.Instance.arrPlayerNetworkController)
		{
			if (!(item2 != null))
			{
				continue;
			}
			item2.object2D.DOLocalRotate(new Vector3(0f, camRotate, 0f), rotateCamSpeed).SetEase(Ease.OutQuad);
			if (item2.network.GetHealth() <= 0f)
			{
				item2.angleRot = item2.angleRotWithoutCam - (float)(Instance.camRotate - 45);
				if (item2.angleRot < 0f)
				{
					item2.angleRot = Mathf.RoundToInt(item2.angleRot + 360f);
				}
				else if (item2.angleRot >= 360f)
				{
					item2.angleRot = Mathf.RoundToInt(item2.angleRot - 360f);
				}
				if (Math.Abs(item2.angleRot - 360f) < 1f)
				{
					item2.angleRot = 0f;
				}
				if (item2.animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("PumpAction"))
				{
					item2.animLowerChar.Play("LegDown" + item2.angleRot);
					item2.animUpperChar.Play("Down" + item2.angleRot);
				}
			}
		}
		foreach (EnemyController item3 in GameManager.Instance.arrEnemyController)
		{
			if (!(item3 != null))
			{
				continue;
			}
			item3.object2D.transform.DOLocalRotate(new Vector3(0f, camRotate, 0f), rotateCamSpeed).SetEase(Ease.OutQuad);
			if ((bool)item3.HeadSprite && item3.HeadSprite.enabled)
			{
				item3.HeadSprite.transform.DOLocalRotate(new Vector3(0f, camRotate, 0f), rotateCamSpeed).SetEase(Ease.OutQuad);
			}
			if (item3.GetCurrentStateHash() == AnimatorHashManager.PatrolHash || item3.GetCurrentStateHash() == AnimatorHashManager.IdleHash)
			{
				item3.movement.angleDirection = item3.network.GetAngleDirection() - (Instance.camRotate - 45);
				if (item3.movement.angleDirection < 0)
				{
					item3.movement.angleDirection += 360;
				}
				item3.movement.angleAnim = item3.movement.SetAngleByCam(item3.movement.angleDirection);
				item3.movement.direction = new Vector3(Mathf.Sin(MathF.PI / 180f * (float)item3.movement.angleDirection), 0f, Mathf.Cos(MathF.PI / 180f * (float)item3.movement.angleDirection)).normalized;
				item3.movement.direction = MathFunc.IsoDirection(item3.movement.direction);
			}
		}
		foreach (ItemPickable item4 in GameManager.Instance.arrItemPickable)
		{
			if (GlobalSaveData.instance.optionData.autoMinimap == 1 && item4.itemMap != null)
			{
				item4.itemMap.transform.DOLocalRotate(new Vector3(90f, 0f, -camRotate), 0f);
			}
			if (item4.itemSprite != null)
			{
				item4.itemSprite.transform.parent.DOLocalRotate(new Vector3(item4.itemSprite.transform.localEulerAngles.x, camRotate, 0f), rotateCamSpeed).SetEase(Ease.OutQuad);
				if (camRotate == 45 || camRotate == 225)
				{
					item4.itemSprite.transform.parent.DOScaleX(Mathf.Abs(item4.itemSprite.transform.parent.localScale.x), 0f).SetDelay(rotateCamSpeed / 2f);
				}
				else
				{
					item4.itemSprite.transform.parent.DOScaleX(0f - Mathf.Abs(item4.itemSprite.transform.parent.localScale.x), 0f).SetDelay(rotateCamSpeed / 2f);
				}
			}
			if (!(item4.objectSprite != null))
			{
				continue;
			}
			item4.objectSprite.transform.DOLocalRotate(new Vector3(item4.itemSprite.transform.localEulerAngles.x, camRotate, 0f), rotateCamSpeed).SetEase(Ease.OutQuad);
			int num = 1;
			if (camRotate == 45)
			{
				if (item4.directionSprite == 225 || item4.directionSprite == 315)
				{
					num = -1;
				}
				if (item4.directionSprite == 45 || item4.directionSprite == 315)
				{
					item4.objectSprite.sprite = item4.assetSprite45;
				}
				if (item4.directionSprite == 135 || item4.directionSprite == 225)
				{
					item4.objectSprite.sprite = item4.assetSprite135;
				}
				item4.objectSprite.transform.localScale = new Vector3((float)num * item4.initScaleX, item4.objectSprite.transform.localScale.y, item4.objectSprite.transform.localScale.z);
			}
			else if (camRotate == 135)
			{
				if (item4.directionSprite == 315 || item4.directionSprite == 45)
				{
					num = -1;
				}
				if (item4.directionSprite == 225 || item4.directionSprite == 315)
				{
					item4.objectSprite.sprite = item4.assetSprite135;
				}
				if (item4.directionSprite == 135 || item4.directionSprite == 45)
				{
					item4.objectSprite.sprite = item4.assetSprite45;
				}
				item4.objectSprite.transform.localScale = new Vector3((float)num * item4.initScaleX, item4.objectSprite.transform.localScale.y, item4.objectSprite.transform.localScale.z);
			}
			else if (camRotate == 225)
			{
				if (item4.directionSprite == 45 || item4.directionSprite == 135)
				{
					num = -1;
				}
				if (item4.directionSprite == 135 || item4.directionSprite == 225)
				{
					item4.objectSprite.sprite = item4.assetSprite45;
				}
				if (item4.directionSprite == 45 || item4.directionSprite == 315)
				{
					item4.objectSprite.sprite = item4.assetSprite135;
				}
				item4.objectSprite.transform.localScale = new Vector3((float)num * item4.initScaleX, item4.objectSprite.transform.localScale.y, item4.objectSprite.transform.localScale.z);
			}
			else if (camRotate == 315)
			{
				if (item4.directionSprite == 225 || item4.directionSprite == 135)
				{
					num = -1;
				}
				if (item4.directionSprite == 225 || item4.directionSprite == 315)
				{
					item4.objectSprite.sprite = item4.assetSprite45;
				}
				if (item4.directionSprite == 135 || item4.directionSprite == 45)
				{
					item4.objectSprite.sprite = item4.assetSprite135;
				}
				item4.objectSprite.transform.localScale = new Vector3((float)num * item4.initScaleX, item4.objectSprite.transform.localScale.y, item4.objectSprite.transform.localScale.z);
			}
			if ((bool)item4.Outline)
			{
				item4.Outline.sprite = item4.objectSprite.sprite;
				item4.Outline.transform.localScale = item4.objectSprite.transform.localScale;
				item4.Outline.transform.DOLocalRotate(new Vector3(item4.itemSprite.transform.localEulerAngles.x, camRotate, 0f), rotateCamSpeed).SetEase(Ease.OutQuad);
			}
		}
		foreach (Transform item5 in object2DCam)
		{
			if (item5 != null)
			{
				item5.DOLocalRotate(new Vector3(item5.localEulerAngles.x, camRotate, 0f), rotateCamSpeed).SetEase(Ease.OutQuad);
			}
		}
		RotateRoomText();
		OnCameraRotateEvent?.Invoke(camRotate);
	}

	public void SetFixedMinimapRoomText()
	{
		RotateMinimapRoomText(isSetDefault: true);
	}

	public void RotateRoomText(bool isSetDefault = false)
	{
		if (GlobalSaveData.instance.optionData.autoMinimap == 1)
		{
			RotateMinimapRoomText(isSetDefault);
		}
	}

	private void RotateMinimapRoomText(bool isSetDefault = false)
	{
		if (!(UIGameManager.Instance.roomTextCanvas != null))
		{
			return;
		}
		for (int i = 0; i < UIGameManager.Instance.roomTextCanvas.transform.childCount; i++)
		{
			Transform child = UIGameManager.Instance.roomTextCanvas.transform.GetChild(i);
			if (!isSetDefault && (camRotate == 135 || camRotate == 225))
			{
				if (child.localEulerAngles.y == 0f || Mathf.Approximately(child.localEulerAngles.y, 180f))
				{
					child.localEulerAngles = new Vector3(child.localEulerAngles.x, 180f, child.localEulerAngles.z);
				}
				foreach (ItemInteractable item in GameManager.Instance.ListBrimCarInteractable)
				{
					item.lockMap.transform.localScale = new Vector3(item.lockMap.transform.localScale.x, 0f - item.lockMap.transform.localScale.x, item.lockMap.transform.localScale.z);
				}
			}
			else
			{
				if (child.localEulerAngles.y == 0f || Mathf.Approximately(child.localEulerAngles.y, 180f))
				{
					child.localEulerAngles = new Vector3(child.localEulerAngles.x, 0f, child.localEulerAngles.z);
				}
				foreach (ItemInteractable item2 in GameManager.Instance.ListBrimCarInteractable)
				{
					item2.lockMap.transform.localScale = new Vector3(item2.lockMap.transform.localScale.x, item2.lockMap.transform.localScale.x, item2.lockMap.transform.localScale.z);
				}
			}
			if (!isSetDefault && (camRotate == 225 || camRotate == 315))
			{
				if (Mathf.Approximately(child.localEulerAngles.y, 90f) || Mathf.Approximately(child.localEulerAngles.y, 270f))
				{
					child.localEulerAngles = new Vector3(child.localEulerAngles.x, 270f, child.localEulerAngles.z);
				}
			}
			else if (Mathf.Approximately(child.localEulerAngles.y, 90f) || Mathf.Approximately(child.localEulerAngles.y, 270f))
			{
				child.localEulerAngles = new Vector3(child.localEulerAngles.x, 90f, child.localEulerAngles.z);
			}
		}
	}

	public void CameraShake(float duration = 0.15f, float amplitude = 0.3f, float frequency = 0f)
	{
		float num = 0f;
		if (GlobalSaveData.instance.optionData.shakeLevel == 1)
		{
			num = 0.5f;
		}
		else if (GlobalSaveData.instance.optionData.shakeLevel == 2)
		{
			num = 1f;
		}
		else if (GlobalSaveData.instance.optionData.shakeLevel == 3)
		{
			num = 1.5f;
		}
		if (feedbackWeapon.IsPlaying)
		{
			feedbackWeapon.StopFeedbacks();
		}
		if (GlobalSaveData.instance.optionData.shakeLevel != 0)
		{
			feedbackShakeWeapon.CameraShakeProperties.Amplitude = amplitude * num;
			feedbackShakeWeapon.CameraShakeProperties.Duration = duration;
			if (frequency > 0f)
			{
				feedbackShakeWeapon.CameraShakeProperties.Frequency = frequency * num;
			}
			feedbackWeapon.PlayFeedbacks();
		}
	}

	public void FeedbackHurt()
	{
		feedbackHurt.PlayFeedbacks();
	}

	private void Update()
	{
		if (shakeTimer > 0f)
		{
			shakeTimer -= Time.deltaTime;
			if (shakeTimer <= 0f)
			{
				CinemachineCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
			}
		}
		if (oclusionLightCollider != null)
		{
			Plane plane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
			Ray ray = new Ray(CinemachineCam.transform.position, CinemachineCam.transform.forward);
			if (plane.Raycast(ray, out var enter))
			{
				oclusionLightCollider.position = ray.GetPoint(enter);
			}
		}
	}

	public void ZoomIn(float value = 1f)
	{
		if (camTransposer.m_CameraDistance > 10f)
		{
			camTransposer.m_CameraDistance -= value;
		}
	}

	public void ZoomOut(float value = 1f)
	{
		if (camTransposer.m_CameraDistance < 50f)
		{
			camTransposer.m_CameraDistance += value;
		}
	}

	public void TiltUp(float value = 1f)
	{
		if (tiltCam < 35f)
		{
			tiltCam += value;
		}
		RotateCamera(0);
	}

	public void TiltDown(float value = 1f)
	{
		if (tiltCam > 25f)
		{
			tiltCam -= value;
		}
		RotateCamera(0);
	}

	public void RemoveAllMember()
	{
		Array.Clear(CinemachineTarget.m_Targets, 0, CinemachineTarget.m_Targets.Length);
		CinemachineTarget.m_Targets = new CinemachineTargetGroup.Target[0];
	}

	public void RemoveMember(Transform target)
	{
		int num = 0;
		while (num >= 0)
		{
			num = CinemachineTarget.FindMember(target);
			CinemachineTarget.RemoveMember(target);
		}
	}

	public void SetVignetteEffect(float intensity, Vector2 centerPosition, float smoothness, bool rounded)
	{
		Vignette vignetteEffect = VignetteEffect;
		if ((bool)vignetteEffect)
		{
			SetVignetteIntensityValue(intensity, withAnimation: true, vignetteEffect.intensity.value);
			vignetteEffect.intensity.value = intensity;
			vignetteEffect.center.value = centerPosition;
			vignetteEffect.smoothness.value = smoothness;
			vignetteEffect.rounded.value = rounded;
		}
	}

	public void SetVignetteIntensityValue(float value, bool withAnimation = false, float from = 0.45f, float duration = 0.6f)
	{
		Vignette vignette = VignetteEffect;
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

	public void ResetVignetteEffect()
	{
		SetVignetteEffect(_defaultVignetteIntensity, _defaultCenterValue, _defaultSmoothnessValue, _defaultRoundedValue);
	}

	public void SetColorAdjustmentEffect(Color newColor, float duration, bool isUtility = false)
	{
		if (isUtility)
		{
			_isUtilityUsed = true;
		}
		else if (_isUtilityUsed)
		{
			return;
		}
		ColorAdjustments colorAdjustmentEffect = ColorAdjustmentEffect;
		if ((bool)colorAdjustmentEffect)
		{
			StartCoroutine(TransitionColor(colorAdjustmentEffect.colorFilter.value, newColor, duration));
		}
	}

	private IEnumerator TransitionColor(Color from, Color to, float duration)
	{
		ColorAdjustments colorAdj = ColorAdjustmentEffect;
		float t = 0f;
		if ((bool)colorAdj)
		{
			while (t < duration)
			{
				t += Time.deltaTime;
				colorAdj.colorFilter.value = Color.Lerp(from, to, t / duration);
				yield return null;
			}
			colorAdj.colorFilter.value = to;
		}
	}

	public void ResetColorAdjustmentEffect(bool isUtility = false)
	{
		GameManager instance = GameManager.Instance;
		if ((object)instance != null && instance.isInfiniteHordeMode)
		{
			SetColorAdjustmentEffect(new Color(1f, 0.45f, 0.45f), 0f);
		}
		else
		{
			SetColorAdjustmentEffect(Color.white, 0f, isUtility);
		}
		if (isUtility)
		{
			_isUtilityUsed = false;
		}
	}

	public void SetActiveChromaticAberrationEffect(bool active)
	{
		if ((bool)ChromaticAberration)
		{
			_chromaticAberrationTweener?.Kill(complete: true);
			if (active)
			{
				ChromaticAberration.intensity.value = 0f;
				ChromaticAberration.active = true;
				SetChromaticAberrationValue(_chromaticAberrationValue, 0f, 0.6f, OnCompleteAction);
			}
			else
			{
				ChromaticAberration.intensity.value = _chromaticAberrationValue;
				ChromaticAberration.active = true;
				SetChromaticAberrationValue(0f, _chromaticAberrationValue, 0.6f, OnCompleteAction);
			}
		}
		void OnCompleteAction()
		{
			ChromaticAberration.active = active;
		}
	}

	public void SetChromaticAberrationValue(float value, float from, float duration, TweenCallback onComplete = null)
	{
		ChromaticAberration chromaticAberration = ChromaticAberration;
		_chromaticAberrationTweener = DOTween.To((float intensityValue) =>
		{
			chromaticAberration.intensity.value = intensityValue;
		}, from, value, duration).OnComplete(onComplete);
	}

	public void SetActiveLensDistortionEffect(bool active)
	{
		if ((bool)LensDistortion)
		{
			_lensDistortionTweener?.Kill(complete: true);
			if (active)
			{
				LensDistortion.intensity.value = 0f;
				LensDistortion.active = true;
				SetLensDistortionValue(_lensDistortionValue, 0f, 1f, OnCompleteAction);
			}
			else
			{
				LensDistortion.intensity.value = _lensDistortionValue;
				LensDistortion.active = true;
				SetLensDistortionValue(0f, _lensDistortionValue, 1f, OnCompleteAction);
			}
		}
		void OnCompleteAction()
		{
			LensDistortion.active = active;
		}
	}

	public void SetLensDistortionValue(float value, float from, float duration, TweenCallback onComplete = null)
	{
		LensDistortion lensDistortion = LensDistortion;
		_lensDistortionTweener = DOTween.To((float intensityValue) =>
		{
			lensDistortion.intensity.value = intensityValue;
		}, from, value, duration).SetDelay(0.3f).OnComplete(onComplete);
	}

	public void SetColorFilter(Color color, float duration, Action onComplete = null)
	{
		if ((bool)ColorAdjustmentEffect)
		{
			ColorAdjustmentEffect.colorFilter.overrideState = true;
			SetColorAdjustmentEffect(color, duration, isUtility: true);
			onComplete?.Invoke();
		}
	}

	public void ResetColorFilter(Action onComplete = null)
	{
		if ((bool)ColorAdjustmentEffect)
		{
			ResetColorAdjustmentEffect(isUtility: true);
			ColorAdjustmentEffect.colorFilter.overrideState = false;
			onComplete?.Invoke();
		}
	}

	public void SetPostExposure(float exposureValue, float duration, TweenCallback onCompleteAction = null)
	{
		if ((bool)ColorAdjustmentEffect)
		{
			_lensPostExposureTweener?.Kill(complete: true);
			ColorAdjustmentEffect.postExposure.value = 0f;
			ColorAdjustmentEffect.postExposure.overrideState = true;
			_lensPostExposureTweener = DOTween.To((float intensityValue) =>
			{
				ColorAdjustmentEffect.postExposure.value = intensityValue;
			}, 0f, exposureValue, duration).OnComplete(onCompleteAction);
		}
	}

	public void ResetPostExposure(Action onComplete = null)
	{
		if ((bool)ColorAdjustmentEffect)
		{
			ColorAdjustmentEffect.postExposure.value = 0f;
			ColorAdjustmentEffect.postExposure.overrideState = false;
			onComplete?.Invoke();
		}
	}

	public void SetNVGFilter(Color color, float postExposure, float duration, Action onComplete = null)
	{
		Shader.SetGlobalColor("_NVGTintColor", color);
		Shader.SetGlobalFloat("_NVGExposureBias", postExposure);
		nvgFeature.SetActive(active: true);
		nvgTransitionCoroutine = StartCoroutine(TransitionNVG(duration));
		onComplete?.Invoke();
	}

	private IEnumerator TransitionNVG(float duration)
	{
		float t = 0f;
		while (t < duration)
		{
			t += Time.deltaTime;
			Shader.SetGlobalFloat("_NVGBlendFactor", t / duration);
			yield return null;
		}
		Shader.SetGlobalFloat("_NVGBlendFactor", 1f);
		nvgTransitionCoroutine = null;
	}

	public void RemoveNVGFilter(Action onComplete = null)
	{
		if (nvgTransitionCoroutine != null)
		{
			StopCoroutine(nvgTransitionCoroutine);
		}
		Shader.SetGlobalFloat("_NVGBlendFactor", 0f);
		nvgFeature.SetActive(active: false);
		onComplete?.Invoke();
	}

	public void SetStonedFilter(float duration, Action onComplete = null)
	{
		if (stonedTransitionCoroutine != null)
		{
			StopCoroutine(stonedTransitionCoroutine);
		}
		stonedTransitionCoroutine = StartCoroutine(TransitionStoned(1f, duration));
		onComplete?.Invoke();
	}

	private IEnumerator TransitionStoned(float targetValue, float duration)
	{
		if (targetValue > 0f)
		{
			stonedFeature.SetActive(active: true);
		}
		float originalValue = _stonedValue;
		float currentTime = 0f;
		while (currentTime < duration)
		{
			currentTime += Time.deltaTime;
			_stonedValue = Mathf.Lerp(originalValue, targetValue, currentTime / duration);
			Shader.SetGlobalFloat("_StonedStrength", _stonedValue);
			yield return null;
		}
		Shader.SetGlobalFloat("_StonedStrength", targetValue);
		if (targetValue == 0f)
		{
			stonedFeature.SetActive(active: false);
		}
		nvgTransitionCoroutine = null;
	}

	public void RemoveStonedFilter(float duration, Action onComplete = null)
	{
		if (stonedTransitionCoroutine != null)
		{
			StopCoroutine(stonedTransitionCoroutine);
		}
		stonedTransitionCoroutine = StartCoroutine(TransitionStoned(0f, duration));
		onComplete?.Invoke();
	}
}
