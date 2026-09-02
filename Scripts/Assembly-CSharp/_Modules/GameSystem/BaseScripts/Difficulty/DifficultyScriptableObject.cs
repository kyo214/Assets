using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

namespace _Modules.GameSystem.BaseScripts.Difficulty;

[CreateAssetMenu(fileName = "DifficultyScriptableObject", menuName = "WMO/ScriptableObjects/Difficulty/DifficultyScriptableObject", order = 0)]
public class DifficultyScriptableObject : ScriptableObject
{
	[TermsPopup("")]
	[SerializeField]
	private string _difficultyLocalization;

	[SerializeField]
	private DifficultyData _difficultyData;

	[SerializeField]
	private bool _disable;

	[SerializeField]
	private List<UnlockItemRequirementBaseSO> _unlockRequirementList;

	public string DifficultyLocalization
	{
		get
		{
			return _difficultyLocalization;
		}
		set
		{
			_difficultyLocalization = value;
		}
	}

	public bool GetDisable()
	{
		return _disable;
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

	public bool CheckDataValid()
	{
		if (!GetDisable())
		{
			return CheckRequirementUnlock();
		}
		return false;
	}

	public DifficultyData GetDifficultyData()
	{
		return _difficultyData;
	}
}
