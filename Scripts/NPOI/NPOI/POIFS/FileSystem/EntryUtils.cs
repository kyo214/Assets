using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NPOI.POIFS.FileSystem;

public class EntryUtils
{
	public static void CopyNodeRecursively(Entry entry, DirectoryEntry target)
	{
		DirectoryEntry directoryEntry = null;
		if (entry.IsDirectoryEntry)
		{
			DirectoryEntry directoryEntry2 = (DirectoryEntry)entry;
			directoryEntry = target.CreateDirectory(entry.Name);
			directoryEntry.StorageClsid = directoryEntry2.StorageClsid;
			IEnumerator<Entry> entries = directoryEntry2.Entries;
			while (entries.MoveNext())
			{
				CopyNodeRecursively(entries.Current, directoryEntry);
			}
		}
		else
		{
			DocumentEntry documentEntry = (DocumentEntry)entry;
			DocumentInputStream documentInputStream = new DocumentInputStream(documentEntry);
			target.CreateDocument(documentEntry.Name, documentInputStream);
			documentInputStream.Close();
		}
	}

	public static void CopyNodes(DirectoryEntry sourceRoot, DirectoryEntry targetRoot)
	{
		foreach (Entry item in sourceRoot)
		{
			CopyNodeRecursively(item, targetRoot);
		}
	}

	public static void CopyNodes(FilteringDirectoryNode filteredSource, FilteringDirectoryNode filteredTarget)
	{
		CopyNodes((DirectoryEntry)filteredSource, (DirectoryEntry)filteredTarget);
	}

	[Obsolete]
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

	public static void CopyNodes(OPOIFSFileSystem source, OPOIFSFileSystem target)
	{
		CopyNodes(source.Root, target.Root);
	}

	public static void CopyNodes(NPOIFSFileSystem source, NPOIFSFileSystem target)
	{
		CopyNodes(source.Root, target.Root);
	}

	public static void CopyNodes(OPOIFSFileSystem source, OPOIFSFileSystem target, List<string> excepts)
	{
		CopyNodes(new FilteringDirectoryNode(source.Root, excepts), new FilteringDirectoryNode(target.Root, excepts));
	}

	public static void CopyNodes(NPOIFSFileSystem source, NPOIFSFileSystem target, List<string> excepts)
	{
		CopyNodes(new FilteringDirectoryNode(source.Root, excepts), new FilteringDirectoryNode(target.Root, excepts));
	}

	public static bool AreDirectoriesIdentical(DirectoryEntry dirA, DirectoryEntry dirB)
	{
		if (!dirA.Name.Equals(dirB.Name))
		{
			return false;
		}
		if (dirA.EntryCount != dirB.EntryCount)
		{
			return false;
		}
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		int num = -12345;
		foreach (Entry item in dirA)
		{
			string name = item.Name;
			if (item.IsDirectoryEntry)
			{
				dictionary.Add(name, num);
			}
			else
			{
				dictionary.Add(name, ((DocumentNode)item).Size);
			}
		}
		foreach (Entry item2 in dirB)
		{
			string name2 = item2.Name;
			if (!dictionary.ContainsKey(name2))
			{
				return false;
			}
			int num2 = ((!item2.IsDirectoryEntry) ? ((DocumentNode)item2).Size : num);
			if (num2 != dictionary[name2])
			{
				return false;
			}
			dictionary.Remove(name2);
		}
		if (dictionary.Count != 0)
		{
			return false;
		}
		foreach (Entry item3 in dirA)
		{
			try
			{
				Entry entry = dirB.GetEntry(item3.Name);
				if (!((!item3.IsDirectoryEntry) ? AreDocumentsIdentical((DocumentEntry)item3, (DocumentEntry)entry) : AreDirectoriesIdentical((DirectoryEntry)item3, (DirectoryEntry)entry)))
				{
					return false;
				}
			}
			catch (FileNotFoundException)
			{
				return false;
			}
			catch (IOException)
			{
				return false;
			}
		}
		return true;
	}

	public static bool AreDocumentsIdentical(DocumentEntry docA, DocumentEntry docB)
	{
		if (!docA.Name.Equals(docB.Name))
		{
			return false;
		}
		if (docA.Size != docB.Size)
		{
			return false;
		}
		bool result = true;
		DocumentInputStream documentInputStream = null;
		DocumentInputStream documentInputStream2 = null;
		try
		{
			documentInputStream = new DocumentInputStream(docA);
			documentInputStream2 = new DocumentInputStream(docB);
			int num;
			int num2;
			do
			{
				num = documentInputStream.Read();
				num2 = documentInputStream2.Read();
				if (num != num2)
				{
					result = false;
					break;
				}
			}
			while (num != -1 && num2 != -1);
		}
		finally
		{
			documentInputStream?.Close();
			documentInputStream2?.Close();
		}
		return result;
	}
}
