using UnityEngine;

public class NoteController : MonoBehaviour
{
	public void Navigate(Vector2 direction)
	{
		ItemPickable component = NetworkGameManager.Instance.ownPlayer.itemCollision.GetComponent<ItemPickable>();
		if (component.itemType == "Note")
		{
			if (direction.x <= -0.5f)
			{
				component.ShowNote();
			}
			else if (direction.x >= 0.5f)
			{
				component.ShowNote();
			}
		}
	}
}
