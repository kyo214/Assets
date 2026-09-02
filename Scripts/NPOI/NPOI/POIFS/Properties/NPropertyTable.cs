using System.Collections.Generic;
using System.IO;
using NPOI.POIFS.Common;
using NPOI.POIFS.FileSystem;
using NPOI.POIFS.Storage;
using NPOI.Util;

namespace NPOI.POIFS.Properties;

public class NPropertyTable : PropertyTableBase
{
	private POIFSBigBlockSize _bigBigBlockSize;

	public override int CountBlocks
	{
		get
		{
			long num = _properties.Count * 128;
			int bigBlockSize = _bigBigBlockSize.GetBigBlockSize();
			int num2 = (int)(num / bigBlockSize);
			if (num % bigBlockSize != 0L)
			{
				num2++;
			}
			return num2;
		}
	}

	public NPropertyTable(HeaderBlock headerBlock)
		: base(headerBlock)
	{
		_bigBigBlockSize = headerBlock.BigBlockSize;
	}

	public NPropertyTable(HeaderBlock headerBlock, NPOIFSFileSystem fileSystem)
		: base(headerBlock, BuildProperties(new NPOIFSStream(fileSystem, headerBlock.PropertyStart).GetEnumerator(), headerBlock.BigBlockSize))
	{
		_bigBigBlockSize = headerBlock.BigBlockSize;
	}

	private static List<Property> BuildProperties(IEnumerator<ByteBuffer> dataSource, POIFSBigBlockSize bigBlockSize)
	{
		List<Property> list = new List<Property>();
		while (dataSource.MoveNext())
		{
			ByteBuffer current = dataSource.Current;
			byte[] array;
			if (current.HasBuffer && current.Offset == 0 && current.Buffer.Length == bigBlockSize.GetBigBlockSize())
			{
				array = current.Buffer;
			}
			else
			{
				array = new byte[bigBlockSize.GetBigBlockSize()];
				int length = array.Length;
				if (current.Remaining() < bigBlockSize.GetBigBlockSize())
				{
					length = current.Remaining();
				}
				current.Read(array, 0, length);
			}
			PropertyFactory.ConvertToProperties(array, list);
		}
		return list;
	}

	public void PreWrite()
	{
		List<Property> list = new List<Property>();
		int num = 0;
		foreach (Property property in _properties)
		{
			if (property != null)
			{
				property.Index = num++;
				list.Add(property);
			}
		}
		foreach (Property item in list)
		{
			item.PreWrite();
		}
	}

	public void Write(NPOIFSStream stream)
	{
		Stream outputStream = stream.GetOutputStream();
		try
		{
			new MemoryStream();
			foreach (Property property in _properties)
			{
				property?.WriteData(outputStream);
			}
			outputStream.Close();
			if (StartBlock != stream.GetStartBlock())
			{
				StartBlock = stream.GetStartBlock();
			}
		}
		catch (IOException ex)
		{
			throw ex;
		}
	}
}
