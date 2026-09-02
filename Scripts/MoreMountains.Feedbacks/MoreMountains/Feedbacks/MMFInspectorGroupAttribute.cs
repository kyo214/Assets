using System;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
public class MMFInspectorGroupAttribute : PropertyAttribute
{
	public string GroupName;

	public bool GroupAllFieldsUntilNextGroupAttribute;

	public int GroupColorIndex;

	public bool RequiresSetup;

	public bool ClosedByDefault;

	public MMFInspectorGroupAttribute(string groupName, bool groupAllFieldsUntilNextGroupAttribute = false, int groupColorIndex = 24, bool requiresSetup = false, bool closedByDefault = false)
	{
		if (groupColorIndex > 139)
		{
			groupColorIndex = 139;
		}
		GroupName = groupName;
		GroupAllFieldsUntilNextGroupAttribute = groupAllFieldsUntilNextGroupAttribute;
		GroupColorIndex = groupColorIndex;
		RequiresSetup = requiresSetup;
		ClosedByDefault = closedByDefault;
	}
}
