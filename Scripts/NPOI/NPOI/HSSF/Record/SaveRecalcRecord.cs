using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class SaveRecalcRecord : StandardRecord
{
	public const short sid = 95;

	private short field_1_recalc;

	public bool Recalc
	{
		get
		{
			return field_1_recalc == 1;
		}
		set
		{
			field_1_recalc = (short)(value ? 1 : 0);
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 95;

	public SaveRecalcRecord()
	{
	}

	public SaveRecalcRecord(RecordInputStream in1)
	{
		field_1_recalc = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[SAVERECALC]\n");
		stringBuilder.Append("    .recalc         = ").Append(Recalc).Append("\n");
		stringBuilder.Append("[/SAVERECALC]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_recalc);
	}

	public override object Clone()
	{
		return new SaveRecalcRecord
		{
			field_1_recalc = field_1_recalc
		};
	}
}
