using System;
using System.IO;
using NPOI.POIFS.Common;
using NPOI.Util;

namespace NPOI.POIFS.Storage;

public class HeaderBlockWriter : HeaderBlockConstants, BlockWritable
{
	private HeaderBlock _header_block;

	public int PropertyStart
	{
		get
		{
			return _header_block.PropertyStart;
		}
		set
		{
			_header_block.PropertyStart = value;
		}
	}

	public int SBAStart
	{
		get
		{
			return _header_block.SBATStart;
		}
		set
		{
			_header_block.SBATStart = value;
		}
	}

	public int SBATStart
	{
		get
		{
			return _header_block.SBATStart;
		}
		set
		{
			_header_block.SBATStart = value;
		}
	}

	public int SBATBlockCount
	{
		get
		{
			return _header_block.SBATBlockCount;
		}
		set
		{
			_header_block.SBATBlockCount = value;
		}
	}

	public HeaderBlockWriter(POIFSBigBlockSize bigBlockSize)
	{
		_header_block = new HeaderBlock(bigBlockSize);
	}

	public HeaderBlockWriter(HeaderBlock headerBlock)
	{
		_header_block = headerBlock;
	}

	public BATBlock[] SetBATBlocks(int blockCount, int startBlock)
	{
		POIFSBigBlockSize bigBlockSize = _header_block.BigBlockSize;
		_header_block.BATCount = blockCount;
		int num = Math.Min(blockCount, 109);
		int[] array = new int[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = startBlock + i;
		}
		_header_block.BATArray = array;
		BATBlock[] array3;
		if (blockCount > 109)
		{
			int num2 = blockCount - 109;
			int[] array2 = new int[num2];
			for (int j = 0; j < num2; j++)
			{
				array2[j] = startBlock + j + 109;
			}
			array3 = BATBlock.CreateXBATBlocks(bigBlockSize, array2, startBlock + blockCount);
			_header_block.XBATStart = startBlock + blockCount;
		}
		else
		{
			array3 = BATBlock.CreateXBATBlocks(bigBlockSize, new int[0], 0);
			_header_block.XBATStart = -2;
		}
		_header_block.XBATCount = array3.Length;
		return array3;
	}

	public static int CalculateXBATStorageRequirements(POIFSBigBlockSize bigBlockSize, int blockCount)
	{
		if (blockCount <= 109)
		{
			return 0;
		}
		return BATBlock.CalculateXBATStorageRequirements(bigBlockSize, blockCount - 109);
	}

	public void WriteBlocks(Stream stream)
	{
		try
		{
			_header_block.WriteData(stream);
		}
		catch (IOException ex)
		{
			throw ex;
		}
	}

	public void WriteBlock(ByteBuffer block)
	{
		MemoryStream memoryStream = new MemoryStream(_header_block.BigBlockSize.GetBigBlockSize());
		_header_block.WriteData(memoryStream);
		block.Write(memoryStream.ToArray());
	}

	public void WriteBlock(byte[] block)
	{
		MemoryStream memoryStream = new MemoryStream(_header_block.BigBlockSize.GetBigBlockSize());
		_header_block.WriteData(memoryStream);
		byte[] array = memoryStream.ToArray();
		Array.Copy(array, 0, block, 0, array.Length);
	}
}
