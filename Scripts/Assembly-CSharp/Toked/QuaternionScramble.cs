using UnityEngine;

namespace Toked;

public class QuaternionScramble : IScramble
{
	private Quaternion _rotation;

	private readonly bool _strictRandomRotation;

	private int[] _angles = new int[4] { 0, 90, 180, 270 };

	public QuaternionScramble(Quaternion rotation)
	{
		_rotation = rotation;
	}

	public QuaternionScramble(bool strictRandomRotation = false)
	{
		_strictRandomRotation = strictRandomRotation;
		Generate();
	}

	public IScramble Generate()
	{
		float z = _angles[Random.Range(_strictRandomRotation ? 1 : 0, _angles.Length)];
		_rotation = Quaternion.Euler(0f, 0f, z);
		return this;
	}

	public Vector2 Apply(Vector2 input)
	{
		return _rotation * input;
	}
}
