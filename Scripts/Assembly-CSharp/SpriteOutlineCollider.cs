using UnityEngine;

public class SpriteOutlineCollider : MonoBehaviour
{
	public SpriteRenderer outlineRenderer;

	public void SetOutline(bool value)
	{
		outlineRenderer.enabled = value;
	}
}
