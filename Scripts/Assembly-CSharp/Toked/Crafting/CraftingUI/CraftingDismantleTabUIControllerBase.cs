using UnityEngine;
using UnityEngine.UI;
using _Modules.Dismantle.Scripts;

namespace Toked.Crafting.CraftingUI;

public class CraftingDismantleTabUIControllerBase : CraftingTabUIControllerBase
{
	[SerializeField]
	private DismantleManager _dismantleManager;

	public override void Init()
	{
		base.Init();
		if (!_dismantleManager.Initialized)
		{
			_dismantleManager.OnShowDismantleUI();
		}
	}

	public override void RefreshButtonData()
	{
		_dismantleManager.RefreshButtonDismantleOptions();
	}

	public override void SelectFirstButton()
	{
		Button buttonDismantle = _dismantleManager.GetButtonDismantle();
		if (buttonDismantle.gameObject.activeSelf)
		{
			buttonDismantle.Select();
		}
		else
		{
			NetworkGameManager.Instance.ownPlayer?.inventoryManager.SelectButton(2);
		}
	}

	public override void SetNavigation(Selectable buttonOnRight = null)
	{
		_dismantleManager.SetNavigation(buttonOnRight);
	}
}
