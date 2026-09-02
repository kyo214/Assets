using System;

namespace Fusion;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class DrawIfAttribute : DoIfAttribute
{
	public bool Hide;

	private new const int DefaultOrder = -11000;

	public DrawIfAttribute(string conditionMemberName, double compareToValue)
		: base(conditionMemberName, compareToValue)
	{
		Compare = DoIfCompareOperator.Equal;
		base.order = -11000;
	}

	public DrawIfAttribute(string conditionMemberName, bool compareToValue)
		: base(conditionMemberName, compareToValue)
	{
		Compare = DoIfCompareOperator.Equal;
		base.order = -11000;
	}

	public DrawIfAttribute(string conditionMemberName)
		: base(conditionMemberName)
	{
		Compare = DoIfCompareOperator.NotZero;
		base.order = -11000;
	}
}
