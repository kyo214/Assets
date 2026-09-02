using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace NPOI.OpenXml4Net.Util;

public class ZipFileZipEntrySource : ZipEntrySource
{
	private ZipFile zipArchive;

	public bool IsClosed => zipArchive == null;

	public IEnumerator Entries
	{
		get
		{
			if (zipArchive == null)
			{
				throw new InvalidDataException("Zip File is closed");
			}
			return zipArchive.GetEnumerator();
		}
	}

	public ZipFileZipEntrySource(ZipFile zipFile)
	{
		zipArchive = zipFile;
	}

	public void Close()
	{
		if (zipArchive != null)
		{
			zipArchive.Close();
		}
		zipArchive = null;
	}

	public Stream GetInputStream(ZipEntry entry)
	{
		if (zipArchive == null)
		{
			throw new InvalidDataException("Zip File is closed");
		}
		return zipArchive.GetInputStream(entry);
	}
}
