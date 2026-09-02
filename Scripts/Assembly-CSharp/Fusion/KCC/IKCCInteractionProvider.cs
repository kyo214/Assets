namespace Fusion.KCC;

public interface IKCCInteractionProvider
{
	bool CanStartInteraction(KCC kcc, KCCData data);

	bool CanStopInteraction(KCC kcc, KCCData data);
}
