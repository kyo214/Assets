using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NPOI.Util;

namespace NPOI.POIFS.FileSystem;

public class NPOIFSStream : IEnumerable<ByteBuffer>, IEnumerable
{
	public class StreamBlockByteBuffer : MemoryStream
	{
		private byte[] oneByte = new byte[1];

		private ByteBuffer buffer;

		private ChainLoopDetector loopDetector;

		private int prevBlock;

		private int nextBlock;

		private NPOIFSStream pStream;

		protected internal StreamBlockByteBuffer(NPOIFSStream pStream)
		{
			this.pStream = pStream;
			loopDetector = pStream.blockStore.GetChainLoopDetector();
			prevBlock = -2;
			nextBlock = pStream.startBlock;
		}

		protected void CreateBlockIfNeeded()
		{
			if (buffer != null && buffer.HasRemaining())
			{
				return;
			}
			int freeBlock = nextBlock;
			if (freeBlock == -2)
			{
				freeBlock = pStream.blockStore.GetFreeBlock();
				loopDetector.Claim(freeBlock);
				nextBlock = -2;
				if (prevBlock != -2)
				{
					pStream.blockStore.SetNextBlock(prevBlock, freeBlock);
				}
				pStream.blockStore.SetNextBlock(freeBlock, -2);
				if (pStream.startBlock == -2)
				{
					pStream.startBlock = freeBlock;
				}
			}
			else
			{
				loopDetector.Claim(freeBlock);
				nextBlock = pStream.blockStore.GetNextBlock(freeBlock);
			}
			buffer = pStream.blockStore.CreateBlockIfNeeded(freeBlock);
			prevBlock = freeBlock;
		}

		public void Write(int b)
		{
			oneByte[0] = (byte)(b & 0xFF);
			base.Write(oneByte, 0, oneByte.Length);
		}

		public override void Write(byte[] b, int off, int len)
		{
			if (off < 0 || off > b.Length || len < 0 || off + len > b.Length || off + len < 0)
			{
				throw new IndexOutOfRangeException();
			}
			if (len != 0)
			{
				do
				{
					CreateBlockIfNeeded();
					int num = Math.Min(buffer.Remaining(), len);
					buffer.Write(b, off, num);
					off += num;
					len -= num;
				}
				while (len > 0);
			}
		}

		public override void Close()
		{
			new NPOIFSStream(pStream.blockStore, nextBlock).Free(loopDetector);
			if (prevBlock != -2)
			{
				pStream.blockStore.SetNextBlock(prevBlock, -2);
			}
			base.Close();
		}
	}

	public class StreamBlockByteBufferIterator : IEnumerator<ByteBuffer>, IDisposable, IEnumerator
	{
		private ChainLoopDetector loopDetector;

		private int nextBlock;

		private ByteBuffer current;

		private NPOIFSStream pStream;

		public ByteBuffer Current => current;

		object IEnumerator.Current => current;

		public StreamBlockByteBufferIterator(NPOIFSStream pStream, int firstBlock)
		{
			this.pStream = pStream;
			nextBlock = firstBlock;
			try
			{
				loopDetector = pStream.blockStore.GetChainLoopDetector();
			}
			catch (IOException ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public bool HasNext()
		{
			if (nextBlock == -2)
			{
				return false;
			}
			return true;
		}

		public ByteBuffer Next()
		{
			if (nextBlock == -2)
			{
				throw new IndexOutOfRangeException("Can't read past the end of the stream");
			}
			try
			{
				loopDetector.Claim(nextBlock);
				ByteBuffer blockAt = pStream.blockStore.GetBlockAt(nextBlock);
				nextBlock = pStream.blockStore.GetNextBlock(nextBlock);
				return blockAt;
			}
			catch (IOException ex)
			{
				throw new RuntimeException(ex.Message);
			}
		}

		public void Remove()
		{
			throw new NotImplementedException("Unsupported Operations!");
		}

		void IEnumerator.Reset()
		{
			throw new NotImplementedException();
		}

		bool IEnumerator.MoveNext()
		{
			if (nextBlock == -2)
			{
				return false;
			}
			try
			{
				loopDetector.Claim(nextBlock);
				current = pStream.blockStore.GetBlockAt(nextBlock);
				nextBlock = pStream.blockStore.GetNextBlock(nextBlock);
				return true;
			}
			catch (IOException)
			{
				return false;
			}
		}

		public void Dispose()
		{
		}
	}

	private BlockStore blockStore;

	private int startBlock;

	private MemoryStream outStream;

	public NPOIFSStream(BlockStore blockStore, int startBlock)
	{
		this.blockStore = blockStore;
		this.startBlock = startBlock;
	}

	public NPOIFSStream(BlockStore blockStore)
	{
		this.blockStore = blockStore;
		startBlock = -2;
	}

	public int GetStartBlock()
	{
		return startBlock;
	}

	public IEnumerator<ByteBuffer> GetEnumerator()
	{
		return GetBlockIterator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetBlockIterator();
	}

	public IEnumerator<ByteBuffer> GetBlockIterator()
	{
		if (startBlock == -2)
		{
			throw new InvalidOperationException("Can't read from a new stream before it has been written to");
		}
		return new StreamBlockByteBufferIterator(this, startBlock);
	}

	public void UpdateContents(byte[] contents)
	{
		Stream outputStream = GetOutputStream();
		outputStream.Write(contents, 0, contents.Length);
		outputStream.Close();
	}

	public Stream GetOutputStream()
	{
		if (outStream == null)
		{
			outStream = new StreamBlockByteBuffer(this);
		}
		return outStream;
	}

	public void Free()
	{
		ChainLoopDetector chainLoopDetector = blockStore.GetChainLoopDetector();
		Free(chainLoopDetector);
	}

	internal void Free(ChainLoopDetector loopDetector)
	{
		int nextBlock = startBlock;
		while (nextBlock != -2)
		{
			int offset = nextBlock;
			loopDetector.Claim(offset);
			nextBlock = blockStore.GetNextBlock(offset);
			blockStore.SetNextBlock(offset, -1);
		}
		startBlock = -2;
	}
}
