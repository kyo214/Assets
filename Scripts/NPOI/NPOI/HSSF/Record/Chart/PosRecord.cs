using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class PosRecord : StandardRecord
{
	public const short sid = 4175;

	private short mdTopLt;

	private short mdBotRt;

	private short x1;

	private short y1;

	private short x2;

	private short y2;

	protected override int DataSize => 20;

	public override short Sid => 4175;

	public PositionMode MDTopLt
	{
		get
		{
			return (PositionMode)mdTopLt;
		}
		set
		{
			mdTopLt = (short)value;
		}
	}

	public PositionMode MdBotRt
	{
		get
		{
			return (PositionMode)mdBotRt;
		}
		set
		{
			mdBotRt = (short)value;
		}
	}

	public short X1
	{
		get
		{
			return x1;
		}
		set
		{
			x1 = value;
		}
	}

	public short X2
	{
		get
		{
			return x2;
		}
		set
		{
			x2 = value;
		}
	}

	public short Y1
	{
		get
		{
			return y1;
		}
		set
		{
			y1 = value;
		}
	}

	public short Y2
	{
		get
		{
			return y2;
		}
		set
		{
			y2 = value;
		}
	}

	public PosRecord()
	{
	}

	public PosRecord(RecordInputStream in1)
	{
		mdTopLt = in1.ReadShort();
		mdBotRt = in1.ReadShort();
		x1 = in1.ReadShort();
		in1.ReadShort();
		y1 = in1.ReadShort();
		in1.ReadShort();
		x2 = in1.ReadShort();
		in1.ReadShort();
		y2 = in1.ReadShort();
		in1.ReadShort();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(mdTopLt);
		out1.WriteShort(mdBotRt);
		out1.WriteShort(x1);
		out1.WriteShort(0);
		out1.WriteShort(y1);
		out1.WriteShort(0);
		out1.WriteShort(x2);
		out1.WriteShort(0);
		out1.WriteShort(y2);
		out1.WriteShort(0);
	}

	public override object Clone()
	{
		return new PosRecord
		{
			MdBotRt = MdBotRt,
			MDTopLt = MDTopLt,
			X1 = X1,
			X2 = X2,
			Y1 = Y1,
			Y2 = Y2
		};
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[POS]\n");
		stringBuilder.Append("mdTopLt       = ").Append(HexDump.ShortToHex(mdTopLt)).Append("\n");
		stringBuilder.Append("mdBotRt       = ").Append(HexDump.ShortToHex(mdTopLt)).Append("\n");
		stringBuilder.Append("x1            = ").Append(HexDump.ShortToHex(x1)).Append("\n");
		stringBuilder.Append("x2            = ").Append(HexDump.ShortToHex(x2)).Append("\n");
		stringBuilder.Append("y1            = ").Append(HexDump.ShortToHex(y1)).Append("\n");
		stringBuilder.Append("y2            = ").Append(HexDump.ShortToHex(y2)).Append("\n");
		stringBuilder.Append("[/POS]\n");
		return stringBuilder.ToString();
	}
}
