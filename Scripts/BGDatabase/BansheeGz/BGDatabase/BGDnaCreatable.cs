namespace BansheeGz.BGDatabase;

public class BGDnaCreatable : BGDna, BGDnaCreatable.CreatableI
{
	public interface CreatableI
	{
		void Create(BGRepo repo, string addon);
	}

	public void Create(BGRepo repo, string addon)
	{
		repo.Transaction(() =>
		{
			foreach (BGDnaMeta meta in Metas)
			{
				((CreatableI)meta).Create(repo, addon);
			}
			foreach (BGDnaMeta meta2 in Metas)
			{
				foreach (BGDnaField field in meta2.Fields)
				{
					((CreatableI)field).Create(repo, addon);
				}
			}
		});
	}

	public virtual void Delete(BGRepo repo)
	{
		repo.Transaction(() =>
		{
			foreach (BGDnaMeta meta in Metas)
			{
				repo.GetMeta(meta.DnaName)?.Delete();
			}
		});
	}
}
