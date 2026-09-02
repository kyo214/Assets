using System;
using UnityEngine;

namespace DG.Tweening.Plugins.Core.PathCore;

[Serializable]
public struct ControlPoint(Vector3 a, Vector3 b)
{
	public Vector3 a = a;

	public Vector3 b = b;

	public static ControlPoint operator +(ControlPoint cp, Vector3 v)
	{
		return new ControlPoint(cp.a + v, cp.b + v);
	}

	public override string ToString()
	{
		return "[" + a.ToString() + " | " + b.ToString() + "]";
	}
}
