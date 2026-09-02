using System;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.UIManager.Components;

namespace Doozy.Runtime.UIManager;

[Serializable]
public struct UISliderSignalData
{
	public string sliderCategory { get; private set; }

	public string sliderName { get; private set; }

	public SliderState sliderState { get; private set; }

	public UISlider slider { get; private set; }

	public UISliderSignalData(string sliderCategory, string sliderName, SliderState sliderState, UISlider slider = null)
	{
		this.sliderCategory = sliderCategory;
		this.sliderName = sliderName;
		this.sliderState = sliderState;
		this.slider = slider;
	}

	public override string ToString()
	{
		return "(" + ObjectNames.NicifyVariableName(sliderState.ToString()) + ") " + sliderCategory + " / " + sliderName;
	}
}
