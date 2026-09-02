using System;

namespace Fusion;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class WarnIfAttribute : DoIfAttribute
{
	public string Message;

	public int MsgType;

	public string MsgProvider;

	public string MsgTypeProvider;

	public string ActionMethod;

	public bool UseMsgIconOnly;

	public WarnIfAttribute()
		: base(null)
	{
		MsgType = 2;
		Compare = DoIfCompareOperator.NotZero;
		base.order = -10000;
	}

	public WarnIfAttribute(string message)
		: base(null)
	{
		Message = message;
		MsgType = 2;
		Compare = DoIfCompareOperator.NotZero;
		base.order = -10000;
	}

	public WarnIfAttribute(string conditionMember, string message)
		: base(conditionMember)
	{
		Message = message;
		MsgType = 2;
		Compare = DoIfCompareOperator.NotZero;
		base.order = -10000;
	}

	public WarnIfAttribute(string conditionMember, double compareToValue, string message)
		: base(conditionMember, compareToValue)
	{
		Message = message;
		Compare = DoIfCompareOperator.Equal;
		base.order = -10000;
	}

	public WarnIfAttribute(string conditionMember, bool compareToValue, string message)
		: base(conditionMember, compareToValue)
	{
		Message = message;
		MsgType = 2;
		Compare = DoIfCompareOperator.Equal;
		base.order = -10000;
	}
}
