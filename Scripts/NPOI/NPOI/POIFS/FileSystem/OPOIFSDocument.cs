using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NPOI.POIFS.Common;
using NPOI.POIFS.Dev;
using NPOI.POIFS.EventFileSystem;
using NPOI.POIFS.Properties;
using NPOI.POIFS.Storage;
using NPOI.Util;

namespace NPOI.POIFS.FileSystem;

public class OPOIFSDocument : BATManaged, BlockWritable, POIFSViewable
{
	internal class SmallBlockStore
	{
		private SmallDocumentBlock[] smallBlocks;

		private POIFSDocumentPath path;

		private string name;

		private int size;

		private POIFSWriterListener writer;

		private POIFSBigBlockSize bigBlockSize;

		internal virtual SmallDocumentBlock[] Blocks
		{
			get
			{
				if (Valid && writer != null)
				{
					MemoryStream memoryStream = new MemoryStream(size);
					DocumentOutputStream stream = new DocumentOutputStream(memoryStream, size);
					writer.ProcessPOIFSWriterEvent(new POIFSWriterEvent(stream, path, name, size));
					smallBlocks = SmallDocumentBlock.Convert(bigBlockSize, memoryStream.ToArray(), size);
				}
				return smallBlocks;
			}
		}

		internal virtual bool Valid
		{
			get
			{
				if (smallBlocks.Length == 0)
				{
					return writer != null;
				}
				return true;
			}
		}

		internal SmallBlockStore(POIFSBigBlockSize bigBlockSize, SmallDocumentBlock[] blocks)
		{
			this.bigBlockSize = bigBlockSize;
			smallBlocks = (SmallDocumentBlock[])blocks.Clone();
			path = null;
			name = null;
			size = -1;
			writer = null;
		}

		internal SmallBlockStore(POIFSBigBlockSize bigBlockSize, POIFSDocumentPath path, string name, int size, POIFSWriterListener writer)
		{
			this.bigBlockSize = bigBlockSize;
			smallBlocks = new SmallDocumentBlock[0];
			this.path = path;
			this.name = name;
			this.size = size;
			this.writer = writer;
		}
	}

	internal class BigBlockStore
	{
		private DocumentBlock[] bigBlocks;

		private POIFSDocumentPath path;

		private string name;

		private int size;

		private POIFSWriterListener writer;

		private POIFSBigBlockSize bigBlockSize;

		internal virtual bool Valid
		{
			get
			{
				if (bigBlocks.Length == 0)
				{
					return writer != null;
				}
				return true;
			}
		}

		internal virtual DocumentBlock[] Blocks
		{
			get
			{
				if (Valid && writer != null)
				{
					MemoryStream memoryStream = new MemoryStream(size);
					DocumentOutputStream stream = new DocumentOutputStream(memoryStream, size);
					writer.ProcessPOIFSWriterEvent(new POIFSWriterEvent(stream, path, name, size));
					bigBlocks = DocumentBlock.Convert(bigBlockSize, memoryStream.ToArray(), size);
				}
				return bigBlocks;
			}
		}

		internal virtual int CountBlocks
		{
			get
			{
				int result = 0;
				if (!Valid)
				{
					return result;
				}
				if (writer != null)
				{
					return (size + 512 - 1) / 512;
				}
				return bigBlocks.Length;
			}
		}

		internal BigBlockStore(POIFSBigBlockSize bigBlockSize, DocumentBlock[] blocks)
		{
			this.bigBlockSize = bigBlockSize;
			bigBlocks = (DocumentBlock[])blocks.Clone();
			path = null;
			name = null;
			size = -1;
			writer = null;
		}

		internal BigBlockStore(POIFSBigBlockSize bigBlockSize, POIFSDocumentPath path, string name, int size, POIFSWriterListener writer)
		{
			this.bigBlockSize = bigBlockSize;
			bigBlocks = new DocumentBlock[0];
			this.path = path;
			this.name = name;
			this.size = size;
			this.writer = writer;
		}

		internal virtual void WriteBlocks(Stream stream)
		{
			if (!Valid)
			{
				return;
			}
			if (writer != null)
			{
				DocumentOutputStream documentOutputStream = new DocumentOutputStream(stream, size);
				writer.ProcessPOIFSWriterEvent(new POIFSWriterEvent(documentOutputStream, path, name, size));
				documentOutputStream.WriteFiller(CountBlocks * 512, DocumentBlock.FillByte);
			}
			else
			{
				for (int i = 0; i < bigBlocks.Length; i++)
				{
					bigBlocks[i].WriteBlocks(stream);
				}
			}
		}
	}

	private static DocumentBlock[] EMPTY_BIG_BLOCK_ARRAY = new DocumentBlock[0];

	private static SmallDocumentBlock[] EMPTY_SMALL_BLOCK_ARRAY = new SmallDocumentBlock[0];

	private DocumentProperty _property;

	private int _size;

	private POIFSBigBlockSize _bigBigBlockSize;

	private SmallBlockStore _small_store;

	private BigBlockStore _big_store;

	public virtual int CountBlocks => _big_store.CountBlocks;

	public virtual DocumentProperty DocumentProperty => _property;

	public virtual bool PreferArray => true;

	public virtual string ShortDescription
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Document: \"").Append(_property.Name).Append("\"");
			stringBuilder.Append(" size = ").Append(Size);
			return stringBuilder.ToString();
		}
	}

	public virtual int Size => _size;

	public virtual SmallDocumentBlock[] SmallBlocks => _small_store.Blocks;

	public virtual int StartBlock
	{
		get
		{
			return _property.StartBlock;
		}
		set
		{
			_property.StartBlock = value;
		}
	}

	public Array ViewableArray
	{
		get
		{
			string text = "<NO DATA>";
			try
			{
				using (new MemoryStream())
				{
					BlockWritable[] array = null;
					if (_big_store.Valid)
					{
						BlockWritable[] blocks = _big_store.Blocks;
						array = blocks;
					}
					else if (_small_store.Valid)
					{
						BlockWritable[] blocks = _small_store.Blocks;
						array = blocks;
					}
					if (array != null)
					{
						ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
						BlockWritable[] blocks = array;
						for (int i = 0; i < blocks.Length; i++)
						{
							blocks[i].WriteBlocks(byteArrayOutputStream);
						}
						int length = (int)Math.Min(byteArrayOutputStream.Length, _property.Size);
						text = HexDump.Dump(byteArrayOutputStream.ToByteArray(), 0L, 0, length);
					}
				}
			}
			catch (IOException ex)
			{
				text = ex.Message;
			}
			return new string[1] { text };
		}
	}

	public virtual IEnumerator ViewableIterator => ArrayList.ReadOnly(new ArrayList()).GetEnumerator();

	public event POIFSWriterEventHandler BeforeWriting;

	public OPOIFSDocument(string name, RawDataBlock[] blocks, int length)
	{
		_size = length;
		if (blocks.Length == 0)
		{
			_bigBigBlockSize = POIFSConstants.SMALLER_BIG_BLOCK_SIZE_DETAILS;
		}
		else
		{
			_bigBigBlockSize = ((blocks[0].BigBlockSize == 512) ? POIFSConstants.SMALLER_BIG_BLOCK_SIZE_DETAILS : POIFSConstants.LARGER_BIG_BLOCK_SIZE_DETAILS);
		}
		_big_store = new BigBlockStore(_bigBigBlockSize, ConvertRawBlocksToBigBlocks(blocks));
		_property = new DocumentProperty(name, _size);
		_small_store = new SmallBlockStore(_bigBigBlockSize, EMPTY_SMALL_BLOCK_ARRAY);
		_property.Document = this;
	}

	private static DocumentBlock[] ConvertRawBlocksToBigBlocks(ListManagedBlock[] blocks)
	{
		DocumentBlock[] array = new DocumentBlock[blocks.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new DocumentBlock((RawDataBlock)blocks[i]);
		}
		return array;
	}

	private static SmallDocumentBlock[] ConvertRawBlocksToSmallBlocks(ListManagedBlock[] blocks)
	{
		if (blocks is SmallDocumentBlock[])
		{
			return (SmallDocumentBlock[])blocks;
		}
		SmallDocumentBlock[] array = new SmallDocumentBlock[blocks.Length];
		Array.Copy(blocks, 0, array, 0, blocks.Length);
		return array;
	}

	public OPOIFSDocument(string name, SmallDocumentBlock[] blocks, int length)
	{
		_size = length;
		if (blocks.Length == 0)
		{
			_bigBigBlockSize = POIFSConstants.SMALLER_BIG_BLOCK_SIZE_DETAILS;
		}
		else
		{
			_bigBigBlockSize = blocks[0].BigBlockSize;
		}
		_big_store = new BigBlockStore(_bigBigBlockSize, EMPTY_BIG_BLOCK_ARRAY);
		_property = new DocumentProperty(name, _size);
		_small_store = new SmallBlockStore(_bigBigBlockSize, blocks);
		_property.Document = this;
	}

	public OPOIFSDocument(string name, POIFSBigBlockSize bigBlockSize, ListManagedBlock[] blocks, int length)
	{
		_size = length;
		_bigBigBlockSize = bigBlockSize;
		_property = new DocumentProperty(name, _size);
		_property.Document = this;
		if (Property.IsSmall(_size))
		{
			_big_store = new BigBlockStore(bigBlockSize, EMPTY_BIG_BLOCK_ARRAY);
			_small_store = new SmallBlockStore(bigBlockSize, ConvertRawBlocksToSmallBlocks(blocks));
		}
		else
		{
			_big_store = new BigBlockStore(bigBlockSize, ConvertRawBlocksToBigBlocks(blocks));
			_small_store = new SmallBlockStore(bigBlockSize, EMPTY_SMALL_BLOCK_ARRAY);
		}
	}

	public OPOIFSDocument(string name, POIFSBigBlockSize bigBlockSize, Stream stream)
	{
		List<DocumentBlock> list = new List<DocumentBlock>();
		_size = 0;
		_bigBigBlockSize = bigBlockSize;
		DocumentBlock documentBlock;
		do
		{
			documentBlock = new DocumentBlock(stream, bigBlockSize);
			int size = documentBlock.Size;
			if (size > 0)
			{
				list.Add(documentBlock);
				_size += size;
			}
		}
		while (!documentBlock.PartiallyRead);
		DocumentBlock[] array = list.ToArray();
		_big_store = new BigBlockStore(bigBlockSize, array);
		_property = new DocumentProperty(name, _size);
		_property.Document = this;
		if (_property.ShouldUseSmallBlocks)
		{
			BlockWritable[] store = array;
			_small_store = new SmallBlockStore(bigBlockSize, SmallDocumentBlock.Convert(bigBlockSize, store, _size));
			_big_store = new BigBlockStore(bigBlockSize, new DocumentBlock[0]);
		}
		else
		{
			_small_store = new SmallBlockStore(bigBlockSize, EMPTY_SMALL_BLOCK_ARRAY);
		}
	}

	public OPOIFSDocument(string name, Stream stream)
		: this(name, POIFSConstants.SMALLER_BIG_BLOCK_SIZE_DETAILS, stream)
	{
	}

	public OPOIFSDocument(string name, int size, POIFSBigBlockSize bigBlockSize, POIFSDocumentPath path, POIFSWriterListener writer)
	{
		_size = size;
		_bigBigBlockSize = bigBlockSize;
		_property = new DocumentProperty(name, _size);
		_property.Document = this;
		if (_property.ShouldUseSmallBlocks)
		{
			_small_store = new SmallBlockStore(_bigBigBlockSize, path, name, size, writer);
			_big_store = new BigBlockStore(_bigBigBlockSize, EMPTY_BIG_BLOCK_ARRAY);
		}
		else
		{
			_small_store = new SmallBlockStore(_bigBigBlockSize, EMPTY_SMALL_BLOCK_ARRAY);
			_big_store = new BigBlockStore(_bigBigBlockSize, path, name, size, writer);
		}
	}

	public OPOIFSDocument(string name, int size, POIFSDocumentPath path, POIFSWriterListener writer)
		: this(name, size, POIFSConstants.SMALLER_BIG_BLOCK_SIZE_DETAILS, path, writer)
	{
	}

	public OPOIFSDocument(string name, ListManagedBlock[] blocks, int length)
		: this(name, POIFSConstants.SMALLER_BIG_BLOCK_SIZE_DETAILS, blocks, length)
	{
	}

	public virtual void Read(byte[] buffer, int offset)
	{
		int num = buffer.Length;
		DataInputBlock dataInputBlock = GetDataInputBlock(offset);
		int num2 = dataInputBlock.Available();
		if (num2 > num)
		{
			dataInputBlock.ReadFully(buffer, 0, num);
			return;
		}
		int num3 = num;
		int num4 = 0;
		int num5 = offset;
		while (num3 > 0)
		{
			bool num6 = num3 >= num2;
			int num7 = ((!num6) ? num3 : num2);
			dataInputBlock.ReadFully(buffer, num4, num7);
			num3 -= num7;
			num4 += num7;
			num5 += num7;
			if (!num6)
			{
				continue;
			}
			if (num5 == _size)
			{
				if (num3 > 0)
				{
					throw new InvalidOperationException("reached end of document stream unexpectedly");
				}
				dataInputBlock = null;
				break;
			}
			dataInputBlock = GetDataInputBlock(num5);
			num2 = dataInputBlock.Available();
		}
	}

	public virtual void WriteBlocks(Stream stream)
	{
		_big_store.WriteBlocks(stream);
	}

	public DataInputBlock GetDataInputBlock(int offset)
	{
		if (offset >= _size)
		{
			if (offset > _size)
			{
				throw new RuntimeException("Request for Offset " + offset + " doc size is " + _size);
			}
			return null;
		}
		if (_property.ShouldUseSmallBlocks)
		{
			return SmallDocumentBlock.GetDataInputBlock(_small_store.Blocks, offset);
		}
		return DocumentBlock.GetDataInputBlock(_big_store.Blocks, offset);
	}

	protected virtual void OnBeforeWriting(POIFSWriterEventArgs e)
	{
		if (BeforeWriting != null)
		{
			BeforeWriting(this, e);
		}
	}
}
