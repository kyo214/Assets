using NPOI.POIFS.Storage;
using NPOI.Util;

namespace NPOI.POIFS.FileSystem;

public abstract class BlockStore
{
	public abstract int GetBlockStoreBlockSize();

	public abstract ByteBuffer GetBlockAt(int offset);

	public abstract bool TryGetBlockAt(int offset, out ByteBuffer byteBuffer);

	public abstract ByteBuffer CreateBlockIfNeeded(int offset);

	public abstract BATBlockAndIndex GetBATBlockAndIndex(int offset);

	public abstract int GetNextBlock(int offset);

	public abstract void SetNextBlock(int offset, int nextBlock);

	public abstract int GetFreeBlock();

	public abstract ChainLoopDetector GetChainLoopDetector();
}
