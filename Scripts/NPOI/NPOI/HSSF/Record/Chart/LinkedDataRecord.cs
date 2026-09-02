using System;
using System.Text;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class LinkedDataRecord : StandardRecord, ICloneable
{
	public static short sid = 4177;

	private static BitField customNumberFormat = BitFieldFactory.GetInstance(1);

	private byte field_1_linkType;

	public static byte LINK_TYPE_TITLE_OR_TEXT = 0;

	public static byte LINK_TYPE_VALUES = 1;

	public static byte LINK_TYPE_CATEGORIES = 2;

	public static byte LINK_TYPE_SECONDARY_CATEGORIES = 3;

	private byte field_2_referenceType;

	public static byte REFERENCE_TYPE_DEFAULT_CATEGORIES = 0;

	public static byte REFERENCE_TYPE_DIRECT = 1;

	public static byte REFERENCE_TYPE_WORKSHEET = 2;

	public static byte REFERENCE_TYPE_NOT_USED = 3;

	public static byte REFERENCE_TYPE_ERROR_REPORTED = 4;

	private short field_3_options;

	private short field_4_indexNumberFmtRecord;

	private Formula field_5_formulaOfLink;

	protected override int DataSize => 6 + field_5_formulaOfLink.EncodedSize;

	public override short Sid => sid;

	public byte LinkType
	{
		get
		{
			return field_1_linkType;
		}
		set
		{
			field_1_linkType = value;
		}
	}

	public byte ReferenceType
	{
		get
		{
			return field_2_referenceType;
		}
		set
		{
			field_2_referenceType = value;
		}
	}

	public short Options
	{
		get
		{
			return field_3_options;
		}
		set
		{
			field_3_options = value;
		}
	}

	public short IndexNumberFmtRecord
	{
		get
		{
			return field_4_indexNumberFmtRecord;
		}
		set
		{
			field_4_indexNumberFmtRecord = value;
		}
	}

	public Ptg[] FormulaOfLink
	{
		get
		{
			return field_5_formulaOfLink.Tokens;
		}
		set
		{
			field_5_formulaOfLink = Formula.Create(value);
		}
	}

	public bool IsCustomNumberFormat
	{
		get
		{
			return customNumberFormat.IsSet(field_3_options);
		}
		set
		{
			field_3_options = customNumberFormat.SetShortBoolean(field_3_options, value);
		}
	}

	public LinkedDataRecord()
	{
	}

	public LinkedDataRecord(RecordInputStream in1)
	{
		field_1_linkType = (byte)in1.ReadByte();
		field_2_referenceType = (byte)in1.ReadByte();
		field_3_options = in1.ReadShort();
		field_4_indexNumberFmtRecord = in1.ReadShort();
		int encodedTokenLen = in1.ReadUShort();
		field_5_formulaOfLink = Formula.Read(encodedTokenLen, in1);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[AI]\n");
		stringBuilder.Append("    .linkType             = ").Append(HexDump.ByteToHex(LinkType)).Append('\n');
		stringBuilder.Append("    .referenceType        = ").Append(HexDump.ByteToHex(ReferenceType)).Append('\n');
		stringBuilder.Append("    .options              = ").Append(HexDump.ShortToHex(Options)).Append('\n');
		stringBuilder.Append("    .customNumberFormat   = ").Append(IsCustomNumberFormat).Append('\n');
		stringBuilder.Append("    .indexNumberFmtRecord = ").Append(HexDump.ShortToHex(IndexNumberFmtRecord)).Append('\n');
		stringBuilder.Append("    .FormulaOfLink        = ").Append('\n');
		Ptg[] tokens = field_5_formulaOfLink.Tokens;
		foreach (Ptg ptg in tokens)
		{
			stringBuilder.Append(ptg.ToString()).Append(ptg.RVAType).Append('\n');
		}
		stringBuilder.Append("[/AI]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteByte(field_1_linkType);
		out1.WriteByte(field_2_referenceType);
		out1.WriteShort(field_3_options);
		out1.WriteShort(field_4_indexNumberFmtRecord);
		field_5_formulaOfLink.Serialize(out1);
	}

	public override object Clone()
	{
		return new LinkedDataRecord
		{
			field_1_linkType = field_1_linkType,
			field_2_referenceType = field_2_referenceType,
			field_3_options = field_3_options,
			field_4_indexNumberFmtRecord = field_4_indexNumberFmtRecord,
			field_5_formulaOfLink = field_5_formulaOfLink.Copy()
		};
	}
}
