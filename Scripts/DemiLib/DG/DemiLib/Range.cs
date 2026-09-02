using System;
using UnityEngine;

namespace DG.DemiLib;

[Serializable]
public struct Range(float min, float max)
{
	public float min = min;

	public float max = max;

	public float RandomWithin()
	{
		return UnityEngine.Random.Range(min, max);
	}

	public override string ToString()
	{
		return "(" + min + "/" + max + ")";
	}
}
