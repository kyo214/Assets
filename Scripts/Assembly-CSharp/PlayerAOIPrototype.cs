using Fusion;
using UnityEngine;

[ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
[NetworkBehaviourWeaved(0)]
public class PlayerAOIPrototype : NetworkBehaviour
{
	[InlineHelp]
	[SerializeField]
	[MultiPropertyDrawersFix]
	protected bool DrawAreaOfInterestRadius;

	[InlineHelp]
	public float Radius = 32f;

	private static Changed<PlayerAOIPrototype> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<PlayerAOIPrototype> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<PlayerAOIPrototype> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	public override void FixedUpdateNetwork()
	{
		if (Runner.Topology == SimulationConfig.Topologies.ClientServer)
		{
			if (!Object.InputAuthority.IsNone && Runner.IsServer)
			{
				Runner.AddPlayerAreaOfInterest(Object.InputAuthority, base.transform.position, Radius);
			}
		}
		else if (!Object.StateAuthority.IsNone && Object.StateAuthority == Runner.LocalPlayer)
		{
			Runner.AddPlayerAreaOfInterest(Object.StateAuthority, base.transform.position, Radius);
		}
	}

	private void OnDrawGizmos()
	{
		if (DrawAreaOfInterestRadius)
		{
			Color color = Gizmos.color;
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(base.transform.position, Radius);
			Gizmos.color = color;
		}
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
	}

	public override void CopyStateToBackingFields()
	{
	}
}
