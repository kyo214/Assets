namespace Fusion;

internal static class HashCodeUtilities
{
	public const int InitialHash = 352654597;

	public static int GetHashDeterministic(this string str, int initialHash = 352654597)
	{
		int num = initialHash;
		int num2 = initialHash;
		for (int i = 0; i < str.Length; i += 2)
		{
			num = ((num << 5) + num) ^ str[i];
			if (i == str.Length - 1)
			{
				break;
			}
			num2 = ((num2 << 5) + num2) ^ str[i + 1];
		}
		return num + num2 * 1566083941;
	}

	public static int CombineHashCodes(int a, int b)
	{
		return ((a << 5) + a) ^ b;
	}

	public static int CombineHashCodes(int a, int b, int c)
	{
		int num = ((a << 5) + a) ^ b;
		return ((num << 5) + num) ^ c;
	}

	public unsafe static int GetArrayHashCode<T>(T* ptr, int length, int initialHash = 352654597) where T : unmanaged
	{
		int num = initialHash;
		for (int i = 0; i < length; i++)
		{
			num = num * 31 + ptr[i].GetHashCode();
		}
		return num;
	}

	public static int GetHashCodeDeterministic(byte[] data, int initialHash = 0)
	{
		int num = initialHash;
		for (int i = 0; i < data.Length; i++)
		{
			num = num * 31 + data[i];
		}
		return num;
	}
}
