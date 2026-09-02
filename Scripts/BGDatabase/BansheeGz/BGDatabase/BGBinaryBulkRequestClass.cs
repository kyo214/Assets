using System;

namespace BansheeGz.BGDatabase;

public class BGBinaryBulkRequestClass
{
	public class CellRequest
	{
		public readonly int EntityIndex;

		public readonly int Offset;

		public readonly int Count;

		public CellRequest(int entityIndex, int offset, int count)
		{
			EntityIndex = entityIndex;
			Offset = offset;
			Count = count;
		}

		public override string ToString()
		{
			int offset = Offset;
			string text = offset.ToString();
			offset = Count;
			return text + ": " + offset;
		}
	}

	public readonly byte[] Array;

	public readonly CellRequest[] CellRequests;

	public readonly Action<Exception> OnError;

	public BGBinaryBulkRequestClass(byte[] array, CellRequest[] cellRequests, Action<Exception> onError)
	{
		Array = array;
		CellRequests = cellRequests;
		OnError = onError;
	}
}
