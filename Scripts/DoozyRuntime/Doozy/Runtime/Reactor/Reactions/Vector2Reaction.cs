using System;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions;

[Serializable]
public class Vector2Reaction : DynamicReaction<Vector2, Vector2>
{
	public Vector2Reaction()
	{
		FromValue = Vector2.zero;
		ToValue = Vector2.one;
	}

	public override float GetProgressAtValue(Vector2 value)
	{
		return Vector2Extensions.InverseLerp(base.fromValue, base.toValue, value);
	}

	public override void UpdateCurrentValue()
	{
		CurrentValue = Vector2.LerpUnclamped(base.cycleFrom, base.cycleTo, base.currentCycleEasedProgress);
		base.setter?.Invoke(CurrentValue);
		OnValueChangedCallback?.Invoke(CurrentValue);
	}

	public override Reaction SetValue(Vector2 value)
	{
		base.SetValue(value);
		base.setter?.Invoke(value);
		return this;
	}

	public override Reaction SetFrom(Vector2 value, bool relative = false)
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

	public override Reaction SetTo(Vector2 value, bool relative = false)
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
			Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
			base.cycleValues[i] = FromValue + new Vector2(ToValue.x * insideUnitCircle.x, ToValue.y * insideUnitCircle.y) * base.settings.strength;
			base.cycleValues[i] = base.cycleValues[i].Round(4);
		}
		base.cycleValues[base.numberOfCycles - 1] = FromValue;
	}
}
