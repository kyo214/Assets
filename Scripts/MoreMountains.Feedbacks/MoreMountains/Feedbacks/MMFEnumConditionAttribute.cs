using System;
using System.Collections;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
public class MMFEnumConditionAttribute : PropertyAttribute
{
	public string ConditionEnum = "";

	public bool Hidden;

	private BitArray bitArray = new BitArray(32);

	public bool ContainsBitFlag(int enumValue)
	{
		return bitArray.Get(enumValue);
	}

	public MMFEnumConditionAttribute(string conditionBoolean, params int[] enumValues)
	{
		ConditionEnum = conditionBoolean;
		Hidden = true;
		for (int i = 0; i < enumValues.Length; i++)
		{
			bitArray.Set(enumValues[i], value: true);
		}
	}
}
