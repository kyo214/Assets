using System;
using System.IO;
using NPOI.POIFS.Common;
using NPOI.Util;

namespace NPOI.POIFS.Storage;

public class DocumentBlock : BigBlock
{
	private static byte _default_value = byte.MaxValue;

	private byte[] _data;

	private int _bytes_Read;

	public int Size => _bytes_Read;

	public bool PartiallyRead => _bytes_Read != 512;

	public static byte FillByte => _default_value;

	public DocumentBlock(RawDataBlock block)
		: base((block.BigBlockSize == 512) ? POIFSConstants.SMALLER_BIG_BLOCK_SIZE_DETAILS : POIFSConstants.LARGER_BIG_BLOCK_SIZE_DETAILS)
	{
		_data = block.Data;
		_bytes_Read = _data.Length;
	}

	public DocumentBlock(Stream stream, POIFSBigBlockSize bigBlockSize)
		: this(bigBlockSize)
	{
		int num = IOUtils.ReadFully(stream, _data);
		_bytes_Read = ((num != -1) ? num : 0);
	}

	public DocumentBlock(POIFSBigBlockSize bigBlockSize)
		: base(bigBlockSize)
	{
		_data = new byte[512];
		Arrays.Fill(_data, _default_value);
	}

	public static DocumentBlock[] Convert(POIFSBigBlockSize bigBlockSize, byte[] array, int size)
	{
		DocumentBlock[] array2 = new DocumentBlock[(size + 512 - 1) / 512];
		int num = 0;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = new DocumentBlock(bigBlockSize);
			if (num < array.Length)
			{
				int num2 = Math.Min(512, array.Length - num);
				Array.Copy(array, num, array2[i]._data, 0, num2);
				if (num2 != 512)
				{
					for (int j = ((num2 > 0) ? (num2 - 1) : num2); j < 512; j++)
					{
						array2[i]._data[j] = _default_value;
					}
				}
			}
			else
			{
				for (int k = 0; k < array2[i]._data.Length; k++)
				{
					array2[i]._data[k] = _default_value;
				}
			}
			num += 512;
		}
		return array2;
	}

	public static void Read(DocumentBlock[] blocks, byte[] buffer, int offset)
	{
		int num = offset / 512;
		int num2 = offset % 512;
		int num3 = (offset + buffer.Length - 1) / 512;
		if (num == num3)
		{
			Array.Copy(blocks[num]._data, num2, buffer, 0, buffer.Length);
			return;
		}
		int num4 = 0;
		Array.Copy(blocks[num]._data, num2, buffer, num4, 512 - num2);
		num4 += 512 - num2;
		for (int i = num + 1; i < num3; i++)
		{
			Array.Copy(blocks[i]._data, 0, buffer, num4, 512);
			num4 += 512;
		}
		Array.Copy(blocks[num3]._data, 0, buffer, num4, buffer.Length - num4);
	}

	public static DataInputBlock GetDataInputBlock(DocumentBlock[] blocks, int offset)
	{
		if (blocks == null || blocks.Length == 0)
		{
			return null;
		}
		POIFSBigBlockSize pOIFSBigBlockSize = blocks[0].bigBlockSize;
		int headerValue = pOIFSBigBlockSize.GetHeaderValue();
		int num = pOIFSBigBlockSize.GetBigBlockSize() - 1;
		int num2 = offset >> headerValue;
		int startOffset = offset & num;
		return new DataInputBlock(blocks[num2]._data, startOffset);
	}

	public override void WriteData(Stream stream)
	{
		WriteData(stream, _data);
	}
}
