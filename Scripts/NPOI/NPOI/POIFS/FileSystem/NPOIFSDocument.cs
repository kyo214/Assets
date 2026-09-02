using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NPOI.POIFS.Dev;
using NPOI.POIFS.EventFileSystem;
using NPOI.POIFS.Properties;
using NPOI.Util;

namespace NPOI.POIFS.FileSystem;

public class NPOIFSDocument : POIFSViewable
{
	private DocumentProperty _property;

	private NPOIFSFileSystem _filesystem;

	private NPOIFSStream _stream;

	private int _block_size;

	internal NPOIFSFileSystem FileSystem => _filesystem;

	public int Size => _property.Size;

	public DocumentProperty DocumentProperty => _property;

	public bool PreferArray => true;

	public string ShortDescription => GetShortDescription();

	public Array ViewableArray => GetViewableArray();

	public IEnumerator ViewableIterator => GetViewableIterator();

	public NPOIFSDocument(DocumentNode document)
		: this((DocumentProperty)document.Property, ((DirectoryNode)document.Parent).NFileSystem)
	{
	}

	public NPOIFSDocument(DocumentProperty property, NPOIFSFileSystem filesystem)
	{
		_property = property;
		_filesystem = filesystem;
		if (property.Size < 4096)
		{
			_stream = new NPOIFSStream(_filesystem.GetMiniStore(), property.StartBlock);
			_block_size = _filesystem.GetMiniStore().GetBlockStoreBlockSize();
		}
		else
		{
			_stream = new NPOIFSStream(_filesystem, property.StartBlock);
			_block_size = _filesystem.GetBlockStoreBlockSize();
		}
	}

	public NPOIFSDocument(string name, NPOIFSFileSystem filesystem, Stream stream)
	{
		_filesystem = filesystem;
		int size = Store(stream);
		_property = new DocumentProperty(name, size);
		_property.StartBlock = _stream.GetStartBlock();
	}

	private int Store(Stream inStream)
	{
		int num = 4096;
		if (inStream.Length < num)
		{
			_stream = new NPOIFSStream(_filesystem.GetMiniStore());
			_block_size = _filesystem.GetMiniStore().GetBlockStoreBlockSize();
		}
		else
		{
			_stream = new NPOIFSStream(_filesystem);
			_block_size = _filesystem.GetBlockStoreBlockSize();
		}
		Stream outputStream = _stream.GetOutputStream();
		byte[] array = new byte[1024];
		int num2 = 0;
		int num3 = 0;
		while (true)
		{
			num3 = inStream.Read(array, 0, array.Length);
			if (num3 <= 0)
			{
				break;
			}
			num2 += num3;
			outputStream.Write(array, 0, num3);
		}
		int num4 = num2 % _block_size;
		if (num4 != 0 && num4 != _block_size)
		{
			byte[] array2 = new byte[_block_size - num4];
			Arrays.Fill(array2, byte.MaxValue);
			outputStream.Write(array2, 0, array2.Length);
		}
		outputStream.Close();
		return num2;
	}

	public NPOIFSDocument(string name, int size, NPOIFSFileSystem filesystem, POIFSWriterListener Writer)
	{
		_filesystem = filesystem;
		if (size < 4096)
		{
			_stream = new NPOIFSStream(filesystem.GetMiniStore());
			_block_size = _filesystem.GetMiniStore().GetBlockStoreBlockSize();
		}
		else
		{
			_stream = new NPOIFSStream(filesystem);
			_block_size = _filesystem.GetBlockStoreBlockSize();
		}
		Stream outputStream = _stream.GetOutputStream();
		DocumentOutputStream stream = new DocumentOutputStream(outputStream, size);
		POIFSDocumentPath pOIFSDocumentPath = new POIFSDocumentPath(name.Split(new string[1] { "\\\\" }, StringSplitOptions.RemoveEmptyEntries));
		string component = pOIFSDocumentPath.GetComponent(pOIFSDocumentPath.Length - 1);
		POIFSWriterEvent @event = new POIFSWriterEvent(stream, pOIFSDocumentPath, component, size);
		Writer.ProcessPOIFSWriterEvent(@event);
		outputStream.Close();
		_property = new DocumentProperty(name, size);
		_property.StartBlock = _stream.GetStartBlock();
	}

	internal void Free()
	{
		_stream.Free();
		_property.StartBlock = -2;
	}

	public int GetDocumentBlockSize()
	{
		return _block_size;
	}

	public IEnumerator<ByteBuffer> GetBlockIterator()
	{
		if (Size > 0)
		{
			return _stream.GetBlockIterator();
		}
		return new List<ByteBuffer>().GetEnumerator();
	}

	public void ReplaceContents(Stream stream)
	{
		Free();
		int size = Store(stream);
		_property.StartBlock = _stream.GetStartBlock();
		_property.UpdateSize(size);
	}

	protected object[] GetViewableArray()
	{
		string text = "<NO DATA>";
		if (Size > 0)
		{
			byte[] array = new byte[Size];
			int num = 0;
			foreach (ByteBuffer item in _stream)
			{
				int num2 = Math.Min(_block_size, array.Length - num);
				item.Read(array, num, num2);
				num += num2;
			}
			text = HexDump.Dump(array, 0L, 0);
		}
		return new string[1] { text };
	}

	protected IEnumerator GetViewableIterator()
	{
		return null;
	}

	protected string GetShortDescription()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Document: \"").Append(_property.Name).Append("\"");
		stringBuilder.Append(" size = ").Append(Size);
		return stringBuilder.ToString();
	}
}
