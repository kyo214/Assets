using System;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Animations;

[Serializable]
public class UIAnimation : ReactorAnimation
{
	[SerializeField]
	private UIAnimationType AnimationType;

	public UIMoveReaction Move;

	public UIRotateReaction Rotate;

	public UIScaleReaction Scale;

	public UIFadeReaction Fade;

	public RectTransform rectTransform { get; internal set; }

	public override bool hasTarget => rectTransform != null;

	public CanvasGroup canvasGroup { get; internal set; }

	public UIAnimationType animationType
	{
		get
		{
			return AnimationType;
		}
		set
		{
			AnimationType = value;
			Move.animationType = value;
		}
	}

	public Vector3 startPosition
	{
		get
		{
			return Move.startPosition;
		}
		set
		{
			Move.startPosition = value;
		}
	}

	public Vector3 startRotation
	{
		get
		{
			return Rotate.startRotation;
		}
		set
		{
			Rotate.startRotation = value;
		}
	}

	public Vector3 startScale
	{
		get
		{
			return Scale.startScale;
		}
		set
		{
			Scale.startScale = value;
		}
	}

	public float startAlpha
	{
		get
		{
			return Fade.startAlpha;
		}
		set
		{
			Fade.startAlpha = value;
		}
	}

	public override bool isEnabled => Move.enabled | Rotate.enabled | Scale.enabled | Fade.enabled;

	public override bool isIdle => Move.isIdle | Rotate.isIdle | Scale.isIdle | Fade.isIdle;

	public override bool isActive => Move.isActive | Rotate.isActive | Scale.isActive | Fade.isActive;

	public override bool isPaused => Move.isPaused | Rotate.isPaused | Scale.isPaused | Fade.isPaused;

	public override bool isPlaying
	{
		get
		{
			if (!Move.isPlaying && !Rotate.isPlaying && !Scale.isPlaying)
			{
				return Fade.isPlaying;
			}
			return true;
		}
	}

	public override bool inStartDelay => Move.inStartDelay | Rotate.inStartDelay | Scale.inStartDelay | Fade.inStartDelay;

	public override bool inLoopDelay => Move.inLoopDelay | Rotate.inLoopDelay | Scale.inLoopDelay | Fade.inLoopDelay;

	public UIAnimation(RectTransform targetRectTransform, CanvasGroup targetCanvasGroup = null)
	{
		SetTarget(targetRectTransform, targetCanvasGroup);
	}

	public void SetTarget(RectTransform targetRectTransform, CanvasGroup targetCanvasGroup = null)
	{
		rectTransform = null;
		canvasGroup = null;
		if (!targetRectTransform)
		{
			throw new NullReferenceException("targetRectTransform");
		}
		rectTransform = targetRectTransform;
		if (targetCanvasGroup == null)
		{
			targetCanvasGroup = targetRectTransform.gameObject.GetComponent<CanvasGroup>();
		}
		canvasGroup = ((targetCanvasGroup == null) ? targetRectTransform.gameObject.AddComponent<CanvasGroup>() : targetCanvasGroup);
		Initialize();
	}

	public void Initialize()
	{
		Move?.Stop(silent: true);
		if (Move == null)
		{
			Move = Reaction.Get<UIMoveReaction>();
		}
		Move.SetTarget(rectTransform);
		Move.animationType = animationType;
		Rotate?.Stop(silent: true);
		if (Rotate == null)
		{
			Rotate = Reaction.Get<UIRotateReaction>();
		}
		Rotate.SetTarget(rectTransform);
		Scale?.Stop(silent: true);
		if (Scale == null)
		{
			Scale = Reaction.Get<UIScaleReaction>();
		}
		Scale.SetTarget(rectTransform);
		Fade?.Stop(silent: true);
		if (Fade == null)
		{
			Fade = Reaction.Get<UIFadeReaction>();
		}
		Fade.SetTarget(rectTransform, canvasGroup);
		UpdateValues();
	}

	public override void Recycle()
	{
		Move?.Recycle();
		Rotate?.Recycle();
		Scale?.Recycle();
		Fade?.Recycle();
	}

	public override void UpdateValues()
	{
		if (canvasGroup != null)
		{
			Fade.UpdateValues();
		}
		Scale.UpdateValues();
		Rotate.UpdateValues();
		Move.UseCustomLocalScale = Scale.enabled;
		Move.CustomFromLocalScale = (Scale.enabled ? Scale.fromValue : startScale);
		Move.CustomToLocalScale = (Scale.enabled ? Scale.toValue : startScale);
		Move.UseCustomLocalRotation = Rotate.enabled;
		Move.CustomFromLocalRotation = (Rotate.enabled ? Rotate.fromValue : startRotation);
		Move.CustomToLocalRotation = (Rotate.enabled ? Rotate.toValue : startRotation);
		Move.animationType = animationType;
		Move.UpdateValues();
	}

	public override void StopAllReactionsOnTarget()
	{
		Reaction.StopAllReactionsByTargetObject(rectTransform, silent: true);
	}

	public override void SetProgressAt(float targetProgress)
	{
		base.SetProgressAt(targetProgress);
		if (Fade.enabled)
		{
			Fade.SetProgressAt(targetProgress);
		}
		if (Scale.enabled)
		{
			Scale.SetProgressAt(targetProgress);
		}
		if (Rotate.enabled)
		{
			Rotate.SetProgressAt(targetProgress);
		}
		if (Move.enabled)
		{
			Move.SetProgressAt(targetProgress);
		}
		if (animationType != UIAnimationType.Custom)
		{
			ResetToStartValues();
		}
	}

	public override void PlayToProgress(float toProgress)
	{
		base.PlayToProgress(toProgress);
		if (Fade.enabled)
		{
			Fade.PlayToProgress(toProgress);
		}
		if (Scale.enabled)
		{
			Scale.PlayToProgress(toProgress);
		}
		if (Rotate.enabled)
		{
			Rotate.PlayToProgress(toProgress);
		}
		if (Move.enabled)
		{
			Move.PlayToProgress(toProgress);
		}
		if (animationType != UIAnimationType.Custom)
		{
			ResetToStartValues();
		}
	}

	public override void PlayFromProgress(float fromProgress)
	{
		base.PlayFromProgress(fromProgress);
		if (Move.enabled)
		{
			Move.PlayFromProgress(fromProgress);
		}
		if (Rotate.enabled)
		{
			Rotate.PlayFromProgress(fromProgress);
		}
		if (Scale.enabled)
		{
			Scale.PlayFromProgress(fromProgress);
		}
		if (Fade.enabled)
		{
			Fade.PlayFromProgress(fromProgress);
		}
		if (animationType != UIAnimationType.Custom)
		{
			ResetToStartValues();
		}
	}

	public override void PlayFromToProgress(float fromProgress, float toProgress)
	{
		base.PlayFromToProgress(fromProgress, toProgress);
		if (Move.enabled)
		{
			Move.PlayFromToProgress(fromProgress, toProgress);
		}
		if (Rotate.enabled)
		{
			Rotate.PlayFromToProgress(fromProgress, toProgress);
		}
		if (Scale.enabled)
		{
			Scale.PlayFromToProgress(fromProgress, toProgress);
		}
		if (Fade.enabled)
		{
			Fade.PlayFromToProgress(fromProgress, toProgress);
		}
		if (animationType != UIAnimationType.Custom)
		{
			ResetToStartValues();
		}
	}

	public override void Play(bool inReverse = false)
	{
		if (!(rectTransform == null))
		{
			RegisterCallbacks();
			if (!isActive)
			{
				StopAllReactionsOnTarget();
				ResetToStartValues();
			}
			if (Move.enabled)
			{
				Move.Play(inReverse);
			}
			if (Rotate.enabled)
			{
				Rotate.Play(inReverse);
			}
			if (Scale.enabled)
			{
				Scale.Play(inReverse);
			}
			if (Fade.enabled)
			{
				Fade.Play(inReverse);
			}
		}
	}

	public override void ResetToStartValues(bool forced = false)
	{
		if (!(rectTransform == null))
		{
			if (forced || !Move.enabled)
			{
				Move.SetValue(startPosition);
			}
			if (forced || !Rotate.enabled)
			{
				Rotate.SetValue(startRotation);
			}
			if (forced || !Scale.enabled)
			{
				Scale.SetValue(startScale);
			}
			if (forced || !Fade.enabled)
			{
				Fade.SetValue(startAlpha);
			}
		}
	}

	public override void Stop()
	{
		if (Move.isActive || Move.enabled)
		{
			Move.Stop();
		}
		if (Rotate.isActive || Rotate.enabled)
		{
			Rotate.Stop();
		}
		if (Scale.isActive || Scale.enabled)
		{
			Scale.Stop();
		}
		if (Fade.isActive || Fade.enabled)
		{
			Fade.Stop();
		}
		base.Stop();
	}

	public override void Finish()
	{
		if (Move.isActive || Move.enabled)
		{
			Move.Finish();
		}
		if (Rotate.isActive || Rotate.enabled)
		{
			Rotate.Finish();
		}
		if (Scale.isActive || Scale.enabled)
		{
			Scale.Finish();
		}
		if (Fade.isActive || Fade.enabled)
		{
			Fade.Finish();
		}
		base.Finish();
	}

	public override void Reverse()
	{
		if (Move.isActive)
		{
			Move.Reverse();
		}
		else if (Move.enabled)
		{
			Move.Play(PlayDirection.Reverse);
		}
		if (Rotate.isActive)
		{
			Rotate.Reverse();
		}
		else if (Rotate.enabled)
		{
			Rotate.Play(PlayDirection.Reverse);
		}
		if (Scale.isActive)
		{
			Scale.Reverse();
		}
		else if (Scale.enabled)
		{
			Scale.Play(PlayDirection.Reverse);
		}
		if (Fade.isActive)
		{
			Fade.Reverse();
		}
		else if (Fade.enabled)
		{
			Fade.Play(PlayDirection.Reverse);
		}
	}

	public override void Rewind()
	{
		if (Move.enabled)
		{
			Move.Rewind();
		}
		if (Rotate.enabled)
		{
			Rotate.Rewind();
		}
		if (Scale.enabled)
		{
			Scale.Rewind();
		}
		if (Fade.enabled)
		{
			Fade.Rewind();
		}
	}

	public override void Pause()
	{
		Move.Pause();
		Rotate.Pause();
		Scale.Pause();
		Fade.Pause();
	}

	public override void Resume()
	{
		Move.Resume();
		Rotate.Resume();
		Scale.Resume();
		Fade.Resume();
	}

	public float GetStartDelay()
	{
		float num = ((!Move.enabled) ? 0f : (Move.isActive ? Move.startDelay : Move.settings.GetStartDelay()));
		float num2 = ((!Rotate.enabled) ? 0f : (Rotate.isActive ? Rotate.startDelay : Rotate.settings.GetStartDelay()));
		float num3 = ((!Scale.enabled) ? 0f : (Scale.isActive ? Scale.startDelay : Scale.settings.GetStartDelay()));
		float num4 = ((!Fade.enabled) ? 0f : (Fade.isActive ? Fade.startDelay : Fade.settings.GetStartDelay()));
		return num + num2 + num3 + num4;
	}

	public float GetDuration()
	{
		float num = ((!Move.enabled) ? 0f : (Move.isActive ? Move.duration : Move.settings.GetDuration()));
		float num2 = ((!Rotate.enabled) ? 0f : (Rotate.isActive ? Rotate.duration : Rotate.settings.GetDuration()));
		float num3 = ((!Scale.enabled) ? 0f : (Scale.isActive ? Scale.duration : Scale.settings.GetDuration()));
		float num4 = ((!Fade.enabled) ? 0f : (Fade.isActive ? Fade.duration : Fade.settings.GetDuration()));
		return num + num2 + num3 + num4;
	}

	public float GetTotalDuration()
	{
		return GetStartDelay() + GetDuration();
	}

	protected override void RegisterCallbacks()
	{
		base.RegisterCallbacks();
		if (Move.enabled)
		{
			base.startedReactionsCount++;
			UIMoveReaction move = Move;
			move.OnPlayCallback = (ReactionCallback)Delegate.Combine(move.OnPlayCallback, new ReactionCallback(base.InvokeOnPlay));
			UIMoveReaction move2 = Move;
			move2.OnStopCallback = (ReactionCallback)Delegate.Combine(move2.OnStopCallback, new ReactionCallback(base.InvokeOnStop));
			UIMoveReaction move3 = Move;
			move3.OnFinishCallback = (ReactionCallback)Delegate.Combine(move3.OnFinishCallback, new ReactionCallback(base.InvokeOnFinish));
		}
		if (Rotate.enabled)
		{
			base.startedReactionsCount++;
			UIRotateReaction rotate = Rotate;
			rotate.OnPlayCallback = (ReactionCallback)Delegate.Combine(rotate.OnPlayCallback, new ReactionCallback(base.InvokeOnPlay));
			UIRotateReaction rotate2 = Rotate;
			rotate2.OnStopCallback = (ReactionCallback)Delegate.Combine(rotate2.OnStopCallback, new ReactionCallback(base.InvokeOnStop));
			UIRotateReaction rotate3 = Rotate;
			rotate3.OnFinishCallback = (ReactionCallback)Delegate.Combine(rotate3.OnFinishCallback, new ReactionCallback(base.InvokeOnFinish));
		}
		if (Scale.enabled)
		{
			base.startedReactionsCount++;
			UIScaleReaction scale = Scale;
			scale.OnPlayCallback = (ReactionCallback)Delegate.Combine(scale.OnPlayCallback, new ReactionCallback(base.InvokeOnPlay));
			UIScaleReaction scale2 = Scale;
			scale2.OnStopCallback = (ReactionCallback)Delegate.Combine(scale2.OnStopCallback, new ReactionCallback(base.InvokeOnStop));
			UIScaleReaction scale3 = Scale;
			scale3.OnFinishCallback = (ReactionCallback)Delegate.Combine(scale3.OnFinishCallback, new ReactionCallback(base.InvokeOnFinish));
		}
		if (Fade.enabled)
		{
			base.startedReactionsCount++;
			UIFadeReaction fade = Fade;
			fade.OnPlayCallback = (ReactionCallback)Delegate.Combine(fade.OnPlayCallback, new ReactionCallback(base.InvokeOnPlay));
			UIFadeReaction fade2 = Fade;
			fade2.OnStopCallback = (ReactionCallback)Delegate.Combine(fade2.OnStopCallback, new ReactionCallback(base.InvokeOnStop));
			UIFadeReaction fade3 = Fade;
			fade3.OnFinishCallback = (ReactionCallback)Delegate.Combine(fade3.OnFinishCallback, new ReactionCallback(base.InvokeOnFinish));
		}
	}

	protected override void UnregisterOnPlayCallbacks()
	{
		if (Move.enabled)
		{
			UIMoveReaction move = Move;
			move.OnPlayCallback = (ReactionCallback)Delegate.Remove(move.OnPlayCallback, new ReactionCallback(base.InvokeOnPlay));
		}
		if (Rotate.enabled)
		{
			UIRotateReaction rotate = Rotate;
			rotate.OnPlayCallback = (ReactionCallback)Delegate.Remove(rotate.OnPlayCallback, new ReactionCallback(base.InvokeOnPlay));
		}
		if (Scale.enabled)
		{
			UIScaleReaction scale = Scale;
			scale.OnPlayCallback = (ReactionCallback)Delegate.Remove(scale.OnPlayCallback, new ReactionCallback(base.InvokeOnPlay));
		}
		if (Fade.enabled)
		{
			UIFadeReaction fade = Fade;
			fade.OnPlayCallback = (ReactionCallback)Delegate.Remove(fade.OnPlayCallback, new ReactionCallback(base.InvokeOnPlay));
		}
	}

	protected override void UnregisterOnStopCallbacks()
	{
		if (Move.enabled)
		{
			UIMoveReaction move = Move;
			move.OnStopCallback = (ReactionCallback)Delegate.Remove(move.OnStopCallback, new ReactionCallback(base.InvokeOnStop));
		}
		if (Rotate.enabled)
		{
			UIRotateReaction rotate = Rotate;
			rotate.OnStopCallback = (ReactionCallback)Delegate.Remove(rotate.OnStopCallback, new ReactionCallback(base.InvokeOnStop));
		}
		if (Scale.enabled)
		{
			UIScaleReaction scale = Scale;
			scale.OnStopCallback = (ReactionCallback)Delegate.Remove(scale.OnStopCallback, new ReactionCallback(base.InvokeOnStop));
		}
		if (Fade.enabled)
		{
			UIFadeReaction fade = Fade;
			fade.OnStopCallback = (ReactionCallback)Delegate.Remove(fade.OnStopCallback, new ReactionCallback(base.InvokeOnStop));
		}
	}

	protected override void UnregisterOnFinishCallbacks()
	{
		if (Move.enabled)
		{
			UIMoveReaction move = Move;
			move.OnFinishCallback = (ReactionCallback)Delegate.Remove(move.OnFinishCallback, new ReactionCallback(base.InvokeOnFinish));
		}
		if (Rotate.enabled)
		{
			UIRotateReaction rotate = Rotate;
			rotate.OnFinishCallback = (ReactionCallback)Delegate.Remove(rotate.OnFinishCallback, new ReactionCallback(base.InvokeOnFinish));
		}
		if (Scale.enabled)
		{
			UIScaleReaction scale = Scale;
			scale.OnFinishCallback = (ReactionCallback)Delegate.Remove(scale.OnFinishCallback, new ReactionCallback(base.InvokeOnFinish));
		}
		if (Fade.enabled)
		{
			UIFadeReaction fade = Fade;
			fade.OnFinishCallback = (ReactionCallback)Delegate.Remove(fade.OnFinishCallback, new ReactionCallback(base.InvokeOnFinish));
		}
	}
}
