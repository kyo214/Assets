using System;
using System.Collections.Generic;
using System.IO;
using NPOI.POIFS.Common;
using NPOI.POIFS.Properties;

namespace NPOI.POIFS.Storage;

public class PropertyBlock : BigBlock
{
	private class AnonymousProperty : Property
	{
		public override bool IsDirectory => false;

		public override void PreWrite()
		{
		}
	}

	private Property[] _properties;

	protected PropertyBlock(POIFSBigBlockSize bigBlockSize, Property[] properties, int offset)
		: base(bigBlockSize)
	{
		_properties = new Property[bigBlockSize.GetPropertiesPerBlock()];
		for (int i = 0; i < _properties.Length; i++)
		{
			_properties[i] = properties[i + offset];
		}
	}

	public static BlockWritable[] CreatePropertyBlockArray(POIFSBigBlockSize bigBlockSize, List<Property> properties)
	{
		int propertiesPerBlock = bigBlockSize.GetPropertiesPerBlock();
		int num = (properties.Count + propertiesPerBlock - 1) / propertiesPerBlock;
		Property[] array = new Property[num * propertiesPerBlock];
		Array.Copy(properties.ToArray(), 0, array, 0, properties.Count);
		for (int i = properties.Count; i < array.Length; i++)
		{
			array[i] = new AnonymousProperty();
		}
		BlockWritable[] array2 = new BlockWritable[num];
		for (int j = 0; j < num; j++)
		{
			array2[j] = new PropertyBlock(bigBlockSize, array, j * propertiesPerBlock);
		}
		return array2;
	}

	public override void WriteData(Stream stream)
	{
		int propertiesPerBlock = bigBlockSize.GetPropertiesPerBlock();
		for (int i = 0; i < propertiesPerBlock; i++)
		{
			_properties[i].WriteData(stream);
		}
	}
}
