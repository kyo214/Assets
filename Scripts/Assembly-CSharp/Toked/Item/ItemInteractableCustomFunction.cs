using UnityEngine;

namespace Toked.Item;

public abstract class ItemInteractableCustomFunction : MonoBehaviour
{
	public abstract void Execute(PlayerController playerController = null);
}
