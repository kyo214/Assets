using Toked;
using UnityEngine;

public class ShellCasing : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem shellCasingSystem;

	public void EjectShell(Vector3 pos, Vector3 rot, RangeWeaponType typeAudio)
	{
		switch (typeAudio)
		{
		case RangeWeaponType.Pistol:
			AudioManager.PlaySFXTransform("pistol-bulletdrop", base.transform, isLocalPlayerTrigger: false);
			break;
		case RangeWeaponType.SMG:
			AudioManager.PlaySFXTransform("rifle-bulletdrop", base.transform, isLocalPlayerTrigger: false);
			break;
		case RangeWeaponType.Shotgun:
			AudioManager.PlaySFXTransform("shotgun-bulletdrop", base.transform, isLocalPlayerTrigger: false);
			break;
		}
		if (typeAudio != RangeWeaponType.Crossbow)
		{
			base.transform.position = pos;
			base.transform.localEulerAngles = rot;
			shellCasingSystem.Play();
		}
	}
}
