using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class ObjectProtectRecord : StandardRecord, ICloneable
{
	public const short sid = 99;

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

	protected override int DataSize => 2;

	public override short Sid => 99;

	public ObjectProtectRecord()
	{
	}

	public ObjectProtectRecord(RecordInputStream in1)
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
		return new ObjectProtectRecord
		{
			field_1_protect = field_1_protect
		};
	}
}
