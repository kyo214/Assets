using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class DefaultDataLabelTextPropertiesRecord : StandardRecord, ICloneable
{
	public static short sid = 4132;

	private short field_1_categoryDataType;

	public static short CATEGORY_DATA_TYPE_SHOW_LABELS_CHARACTERISTIC = 0;

	public static short CATEGORY_DATA_TYPE_VALUE_AND_PERCENTAGE_CHARACTERISTIC = 1;

	public static short CATEGORY_DATA_TYPE_ALL_TEXT_CHARACTERISTIC = 2;

	protected override int DataSize => 2;

	public override short Sid => sid;

	public short CategoryDataType
	{
		get
		{
			return field_1_categoryDataType;
		}
		set
		{
			field_1_categoryDataType = value;
		}
	}

	public DefaultDataLabelTextPropertiesRecord()
	{
	}

	public DefaultDataLabelTextPropertiesRecord(RecordInputStream in1)
	{
		field_1_categoryDataType = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[DEFAULTTEXT]\n");
		stringBuilder.Append("    .categoryDataType     = ").Append("0x").Append(HexDump.ToHex(CategoryDataType))
			.Append(" (")
			.Append(CategoryDataType)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/DEFAULTTEXT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_categoryDataType);
	}

	public override object Clone()
	{
		return new DefaultDataLabelTextPropertiesRecord
		{
			field_1_categoryDataType = field_1_categoryDataType
		};
	}
}
