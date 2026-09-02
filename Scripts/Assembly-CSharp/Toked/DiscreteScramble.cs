using System.Collections.Generic;
using UnityEngine;

namespace Toked;

public class DiscreteScramble : IScramble
{
	private Dictionary<Vector2, Vector2> _mapping;

	private bool _allowMirror;

	private readonly Vector2[] _directions = new Vector2[4]
	{
		Vector2.up,
		Vector2.down,
		Vector2.left,
		Vector2.right
	};

	public DiscreteScramble(bool allowMirror = true)
	{
		_allowMirror = allowMirror;
		Generate();
	}

	public IScramble Generate()
	{
		_mapping = new Dictionary<Vector2, Vector2>();
		List<Vector2> list = new List<Vector2>(_directions);
		Vector2[] directions = _directions;
		foreach (Vector2 vector in directions)
		{
			List<Vector2> list2 = new List<Vector2>(list);
			if (!_allowMirror)
			{
				Vector2 item = -vector;
				list2.Remove(item);
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
}
