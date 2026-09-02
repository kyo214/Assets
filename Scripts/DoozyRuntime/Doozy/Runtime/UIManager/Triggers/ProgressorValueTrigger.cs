using Doozy.Runtime.Reactor;
using Doozy.Runtime.UIManager.Triggers.Internal;

namespace Doozy.Runtime.UIManager.Triggers;

public class ProgressorValueTrigger : BaseValueTrigger<Progressor>
{
	protected override float value => Target.currentValue;
}
