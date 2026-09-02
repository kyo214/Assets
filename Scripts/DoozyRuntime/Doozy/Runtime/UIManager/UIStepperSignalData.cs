using System;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.UIManager.Components;

namespace Doozy.Runtime.UIManager;

[Serializable]
public struct UIStepperSignalData
{
	public string stepperCategory { get; private set; }

	public string stepperName { get; private set; }

	public StepperState stepperState { get; private set; }

	public UIStepper stepper { get; private set; }

	public UIStepperSignalData(string stepperCategory, string stepperName, StepperState stepperState, UIStepper stepper = null)
	{
		this.stepperCategory = stepperCategory;
		this.stepperName = stepperName;
		this.stepperState = stepperState;
		this.stepper = stepper;
	}

	public override string ToString()
	{
		return "(" + ObjectNames.NicifyVariableName(stepperState.ToString()) + ") " + stepperCategory + " / " + stepperName;
	}
}
