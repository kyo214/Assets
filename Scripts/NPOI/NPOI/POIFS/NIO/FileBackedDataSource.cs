using System;
using System.Collections.Generic;
using System.IO;
using NPOI.Util;

namespace NPOI.POIFS.NIO;

public class FileBackedDataSource : DataSource
{
	private MemoryStream fileStream;

	private FileInfo fileinfo;

	private bool writable;

	private List<ByteBuffer> buffersToClean = new List<ByteBuffer>();

	public bool IsWriteable => writable;

	public Stream Stream => fileStream;

	public override long Size
	{
		get
		{
			if (fileStream != null)
			{
				return fileStream.Length;
			}
			return fileinfo.Length;
		}
	}

	public FileBackedDataSource(FileInfo file)
		: this(file, readOnly: false)
	{
	}

	public FileBackedDataSource(FileInfo file, bool readOnly)
	{
		if (!file.Exists)
		{
			throw new FileNotFoundException(file.FullName);
		}
		fileinfo = file;
		FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
		byte[] array = new byte[fileStream.Length];
		fileStream.Read(array, 0, (int)fileStream.Length);
		MemoryStream memoryStream = new MemoryStream(array, 0, array.Length);
		this.fileStream = memoryStream;
		writable = !readOnly;
		fileStream.Position = 0L;
	}

	public FileBackedDataSource(FileStream stream, bool readOnly)
	{
		stream.Position = 0L;
		byte[] array = new byte[stream.Length];
		stream.Read(array, 0, (int)stream.Length);
		MemoryStream memoryStream = new MemoryStream(array, 0, array.Length);
		fileStream = memoryStream;
		writable = !readOnly;
		stream.Position = 0L;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (disposing && fileStream != null)
		{
			fileStream.Dispose();
			fileStream = null;
		}
	}

	~FileBackedDataSource()
	{
		Dispose(disposing: false);
	}

	public override ByteBuffer Read(int length, long position)
	{
		if (position >= Size)
		{
			throw new IndexOutOfRangeException("Position " + position + " past the end of the file");
		}
		ByteBuffer byteBuffer;
		if (writable)
		{
			byteBuffer = ByteBuffer.CreateBuffer(length);
			buffersToClean.Add(byteBuffer);
		}
		else
		{
			fileStream.Position = position;
			byteBuffer = ByteBuffer.CreateBuffer(length);
			if (IOUtils.ReadFully(fileStream, byteBuffer.Buffer) == -1)
			{
				throw new IndexOutOfRangeException("Position " + position + " past the end of the file");
			}
		}
		byteBuffer.Position = 0;
		return byteBuffer;
	}

	public override void Write(ByteBuffer src, long position)
	{
		fileStream.Write(src.Buffer, (int)position, src.Length);
	}

	public override void CopyTo(Stream stream)
	{
		byte[] array = fileStream.ToArray();
		stream.Write(array, 0, array.Length);
	}

	public override void Close()
	{
		foreach (ByteBuffer item in buffersToClean)
		{
			unmap(item);
		}
		buffersToClean.Clear();
		if (fileStream != null)
		{
			fileStream.Close();
		}
	}

	private static void unmap(ByteBuffer bb)
	{
	}
}
