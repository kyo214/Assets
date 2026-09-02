using System;

namespace MoreMountains.Feedbacks;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class MMFHiddenPropertiesAttribute : Attribute
{
	public string[] PropertiesNames;

	public MMFHiddenPropertiesAttribute(params string[] propertiesNames)
	{
		PropertiesNames = propertiesNames;
	}
}
