using System.Collections.Generic;

namespace NPOI.POIFS.Storage;

public class SmallDocumentBlockList : BlockListImpl
{
	public SmallDocumentBlockList(List<SmallDocumentBlock> blocks)
	{
		ListManagedBlock[] blocks2 = blocks.ToArray();
		SetBlocks(blocks2);
	}
}
