using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGDna
{
	protected readonly List<BGDnaMeta> Metas = new List<BGDnaMeta>();

	public bool IsObsolete
	{
		get
		{
			int count = Metas.Count;
			for (int i = 0; i < count; i++)
			{
				BGDnaMeta bGDnaMeta = Metas[i];
				if (bGDnaMeta.Meta == null || bGDnaMeta.Meta.IsDeleted)
				{
					return true;
				}
			}
			return false;
		}
	}

	public virtual void Add(BGDnaMeta meta)
	{
		Metas.Add(meta);
	}

	public void Bind(BGRepo repo)
	{
		foreach (BGDnaMeta meta in Metas)
		{
			meta.Bind(repo);
		}
	}
}
