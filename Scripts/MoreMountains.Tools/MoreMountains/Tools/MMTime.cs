using System;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMTime : MonoBehaviour
{
	public static string FloatToTimeString(float t, bool displayHours = false, bool displayMinutes = true, bool displaySeconds = true, bool displayMilliseconds = false)
	{
		int num = (int)t;
		int num2 = num / 3600;
		int num3 = num / 60;
		int num4 = num % 60;
		int num5 = Mathf.FloorToInt(t * 1000f % 1000f);
		if (displayHours & displayMinutes & displaySeconds & displayMilliseconds)
		{
			return $"{num2:00}:{num3:00}:{num4:00}.{num5:D3}";
		}
		if (!displayHours & displayMinutes & displaySeconds & displayMilliseconds)
		{
			return $"{num3:00}:{num4:00}.{num5:D3}";
		}
		if ((!displayHours && !displayMinutes) & displaySeconds & displayMilliseconds)
		{
			return $"{num4:D2}.{num5:D3}";
		}
		if (((!displayHours && !displayMinutes) & displaySeconds) && !displayMilliseconds)
		{
			return $"{num4:00}";
		}
		if ((displayHours & displayMinutes & displaySeconds) && !displayMilliseconds)
		{
			return $"{num2:00}:{num3:00}:{num4:00}";
		}
		if ((!displayHours & displayMinutes & displaySeconds) && !displayMilliseconds)
		{
			return $"{num3:00}:{num4:00}";
		}
		return null;
	}

	public static float TimeStringToFloat(string timeInStringNotation)
	{
		if (timeInStringNotation.Length != 12)
		{
			throw new Exception("The time in the TimeStringToFloat method must be specified using a hh:mm:ss:SSS syntax");
		}
		string[] array = timeInStringNotation.Split(new string[1] { ":" }, StringSplitOptions.None);
		float num = 0f;
		if (float.TryParse(array[0], out var result))
		{
			num += result * 3600f;
		}
		if (float.TryParse(array[1], out result))
		{
			num += result * 60f;
		}
		if (float.TryParse(array[2], out result))
		{
			num += result;
		}
		if (float.TryParse(array[3], out result))
		{
			num += result / 1000f;
		}
		return num;
	}
}
