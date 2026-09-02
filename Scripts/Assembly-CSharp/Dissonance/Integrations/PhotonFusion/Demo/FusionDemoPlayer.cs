using Fusion;

namespace Dissonance.Integrations.PhotonFusion.Demo;

[NetworkBehaviourWeaved(0)]
public class FusionDemoPlayer : NetworkBehaviour
{
	private NetworkCharacterControllerPrototype _cc;

	private static Changed<FusionDemoPlayer> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<FusionDemoPlayer> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<FusionDemoPlayer> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	private void Awake()
	{
		_cc = GetComponent<NetworkCharacterControllerPrototype>();
	}

	public override void FixedUpdateNetwork()
	{
		if (GetInput<NetworkInputData>(out var input))
		{
			input.direction.Normalize();
			_cc.Move(5f * Runner.DeltaTime * input.direction);
		}
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
	}

	public override void CopyStateToBackingFields()
	{
	}
}
