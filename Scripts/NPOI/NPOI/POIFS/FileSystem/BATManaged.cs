namespace NPOI.POIFS.FileSystem;

public interface BATManaged
{
	int CountBlocks { get; }

	int StartBlock { set; }
}
