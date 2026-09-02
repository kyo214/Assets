using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

namespace _Modules.GameSystem.BaseScripts.Scenario;

[CreateAssetMenu(fileName = "ScenarioScriptableObject", menuName = "WMO/ScriptableObjects/Scenario/Scenario", order = 0)]
public class ScenarioScriptableObject : ScriptableObject
{
	[SerializeField]
	private string _scenarioId;

	[SerializeField]
	[TermsPopup("")]
	private string _scenarioNameLocalization;

	[SerializeField]
	private MissionBoardMap _missionBoardMap;

	[SerializeField]
	private bool _disable;

	[SerializeField]
	private List<UnlockItemRequirementBaseSO> _scenarioUnlockRequirementList;

	public string ScenarioId
	{
		get
		{
			return _scenarioId;
		}
		set
		{
			_scenarioId = value;
		}
	}

	public string ScenarioNameLocalization
	{
		get
		{
			return _scenarioNameLocalization;
		}
		set
		{
			_scenarioNameLocalization = value;
		}
	}

	public MissionBoardMap MissionBoardMap
	{
		get
		{
			return _missionBoardMap;
		}
		set
		{
			_missionBoardMap = value;
		}
	}

	public bool GetDisable()
	{
		return _disable;
	}

	public bool CheckRequirementUnlock()
	{
		foreach (UnlockItemRequirementBaseSO scenarioUnlockRequirement in _scenarioUnlockRequirementList)
		{
			if (!(scenarioUnlockRequirement == null) && !scenarioUnlockRequirement.CheckRequirement())
			{
				return false;
			}
		}
		return true;
	}

	public bool CheckDataValid()
	{
		if (!GetDisable())
		{
			return CheckRequirementUnlock();
		}
		return false;
	}
}
