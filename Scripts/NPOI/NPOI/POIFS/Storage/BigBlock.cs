using System.IO;
using NPOI.POIFS.Common;

namespace NPOI.POIFS.Storage;

public abstract class BigBlock : BlockWritable
{
	protected POIFSBigBlockSize bigBlockSize;

	protected BigBlock()
	{
	}

	protected BigBlock(POIFSBigBlockSize bigBlockSize)
	{
		this.bigBlockSize = bigBlockSize;
	}

	protected void WriteData(Stream stream, byte[] data)
	{
		stream.Write(data, 0, data.Length);
	}

	public void WriteBlocks(Stream stream)
	{
		WriteData(stream);
	}

	public abstract void WriteData(Stream stream);
}
