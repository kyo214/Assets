using UnityEngine;

namespace DestroyIt;

public class PoweredTag : MonoBehaviour
{
	public PowerSource powerSource;

	private void Update()
	{
		if (powerSource == null || !powerSource.hasPower)
		{
			base.gameObject.RemoveTag(Tag.Powered);
		}
		else
		{
			base.gameObject.AddTag(Tag.Powered);
		}
	}
}
