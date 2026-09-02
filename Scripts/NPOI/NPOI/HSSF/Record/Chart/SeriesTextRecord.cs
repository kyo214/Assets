using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class SeriesTextRecord : StandardRecord
{
	private const int MAX_LEN = 255;

	public const short sid = 4109;

	private short field_1_id;

	private bool is16bit;

	private string field_4_text;

	protected override int DataSize => 4 + field_4_text.Length * ((!is16bit) ? 1 : 2);

	public override short Sid => 4109;

	public short Id
	{
		get
		{
			return field_1_id;
		}
		set
		{
			field_1_id = value;
		}
	}

	public string Text
	{
		get
		{
			return field_4_text;
		}
		set
		{
			if (value.Length > 255)
			{
				throw new ArgumentException("Text is too long (" + value.Length + ">" + 255 + ")");
			}
			field_4_text = value;
			is16bit = StringUtil.HasMultibyte(value);
		}
	}

	public SeriesTextRecord()
	{
		field_4_text = "";
		is16bit = false;
	}

	public SeriesTextRecord(RecordInputStream in1)
	{
		field_1_id = in1.ReadShort();
		int requestedLength = (byte)in1.ReadByte();
		is16bit = (in1.ReadUByte() & 1) != 0;
		if (is16bit)
		{
			field_4_text = in1.ReadUnicodeLEString(requestedLength);
		}
		else
		{
			field_4_text = in1.ReadCompressedUnicode(requestedLength);
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[SERIESTEXT]\n");
		stringBuilder.Append("    .id                   = ").Append("0x").Append(HexDump.ToHex(Id))
			.Append(" (")
			.Append(Id)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .textLength           = ").Append(field_4_text.Length);
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .is16bit         = ").Append(is16bit);
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .text                 = ").Append(" (").Append(Text)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/SERIESTEXT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_id);
		out1.WriteByte(field_4_text.Length);
		if (is16bit)
		{
			out1.WriteByte(1);
			StringUtil.PutUnicodeLE(field_4_text, out1);
		}
		else
		{
			out1.WriteByte(0);
			StringUtil.PutCompressedUnicode(field_4_text, out1);
		}
	}

	public override object Clone()
	{
		return new SeriesTextRecord
		{
			field_1_id = field_1_id,
			is16bit = is16bit,
			field_4_text = field_4_text
		};
	}
}
