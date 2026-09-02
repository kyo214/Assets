using System;

namespace Fusion;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
public class BehaviourWarnAttribute : BehaviourActionAttribute
{
	internal readonly string WarnText;

	public BehaviourWarnAttribute(string warnText, string conditionMember)
		: base(null, conditionMember, ActionFlags.ShowAtNotRuntime)
	{
		WarnText = warnText;
	}
}
