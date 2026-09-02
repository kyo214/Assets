namespace Fusion;

public struct NetworkBehaviourCallbackReference
{
	internal int IndexOffsetByOne;

	internal object Delegate;

	public bool IsValid => IndexOffsetByOne > 0 && Delegate != null;
}
