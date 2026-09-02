using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class BlankRecord : StandardRecord, CellValueRecordInterface, IComparable, ICloneable
{
	public const short sid = 513;

	private int field_1_row;

	private int field_2_col;

	private short field_3_xf;

	public int Row
	{
		get
		{
			return field_1_row;
		}
		set
		{
			field_1_row = value;
		}
	}

	public int Column
	{
		get
		{
			return field_2_col;
		}
		set
		{
			field_2_col = value;
		}
	}

	public short XFIndex
	{
		get
		{
			return field_3_xf;
		}
		set
		{
			field_3_xf = value;
		}
	}

	public override short Sid => 513;

	protected override int DataSize => 6;

	public BlankRecord()
	{
	}

	public BlankRecord(RecordInputStream in1)
	{
		field_1_row = in1.ReadUShort();
		field_2_col = in1.ReadShort();
		field_3_xf = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[BLANK]\n");
		stringBuilder.Append("row       = ").Append(HexDump.ShortToHex(Row)).Append("\n");
		stringBuilder.Append("col       = ").Append(HexDump.ShortToHex(Column)).Append("\n");
		stringBuilder.Append("xf        = ").Append(HexDump.ShortToHex(XFIndex)).Append("\n");
		stringBuilder.Append("[/BLANK]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(Row);
		out1.WriteShort(Column);
		out1.WriteShort(XFIndex);
	}

	public int CompareTo(object obj)
	{
		CellValueRecordInterface cellValueRecordInterface = (CellValueRecordInterface)obj;
		if (Row == cellValueRecordInterface.Row && Column == cellValueRecordInterface.Column)
		{
			return 0;
		}
		if (Row < cellValueRecordInterface.Row)
		{
			return -1;
		}
		if (Row > cellValueRecordInterface.Row)
		{
			return 1;
		}
		if (Column < cellValueRecordInterface.Column)
		{
			return -1;
		}
		if (Column > cellValueRecordInterface.Column)
		{
			return 1;
		}
		return -1;
	}

	public override object Clone()
	{
		return new BlankRecord
		{
			field_1_row = field_1_row,
			field_2_col = field_2_col,
			field_3_xf = field_3_xf
		};
	}
}
