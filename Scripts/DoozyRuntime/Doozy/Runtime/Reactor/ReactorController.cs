using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Animators.Internal;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Reactor;

[AddComponentMenu("Reactor/Reactor Controller")]
public class ReactorController : MonoBehaviour
{
	public enum Mode
	{
		Manual = 0,
		Automatic = 1
	}

	public string ControllerName;

	public AnimatorBehaviour OnStartBehaviour;

	public AnimatorBehaviour OnEnableBehaviour;

	public bool OverrideAnimatorsBehaviors;

	public Mode ControllerMode;

	[SerializeField]
	private List<ReactorAnimator> Animators;

	public List<ReactorAnimator> animators
	{
		get
		{
			return Animators ?? (Animators = new List<ReactorAnimator>());
		}
		private set
		{
			Animators = value;
		}
	}

	public bool hasAnimators => animators.Count > 0;

	public bool initialized { get; private set; }

	protected virtual void Awake()
	{
		if (Application.isPlaying)
		{
			initialized = false;
			Initialize();
		}
	}

	protected virtual void OnEnable()
	{
		if (Application.isPlaying)
		{
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

	protected virtual void Initialize()
	{
		if (initialized)
		{
			return;
		}
		switch (ControllerMode)
		{
		case Mode.Automatic:
			animators = (from c in GetComponentsInChildren<ReactorAnimator>()
				where c.isActiveAndEnabled
				select c).ToList();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case Mode.Manual:
			break;
		}
		if (!hasAnimators)
		{
			initialized = true;
			return;
		}
		if (OverrideAnimatorsBehaviors)
		{
			foreach (ReactorAnimator item in animators.RemoveNulls())
			{
				item.OnStartBehaviour = AnimatorBehaviour.Disabled;
				item.OnEnableBehaviour = AnimatorBehaviour.Disabled;
			}
		}
		initialized = true;
	}

	protected virtual void RunBehaviour(AnimatorBehaviour behaviour)
	{
		if (behaviour == AnimatorBehaviour.Disabled)
		{
			return;
		}
		bool flag = true;
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			if (!item.animatorInitialized)
			{
				flag = false;
			}
		}
		if (!initialized || !flag)
		{
			DelayExecution(() =>
			{
				RunBehaviour(behaviour);
			});
			return;
		}
		InitializeAnimators();
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
			throw new ArgumentOutOfRangeException("behaviour", behaviour, null);
		}
	}

	protected void DelayExecution(UnityAction callback)
	{
		StartCoroutine(ExecuteAfterControllerInitialized(callback));
	}

	protected IEnumerator ExecuteAfterControllerInitialized(UnityAction callback)
	{
		yield return new WaitUntil(() => initialized);
		callback?.Invoke();
	}

	public void InitializeAnimators()
	{
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			item.Initialize();
		}
	}

	public void UpdateValues()
	{
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			item.UpdateValues();
		}
	}

	public void SetProgressAtOne()
	{
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			item.SetProgressAtOne();
		}
	}

	public void SetProgressAtZero()
	{
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			item.SetProgressAtZero();
		}
	}

	public void SetProgressAt(float targetProgress)
	{
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			item.SetProgressAt(targetProgress);
		}
	}

	public void PlayToProgress(float toProgress)
	{
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			item.PlayToProgress(toProgress);
		}
	}

	public void PlayFromProgress(float fromProgress)
	{
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			item.PlayFromProgress(fromProgress);
		}
	}

	public void PlayFromToProgress(float fromProgress, float toProgress)
	{
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			item.PlayFromToProgress(fromProgress, toProgress);
		}
	}

	public void Play(PlayDirection playDirection)
	{
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			item.Play(playDirection);
		}
	}

	public void Play(bool inReverse = false)
	{
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			item.Play(inReverse);
		}
	}

	public void ResetToStartValues(bool forced = false)
	{
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			item.ResetToStartValues(forced);
		}
	}

	public void Stop()
	{
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			item.Stop();
		}
	}

	public void Finish()
	{
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			item.Finish();
		}
	}

	public void Reverse()
	{
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			item.Reverse();
		}
	}

	public void Rewind()
	{
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			item.Rewind();
		}
	}

	public void Pause()
	{
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			item.Pause();
		}
	}

	public void Resume()
	{
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			item.Resume();
		}
	}

	public void UpdateSettings()
	{
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			item.UpdateSettings();
		}
	}

	public float GetStartDelay()
	{
		float num = 0f;
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			num = Mathf.Max(num, item.GetStartDelay());
		}
		return num;
	}

	public float GetDuration()
	{
		float num = 0f;
		foreach (ReactorAnimator item in animators.RemoveNulls())
		{
			num = Mathf.Max(num, item.GetDuration());
		}
		return num;
	}

	public float GetTotalDuration()
	{
		return GetStartDelay() + GetDuration();
	}
}
