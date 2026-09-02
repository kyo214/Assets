using System;
using System.IO;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public abstract class SubRecord : ICloneable
{
	public abstract short Sid { get; }

	public abstract int DataSize { get; }

	public virtual bool IsTerminating => false;

	public static SubRecord CreateSubRecord(ILittleEndianInput in1, CommonObjectType cmoOt)
	{
		int num = in1.ReadUShort();
		int num2 = in1.ReadUShort();
		return num switch
		{
			21 => new CommonObjectDataSubRecord(in1, num2), 
			9 => new EmbeddedObjectRefSubRecord(in1, num2), 
			6 => new GroupMarkerSubRecord(in1, num2), 
			0 => new EndSubRecord(in1, num2), 
			13 => new NoteStructureSubRecord(in1, num2), 
			19 => new LbsDataSubRecord(in1, num2, (int)cmoOt), 
			12 => new FtCblsSubRecord(in1, num2), 
			8 => new FtPioGrbitSubRecord(in1, num2), 
			7 => new FtCfSubRecord(in1, num2), 
			_ => new UnknownSubRecord(in1, num, num2), 
		};
	}

	public abstract void Serialize(ILittleEndianOutput out1);

	public byte[] Serialize()
	{
		int num = DataSize + 4;
		using MemoryStream memoryStream = new MemoryStream(num);
		Serialize(new LittleEndianOutputStream(memoryStream));
		if (memoryStream.Length != num)
		{
			throw new Exception("write size mismatch");
		}
		return memoryStream.ToArray();
	}

	public abstract object Clone();
}
