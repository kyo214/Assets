using System;
using UnityEngine;

namespace Doozy.Runtime.Common;

[Serializable]
public class RandomInt
{
	[SerializeField]
	private int MIN;

	[SerializeField]
	private int MAX;

	public int min
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

	public int max
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

	public int currentValue { get; private set; }

	public int previousValue { get; private set; }

	public int randomValue
	{
		get
		{
			previousValue = currentValue;
			currentValue = random;
			int num = 100;
			while (currentValue == previousValue && num > 0)
			{
				currentValue = random;
				num--;
			}
			return currentValue;
		}
	}

	private int random => UnityEngine.Random.Range(MIN, MAX + 1);

	public RandomInt(RandomInt other)
		: this(other.min, other.max)
	{
	}

	public RandomInt()
		: this(0, 1)
	{
	}

	public RandomInt(int minValue, int maxValue)
	{
		Reset(minValue, maxValue);
	}

	public void Reset(int minValue = 0, int maxValue = 1)
	{
		MIN = minValue;
		MAX = maxValue;
		int num = (currentValue = minValue);
		previousValue = num;
	}
}
