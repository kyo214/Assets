using System;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMSignal
{
	public enum SignalType
	{
		Sine = 0,
		Pulse = 1,
		Sawtooth = 2,
		Square = 3,
		Triangle = 4,
		DigitalNoise = 5,
		WhiteNoise = 6,
		PerlinNoise = 7,
		ValueNoise = 8,
		AnimationCurve = 9,
		MMTween = 10
	}

	private static int[] hash = new int[256]
	{
		151, 160, 137, 91, 90, 15, 131, 13, 201, 95,
		96, 53, 194, 233, 7, 225, 140, 36, 103, 30,
		69, 142, 8, 99, 37, 240, 21, 10, 23, 190,
		6, 148, 247, 120, 234, 75, 0, 26, 197, 62,
		94, 252, 219, 203, 117, 35, 11, 32, 57, 177,
		33, 88, 237, 149, 56, 87, 174, 20, 125, 136,
		171, 168, 68, 175, 74, 165, 71, 134, 139, 48,
		27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
		60, 211, 133, 230, 220, 105, 92, 41, 55, 46,
		245, 40, 244, 102, 143, 54, 65, 25, 63, 161,
		1, 216, 80, 73, 209, 76, 132, 187, 208, 89,
		18, 169, 200, 196, 135, 130, 116, 188, 159, 86,
		164, 100, 109, 198, 173, 186, 3, 64, 52, 217,
		226, 250, 124, 123, 5, 202, 38, 147, 118, 126,
		255, 82, 85, 212, 207, 206, 59, 227, 47, 16,
		58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
		119, 248, 152, 2, 44, 154, 163, 70, 221, 153,
		101, 155, 167, 43, 172, 9, 129, 22, 39, 253,
		19, 98, 108, 110, 79, 113, 224, 232, 178, 185,
		112, 104, 218, 246, 97, 228, 251, 34, 242, 193,
		238, 210, 144, 12, 191, 179, 162, 241, 81, 51,
		145, 235, 249, 14, 239, 107, 49, 192, 214, 31,
		181, 199, 106, 157, 184, 84, 204, 176, 115, 121,
		50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
		222, 114, 67, 29, 24, 72, 243, 141, 128, 195,
		78, 66, 215, 61, 156, 180
	};

	private const int hashMask = 255;

	public static float GetValue(float time, SignalType signalType, float phase, float amplitude, float frequency, float offset, bool Invert = false, AnimationCurve curve = null, MMTween.MMTweenCurve tweenCurve = MMTween.MMTweenCurve.LinearTween)
	{
		float num = 0f;
		float num2 = ((!Invert) ? 1 : (-1));
		float num3 = frequency * time + phase;
		switch (signalType)
		{
		case SignalType.Sine:
			num = Mathf.Sin(MathF.PI * 2f * num3);
			break;
		case SignalType.Square:
			num = Mathf.Sign(Mathf.Sin(MathF.PI * 2f * num3));
			break;
		case SignalType.Triangle:
			num = 1f - 4f * Mathf.Abs(Mathf.Round(num3 - 0.25f) - (num3 - 0.25f));
			break;
		case SignalType.Sawtooth:
			num = 2f * (num3 - Mathf.Floor(num3 + 0.5f));
			break;
		case SignalType.Pulse:
			num = ((!((double)Mathf.Abs(Mathf.Sin(MathF.PI * 2f * num3)) < 0.99)) ? 1 : 0);
			break;
		case SignalType.WhiteNoise:
			num = 2f * (float)UnityEngine.Random.Range(0, int.MaxValue) / 2.1474836E+09f - 1f;
			break;
		case SignalType.DigitalNoise:
			num = UnityEngine.Random.Range(0, 2);
			break;
		case SignalType.PerlinNoise:
			num = Mathf.PerlinNoise(time * frequency, time * amplitude);
			break;
		case SignalType.ValueNoise:
			num = ValueNoise(time, frequency) * amplitude;
			break;
		case SignalType.AnimationCurve:
			if (curve == null)
			{
				return 0f;
			}
			num3 = ((num3 != 1f) ? (num3 - Mathf.Floor(num3)) : 1f);
			num = curve.Evaluate(num3);
			break;
		case SignalType.MMTween:
			num3 = ((num3 != 1f) ? (num3 - Mathf.Floor(num3)) : 1f);
			num = MMTween.Tween(num3, 0f, 1f, 0f, 1f, tweenCurve);
			break;
		}
		return num2 * amplitude * num + offset;
	}

	public static float GetValueNormalized(float time, SignalType signalType, float phase, float amplitude, float frequency, float offset, bool Invert = false, AnimationCurve curve = null, MMTween.MMTweenCurve tweenCurve = MMTween.MMTweenCurve.LinearTween, bool clamp = true, float clampMin = 0f, float clampMax = 1f, bool backAndForth = false, float backAndForthTippingPoint = 0.5f)
	{
		float num = 0f;
		if (backAndForth)
		{
			if (time < backAndForthTippingPoint)
			{
				time = MMMaths.Remap(time, 0f, backAndForthTippingPoint, 0f, 1f);
			}
			else if (time == backAndForthTippingPoint)
			{
				time = 1f;
			}
			else if (time > backAndForthTippingPoint)
			{
				time = MMMaths.Remap(time, backAndForthTippingPoint, 1f, 1f, 0f);
			}
		}
		float num2 = frequency * time + phase;
		switch (signalType)
		{
		case SignalType.Sine:
			num = Mathf.Sin(MathF.PI * 2f * num2);
			num = MMMaths.Remap(num, -1f, 1f, 0f, 1f);
			break;
		case SignalType.Square:
			num = Mathf.Sign(Mathf.Sin(MathF.PI * 2f * num2));
			num = MMMaths.Remap(num, -1f, 1f, 0f, 1f);
			break;
		case SignalType.Triangle:
			num = 1f - 4f * Mathf.Abs(Mathf.Round(num2 - 0.25f) - (num2 - 0.25f));
			num = MMMaths.Remap(num, -1f, 1f, 0f, 1f);
			break;
		case SignalType.Sawtooth:
			num = 2f * (num2 - Mathf.Floor(num2 + 0.5f));
			num = MMMaths.Remap(num, -1f, 1f, 0f, 1f);
			break;
		case SignalType.Pulse:
			num = ((!((double)Mathf.Abs(Mathf.Sin(MathF.PI * 2f * num2)) < 0.99)) ? 1 : 0);
			break;
		case SignalType.WhiteNoise:
			num = 2f * (float)UnityEngine.Random.Range(0, int.MaxValue) / 2.1474836E+09f - 1f;
			num = MMMaths.Remap(num, -1f, 1f, 0f, 1f);
			break;
		case SignalType.DigitalNoise:
			num = UnityEngine.Random.Range(0, 2);
			break;
		case SignalType.PerlinNoise:
			num = Mathf.PerlinNoise(num2, num2 * amplitude);
			break;
		case SignalType.ValueNoise:
			num = ValueNoise(time, frequency) * amplitude;
			break;
		case SignalType.AnimationCurve:
			if (curve == null)
			{
				return 0f;
			}
			num2 = ((num2 != 1f) ? (num2 - Mathf.Floor(num2)) : 1f);
			num = curve.Evaluate(num2);
			break;
		case SignalType.MMTween:
			num2 = ((num2 != 1f) ? (num2 - Mathf.Floor(num2)) : 1f);
			num = MMTween.Tween(num2, 0f, 1f, 0f, 1f, tweenCurve);
			break;
		}
		if (Invert)
		{
			num = MMMaths.Remap(num, 0f, 1f, 1f, 0f);
		}
		float num3 = amplitude * num + offset;
		if (clamp)
		{
			num3 = Mathf.Clamp(num3, clampMin, clampMax);
		}
		return num3;
	}

	protected static float ValueNoise(float time, float frequency)
	{
		time *= frequency;
		int num = Mathf.FloorToInt(time);
		num &= 0xFF;
		return (float)hash[num] * (1f / 255f);
	}
}
