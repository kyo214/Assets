using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.Feel;

public class FeelCardsUIStackButton : MonoBehaviour
{
	public MMFeedbacks StackFeedback;

	public List<MMFeedbacks> BlockerFeedbacks;

	public virtual void Stack()
	{
		bool flag = false;
		foreach (MMFeedbacks blockerFeedback in BlockerFeedbacks)
		{
			if (blockerFeedback.IsPlaying)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			StackFeedback?.PlayFeedbacks();
			base.gameObject.SetActive(value: false);
		}
	}
}
