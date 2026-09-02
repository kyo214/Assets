namespace Fusion.KCC;

public sealed class KCCModifier : KCCInteraction<KCCModifier>
{
	public IKCCProcessor Processor;

	public override void Initialize()
	{
		Processor = ((Provider is IKCCProcessorProvider iKCCProcessorProvider) ? iKCCProcessorProvider.GetProcessor() : null);
	}

	public override void Deinitialize()
	{
		Processor = null;
	}

	public override void CopyFromOther(KCCModifier other)
	{
		Processor = other.Processor;
	}
}
