using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace NPOI.OpenXml4Net.Util;

public interface ZipEntrySource
{
	IEnumerator Entries { get; }

	bool IsClosed { get; }

	Stream GetInputStream(ZipEntry entry);

	void Close();
}
