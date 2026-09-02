using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.Feel;

public class LettersDemoManager : MonoBehaviour
{
	[Header("Feedbacks")]
	public MMFeedbacks FeedbackF;

	public MMFeedbacks FeedbackE1;

	public MMFeedbacks FeedbackE2;

	public MMFeedbacks FeedbackL;

	protected Vector3 _mousePosition;

	protected virtual void Update()
	{
		HandleInput();
	}

	protected virtual void HandleInput()
	{
		if (FeelDemosInputHelper.CheckAlphaInputPressedThisFrame(1))
		{
			PlayF();
		}
		if (FeelDemosInputHelper.CheckAlphaInputPressedThisFrame(2))
		{
			PlayE1();
		}
		if (FeelDemosInputHelper.CheckAlphaInputPressedThisFrame(3))
		{
			PlayE2();
		}
		if (FeelDemosInputHelper.CheckAlphaInputPressedThisFrame(4))
		{
			PlayL();
		}
		if (FeelDemosInputHelper.CheckMouseDown() && Physics.Raycast(Camera.main.ScreenPointToRay(FeelDemosInputHelper.MousePosition()), out var hitInfo, 100f))
		{
			switch (hitInfo.transform.name)
			{
			case "ColliderF":
				PlayF();
				break;
			case "ColliderE1":
				PlayE1();
				break;
			case "ColliderE2":
				PlayE2();
				break;
			case "ColliderL":
				PlayL();
				break;
			}
		}
	}

	protected virtual void PlayF()
	{
		FeedbackF?.PlayFeedbacks();
	}

	protected virtual void PlayE1()
	{
		FeedbackE1?.PlayFeedbacks();
	}

	protected virtual void PlayE2()
	{
		FeedbackE2?.PlayFeedbacks();
	}

	protected virtual void PlayL()
	{
		FeedbackL?.PlayFeedbacks();
	}
}
