namespace Fusion;

public abstract class DoIfAttribute : DecoratingPropertyAttribute
{
	public string ConditionMember;

	public double CompareToValue;

	public DoIfCompareOperator Compare;

	public DoIfAttribute(string conditionMemberName, double compareToValue)
	{
		ConditionMember = conditionMemberName;
		CompareToValue = compareToValue;
		Compare = DoIfCompareOperator.Equal;
	}

	public DoIfAttribute(string conditionMemberName, bool compareToValue)
	{
		ConditionMember = conditionMemberName;
		CompareToValue = (compareToValue ? 1 : 0);
		Compare = DoIfCompareOperator.Equal;
	}

	public DoIfAttribute(string conditionMemberName)
	{
		ConditionMember = conditionMemberName;
		Compare = DoIfCompareOperator.NotZero;
	}
}
