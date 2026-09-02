using Cysharp.Threading.Tasks;
using UnityEngine;

public class DeadExplosionHollowMother : MonoBehaviour
{
	[SerializeField]
	private EnemyController enemy;

	[SerializeField]
	private GameObject explosionParticle;

	[SerializeField]
	private string sfxName;

	[SerializeField]
	private string animationName;

	[SerializeField]
	private float delayFading = 2f;

	[SerializeField]
	private int spawnItemID;

	public void DeadSpecial()
	{
		UniTaskUtil.DelayedCall(this, 1f, () =>
		{
			enemy.attack.meleeCollider?.gameObject.SetActive(value: false);
			enemy.network.SetAnimation(animationName + enemy.movement.angleAnim);
		}).Forget();
		UniTaskUtil.DelayedCall(this, delayFading, () =>
		{
			enemy.Fading().Forget();
		}).Forget();
		UniTaskUtil.DelayedCall(this, 10f, () =>
		{
			explosionParticle.SetActive(value: false);
		}).Forget();
		if (spawnItemID > 0 && NetworkGameManager.Instance.isServer)
		{
			NetworkGameManager.Instance.ownPlayer.network.SetSpawnItem(spawnItemID, base.transform.position);
		}
	}
}
