using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class IterationRecord : StandardRecord, ICloneable
{
	public const short sid = 17;

	private static BitField iterationOn = BitFieldFactory.GetInstance(1);

	private int _flags;

	public bool Iteration
	{
		get
		{
			return iterationOn.IsSet(_flags);
		}
		set
		{
			_flags = iterationOn.SetBoolean(_flags, value);
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 17;

	public IterationRecord(bool iterateOn)
	{
		_flags = iterationOn.SetBoolean(0, iterateOn);
	}

	public IterationRecord(RecordInputStream in1)
	{
		_flags = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[ITERATION]\n");
		stringBuilder.Append("    .flags      = ").Append(HexDump.ShortToHex(_flags)).Append("\n");
		stringBuilder.Append("[/ITERATION]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(_flags);
	}

	public override object Clone()
	{
		return new IterationRecord(Iteration);
	}
}
