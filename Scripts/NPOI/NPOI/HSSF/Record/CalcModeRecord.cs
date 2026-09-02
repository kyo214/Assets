using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class CalcModeRecord : StandardRecord, ICloneable
{
	public const short sid = 13;

	public const short MANUAL = 0;

	public const short AUTOMATIC = 1;

	public const short AUTOMATIC_EXCEPT_TABLES = -1;

	private short field_1_calcmode;

	protected override int DataSize => 2;

	public override short Sid => 13;

	public CalcModeRecord()
	{
	}

	public CalcModeRecord(RecordInputStream in1)
	{
		field_1_calcmode = in1.ReadShort();
	}

	public void SetCalcMode(short calcmode)
	{
		field_1_calcmode = calcmode;
	}

	public short GetCalcMode()
	{
		return field_1_calcmode;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[CALCMODE]\n");
		stringBuilder.Append("    .calcmode       = ").Append(StringUtil.ToHexString(GetCalcMode())).Append("\n");
		stringBuilder.Append("[/CALCMODE]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(GetCalcMode());
	}

	public override object Clone()
	{
		return new CalcModeRecord
		{
			field_1_calcmode = field_1_calcmode
		};
	}
}
