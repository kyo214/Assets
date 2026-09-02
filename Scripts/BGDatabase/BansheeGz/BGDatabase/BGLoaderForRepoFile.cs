using System.IO;

namespace BansheeGz.BGDatabase;

public class BGLoaderForRepoFile : BGLoaderForRepo
{
	private readonly string filePath;

	public override string Name => "File";

	public string FilePath => filePath;

	public BGLoaderForRepoFile(string filePath)
	{
		this.filePath = filePath;
	}

	public override byte[] Load(LoadRequest request)
	{
		byte[] array = null;
		if (IsEmpty(request))
		{
			return Load(filePath);
		}
		string path = ToPath(request);
		return Load(path);
	}

	protected override string ToPath(LoadRequest request)
	{
		string path = filePath ?? request.basePath;
		string basePath = Path.ChangeExtension(path, null);
		string path2 = BGLoaderForRepo.AppendPaths(basePath, request.paths);
		return Path.ChangeExtension(path2, "bytes");
	}

	public byte[] Load(string path)
	{
		if (!File.Exists(path))
		{
			return null;
		}
		return File.ReadAllBytes(path);
	}

	public BGRepo Load()
	{
		byte[] array = Load((LoadRequest)null);
		if (array == null)
		{
			return null;
		}
		BGRepo bGRepo = new BGRepo();
		bGRepo.Load(array);
		bGRepo.RepoLoader = this;
		return bGRepo;
	}
}
