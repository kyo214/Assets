using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class UserSViewEnd : StandardRecord
{
	public const short sid = 427;

	private byte[] _rawData;

	protected override int DataSize => _rawData.Length;

	public override short Sid => 427;

	public UserSViewEnd(byte[] data)
	{
		_rawData = data;
	}

	public UserSViewEnd(RecordInputStream in1)
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
		stringBuilder.Append("[").Append("USERSVIEWEND").Append("] (0x");
		stringBuilder.Append(StringUtil.ToHexString((short)427).ToUpper() + ")\n");
		stringBuilder.Append("  rawData=").Append(HexDump.ToHex(_rawData)).Append("\n");
		stringBuilder.Append("[/").Append("USERSVIEWEND").Append("]\n");
		return stringBuilder.ToString();
	}

	public override object Clone()
	{
		return CloneViaReserialise();
	}
}
