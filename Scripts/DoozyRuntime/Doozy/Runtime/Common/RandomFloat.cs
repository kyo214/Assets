using System;
using UnityEngine;

namespace Doozy.Runtime.Common;

[Serializable]
public class RandomFloat
{
	[SerializeField]
	private float MIN;

	[SerializeField]
	private float MAX;

	public float min
	{
		get
		{
			return MIN;
		}
		set
		{
			MIN = value;
		}
	}

	public float max
	{
		get
		{
			return MAX;
		}
		set
		{
			MAX = value;
		}
	}

	public float currentValue { get; private set; }

	public float previousValue { get; private set; }

	public float randomValue
	{
		get
		{
			previousValue = currentValue;
			currentValue = random;
			int num = 100;
			while (Mathf.Approximately(currentValue, previousValue) && num > 0)
			{
				currentValue = random;
				num--;
			}
			return currentValue;
		}
	}

	private float random => UnityEngine.Random.Range(MIN, MAX);

	public RandomFloat(RandomFloat other)
		: this(other.min, other.max)
	{
	}

	public RandomFloat()
		: this(0f, 1f)
	{
	}

	public RandomFloat(float minValue, float maxValue)
	{
		Reset(minValue, maxValue);
	}

	public void Reset(float minValue = 0f, float maxValue = 1f)
	{
		MIN = minValue;
		MAX = maxValue;
		float num = (currentValue = minValue);
		previousValue = num;
	}
}
