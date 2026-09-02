using System.Collections;
using System.Collections.Generic;
using System.IO;
using NPOI.POIFS.Common;
using NPOI.POIFS.FileSystem;
using NPOI.POIFS.Properties;

namespace NPOI.POIFS.Storage;

public class SmallBlockTableWriter : BlockWritable, BATManaged
{
	private BlockAllocationTableWriter _sbat;

	private IList<SmallDocumentBlock> _small_blocks;

	private int _big_block_count;

	private RootProperty _root;

	public int SBATBlockCount => (_big_block_count + 15) / 16;

	public BlockAllocationTableWriter SBAT => _sbat;

	public int CountBlocks => _big_block_count;

	public int StartBlock
	{
		set
		{
			_root.StartBlock = value;
		}
	}

	public SmallBlockTableWriter(POIFSBigBlockSize bigBlockSize, IList<OPOIFSDocument> documents, RootProperty root)
	{
		_sbat = new BlockAllocationTableWriter(bigBlockSize);
		_small_blocks = new List<SmallDocumentBlock>();
		_root = root;
		IEnumerator enumerator = documents.GetEnumerator();
		while (enumerator.MoveNext())
		{
			OPOIFSDocument oPOIFSDocument = (OPOIFSDocument)enumerator.Current;
			SmallDocumentBlock[] smallBlocks = oPOIFSDocument.SmallBlocks;
			if (smallBlocks.Length != 0)
			{
				oPOIFSDocument.StartBlock = _sbat.AllocateSpace(smallBlocks.Length);
				for (int i = 0; i < smallBlocks.Length; i++)
				{
					_small_blocks.Add(smallBlocks[i]);
				}
			}
			else
			{
				oPOIFSDocument.StartBlock = -2;
			}
		}
		_sbat.SimpleCreateBlocks();
		_root.Size = _small_blocks.Count;
		_big_block_count = SmallDocumentBlock.Fill(bigBlockSize, _small_blocks);
	}

	public void WriteBlocks(Stream stream)
	{
		foreach (SmallDocumentBlock small_block in _small_blocks)
		{
			((BlockWritable)small_block).WriteBlocks(stream);
		}
	}
}
