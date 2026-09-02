using UnityEngine;

public static class FloatExtensions
{
	public static float RoundDecimalPlaces(this float value, int decimalPlaces)
	{
		float num = Mathf.Pow(10f, decimalPlaces);
		return Mathf.Round(value * num) / num;
	}
}
