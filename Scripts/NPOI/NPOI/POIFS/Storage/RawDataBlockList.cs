using System.Collections.Generic;
using System.IO;
using NPOI.POIFS.Common;

namespace NPOI.POIFS.Storage;

public class RawDataBlockList : BlockListImpl
{
	public RawDataBlockList(Stream stream, POIFSBigBlockSize bigBlockSize)
	{
		List<RawDataBlock> list = new List<RawDataBlock>();
		RawDataBlock rawDataBlock;
		do
		{
			rawDataBlock = new RawDataBlock(stream, bigBlockSize.GetBigBlockSize());
			if (rawDataBlock.HasData)
			{
				list.Add(rawDataBlock);
			}
		}
		while (!rawDataBlock.EOF);
		ListManagedBlock[] blocks = list.ToArray();
		SetBlocks(blocks);
	}
}
