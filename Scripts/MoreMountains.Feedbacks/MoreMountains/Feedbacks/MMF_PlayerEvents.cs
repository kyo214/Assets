using System;
using UnityEngine;
using UnityEngine.Events;

namespace MoreMountains.Feedbacks;

[Serializable]
public class MMF_PlayerEvents
{
	[Tooltip("whether or not this MMFeedbacks should fire MMFeedbacksEvents")]
	public bool TriggerMMFeedbacksEvents;

	[Tooltip("whether or not this MMFeedbacks should fire Unity Events")]
	public bool TriggerUnityEvents = true;

	[Tooltip("This event will fire every time this MMFeedbacks gets played")]
	public UnityEvent OnPlay;

	[Tooltip("This event will fire every time this MMFeedbacks starts a holding pause")]
	public UnityEvent OnPause;

	[Tooltip("This event will fire every time this MMFeedbacks resumes after a holding pause")]
	public UnityEvent OnResume;

	[Tooltip("This event will fire every time this MMFeedbacks reverts its play direction")]
	public UnityEvent OnRevert;

	[Tooltip("This event will fire every time this MMFeedbacks plays its last MMFeedback")]
	public UnityEvent OnComplete;

	public bool OnPlayIsNull { get; protected set; }

	public bool OnPauseIsNull { get; protected set; }

	public bool OnResumeIsNull { get; protected set; }

	public bool OnRevertIsNull { get; protected set; }

	public bool OnCompleteIsNull { get; protected set; }

	public virtual void Initialization()
	{
		OnPlayIsNull = OnPlay == null;
		OnPauseIsNull = OnPause == null;
		OnResumeIsNull = OnResume == null;
		OnRevertIsNull = OnRevert == null;
		OnCompleteIsNull = OnComplete == null;
	}

	public virtual void TriggerOnPlay(MMF_Player source)
	{
		if (!OnPlayIsNull && TriggerUnityEvents)
		{
			OnPlay.Invoke();
		}
		if (TriggerMMFeedbacksEvents)
		{
			MMF_PlayerEvent.Trigger(source, MMF_PlayerEvent.EventTypes.Play);
		}
	}

	public virtual void TriggerOnPause(MMF_Player source)
	{
		if (!OnPauseIsNull && TriggerUnityEvents)
		{
			OnPause.Invoke();
		}
		if (TriggerMMFeedbacksEvents)
		{
			MMF_PlayerEvent.Trigger(source, MMF_PlayerEvent.EventTypes.Pause);
		}
	}

	public virtual void TriggerOnResume(MMF_Player source)
	{
		if (!OnResumeIsNull && TriggerUnityEvents)
		{
			OnResume.Invoke();
		}
		if (TriggerMMFeedbacksEvents)
		{
			MMF_PlayerEvent.Trigger(source, MMF_PlayerEvent.EventTypes.Resume);
		}
	}

	public virtual void TriggerOnRevert(MMF_Player source)
	{
		if (!OnRevertIsNull && TriggerUnityEvents)
		{
			OnRevert.Invoke();
		}
		if (TriggerMMFeedbacksEvents)
		{
			MMF_PlayerEvent.Trigger(source, MMF_PlayerEvent.EventTypes.Revert);
		}
	}

	public virtual void TriggerOnComplete(MMF_Player source)
	{
		if (!OnCompleteIsNull && TriggerUnityEvents)
		{
			OnComplete.Invoke();
		}
		if (TriggerMMFeedbacksEvents)
		{
			MMF_PlayerEvent.Trigger(source, MMF_PlayerEvent.EventTypes.Complete);
		}
	}
}
