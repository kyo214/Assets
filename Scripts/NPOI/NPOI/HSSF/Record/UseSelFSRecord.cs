using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class UseSelFSRecord : StandardRecord
{
	public const short sid = 352;

	private static BitField useNaturalLanguageFormulasFlag = BitFieldFactory.GetInstance(1);

	private int _options;

	protected override int DataSize => 2;

	public override short Sid => 352;

	public UseSelFSRecord(int options)
	{
		_options = options;
	}

	public UseSelFSRecord(RecordInputStream in1)
		: this(in1.ReadUShort())
	{
	}

	public UseSelFSRecord(bool b)
		: this(0)
	{
		_options = useNaturalLanguageFormulasFlag.SetBoolean(_options, b);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[USESELFS]\n");
		stringBuilder.Append("    .flag            = ").Append(HexDump.ShortToHex(_options)).Append("\n");
		stringBuilder.Append("[/USESELFS]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(_options);
	}
}
