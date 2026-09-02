using System;
using System.Text;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.HSSF.Record.Common;

public class FtrHeader : ICloneable
{
	private short recordType;

	private short grbitFrt;

	private CellRangeAddress associatedRange;

	public short RecordType
	{
		get
		{
			return recordType;
		}
		set
		{
			recordType = value;
		}
	}

	public short GrbitFrt
	{
		get
		{
			return grbitFrt;
		}
		set
		{
			grbitFrt = value;
		}
	}

	public CellRangeAddress AssociatedRange
	{
		get
		{
			return associatedRange;
		}
		set
		{
			associatedRange = value;
		}
	}

	public FtrHeader()
	{
		associatedRange = new CellRangeAddress(0, 0, 0, 0);
	}

	public FtrHeader(RecordInputStream in1)
	{
		recordType = in1.ReadShort();
		grbitFrt = in1.ReadShort();
		associatedRange = new CellRangeAddress(in1);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(" [FUTURE HEADER]\n");
		stringBuilder.Append("   type " + recordType);
		stringBuilder.Append("   flags " + grbitFrt);
		stringBuilder.Append(" [/FUTURE HEADER]\n");
		return stringBuilder.ToString();
	}

	public void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(recordType);
		out1.WriteShort(grbitFrt);
		associatedRange.Serialize(out1);
	}

	public static int GetDataSize()
	{
		return 12;
	}

	public object Clone()
	{
		return new FtrHeader
		{
			recordType = recordType,
			grbitFrt = grbitFrt,
			associatedRange = associatedRange.Copy()
		};
	}
}
