using System;
using UnityEngine;

namespace Fusion;

[Serializable]
public struct Accuracy
{
	[SerializeField]
	internal float _value;

	[SerializeField]
	internal float _inverse;

	[SerializeField]
	internal int _hash;

	public float Value
	{
		get
		{
			if (_inverse == 0f)
			{
				GetAccuracyFromHash();
			}
			return _value;
		}
	}

	internal float Inverse
	{
		get
		{
			if (_inverse == 0f)
			{
				GetAccuracyFromHash();
			}
			return _inverse;
		}
	}

	private void GetAccuracyFromHash()
	{
		if (NetworkProjectConfig.Global.AccuracyDefaults.TryGetAccuracy(_hash, out var accuracy))
		{
			_value = accuracy._value;
			_inverse = 1f / _value;
		}
		else
		{
			_value = 0.001f;
			_inverse = 999.99994f;
		}
	}

	internal void SetValue(float accuracy)
	{
		if (accuracy == 0f)
		{
			_value = 0f;
			_inverse = float.PositiveInfinity;
		}
		else
		{
			_value = accuracy;
			_inverse = 1f / accuracy;
		}
	}

	internal void UseGlobalAccuracy(string tag)
	{
		if (tag != null && tag != "")
		{
			_hash = tag.GetHashDeterministic();
			_inverse = 0f;
		}
		else
		{
			_inverse = 1f / _value;
		}
	}

	public WriteAccuracy GetWriteAccuracy(NetworkProjectConfig config)
	{
		Accuracy accuracy;
		if (_inverse == 0f)
		{
			bool flag = config.AccuracyDefaults.TryGetAccuracy(_hash, out accuracy);
		}
		else
		{
			accuracy = this;
		}
		if (accuracy._value == 0f)
		{
			return new WriteAccuracy(0f);
		}
		return new WriteAccuracy(accuracy._inverse);
	}

	public ReadAccuracy GetReadAccuracy(NetworkProjectConfig config)
	{
		Accuracy accuracy;
		if (_inverse == 0f)
		{
			bool flag = config.AccuracyDefaults.TryGetAccuracy(_hash, out accuracy);
		}
		else
		{
			accuracy = this;
		}
		return new ReadAccuracy(accuracy._value);
	}

	public Accuracy(float accuracy)
	{
		if (accuracy == 0f)
		{
			_value = 0f;
			_inverse = float.PositiveInfinity;
		}
		else
		{
			_value = accuracy;
			_inverse = 1f / accuracy;
		}
		_hash = 0;
	}

	internal Accuracy(string tag, float value)
		: this(value)
	{
		_hash = tag.GetHashDeterministic();
	}

	public Accuracy(string tag)
	{
		_hash = tag.GetHashDeterministic();
		_value = 0f;
		_inverse = 0f;
	}

	public static implicit operator Accuracy(float value)
	{
		return new Accuracy(value);
	}

	public override string ToString()
	{
		return "[" + _hash + " v: " + _value + " i:" + _inverse + "]";
	}
}
