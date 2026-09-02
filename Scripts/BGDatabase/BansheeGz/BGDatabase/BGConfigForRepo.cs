namespace BansheeGz.BGDatabase;

public class BGConfigForRepo
{
	private readonly string assetPath;

	private readonly int assetId;

	public string AssetPath => assetPath;

	public int AssetId => assetId;

	public BGConfigForRepo(string assetPath, int assetId)
	{
		this.assetPath = assetPath;
		this.assetId = assetId;
	}
}
