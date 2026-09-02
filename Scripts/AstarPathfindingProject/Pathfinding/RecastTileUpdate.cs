using System;
using UnityEngine;

namespace Pathfinding;

[AddComponentMenu("Pathfinding/Navmesh/RecastTileUpdate")]
[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_recast_tile_update.php")]
public class RecastTileUpdate : MonoBehaviour
{
	public static event Action<Bounds> OnNeedUpdates;

	private void Start()
	{
		ScheduleUpdate();
	}

	private void OnDestroy()
	{
		ScheduleUpdate();
	}

	public void ScheduleUpdate()
	{
		Collider component = GetComponent<Collider>();
		if (component != null)
		{
			if (OnNeedUpdates != null)
			{
				OnNeedUpdates(component.bounds);
			}
		}
		else if (OnNeedUpdates != null)
		{
			OnNeedUpdates(new Bounds(base.transform.position, Vector3.zero));
		}
	}
}
