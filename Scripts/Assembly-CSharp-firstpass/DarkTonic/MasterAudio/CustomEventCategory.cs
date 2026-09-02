using System;
using System.Collections.Generic;

namespace DarkTonic.MasterAudio;

[Serializable]
public class CustomEventCategory
{
	public string CatName = "[Uncategorized]";

	public bool IsExpanded = true;

	public bool IsEditing;

	public bool IsTemporary;

	public string ProspectiveName = "[Uncategorized]";

	private readonly List<int> _actorInstanceIds = new List<int>();

	public bool HasLiveActors => _actorInstanceIds.Count > 0;

	public void AddActorInstanceId(int instanceId)
	{
		if (!_actorInstanceIds.Contains(instanceId))
		{
			_actorInstanceIds.Add(instanceId);
		}
	}

	public void RemoveActorInstanceId(int instanceId)
	{
		_actorInstanceIds.Remove(instanceId);
	}
}
