using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class MulBlankRecord : StandardRecord, ICloneable
{
	public const short sid = 190;

	private int _row;

	private int _first_col;

	private short[] _xfs;

	private int _last_col;

	public int Row => _row;

	public int FirstColumn => _first_col;

	public int LastColumn => _last_col;

	public int NumColumns => _last_col - _first_col + 1;

	public override short Sid => 190;

	protected override int DataSize => 6 + _xfs.Length * 2;

	public MulBlankRecord()
	{
	}

	public MulBlankRecord(int row, int firstCol, short[] xfs)
	{
		_row = row;
		_first_col = firstCol;
		_xfs = xfs;
		_last_col = firstCol + xfs.Length - 1;
	}

	public MulBlankRecord(RecordInputStream in1)
	{
		_row = in1.ReadUShort();
		_first_col = in1.ReadShort();
		_xfs = ParseXFs(in1);
		_last_col = in1.ReadShort();
	}

	public short GetXFAt(int coffset)
	{
		return _xfs[coffset];
	}

	private short[] ParseXFs(RecordInputStream in1)
	{
		short[] array = new short[(in1.Remaining - 2) / 2];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = in1.ReadShort();
		}
		return array;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[MULBLANK]\n");
		stringBuilder.Append("row  = ").Append(StringUtil.ToHexString(Row)).Append("\n");
		stringBuilder.Append("firstcol  = ").Append(StringUtil.ToHexString(FirstColumn)).Append("\n");
		stringBuilder.Append(" lastcol  = ").Append(StringUtil.ToHexString(LastColumn)).Append("\n");
		for (int i = 0; i < NumColumns; i++)
		{
			stringBuilder.Append("xf").Append(i).Append("        = ")
				.Append(StringUtil.ToHexString(GetXFAt(i)))
				.Append("\n");
		}
		stringBuilder.Append("[/MULBLANK]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(_row);
		out1.WriteShort(_first_col);
		int num = _xfs.Length;
		for (int i = 0; i < num; i++)
		{
			out1.WriteShort(_xfs[i]);
		}
		out1.WriteShort(_last_col);
	}

	public override object Clone()
	{
		return this;
	}
}
