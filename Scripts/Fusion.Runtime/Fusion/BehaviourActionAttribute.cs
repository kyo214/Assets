using System;

namespace Fusion;

[AttributeUsage(AttributeTargets.Method)]
public class BehaviourActionAttribute : Attribute
{
	[Flags]
	public enum ActionFlags
	{
		ShowAtRuntime = 1,
		ShowAtNotRuntime = 2,
		AlwaysShow = ShowAtRuntime | ShowAtNotRuntime,
		DirtyAfterButton = 4
	}

	public ActionFlags ConditionFlags;

	public string ExecuteMethod;

	public string ConditionMember;

	protected BehaviourActionAttribute(string executeMethod = null, string conditionMember = null, ActionFlags flags = ActionFlags.AlwaysShow)
	{
		ExecuteMethod = executeMethod;
		ConditionMember = conditionMember;
		ConditionFlags = flags;
	}

	public BehaviourActionAttribute(string conditionMember = null, ActionFlags flags = ActionFlags.AlwaysShow)
	{
		ExecuteMethod = null;
		ConditionMember = conditionMember;
		ConditionFlags = flags;
	}
}
