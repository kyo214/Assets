namespace NPOI.POIFS.Storage;

public interface BlockList
{
	BlockAllocationTableReader BAT { set; }

	void Zap(int index);

	ListManagedBlock Remove(int index);

	ListManagedBlock[] FetchBlocks(int startBlock, int headerPropertiesStartBlock);

	int BlockCount();
}
