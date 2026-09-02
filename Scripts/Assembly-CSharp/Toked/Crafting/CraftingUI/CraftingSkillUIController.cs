using Doozy.Runtime.UIManager.Components;
using Toked.Skill;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using _Modules.UIInGame.Scripts;

namespace Toked.Crafting.CraftingUI;

public class CraftingSkillUIController : UISkillStatusController
{
	[SerializeField]
	private CraftingDescriptionsUI _craftingDescriptionsUI;

	private GameObject _lastSelectedButton;

	private PlayerInputActions _playerInputActions;

	public void Init()
	{
		Init(NetworkGameManager.Instance.ownPlayer);
		SetNavigation();
	}

	protected override void InitButton(int index, SkillScriptableObject so)
	{
		GetCraftingSkillUI(index).Init(so, OnHoverAction);
	}

	public void SetFirstButtonSelected()
	{
		CraftingSkillUI craftingSkillUI = GetCraftingSkillUI(0, createNew: false);
		if (!(craftingSkillUI == null) && craftingSkillUI.gameObject.activeSelf)
		{
			_lastSelectedButton = EventSystem.current.currentSelectedGameObject;
			craftingSkillUI.Select();
			InputManager.ToggleActionMap(InputManager.PlayerInputToggleAction.DISABLE_PLAYER_INPUT, InputManager.inputActions.SkillDescription);
		}
	}

	private void OnHoverAction(SkillScriptableObject skill)
	{
		_craftingDescriptionsUI?.Set(skill);
	}

	private void OnEnable()
	{
		_playerInputActions = InputManager.inputActions;
		_playerInputActions.SkillDescription.Back.performed += OnInputBackButton;
	}

	private void OnDisable()
	{
		_playerInputActions.SkillDescription.Back.performed -= OnInputBackButton;
	}

	private void OnInputBackButton(InputAction.CallbackContext obj)
	{
		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(_lastSelectedButton);
		InputManager.ToggleActionMap(InputManager.PlayerInputToggleAction.DISABLE_PLAYER_INPUT, InputManager.inputActions.InventoryUI);
	}

	private void SetNavigation()
	{
		for (int i = 0; i <= _lastActiveButtonIndex; i++)
		{
			int num = i - 1;
			if (_lastActiveButtonIndex < _craftingSkillUIList.Count)
			{
				UISelectable selectOnLeft = ((num < 0) ? _craftingSkillUIList[_lastActiveButtonIndex].UISelectable : _craftingSkillUIList[num].UISelectable);
				UISelectable uISelectable = _craftingSkillUIList[i].UISelectable;
				int num2 = i + 1;
				UISelectable selectOnRight = ((num2 > _lastActiveButtonIndex) ? _craftingSkillUIList[0].UISelectable : _craftingSkillUIList[num2].UISelectable);
				uISelectable.navigation = new Navigation
				{
					mode = Navigation.Mode.Explicit,
					selectOnDown = null,
					selectOnLeft = selectOnLeft,
					selectOnRight = selectOnRight,
					selectOnUp = null
				};
			}
		}
	}
}
