namespace BansheeGz.BGDatabase;

public static class BGUtilForTest
{
	public enum TestEnumInt
	{
		first = 0,
		second = -1000000000,
		third = -2000000000,
		forth = int.MinValue,
		fifth = 1000000000,
		sixth = 2000000000,
		seventh = int.MaxValue
	}

	public enum TestEnumShort : short
	{
		first = 0,
		second = -16000,
		third = -32000,
		forth = short.MinValue,
		fifth = 16000,
		sixth = 32000,
		seventh = short.MaxValue
	}

	public enum TestEnumByte : byte
	{
		first = 0,
		second = 1,
		third = 127,
		forth = 200,
		fifth = byte.MaxValue
	}
}
