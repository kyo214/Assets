using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class DBCellRecord : StandardRecord, ICloneable
{
	public const int BLOCK_SIZE = 32;

	public const short sid = 215;

	private int field_1_row_offset;

	private short[] field_2_cell_offsets;

	public int RowOffset
	{
		get
		{
			return field_1_row_offset;
		}
		set
		{
			field_1_row_offset = value;
		}
	}

	public int NumCellOffsets => field_2_cell_offsets.Length;

	protected override int DataSize => 4 + field_2_cell_offsets.Length * 2;

	public override short Sid => 215;

	public DBCellRecord()
	{
		field_2_cell_offsets = new short[0];
	}

	public DBCellRecord(RecordInputStream in1)
	{
		field_1_row_offset = in1.ReadUShort();
		int remaining = in1.Remaining;
		field_2_cell_offsets = new short[remaining / 2];
		for (int i = 0; i < field_2_cell_offsets.Length; i++)
		{
			field_2_cell_offsets[i] = in1.ReadShort();
		}
	}

	public DBCellRecord(int rowOffset, short[] cellOffsets)
	{
		field_1_row_offset = rowOffset;
		field_2_cell_offsets = cellOffsets;
	}

	public void AddCellOffset(short offset)
	{
		if (field_2_cell_offsets == null)
		{
			field_2_cell_offsets = new short[1];
		}
		else
		{
			short[] destinationArray = new short[field_2_cell_offsets.Length + 1];
			Array.Copy(field_2_cell_offsets, 0, destinationArray, 0, field_2_cell_offsets.Length);
			field_2_cell_offsets = destinationArray;
		}
		field_2_cell_offsets[field_2_cell_offsets.Length - 1] = offset;
	}

	public short GetCellOffsetAt(int index)
	{
		return field_2_cell_offsets[index];
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[DBCELL]\n");
		stringBuilder.Append("    .rowoffset       = ").Append(StringUtil.ToHexString(RowOffset)).Append("\n");
		for (int i = 0; i < field_2_cell_offsets.Length; i++)
		{
			stringBuilder.Append("    .cell_").Append(i).Append(" = ")
				.Append(HexDump.ShortToHex(field_2_cell_offsets[i]))
				.Append("\n");
		}
		stringBuilder.Append("[/DBCELL]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteInt(field_1_row_offset);
		for (int i = 0; i < field_2_cell_offsets.Length; i++)
		{
			out1.WriteShort(field_2_cell_offsets[i]);
		}
	}

	public static int CalculateSizeOfRecords(int nBlocks, int nRows)
	{
		return nBlocks * 8 + nRows * 2;
	}

	public override object Clone()
	{
		return this;
	}
}
