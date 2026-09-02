using System.Collections.Generic;
using System.IO;
using NPOI.POIFS.Common;
using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI.POIFS.Storage;

public class BlockAllocationTableWriter : BlockWritable, BATManaged
{
	private List<int> _entries;

	private BATBlock[] _blocks;

	private int _start_block;

	private POIFSBigBlockSize _bigBlockSize;

	private static int _default_size = 128;

	public int StartBlock
	{
		get
		{
			return _start_block;
		}
		set
		{
			_start_block = value;
		}
	}

	public int CountBlocks => _blocks.Length;

	public BlockAllocationTableWriter(POIFSBigBlockSize bigBlockSize)
	{
		_start_block = -2;
		_entries = new List<int>(_default_size);
		_blocks = new BATBlock[0];
		_bigBlockSize = bigBlockSize;
	}

	public int CreateBlocks()
	{
		int num = 0;
		int num2 = 0;
		while (true)
		{
			int num3 = BATBlock.CalculateStorageRequirements(_bigBlockSize, num2 + num + _entries.Count);
			int num4 = HeaderBlockWriter.CalculateXBATStorageRequirements(_bigBlockSize, num3);
			if (num2 == num3 && num == num4)
			{
				break;
			}
			num2 = num3;
			num = num4;
		}
		int result = AllocateSpace(num2);
		AllocateSpace(num);
		SimpleCreateBlocks();
		return result;
	}

	public int AllocateSpace(int blockCount)
	{
		int count = _entries.Count;
		if (blockCount > 0)
		{
			int num = blockCount - 1;
			int num2 = count + 1;
			for (int i = 0; i < num; i++)
			{
				_entries.Add(num2++);
			}
			_entries.Add(-2);
		}
		return count;
	}

	internal void SimpleCreateBlocks()
	{
		_blocks = BATBlock.CreateBATBlocks(_bigBlockSize, _entries.ToArray());
	}

	public void WriteBlocks(Stream stream)
	{
		for (int i = 0; i < _blocks.Length; i++)
		{
			_blocks[i].WriteBlocks(stream);
		}
	}

	public static void WriteBlock(BATBlock bat, ByteBuffer block)
	{
		bat.WriteData(block);
	}
}
