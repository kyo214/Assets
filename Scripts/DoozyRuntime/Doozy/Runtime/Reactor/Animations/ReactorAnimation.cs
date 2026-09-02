using System;
using Doozy.Runtime.Global;
using UnityEngine.Events;

namespace Doozy.Runtime.Reactor.Animations;

[Serializable]
public abstract class ReactorAnimation
{
	public UnityEvent OnPlayCallback;

	public UnityEvent OnStopCallback;

	public UnityEvent OnFinishCallback;

	private int m_InfiniteLoopCount;

	private const int MAX_INFINITE_LOOP_COUNT = 10;

	public abstract bool hasTarget { get; }

	public abstract bool isEnabled { get; }

	public abstract bool isIdle { get; }

	public abstract bool isActive { get; }

	public abstract bool isPaused { get; }

	public abstract bool isPlaying { get; }

	public abstract bool inStartDelay { get; }

	public abstract bool inLoopDelay { get; }

	protected int startedReactionsCount { get; set; }

	protected int stoppedReactionsCount { get; set; }

	protected int finishedReactionsCount { get; set; }

	protected bool onPlayInvoked { get; set; }

	protected void InvokeOnPlay()
	{
		if (startedReactionsCount > 0 && !onPlayInvoked)
		{
			OnPlayCallback?.Invoke();
			onPlayInvoked = true;
		}
	}

	protected void InvokeOnStop()
	{
		if (startedReactionsCount > 0)
		{
			stoppedReactionsCount++;
			if (stoppedReactionsCount >= startedReactionsCount)
			{
				OnStopCallback?.Invoke();
			}
		}
	}

	protected void InvokeOnFinish()
	{
		if (startedReactionsCount > 0)
		{
			finishedReactionsCount++;
			if (finishedReactionsCount >= startedReactionsCount)
			{
				OnFinishCallback?.Invoke();
			}
		}
	}

	public abstract void Recycle();

	public abstract void UpdateValues();

	public abstract void StopAllReactionsOnTarget();

	public void SetProgressAtOne()
	{
		SetProgressAt(1f);
	}

	public void SetProgressAtZero()
	{
		SetProgressAt(0f);
	}

	public virtual void SetProgressAt(float targetProgress)
	{
		if (!hasTarget)
		{
			if (m_InfiniteLoopCount <= 10)
			{
				m_InfiniteLoopCount++;
				Coroutiner.ExecuteAtEndOfFrame(() =>
				{
					SetProgressAt(targetProgress);
				});
			}
		}
		else
		{
			m_InfiniteLoopCount = 0;
			StopAllReactionsOnTarget();
			UpdateValues();
		}
	}

	public virtual void PlayToProgress(float toProgress)
	{
		if (!hasTarget)
		{
			if (m_InfiniteLoopCount <= 10)
			{
				m_InfiniteLoopCount++;
				Coroutiner.ExecuteAtEndOfFrame(() =>
				{
					PlayToProgress(toProgress);
				});
			}
		}
		else
		{
			m_InfiniteLoopCount = 0;
			StopAllReactionsOnTarget();
			UpdateValues();
			RegisterCallbacks();
		}
	}

	public virtual void PlayFromProgress(float fromProgress)
	{
		if (!hasTarget)
		{
			if (m_InfiniteLoopCount <= 10)
			{
				m_InfiniteLoopCount++;
				Coroutiner.ExecuteAtEndOfFrame(() =>
				{
					PlayFromProgress(fromProgress);
				});
			}
		}
		else
		{
			m_InfiniteLoopCount = 0;
			StopAllReactionsOnTarget();
			UpdateValues();
			RegisterCallbacks();
		}
	}

	public virtual void PlayFromToProgress(float fromProgress, float toProgress)
	{
		if (!hasTarget)
		{
			if (m_InfiniteLoopCount <= 10)
			{
				m_InfiniteLoopCount++;
				Coroutiner.ExecuteAtEndOfFrame(() =>
				{
					PlayFromToProgress(fromProgress, toProgress);
				});
			}
		}
		else
		{
			StopAllReactionsOnTarget();
			UpdateValues();
			RegisterCallbacks();
		}
	}

	public void Play(PlayDirection playDirection)
	{
		Play(playDirection == PlayDirection.Reverse);
	}

	public abstract void Play(bool inReverse = false);

	public abstract void ResetToStartValues(bool forced = false);

	public virtual void Stop()
	{
		UnregisterOnPlayCallbacks();
		UnregisterOnStopCallbacks();
	}

	public virtual void Finish()
	{
		UnregisterCallbacks();
	}

	public abstract void Reverse();

	public abstract void Rewind();

	public abstract void Pause();

	public abstract void Resume();

	protected virtual void RegisterCallbacks()
	{
		UnregisterCallbacks();
		onPlayInvoked = false;
		startedReactionsCount = 0;
		stoppedReactionsCount = 0;
		finishedReactionsCount = 0;
	}

	protected void UnregisterCallbacks()
	{
		UnregisterOnPlayCallbacks();
		UnregisterOnStopCallbacks();
		UnregisterOnFinishCallbacks();
	}

	protected abstract void UnregisterOnPlayCallbacks();

	protected abstract void UnregisterOnStopCallbacks();

	protected abstract void UnregisterOnFinishCallbacks();
}
