using System;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public abstract class StandardRecord : Record
{
	protected abstract int DataSize { get; }

	public override int RecordSize => 4 + DataSize;

	public override int Serialize(int offset, byte[] data)
	{
		int dataSize = DataSize;
		int num = 4 + dataSize;
		LittleEndianByteArrayOutputStream littleEndianByteArrayOutputStream = new LittleEndianByteArrayOutputStream(data, offset, num);
		littleEndianByteArrayOutputStream.WriteShort(Sid);
		littleEndianByteArrayOutputStream.WriteShort(dataSize);
		Serialize(littleEndianByteArrayOutputStream);
		if (littleEndianByteArrayOutputStream.WriteIndex - offset != num)
		{
			throw new InvalidOperationException("Error in serialization of (" + GetType().Name + "): Incorrect number of bytes written - expected " + num + " but got " + (littleEndianByteArrayOutputStream.WriteIndex - offset));
		}
		return num;
	}

	public abstract void Serialize(ILittleEndianOutput out1);
}
