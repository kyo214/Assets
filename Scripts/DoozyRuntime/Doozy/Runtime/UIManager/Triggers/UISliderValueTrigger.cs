using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Triggers.Internal;

namespace Doozy.Runtime.UIManager.Triggers;

public class UISliderValueTrigger : BaseValueTrigger<UISlider>
{
	protected override float value => Target.value;
}
