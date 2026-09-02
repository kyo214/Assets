namespace UnityEngine.UIElements;

internal struct RuleMatcher(StyleSheet sheet, StyleComplexSelector complexSelector, int styleSheetIndexInStack)
{
	public StyleSheet sheet = sheet;

	public StyleComplexSelector complexSelector = complexSelector;

	public override string ToString()
	{
		return complexSelector.ToString();
	}
}
