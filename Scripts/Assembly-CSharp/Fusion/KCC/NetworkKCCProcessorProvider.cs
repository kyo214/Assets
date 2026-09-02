using UnityEngine;

namespace Fusion.KCC;

[RequireComponent(typeof(NetworkObject))]
public sealed class NetworkKCCProcessorProvider : MonoBehaviour, IKCCProcessorProvider, IKCCInteractionProvider
{
	[SerializeField]
	private NetworkKCCProcessor _processor;

	bool IKCCInteractionProvider.CanStartInteraction(KCC kcc, KCCData data)
	{
		return true;
	}

	bool IKCCInteractionProvider.CanStopInteraction(KCC kcc, KCCData data)
	{
		return true;
	}

	IKCCProcessor IKCCProcessorProvider.GetProcessor()
	{
		return _processor;
	}
}
