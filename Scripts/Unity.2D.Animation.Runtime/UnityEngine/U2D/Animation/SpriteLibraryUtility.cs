using System;

namespace UnityEngine.U2D.Animation;

internal static class SpriteLibraryUtility
{
	internal static Func<string, int> GetStringHash = Bit30Hash_GetStringHash;

	internal static int Convert32BitTo30BitHash(int input)
	{
		return PreserveFirst30Bits(input);
	}

	private static int Bit30Hash_GetStringHash(string value)
	{
		return PreserveFirst30Bits(Animator.StringToHash(value));
	}

	private static int PreserveFirst30Bits(int input)
	{
		return input & 0x3FFFFFFF;
	}

	internal static long GenerateHash()
	{
		return DateTime.Now.Ticks;
	}
}
