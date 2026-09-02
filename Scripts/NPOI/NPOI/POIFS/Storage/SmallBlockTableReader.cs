using NPOI.POIFS.Common;
using NPOI.POIFS.Properties;

namespace NPOI.POIFS.Storage;

public class SmallBlockTableReader
{
	private static BlockList prepareSmallDocumentBlocks(POIFSBigBlockSize bigBlockSize, RawDataBlockList blockList, RootProperty root, int sbatStart)
	{
		ListManagedBlock[] blocks = blockList.FetchBlocks(root.StartBlock, -1);
		return new SmallDocumentBlockList(SmallDocumentBlock.Extract(bigBlockSize, blocks));
	}

	private static BlockAllocationTableReader prepareReader(POIFSBigBlockSize bigBlockSize, RawDataBlockList blockList, BlockList list, RootProperty root, int sbatStart)
	{
		return new BlockAllocationTableReader(bigBlockSize, blockList.FetchBlocks(sbatStart, -1), list);
	}

	public static BlockAllocationTableReader _getSmallDocumentBlockReader(POIFSBigBlockSize bigBlockSize, RawDataBlockList blockList, RootProperty root, int sbatStart)
	{
		BlockList list = prepareSmallDocumentBlocks(bigBlockSize, blockList, root, sbatStart);
		return prepareReader(bigBlockSize, blockList, list, root, sbatStart);
	}

	public static BlockList GetSmallDocumentBlocks(POIFSBigBlockSize bigBlockSize, RawDataBlockList blockList, RootProperty root, int sbatStart)
	{
		BlockList blockList2 = new SmallDocumentBlockList(SmallDocumentBlock.Extract(bigBlockSize, blockList.FetchBlocks(root.StartBlock, -1)));
		new BlockAllocationTableReader(bigBlockSize, blockList.FetchBlocks(sbatStart, -1), blockList2);
		return blockList2;
	}
}
