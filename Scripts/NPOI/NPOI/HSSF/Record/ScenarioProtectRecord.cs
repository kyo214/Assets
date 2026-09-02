using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class ScenarioProtectRecord : StandardRecord
{
	public const short sid = 221;

	private short field_1_protect;

	public bool Protect
	{
		get
		{
			return field_1_protect == 1;
		}
		set
		{
			if (value)
			{
				field_1_protect = 1;
			}
			else
			{
				field_1_protect = 0;
			}
		}
	}

	public override short Sid => 221;

	protected override int DataSize => 2;

	public ScenarioProtectRecord()
	{
	}

	public ScenarioProtectRecord(RecordInputStream in1)
	{
		field_1_protect = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[SCENARIOPROTECT]\n");
		stringBuilder.Append("    .protect         = ").Append(Protect).Append("\n");
		stringBuilder.Append("[/SCENARIOPROTECT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_protect);
	}

	public override object Clone()
	{
		return new ScenarioProtectRecord
		{
			field_1_protect = field_1_protect
		};
	}
}
