using System;
using System.Collections.Generic;
using System.Text;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class BoundSheetRecord : StandardRecord
{
	private class BOFComparator : IComparer<BoundSheetRecord>
	{
		public int Compare(BoundSheetRecord bsr1, BoundSheetRecord bsr2)
		{
			return bsr1.PositionOfBof - bsr2.PositionOfBof;
		}
	}

	public const short sid = 133;

	private static BitField hiddenFlag = BitFieldFactory.GetInstance(1);

	private static BitField veryHiddenFlag = BitFieldFactory.GetInstance(2);

	private int field_1_position_of_BOF;

	private int field_2_option_flags;

	private int field_4_isMultibyteUnicode;

	private string field_5_sheetname;

	public int PositionOfBof
	{
		get
		{
			return field_1_position_of_BOF;
		}
		set
		{
			field_1_position_of_BOF = value;
		}
	}

	public bool IsVeryHidden
	{
		get
		{
			return veryHiddenFlag.IsSet(field_2_option_flags);
		}
		set
		{
			field_2_option_flags = veryHiddenFlag.SetBoolean(field_2_option_flags, value);
		}
	}

	public string Sheetname
	{
		get
		{
			return field_5_sheetname;
		}
		set
		{
			WorkbookUtil.ValidateSheetName(value);
			field_5_sheetname = value;
			field_4_isMultibyteUnicode = (StringUtil.HasMultibyte(value) ? 1 : 0);
		}
	}

	private bool IsMultibyte => (field_4_isMultibyteUnicode & 1) != 0;

	protected override int DataSize => 8 + field_5_sheetname.Length * ((!IsMultibyte) ? 1 : 2);

	public override short Sid => 133;

	public bool IsHidden
	{
		get
		{
			return hiddenFlag.IsSet(field_2_option_flags);
		}
		set
		{
			field_2_option_flags = hiddenFlag.SetBoolean(field_2_option_flags, value);
		}
	}

	public BoundSheetRecord(string sheetname)
	{
		field_2_option_flags = 0;
		Sheetname = sheetname;
	}

	public BoundSheetRecord(RecordInputStream in1)
	{
		field_1_position_of_BOF = in1.ReadInt();
		field_2_option_flags = in1.ReadShort();
		int requestedLength = in1.ReadUByte();
		field_4_isMultibyteUnicode = (byte)in1.ReadByte();
		if (IsMultibyte)
		{
			field_5_sheetname = in1.ReadUnicodeLEString(requestedLength);
		}
		else
		{
			field_5_sheetname = in1.ReadCompressedUnicode(requestedLength);
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[BOUNDSHEET]\n");
		stringBuilder.Append("    .bof        = ").Append(HexDump.IntToHex(PositionOfBof)).Append("\n");
		stringBuilder.Append("    .options    = ").Append(HexDump.ShortToHex(field_2_option_flags)).Append("\n");
		stringBuilder.Append("    .unicodeflag= ").Append(HexDump.ByteToHex(field_4_isMultibyteUnicode)).Append("\n");
		stringBuilder.Append("    .sheetname  = ").Append(field_5_sheetname).Append("\n");
		stringBuilder.Append("[/BOUNDSHEET]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteInt(PositionOfBof);
		out1.WriteShort(field_2_option_flags);
		string text = field_5_sheetname;
		out1.WriteByte(text.Length);
		out1.WriteByte(field_4_isMultibyteUnicode);
		if (IsMultibyte)
		{
			StringUtil.PutUnicodeLE(text, out1);
		}
		else
		{
			StringUtil.PutCompressedUnicode(text, out1);
		}
	}

	public static BoundSheetRecord[] OrderByBofPosition(List<BoundSheetRecord> boundSheetRecords)
	{
		BoundSheetRecord[] array = boundSheetRecords.ToArray();
		Array.Sort(array, new BOFComparator());
		return array;
	}
}
