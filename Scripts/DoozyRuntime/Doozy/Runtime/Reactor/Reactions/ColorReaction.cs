using System;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions;

[Serializable]
public class ColorReaction : DynamicReaction<Color, Color>
{
	public ColorReaction()
	{
		FromValue = Color.white;
		ToValue = Color.black;
	}

	public override float GetProgressAtValue(Color value)
	{
		return (value.Hue() - base.fromValue.Hue()) / (base.toValue.Hue() - base.fromValue.Hue());
	}

	public override void UpdateCurrentValue()
	{
		CurrentValue = Color.LerpUnclamped(base.cycleFrom, base.cycleTo, base.currentCycleEasedProgress);
		base.setter?.Invoke(CurrentValue);
		OnValueChangedCallback?.Invoke(CurrentValue);
	}

	public override Reaction SetValue(Color value)
	{
		base.SetValue(value);
		base.setter?.Invoke(CurrentValue);
		return this;
	}

	public override Reaction SetFrom(Color value, bool relative = false)
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

	public override Reaction SetTo(Color value, bool relative = false)
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
			Color color = UnityEngine.Random.ColorHSV();
			base.cycleValues[i] = FromValue + ToValue * color * base.settings.strength;
		}
		base.cycleValues[base.numberOfCycles - 1] = FromValue;
	}
}
