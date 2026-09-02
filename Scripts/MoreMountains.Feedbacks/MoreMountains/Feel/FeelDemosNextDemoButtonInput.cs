using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.Feel;

public class FeelDemosNextDemoButtonInput : MonoBehaviour
{
	public MMFeedbacks OnInputFeedback;

	protected virtual void Update()
	{
		if (FeelDemosInputHelper.CheckEnterPressedThisFrame())
		{
			OnInputFeedback?.PlayFeedbacks();
		}
	}
}
