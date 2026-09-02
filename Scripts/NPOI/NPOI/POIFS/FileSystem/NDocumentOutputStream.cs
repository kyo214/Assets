using System.IO;
using NPOI.POIFS.Properties;

namespace NPOI.POIFS.FileSystem;

public class NDocumentOutputStream : MemoryStream
{
	private int _document_size;

	private bool _closed;

	private NPOIFSDocument _document;

	private DocumentProperty _property;

	private MemoryStream _buffer = new MemoryStream(4096);

	private NPOIFSStream _stream;

	private Stream _stream_output;

	public NDocumentOutputStream(DocumentEntry document)
	{
		if (!(document is DocumentNode))
		{
			throw new IOException("Cannot open internal document storage, " + document?.ToString() + " not a Document Node");
		}
		_document_size = 0;
		_closed = false;
		_property = (DocumentProperty)((DocumentNode)document).Property;
		_document = new NPOIFSDocument((DocumentNode)document);
		_document.Free();
	}

	public NDocumentOutputStream(DirectoryEntry parent, string name)
	{
		if (!(parent is DirectoryNode))
		{
			throw new IOException("Cannot open internal directory storage, " + parent?.ToString() + " not a Directory Node");
		}
		_document_size = 0;
		_closed = false;
		DocumentEntry documentEntry = parent.CreateDocument(name, new MemoryStream(new byte[0]));
		_property = (DocumentProperty)((DocumentNode)documentEntry).Property;
		_document = new NPOIFSDocument((DocumentNode)documentEntry);
	}

	private void dieIfClosed()
	{
		if (_closed)
		{
			throw new IOException("cannot perform requested operation on a closed stream");
		}
	}

	private void CheckBufferSize()
	{
		if (_buffer.Length > 4096)
		{
			byte[] array = _buffer.ToArray();
			_buffer = null;
			Write(array, 0, array.Length);
		}
	}

	public void Write(int b)
	{
		dieIfClosed();
		if (_buffer != null)
		{
			_buffer.WriteByte((byte)b);
			CheckBufferSize();
		}
		else
		{
			Write(new byte[1] { (byte)b });
		}
	}

	public void Write(byte[] b)
	{
		dieIfClosed();
		if (_buffer != null)
		{
			_buffer.Write(b, 0, b.Length);
			CheckBufferSize();
		}
		else
		{
			Write(b, 0, b.Length);
		}
	}

	public override void Write(byte[] b, int off, int len)
	{
		dieIfClosed();
		if (_buffer != null)
		{
			_buffer.Write(b, off, len);
			CheckBufferSize();
			return;
		}
		if (_stream == null)
		{
			_stream = new NPOIFSStream(_document.FileSystem);
			_stream_output = _stream.GetOutputStream();
		}
		_stream_output.Write(b, off, len);
		_document_size += len;
	}

	public override void Close()
	{
		base.Close();
		if (_buffer != null)
		{
			_document.ReplaceContents(new MemoryStream(_buffer.ToArray()));
		}
		else
		{
			_stream_output.Close();
			_property.UpdateSize(_document_size);
			_property.StartBlock = _stream.GetStartBlock();
		}
		_closed = true;
	}
}
