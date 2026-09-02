using System.Collections.Generic;
using System.IO;
using NPOI.POIFS.FileSystem;

namespace NPOI.HPSF;

public class HPSFPropertiesOnlyDocument : POIDocument
{
	public HPSFPropertiesOnlyDocument(NPOIFSFileSystem fs)
		: base(fs.Root)
	{
	}

	public HPSFPropertiesOnlyDocument(OPOIFSFileSystem fs)
		: base(fs)
	{
	}

	public HPSFPropertiesOnlyDocument(POIFSFileSystem fs)
		: base(fs)
	{
	}

	public override void Write()
	{
		NPOIFSFileSystem fileSystem = directory.FileSystem;
		ValidateInPlaceWritePossible();
		WriteProperties(fileSystem, null);
		fileSystem.WriteFileSystem();
	}

	public override void Write(FileInfo newFile)
	{
		POIFSFileSystem pOIFSFileSystem = POIFSFileSystem.Create(newFile);
		try
		{
			Write(pOIFSFileSystem);
			pOIFSFileSystem.WriteFileSystem();
		}
		finally
		{
			pOIFSFileSystem.Close();
		}
	}

	public override void Write(Stream out1)
	{
		NPOIFSFileSystem nPOIFSFileSystem = new NPOIFSFileSystem();
		try
		{
			Write(nPOIFSFileSystem);
			nPOIFSFileSystem.WriteFileSystem(out1);
		}
		finally
		{
			nPOIFSFileSystem.Close();
		}
	}

	private void Write(NPOIFSFileSystem fs)
	{
		List<string> list = new List<string>(1);
		WriteProperties(fs, list);
		EntryUtils.CopyNodes(directory, fs.Root, list);
	}
}
