using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class RecalcIdRecord : StandardRecord
{
	public const short sid = 449;

	private int _reserved0;

	private int _engineId;

	public bool IsNeeded => true;

	public int EngineId
	{
		get
		{
			return _engineId;
		}
		set
		{
			_engineId = value;
		}
	}

	protected override int DataSize => 8;

	public override short Sid => 449;

	public RecalcIdRecord()
	{
		_reserved0 = 0;
		_engineId = 0;
	}

	public RecalcIdRecord(RecordInputStream in1)
	{
		in1.ReadUShort();
		_reserved0 = in1.ReadUShort();
		_engineId = in1.ReadInt();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[RECALCID]\n");
		stringBuilder.Append("    .reserved = ").Append(HexDump.ShortToHex(_reserved0)).Append("\n");
		stringBuilder.Append("    .engineId = ").Append(HexDump.IntToHex(_engineId)).Append("\n");
		stringBuilder.Append("[/RECALCID]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(449);
		out1.WriteShort(_reserved0);
		out1.WriteInt(_engineId);
	}
}
