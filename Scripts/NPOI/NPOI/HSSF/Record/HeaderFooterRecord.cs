using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class HeaderFooterRecord : StandardRecord, ICloneable
{
	private static byte[] BLANK_GUID = new byte[16];

	public const short sid = 2204;

	private byte[] _rawData;

	protected override int DataSize => _rawData.Length;

	public override short Sid => 2204;

	public byte[] Guid
	{
		get
		{
			byte[] array = new byte[16];
			Array.Copy(_rawData, 12, array, 0, array.Length);
			return array;
		}
	}

	public bool IsCurrentSheet => Arrays.Equals(Guid, BLANK_GUID);

	public HeaderFooterRecord(byte[] data)
	{
		_rawData = data;
	}

	public HeaderFooterRecord(RecordInputStream in1)
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
		stringBuilder.Append("[").Append("HEADERFOOTER").Append("] (0x");
		stringBuilder.Append(StringUtil.ToHexString((short)2204).ToUpper() + ")\n");
		stringBuilder.Append("  rawData=").Append(HexDump.ToHex(_rawData)).Append("\n");
		stringBuilder.Append("[/").Append("HEADERFOOTER").Append("]\n");
		return stringBuilder.ToString();
	}

	public override object Clone()
	{
		return CloneViaReserialise();
	}
}
