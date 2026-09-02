using System;

namespace Fusion;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
public class BehaviourButtonActionAttribute : BehaviourActionAttribute
{
	internal readonly string ButtonName;

	public BehaviourButtonActionAttribute(string buttonName, string executeMethod = null, string conditionMember = null)
		: base(executeMethod, conditionMember)
	{
		ButtonName = buttonName;
	}

	public BehaviourButtonActionAttribute(string buttonName, string executeMethod, bool showWhileRunning, bool showWhileNotRunning, string conditionMember = null)
		: base(executeMethod, conditionMember, (ActionFlags)((showWhileRunning ? 1 : 0) | (showWhileNotRunning ? 2 : 0)))
	{
		ButtonName = buttonName;
	}

	public BehaviourButtonActionAttribute(string buttonName, bool showWhileRunning, bool showWhileNotRunning, string conditionMember = null)
		: base(null, conditionMember, (ActionFlags)((showWhileRunning ? 1 : 0) | (showWhileNotRunning ? 2 : 0)))
	{
		ButtonName = buttonName;
	}
}
