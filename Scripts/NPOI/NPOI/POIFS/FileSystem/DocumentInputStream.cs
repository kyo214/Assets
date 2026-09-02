using System.IO;
using NPOI.Util;

namespace NPOI.POIFS.FileSystem;

public class DocumentInputStream : ByteArrayInputStream, ILittleEndianInput
{
	protected static int EOF = -1;

	protected static int SIZE_SHORT = 2;

	protected static int SIZE_INT = 4;

	protected static int SIZE_LONG = 8;

	private DocumentInputStream delegate1;

	public override long Length => delegate1.Length;

	public override long Position
	{
		get
		{
			return delegate1.Position;
		}
		set
		{
			delegate1.Position = value;
		}
	}

	protected DocumentInputStream()
	{
	}

	public DocumentInputStream(DocumentEntry document)
	{
		if (!(document is DocumentNode))
		{
			throw new IOException("Cannot open internal document storage");
		}
		DocumentNode obj = (DocumentNode)document;
		DirectoryNode directoryNode = (DirectoryNode)document.Parent;
		if (obj.Document != null)
		{
			delegate1 = new ODocumentInputStream(document);
			return;
		}
		if (directoryNode.OFileSystem != null)
		{
			delegate1 = new ODocumentInputStream(document);
			return;
		}
		if (directoryNode.NFileSystem != null)
		{
			delegate1 = new NDocumentInputStream(document);
			return;
		}
		throw new IOException("No FileSystem bound on the parent, can't read contents");
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return delegate1.Seek(offset, origin);
	}

	public DocumentInputStream(OPOIFSDocument document)
	{
		delegate1 = new ODocumentInputStream(document);
	}

	public DocumentInputStream(NPOIFSDocument document)
	{
		delegate1 = new NDocumentInputStream(document);
	}

	public override int Available()
	{
		return delegate1.Available();
	}

	public override void Close()
	{
		delegate1.Close();
	}

	public override void Mark(int ignoredReadlimit)
	{
		delegate1.Mark(ignoredReadlimit);
	}

	public override bool MarkSupported()
	{
		return true;
	}

	public override int Read()
	{
		return delegate1.Read();
	}

	public override int Read(byte[] b)
	{
		return Read(b, 0, b.Length);
	}

	public override int Read(byte[] b, int off, int len)
	{
		return delegate1.Read(b, off, len);
	}

	public override void Reset()
	{
		delegate1.Reset();
	}

	public new virtual long Skip(long n)
	{
		return delegate1.Skip(n);
	}

	public override int ReadByte()
	{
		return delegate1.ReadByte();
	}

	public virtual double ReadDouble()
	{
		return delegate1.ReadDouble();
	}

	public virtual short ReadShort()
	{
		return (short)ReadUShort();
	}

	public virtual void ReadFully(byte[] buf)
	{
		ReadFully(buf, 0, buf.Length);
	}

	public virtual void ReadFully(byte[] buf, int off, int len)
	{
		delegate1.ReadFully(buf, off, len);
	}

	public virtual long ReadLong()
	{
		return delegate1.ReadLong();
	}

	public virtual int ReadInt()
	{
		return delegate1.ReadInt();
	}

	public virtual int ReadUShort()
	{
		return delegate1.ReadUShort();
	}

	public virtual int ReadUByte()
	{
		return delegate1.ReadUByte();
	}
}
