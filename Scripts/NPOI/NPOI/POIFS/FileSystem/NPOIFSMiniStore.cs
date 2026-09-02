using System;
using System.Collections.Generic;
using NPOI.POIFS.Properties;
using NPOI.POIFS.Storage;
using NPOI.Util;

namespace NPOI.POIFS.FileSystem;

public class NPOIFSMiniStore : BlockStore
{
	private NPOIFSFileSystem _filesystem;

	private NPOIFSStream _mini_stream;

	private List<BATBlock> _sbat_blocks;

	private HeaderBlock _header;

	private RootProperty _root;

	public NPOIFSMiniStore(NPOIFSFileSystem filesystem, RootProperty root, List<BATBlock> sbats, HeaderBlock header)
	{
		_filesystem = filesystem;
		_sbat_blocks = sbats;
		_header = header;
		_root = root;
		_mini_stream = new NPOIFSStream(filesystem, root.StartBlock);
	}

	private ByteBuffer GetBlockAt(int offset, bool throwIfNotFound)
	{
		int num = offset * 64;
		int num2 = num / _filesystem.GetBigBlockSize();
		int num3 = num % _filesystem.GetBigBlockSize();
		NPOIFSStream.StreamBlockByteBufferIterator streamBlockByteBufferIterator = _mini_stream.GetBlockIterator() as NPOIFSStream.StreamBlockByteBufferIterator;
		for (int i = 0; i < num2; i++)
		{
			streamBlockByteBufferIterator.Next();
		}
		if (!streamBlockByteBufferIterator.HasNext())
		{
			if (throwIfNotFound)
			{
				throw new IndexOutOfRangeException("Big block " + num2 + " outside stream");
			}
			return null;
		}
		ByteBuffer byteBuffer = streamBlockByteBufferIterator.Next();
		byteBuffer.Position += num3;
		ByteBuffer byteBuffer2 = byteBuffer.Slice();
		byteBuffer2.Limit = 64;
		return byteBuffer2;
	}

	public override ByteBuffer GetBlockAt(int offset)
	{
		return GetBlockAt(offset, throwIfNotFound: true);
	}

	public override bool TryGetBlockAt(int offset, out ByteBuffer byteBuffer)
	{
		byteBuffer = null;
		try
		{
			byteBuffer = GetBlockAt(offset, throwIfNotFound: false);
			return byteBuffer != null;
		}
		catch (IndexOutOfRangeException)
		{
			return false;
		}
	}

	public override ByteBuffer CreateBlockIfNeeded(int offset)
	{
		bool flag = false;
		if (_mini_stream.GetStartBlock() == -2)
		{
			flag = true;
		}
		if (!flag && TryGetBlockAt(offset, out var byteBuffer))
		{
			return byteBuffer;
		}
		int freeBlock = _filesystem.GetFreeBlock();
		_filesystem.CreateBlockIfNeeded(freeBlock);
		if (flag)
		{
			_filesystem.PropertyTable.Root.StartBlock = freeBlock;
			_mini_stream = new NPOIFSStream(_filesystem, freeBlock);
		}
		else
		{
			ChainLoopDetector chainLoopDetector = _filesystem.GetChainLoopDetector();
			int offset2 = _mini_stream.GetStartBlock();
			while (true)
			{
				chainLoopDetector.Claim(offset2);
				int nextBlock = _filesystem.GetNextBlock(offset2);
				if (nextBlock == -2)
				{
					break;
				}
				offset2 = nextBlock;
			}
			_filesystem.SetNextBlock(offset2, freeBlock);
		}
		_filesystem.SetNextBlock(freeBlock, -2);
		return CreateBlockIfNeeded(offset);
	}

	public override BATBlockAndIndex GetBATBlockAndIndex(int offset)
	{
		return BATBlock.GetSBATBlockAndIndex(offset, _header, _sbat_blocks);
	}

	public override int GetNextBlock(int offset)
	{
		BATBlockAndIndex bATBlockAndIndex = GetBATBlockAndIndex(offset);
		return bATBlockAndIndex.Block.GetValueAt(bATBlockAndIndex.Index);
	}

	public override void SetNextBlock(int offset, int nextBlock)
	{
		BATBlockAndIndex bATBlockAndIndex = GetBATBlockAndIndex(offset);
		bATBlockAndIndex.Block.SetValueAt(bATBlockAndIndex.Index, nextBlock);
	}

	public override int GetFreeBlock()
	{
		int bATEntriesPerBlock = _filesystem.GetBigBlockSizeDetails().GetBATEntriesPerBlock();
		int num = 0;
		for (int i = 0; i < _sbat_blocks.Count; i++)
		{
			BATBlock bATBlock = _sbat_blocks[i];
			if (bATBlock.HasFreeSectors)
			{
				for (int j = 0; j < bATEntriesPerBlock; j++)
				{
					if (bATBlock.GetValueAt(j) == -1)
					{
						return num + j;
					}
				}
			}
			num += bATEntriesPerBlock;
		}
		BATBlock bATBlock2 = BATBlock.CreateEmptyBATBlock(_filesystem.GetBigBlockSizeDetails(), isXBAT: false);
		int num2 = (bATBlock2.OurBlockIndex = _filesystem.GetFreeBlock());
		if (_header.SBATCount == 0)
		{
			_header.SBATStart = num2;
			_header.SBATBlockCount = 1;
		}
		else
		{
			ChainLoopDetector chainLoopDetector = _filesystem.GetChainLoopDetector();
			int offset = _header.SBATStart;
			while (true)
			{
				chainLoopDetector.Claim(offset);
				int nextBlock = _filesystem.GetNextBlock(offset);
				if (nextBlock == -2)
				{
					break;
				}
				offset = nextBlock;
			}
			_filesystem.SetNextBlock(offset, num2);
			_header.SBATBlockCount = _header.SBATCount + 1;
		}
		_filesystem.SetNextBlock(num2, -2);
		_sbat_blocks.Add(bATBlock2);
		return num;
	}

	public override ChainLoopDetector GetChainLoopDetector()
	{
		return new ChainLoopDetector(_root.Size, this);
	}

	public override int GetBlockStoreBlockSize()
	{
		return 64;
	}

	public void SyncWithDataSource()
	{
		int num = 0;
		foreach (BATBlock sbat_block in _sbat_blocks)
		{
			ByteBuffer blockAt = _filesystem.GetBlockAt(sbat_block.OurBlockIndex);
			BlockAllocationTableWriter.WriteBlock(sbat_block, blockAt);
			num = (sbat_block.HasFreeSectors ? (num + sbat_block.GetUsedSectors(isAnXBAT: false)) : (num + _filesystem.GetBigBlockSizeDetails().GetBATEntriesPerBlock()));
		}
		_filesystem.PropertyTable.Root.Size = num;
	}
}
