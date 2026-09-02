using System.Collections.Generic;
using UnityEngine;

namespace Toked;

public class DiscreteScramble8 : IScramble
{
	private Dictionary<Vector2, Vector2> _mapping;

	private bool _allowMirror;

	private static readonly Vector2[] directions = new Vector2[8]
	{
		Vector2.up,
		Vector2.down,
		Vector2.left,
		Vector2.right,
		new Vector2(1f, 1f).normalized,
		new Vector2(-1f, 1f).normalized,
		new Vector2(1f, -1f).normalized,
		new Vector2(-1f, -1f).normalized
	};

	public DiscreteScramble8(bool allowMirror = true)
	{
		_allowMirror = allowMirror;
		Generate();
	}

	public IScramble Generate()
	{
		_mapping = new Dictionary<Vector2, Vector2>();
		List<Vector2> list = new List<Vector2>(directions);
		Vector2[] array = directions;
		foreach (Vector2 vector in array)
		{
			List<Vector2> list2 = new List<Vector2>(list);
			if (!_allowMirror)
			{
				Vector2 opposite = -vector;
				list2.RemoveAll((Vector2 t) => ApproximatelyEqual(t, opposite));
			}
			if (list2.Count != 0)
			{
				int index = Random.Range(0, list2.Count);
				Vector2 vector2 = list2[index];
				_mapping[vector] = vector2;
				list.Remove(vector2);
			}
		}
		return this;
	}

	public Vector2 Apply(Vector2 input)
	{
		if (input == Vector2.zero)
		{
			return Vector2.zero;
		}
		Vector2 key = Vector2.zero;
		float num = float.NegativeInfinity;
		foreach (Vector2 key2 in _mapping.Keys)
		{
			float num2 = Vector2.Dot(input.normalized, key2);
			if (num2 > num)
			{
				num = num2;
				key = key2;
			}
		}
		return _mapping[key] * input.magnitude;
	}

	public void PrintMapping()
	{
		foreach (KeyValuePair<Vector2, Vector2> item in _mapping)
		{
			Debug.Log($"{item.Key} -> {item.Value}");
		}
	}

	private bool ApproximatelyEqual(Vector2 a, Vector2 b, float tolerance = 0.01f)
	{
		return Vector2.Distance(a, b) < tolerance;
	}
}
