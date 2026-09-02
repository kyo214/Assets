using System;

namespace Fusion;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class ScriptHelpAttribute : Attribute
{
	public string Url { get; set; }

	public EditorHeaderBackColor BackColor { get; set; } = EditorHeaderBackColor.Gray;

	public EditorHeaderIcon Icon { get; set; } = EditorHeaderIcon.FusionGray;

	public string Title { get; set; }
}
