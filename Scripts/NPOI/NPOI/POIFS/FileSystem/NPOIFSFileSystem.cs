using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NPOI.POIFS.Common;
using NPOI.POIFS.Dev;
using NPOI.POIFS.EventFileSystem;
using NPOI.POIFS.NIO;
using NPOI.POIFS.Properties;
using NPOI.POIFS.Storage;
using NPOI.Util;

namespace NPOI.POIFS.FileSystem;

public class NPOIFSFileSystem : BlockStore, POIFSViewable, ICloseable
{
	private static POILogger _logger = POILogFactory.GetLogger(typeof(NPOIFSFileSystem));

	private NPOIFSMiniStore _mini_store;

	private NPropertyTable _property_table;

	private List<BATBlock> _xbat_blocks;

	private List<BATBlock> _bat_blocks;

	private HeaderBlock _header;

	private DirectoryNode _root;

	private DataSource _data;

	private POIFSBigBlockSize bigBlockSize = POIFSConstants.SMALLER_BIG_BLOCK_SIZE_DETAILS;

	public DataSource Data
	{
		get
		{
			return _data;
		}
		set
		{
			_data = value;
		}
	}

	protected internal long Size => _data.Size;

	public NPropertyTable PropertyTable => _property_table;

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

	public bool PreferArray => ((POIFSViewable)Root).PreferArray;

	public string ShortDescription => GetShortDescription();

	public Array ViewableArray => GetViewableArray();

	public IEnumerator ViewableIterator => GetViewableIterator();

	public static Stream CreateNonClosingInputStream(Stream stream)
	{
		return new CloseIgnoringInputStream(stream);
	}

	private NPOIFSFileSystem(bool newFS)
	{
		_header = new HeaderBlock(bigBlockSize);
		_property_table = new NPropertyTable(_header);
		_mini_store = new NPOIFSMiniStore(this, _property_table.Root, new List<BATBlock>(), _header);
		_xbat_blocks = new List<BATBlock>();
		_bat_blocks = new List<BATBlock>();
		_root = null;
		if (newFS)
		{
			_data = new ByteArrayBackedDataSource(new byte[bigBlockSize.GetBigBlockSize() * 3]);
		}
	}

	public NPOIFSFileSystem()
		: this(newFS: true)
	{
		_header.BATCount = 1;
		_header.BATArray = new int[1] { 1 };
		BATBlock bATBlock = BATBlock.CreateEmptyBATBlock(bigBlockSize, isXBAT: false);
		bATBlock.OurBlockIndex = 1;
		_bat_blocks.Add(bATBlock);
		SetNextBlock(0, -2);
		SetNextBlock(1, -3);
		_property_table.StartBlock = 0;
	}

	public NPOIFSFileSystem(FileInfo file)
		: this(file, readOnly: true)
	{
	}

	public NPOIFSFileSystem(FileInfo file, bool readOnly)
		: this(null, file, readOnly, closeChannelOnError: true)
	{
	}

	public NPOIFSFileSystem(FileStream channel)
		: this(channel, readOnly: true)
	{
	}

	public NPOIFSFileSystem(FileStream channel, bool readOnly)
		: this(channel, null, readOnly, closeChannelOnError: false)
	{
	}

	public NPOIFSFileSystem(FileStream channel, FileInfo srcFile, bool readOnly, bool closeChannelOnError)
		: this(newFS: false)
	{
		try
		{
			if (srcFile != null)
			{
				if (srcFile.Length == 0L)
				{
					throw new EmptyFileException();
				}
				channel = new FileStream(srcFile.FullName, FileMode.Open, FileAccess.Read);
				_data = new FileBackedDataSource(channel, readOnly);
			}
			else
			{
				_data = new FileBackedDataSource(channel, readOnly);
			}
			try
			{
				byte[] array = new byte[512];
				IOUtils.ReadFully(channel, array);
				_header = new HeaderBlock(array);
				ReadCoreContents();
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				channel?.Close();
			}
		}
		catch (IOException ex2)
		{
			if (closeChannelOnError && channel != null)
			{
				channel.Close();
				channel = null;
			}
			throw ex2;
		}
		catch (RuntimeException ex3)
		{
			if (closeChannelOnError && channel != null)
			{
				channel.Close();
				channel = null;
			}
			throw ex3;
		}
	}

	public NPOIFSFileSystem(Stream stream)
		: this(newFS: false)
	{
		Stream stream2 = null;
		bool success = false;
		try
		{
			stream2 = stream;
			ByteBuffer byteBuffer = ByteBuffer.CreateBuffer(512);
			IOUtils.ReadFully(stream2, byteBuffer.Buffer);
			_header = new HeaderBlock(byteBuffer);
			BlockAllocationTableReader.SanityCheckBlockCount(_header.BATCount);
			long num = BATBlock.CalculateMaximumSize(_header);
			if (num > int.MaxValue)
			{
				throw new ArgumentException("Unable read a >2gb file via an InputStream");
			}
			ByteBuffer byteBuffer2 = ByteBuffer.CreateBuffer((int)num);
			byteBuffer.Position = 0;
			byteBuffer2.Write(byteBuffer.Buffer);
			byteBuffer2.Position = byteBuffer.Length;
			byteBuffer2.Position += IOUtils.ReadFully(stream2, byteBuffer2.Buffer, byteBuffer2.Position, (int)stream2.Length);
			success = true;
			_data = new ByteArrayBackedDataSource(byteBuffer2.Buffer, byteBuffer2.Position);
		}
		finally
		{
			stream2?.Close();
			CloseInputStream(stream, success);
		}
		ReadCoreContents();
	}

	private void CloseInputStream(Stream stream, bool success)
	{
		try
		{
			stream.Close();
		}
		catch (IOException ex)
		{
			if (success)
			{
				throw new Exception(ex.Message);
			}
		}
	}

	public static bool HasPOIFSHeader(Stream inp)
	{
		byte[] array = new byte[8];
		int len = IOUtils.ReadFully(inp, array);
		LongField longField = new LongField(0, array);
		if (inp is PushbackInputStream)
		{
			((PushbackInputStream)inp).Unread(array, 0, len);
		}
		else
		{
			inp.Position = 0L;
		}
		return longField.Value == -2226271756974174256L;
	}

	public static bool HasPOIFSHeader(byte[] header8Bytes)
	{
		return new LongField(0, header8Bytes).Value == -2226271756974174256L;
	}

	private void ReadCoreContents()
	{
		bigBlockSize = _header.BigBlockSize;
		ChainLoopDetector chainLoopDetector = GetChainLoopDetector();
		int[] bATArray = _header.BATArray;
		foreach (int batAt in bATArray)
		{
			ReadBAT(batAt, chainLoopDetector);
		}
		int num = _header.BATCount - _header.BATArray.Length;
		int num2 = _header.XBATIndex;
		for (int j = 0; j < _header.XBATCount; j++)
		{
			chainLoopDetector.Claim(num2);
			ByteBuffer blockAt = GetBlockAt(num2);
			BATBlock bATBlock = BATBlock.CreateBATBlock(bigBlockSize, blockAt);
			bATBlock.OurBlockIndex = num2;
			num2 = bATBlock.GetValueAt(bigBlockSize.GetXBATEntriesPerBlock());
			_xbat_blocks.Add(bATBlock);
			int num3 = Math.Min(num, bigBlockSize.GetXBATEntriesPerBlock());
			for (int k = 0; k < num3; k++)
			{
				int valueAt = bATBlock.GetValueAt(k);
				if (valueAt == -1 || valueAt == -2)
				{
					break;
				}
				ReadBAT(valueAt, chainLoopDetector);
			}
			num -= num3;
		}
		_property_table = new NPropertyTable(_header, this);
		List<BATBlock> list = new List<BATBlock>();
		_mini_store = new NPOIFSMiniStore(this, _property_table.Root, list, _header);
		num2 = _header.SBATStart;
		for (int l = 0; l < _header.SBATCount; l++)
		{
			if (num2 == -2)
			{
				break;
			}
			chainLoopDetector.Claim(num2);
			ByteBuffer blockAt2 = GetBlockAt(num2);
			BATBlock bATBlock2 = BATBlock.CreateBATBlock(bigBlockSize, blockAt2);
			bATBlock2.OurBlockIndex = num2;
			list.Add(bATBlock2);
			num2 = GetNextBlock(num2);
		}
	}

	private void ReadBAT(int batAt, ChainLoopDetector loopDetector)
	{
		loopDetector.Claim(batAt);
		ByteBuffer blockAt = GetBlockAt(batAt);
		BATBlock bATBlock = BATBlock.CreateBATBlock(bigBlockSize, blockAt);
		bATBlock.OurBlockIndex = batAt;
		_bat_blocks.Add(bATBlock);
	}

	private BATBlock CreateBAT(int offset, bool isBAT)
	{
		BATBlock bATBlock = BATBlock.CreateEmptyBATBlock(bigBlockSize, !isBAT);
		bATBlock.OurBlockIndex = offset;
		ByteBuffer src = ByteBuffer.CreateBuffer(bigBlockSize.GetBigBlockSize());
		int num = (1 + offset) * bigBlockSize.GetBigBlockSize();
		_data.Write(src, num);
		return bATBlock;
	}

	public override ByteBuffer GetBlockAt(int offset)
	{
		ByteBuffer byteBuffer = null;
		if (!TryGetBlockAt(offset, out byteBuffer))
		{
			throw new IndexOutOfRangeException("Block " + offset + " not found");
		}
		return byteBuffer;
	}

	public override bool TryGetBlockAt(int offset, out ByteBuffer buffer)
	{
		long num = (offset + 1) * bigBlockSize.GetBigBlockSize();
		buffer = null;
		if (num >= _data.Size)
		{
			return false;
		}
		try
		{
			buffer = _data.Read(bigBlockSize.GetBigBlockSize(), num);
			return true;
		}
		catch (IndexOutOfRangeException innerException)
		{
			throw new IndexOutOfRangeException("Block " + offset + " not found - ", innerException);
		}
	}

	public override ByteBuffer CreateBlockIfNeeded(int offset)
	{
		if (TryGetBlockAt(offset, out var byteBuffer))
		{
			return byteBuffer;
		}
		long position = (offset + 1) * bigBlockSize.GetBigBlockSize();
		ByteBuffer src = ByteBuffer.CreateBuffer(GetBigBlockSize());
		_data.Write(src, position);
		return GetBlockAt(offset);
	}

	public override BATBlockAndIndex GetBATBlockAndIndex(int offset)
	{
		return BATBlock.GetBATBlockAndIndex(offset, _header, _bat_blocks);
	}

	public override int GetNextBlock(int offset)
	{
		BATBlockAndIndex bATBlockAndIndex = GetBATBlockAndIndex(offset);
		return bATBlockAndIndex.Block.GetValueAt(bATBlockAndIndex.Index);
	}

	public override void SetNextBlock(int offset, int nextBlock)
	{
		BATBlockAndIndex bATBlockAndIndex = GetBATBlockAndIndex(offset);
		bATBlockAndIndex.Block.SetValueAt(bATBlockAndIndex.Index, nextBlock);
	}

	public override int GetFreeBlock()
	{
		int bATEntriesPerBlock = bigBlockSize.GetBATEntriesPerBlock();
		int num = 0;
		foreach (BATBlock bat_block in _bat_blocks)
		{
			if (bat_block.HasFreeSectors)
			{
				for (int i = 0; i < bATEntriesPerBlock; i++)
				{
					if (bat_block.GetValueAt(i) == -1)
					{
						return num + i;
					}
				}
			}
			num += bATEntriesPerBlock;
		}
		BATBlock bATBlock = CreateBAT(num, isBAT: true);
		bATBlock.SetValueAt(0, -3);
		_bat_blocks.Add(bATBlock);
		if (_header.BATCount >= 109)
		{
			BATBlock bATBlock2 = null;
			foreach (BATBlock xbat_block in _xbat_blocks)
			{
				if (xbat_block.HasFreeSectors)
				{
					bATBlock2 = xbat_block;
					break;
				}
			}
			if (bATBlock2 == null)
			{
				bATBlock2 = CreateBAT(num + 1, isBAT: false);
				bATBlock2.SetValueAt(0, num);
				bATBlock.SetValueAt(1, -4);
				num++;
				if (_xbat_blocks.Count == 0)
				{
					_header.XBATStart = num;
				}
				else
				{
					_xbat_blocks[_xbat_blocks.Count - 1].SetValueAt(bigBlockSize.GetXBATEntriesPerBlock(), num);
				}
				_xbat_blocks.Add(bATBlock2);
				_header.XBATCount = _xbat_blocks.Count;
			}
			else
			{
				for (int j = 0; j < bigBlockSize.GetXBATEntriesPerBlock(); j++)
				{
					if (bATBlock2.GetValueAt(j) == -1)
					{
						bATBlock2.SetValueAt(j, num);
						break;
					}
				}
			}
		}
		else
		{
			int[] array = new int[_header.BATCount + 1];
			Array.Copy(_header.BATArray, 0, array, 0, array.Length - 1);
			array[^1] = num;
			_header.BATArray = array;
		}
		_header.BATCount = _bat_blocks.Count;
		return num + 1;
	}

	public override ChainLoopDetector GetChainLoopDetector()
	{
		return new ChainLoopDetector(_data.Size, this);
	}

	public NPOIFSMiniStore GetMiniStore()
	{
		return _mini_store;
	}

	public void AddDocument(NPOIFSDocument document)
	{
		_property_table.AddProperty(document.DocumentProperty);
	}

	public void AddDirectory(DirectoryProperty directory)
	{
		_property_table.AddProperty(directory);
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

	public DocumentEntry CreateOrUpdateDocument(Stream stream, string name)
	{
		return Root.CreateOrUpdateDocument(name, stream);
	}

	public bool IsInPlaceWriteable()
	{
		if (_data is FileBackedDataSource && ((FileBackedDataSource)_data).IsWriteable)
		{
			return true;
		}
		return false;
	}

	public void WriteFileSystem()
	{
		if (!(_data is FileBackedDataSource))
		{
			throw new ArgumentException("POIFS opened from an inputstream, so WriteFilesystem() may not be called. Use WriteFilesystem(OutputStream) instead");
		}
		syncWithDataSource();
	}

	public void WriteFileSystem(Stream stream)
	{
		syncWithDataSource();
		_data.CopyTo(stream);
	}

	private void syncWithDataSource()
	{
		_mini_store.SyncWithDataSource();
		NPOIFSStream stream = new NPOIFSStream(this, _header.PropertyStart);
		_property_table.PreWrite();
		_property_table.Write(stream);
		new HeaderBlockWriter(_header).WriteBlock(GetBlockAt(-1));
		foreach (BATBlock bat_block in _bat_blocks)
		{
			ByteBuffer blockAt = GetBlockAt(bat_block.OurBlockIndex);
			BlockAllocationTableWriter.WriteBlock(bat_block, blockAt);
		}
		foreach (BATBlock xbat_block in _xbat_blocks)
		{
			ByteBuffer blockAt2 = GetBlockAt(xbat_block.OurBlockIndex);
			BlockAllocationTableWriter.WriteBlock(xbat_block, blockAt2);
		}
	}

	public void Close()
	{
		_data.Close();
	}

	public DocumentInputStream CreateDocumentInputStream(string documentName)
	{
		return Root.CreateDocumentInputStream(documentName);
	}

	public void Remove(EntryNode entry)
	{
		if (entry is DocumentEntry)
		{
			new NPOIFSDocument((DocumentProperty)entry.Property, this).Free();
		}
		_property_table.RemoveProperty(entry.Property);
	}

	protected object[] GetViewableArray()
	{
		if (PreferArray)
		{
			Array viewableArray = ((POIFSViewable)Root).ViewableArray;
			object[] array = new object[viewableArray.Length];
			for (int i = 0; i < viewableArray.Length; i++)
			{
				array[i] = viewableArray.GetValue(i);
			}
			return array;
		}
		return new object[0];
	}

	protected IEnumerator GetViewableIterator()
	{
		if (!PreferArray)
		{
			return ((POIFSViewable)Root).ViewableIterator;
		}
		return null;
	}

	protected string GetShortDescription()
	{
		return "POIFS FileSystem";
	}

	public int GetBigBlockSize()
	{
		return bigBlockSize.GetBigBlockSize();
	}

	public POIFSBigBlockSize GetBigBlockSizeDetails()
	{
		return bigBlockSize;
	}

	public override int GetBlockStoreBlockSize()
	{
		return GetBigBlockSize();
	}
}
