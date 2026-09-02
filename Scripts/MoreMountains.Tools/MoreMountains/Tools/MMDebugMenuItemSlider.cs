using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Tools;

public class MMDebugMenuItemSlider : MonoBehaviour
{
	public enum Modes
	{
		Float = 0,
		Int = 1
	}

	[Header("Bindings")]
	public Modes Mode;

	public Slider TargetSlider;

	public Text SliderText;

	public Text SliderValueText;

	public Image SliderKnob;

	public Image SliderLine;

	public float RemapZero;

	public float RemapOne = 1f;

	public string SliderEventName = "Checkbox";

	[MMReadOnly]
	public float SliderValue;

	[MMReadOnly]
	public int SliderValueInt;

	protected bool _valueSetThisFrame;

	protected bool _listening;

	protected virtual void Awake()
	{
		TargetSlider.onValueChanged.AddListener(delegate
		{
			ValueChangeCheck();
		});
	}

	public void ValueChangeCheck()
	{
		if (_valueSetThisFrame)
		{
			_valueSetThisFrame = false;
			return;
		}
		bool flag = true;
		SliderValue = MMMaths.Remap(TargetSlider.value, 0f, 1f, RemapZero, RemapOne);
		if (Mode == Modes.Int)
		{
			SliderValue = Mathf.Round(SliderValue);
			if (SliderValue == (float)SliderValueInt)
			{
				flag = false;
			}
			SliderValueInt = (int)SliderValue;
		}
		if (flag)
		{
			UpdateValue(SliderValue);
		}
		TriggerSliderEvent(SliderValue);
	}

	protected virtual void UpdateValue(float newValue)
	{
		SliderValueText.text = ((Mode == Modes.Int) ? newValue.ToString() : newValue.ToString("F3"));
	}

	protected virtual void TriggerSliderEvent(float value)
	{
		MMDebugMenuSliderEvent.Trigger(SliderEventName, value);
	}

	protected virtual void OnMMDebugMenuSliderEvent(string sliderEventName, float value, MMDebugMenuSliderEvent.EventModes eventMode)
	{
		if (eventMode == MMDebugMenuSliderEvent.EventModes.SetSlider && sliderEventName == SliderEventName)
		{
			_valueSetThisFrame = true;
			TargetSlider.value = MMMaths.Remap(value, RemapZero, RemapOne, 0f, 1f);
			UpdateValue(value);
		}
	}

	public virtual void OnEnable()
	{
		if (!_listening)
		{
			MMDebugMenuSliderEvent.Register(OnMMDebugMenuSliderEvent);
			_listening = true;
		}
	}

	public virtual void OnDestroy()
	{
		_listening = false;
		MMDebugMenuSliderEvent.Unregister(OnMMDebugMenuSliderEvent);
	}
}
