using System;
using System.IO;

namespace NPOI.HSSF.Record;

[Serializable]
public abstract class Record : RecordBase
{
	public abstract short Sid { get; }

	public Record()
	{
	}

	public byte[] Serialize()
	{
		byte[] array = new byte[RecordSize];
		Serialize(0, array);
		return array;
	}

	public virtual object Clone()
	{
		throw new NotSupportedException("The class " + GetType().Name + " needs to define a Clone method");
	}

	public Record CloneViaReserialise()
	{
		using MemoryStream @in = new MemoryStream(Serialize());
		RecordInputStream recordInputStream = new RecordInputStream(@in);
		recordInputStream.NextRecord();
		Record[] array = RecordFactory.CreateRecord(recordInputStream);
		if (array.Length != 1)
		{
			throw new InvalidOperationException("Re-serialised a record to clone it, but got " + array.Length + " records back!");
		}
		return array[0];
	}
}
