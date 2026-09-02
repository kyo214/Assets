using UnityEngine;

namespace Fusion.KCC;

[RequireComponent(typeof(NetworkObject))]
public sealed class NetworkAoIKCCProcessorProvider : MonoBehaviour, IKCCProcessorProvider, IKCCInteractionProvider
{
	[SerializeField]
	private NetworkAoIKCCProcessor _processor;

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
