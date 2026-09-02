using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class DConRefRecord : StandardRecord
{
	public const short sid = 81;

	private int firstRow;

	private int lastRow;

	private int firstCol;

	private int lastCol;

	private int charCount;

	private int charType;

	private byte[] path;

	private byte[] _unused;

	protected override int DataSize
	{
		get
		{
			int num = 9 + path.Length;
			if (path[0] == 2)
			{
				num += _unused.Length;
			}
			return num;
		}
	}

	public override short Sid => 81;

	public int FirstColumn => firstCol;

	public int FirstRow => firstRow;

	public int LastColumn => lastCol;

	public int LastRow => lastRow;

	public string ReadablePath
	{
		get
		{
			if (path != null)
			{
				int i;
				for (i = 1; path[i] < 32 && i < path.Length; i++)
				{
				}
				return Encoding.UTF8.GetString(Arrays.CopyOfRange(path, i, path.Length)).Replace("\u0003", "/");
			}
			return null;
		}
	}

	public bool IsExternalRef
	{
		get
		{
			if (path[0] == 1)
			{
				return true;
			}
			return false;
		}
	}

	public DConRefRecord(byte[] data)
	{
		int num = 0;
		if (LittleEndian.GetShort(data, num) != 81)
		{
			throw new RecordFormatException("incompatible sid.");
		}
		num += 2;
		num += 2;
		firstRow = LittleEndian.GetUShort(data, num);
		num += 2;
		lastRow = LittleEndian.GetUShort(data, num);
		num += 2;
		firstCol = LittleEndian.GetUByte(data, num);
		num++;
		lastCol = LittleEndian.GetUByte(data, num);
		num++;
		charCount = LittleEndian.GetUShort(data, num);
		num += 2;
		if (charCount < 2)
		{
			throw new RecordFormatException("Character count must be >= 2");
		}
		charType = LittleEndian.GetUByte(data, num);
		num++;
		int num2 = charCount * ((charType & 1) + 1);
		path = LittleEndian.GetByteArray(data, num, num2);
		num += num2;
		if (path[0] == 2)
		{
			_unused = LittleEndian.GetByteArray(data, num, charType + 1);
		}
	}

	public DConRefRecord(RecordInputStream inStream)
	{
		if (inStream.Sid != 81)
		{
			throw new RecordFormatException("Wrong sid: " + inStream.Sid);
		}
		firstRow = inStream.ReadUShort();
		lastRow = inStream.ReadUShort();
		firstCol = inStream.ReadUByte();
		lastCol = inStream.ReadUByte();
		charCount = inStream.ReadUShort();
		charType = inStream.ReadUByte() & 1;
		int num = charCount * (charType + 1);
		path = new byte[num];
		inStream.ReadFully(path);
		if (path[0] == 2)
		{
			_unused = inStream.ReadRemainder();
		}
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(firstRow);
		out1.WriteShort(lastRow);
		out1.WriteByte(firstCol);
		out1.WriteByte(lastCol);
		out1.WriteShort(charCount);
		out1.WriteByte(charType);
		out1.Write(path);
		if (path[0] == 2)
		{
			out1.Write(_unused);
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[DCONREF]\n");
		stringBuilder.Append("    .ref\n");
		stringBuilder.Append("        .firstrow   = ").Append(firstRow).Append("\n");
		stringBuilder.Append("        .lastrow    = ").Append(lastRow).Append("\n");
		stringBuilder.Append("        .firstcol   = ").Append(firstCol).Append("\n");
		stringBuilder.Append("        .lastcol    = ").Append(lastCol).Append("\n");
		stringBuilder.Append("    .cch            = ").Append(charCount).Append("\n");
		stringBuilder.Append("    .stFile\n");
		stringBuilder.Append("        .h          = ").Append(charType).Append("\n");
		stringBuilder.Append("        .rgb        = ").Append(ReadablePath).Append("\n");
		stringBuilder.Append("[/DCONREF]\n");
		return stringBuilder.ToString();
	}

	public byte[] GetPath()
	{
		return Arrays.CopyOf(path, path.Length);
	}
}
