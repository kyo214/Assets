using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class UserSViewBegin : StandardRecord
{
	public const short sid = 426;

	private byte[] _rawData;

	protected override int DataSize => _rawData.Length;

	public override short Sid => 426;

	public byte[] Guid
	{
		get
		{
			byte[] array = new byte[16];
			Array.Copy(_rawData, 0, array, 0, array.Length);
			return array;
		}
	}

	public UserSViewBegin(byte[] data)
	{
		_rawData = data;
	}

	public UserSViewBegin(RecordInputStream in1)
	{
		_rawData = in1.ReadRemainder();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.Write(_rawData);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[").Append("USERSVIEWBEGIN").Append("] (0x");
		stringBuilder.Append(StringUtil.ToHexString((short)426).ToUpper() + ")\n");
		stringBuilder.Append("  rawData=").Append(HexDump.ToHex(_rawData)).Append("\n");
		stringBuilder.Append("[/").Append("USERSVIEWBEGIN").Append("]\n");
		return stringBuilder.ToString();
	}

	public override object Clone()
	{
		return CloneViaReserialise();
	}
}
