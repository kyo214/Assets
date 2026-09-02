namespace BansheeGz.BGDatabase;

public abstract class BGLoaderForRepo
{
	public class LoadRequest
	{
		public readonly BGRepoCustomLoaderModel.DatabaseResource databaseResource;

		public readonly string basePath;

		public readonly string[] paths;

		public LoadRequest(BGRepoCustomLoaderModel.DatabaseResource databaseResource)
		{
			this.databaseResource = databaseResource;
		}

		public LoadRequest(string basePath)
		{
			this.basePath = basePath;
		}

		public LoadRequest(string basePath, params string[] paths)
			: this(basePath)
		{
			this.paths = paths;
		}

		public string ToPath(BGLoaderForRepo loader)
		{
			return loader.ToPath(this);
		}
	}

	public abstract string Name { get; }

	public abstract byte[] Load(LoadRequest request);

	protected static string AppendPaths(string basePath, params string[] paths)
	{
		if (paths == null)
		{
			return basePath;
		}
		for (int i = 0; i < paths.Length; i++)
		{
			basePath += "_";
			basePath += paths[i];
		}
		return basePath;
	}

	protected bool IsEmpty(LoadRequest request)
	{
		if (request != null)
		{
			return string.IsNullOrEmpty(request.basePath);
		}
		return true;
	}

	protected abstract string ToPath(LoadRequest request);
}
