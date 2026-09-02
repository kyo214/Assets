using UnityEngine;

namespace Dissonance;

[HelpURL("https://placeholder-software.co.uk/dissonance/docs/Reference/Components/Voice-Proximity-Receipt-Trigger/")]
public class VoiceProximityReceiptTrigger : BaseProximityTrigger<RoomMembership>, IVoiceReceiptTrigger
{
	private class ReceiptGrid : Grid
	{
		private readonly VoiceProximityReceiptTrigger _parent;

		public ReceiptGrid(VoiceProximityReceiptTrigger parent)
			: base((BaseProximityTrigger<RoomMembership>)parent)
		{
			_parent = parent;
		}

		protected override RoomMembership CreateHandle(Vector3Int id, string name)
		{
			return base.Parent.Comms.Rooms.Join(new RoomName(name, suppress: true));
		}

		protected override void CloseHandle(RoomMembership handle)
		{
			_parent.Comms.Rooms.Leave(handle);
		}
	}

	[SerializeField]
	private bool _roomExpanded = true;

	[SerializeField]
	private bool _tokensExpanded;

	[SerializeField]
	private bool _colliderExpanded;

	protected override Grid CreateGrid()
	{
		return new ReceiptGrid(this);
	}
}
