using System;
using System.Globalization;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class ContinueRecord : StandardRecord, ICloneable
{
	public const short sid = 60;

	private byte[] field_1_data;

	protected override int DataSize => field_1_data.Length;

	public byte[] Data
	{
		get
		{
			return field_1_data;
		}
		set
		{
			field_1_data = value;
		}
	}

	public override short Sid => 60;

	private ContinueRecord()
	{
	}

	public ContinueRecord(byte[] data)
	{
		field_1_data = data;
	}

	public ContinueRecord(RecordInputStream in1)
	{
		field_1_data = in1.ReadRemainder();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.Write(field_1_data);
	}

	[Obsolete]
	public static int Write(byte[] destBuf, int destOffset, byte? initialDataByte, byte[] srcData)
	{
		return Write(destBuf, destOffset, initialDataByte, srcData, 0, srcData.Length);
	}

	[Obsolete]
	public static int Write(byte[] destBuf, int destOffset, byte? initialDataByte, byte[] srcData, int srcOffset, int len)
	{
		int num = len + (initialDataByte.HasValue ? 1 : 0);
		LittleEndian.PutUShort(destBuf, destOffset, 60);
		LittleEndian.PutUShort(destBuf, destOffset + 2, num);
		int num2 = destOffset + 4;
		if (initialDataByte.HasValue)
		{
			LittleEndian.PutByte(destBuf, num2, Convert.ToByte(initialDataByte, CultureInfo.InvariantCulture));
			num2++;
		}
		Array.Copy(srcData, srcOffset, destBuf, num2, len);
		return 4 + num;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[CONTINUE RECORD]\n");
		stringBuilder.Append("    .data        = ").Append(StringUtil.ToHexString((short)60)).Append("\n");
		stringBuilder.Append("[/CONTINUE RECORD]\n");
		return stringBuilder.ToString();
	}

	public override object Clone()
	{
		return new ContinueRecord
		{
			Data = field_1_data
		};
	}
}
