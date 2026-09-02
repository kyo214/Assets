using System;
using System.IO;
using System.Text;
using NPOI.Util;

namespace NPOI.POIFS.FileSystem;

public class Ole10Native
{
	private enum EncodingMode
	{
		parsed = 0,
		unparsed = 1,
		compact = 2
	}

	public static string OLE10_NATIVE = "\u0001Ole10Native";

	protected static string ISO1 = "ISO-8859-1";

	private int totalSize;

	private short flags1 = 2;

	private string label;

	private string fileName;

	private short flags2;

	private short unknown1 = 3;

	private string command;

	private byte[] dataBuffer;

	private short flags3;

	private EncodingMode mode;

	public int TotalSize => totalSize;

	public short Flags1
	{
		get
		{
			return flags1;
		}
		set
		{
			flags1 = value;
		}
	}

	public string Label
	{
		get
		{
			return label;
		}
		set
		{
			label = value;
		}
	}

	public string FileName
	{
		get
		{
			return fileName;
		}
		set
		{
			fileName = value;
		}
	}

	public short Flags2
	{
		get
		{
			return flags2;
		}
		set
		{
			flags2 = value;
		}
	}

	public short Unknown1
	{
		get
		{
			return unknown1;
		}
		set
		{
			unknown1 = value;
		}
	}

	public string Command
	{
		get
		{
			return command;
		}
		set
		{
			command = value;
		}
	}

	public int DataSize => dataBuffer.Length;

	public byte[] DataBuffer
	{
		get
		{
			return dataBuffer;
		}
		set
		{
			dataBuffer = (byte[])value.Clone();
		}
	}

	public short Flags3
	{
		get
		{
			return flags3;
		}
		set
		{
			flags3 = value;
		}
	}

	public static Ole10Native CreateFromEmbeddedOleObject(POIFSFileSystem poifs)
	{
		return CreateFromEmbeddedOleObject(poifs.Root);
	}

	public static Ole10Native CreateFromEmbeddedOleObject(DirectoryNode directory)
	{
		DocumentEntry documentEntry = (DocumentEntry)directory.GetEntry(OLE10_NATIVE);
		byte[] array = new byte[documentEntry.Size];
		directory.CreateDocumentInputStream(documentEntry).Read(array);
		return new Ole10Native(array, 0);
	}

	public Ole10Native(string label, string filename, string command, byte[] data)
	{
		Label = label;
		FileName = filename;
		Command = command;
		DataBuffer = data;
		mode = EncodingMode.parsed;
	}

	public Ole10Native(byte[] data, int offset)
	{
		int num = offset;
		if (data.Length < offset + 2)
		{
			throw new Ole10NativeException("data is too small");
		}
		totalSize = LittleEndian.GetInt(data, num);
		num += 4;
		mode = EncodingMode.unparsed;
		if (LittleEndian.GetShort(data, num) == 2)
		{
			if (char.IsControl((char)data[num + 2]))
			{
				mode = EncodingMode.compact;
			}
			else
			{
				mode = EncodingMode.parsed;
			}
		}
		int num2 = 0;
		switch (mode)
		{
		case EncodingMode.parsed:
		{
			flags1 = LittleEndian.GetShort(data, num);
			num += 2;
			int stringLength = GetStringLength(data, num);
			label = StringUtil.GetFromCompressedUnicode(data, num, stringLength - 1);
			num += stringLength;
			stringLength = GetStringLength(data, num);
			fileName = StringUtil.GetFromCompressedUnicode(data, num, stringLength - 1);
			num += stringLength;
			flags2 = LittleEndian.GetShort(data, num);
			num += 2;
			unknown1 = LittleEndian.GetShort(data, num);
			num += 2;
			stringLength = LittleEndian.GetInt(data, num);
			num += 4;
			command = StringUtil.GetFromCompressedUnicode(data, num, stringLength - 1);
			num += stringLength;
			if (totalSize < num)
			{
				throw new Ole10NativeException("Invalid Ole10Native");
			}
			num2 = LittleEndian.GetInt(data, num);
			num += 4;
			if (num2 < 0 || totalSize - (num - 4) < num2)
			{
				throw new Ole10NativeException("Invalid Ole10Native");
			}
			break;
		}
		case EncodingMode.compact:
			flags1 = LittleEndian.GetShort(data, num);
			num += 2;
			num2 = totalSize - 2;
			break;
		case EncodingMode.unparsed:
			num2 = totalSize;
			break;
		}
		dataBuffer = new byte[num2];
		Array.Copy(data, num, dataBuffer, 0, num2);
		num += num2;
	}

	private static int GetStringLength(byte[] data, int ofs)
	{
		int i;
		for (i = 0; i + ofs < data.Length && data[ofs + i] != 0; i++)
		{
		}
		return i + 1;
	}

	public void WriteOut(Stream out1)
	{
		_ = new byte[4];
		_ = new byte[2];
		_ = new byte[4];
		LittleEndianOutputStream littleEndianOutputStream = new LittleEndianOutputStream(out1);
		switch (mode)
		{
		case EncodingMode.parsed:
		{
			MemoryStream memoryStream = new MemoryStream();
			LittleEndianOutputStream littleEndianOutputStream2 = new LittleEndianOutputStream(memoryStream);
			littleEndianOutputStream2.WriteShort(Flags1);
			littleEndianOutputStream2.Write(Encoding.GetEncoding(ISO1).GetBytes(Label));
			littleEndianOutputStream2.WriteByte(0);
			littleEndianOutputStream2.Write(Encoding.GetEncoding(ISO1).GetBytes(FileName));
			littleEndianOutputStream2.WriteByte(0);
			littleEndianOutputStream2.WriteShort(Flags2);
			littleEndianOutputStream2.WriteShort(Unknown1);
			littleEndianOutputStream2.WriteInt(Command.Length + 1);
			littleEndianOutputStream2.Write(Encoding.GetEncoding(ISO1).GetBytes(Command));
			littleEndianOutputStream2.WriteByte(0);
			littleEndianOutputStream2.WriteInt(DataSize);
			littleEndianOutputStream2.Write(DataBuffer);
			littleEndianOutputStream2.WriteShort(Flags3);
			littleEndianOutputStream.WriteInt((int)memoryStream.Length);
			memoryStream.WriteTo(out1);
			break;
		}
		case EncodingMode.compact:
			littleEndianOutputStream.WriteInt(DataSize + 2);
			littleEndianOutputStream.WriteShort(Flags1);
			out1.Write(DataBuffer, 0, DataBuffer.Length);
			break;
		default:
			littleEndianOutputStream.WriteInt(DataSize);
			out1.Write(DataBuffer, 0, DataBuffer.Length);
			break;
		}
	}
}
