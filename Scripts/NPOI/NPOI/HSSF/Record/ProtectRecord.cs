using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class ProtectRecord : StandardRecord
{
	public const short sid = 18;

	private static BitField protectFlag = BitFieldFactory.GetInstance(1);

	private short _options;

	public bool Protect
	{
		get
		{
			return protectFlag.IsSet(_options);
		}
		set
		{
			_options = (short)protectFlag.SetBoolean(_options, value);
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 18;

	public ProtectRecord(short options)
	{
		_options = options;
	}

	public ProtectRecord(RecordInputStream in1)
		: this(in1.ReadShort())
	{
	}

	public ProtectRecord(bool isProtected)
		: this(0)
	{
		Protect = isProtected;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[PROTECT]\n");
		stringBuilder.Append("    .options = ").Append(HexDump.ShortToHex(_options)).Append("\n");
		stringBuilder.Append("[/PROTECT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(_options);
	}

	public override object Clone()
	{
		return new ProtectRecord(_options);
	}
}
