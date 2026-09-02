using System.Collections;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

public class DemoBall : MonoBehaviour
{
	public float LifeSpan = 2f;

	public MMFeedbacks DeathFeedback;

	protected virtual void Start()
	{
		StartCoroutine(ProgrammedDeath());
	}

	protected virtual IEnumerator ProgrammedDeath()
	{
		yield return MMCoroutine.WaitFor(LifeSpan);
		DeathFeedback?.PlayFeedbacks();
		base.gameObject.SetActive(value: false);
	}
}
