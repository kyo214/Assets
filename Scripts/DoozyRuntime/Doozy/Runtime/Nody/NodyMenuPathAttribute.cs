using System;

namespace Doozy.Runtime.Nody;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class NodyMenuPathAttribute : Attribute
{
	public string category { get; }

	public string name { get; }

	public NodyMenuPathAttribute(string category, string name)
	{
		this.category = category;
		this.name = name;
	}
}
