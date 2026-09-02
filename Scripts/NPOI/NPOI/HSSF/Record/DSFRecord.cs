using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class DSFRecord : StandardRecord
{
	public const short sid = 353;

	private int _options;

	private static BitField biff5BookStreamFlag = BitFieldFactory.GetInstance(1);

	public bool IsBiff5BookStreamPresent => biff5BookStreamFlag.IsSet(_options);

	protected override int DataSize => 2;

	public override short Sid => 353;

	private DSFRecord(int options)
	{
		_options = options;
	}

	public DSFRecord(bool isBiff5BookStreamPresent)
		: this(0)
	{
		_options = biff5BookStreamFlag.SetBoolean(0, isBiff5BookStreamPresent);
	}

	public DSFRecord(RecordInputStream in1)
		: this(in1.ReadShort())
	{
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[DSF]\n");
		stringBuilder.Append("    .IsDSF           = ").Append(StringUtil.ToHexString(_options)).Append("\n");
		stringBuilder.Append("[/DSF]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(_options);
	}
}
