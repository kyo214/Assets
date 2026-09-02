using System.Collections.Generic;
using Toked.Crafting.CraftingUI;
using Toked.Skill;
using UnityEngine;

namespace _Modules.UIInGame.Scripts;

public class UISkillStatusController : MonoBehaviour
{
	[SerializeField]
	protected CraftingSkillUI _craftingSkillUIPrefab;

	[SerializeField]
	protected Transform _contentPanel;

	[SerializeField]
	protected List<CraftingSkillUI> _craftingSkillUIList = new List<CraftingSkillUI>();

	protected int _lastActiveButtonIndex;

	protected PlayerController _player;

	protected bool _initEvent;

	public void Init(PlayerController player, bool initEvent = true, bool isVisiblePerkSkill = true)
	{
		_player = player ?? NetworkGameManager.Instance.ownPlayer;
		SetUI();
		if (initEvent)
		{
			InitEvent();
		}
		if (!isVisiblePerkSkill)
		{
			CraftingSkillUI craftingSkillUI = _craftingSkillUIList[0];
			if ((bool)craftingSkillUI)
			{
				craftingSkillUI.SetActive(active: false);
				craftingSkillUI.ResetImage();
			}
		}
	}

	private void SetUI()
	{
		int index = 0;
		foreach (string item in _player.data.GetSkillLearn())
		{
			SkillScriptableObject skillScriptableObject = DataManager.Instance.Get<SkillLibraryScriptableObject>()?.GetData(item);
			if (!(skillScriptableObject == null))
			{
				InitButton(index++, skillScriptableObject);
			}
		}
		DisableCraftingSkillUI(index);
	}

	protected virtual void InitButton(int index, SkillScriptableObject so)
	{
		GetCraftingSkillUI(index).Init(so, null);
	}

	public CraftingSkillUI GetCraftingSkillUI(int index, bool createNew = true)
	{
		if (index >= _craftingSkillUIList.Count)
		{
			if (!createNew)
			{
				return null;
			}
			CraftingSkillUI craftingSkillUI = Object.Instantiate(_craftingSkillUIPrefab, _contentPanel);
			_craftingSkillUIList.Add(craftingSkillUI);
			return craftingSkillUI;
		}
		return _craftingSkillUIList[index];
	}

	protected void DisableCraftingSkillUI(int index)
	{
		_lastActiveButtonIndex = index;
		int num = _craftingSkillUIList.Count - 1;
		for (int i = index; i <= num; i++)
		{
			CraftingSkillUI craftingSkillUI = _craftingSkillUIList[i];
			if ((bool)craftingSkillUI)
			{
				craftingSkillUI.SetActive(active: false);
				craftingSkillUI.ResetImage();
			}
		}
	}

	public void HideUI()
	{
		DisableCraftingSkillUI(0);
		RemoveEvent();
	}

	private void InitEvent()
	{
		if (!_initEvent && (bool)_player)
		{
			_player.data.SkillData.OnChangedSkillLearnEvent += OnSkillChanged;
			_initEvent = true;
		}
	}

	private void OnDestroy()
	{
		RemoveEvent();
	}

	private void RemoveEvent()
	{
		if (_initEvent && (bool)_player)
		{
			_player.data.SkillData.OnChangedSkillLearnEvent -= OnSkillChanged;
			_initEvent = false;
		}
	}

	private void OnSkillChanged(string perkId)
	{
		SetUI();
	}
}
