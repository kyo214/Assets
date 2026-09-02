using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class OldStringRecord
{
	public const short biff2_sid = 7;

	public const short biff345_sid = 519;

	private short sid;

	private short field_1_string_len;

	private byte[] field_2_bytes;

	private CodepageRecord codepage;

	public bool IsBiff2 => sid == 7;

	public short Sid => sid;

	public OldStringRecord(RecordInputStream in1)
	{
		sid = in1.Sid;
		if (in1.Sid == 7)
		{
			field_1_string_len = (short)in1.ReadUByte();
		}
		else
		{
			field_1_string_len = in1.ReadShort();
		}
		field_2_bytes = new byte[field_1_string_len];
		in1.Read(field_2_bytes, 0, field_1_string_len);
	}

	public void SetCodePage(CodepageRecord codepage)
	{
		this.codepage = codepage;
	}

	public string GetString()
	{
		return GetString(field_2_bytes, codepage);
	}

	protected internal static string GetString(byte[] data, CodepageRecord codepage)
	{
		int num = 28591;
		if (codepage != null)
		{
			num = codepage.Codepage & 0xFFFF;
		}
		try
		{
			return CodePageUtil.GetStringFromCodePage(data, num);
		}
		catch (EncoderFallbackException innerException)
		{
			throw new ArgumentException("Unsupported codepage requested", innerException);
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[OLD STRING]\n");
		stringBuilder.Append("    .string            = ").Append(GetString()).Append("\n");
		stringBuilder.Append("[/OLD STRING]\n");
		return stringBuilder.ToString();
	}
}
