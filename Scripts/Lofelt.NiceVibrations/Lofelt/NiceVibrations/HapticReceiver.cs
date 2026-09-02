using System.ComponentModel;
using UnityEngine;

namespace Lofelt.NiceVibrations;

[AddComponentMenu("Nice Vibrations/Haptic Receiver")]
public class HapticReceiver : MonoBehaviour, ISerializationCallbackReceiver
{
	[SerializeField]
	[Range(0f, 5f)]
	private float _outputLevel = 1f;

	[SerializeField]
	private bool _hapticsEnabled = true;

	[DefaultValue(1f)]
	public float outputLevel
	{
		get
		{
			return HapticController.outputLevel;
		}
		set
		{
			HapticController.outputLevel = value;
		}
	}

	[DefaultValue(true)]
	public bool hapticsEnabled
	{
		get
		{
			return HapticController.hapticsEnabled;
		}
		set
		{
			HapticController.hapticsEnabled = value;
		}
	}

	public void OnBeforeSerialize()
	{
		_outputLevel = HapticController._outputLevel;
		_hapticsEnabled = HapticController._hapticsEnabled;
	}

	public void OnAfterDeserialize()
	{
		HapticController._outputLevel = _outputLevel;
		HapticController._hapticsEnabled = _hapticsEnabled;
	}

	private void Start()
	{
		HapticController.Init();
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		HapticController.ProcessApplicationFocus(hasFocus);
	}

	private void OnDestroy()
	{
		GamepadRumbler.Stop();
	}
}
