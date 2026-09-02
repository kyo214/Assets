using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class RefreshAllRecord : StandardRecord
{
	public const short sid = 439;

	private static BitField refreshFlag = BitFieldFactory.GetInstance(1);

	private int _options;

	public bool RefreshAll
	{
		get
		{
			return refreshFlag.IsSet(_options);
		}
		set
		{
			_options = refreshFlag.SetBoolean(_options, value);
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 439;

	public RefreshAllRecord(int options)
	{
		_options = options;
	}

	public RefreshAllRecord(RecordInputStream in1)
		: this(in1.ReadUShort())
	{
	}

	public RefreshAllRecord(bool refreshAll)
		: this(0)
	{
		RefreshAll = refreshAll;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[REFRESHALL]\n");
		stringBuilder.Append("    .refreshall      = ").Append(RefreshAll).Append("\n");
		stringBuilder.Append("[/REFRESHALL]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(_options);
	}

	public override object Clone()
	{
		return new RefreshAllRecord(_options);
	}
}
