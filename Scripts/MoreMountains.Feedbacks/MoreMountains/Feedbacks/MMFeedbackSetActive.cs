using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback allows you to change the state of the target gameobject from active to inactive (or the opposite), on init, play, stop or reset. For each of these you can specify if you want to force a state (active or inactive), or toggle it (active becomes inactive, inactive becomes active).")]
[FeedbackPath("GameObject/Set Active")]
public class MMFeedbackSetActive : MMFeedback
{
	public enum PossibleStates
	{
		Active = 0,
		Inactive = 1,
		Toggle = 2
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Set Active")]
	[Tooltip("the gameobject we want to change the active state of")]
	public GameObject TargetGameObject;

	[Header("States")]
	[Tooltip("whether or not we should alter the state of the target object on init")]
	public bool SetStateOnInit;

	[MMFCondition("SetStateOnInit", true)]
	[Tooltip("how to change the state on init")]
	public PossibleStates StateOnInit = PossibleStates.Inactive;

	[Tooltip("whether or not we should alter the state of the target object on play")]
	public bool SetStateOnPlay;

	[Tooltip("how to change the state on play")]
	[MMFCondition("SetStateOnPlay", true)]
	public PossibleStates StateOnPlay = PossibleStates.Inactive;

	[Tooltip("whether or not we should alter the state of the target object on stop")]
	public bool SetStateOnStop;

	[Tooltip("how to change the state on stop")]
	[MMFCondition("SetStateOnStop", true)]
	public PossibleStates StateOnStop = PossibleStates.Inactive;

	[Tooltip("whether or not we should alter the state of the target object on reset")]
	public bool SetStateOnReset;

	[Tooltip("how to change the state on reset")]
	[MMFCondition("SetStateOnReset", true)]
	public PossibleStates StateOnReset = PossibleStates.Inactive;

	protected override void CustomInitialization(GameObject owner)
	{
		base.CustomInitialization(owner);
		if (Active && TargetGameObject != null && SetStateOnInit)
		{
			SetStatus(StateOnInit);
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && !(TargetGameObject == null) && SetStateOnPlay)
		{
			SetStatus(StateOnPlay);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		base.CustomStopFeedback(position, feedbacksIntensity);
		if (Active && FeedbackTypeAuthorized && TargetGameObject != null && SetStateOnStop)
		{
			SetStatus(StateOnStop);
		}
	}

	protected override void CustomReset()
	{
		base.CustomReset();
		if (!InCooldown && Active && FeedbackTypeAuthorized && TargetGameObject != null && SetStateOnReset)
		{
			SetStatus(StateOnReset);
		}
	}

	protected virtual void SetStatus(PossibleStates state)
	{
		bool active = false;
		switch (state)
		{
		case PossibleStates.Active:
			active = (NormalPlayDirection ? true : false);
			break;
		case PossibleStates.Inactive:
			active = !NormalPlayDirection;
			break;
		case PossibleStates.Toggle:
			active = !TargetGameObject.activeInHierarchy;
			break;
		}
		TargetGameObject.SetActive(active);
	}
}
