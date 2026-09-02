using System;
using NPOI.Util;

namespace NPOI.HSSF.Record.CF;

public class IconMultiStateThreshold : Threshold, ICloneable
{
	public static byte EQUALS_EXCLUDE = 0;

	public static byte EQUALS_INCLUDE = 1;

	private byte equals;

	public override int DataLength => base.DataLength + 5;

	public IconMultiStateThreshold()
	{
		equals = EQUALS_INCLUDE;
	}

	public IconMultiStateThreshold(ILittleEndianInput in1)
		: base(in1)
	{
		equals = (byte)in1.ReadByte();
		in1.ReadInt();
	}

	public byte GetEquals()
	{
		return equals;
	}

	public void SetEquals(byte Equals)
	{
		equals = Equals;
	}

	public object Clone()
	{
		IconMultiStateThreshold iconMultiStateThreshold = new IconMultiStateThreshold();
		CopyTo(iconMultiStateThreshold);
		iconMultiStateThreshold.equals = equals;
		return iconMultiStateThreshold;
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
		out1.WriteByte(equals);
		out1.WriteInt(0);
	}
}
