using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class BOFRecord : StandardRecord, ICloneable
{
	public const short sid = 2057;

	public const short biff2_sid = 9;

	public const short biff3_sid = 521;

	public const short biff4_sid = 1033;

	public const short biff5_sid = 2057;

	private int field_1_version;

	private int field_2_type;

	private int field_3_build;

	private int field_4_year;

	private int field_5_history;

	private int field_6_rversion;

	public const short VERSION = 6;

	public const short BUILD = 4307;

	public const short BUILD_YEAR = 1996;

	public const short HISTORY_MASK = 65;

	public int Version
	{
		get
		{
			return field_1_version;
		}
		set
		{
			field_1_version = value;
		}
	}

	public int HistoryBitMask
	{
		get
		{
			return field_5_history;
		}
		set
		{
			field_5_history = value;
		}
	}

	public int RequiredVersion
	{
		get
		{
			return field_6_rversion;
		}
		set
		{
			field_6_rversion = value;
		}
	}

	public BOFRecordType Type
	{
		get
		{
			return (BOFRecordType)field_2_type;
		}
		set
		{
			field_2_type = (int)value;
		}
	}

	private string TypeName => Type switch
	{
		BOFRecordType.Chart => "chart", 
		BOFRecordType.Excel4Macro => "excel 4 macro", 
		BOFRecordType.VBModule => "vb module", 
		BOFRecordType.Workbook => "workbook", 
		BOFRecordType.Worksheet => "worksheet", 
		BOFRecordType.WorkspaceFile => "workspace file", 
		_ => "#error unknown type#", 
	};

	public int Build
	{
		get
		{
			return field_3_build;
		}
		set
		{
			field_3_build = value;
		}
	}

	public int BuildYear
	{
		get
		{
			return field_4_year;
		}
		set
		{
			field_4_year = value;
		}
	}

	protected override int DataSize => 16;

	public override short Sid => 2057;

	public BOFRecord()
	{
	}

	private BOFRecord(BOFRecordType type)
	{
		field_1_version = 6;
		field_2_type = (int)type;
		field_3_build = 4307;
		field_4_year = 1996;
		field_5_history = 1;
		field_6_rversion = 6;
	}

	public static BOFRecord CreateSheetBOF()
	{
		return new BOFRecord(BOFRecordType.Worksheet);
	}

	public BOFRecord(RecordInputStream in1)
	{
		field_1_version = in1.ReadShort();
		field_2_type = in1.ReadShort();
		if (in1.Remaining >= 2)
		{
			field_3_build = in1.ReadShort();
		}
		if (in1.Remaining >= 2)
		{
			field_4_year = in1.ReadShort();
		}
		if (in1.Remaining >= 4)
		{
			field_5_history = in1.ReadInt();
		}
		if (in1.Remaining >= 4)
		{
			field_6_rversion = in1.ReadInt();
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[BOF RECORD]\n");
		stringBuilder.Append("    .version         = ").Append(StringUtil.ToHexString(Version)).Append("\n");
		stringBuilder.Append("    .type            = ").Append(StringUtil.ToHexString((int)Type)).Append("\n");
		stringBuilder.Append(" (").Append(TypeName).Append(")")
			.Append("\n");
		stringBuilder.Append("    .build           = ").Append(StringUtil.ToHexString(Build)).Append("\n");
		stringBuilder.Append("    .buildyear       = ").Append(BuildYear).Append("\n");
		stringBuilder.Append("    .history         = ").Append(StringUtil.ToHexString(HistoryBitMask)).Append("\n");
		stringBuilder.Append("    .requiredversion = ").Append(StringUtil.ToHexString(RequiredVersion)).Append("\n");
		stringBuilder.Append("[/BOF RECORD]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(Version);
		out1.WriteShort((int)Type);
		out1.WriteShort(Build);
		out1.WriteShort(BuildYear);
		out1.WriteInt(HistoryBitMask);
		out1.WriteInt(RequiredVersion);
	}

	public override object Clone()
	{
		return new BOFRecord
		{
			field_1_version = field_1_version,
			field_2_type = field_2_type,
			field_3_build = field_3_build,
			field_4_year = field_4_year,
			field_5_history = field_5_history,
			field_6_rversion = field_6_rversion
		};
	}
}
