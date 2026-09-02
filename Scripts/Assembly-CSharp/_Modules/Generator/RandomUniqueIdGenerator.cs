using System;

namespace _Modules.Generator;

public class RandomUniqueIdGenerator
{
	public enum Type
	{
		NUMERIC = 0,
		ALPAHNUMERIC = 1
	}

	private static string _alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

	private static string _small_alphabets = "abcdefghijklmnopqrstuvwxyz";

	private static string _numbers = "1234567890";

	public static string GenerateID(Type t = Type.ALPAHNUMERIC, int idLength = 10)
	{
		string text = _numbers;
		switch (t)
		{
		case Type.NUMERIC:
			text = _numbers;
			break;
		case Type.ALPAHNUMERIC:
			text = text + _alphabets + _small_alphabets + _numbers;
			break;
		}
		string text2 = string.Empty;
		for (int i = 0; i < idLength; i++)
		{
			string empty = string.Empty;
			do
			{
				int num = new Random().Next(0, text.Length);
				empty = text.ToCharArray()[num].ToString();
			}
			while (text2.IndexOf(empty, StringComparison.Ordinal) != -1);
			text2 += empty;
		}
		return text2;
	}
}
