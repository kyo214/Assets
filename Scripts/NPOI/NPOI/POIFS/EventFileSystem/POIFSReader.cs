using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NPOI.POIFS.FileSystem;
using NPOI.POIFS.Properties;
using NPOI.POIFS.Storage;

namespace NPOI.POIFS.EventFileSystem;

public class POIFSReader
{
	private POIFSReaderRegistry registry;

	private bool registryClosed;

	public event POIFSReaderEventHandler StreamReaded;

	protected virtual void OnStreamReaded(POIFSReaderEventArgs e)
	{
		if (StreamReaded != null)
		{
			StreamReaded(this, e);
		}
	}

	public POIFSReader()
	{
		registry = new POIFSReaderRegistry();
		registryClosed = false;
	}

	public List<DocumentDescriptor> Read(Stream stream)
	{
		registryClosed = true;
		HeaderBlock headerBlock = new HeaderBlock(stream);
		RawDataBlockList rawDataBlockList = new RawDataBlockList(stream, headerBlock.BigBlockSize);
		new BlockAllocationTableReader(headerBlock.BigBlockSize, headerBlock.BATCount, headerBlock.BATArray, headerBlock.XBATCount, headerBlock.XBATIndex, rawDataBlockList);
		PropertyTable propertyTable = new PropertyTable(headerBlock, rawDataBlockList);
		return ProcessProperties(SmallBlockTableReader.GetSmallDocumentBlocks(headerBlock.BigBlockSize, rawDataBlockList, propertyTable.Root, headerBlock.SBATStart), rawDataBlockList, propertyTable.Root.Children, new POIFSDocumentPath());
	}

	public void RegisterListener(POIFSReaderListener listener)
	{
		if (listener == null)
		{
			throw new NullReferenceException();
		}
		if (registryClosed)
		{
			throw new InvalidOperationException();
		}
		registry.RegisterListener(listener);
	}

	public void RegisterListener(POIFSReaderListener listener, string name)
	{
		RegisterListener(listener, null, name);
	}

	public void RegisterListener(POIFSReaderListener listener, POIFSDocumentPath path, string name)
	{
		if (listener == null || name == null || name.Length == 0)
		{
			throw new NullReferenceException();
		}
		if (registryClosed)
		{
			throw new InvalidOperationException();
		}
		registry.RegisterListener(listener, (path == null) ? new POIFSDocumentPath() : path, name);
	}

	private List<DocumentDescriptor> ProcessProperties(BlockList small_blocks, BlockList big_blocks, IEnumerator properties, POIFSDocumentPath path)
	{
		List<DocumentDescriptor> result = new List<DocumentDescriptor>();
		while (properties.MoveNext())
		{
			Property property = (Property)properties.Current;
			string name = property.Name;
			if (property.IsDirectory)
			{
				POIFSDocumentPath path2 = new POIFSDocumentPath(path, new string[1] { name });
				ProcessProperties(small_blocks, big_blocks, ((DirectoryProperty)property).Children, path2);
				continue;
			}
			int startBlock = property.StartBlock;
			IEnumerator listeners = registry.GetListeners(path, name);
			OPOIFSDocument oPOIFSDocument = null;
			if (listeners.MoveNext())
			{
				listeners.Reset();
				int size = property.Size;
				oPOIFSDocument = ((!property.ShouldUseSmallBlocks) ? new OPOIFSDocument(name, big_blocks.FetchBlocks(startBlock, -1), size) : new OPOIFSDocument(name, small_blocks.FetchBlocks(startBlock, -1), size));
				while (listeners.MoveNext())
				{
					((POIFSReaderListener)listeners.Current).ProcessPOIFSReaderEvent(new POIFSReaderEvent(new DocumentInputStream(oPOIFSDocument), path, name));
				}
			}
			else if (property.ShouldUseSmallBlocks)
			{
				small_blocks.FetchBlocks(startBlock, -1);
			}
			else
			{
				big_blocks.FetchBlocks(startBlock, -1);
			}
		}
		return result;
	}
}
