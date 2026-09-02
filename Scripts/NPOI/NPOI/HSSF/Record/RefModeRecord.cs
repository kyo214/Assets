using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class RefModeRecord : StandardRecord
{
	public const short sid = 15;

	public const short USE_A1_MODE = 1;

	public const short USE_R1C1_MODE = 0;

	private short field_1_mode;

	public short Mode
	{
		get
		{
			return field_1_mode;
		}
		set
		{
			field_1_mode = value;
		}
	}

	public override short Sid => 15;

	protected override int DataSize => 2;

	public RefModeRecord()
	{
	}

	public RefModeRecord(RecordInputStream in1)
	{
		field_1_mode = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[REFMODE]\n");
		stringBuilder.Append("    .mode           = ").Append(StringUtil.ToHexString(Mode)).Append("\n");
		stringBuilder.Append("[/REFMODE]\n");
		return stringBuilder.ToString();
	}

	public override object Clone()
	{
		return new RefModeRecord
		{
			field_1_mode = field_1_mode
		};
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(Mode);
	}
}
