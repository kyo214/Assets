using Toked.Item;

public class HideObjectInteractableFunction : ItemInteractableCustomFunction
{
	public override void Execute(PlayerController playerController = null)
	{
		base.gameObject.SetActive(value: false);
	}
}
