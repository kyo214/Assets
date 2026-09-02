using System;

namespace Fusion;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
public class BehaviourToggleAttribute : BehaviourActionAttribute
{
	public BehaviourToggleAttribute(string conditionMember = null, ActionFlags flags = ActionFlags.AlwaysShow)
		: base(null, conditionMember, flags)
	{
	}
}
