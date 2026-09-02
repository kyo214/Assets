using System.Text;
using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

public class TblPtg : ControlPtg
{
	private const int SIZE = 5;

	public const byte sid = 2;

	private int field_1_first_row;

	private int field_2_first_col;

	public override int Size => 5;

	public int Row => field_1_first_row;

	public int Column => field_2_first_col;

	public TblPtg(ILittleEndianInput in1)
	{
		field_1_first_row = in1.ReadUShort();
		field_2_first_col = in1.ReadUShort();
	}

	public override void Write(ILittleEndianOutput out1)
	{
		out1.WriteByte(2 + base.PtgClass);
		out1.WriteShort(field_1_first_row);
		out1.WriteShort(field_2_first_col);
	}

	public override string ToFormulaString()
	{
		throw new RecordFormatException("Table and Arrays are not yet supported");
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder("[Data Table - Parent cell is an interior cell in a data table]\n");
		stringBuilder.Append("top left row = ").Append(Row).Append("\n");
		stringBuilder.Append("top left col = ").Append(Column).Append("\n");
		return stringBuilder.ToString();
	}
}
