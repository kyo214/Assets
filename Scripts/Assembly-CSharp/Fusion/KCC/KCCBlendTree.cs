using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCBlendTree
{
	private int _count;

	private float _scale;

	private float[] _weights;

	private Vector2[] _basePositions;

	private float[] _baseMagnitudes;

	private Vector2[] _scaledPositions;

	private float[] _scaledMagnitudes;

	private Vector2[][] _scaledPolarDistances;

	private float[][] _inverseAverageMagnitudes;

	public float[] Weights => _weights;

	public KCCBlendTree(Vector2[] positions)
	{
		_count = positions.Length;
		_weights = new float[_count];
		_basePositions = new Vector2[_count];
		_baseMagnitudes = new float[_count];
		_scaledPositions = new Vector2[_count];
		_scaledMagnitudes = new float[_count];
		_scaledPolarDistances = new Vector2[_count][];
		_inverseAverageMagnitudes = new float[_count][];
		for (int i = 0; i < _count; i++)
		{
			_scaledPolarDistances[i] = new Vector2[_count];
			_inverseAverageMagnitudes[i] = new float[_count];
		}
		for (int j = 0; j < _count; j++)
		{
			_basePositions[j] = positions[j];
			_baseMagnitudes[j] = positions[j].magnitude;
		}
		_scale = 1f;
		PrecalculateWeights();
	}

	public void SetPositions(Vector2[] positions)
	{
		_count = positions.Length;
		_weights = new float[_count];
		_basePositions = new Vector2[_count];
		_baseMagnitudes = new float[_count];
		_scaledPositions = new Vector2[_count];
		_scaledMagnitudes = new float[_count];
		_scaledPolarDistances = new Vector2[_count][];
		_inverseAverageMagnitudes = new float[_count][];
		for (int i = 0; i < _count; i++)
		{
			_scaledPolarDistances[i] = new Vector2[_count];
			_inverseAverageMagnitudes[i] = new float[_count];
		}
		for (int j = 0; j < _count; j++)
		{
			_basePositions[j] = positions[j];
			_baseMagnitudes[j] = positions[j].magnitude;
		}
		PrecalculateWeights();
	}

	public void SetScale(float scale)
	{
		_scale = scale;
		PrecalculateWeights();
	}

	public void CalculateWeights(Vector2 position)
	{
		float magnitude = position.magnitude;
		float num = 0f;
		for (int i = 0; i < _count; i++)
		{
			float num2 = 1f;
			float angleFast = GetAngleFast(_scaledPositions[i], position);
			float num3 = magnitude - _scaledMagnitudes[i];
			Vector2[] array = _scaledPolarDistances[i];
			float[] array2 = _inverseAverageMagnitudes[i];
			for (int j = 0; j < _count; j++)
			{
				if (i != j)
				{
					Vector2 vector = array[j];
					Vector2 vector2 = new Vector2(num3 * array2[j], angleFast);
					float num4 = 1f - vector.x * vector2.x - vector.y * vector2.y;
					if (num4 < num2)
					{
						num2 = num4;
					}
				}
			}
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			_weights[i] = num2;
			num += num2;
		}
		if (num > 0f)
		{
			float num5 = 1f / num;
			for (int k = 0; k < _count; k++)
			{
				_weights[k] *= num5;
			}
		}
	}

	private void PrecalculateWeights()
	{
		for (int i = 0; i < _count; i++)
		{
			_scaledPositions[i] = _basePositions[i] * _scale;
			_scaledMagnitudes[i] = _baseMagnitudes[i] * _scale;
		}
		for (int j = 0; j < _count; j++)
		{
			Vector2 a = _scaledPositions[j];
			float num = _scaledMagnitudes[j];
			Vector2[] array = _scaledPolarDistances[j];
			float[] array2 = _inverseAverageMagnitudes[j];
			for (int k = 0; k < _count; k++)
			{
				Vector2 b = _scaledPositions[k];
				float num2 = _scaledMagnitudes[k];
				Vector2[] obj = _scaledPolarDistances[k];
				float[] array3 = _inverseAverageMagnitudes[k];
				float num3 = (num + num2) * 0.5f;
				float num4 = 1f / num3;
				float angle = GetAngle(a, b);
				float num5 = num2 - num;
				Vector2 vector = new Vector2(num5 * num4, angle);
				vector /= vector.sqrMagnitude;
				array[k] = vector;
				obj[j] = -vector;
				array2[k] = num4;
				array3[j] = num4;
			}
		}
	}

	private static float GetAngle(Vector2 a, Vector2 b)
	{
		if ((a.x == 0f && a.y == 0f) || (b.x == 0f && b.y == 0f))
		{
			return 0f;
		}
		float x = a.x * b.x + a.y * b.y;
		return Mathf.Atan2(a.x * b.y - a.y * b.x, x);
	}

	private static float GetAngleFast(Vector2 a, Vector2 b)
	{
		if ((a.x == 0f && a.y == 0f) || (b.x == 0f && b.y == 0f))
		{
			return 0f;
		}
		float x = a.x * b.x + a.y * b.y;
		return KCCMathUtility.FastAtan2(a.x * b.y - a.y * b.x, x);
	}
}
