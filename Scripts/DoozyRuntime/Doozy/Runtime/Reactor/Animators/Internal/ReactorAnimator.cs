using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Reactor.Animators.Internal;

[Serializable]
public abstract class ReactorAnimator : MonoBehaviour
{
	public string AnimatorName;

	public AnimatorBehaviour OnStartBehaviour;

	public AnimatorBehaviour OnEnableBehaviour;

	protected Coroutine initializeLater { get; set; }

	public bool animatorInitialized { get; set; }

	protected virtual void Awake()
	{
		if (Application.isPlaying)
		{
			animatorInitialized = false;
		}
	}

	protected virtual void OnEnable()
	{
		if (Application.isPlaying)
		{
			Initialize();
			RunBehaviour(OnEnableBehaviour);
		}
	}

	protected virtual void Start()
	{
		if (Application.isPlaying)
		{
			RunBehaviour(OnStartBehaviour);
		}
	}

	protected virtual void OnDestroy()
	{
		if (Application.isPlaying)
		{
			Recycle();
		}
	}

	public virtual void Initialize()
	{
		if (!animatorInitialized)
		{
			if (initializeLater != null)
			{
				StopCoroutine(initializeLater);
				initializeLater = null;
			}
			initializeLater = StartCoroutine(InitializeLater());
		}
	}

	protected IEnumerator InitializeLater()
	{
		yield return new WaitForEndOfFrame();
		InitializeAnimator();
	}

	public virtual void InitializeAnimator()
	{
		UpdateSettings();
		animatorInitialized = true;
	}

	protected void RunBehaviour(AnimatorBehaviour behaviour)
	{
		if (behaviour == AnimatorBehaviour.Disabled)
		{
			return;
		}
		if (!animatorInitialized)
		{
			DelayExecution(() =>
			{
				RunBehaviour(behaviour);
			});
			return;
		}
		switch (behaviour)
		{
		case AnimatorBehaviour.PlayForward:
			Play(PlayDirection.Forward);
			break;
		case AnimatorBehaviour.PlayReverse:
			Play(PlayDirection.Reverse);
			break;
		case AnimatorBehaviour.SetFromValue:
			SetProgressAtZero();
			break;
		case AnimatorBehaviour.SetToValue:
			SetProgressAtOne();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	protected void DelayExecution(UnityAction callback)
	{
		StartCoroutine(ExecuteAfterAnimatorInitialized(callback));
	}

	protected IEnumerator ExecuteAfterAnimatorInitialized(UnityAction callback)
	{
		yield return new WaitUntil(() => animatorInitialized);
		callback?.Invoke();
	}

	protected abstract void Recycle();

	public abstract void UpdateValues();

	public abstract void SetProgressAtOne();

	public abstract void SetProgressAtZero();

	public abstract void SetProgressAt(float targetProgress);

	public abstract void PlayToProgress(float toProgress);

	public abstract void PlayFromProgress(float fromProgress);

	public abstract void PlayFromToProgress(float fromProgress, float toProgress);

	public abstract void Play(PlayDirection playDirection);

	public abstract void Play(bool inReverse = false);

	public abstract void ResetToStartValues(bool forced = false);

	public abstract void Stop();

	public abstract void Finish();

	public abstract void Reverse();

	public abstract void Rewind();

	public abstract void Pause();

	public abstract void Resume();

	public abstract void SetTarget(object target);

	public abstract void UpdateSettings();

	public abstract float GetStartDelay();

	public abstract float GetDuration();

	public abstract float GetTotalDuration();

	public abstract List<Heartbeat> SetHeartbeat<T>() where T : Heartbeat, new();
}
