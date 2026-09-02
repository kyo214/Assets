using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class StyleRecord : StandardRecord
{
	public const short sid = 659;

	private static BitField styleIndexMask = BitFieldFactory.GetInstance(4095);

	private static BitField isBuiltinFlag = BitFieldFactory.GetInstance(32768);

	private int field_1_xf_index;

	private int field_2_builtin_style;

	private int field_3_outline_style_level;

	private bool field_3_stringHasMultibyte;

	private string field_4_name;

	public bool IsBuiltin => isBuiltinFlag.IsSet(field_1_xf_index);

	public short XFIndex
	{
		get
		{
			return (short)(field_1_xf_index & 0x1FFF);
		}
		set
		{
			field_1_xf_index = SetField(field_1_xf_index, value, 8191, 0);
		}
	}

	public string Name
	{
		get
		{
			return field_4_name;
		}
		set
		{
			field_4_name = value;
			field_3_stringHasMultibyte = StringUtil.HasMultibyte(value);
			field_1_xf_index = isBuiltinFlag.Clear(field_1_xf_index);
		}
	}

	public int OutlineStyleLevel
	{
		get
		{
			return field_3_outline_style_level;
		}
		set
		{
			field_3_outline_style_level = value & 0xFF;
		}
	}

	protected override int DataSize
	{
		get
		{
			if (IsBuiltin)
			{
				return 4;
			}
			return 5 + field_4_name.Length * ((!field_3_stringHasMultibyte) ? 1 : 2);
		}
	}

	public override short Sid => 659;

	public StyleRecord()
	{
		field_1_xf_index = isBuiltinFlag.Set(0);
	}

	public StyleRecord(RecordInputStream in1)
	{
		field_1_xf_index = in1.ReadShort();
		if (IsBuiltin)
		{
			field_2_builtin_style = in1.ReadByte();
			field_3_outline_style_level = in1.ReadByte();
			return;
		}
		int num = in1.ReadShort();
		if (in1.Remaining < 1)
		{
			if (num != 0)
			{
				throw new RecordFormatException("Ran out of data reading style record");
			}
			field_4_name = "";
		}
		else
		{
			field_3_stringHasMultibyte = in1.ReadByte() != 0;
			if (field_3_stringHasMultibyte)
			{
				field_4_name = StringUtil.ReadUnicodeLE(in1, num);
			}
			else
			{
				field_4_name = StringUtil.ReadCompressedUnicode(in1, num);
			}
		}
	}

	public void SetBuiltinStyle(int builtinStyleId)
	{
		field_1_xf_index = isBuiltinFlag.Set(field_1_xf_index);
		field_2_builtin_style = builtinStyleId;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[STYLE]\n");
		stringBuilder.Append("    .xf_index_raw    = ").Append(HexDump.ShortToHex(field_1_xf_index)).Append("\n");
		stringBuilder.Append("        .type        = ").Append(IsBuiltin ? "built-in" : "user-defined").Append("\n");
		stringBuilder.Append("        .xf_index    = ").Append(HexDump.ShortToHex(XFIndex)).Append("\n");
		if (IsBuiltin)
		{
			stringBuilder.Append("    .builtin_style   = ").Append(HexDump.ByteToHex(field_2_builtin_style)).Append("\n");
			stringBuilder.Append("    .outline_level   = ").Append(HexDump.ByteToHex(field_3_outline_style_level)).Append("\n");
		}
		else
		{
			stringBuilder.Append("    .name            = ").Append(Name).Append("\n");
		}
		stringBuilder.Append("[/STYLE]\n");
		return stringBuilder.ToString();
	}

	private short SetField(int fieldValue, int new_value, int mask, int ShiftLeft)
	{
		return (short)((fieldValue & ~mask) | ((new_value << ShiftLeft) & mask));
	}

	public override void Serialize(ILittleEndianOutput o)
	{
		o.WriteShort(field_1_xf_index);
		if (IsBuiltin)
		{
			o.WriteByte(field_2_builtin_style);
			o.WriteByte(field_3_outline_style_level);
			return;
		}
		o.WriteShort(field_4_name.Length);
		o.WriteByte(field_3_stringHasMultibyte ? 1 : 0);
		if (field_3_stringHasMultibyte)
		{
			StringUtil.PutUnicodeLE(Name, o);
		}
		else
		{
			StringUtil.PutCompressedUnicode(Name, o);
		}
	}
}
