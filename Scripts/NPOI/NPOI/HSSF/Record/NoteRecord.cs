using System;
using System.Globalization;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class NoteRecord : StandardRecord, ICloneable
{
	public static readonly NoteRecord[] EMPTY_ARRAY = new NoteRecord[0];

	public const short sid = 28;

	public const short NOTE_HIDDEN = 0;

	public const short NOTE_VISIBLE = 2;

	private int field_1_row;

	private int field_2_col;

	private short field_3_flags;

	private int field_4_shapeid;

	private bool field_5_hasMultibyte;

	private string field_6_author;

	private const byte DEFAULT_PADDING = 0;

	private byte? field_7_padding;

	public override short Sid => 28;

	protected override int DataSize => 11 + field_6_author.Length * ((!field_5_hasMultibyte) ? 1 : 2) + (field_7_padding.HasValue ? 1 : 0);

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

	public short Flags
	{
		get
		{
			return field_3_flags;
		}
		set
		{
			field_3_flags = value;
		}
	}

	public int ShapeId
	{
		get
		{
			return field_4_shapeid;
		}
		set
		{
			field_4_shapeid = value;
		}
	}

	public string Author
	{
		get
		{
			return field_6_author;
		}
		set
		{
			field_6_author = value;
			field_5_hasMultibyte = StringUtil.HasMultibyte(value);
		}
	}

	internal bool AuthorIsMultibyte => field_5_hasMultibyte;

	public NoteRecord()
	{
		field_6_author = "";
		field_3_flags = 0;
		field_7_padding = 0;
	}

	public NoteRecord(RecordInputStream in1)
	{
		field_1_row = in1.ReadShort();
		field_2_col = in1.ReadUShort();
		field_3_flags = in1.ReadShort();
		field_4_shapeid = in1.ReadUShort();
		int num = in1.ReadShort();
		field_5_hasMultibyte = in1.ReadByte() != 0;
		if (field_5_hasMultibyte)
		{
			field_6_author = StringUtil.ReadUnicodeLE(in1, num);
		}
		else
		{
			field_6_author = StringUtil.ReadCompressedUnicode(in1, num);
		}
		if (in1.Available() == 1)
		{
			field_7_padding = (byte)in1.ReadByte();
		}
		else if (in1.Available() == 2 && num == 0)
		{
			field_7_padding = (byte)in1.ReadByte();
			in1.ReadByte();
		}
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_row);
		out1.WriteShort(field_2_col);
		out1.WriteShort(field_3_flags);
		out1.WriteShort(field_4_shapeid);
		out1.WriteShort(field_6_author.Length);
		out1.WriteByte(field_5_hasMultibyte ? 1 : 0);
		if (field_5_hasMultibyte)
		{
			StringUtil.PutUnicodeLE(field_6_author, out1);
		}
		else
		{
			StringUtil.PutCompressedUnicode(field_6_author, out1);
		}
		if (field_7_padding.HasValue)
		{
			out1.WriteByte(Convert.ToInt32(field_7_padding, CultureInfo.InvariantCulture));
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[NOTE]\n");
		stringBuilder.Append("    .recordid = 0x" + StringUtil.ToHexString(Sid) + ", size = " + RecordSize + "\n");
		stringBuilder.Append("    .row =     " + field_1_row + "\n");
		stringBuilder.Append("    .col =     " + field_2_col + "\n");
		stringBuilder.Append("    .flags =   " + field_3_flags + "\n");
		stringBuilder.Append("    .shapeid = " + field_4_shapeid + "\n");
		stringBuilder.Append("    .author =  " + field_6_author + "\n");
		stringBuilder.Append("[/NOTE]\n");
		return stringBuilder.ToString();
	}

	public override object Clone()
	{
		return new NoteRecord
		{
			field_1_row = field_1_row,
			field_2_col = field_2_col,
			field_3_flags = field_3_flags,
			field_4_shapeid = field_4_shapeid,
			field_6_author = field_6_author
		};
	}
}
