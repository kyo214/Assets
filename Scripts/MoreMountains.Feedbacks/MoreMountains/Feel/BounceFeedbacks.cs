using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.Feel;

public class BounceFeedbacks : MonoBehaviour
{
	public MMFeedbacks ChargeFeedbacks;

	public MMFeedbacks JumpFeedbacks;

	public MMFeedbacks LandingFeedbacks;

	public virtual void PlayCharge()
	{
		ChargeFeedbacks?.PlayFeedbacks();
	}

	public virtual void PlayJump()
	{
		JumpFeedbacks?.PlayFeedbacks();
	}

	public virtual void PlayLanding()
	{
		LandingFeedbacks?.PlayFeedbacks();
	}
}
