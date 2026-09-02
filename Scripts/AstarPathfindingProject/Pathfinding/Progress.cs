using UnityEngine;

namespace Pathfinding;

public struct Progress(float progress, string description)
{
	public readonly float progress = progress;

	public readonly string description = description;

	public Progress MapTo(float min, float max, string prefix = null)
	{
		return new Progress(Mathf.Lerp(min, max, progress), prefix + description);
	}

	public override string ToString()
	{
		float num = progress;
		return num.ToString("0.0") + " " + description;
	}
}
