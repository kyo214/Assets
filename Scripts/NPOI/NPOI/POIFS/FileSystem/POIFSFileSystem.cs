using System;
using System.IO;
using NPOI.POIFS.Dev;

namespace NPOI.POIFS.FileSystem;

[Serializable]
public class POIFSFileSystem : NPOIFSFileSystem, POIFSViewable
{
	public new static Stream CreateNonClosingInputStream(Stream stream)
	{
		return new CloseIgnoringInputStream(stream);
	}

	public POIFSFileSystem()
	{
	}

	public POIFSFileSystem(Stream stream)
		: base(stream)
	{
	}

	public POIFSFileSystem(FileInfo file, bool readOnly)
		: base(file, readOnly)
	{
	}

	public POIFSFileSystem(FileInfo file)
		: base(file)
	{
	}

	public new static bool HasPOIFSHeader(Stream inp)
	{
		return NPOIFSFileSystem.HasPOIFSHeader(inp);
	}

	public new static bool HasPOIFSHeader(byte[] header8Bytes)
	{
		return NPOIFSFileSystem.HasPOIFSHeader(header8Bytes);
	}

	public static POIFSFileSystem Create(FileInfo file)
	{
		POIFSFileSystem pOIFSFileSystem = new POIFSFileSystem();
		FileStream fileStream = file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
		pOIFSFileSystem.WriteFileSystem(fileStream);
		fileStream.Close();
		pOIFSFileSystem.Close();
		return new POIFSFileSystem(file, readOnly: false);
	}
}
