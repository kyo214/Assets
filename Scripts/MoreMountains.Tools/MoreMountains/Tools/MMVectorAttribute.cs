using UnityEngine;

namespace MoreMountains.Tools;

public class MMVectorAttribute : PropertyAttribute
{
	public readonly string[] Labels;

	public MMVectorAttribute(params string[] labels)
	{
		Labels = labels;
	}
}
