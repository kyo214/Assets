using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using Sirenix.OdinInspector;
using UnityEngine;
using _Modules.GameSystem.BaseScripts;
using _Modules.Localization.Scripts;

namespace Toked.Skill;

[CreateAssetMenu(fileName = "SkillScriptableObject", menuName = "WMO/ScriptableObjects/Skill/SkillScriptableObject", order = 0)]
public class SkillScriptableObject : ScriptableObject
{
	public enum GameModeType
	{
		All = 0,
		Solo = 1,
		Coop = 2
	}

	[SerializeField]
	private int _sortIndex;

	[SerializeField]
	private string _id;

	[SerializeField]
	private int _idNumber;

	[SerializeField]
	private GameModeType _gameModeTypeUse;

	[SerializeField]
	private Sprite _skillSprite;

	[SerializeField]
	[TermsPopup("")]
	private string _skillNameLocalizeId;

	[SerializeField]
	[TermsPopup("")]
	private string _skillSubNameLocalizeId;

	[SerializeField]
	[TermsPopup("")]
	private string _skillDescriptionLocalizeId;

	[SerializeField]
	private List<StatsValueLocalization> _statsValueLocalizations = new List<StatsValueLocalization>();

	[SerializeField]
	private List<SkillEffectBaseAction> _skillEffectBaseActionList = new List<SkillEffectBaseAction>();

	[SerializeField]
	private List<UnlockItemRequirementBaseSO> _unlockRequirementList = new List<UnlockItemRequirementBaseSO>();

	public int SortIndex
	{
		get
		{
			return _sortIndex;
		}
		set
		{
			_sortIndex = value;
		}
	}

	public string ID
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
		}
	}

	public int IDNumber
	{
		get
		{
			return _idNumber;
		}
		set
		{
			_idNumber = value;
		}
	}

	public GameModeType GameModeTypeUse
	{
		get
		{
			return _gameModeTypeUse;
		}
		set
		{
			_gameModeTypeUse = value;
		}
	}

	public Sprite SkillSprite
	{
		get
		{
			return _skillSprite;
		}
		set
		{
			_skillSprite = value;
		}
	}

	public string SkillNameLocalizeId
	{
		get
		{
			return _skillNameLocalizeId;
		}
		set
		{
			_skillNameLocalizeId = value;
		}
	}

	public string SkillSubNameLocalizeId
	{
		get
		{
			return _skillSubNameLocalizeId;
		}
		set
		{
			_skillSubNameLocalizeId = value;
		}
	}

	public string SkillDescriptionLocalizeId
	{
		get
		{
			return _skillDescriptionLocalizeId;
		}
		set
		{
			_skillDescriptionLocalizeId = value;
		}
	}

	public List<SkillEffectBaseAction> SkillEffectBaseActionList
	{
		get
		{
			return _skillEffectBaseActionList;
		}
		set
		{
			_skillEffectBaseActionList = value;
		}
	}

	public void ExecuteEffectSkill(PlayerController playerController)
	{
		foreach (SkillEffectBaseAction skillEffectBaseAction in _skillEffectBaseActionList)
		{
			skillEffectBaseAction?.Apply(playerController, this);
		}
	}

	public void SetStatsValueLocalization(LocalizationParamsManager localizationParamsManager)
	{
		if (localizationParamsManager == null)
		{
			return;
		}
		foreach (StatsValueLocalization statsValueLocalization in _statsValueLocalizations)
		{
			statsValueLocalization?.SetStatsValueLocalization(localizationParamsManager);
		}
	}

	public string SetStatsValueLocalization(string text)
	{
		string text2 = text;
		foreach (StatsValueLocalization statsValueLocalization2 in _statsValueLocalizations)
		{
			IStatsValueLocalization statsValueLocalization = statsValueLocalization2.StatValueLocalization as IStatsValueLocalization;
			string oldValue = "{[" + statsValueLocalization2.StatsTag + "]}";
			text2 = text2.Replace(oldValue, statsValueLocalization?.GetStatsValueLocalization());
		}
		return text2;
	}

	public bool CheckSkillGameModeTypeUse()
	{
		switch (_gameModeTypeUse)
		{
		case GameModeType.All:
			return true;
		case GameModeType.Solo:
			if ((bool)NetworkGameManager.Instance)
			{
				return NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo;
			}
			return true;
		case GameModeType.Coop:
			if ((bool)NetworkGameManager.Instance)
			{
				return NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo;
			}
			return true;
		default:
			return true;
		}
	}

	private IEnumerable GetItemId()
	{
		ValueDropdownList<StatsValueLocalization> result = new ValueDropdownList<StatsValueLocalization>();
		foreach (SkillEffectBaseAction skillEffectBaseAction in _skillEffectBaseActionList)
		{
			if (skillEffectBaseAction is IStatsValueLocalization)
			{
				StatsValueLocalization value = new StatsValueLocalization(skillEffectBaseAction);
				AddToList(skillEffectBaseAction.name, value);
			}
		}
		return result;
		void AddToList(string inspectorName, StatsValueLocalization value2)
		{
			if (!CheckContainValue(value2))
			{
				result.Add(inspectorName, value2);
			}
		}
		bool CheckContainValue(StatsValueLocalization statsValueLocalization)
		{
			foreach (StatsValueLocalization statsValueLocalization in _statsValueLocalizations)
			{
				if (statsValueLocalization.StatValueLocalization.name == statsValueLocalization.StatValueLocalization.name)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool CheckRequirementUnlock()
	{
		foreach (UnlockItemRequirementBaseSO unlockRequirement in _unlockRequirementList)
		{
			if (!(unlockRequirement == null) && !unlockRequirement.CheckRequirement())
			{
				return false;
			}
		}
		return true;
	}

	public List<T> GetEffectValues<T>()
	{
		List<T> list = new List<T>();
		foreach (SkillEffectBaseAction skillEffectBaseAction in _skillEffectBaseActionList)
		{
			if (skillEffectBaseAction is ISkillEffectValues<T> skillEffectValues)
			{
				list.AddRange(skillEffectValues.GetValues());
			}
		}
		return list;
	}
}
