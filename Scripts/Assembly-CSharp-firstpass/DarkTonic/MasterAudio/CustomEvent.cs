using System;
using System.Collections.Generic;

namespace DarkTonic.MasterAudio;

[Serializable]
public class CustomEvent
{
	public string EventName;

	public string ProspectiveName;

	public bool IsEditing;

	public bool eventExpanded = true;

	public MasterAudio.CustomEventReceiveMode eventReceiveMode;

	public float distanceThreshold = 1f;

	public MasterAudio.EventReceiveFilter eventRcvFilterMode;

	public int filterModeQty = 1;

	public bool isTemporary;

	public int frameLastFired = -1;

	public string categoryName = "[Uncategorized]";

	private readonly List<int> _actorInstanceIds = new List<int>();

	public bool HasLiveActors => _actorInstanceIds.Count > 0;

	public CustomEvent(string eventName)
	{
		EventName = eventName;
		ProspectiveName = eventName;
	}

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
