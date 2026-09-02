namespace BansheeGz.BGDatabase;

public class BGBinaryBulkRequestStruct
{
	public readonly byte[] Array;

	public readonly int Offset;

	public readonly int EntitiesCount;

	public BGBinaryBulkRequestStruct(byte[] array, int offset, int entitiesCount)
	{
		Array = array;
		Offset = offset;
		EntitiesCount = entitiesCount;
	}

	public override string ToString()
	{
		int offset = Offset;
		string text = offset.ToString();
		offset = EntitiesCount;
		return text + ": " + offset;
	}
}
