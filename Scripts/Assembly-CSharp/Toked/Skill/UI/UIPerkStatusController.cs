using Toked.Crafting.CraftingUI;
using UnityEngine;

namespace Toked.Skill.UI;

public class UIPerkStatusController : MonoBehaviour
{
	[SerializeField]
	private CraftingSkillUI _perkStatusUI;

	private PlayerController _player;

	private bool _initEvent;

	public void Init(PlayerController player)
	{
		_player = player ?? NetworkGameManager.Instance.ownPlayer;
		SetUI(_player.data.SkillData.PerkId);
		InitEvent();
	}

	private void SetUI(string perkId)
	{
		SkillScriptableObject skillScriptableObject = DataManager.Instance.Get<PerkLibraryScriptableObject>()?.GetData(perkId);
		if (!(skillScriptableObject == null))
		{
			InitButton(skillScriptableObject);
		}
	}

	private void InitButton(SkillScriptableObject so)
	{
		_perkStatusUI.Init(so, null);
	}

	private void InitEvent()
	{
		if (!_initEvent && (bool)_player)
		{
			_player.data.SkillData.OnChangedPerkEvent += OnPerkChanged;
			_initEvent = true;
		}
	}

	public void HideUI()
	{
		ResetUI();
		RemoveEvent();
	}

	private void OnDestroy()
	{
		RemoveEvent();
	}

	private void RemoveEvent()
	{
		if (_initEvent && (bool)_player)
		{
			_player.data.SkillData.OnChangedPerkEvent -= OnPerkChanged;
			_initEvent = false;
		}
	}

	private void OnPerkChanged(string perkId)
	{
		SetUI(perkId);
	}

	private void ResetUI()
	{
		if ((bool)_perkStatusUI)
		{
			_perkStatusUI.SetActive(active: false);
			_perkStatusUI.ResetImage();
		}
	}
}
