using System;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions;

[Serializable]
public class FloatReaction : DynamicReaction<float, float>
{
	public FloatReaction()
	{
		FromValue = 0f;
		ToValue = 1f;
	}

	public override float GetProgressAtValue(float value)
	{
		return Mathf.Clamp01(Mathf.InverseLerp(FromValue, ToValue, value));
	}

	public override void UpdateCurrentValue()
	{
		CurrentValue = Mathf.LerpUnclamped(base.cycleFrom, base.cycleTo, base.currentCycleEasedProgress);
		base.setter?.Invoke(CurrentValue);
		OnValueChangedCallback?.Invoke(CurrentValue);
	}

	public override Reaction SetValue(float value)
	{
		base.SetValue(value);
		base.setter?.Invoke(value);
		return this;
	}

	public override Reaction SetFrom(float value, bool relative = false)
	{
		FromValue = value;
		if (relative)
		{
			FromValue += CurrentValue;
		}
		if (base.isActive)
		{
			ComputePlayMode();
		}
		return this;
	}

	public override Reaction SetTo(float value, bool relative = false)
	{
		ToValue = value;
		if (relative)
		{
			ToValue += CurrentValue;
		}
		if (base.isActive)
		{
			ComputePlayMode();
		}
		return this;
	}

	protected override void ComputeSpring()
	{
		base.ComputeSpring();
		float num = base.settings.strength;
		float num2 = num / (float)(base.numberOfCycles - 1);
		for (int i = 0; i < base.numberOfCycles; i++)
		{
			base.cycleValues[i] = FromValue + ToValue * ((i % 2 == 0) ? num : ((0f - num) * base.settings.elasticity));
			base.cycleValues[i] = base.cycleValues[i].Round(4);
			num -= num2;
		}
		base.cycleValues[base.numberOfCycles - 1] = FromValue;
	}

	protected override void ComputeShake()
	{
		base.ComputeShake();
		for (int i = 0; i < base.numberOfCycles; i++)
		{
			if (i % 2 == 0)
			{
				base.cycleValues[i] = FromValue;
				continue;
			}
			float value = UnityEngine.Random.value;
			base.cycleValues[i] = FromValue + ToValue * value * base.settings.strength;
			base.cycleValues[i] = base.cycleValues[i].Round(4);
		}
		base.cycleValues[base.numberOfCycles - 1] = FromValue;
	}
}
