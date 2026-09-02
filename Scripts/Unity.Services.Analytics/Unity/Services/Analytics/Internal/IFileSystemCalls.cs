using System.IO;

namespace Unity.Services.Analytics.Internal;

internal interface IFileSystemCalls
{
	bool CanAccessFileSystem();

	bool FileExists(string path);

	void DeleteFile(string path);

	Stream OpenFileForWriting(string path);

	Stream OpenFileForReading(string path);
}
