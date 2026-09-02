using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace Dissonance.Demo;

public class SpeakerIndicator : MonoBehaviour
{
	private GameObject _indicator;

	private Light _light;

	private Transform _transform;

	private float _intensity;

	private IDissonancePlayer _player;

	private VoicePlayerState _state;

	private bool IsSpeaking
	{
		get
		{
			if (_player.Type == NetworkPlayerType.Remote && _state != null)
			{
				return _state.IsSpeaking;
			}
			return false;
		}
	}

	private void OnEnable()
	{
		_indicator = Object.Instantiate(Resources.Load<GameObject>("SpeechIndicator"));
		_indicator.transform.SetParent(base.transform);
		_indicator.transform.localPosition = new Vector3(0f, 3f, 0f);
		_light = _indicator.GetComponent<Light>();
		_transform = _indicator.GetComponent<Transform>();
		_player = GetComponent<IDissonancePlayer>();
		StartCoroutine(FindPlayerState());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator FindPlayerState()
	{
		while (!_player.IsTracking)
		{
			yield return null;
		}
		while (_state == null)
		{
			_state = Object.FindObjectOfType<DissonanceComms>().FindPlayer(_player.PlayerId);
			yield return null;
		}
	}

	private void Update()
	{
		if (IsSpeaking)
		{
			_intensity = Mathf.Max(Mathf.Clamp(Mathf.Pow(_state.Amplitude, 0.175f), 0.25f, 1f), _intensity - Time.unscaledDeltaTime);
			_indicator.SetActive(value: true);
		}
		else
		{
			_intensity -= Time.unscaledDeltaTime * 2f;
			if (_intensity <= 0f)
			{
				_indicator.SetActive(value: false);
			}
		}
		UpdateLight(_light, _intensity);
		UpdateChildTransform(_transform, _intensity);
	}

	private static void UpdateChildTransform([NotNull] Transform transform, float intensity)
	{
		transform.localScale = new Vector3(intensity, intensity, intensity);
	}

	private static void UpdateLight([NotNull] Light light, float intensity)
	{
		light.intensity = intensity;
	}
}
