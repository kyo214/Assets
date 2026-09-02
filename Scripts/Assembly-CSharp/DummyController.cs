using DG.Tweening;
using Toked;
using UnityEngine;

public class DummyController : MonoBehaviour
{
	public Collider MyCollider;

	public Animator HitEffect;

	public string SFXMeleeHit;

	public void GetHit(Vector3 direction, bool isCharging, int idWeaponMelee)
	{
		base.transform.localRotation = Quaternion.LookRotation(direction);
		base.transform.DOKill(complete: true);
		if (BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).OneHitKnockback | isCharging)
		{
			base.transform.DOLocalRotate(new Vector3(0f, base.transform.localEulerAngles.y, base.transform.localEulerAngles.z), 0f).SetEase(Ease.Linear);
			base.transform.DOLocalRotate(new Vector3(base.transform.localEulerAngles.x + 80f, base.transform.localEulerAngles.y, base.transform.localEulerAngles.z), 0.4f).SetEase(Ease.OutBounce);
			base.transform.DOLocalRotate(new Vector3(0f, base.transform.localEulerAngles.y, base.transform.localEulerAngles.z), 0.5f).SetEase(Ease.OutBounce).SetDelay(2f);
		}
		else
		{
			base.transform.DOLocalRotate(new Vector3(0f, base.transform.localEulerAngles.y, base.transform.localEulerAngles.z), 0f).SetEase(Ease.Linear);
			base.transform.DOLocalRotate(new Vector3(base.transform.localEulerAngles.z + 30f, base.transform.localEulerAngles.y, base.transform.localEulerAngles.z), 0.05f).SetEase(Ease.OutBounce);
			base.transform.DOLocalRotate(new Vector3(0f, base.transform.localEulerAngles.y, base.transform.localEulerAngles.z), 0.5f).SetEase(Ease.OutBounce).SetDelay(0.05f);
		}
		HitEffect.gameObject.SetActive(value: true);
		HitEffect.transform.localEulerAngles = new Vector3(0f, CameraGame.Instance.camRotate, 0f);
		if (isCharging)
		{
			HitEffect.Play("ChargeMeleeHit");
		}
		else
		{
			HitEffect.Play("MeleeHit" + Random.Range(1, 3));
		}
		AudioManager.PlaySFXTransform(SFXMeleeHit, base.transform, isLocalPlayerTrigger: false);
	}
}
