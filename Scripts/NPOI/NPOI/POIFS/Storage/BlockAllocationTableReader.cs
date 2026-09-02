using System;
using System.Collections.Generic;
using System.IO;
using NPOI.POIFS.Common;
using NPOI.Util;

namespace NPOI.POIFS.Storage;

public class BlockAllocationTableReader
{
	private static POILogger _logger = POILogFactory.GetLogger(typeof(BlockAllocationTableReader));

	private const int MAX_BLOCK_COUNT = 65535;

	private List<int> _entries;

	private POIFSBigBlockSize bigBlockSize;

	public BlockAllocationTableReader(POIFSBigBlockSize bigBlockSizse, int block_count, int[] block_array, int xbat_count, int xbat_index, BlockList raw_block_list)
		: this(bigBlockSizse)
	{
		SanityCheckBlockCount(block_count);
		RawDataBlock[] array = new RawDataBlock[block_count];
		int num = Math.Min(block_count, block_array.Length);
		int i;
		for (i = 0; i < num; i++)
		{
			int num2 = block_array[i];
			if (num2 > raw_block_list.BlockCount())
			{
				throw new IOException("Your file contains " + raw_block_list.BlockCount() + " sectors, but the initial DIFAT array at index " + i + " referenced block # " + num2 + ". This isn't allowed and  your file is corrupt");
			}
			array[i] = (RawDataBlock)raw_block_list.Remove(num2);
		}
		if (i < block_count)
		{
			if (xbat_index < 0)
			{
				throw new IOException("BAT count exceeds limit, yet XBAT index indicates no valid entries");
			}
			int num3 = xbat_index;
			int entriesPerXBATBlock = BATBlock.EntriesPerXBATBlock;
			int xBATChainOffset = BATBlock.XBATChainOffset;
			for (int j = 0; j < xbat_count; j++)
			{
				num = Math.Min(block_count - i, entriesPerXBATBlock);
				byte[] data = raw_block_list.Remove(num3).Data;
				int num4 = 0;
				for (int k = 0; k < num; k++)
				{
					array[i++] = (RawDataBlock)raw_block_list.Remove(LittleEndian.GetInt(data, num4));
					num4 += 4;
				}
				num3 = LittleEndian.GetInt(data, xBATChainOffset);
				if (num3 == -2)
				{
					break;
				}
			}
		}
		if (i != block_count)
		{
			throw new IOException("Could not find all blocks");
		}
		ListManagedBlock[] blocks = array;
		SetEntries(blocks, raw_block_list);
	}

	public BlockAllocationTableReader(POIFSBigBlockSize bigBlockSize, ListManagedBlock[] blocks, BlockList raw_block_list)
		: this(bigBlockSize)
	{
		SetEntries(blocks, raw_block_list);
	}

	public BlockAllocationTableReader(POIFSBigBlockSize bigBlockSize)
	{
		this.bigBlockSize = bigBlockSize;
		_entries = new List<int>();
	}

	public ListManagedBlock[] FetchBlocks(int startBlock, int headerPropertiesStartBlock, BlockList blockList)
	{
		List<ListManagedBlock> list = new List<ListManagedBlock>();
		int num = startBlock;
		bool flag = true;
		ListManagedBlock listManagedBlock = null;
		while (num != -2)
		{
			try
			{
				listManagedBlock = blockList.Remove(num);
				list.Add(listManagedBlock);
				num = _entries[num];
				flag = false;
			}
			catch (Exception)
			{
				if (num == headerPropertiesStartBlock)
				{
					_logger.Log(5, "Warning, header block comes after data blocks in POIFS block listing");
					num = -2;
					continue;
				}
				if ((num == 0) & flag)
				{
					_logger.Log(5, "Warning, incorrectly terminated empty data blocks in POIFS block listing (should end at -2, ended at 0)");
					num = -2;
					continue;
				}
				throw;
			}
		}
		return list.ToArray();
	}

	public bool IsUsed(int index)
	{
		bool result = false;
		try
		{
			result = _entries[index] != -1;
		}
		catch (IndexOutOfRangeException)
		{
		}
		return result;
	}

	public int GetNextBlockIndex(int index)
	{
		if (IsUsed(index))
		{
			return _entries[index];
		}
		throw new IOException("index " + index + " is unused");
	}

	private void SetEntries(ListManagedBlock[] blocks, BlockList raw_blocks)
	{
		int bATEntriesPerBlock = bigBlockSize.GetBATEntriesPerBlock();
		for (int i = 0; i < blocks.Length; i++)
		{
			byte[] data = blocks[i].Data;
			int num = 0;
			for (int j = 0; j < bATEntriesPerBlock; j++)
			{
				int num2 = LittleEndian.GetInt(data, num);
				if (num2 == -1)
				{
					raw_blocks.Zap(_entries.Count);
				}
				_entries.Add(num2);
				num += 4;
			}
			blocks[i] = null;
		}
		raw_blocks.BAT = this;
	}

	public static void SanityCheckBlockCount(int block_count)
	{
		if (block_count <= 0)
		{
			throw new IOException("Illegal block count; minimum count is 1, got " + block_count + " instead");
		}
		if (block_count > 65535)
		{
			throw new IOException("Block count " + block_count + " is too high. POI maximum is " + 65535 + ".");
		}
	}
}
