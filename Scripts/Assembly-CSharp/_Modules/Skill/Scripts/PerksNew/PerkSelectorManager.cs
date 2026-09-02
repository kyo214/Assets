using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toked;
using Toked.Skill;
using UnityEngine;

namespace _Modules.Skill.Scripts.PerksNew;

public class PerkSelectorManager : MonoBehaviour
{
	[SerializeField]
	private int _chooseOptionPerk = 5;

	[SerializeField]
	private bool _useHostDataSync = true;

	[SerializeField]
	private bool _useSeed = true;

	private Dictionary<int, SkillScriptableObject> _perkSelectorDictionary = new Dictionary<int, SkillScriptableObject>();

	private bool _init;

	private IEnumerator Start()
	{
		yield return new WaitUntil(() => GameManagerPhoton.Instance != null);
		if (NetworkGameManager.Instance.isServer && !GameManagerPhoton.Instance.InitPerkSelectionIndex)
		{
			GetRandomPerks();
		}
	}

	public List<SkillScriptableObject> GetRandomPerks()
	{
		return GetRandomPerks(_useSeed);
	}

	public List<SkillScriptableObject> GetRandomPerks(bool useSeed)
	{
		_perkSelectorDictionary.Clear();
		List<SkillScriptableObject> list = ShuffleList(useSeed);
		List<SkillScriptableObject> list2 = new List<SkillScriptableObject>();
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			SkillScriptableObject skillScriptableObject = list[i];
			if (CheckCheckRequirementUnlock(skillScriptableObject, i))
			{
				_perkSelectorDictionary.Add(i, skillScriptableObject);
				list2.Add(skillScriptableObject);
				num++;
			}
			if (num == _chooseOptionPerk)
			{
				break;
			}
		}
		SetPerkSelectionIndexNetwork();
		return list2;
		bool CheckCheckRequirementUnlock(SkillScriptableObject skill, int index)
		{
			if (_useHostDataSync)
			{
				if (NetworkGameManager.Instance.isServer || !GameManagerPhoton.Instance)
				{
					return skill.CheckRequirementUnlock();
				}
				return GameManagerPhoton.Instance.ContainsPerkSelectionIndex(index);
			}
			return skill.CheckRequirementUnlock();
		}
		void SetPerkSelectionIndexNetwork()
		{
			if (_useHostDataSync && NetworkGameManager.Instance.isServer)
			{
				GameManagerPhoton.Instance?.SetPerkSelectionIndex(_perkSelectorDictionary.Keys.ToArray());
			}
		}
	}

	private List<SkillScriptableObject> ShuffleList(bool useSeed)
	{
		List<SkillScriptableObject> list = new List<SkillScriptableObject>(DataManager.Instance.Get<PerkLibraryScriptableObject>().DataList);
		if (useSeed)
		{
			list.Shuffle(GameManagerPhoton.Instance.ServerName.GetFnvHashCode());
		}
		else
		{
			list.Shuffle();
		}
		return list;
	}

	public bool CheckIsTaken(SkillScriptableObject skillScriptableObject)
	{
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerNetworkController)
		{
			if ((bool)item && item.data.SkillData.PerkId == skillScriptableObject.ID)
			{
				return true;
			}
		}
		return false;
	}

	public void UpdatePerkUI(PerkLearnNewButton perkLearnNewButton)
	{
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerNetworkController)
		{
			if ((bool)item && item.data.SkillData.PerkId == perkLearnNewButton.SkillScriptableObject?.ID)
			{
				perkLearnNewButton.SetPlayer(item);
				break;
			}
		}
	}
}
