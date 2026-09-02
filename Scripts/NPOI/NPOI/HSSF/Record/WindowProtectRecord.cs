using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class WindowProtectRecord : StandardRecord
{
	public const short sid = 25;

	private static BitField settingsProtectedFlag = BitFieldFactory.GetInstance(1);

	private int _options;

	public bool Protect
	{
		get
		{
			return settingsProtectedFlag.IsSet(_options);
		}
		set
		{
			_options = settingsProtectedFlag.SetBoolean(_options, value);
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 25;

	public WindowProtectRecord(int options)
	{
		_options = options;
	}

	public WindowProtectRecord(RecordInputStream in1)
		: this(in1.ReadUShort())
	{
	}

	public WindowProtectRecord(bool protect)
		: this(0)
	{
		Protect = protect;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[WINDOWPROTECT]\n");
		stringBuilder.Append("    .protect         = ").Append(Protect).Append("\n");
		stringBuilder.Append("[/WINDOWPROTECT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(_options);
	}
}
