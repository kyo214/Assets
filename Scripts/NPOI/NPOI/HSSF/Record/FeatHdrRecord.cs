using System;
using System.Text;
using NPOI.HSSF.Record.Common;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class FeatHdrRecord : StandardRecord, ICloneable
{
	public const int SHAREDFEATURES_ISFPROTECTION = 2;

	public const int SHAREDFEATURES_ISFFEC2 = 3;

	public const int SHAREDFEATURES_ISFFACTOID = 4;

	public const int SHAREDFEATURES_ISFLIST = 5;

	public const short sid = 2151;

	private FtrHeader futureHeader;

	private int isf_sharedFeatureType;

	private byte reserved;

	private long cbHdrData;

	private byte[] rgbHdrData;

	public override short Sid => 2151;

	protected override int DataSize => 19 + rgbHdrData.Length;

	public FeatHdrRecord()
	{
		futureHeader = new FtrHeader();
		futureHeader.RecordType = 2151;
	}

	public FeatHdrRecord(RecordInputStream in1)
	{
		futureHeader = new FtrHeader(in1);
		isf_sharedFeatureType = in1.ReadShort();
		reserved = (byte)in1.ReadByte();
		cbHdrData = in1.ReadInt();
		rgbHdrData = in1.ReadRemainder();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[FEATURE HEADER]\n");
		stringBuilder.Append("[/FEATURE HEADER]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		futureHeader.Serialize(out1);
		out1.WriteShort(isf_sharedFeatureType);
		out1.WriteByte(reserved);
		out1.WriteInt((int)cbHdrData);
		out1.Write(rgbHdrData);
	}

	public override object Clone()
	{
		return CloneViaReserialise();
	}
}
