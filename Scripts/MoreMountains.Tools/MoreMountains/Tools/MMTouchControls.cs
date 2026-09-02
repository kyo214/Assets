using UnityEngine;

namespace MoreMountains.Tools;

[RequireComponent(typeof(CanvasGroup))]
[AddComponentMenu("More Mountains/Tools/Controls/MMTouchControls")]
public class MMTouchControls : MonoBehaviour
{
	public enum InputForcedMode
	{
		None = 0,
		Mobile = 1,
		Desktop = 2
	}

	[MMInformation("If you check Auto Mobile Detection, the engine will automatically switch to mobile controls when your build target is Android or iOS. You can also force mobile or desktop (keyboard, gamepad) controls using the dropdown below.\nNote that if you don't need mobile controls and/or GUI this component can also work on its own, just put it on an empty GameObject instead.", MMInformationAttribute.InformationType.Info, false)]
	public bool AutoMobileDetection = true;

	public InputForcedMode ForcedMode;

	protected CanvasGroup _canvasGroup;

	protected float _initialMobileControlsAlpha;

	public bool IsMobile { get; protected set; }

	protected virtual void Start()
	{
		_canvasGroup = GetComponent<CanvasGroup>();
		_initialMobileControlsAlpha = _canvasGroup.alpha;
		SetMobileControlsActive(state: false);
		IsMobile = false;
		_ = AutoMobileDetection;
		if (ForcedMode == InputForcedMode.Mobile)
		{
			SetMobileControlsActive(state: true);
			IsMobile = true;
		}
		if (ForcedMode == InputForcedMode.Desktop)
		{
			SetMobileControlsActive(state: false);
			IsMobile = false;
		}
	}

	public virtual void SetMobileControlsActive(bool state)
	{
		if (_canvasGroup != null)
		{
			_canvasGroup.gameObject.SetActive(state);
			if (state)
			{
				_canvasGroup.alpha = _initialMobileControlsAlpha;
			}
			else
			{
				_canvasGroup.alpha = 0f;
			}
		}
	}
}
