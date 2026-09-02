using System;

namespace Fusion;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field)]
public class InlineHelpAttribute : DecoratingPropertyAttribute
{
	private new const int DefaultOrder = -9000;

	public InlineHelpButtonPlacement ButtonPlacement = InlineHelpButtonPlacement.BeforeLabel;

	public InlineHelpAttribute()
		: base(-9000)
	{
	}
}
