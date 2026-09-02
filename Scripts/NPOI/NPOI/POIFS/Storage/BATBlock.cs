using System;
using System.Collections.Generic;
using System.IO;
using NPOI.POIFS.Common;
using NPOI.Util;

namespace NPOI.POIFS.Storage;

public class BATBlock : BigBlock
{
	private static int _entries_per_block = 128;

	private static int _entries_per_xbat_block = _entries_per_block - 1;

	private static int _xbat_chain_offset = _entries_per_xbat_block * 4;

	private static byte _default_value = byte.MaxValue;

	private IntegerField[] _fields;

	private byte[] _data;

	private int[] _values;

	private bool _has_free_sectors;

	private int ourBlockIndex;

	public static int EntriesPerBlock => _entries_per_block;

	public static int EntriesPerXBATBlock => _entries_per_xbat_block;

	public static int XBATChainOffset => _xbat_chain_offset;

	public bool HasFreeSectors => _has_free_sectors;

	public int OurBlockIndex
	{
		get
		{
			return ourBlockIndex;
		}
		set
		{
			ourBlockIndex = value;
		}
	}

	protected BATBlock()
	{
		_data = new byte[512];
		for (int i = 0; i < _data.Length; i++)
		{
			_data[i] = _default_value;
		}
		_fields = new IntegerField[_entries_per_block];
		int num = 0;
		for (int j = 0; j < _entries_per_block; j++)
		{
			_fields[j] = new IntegerField(num);
			num += 4;
		}
	}

	protected BATBlock(POIFSBigBlockSize bigBlockSize)
		: base(bigBlockSize)
	{
		int bATEntriesPerBlock = bigBlockSize.GetBATEntriesPerBlock();
		_values = new int[bATEntriesPerBlock];
		_has_free_sectors = true;
		for (int i = 0; i < _values.Length; i++)
		{
			_values[i] = -1;
		}
	}

	protected BATBlock(POIFSBigBlockSize bigBlockSize, int[] entries, int start_index, int end_index)
		: this(bigBlockSize)
	{
		for (int i = start_index; i < end_index; i++)
		{
			_values[i - start_index] = entries[i];
		}
		if (end_index - start_index == _values.Length)
		{
			RecomputeFree();
		}
	}

	private void RecomputeFree()
	{
		bool has_free_sectors = false;
		for (int i = 0; i < _values.Length; i++)
		{
			if (_values[i] == -1)
			{
				has_free_sectors = true;
				break;
			}
		}
		_has_free_sectors = has_free_sectors;
	}

	public static BATBlock CreateBATBlock(POIFSBigBlockSize bigBlockSize, BinaryReader data)
	{
		BATBlock bATBlock = new BATBlock(bigBlockSize);
		byte[] array = new byte[4];
		for (int i = 0; i < bATBlock._values.Length; i++)
		{
			data.Read(array, 0, array.Length);
			bATBlock._values[i] = LittleEndian.GetInt(array);
		}
		bATBlock.RecomputeFree();
		return bATBlock;
	}

	public static BATBlock CreateBATBlock(POIFSBigBlockSize bigBlockSize, ByteBuffer data)
	{
		BATBlock bATBlock = new BATBlock(bigBlockSize);
		byte[] array = new byte[4];
		for (int i = 0; i < bATBlock._values.Length; i++)
		{
			data.Read(array);
			bATBlock._values[i] = LittleEndian.GetInt(array);
		}
		bATBlock.RecomputeFree();
		return bATBlock;
	}

	public static BATBlock CreateEmptyBATBlock(POIFSBigBlockSize bigBlockSize, bool isXBAT)
	{
		BATBlock bATBlock = new BATBlock(bigBlockSize);
		if (isXBAT)
		{
			bATBlock.SetXBATChain(bigBlockSize, -2);
		}
		return bATBlock;
	}

	public static BATBlock[] CreateBATBlocks(POIFSBigBlockSize bigBlockSize, int[] entries)
	{
		BATBlock[] array = new BATBlock[CalculateStorageRequirements(entries.Length)];
		int num = 0;
		int num2 = entries.Length;
		for (int i = 0; i < entries.Length; i += _entries_per_block)
		{
			array[num++] = new BATBlock(bigBlockSize, entries, i, (num2 > _entries_per_block) ? (i + _entries_per_block) : entries.Length);
			num2 -= _entries_per_block;
		}
		return array;
	}

	public static BATBlock[] CreateXBATBlocks(POIFSBigBlockSize bigBlockSize, int[] entries, int startBlock)
	{
		int num = CalculateXBATStorageRequirements(entries.Length);
		BATBlock[] array = new BATBlock[num];
		int num2 = 0;
		int num3 = entries.Length;
		if (num != 0)
		{
			for (int i = 0; i < entries.Length; i += _entries_per_xbat_block)
			{
				array[num2++] = new BATBlock(bigBlockSize, entries, i, (num3 > _entries_per_xbat_block) ? (i + _entries_per_xbat_block) : entries.Length);
				num3 -= _entries_per_xbat_block;
			}
			for (num2 = 0; num2 < array.Length - 1; num2++)
			{
				array[num2].SetXBATChain(bigBlockSize, startBlock + num2 + 1);
			}
			array[num2].SetXBATChain(bigBlockSize, -2);
		}
		return array;
	}

	public static int CalculateStorageRequirements(int entryCount)
	{
		return (entryCount + _entries_per_block - 1) / _entries_per_block;
	}

	public static int CalculateStorageRequirements(POIFSBigBlockSize bigBlockSize, int entryCount)
	{
		int bATEntriesPerBlock = bigBlockSize.GetBATEntriesPerBlock();
		return (entryCount + bATEntriesPerBlock - 1) / bATEntriesPerBlock;
	}

	public static int CalculateXBATStorageRequirements(int entryCount)
	{
		return (entryCount + _entries_per_xbat_block - 1) / _entries_per_xbat_block;
	}

	public static int CalculateXBATStorageRequirements(POIFSBigBlockSize bigBlockSize, int entryCount)
	{
		int xBATEntriesPerBlock = bigBlockSize.GetXBATEntriesPerBlock();
		return (entryCount + xBATEntriesPerBlock - 1) / xBATEntriesPerBlock;
	}

	public static long CalculateMaximumSize(POIFSBigBlockSize bigBlockSize, int numBATs)
	{
		return (1 + (long)numBATs * (long)bigBlockSize.GetBATEntriesPerBlock()) * bigBlockSize.GetBigBlockSize();
	}

	public static long CalculateMaximumSize(HeaderBlock header)
	{
		return CalculateMaximumSize(header.BigBlockSize, header.BATCount);
	}

	public static BATBlockAndIndex GetBATBlockAndIndex(int offset, HeaderBlock header, List<BATBlock> bats)
	{
		int bATEntriesPerBlock = header.BigBlockSize.GetBATEntriesPerBlock();
		int index = (int)Math.Floor(1.0 * (double)offset / (double)bATEntriesPerBlock);
		return new BATBlockAndIndex(offset % bATEntriesPerBlock, bats[index]);
	}

	public static BATBlockAndIndex GetSBATBlockAndIndex(int offset, HeaderBlock header, List<BATBlock> sbats)
	{
		int bATEntriesPerBlock = header.BigBlockSize.GetBATEntriesPerBlock();
		int index = (int)Math.Floor(1.0 * (double)offset / (double)bATEntriesPerBlock);
		return new BATBlockAndIndex(offset % bATEntriesPerBlock, sbats[index]);
	}

	private void SetXBATChain(int chainIndex)
	{
		_fields[_entries_per_xbat_block].Set(chainIndex, _data);
	}

	private void SetXBATChain(POIFSBigBlockSize bigBlockSize, int chainIndex)
	{
		int xBATEntriesPerBlock = bigBlockSize.GetXBATEntriesPerBlock();
		_values[xBATEntriesPerBlock] = chainIndex;
	}

	public int GetUsedSectors(bool isAnXBAT)
	{
		int num = 0;
		int num2 = _values.Length;
		if (isAnXBAT)
		{
			num2--;
		}
		for (int i = 0; i < num2; i++)
		{
			if (_values[i] != -1)
			{
				num++;
			}
		}
		return num;
	}

	public int GetValueAt(int relativeOffset)
	{
		if (relativeOffset >= _values.Length)
		{
			throw new IndexOutOfRangeException("Unable to fetch offset " + relativeOffset + " as the BAT only contains " + _values.Length + " entries");
		}
		return _values[relativeOffset];
	}

	public void SetValueAt(int relativeOffset, int value)
	{
		int num = _values[relativeOffset];
		_values[relativeOffset] = value;
		if (value == -1)
		{
			_has_free_sectors = true;
		}
		else if (num == -1)
		{
			RecomputeFree();
		}
	}

	private BATBlock(int[] entries, int start_index, int end_index)
		: this()
	{
		for (int i = start_index; i < end_index; i++)
		{
			_fields[i - start_index].Set(entries[i], _data);
		}
	}

	public void WriteData(ByteBuffer block)
	{
		block.Write(Serialize());
	}

	public override void WriteData(Stream stream)
	{
		byte[] array = Serialize();
		stream.Write(array, 0, array.Length);
	}

	public void WriteData(byte[] block)
	{
		byte[] array = Serialize();
		for (int i = 0; i < array.Length; i++)
		{
			block[i] = array[i];
		}
	}

	private byte[] Serialize()
	{
		byte[] array = new byte[bigBlockSize.GetBigBlockSize()];
		int num = 0;
		for (int i = 0; i < _values.Length; i++)
		{
			LittleEndian.PutInt(array, num, _values[i]);
			num += 4;
		}
		return array;
	}
}
