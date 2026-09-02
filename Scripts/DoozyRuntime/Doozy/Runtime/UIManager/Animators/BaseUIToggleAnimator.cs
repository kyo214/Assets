using System;
using System.Collections;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Animators;

public abstract class BaseUIToggleAnimator : BaseTargetComponentAnimator<UIToggle>
{
	private Coroutine initializeCoroutine { get; set; }

	protected abstract bool onAnimationIsActive { get; }

	protected abstract bool offAnimationIsActive { get; }

	protected abstract UnityAction playOnAnimation { get; }

	protected abstract UnityAction playOffAnimation { get; }

	protected abstract UnityAction reverseOnAnimation { get; }

	protected abstract UnityAction reverseOffAnimation { get; }

	protected abstract UnityAction instantPlayOnAnimation { get; }

	protected abstract UnityAction instantPlayOffAnimation { get; }

	protected abstract UnityAction stopOnAnimation { get; }

	protected abstract UnityAction stopOffAnimation { get; }

	protected abstract UnityAction addResetToOnStateCallback { get; }

	protected abstract UnityAction removeResetToOnStateCallback { get; }

	protected abstract UnityAction addResetToOffStateCallback { get; }

	protected abstract UnityAction removeResetToOffStateCallback { get; }

	protected override void ConnectToController()
	{
		if (!(base.controller == null))
		{
			UIToggle uIToggle = base.controller;
			uIToggle.onToggleValueChangedCallback = (UnityAction<ToggleValueChangedEvent>)Delegate.Remove(uIToggle.onToggleValueChangedCallback, new UnityAction<ToggleValueChangedEvent>(OnValueChanged));
			UIToggle uIToggle2 = base.controller;
			uIToggle2.onToggleValueChangedCallback = (UnityAction<ToggleValueChangedEvent>)Delegate.Combine(uIToggle2.onToggleValueChangedCallback, new UnityAction<ToggleValueChangedEvent>(OnValueChanged));
			OnValueChanged(new ToggleValueChangedEvent(base.controller.isOn, base.controller.isOn, animateChange: false));
		}
	}

	protected override void DisconnectFromController()
	{
		if (!(base.controller == null))
		{
			UIToggle uIToggle = base.controller;
			uIToggle.onToggleValueChangedCallback = (UnityAction<ToggleValueChangedEvent>)Delegate.Remove(uIToggle.onToggleValueChangedCallback, new UnityAction<ToggleValueChangedEvent>(OnValueChanged));
		}
	}

	protected virtual void OnValueChanged(ToggleValueChangedEvent evt)
	{
		if (base.controller == null)
		{
			return;
		}
		if (initializeCoroutine != null)
		{
			StopCoroutine(initializeCoroutine);
			initializeCoroutine = null;
		}
		if (!base.animatorInitialized)
		{
			initializeCoroutine = StartCoroutine(InitializeAfterAnimatorInitialized());
		}
		else if ((evt.newValue == evt.previousValue) & !base.controller.inToggleGroup)
		{
			if (evt.newValue)
			{
				InstantPlayOnAnimation();
			}
			else
			{
				InstantPlayOffAnimation();
			}
		}
		else if (evt.newValue)
		{
			if (offAnimationIsActive)
			{
				ReverseOffAnimation();
			}
			else if (evt.animateChange)
			{
				PlayOnAnimation();
			}
			else
			{
				InstantPlayOnAnimation();
			}
		}
		else if (onAnimationIsActive)
		{
			ReverseOnAnimation();
		}
		else if (evt.animateChange)
		{
			PlayOffAnimation();
		}
		else
		{
			InstantPlayOffAnimation();
		}
	}

	private IEnumerator InitializeAfterAnimatorInitialized()
	{
		yield return new WaitUntil(() => base.animatorInitialized);
		yield return new WaitForEndOfFrame();
		OnValueChanged(new ToggleValueChangedEvent(base.controller.isOn, base.controller.isOn, animateChange: false));
		initializeCoroutine = null;
	}

	public virtual void PlayOnAnimation()
	{
		if (base.animatorInitialized)
		{
			if (offAnimationIsActive)
			{
				stopOffAnimation?.Invoke();
			}
			playOnAnimation?.Invoke();
		}
	}

	public virtual void PlayOffAnimation()
	{
		if (base.animatorInitialized)
		{
			if (onAnimationIsActive)
			{
				stopOnAnimation?.Invoke();
			}
			playOffAnimation?.Invoke();
		}
	}

	public virtual void InstantPlayOnAnimation()
	{
		if (base.animatorInitialized)
		{
			if (offAnimationIsActive)
			{
				stopOffAnimation?.Invoke();
			}
			instantPlayOnAnimation?.Invoke();
		}
	}

	public virtual void InstantPlayOffAnimation()
	{
		if (base.animatorInitialized)
		{
			if (onAnimationIsActive)
			{
				stopOnAnimation?.Invoke();
			}
			instantPlayOffAnimation?.Invoke();
		}
	}

	public virtual void ReverseOnAnimation()
	{
		if (base.animatorInitialized)
		{
			if (offAnimationIsActive)
			{
				StopOffAnimation();
			}
			addResetToOffStateCallback?.Invoke();
			reverseOnAnimation?.Invoke();
		}
	}

	public virtual void ReverseOffAnimation()
	{
		if (base.animatorInitialized)
		{
			if (onAnimationIsActive)
			{
				StopOnAnimation();
			}
			addResetToOnStateCallback?.Invoke();
			reverseOffAnimation?.Invoke();
		}
	}

	public virtual void StopOnAnimation()
	{
		if (base.animatorInitialized)
		{
			stopOnAnimation?.Invoke();
		}
	}

	public virtual void StopOffAnimation()
	{
		if (base.animatorInitialized)
		{
			stopOffAnimation?.Invoke();
		}
	}

	public virtual void ResetToOnState()
	{
		removeResetToOnStateCallback?.Invoke();
		if (base.controller.isOn)
		{
			instantPlayOnAnimation?.Invoke();
		}
	}

	public virtual void ResetToOffState()
	{
		removeResetToOffStateCallback?.Invoke();
		if (!base.controller.isOn)
		{
			instantPlayOffAnimation?.Invoke();
		}
	}
}
