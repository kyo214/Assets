using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Triggers.Internal;

namespace Doozy.Runtime.UIManager.Triggers;

public class UIStepperValueTrigger : BaseValueTrigger<UIStepper>
{
	protected override float value => Target.value;
}
