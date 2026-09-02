using System.Collections.Generic;
using System.IO;
using NPOI.POIFS.Common;
using NPOI.POIFS.Storage;

namespace NPOI.POIFS.Properties;

public class PropertyTable : PropertyTableBase, BlockWritable
{
	private POIFSBigBlockSize _bigBigBlockSize;

	private BlockWritable[] _blocks;

	public override int CountBlocks
	{
		get
		{
			if (_blocks != null)
			{
				return _blocks.Length;
			}
			return 0;
		}
	}

	public PropertyTable(HeaderBlock headerBlock)
		: base(headerBlock)
	{
		_bigBigBlockSize = headerBlock.BigBlockSize;
		_blocks = null;
	}

	public PropertyTable(HeaderBlock headerBlock, RawDataBlockList blockList)
		: base(headerBlock, PropertyFactory.ConvertToProperties(blockList.FetchBlocks(headerBlock.PropertyStart, -1)))
	{
		_bigBigBlockSize = headerBlock.BigBlockSize;
		_blocks = null;
	}

	public void PreWrite()
	{
		List<Property> list = new List<Property>(_properties.Count);
		for (int i = 0; i < _properties.Count; i++)
		{
			list.Add(_properties[i]);
		}
		for (int j = 0; j < list.Count; j++)
		{
			list[j].Index = j;
		}
		_blocks = PropertyBlock.CreatePropertyBlockArray(_bigBigBlockSize, list);
		for (int k = 0; k < list.Count; k++)
		{
			list[k].PreWrite();
		}
	}

	public void WriteBlocks(Stream stream)
	{
		if (_blocks != null)
		{
			for (int i = 0; i < _blocks.Length; i++)
			{
				_blocks[i].WriteBlocks(stream);
			}
		}
	}
}
