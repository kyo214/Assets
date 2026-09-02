using UnityEngine;
using UnityEngine.VFX;

public class VfxCursedBlindness : MonoBehaviour, IVfxControl
{
	[SerializeField]
	private bool _autoPlay;

	private VisualEffect _effect;

	private void Start()
	{
		_effect = GetComponent<VisualEffect>();
		if (!_autoPlay)
		{
			_effect.Stop();
		}
	}

	public void Play()
	{
		_effect?.Play();
	}

	public void Stop()
	{
		_effect.Stop();
		base.gameObject.SetActive(value: false);
	}

	private void OnEnable()
	{
		Play();
	}

	private void OnDisable()
	{
		_effect?.Stop();
	}
}
