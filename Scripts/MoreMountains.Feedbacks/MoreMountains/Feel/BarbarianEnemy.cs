using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.Feel;

public class BarbarianEnemy : MonoBehaviour
{
	public MMFeedbacks DamageFeedback;

	public float DamageCooldown = 1f;

	protected float _lastDamageTakenAt = -10f;

	public virtual void TakeDamage(int damage)
	{
		if (!(Time.time - _lastDamageTakenAt < DamageCooldown))
		{
			_lastDamageTakenAt = Time.time;
			DamageFeedback?.PlayFeedbacks(base.transform.position, damage);
		}
	}
}
