using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCCollision : KCCInteraction<KCCCollision>
{
	public Collider Collider;

	public IKCCProcessor Processor;

	public override void Initialize()
	{
		Collider = NetworkObject.GetComponentNoAlloc<Collider>();
		Processor = ((Provider is IKCCProcessorProvider iKCCProcessorProvider) ? iKCCProcessorProvider.GetProcessor() : null);
	}

	public override void Deinitialize()
	{
		Collider = null;
		Processor = null;
	}

	public override void CopyFromOther(KCCCollision other)
	{
		Collider = other.Collider;
		Processor = other.Processor;
	}
}
