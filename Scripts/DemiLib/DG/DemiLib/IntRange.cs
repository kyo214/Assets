using System;
using UnityEngine;

namespace DG.DemiLib;

[Serializable]
public struct IntRange(int min, int max)
{
	public int min = min;

	public int max = max;

	public float RandomWithin()
	{
		return UnityEngine.Random.Range(min, max + 1);
	}

	public override string ToString()
	{
		return "(" + min + "/" + max + ")";
	}
}
