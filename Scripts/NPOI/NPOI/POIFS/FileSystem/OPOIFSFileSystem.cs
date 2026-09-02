using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NPOI.POIFS.Common;
using NPOI.POIFS.Dev;
using NPOI.POIFS.EventFileSystem;
using NPOI.POIFS.Properties;
using NPOI.POIFS.Storage;
using NPOI.Util;

namespace NPOI.POIFS.FileSystem;

[Serializable]
public class OPOIFSFileSystem : POIFSViewable
{
	private PropertyTable _property_table;

	private IList<OPOIFSDocument> _documents;

	private DirectoryNode _root;

	private POIFSBigBlockSize bigBlockSize = POIFSConstants.SMALLER_BIG_BLOCK_SIZE_DETAILS;

	public DirectoryNode Root
	{
		get
		{
			if (_root == null)
			{
				_root = new DirectoryNode(_property_table.Root, this, null);
			}
			return _root;
		}
	}

	public Array ViewableArray
	{
		get
		{
			if (PreferArray)
			{
				return ((POIFSViewable)Root).ViewableArray;
			}
			return new object[0];
		}
	}

	public IEnumerator ViewableIterator
	{
		get
		{
			if (!PreferArray)
			{
				return ((POIFSViewable)Root).ViewableIterator;
			}
			return ArrayList.ReadOnly(new ArrayList()).GetEnumerator();
		}
	}

	public bool PreferArray => ((POIFSViewable)Root).PreferArray;

	public string ShortDescription => "POIFS FileSystem";

	public int BigBlockSize => bigBlockSize.GetBigBlockSize();

	public static Stream CreateNonClosingInputStream(Stream stream)
	{
		return new CloseIgnoringInputStream(stream);
	}

	public OPOIFSFileSystem()
	{
		HeaderBlock headerBlock = new HeaderBlock(bigBlockSize);
		_property_table = new PropertyTable(headerBlock);
		_documents = new List<OPOIFSDocument>();
		_root = null;
	}

	public OPOIFSFileSystem(Stream stream)
		: this()
	{
		bool success = false;
		HeaderBlock headerBlock;
		RawDataBlockList rawDataBlockList;
		try
		{
			headerBlock = new HeaderBlock(stream);
			bigBlockSize = headerBlock.BigBlockSize;
			rawDataBlockList = new RawDataBlockList(stream, bigBlockSize);
			success = true;
		}
		finally
		{
			CloseInputStream(stream, success);
		}
		new BlockAllocationTableReader(headerBlock.BigBlockSize, headerBlock.BATCount, headerBlock.BATArray, headerBlock.XBATCount, headerBlock.XBATIndex, rawDataBlockList);
		PropertyTable propertyTable = new PropertyTable(headerBlock, rawDataBlockList);
		ProcessProperties(SmallBlockTableReader.GetSmallDocumentBlocks(bigBlockSize, rawDataBlockList, propertyTable.Root, headerBlock.SBATStart), rawDataBlockList, propertyTable.Root.Children, null, headerBlock.PropertyStart);
		Root.StorageClsid = propertyTable.Root.StorageClsid;
	}

	private void CloseInputStream(Stream stream, bool success)
	{
		if (stream is MemoryStream)
		{
			_ = "POIFS is closing the supplied input stream of type (" + stream.GetType().Name + ") which supports mark/reset.  This will be a problem for the caller if the stream will still be used.  If that is the case the caller should wrap the input stream to avoid this Close logic.  This warning is only temporary and will not be present in future versions of POI.";
		}
		try
		{
			stream.Close();
		}
		catch (IOException)
		{
			if (success)
			{
				throw;
			}
		}
	}

	public static bool HasPOIFSHeader(InputStream inp)
	{
		return HasPOIFSHeader(IOUtils.PeekFirst8Bytes(inp));
	}

	public static bool HasPOIFSHeader(byte[] header8Bytes)
	{
		return new LongField(0, header8Bytes).Value == -2226271756974174256L;
	}

	public DocumentEntry CreateDocument(Stream stream, string name)
	{
		return Root.CreateDocument(name, stream);
	}

	public DocumentEntry CreateDocument(string name, int size, POIFSWriterListener writer)
	{
		return Root.CreateDocument(name, size, writer);
	}

	public DirectoryEntry CreateDirectory(string name)
	{
		return Root.CreateDirectory(name);
	}

	public DocumentInputStream CreateDocumentInputStream(string documentName)
	{
		return Root.CreateDocumentInputStream(documentName);
	}

	public void WriteFileSystem(Stream stream)
	{
		_property_table.PreWrite();
		SmallBlockTableWriter smallBlockTableWriter = new SmallBlockTableWriter(bigBlockSize, _documents, _property_table.Root);
		BlockAllocationTableWriter blockAllocationTableWriter = new BlockAllocationTableWriter(bigBlockSize);
		List<object> list = new List<object>();
		list.AddRange(_documents);
		list.Add(_property_table);
		list.Add(smallBlockTableWriter);
		list.Add(smallBlockTableWriter.SBAT);
		IEnumerator enumerator = list.GetEnumerator();
		while (enumerator.MoveNext())
		{
			BATManaged bATManaged = (BATManaged)enumerator.Current;
			int countBlocks = bATManaged.CountBlocks;
			if (countBlocks != 0)
			{
				bATManaged.StartBlock = blockAllocationTableWriter.AllocateSpace(countBlocks);
			}
		}
		int startBlock = blockAllocationTableWriter.CreateBlocks();
		HeaderBlockWriter headerBlockWriter = new HeaderBlockWriter(bigBlockSize);
		BATBlock[] array = headerBlockWriter.SetBATBlocks(blockAllocationTableWriter.CountBlocks, startBlock);
		headerBlockWriter.PropertyStart = _property_table.StartBlock;
		headerBlockWriter.SBATStart = smallBlockTableWriter.SBAT.StartBlock;
		headerBlockWriter.SBATBlockCount = smallBlockTableWriter.SBATBlockCount;
		List<object> list2 = new List<object>();
		list2.Add(headerBlockWriter);
		list2.AddRange(_documents);
		list2.Add(_property_table);
		list2.Add(smallBlockTableWriter);
		list2.Add(smallBlockTableWriter.SBAT);
		list2.Add(blockAllocationTableWriter);
		for (int i = 0; i < array.Length; i++)
		{
			list2.Add(array[i]);
		}
		enumerator = list2.GetEnumerator();
		while (enumerator.MoveNext())
		{
			((BlockWritable)enumerator.Current).WriteBlocks(stream);
		}
		list2 = null;
		enumerator = null;
	}

	public void AddDocument(OPOIFSDocument document)
	{
		_documents.Add(document);
		_property_table.AddProperty(document.DocumentProperty);
	}

	public void AddDirectory(DirectoryProperty directory)
	{
		_property_table.AddProperty(directory);
	}

	public void Remove(EntryNode entry)
	{
		_property_table.RemoveProperty(entry.Property);
		if (entry.IsDocumentEntry)
		{
			_documents.Remove(((DocumentNode)entry).Document);
		}
	}

	private void ProcessProperties(BlockList small_blocks, BlockList big_blocks, IEnumerator properties, DirectoryNode dir, int headerPropertiesStartAt)
	{
		while (properties.MoveNext())
		{
			Property property = (Property)properties.Current;
			string name = property.Name;
			DirectoryNode directoryNode = ((dir == null) ? Root : dir);
			if (property.IsDirectory)
			{
				DirectoryNode directoryNode2 = (DirectoryNode)directoryNode.CreateDirectory(name);
				directoryNode2.StorageClsid = property.StorageClsid;
				ProcessProperties(small_blocks, big_blocks, ((DirectoryProperty)property).Children, directoryNode2, headerPropertiesStartAt);
			}
			else
			{
				int startBlock = property.StartBlock;
				int size = property.Size;
				OPOIFSDocument oPOIFSDocument = null;
				oPOIFSDocument = ((!property.ShouldUseSmallBlocks) ? new OPOIFSDocument(name, big_blocks.FetchBlocks(startBlock, headerPropertiesStartAt), size) : new OPOIFSDocument(name, small_blocks.FetchBlocks(startBlock, headerPropertiesStartAt), size));
				directoryNode.CreateDocument(oPOIFSDocument);
			}
		}
	}
}
