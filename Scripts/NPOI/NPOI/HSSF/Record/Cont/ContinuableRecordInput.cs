using NPOI.Util;

namespace NPOI.HSSF.Record.Cont;

public class ContinuableRecordInput : ILittleEndianInput
{
	private RecordInputStream _in;

	public ContinuableRecordInput(RecordInputStream in1)
	{
		_in = in1;
	}

	public int Available()
	{
		return _in.Available();
	}

	public int ReadByte()
	{
		return _in.ReadByte();
	}

	public int ReadUByte()
	{
		return _in.ReadUByte();
	}

	public short ReadShort()
	{
		return _in.ReadShort();
	}

	public int ReadUShort()
	{
		int num = ReadUByte();
		return (ReadUByte() << 8) + num;
	}

	public int ReadInt()
	{
		int num = _in.ReadUByte();
		int num2 = _in.ReadUByte();
		int num3 = _in.ReadUByte();
		return (_in.ReadUByte() << 24) + (num3 << 16) + (num2 << 8) + num;
	}

	public long ReadLong()
	{
		int num = _in.ReadUByte();
		int num2 = _in.ReadUByte();
		int num3 = _in.ReadUByte();
		int num4 = _in.ReadUByte();
		int num5 = _in.ReadUByte();
		int num6 = _in.ReadUByte();
		int num7 = _in.ReadUByte();
		return ((long)_in.ReadUByte() << 56) + ((long)num7 << 48) + ((long)num6 << 40) + ((long)num5 << 32) + ((long)num4 << 24) + (num3 << 16) + (num2 << 8) + num;
	}

	public double ReadDouble()
	{
		return _in.ReadDouble();
	}

	public void ReadFully(byte[] buf)
	{
		_in.ReadFully(buf);
	}

	public void ReadFully(byte[] buf, int off, int len)
	{
		_in.ReadFully(buf, off, len);
	}
}
