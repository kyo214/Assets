using System.Collections;
using System.Collections.Generic;
using System.IO;
using NPOI.POIFS.EventFileSystem;
using NPOI.Util;

namespace NPOI.POIFS.FileSystem;

public interface DirectoryEntry : Entry, IEnumerable<Entry>, IEnumerable
{
	IEnumerator<Entry> Entries { get; }

	List<string> EntryNames { get; }

	bool IsEmpty { get; }

	int EntryCount { get; }

	ClassID StorageClsid { get; set; }

	Entry GetEntry(string name);

	DocumentEntry CreateDocument(string name, Stream stream);

	DocumentEntry CreateDocument(string name, int size, POIFSWriterListener writer);

	DirectoryEntry CreateDirectory(string name);

	bool HasEntry(string name);
}
