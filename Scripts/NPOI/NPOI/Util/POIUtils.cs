using System.Collections;
using System.Collections.Generic;
using NPOI.POIFS.FileSystem;

namespace NPOI.Util;

public class POIUtils
{
	public static void CopyNodeRecursively(Entry entry, DirectoryEntry target)
	{
		DirectoryEntry directoryEntry = null;
		if (entry.IsDirectoryEntry)
		{
			directoryEntry = target.CreateDirectory(entry.Name);
			IEnumerator entries = ((DirectoryEntry)entry).Entries;
			while (entries.MoveNext())
			{
				CopyNodeRecursively((Entry)entries.Current, directoryEntry);
			}
			return;
		}
		DocumentEntry documentEntry = (DocumentEntry)entry;
		using DocumentInputStream stream = new DocumentInputStream(documentEntry);
		target.CreateDocument(documentEntry.Name, stream);
	}

	public static void CopyNodes(DirectoryEntry sourceRoot, DirectoryEntry targetRoot, List<string> excepts)
	{
		IEnumerator entries = sourceRoot.Entries;
		while (entries.MoveNext())
		{
			Entry entry = (Entry)entries.Current;
			if (!excepts.Contains(entry.Name))
			{
				CopyNodeRecursively(entry, targetRoot);
			}
		}
	}

	public static void CopyNodes(POIFSFileSystem source, POIFSFileSystem target, List<string> excepts)
	{
		CopyNodes(source.Root, target.Root, excepts);
	}
}
