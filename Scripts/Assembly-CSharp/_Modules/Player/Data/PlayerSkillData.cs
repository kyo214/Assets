using System;
using System.Collections.Generic;
using System.Linq;
using Toked.Skill;
using UnityEngine;

namespace _Modules.Player.Data;

public class PlayerSkillData : MonoBehaviour
{
	[SerializeField]
	private string _perkId;

	[SerializeField]
	private List<string> _additionalPerkSkillDataList = new List<string>();

	[SerializeField]
	private int _skillPoint;

	[SerializeField]
	private List<string> _skillLearnDataList = new List<string>();

	public string PerkId => _perkId;

	public List<string> AdditionalPerkSkillDataList => _additionalPerkSkillDataList;

	public int SkillPoint => _skillPoint;

	public List<string> SkillLearnDataList => _skillLearnDataList;

	public event Action<string> OnChangedPerkEvent;

	public event Action<List<string>> OnSetAdditionalPerkSkillEvent;

	public event Action<string> OnChangedAdditionalPerkSkillEvent;

	public event Action<string> OnChangedSkillLearnEvent;

	public event System.Action OnResetSkillLearnEvent;

	public event Action<int> OnChangedSkillPointEvent;

	public event Action<int> OnResetSkillPointEvent;

	public PlayerSkillData(PlayerSkillData skinData)
	{
		_perkId = skinData.PerkId;
		_skillPoint = skinData.SkillPoint;
		_skillLearnDataList = new List<string>(skinData.SkillLearnDataList);
	}

	public void SetPerk(string perkId, bool executeEvent = true)
	{
		if (!string.IsNullOrWhiteSpace(perkId) && !(perkId == _perkId))
		{
			_perkId = perkId;
			OnChangedPerkEvent?.Invoke(perkId);
		}
	}

	public void SetAdditionalPerkSkill(List<string> idSkillList, bool executeEvent = false)
	{
		_additionalPerkSkillDataList.Clear();
		_additionalPerkSkillDataList.AddRange(idSkillList.Distinct());
		if (executeEvent)
		{
			OnSetAdditionalPerkSkillEvent?.Invoke(_additionalPerkSkillDataList.ToList());
		}
	}

	public void AddAdditionalPerkSkill(string idSkill)
	{
		if (!_additionalPerkSkillDataList.Contains(idSkill))
		{
			_additionalPerkSkillDataList.Add(idSkill);
			OnChangedAdditionalPerkSkillEvent?.Invoke(idSkill);
		}
	}

	public void SetSkillLearn(List<string> idSkillList)
	{
		_skillLearnDataList.Clear();
		_skillLearnDataList.AddRange(idSkillList);
	}

	public void AddSkillLearn(SkillScriptableObject skillScriptableObject)
	{
		AddSkillLearn(skillScriptableObject.ID);
	}

	public void AddSkillLearn(string idSkill)
	{
		if (!_skillLearnDataList.Contains(idSkill))
		{
			_skillLearnDataList.Add(idSkill);
		}
		OnChangedSkillLearnEvent?.Invoke(idSkill);
	}

	public bool CheckAdditionalPerkSkillLearn(string id)
	{
		return _additionalPerkSkillDataList.Contains(id);
	}

	public bool CheckSkillLearn(string id)
	{
		return _skillLearnDataList.Contains(id);
	}

	public int GetTotalSkillLearn()
	{
		return _skillLearnDataList.Count;
	}

	public void ResetSkillLearnData(bool executeEvent = true)
	{
		_skillLearnDataList.Clear();
		if (executeEvent)
		{
			OnResetSkillLearnEvent?.Invoke();
		}
	}

	public void SetSkillPoint(int point, bool executeEvent = true)
	{
		_skillPoint = point;
		if (executeEvent)
		{
			OnChangedSkillPointEvent?.Invoke(_skillPoint);
		}
	}

	public void AddSkillPoint(int point)
	{
		_skillPoint += point;
		OnChangedSkillPointEvent?.Invoke(_skillPoint);
	}

	public void RemoveSkillPoint(int point)
	{
		int val = _skillPoint - point;
		_skillPoint = Math.Max(val, 0);
		OnChangedSkillPointEvent?.Invoke(_skillPoint);
	}

	public void ResetSkillPoint()
	{
		_skillPoint = 0;
		OnResetSkillPointEvent?.Invoke(_skillPoint);
	}

	public bool CheckSkillPoint(int point)
	{
		return _skillPoint >= point;
	}
}
