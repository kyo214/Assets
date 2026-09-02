using System.Collections;
using UnityEngine;

namespace Lofelt.NiceVibrations;

public class MMUIShaker : MonoBehaviour
{
	public float Amplitude;

	public float Frequency;

	public bool Shaking;

	protected Vector3 _initialPosition;

	protected Vector3 _shakePosition;

	protected RectTransform _rectTransform;

	protected virtual void Start()
	{
		_rectTransform = base.gameObject.GetComponent<RectTransform>();
		_initialPosition = _rectTransform.localPosition;
	}

	public virtual IEnumerator Shake(float duration)
	{
		Shaking = true;
		yield return new WaitForSeconds(duration);
		Shaking = false;
	}

	protected virtual void Update()
	{
		if (!Shaking)
		{
			_rectTransform.localPosition = _initialPosition;
			return;
		}
		_shakePosition.x = Mathf.PerlinNoise((0f - Time.time) * Frequency, Time.time * Frequency) * Amplitude - Amplitude / 2f;
		_shakePosition.y = Mathf.PerlinNoise((0f - (Time.time + 0.25f)) * Frequency, Time.time * Frequency) * Amplitude - Amplitude / 2f;
		_shakePosition.z = Mathf.PerlinNoise((0f - (Time.time + 0.5f)) * Frequency, Time.time * Frequency) * Amplitude - Amplitude / 2f;
		_rectTransform.localPosition = _initialPosition + _shakePosition;
	}
}
