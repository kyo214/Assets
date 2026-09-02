using UnityEngine;
using UnityEngine.VFX;

public class ObjectDestroyControl : MonoBehaviour
{
	[SerializeField]
	private VisualEffect[] _effects;

	private void Start()
	{
	}

	public void PlayExplode()
	{
		for (int i = 0; i < _effects.Length; i++)
		{
			_effects[i].Play();
		}
	}
}
