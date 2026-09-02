using System;
using System.Text;
using NPOI.HSSF.Record.Common;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class FeatRecord : StandardRecord, ICloneable
{
	private static POILogger logger = POILogFactory.GetLogger(typeof(FeatRecord));

	public const short sid = 2152;

	public const short v11_sid = 2162;

	public const short v12_sid = 2168;

	private FtrHeader futureHeader;

	private int isf_sharedFeatureType;

	private byte reserved1;

	private long reserved2;

	private long cbFeatData;

	private int reserved3;

	private CellRangeAddress[] cellRefs;

	private SharedFeature sharedFeature;

	public override short Sid => 2152;

	protected override int DataSize => 27 + cellRefs.Length * 8 + sharedFeature.DataSize;

	public int Isf_sharedFeatureType => isf_sharedFeatureType;

	public long CbFeatData
	{
		get
		{
			return cbFeatData;
		}
		set
		{
			cbFeatData = value;
		}
	}

	public CellRangeAddress[] CellRefs
	{
		get
		{
			return cellRefs;
		}
		set
		{
			cellRefs = value;
		}
	}

	public SharedFeature SharedFeature
	{
		get
		{
			return sharedFeature;
		}
		set
		{
			sharedFeature = value;
			if (value is FeatProtection)
			{
				isf_sharedFeatureType = 2;
			}
			if (value is FeatFormulaErr2)
			{
				isf_sharedFeatureType = 3;
			}
			if (value is FeatSmartTag)
			{
				isf_sharedFeatureType = 4;
			}
			if (isf_sharedFeatureType == 3)
			{
				cbFeatData = sharedFeature.DataSize;
			}
			else
			{
				cbFeatData = 0L;
			}
		}
	}

	public FeatRecord()
	{
		futureHeader = new FtrHeader();
		futureHeader.RecordType = 2152;
	}

	public FeatRecord(RecordInputStream in1)
	{
		futureHeader = new FtrHeader(in1);
		isf_sharedFeatureType = in1.ReadShort();
		reserved1 = (byte)in1.ReadByte();
		reserved2 = in1.ReadInt();
		int num = in1.ReadUShort();
		cbFeatData = in1.ReadInt();
		reserved3 = in1.ReadShort();
		cellRefs = new CellRangeAddress[num];
		for (int i = 0; i < cellRefs.Length; i++)
		{
			cellRefs[i] = new CellRangeAddress(in1);
		}
		switch (isf_sharedFeatureType)
		{
		case 2:
			sharedFeature = new FeatProtection(in1);
			break;
		case 3:
			sharedFeature = new FeatFormulaErr2(in1);
			break;
		case 4:
			sharedFeature = new FeatSmartTag(in1);
			break;
		default:
			logger.Log(7, "Unknown Shared Feature " + isf_sharedFeatureType + " found!");
			break;
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[SHARED FEATURE]\n");
		stringBuilder.Append("[/SHARED FEATURE]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		futureHeader.Serialize(out1);
		out1.WriteShort(isf_sharedFeatureType);
		out1.WriteByte(reserved1);
		out1.WriteInt((int)reserved2);
		out1.WriteShort(cellRefs.Length);
		out1.WriteInt((int)cbFeatData);
		out1.WriteShort(reserved3);
		for (int i = 0; i < cellRefs.Length; i++)
		{
			cellRefs[i].Serialize(out1);
		}
		sharedFeature.Serialize(out1);
	}

	public override object Clone()
	{
		return CloneViaReserialise();
	}
}
