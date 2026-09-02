using UnityEngine;

namespace _Modules.Dismantle.Scripts;

public class ItemSlotDismantleDropDetector : MonoBehaviour
{
	[SerializeField]
	private DismantleManager _dismantleManager;

	public void DropAction(int index)
	{
		_dismantleManager?.Init(index);
	}
}
