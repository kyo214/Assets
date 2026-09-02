using System.IO;

namespace NPOI.POIFS.Storage;

public interface BlockWritable
{
	void WriteBlocks(Stream stream);
}
