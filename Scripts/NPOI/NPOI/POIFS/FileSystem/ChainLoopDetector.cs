using System;

namespace NPOI.POIFS.FileSystem;

public class ChainLoopDetector
{
	private bool[] used_blocks;

	private BlockStore blockStore;

	public ChainLoopDetector(long rawSize, BlockStore blockStore)
	{
		this.blockStore = blockStore;
		int blockStoreBlockSize = blockStore.GetBlockStoreBlockSize();
		int num = (int)(rawSize / blockStoreBlockSize);
		if (rawSize % blockStoreBlockSize != 0L)
		{
			num++;
		}
		used_blocks = new bool[num];
	}

	public void Claim(int offset)
	{
		if (offset < used_blocks.Length)
		{
			if (used_blocks[offset])
			{
				throw new InvalidOperationException("Potential loop detected - Block " + offset + " was already claimed but was just requested again");
			}
			used_blocks[offset] = true;
		}
	}
}
