namespace NPOI.POIFS.FileSystem;

public interface Entry
{
	string Name { get; }

	bool IsDirectoryEntry { get; }

	bool IsDocumentEntry { get; }

	DirectoryEntry Parent { get; }

	bool Delete();

	bool RenameTo(string newName);
}
