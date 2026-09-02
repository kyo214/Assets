using System.IO;
using UnityEngine;

namespace Unity.Services.Analytics.Internal;

internal class FileSystemCalls : IFileSystemCalls
{
	private readonly bool m_CanAccessFileSystem;

	internal FileSystemCalls()
	{
		m_CanAccessFileSystem = Application.platform != RuntimePlatform.Switch && Application.platform != RuntimePlatform.GameCoreXboxOne && Application.platform != RuntimePlatform.GameCoreXboxSeries && Application.platform != RuntimePlatform.PS5 && Application.platform != RuntimePlatform.PS4 && !string.IsNullOrEmpty(Application.persistentDataPath);
	}

	public bool CanAccessFileSystem()
	{
		return m_CanAccessFileSystem;
	}

	public bool FileExists(string path)
	{
		return File.Exists(path);
	}

	public void DeleteFile(string path)
	{
		File.Delete(path);
	}

	public Stream OpenFileForWriting(string path)
	{
		return new FileStream(path, FileMode.Create);
	}

	public Stream OpenFileForReading(string path)
	{
		return new FileStream(path, FileMode.Open);
	}
}
