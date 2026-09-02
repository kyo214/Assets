using UnityEngine;

namespace Fusion.KCC;

[DisallowMultipleComponent]
[RequireComponent(typeof(NetworkObject))]
public abstract class KCCProcessor : BaseKCCProcessor, IKCCProcessorProvider, IKCCInteractionProvider
{
	public virtual bool CanStartInteraction(KCC kcc, KCCData data)
	{
		return true;
	}

	public virtual bool CanStopInteraction(KCC kcc, KCCData data)
	{
		return true;
	}

	IKCCProcessor IKCCProcessorProvider.GetProcessor()
	{
		return this;
	}
}
