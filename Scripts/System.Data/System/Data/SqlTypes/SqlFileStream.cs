using System.IO;
using System.Runtime.InteropServices;
using Unity;

namespace System.Data.SqlTypes;

public sealed class SqlFileStream : Stream
{
	public override bool CanRead
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default;
		}
	}

	public override bool CanSeek
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default;
		}
	}

	public override bool CanWrite
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default;
		}
	}

	public override long Length
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default;
		}
	}

	public string Name
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}

	public override long Position
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default;
		}
		set
		{
			Unity.ThrowStub.ThrowNotSupportedException();
		}
	}

	public byte[] TransactionContext
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}

	public SqlFileStream(string path, byte[] transactionContext, FileAccess access)
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}

	public SqlFileStream(string path, byte[] transactionContext, FileAccess access, FileOptions options, long allocationSize)
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}

	public override void Flush()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}

	public override int Read([In][Out] byte[] buffer, int offset, int count)
	{
		Unity.ThrowStub.ThrowNotSupportedException();
		return default;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		Unity.ThrowStub.ThrowNotSupportedException();
		return default;
	}

	public override void SetLength(long value)
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
