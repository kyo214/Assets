using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class ProtectionRev4Record : StandardRecord
{
	public const short sid = 431;

	private static BitField protectedFlag = BitFieldFactory.GetInstance(1);

	private short _options;

	public bool Protect
	{
		get
		{
			return protectedFlag.IsSet(_options);
		}
		set
		{
			_options = (short)protectedFlag.SetBoolean(_options, value);
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 431;

	public ProtectionRev4Record(short options)
	{
		_options = options;
	}

	public ProtectionRev4Record(bool protect)
		: this(0)
	{
		Protect = protect;
	}

	public ProtectionRev4Record(RecordInputStream in1)
		: this(in1.ReadShort())
	{
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[PROT4REV]\n");
		stringBuilder.Append("    .protect         = ").Append(Protect).Append("\n");
		stringBuilder.Append("[/PROT4REV]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(_options);
	}
}
