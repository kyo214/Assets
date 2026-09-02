namespace Fusion.KCC;

public abstract class KCCInteraction<TInteraction> where TInteraction : KCCInteraction<TInteraction>, new()
{
	public KCCNetworkID NetworkID;

	public NetworkObject NetworkObject;

	public IKCCInteractionProvider Provider;

	public abstract void Initialize();

	public abstract void Deinitialize();

	public abstract void CopyFromOther(TInteraction other);
}
