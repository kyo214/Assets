using System;
using UnityEngine;

namespace MoreMountains.Tools;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
public class MMInspectorGroupAttribute : PropertyAttribute
{
	public string GroupName;

	public bool GroupAllFieldsUntilNextGroupAttribute;

	public int GroupColorIndex;

	public MMInspectorGroupAttribute(string groupName, bool groupAllFieldsUntilNextGroupAttribute = false, int groupColorIndex = 24)
	{
		if (groupColorIndex > 139)
		{
			groupColorIndex = 139;
		}
		GroupName = groupName;
		GroupAllFieldsUntilNextGroupAttribute = groupAllFieldsUntilNextGroupAttribute;
		GroupColorIndex = groupColorIndex;
	}
}
