using System;
using System.IO;

namespace NPOI.POIFS.Storage;

public class BlockListImpl : BlockList
{
	private ListManagedBlock[] _blocks;

	private BlockAllocationTableReader _bat;

	public virtual BlockAllocationTableReader BAT
	{
		set
		{
			if (_bat != null)
			{
				throw new IOException("Attempt to replace existing BlockAllocationTable");
			}
			_bat = value;
		}
	}

	public BlockListImpl()
	{
		_blocks = new ListManagedBlock[0];
		_bat = null;
	}

	public virtual void SetBlocks(ListManagedBlock[] blocks)
	{
		_blocks = (ListManagedBlock[])blocks.Clone();
	}

	public virtual void Zap(int index)
	{
		if (index >= 0 && index < _blocks.Length)
		{
			_blocks[index] = null;
		}
	}

	protected ListManagedBlock Get(int index)
	{
		return _blocks[index];
	}

	public virtual ListManagedBlock Remove(int index)
	{
		ListManagedBlock listManagedBlock = null;
		try
		{
			listManagedBlock = _blocks[index];
			if (listManagedBlock == null)
			{
				throw new IOException("block[ " + index + " ] already removed");
			}
			_blocks[index] = null;
			return listManagedBlock;
		}
		catch (IndexOutOfRangeException)
		{
			throw new IOException("Cannot remove block[ " + index + " ]; out of range[ 0 - " + (_blocks.Length - 1) + " ]");
		}
	}

	public virtual ListManagedBlock[] FetchBlocks(int startBlock, int headerPropertiesStartBlock)
	{
		if (_bat == null)
		{
			throw new IOException("Improperly initialized list: no block allocation table provided");
		}
		return _bat.FetchBlocks(startBlock, headerPropertiesStartBlock, this);
	}

	public virtual int BlockCount()
	{
		return _blocks.Length;
	}

	protected int RemainingBlocks()
	{
		int num = 0;
		for (int i = 0; i < _blocks.Length; i++)
		{
			if (_blocks[i] != null)
			{
				num++;
			}
		}
		return num;
	}
}
