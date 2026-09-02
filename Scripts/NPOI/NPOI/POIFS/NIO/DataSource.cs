using System.IO;
using NPOI.Util;

namespace NPOI.POIFS.NIO;

public abstract class DataSource
{
	public abstract long Size { get; }

	public abstract ByteBuffer Read(int length, long position);

	public abstract void Write(ByteBuffer src, long position);

	public abstract void Close();

	public abstract void CopyTo(Stream stream);
}
