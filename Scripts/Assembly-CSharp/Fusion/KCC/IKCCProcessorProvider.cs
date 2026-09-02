namespace Fusion.KCC;

public interface IKCCProcessorProvider : IKCCInteractionProvider
{
	IKCCProcessor GetProcessor();
}
