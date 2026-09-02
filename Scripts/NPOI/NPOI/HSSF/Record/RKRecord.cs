using System.Text;
using NPOI.HSSF.Util;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class RKRecord : CellRecord
{
	public const short sid = 638;

	public const short RK_IEEE_NUMBER = 0;

	public const short RK_IEEE_NUMBER_TIMES_100 = 1;

	public const short RK_INTEGER = 2;

	public const short RK_INTEGER_TIMES_100 = 3;

	private int field_4_rk_number;

	public int RKField => field_4_rk_number;

	public short RKType => (short)(field_4_rk_number & 3);

	public double RKNumber => RKUtil.DecodeNumber(field_4_rk_number);

	protected override string RecordName => "RK";

	protected override int ValueDataSize => 4;

	public override short Sid => 638;

	public RKRecord()
	{
	}

	public RKRecord(RecordInputStream in1)
		: base(in1)
	{
		field_4_rk_number = in1.ReadInt();
	}

	protected override void AppendValueText(StringBuilder sb)
	{
		sb.Append("  .value= ").Append(RKNumber);
	}

	protected override void SerializeValue(ILittleEndianOutput out1)
	{
		out1.WriteInt(field_4_rk_number);
	}

	public new object Clone()
	{
		RKRecord rKRecord = new RKRecord();
		CopyBaseFields(rKRecord);
		rKRecord.field_4_rk_number = field_4_rk_number;
		return rKRecord;
	}
}
