namespace System.Reflection;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
internal sealed class AssemblyMetadataAttribute : Attribute
{
	public string Key { get; }

	public string Value { get; }

	public AssemblyMetadataAttribute(string key, string value)
	{
		Key = key;
		Value = value;
	}
}
