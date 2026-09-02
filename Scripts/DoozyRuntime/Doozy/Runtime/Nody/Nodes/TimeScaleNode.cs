using System;
using Doozy.Runtime.Nody.Nodes.Internal;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using UnityEngine;

namespace Doozy.Runtime.Nody.Nodes;

[Serializable]
[NodyMenuPath("Time", "TimeScale")]
public sealed class TimeScaleNode : SimpleNode
{
	public float TargetValue;

	public bool AnimateValue;

	public float AnimationDuration;

	public Ease AnimationEase;

	public bool WaitForAnimationToFinish;

	public static string timescaleAnimationId => "TimeScaleNode TimeScale Animation";

	public FloatReaction timeScaleReaction { get; private set; }

	public TimeScaleNode()
	{
		TargetValue = 1f;
		AnimateValue = false;
		AnimationDuration = 1f;
		AnimationEase = Ease.Linear;
		WaitForAnimationToFinish = false;
		AddInputPort().SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
		AddOutputPort().SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
	}

	public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
	{
		base.OnEnter(previousNode, previousPort);
		StartTimer();
	}

	private void StartTimer()
	{
		if (Math.Abs(Time.timeScale - TargetValue) < 0.01f)
		{
			Time.timeScale = TargetValue;
			GoToNextNode(base.firstOutputPort);
			return;
		}
		timeScaleReaction?.Recycle();
		timeScaleReaction = Reaction.Get<FloatReaction>().SetStringId(timescaleAnimationId).SetSetter((float value) =>
		{
			Time.timeScale = value;
		})
			.SetGetter(() => Time.timeScale);
		timeScaleReaction.SetEase(AnimationEase);
		if (AnimateValue && AnimationDuration > 0f)
		{
			timeScaleReaction.settings.duration = AnimationDuration;
			timeScaleReaction.SetFrom(Time.timeScale);
			timeScaleReaction.SetTo(TargetValue);
			timeScaleReaction.ClearOnFinishCallback();
			if (WaitForAnimationToFinish)
			{
				timeScaleReaction.AddOnFinishCallback(() =>
				{
					StopTimer();
					GoToNextNode(base.firstOutputPort);
					timeScaleReaction?.Recycle();
				});
				timeScaleReaction.Play();
			}
			else
			{
				timeScaleReaction.AddOnFinishCallback(StopTimer);
				timeScaleReaction.Play();
				GoToNextNode(base.firstOutputPort);
				timeScaleReaction?.Recycle();
			}
		}
		else
		{
			Time.timeScale = TargetValue;
			GoToNextNode(base.firstOutputPort);
		}
	}

	private void StopTimer()
	{
		if (timeScaleReaction != null)
		{
			timeScaleReaction.Finish();
			timeScaleReaction.ClearOnFinishCallback();
			timeScaleReaction.Recycle();
			timeScaleReaction = null;
		}
	}
}
