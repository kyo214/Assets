using System;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Internal;

[Serializable]
public abstract class DynamicReaction<T1, T2> : Reaction
{
	[SerializeField]
	protected internal T2 FromValue;

	[SerializeField]
	protected internal T2 ToValue;

	[SerializeField]
	protected internal T2 CurrentValue;

	public ReactionCallback<T2> OnValueChangedCallback;

	internal Type typeOfPropertyType { get; set; }

	internal Type typeOfValueType { get; set; }

	public T2 fromValue => FromValue;

	public T2 toValue => ToValue;

	public T2 currentValue => CurrentValue;

	public PropertyGetter<T1> getter { get; set; }

	public PropertySetter<T1> setter { get; set; }

	protected T2[] cycleValues { get; set; }

	protected T2 cycleFrom
	{
		get
		{
			if (base.currentCycleIndex != 0)
			{
				return cycleValues[base.currentCycleIndex - 1];
			}
			return FromValue;
		}
	}

	protected T2 cycleTo
	{
		get
		{
			if (base.currentCycleIndex != 0)
			{
				return cycleValues[base.currentCycleIndex];
			}
			return ToValue;
		}
	}

	protected DynamicReaction()
	{
		typeOfPropertyType = typeof(T1);
		typeOfValueType = typeof(T2);
	}

	public abstract float GetProgressAtValue(T2 value);

	public override void Reset()
	{
		base.Reset();
		getter = null;
		setter = null;
		OnValueChangedCallback = null;
	}

	public abstract Reaction SetFrom(T2 value, bool relative = false);

	public abstract Reaction SetTo(T2 value, bool relative = false);

	public virtual Reaction SetValue(T2 value)
	{
		if (base.isActive)
		{
			Stop();
		}
		CurrentValue = value;
		return this;
	}

	public virtual Reaction PlayToValue(T2 value, bool relative = false)
	{
		if (base.isActive)
		{
			Stop();
		}
		SetFrom(CurrentValue);
		SetTo(value, relative);
		Play();
		return this;
	}

	public virtual Reaction PlayFromValue(T2 value, bool relative = false)
	{
		if (base.isActive)
		{
			Stop();
		}
		SetFrom(value, relative);
		SetTo(CurrentValue);
		Play();
		return this;
	}

	public virtual Reaction Play(T2 from, T2 to, bool reversed = false)
	{
		SetFrom(from);
		SetTo(to);
		Play(reversed);
		return this;
	}

	public override void Stop(bool silent = false, bool recycle = false)
	{
		switch (base.settings.playMode)
		{
		case PlayMode.Spring:
		case PlayMode.Shake:
			if (base.isPlaying)
			{
				base.elapsedDuration = ((base.direction == PlayDirection.Forward) ? base.startDuration : base.targetDuration);
				UpdateCurrentValue();
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case PlayMode.Normal:
		case PlayMode.PingPong:
			break;
		}
		base.Stop(silent, recycle);
	}

	protected override void ComputeNormal()
	{
		base.ComputeNormal();
		cycleValues = new T2[1] { ToValue };
	}

	protected override void ComputePingPong()
	{
		base.ComputePingPong();
		cycleValues = new T2[2] { ToValue, FromValue };
	}

	protected override void ComputeSpring()
	{
		base.ComputeSpring();
		cycleValues = new T2[base.numberOfCycles];
	}

	protected override void ComputeShake()
	{
		base.ComputeShake();
		cycleValues = new T2[base.numberOfCycles];
	}
}
