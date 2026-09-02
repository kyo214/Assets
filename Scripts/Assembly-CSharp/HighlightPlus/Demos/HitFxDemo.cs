using UnityEngine;

namespace HighlightPlus.Demos;

public class HitFxDemo : MonoBehaviour
{
	public AudioClip hitSound;

	private void Update()
	{
		if (InputProxy.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(InputProxy.mousePosition), out var hitInfo))
		{
			hitInfo.collider.TryGetComponent<HighlightEffect>(out var component);
			if (!(component == null))
			{
				AudioSource.PlayClipAtPoint(hitSound, hitInfo.point);
				component.HitFX(hitInfo.point);
			}
		}
	}
}
