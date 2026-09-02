using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public abstract class OldCellRecord
{
	private short sid;

	private bool isBiff2;

	private int field_1_row;

	private short field_2_column;

	private int field_3_cell_attrs;

	private short field_3_xf_index;

	public int Row => field_1_row;

	public short Column => field_2_column;

	public short XFIndex => field_3_xf_index;

	public int CellAttrs => field_3_cell_attrs;

	public virtual bool IsBiff2 => isBiff2;

	public virtual short Sid => sid;

	protected abstract string RecordName { get; }

	protected OldCellRecord(RecordInputStream in1, bool isBiff2)
	{
		sid = in1.Sid;
		this.isBiff2 = isBiff2;
		field_1_row = in1.ReadUShort();
		field_2_column = in1.ReadShort();
		if (isBiff2)
		{
			field_3_cell_attrs = in1.ReadUShort() << 8;
			field_3_cell_attrs += in1.ReadUByte();
		}
		else
		{
			field_3_xf_index = in1.ReadShort();
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		string recordName = RecordName;
		stringBuilder.Append("[").Append(recordName).Append("]\n");
		stringBuilder.Append("    .row    = ").Append(HexDump.ShortToHex(Row)).Append("\n");
		stringBuilder.Append("    .col    = ").Append(HexDump.ShortToHex(Column)).Append("\n");
		if (IsBiff2)
		{
			stringBuilder.Append("    .cellattrs = ").Append(HexDump.ShortToHex(CellAttrs)).Append("\n");
		}
		else
		{
			stringBuilder.Append("    .xFindex   = ").Append(HexDump.ShortToHex(XFIndex)).Append("\n");
		}
		AppendValueText(stringBuilder);
		stringBuilder.Append("\n");
		stringBuilder.Append("[/").Append(recordName).Append("]\n");
		return stringBuilder.ToString();
	}

	protected abstract void AppendValueText(StringBuilder sb);
}
