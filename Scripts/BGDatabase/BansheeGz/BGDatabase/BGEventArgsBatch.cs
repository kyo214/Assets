using System.Collections.Generic;
using System.Text;

namespace BansheeGz.BGDatabase;

public class BGEventArgsBatch : BGEventArgsA
{
	private static readonly BGObjectPoolNTS<BGEventArgsBatch> pool = new BGObjectPoolNTS<BGEventArgsBatch>(() => new BGEventArgsBatch());

	public bool StructureChange;

	public bool EverythingChanged;

	private readonly HashSet<BGId> metaWithAddedEntities = new HashSet<BGId>();

	private readonly HashSet<BGId> metaWithDeletedEntities = new HashSet<BGId>();

	private readonly HashSet<BGId> metaWithUpdatedEntities = new HashSet<BGId>();

	private readonly HashSet<BGId> metaWithEntitiesOrderChanged = new HashSet<BGId>();

	protected override BGObjectPool Pool => pool;

	public BGRepo Repo { get; private set; }

	public bool IsEmpty
	{
		get
		{
			if (!StructureChange && !EverythingChanged && metaWithAddedEntities.Count == 0 && metaWithDeletedEntities.Count == 0 && metaWithUpdatedEntities.Count == 0)
			{
				return metaWithEntitiesOrderChanged.Count == 0;
			}
			return false;
		}
	}

	private BGEventArgsBatch()
	{
	}

	public static BGEventArgsBatch GetInstance(BGRepo repo)
	{
		BGEventArgsBatch bGEventArgsBatch = pool.Get();
		bGEventArgsBatch.Clear();
		bGEventArgsBatch.Repo = repo;
		return bGEventArgsBatch;
	}

	public void AddMetaWithAddedEntities(BGId metaId)
	{
		metaWithAddedEntities.Add(metaId);
	}

	public void AddMetaWithDeletedEntities(BGId metaId)
	{
		metaWithDeletedEntities.Add(metaId);
	}

	public void AddMetaWithUpdatedEntities(BGId metaId)
	{
		metaWithUpdatedEntities.Add(metaId);
	}

	public void AddMetaEntitiesOrderChanged(BGId metaId)
	{
		metaWithEntitiesOrderChanged.Add(metaId);
	}

	public bool WasEntitiesAdded(BGId metaId)
	{
		if (!EverythingChanged)
		{
			return metaWithAddedEntities.Contains(metaId);
		}
		return true;
	}

	public bool WasEntitiesDeleted(BGId metaId)
	{
		if (!EverythingChanged)
		{
			return metaWithDeletedEntities.Contains(metaId);
		}
		return true;
	}

	public bool WasEntitiesUpdated(BGId metaId)
	{
		if (!EverythingChanged)
		{
			return metaWithUpdatedEntities.Contains(metaId);
		}
		return true;
	}

	public bool WasEntitiesOrderChanged(BGId metaId)
	{
		if (!EverythingChanged)
		{
			return metaWithEntitiesOrderChanged.Contains(metaId);
		}
		return true;
	}

	public override void Clear()
	{
		Repo = null;
		StructureChange = false;
		EverythingChanged = false;
		metaWithAddedEntities.Clear();
		metaWithDeletedEntities.Clear();
		metaWithUpdatedEntities.Clear();
		metaWithEntitiesOrderChanged.Clear();
	}

	public override string ToString()
	{
		return $"BGEventArgsBatch: StructureChange [{StructureChange}], EverythingChanged [{EverythingChanged}], " + "added meta Ids [" + GetString(metaWithAddedEntities) + "], deleted meta Ids [" + GetString(metaWithDeletedEntities) + "]changed meta Ids [" + GetString(metaWithUpdatedEntities) + "]";
	}

	private static string GetString(HashSet<BGId> hashSet)
	{
		if (hashSet.Count == 0)
		{
			return "None";
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (BGId item in hashSet)
		{
			if (stringBuilder.Length != 0)
			{
				stringBuilder.Append('|');
			}
			stringBuilder.Append(item);
		}
		return stringBuilder.ToString();
	}
}
