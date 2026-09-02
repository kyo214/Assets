using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class StartBlockRecord : StandardRecord
{
	public static short sid = 2130;

	private short rt;

	private short grbitFrt;

	public ObjectKind ObjectKind { get; set; }

	public short ObjectContext { get; set; }

	public short ObjectInstance1 { get; set; }

	public short ObjectInstance2 { get; set; }

	protected override int DataSize => 12;

	public override short Sid => sid;

	public StartBlockRecord()
	{
		rt = sid;
		grbitFrt = 0;
	}

	public StartBlockRecord(RecordInputStream in1)
	{
		rt = in1.ReadShort();
		grbitFrt = in1.ReadShort();
		ObjectKind = (ObjectKind)in1.ReadShort();
		ObjectContext = in1.ReadShort();
		ObjectInstance1 = in1.ReadShort();
		ObjectInstance2 = in1.ReadShort();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(rt);
		out1.WriteShort(grbitFrt);
		out1.WriteShort((int)ObjectKind);
		out1.WriteShort(ObjectContext);
		out1.WriteShort(ObjectInstance1);
		out1.WriteShort(ObjectInstance2);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[STARTBLOCK]\n");
		stringBuilder.Append("    .rt              =").Append(HexDump.ShortToHex(rt)).Append('\n');
		stringBuilder.Append("    .grbitFrt        =").Append(HexDump.ShortToHex(grbitFrt)).Append('\n');
		stringBuilder.Append("    .iObjectKind     =").Append(HexDump.ShortToHex((int)ObjectKind)).Append('\n');
		stringBuilder.Append("    .iObjectContext  =").Append(HexDump.ShortToHex(ObjectContext)).Append('\n');
		stringBuilder.Append("    .iObjectInstance1=").Append(HexDump.ShortToHex(ObjectInstance1)).Append('\n');
		stringBuilder.Append("    .iObjectInstance2=").Append(HexDump.ShortToHex(ObjectInstance2)).Append('\n');
		stringBuilder.Append("[/STARTBLOCK]\n");
		return stringBuilder.ToString();
	}

	public override object Clone()
	{
		return new StartBlockRecord
		{
			rt = rt,
			grbitFrt = grbitFrt,
			ObjectKind = ObjectKind,
			ObjectContext = ObjectContext,
			ObjectInstance1 = ObjectInstance1,
			ObjectInstance2 = ObjectInstance2
		};
	}

	public static StartBlockRecord CreateStartBlock(ObjectKind objectKind)
	{
		return CreateStartBlock(objectKind, 0, 0, 0);
	}

	public static StartBlockRecord CreateStartBlock(ObjectKind objectKind, short objectContext)
	{
		return CreateStartBlock(objectKind, objectContext, 0, 0);
	}

	public static StartBlockRecord CreateStartBlock(ObjectKind objectKind, short objectContext, short objectInstance1)
	{
		return CreateStartBlock(objectKind, objectContext, objectInstance1, 0);
	}

	public static StartBlockRecord CreateStartBlock(ObjectKind objectKind, short objectContext, short objectInstance1, short objectInstance2)
	{
		return new StartBlockRecord
		{
			ObjectKind = objectKind,
			ObjectContext = objectContext,
			ObjectInstance1 = objectInstance1,
			ObjectInstance2 = objectInstance2
		};
	}
}
