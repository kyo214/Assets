namespace Doozy.Runtime.UIManager;

public enum StepperState
{
	Reset = 0,
	ValueChanged = 1,
	ValueIncremented = 2,
	ValueDecremented = 3,
	ReachedMinValue = 4,
	ReachedMaxValue = 5
}
