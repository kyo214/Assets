using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.Feel;

public class MMSequencerDemoManager : MonoBehaviour
{
	[Header("Sequence")]
	public MMFeedbacksSequencer TargetSequencer;

	protected virtual void Update()
	{
		HandleInput();
	}

	protected virtual void HandleInput()
	{
		if (FeelDemosInputHelper.CheckMainActionInputPressedThisFrame())
		{
			Toggle();
		}
	}

	protected virtual void Toggle()
	{
		if (TargetSequencer.Playing)
		{
			TargetSequencer.StopSequence();
		}
		else
		{
			TargetSequencer.PlaySequence();
		}
	}
}
