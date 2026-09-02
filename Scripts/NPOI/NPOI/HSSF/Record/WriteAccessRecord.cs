using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class WriteAccessRecord : StandardRecord
{
	public const short sid = 92;

	private string field_1_username = string.Empty;

	private const byte PAD_CHAR = 32;

	private const int DATA_SIZE = 112;

	private static byte[] PADDING;

	public string Username
	{
		get
		{
			return field_1_username;
		}
		set
		{
			bool flag = StringUtil.HasMultibyte(value);
			int num = 3 + Username.Length * ((!flag) ? 1 : 2);
			if (112 - num < 0)
			{
				throw new ArgumentException("Name is too long: " + value);
			}
			field_1_username = value;
		}
	}

	protected override int DataSize => 112;

	public override short Sid => 92;

	static WriteAccessRecord()
	{
		PADDING = new byte[112];
		Arrays.Fill(PADDING, 32);
	}

	public WriteAccessRecord()
	{
	}

	public WriteAccessRecord(RecordInputStream in1)
	{
		if (in1.Remaining > 112)
		{
			throw new RecordFormatException("Expected data size (" + 112 + ") but got (" + in1.Remaining + ")");
		}
		int num = in1.ReadUShort();
		int num2 = in1.ReadUByte();
		if (num > 112 || (num2 & 0xFE) != 0)
		{
			byte[] array = new byte[3 + in1.Remaining];
			LittleEndian.PutUShort(array, 0, num);
			LittleEndian.PutByte(array, 2, num2);
			in1.ReadFully(array, 3, array.Length - 3);
			char[] array2 = new char[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = (char)array[i];
			}
			string text = new string(array2);
			Username = text.Trim();
		}
		else
		{
			field_1_username = (((num2 & 1) != 0) ? StringUtil.ReadUnicodeLE(in1, num) : StringUtil.ReadCompressedUnicode(in1, num)).Trim();
			for (int num3 = in1.Remaining; num3 > 0; num3--)
			{
				in1.ReadUByte();
			}
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[WriteACCESS]\n");
		stringBuilder.Append("    .name            = ").Append(field_1_username).Append("\n");
		stringBuilder.Append("[/WriteACCESS]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		string username = Username;
		bool flag = StringUtil.HasMultibyte(username);
		out1.WriteShort(username.Length);
		out1.WriteByte(flag ? 1 : 0);
		if (flag)
		{
			StringUtil.PutUnicodeLE(username, out1);
		}
		else
		{
			StringUtil.PutCompressedUnicode(username, out1);
		}
		int num = 3 + username.Length * ((!flag) ? 1 : 2);
		int len = 112 - num;
		out1.Write(PADDING, 0, len);
	}
}
