using System;
using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCResolver
{
	private sealed class Correction
	{
		public Vector3 Amount;

		public Vector3 Direction;

		public float Distance;

		public float Error;
	}

	private int _size;

	private int _iterations;

	private Correction[] _corrections;

	private Vector3 _minCorrection;

	private Vector3 _maxCorrection;

	private Vector3 _targetCorrection;

	public int Size => _size;

	public int Iterations => _iterations;

	public Vector3 TargetCorrection => _targetCorrection;

	public KCCResolver(int maxSize)
	{
		_corrections = new Correction[maxSize];
		for (int i = 0; i < maxSize; i++)
		{
			_corrections[i] = new Correction();
		}
	}

	public void Reset()
	{
		_size = 0;
		_iterations = 0;
		_minCorrection = default;
		_maxCorrection = default;
		_targetCorrection = default;
	}

	public void AddCorrection(Vector3 direction, float distance)
	{
		Correction correction = _corrections[_size];
		correction.Amount = direction * distance;
		correction.Direction = direction;
		correction.Distance = distance;
		_minCorrection = Vector3.Min(_minCorrection, correction.Amount);
		_maxCorrection = Vector3.Max(_maxCorrection, correction.Amount);
		_size++;
	}

	public Vector3 GetCorrection(int index)
	{
		return _corrections[index].Amount;
	}

	public Vector3 GetCorrection(int index, out Vector3 direction)
	{
		Correction correction = _corrections[index];
		direction = correction.Direction;
		return correction.Amount;
	}

	public Vector3 GetCorrection(int index, out Vector3 direction, out float distance)
	{
		Correction correction = _corrections[index];
		direction = correction.Direction;
		distance = correction.Distance;
		return correction.Amount;
	}

	public Vector3 CalculateMinMax()
	{
		_iterations = 0;
		_targetCorrection = _minCorrection + _maxCorrection;
		return _targetCorrection;
	}

	public Vector3 CalculateSum()
	{
		_iterations = 0;
		_targetCorrection = default;
		int i = 0;
		for (int size = _size; i < size; i++)
		{
			_targetCorrection += _corrections[i].Amount;
		}
		return _targetCorrection;
	}

	public Vector3 CalculateBinary()
	{
		if (_size != 2)
		{
			throw new InvalidOperationException("Size must be 2!");
		}
		_iterations = 0;
		_targetCorrection = _minCorrection + _maxCorrection;
		Correction correction = _corrections[0];
		Correction correction2 = _corrections[1];
		float num = Vector3.Dot(correction.Direction, correction2.Direction);
		if (num > 0.999f || num < -0.999f)
		{
			return _targetCorrection;
		}
		Vector3 normalized = Vector3.Cross(Vector3.Cross(correction.Direction, correction2.Direction), correction.Direction).normalized;
		float num2 = (correction2.Distance - correction.Distance * num) / Mathf.Sqrt(1f - num * num);
		_targetCorrection = correction.Amount + normalized * num2;
		return _targetCorrection;
	}

	public Vector3 CalculateGradientDescent(int maxIterations, float maxError)
	{
		_iterations = 0;
		_targetCorrection = _minCorrection + _maxCorrection;
		if (_size <= 1)
		{
			return _targetCorrection;
		}
		Vector3 targetCorrection = _targetCorrection;
		Correction[] corrections = _corrections;
		while (_iterations < maxIterations)
		{
			Vector3 vector = default;
			float num = 0f;
			float num2 = 0f;
			int i = 0;
			for (int size = _size; i < size; i++)
			{
				Correction correction = corrections[i];
				correction.Error = correction.Direction.x * targetCorrection.x + correction.Direction.y * targetCorrection.y + correction.Direction.z * targetCorrection.z - correction.Distance;
				vector += correction.Direction * correction.Error;
			}
			if (vector.IsAlmostZero(maxError))
			{
				break;
			}
			vector.Normalize();
			int j = 0;
			for (int size2 = _size; j < size2; j++)
			{
				Correction correction = corrections[j];
				float num3 = correction.Direction.x * vector.x + correction.Direction.y * vector.y + correction.Direction.z * vector.z;
				num += correction.Error * num3;
				num2 = ((!(num3 >= 0f)) ? (num2 - num3) : (num2 + num3));
			}
			if (num2 < 1E-06f)
			{
				break;
			}
			num /= num2;
			if (num.IsAlmostZero(maxError))
			{
				break;
			}
			targetCorrection -= vector * num;
			_iterations++;
		}
		_targetCorrection = targetCorrection;
		return targetCorrection;
	}
}
