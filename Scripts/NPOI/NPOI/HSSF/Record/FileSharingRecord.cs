using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class FileSharingRecord : StandardRecord, ICloneable
{
	public const short sid = 91;

	private short field_1_Readonly;

	private short field_2_password;

	private byte field_3_username_unicode_options;

	private string field_3_username_value;

	public short ReadOnly
	{
		get
		{
			return field_1_Readonly;
		}
		set
		{
			field_1_Readonly = value;
		}
	}

	public short Password
	{
		get
		{
			return field_2_password;
		}
		set
		{
			field_2_password = value;
		}
	}

	public string Username
	{
		get
		{
			return field_3_username_value;
		}
		set
		{
			field_3_username_value = value;
		}
	}

	protected override int DataSize
	{
		get
		{
			int length = field_3_username_value.Length;
			if (length < 1)
			{
				return 6;
			}
			return 7 + length;
		}
	}

	public override short Sid => 91;

	public FileSharingRecord()
	{
	}

	public FileSharingRecord(RecordInputStream in1)
	{
		field_1_Readonly = in1.ReadShort();
		field_2_password = in1.ReadShort();
		int num = in1.ReadShort();
		if (num > 0)
		{
			field_3_username_unicode_options = (byte)in1.ReadByte();
			field_3_username_value = in1.ReadCompressedUnicode(num);
			if (field_3_username_value == null)
			{
				field_3_username_value = "";
			}
		}
		else
		{
			field_3_username_value = "";
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[FILESHARING]\n");
		stringBuilder.Append("    .Readonly       = ").Append((ReadOnly == 1) ? "true" : "false").Append("\n");
		stringBuilder.Append("    .password       = ").Append(StringUtil.ToHexString(Password)).Append("\n");
		stringBuilder.Append("    .username       = ").Append(Username).Append("\n");
		stringBuilder.Append("[/FILESHARING]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(ReadOnly);
		out1.WriteShort(Password);
		out1.WriteShort(field_3_username_value.Length);
		if (field_3_username_value.Length > 0)
		{
			out1.WriteByte(field_3_username_unicode_options);
			StringUtil.PutCompressedUnicode(Username, out1);
		}
	}

	public override object Clone()
	{
		return new FileSharingRecord
		{
			ReadOnly = field_1_Readonly,
			Password = field_2_password,
			Username = field_3_username_value
		};
	}
}
