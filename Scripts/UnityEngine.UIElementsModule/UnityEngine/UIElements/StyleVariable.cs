namespace UnityEngine.UIElements;

internal struct StyleVariable(string name, StyleSheet sheet, StyleValueHandle[] handles)
{
	public readonly string name = name;

	public readonly StyleSheet sheet = sheet;

	public readonly StyleValueHandle[] handles = handles;

	public override int GetHashCode()
	{
		int hashCode = name.GetHashCode();
		hashCode = (hashCode * 397) ^ sheet.GetHashCode();
		return (hashCode * 397) ^ handles.GetHashCode();
	}
}
