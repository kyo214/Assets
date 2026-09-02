using System.Collections.Generic;
using UnityEngine;
using _Modules.UITitle.CreateRoom;

namespace _Modules.GameSystem.BaseScripts.Difficulty;

public class DifficultyPanelUI : CreateGameSettingPanel<DifficultyScriptableObject>
{
	[SerializeField]
	private DifficultyScriptableObjectLibrary _difficultyScriptableObjectLibrary;

	[SerializeField]
	private DifficultySetting.Difficulty _difficultyDefault = DifficultySetting.Difficulty.Normal;

	protected override void InitDataList()
	{
		_listData.Clear();
		if ((object)_difficultyScriptableObjectLibrary == null)
		{
			_difficultyScriptableObjectLibrary = DataManager.Instance.Get<DifficultyScriptableObjectLibrary>();
		}
		foreach (DifficultyScriptableObject data in _difficultyScriptableObjectLibrary.DataList)
		{
			_listData.Add(data);
		}
		_index = (int)_difficultyDefault;
	}

	protected override string GetTermValue()
	{
		return _listData[_index].DifficultyLocalization;
	}

	public override void SetDataWhenCreateGame(bool isLoad)
	{
		GameModes.Instance.SetDifficulty(_listData[_index].GetDifficultyData().DifficultySetting);
	}

	public override void OnValueChangedAction(int index)
	{
		base.OnValueChangedAction(index);
		base.IsCurrentValueValid = _listData[index].CheckDataValid();
		OnChangeValueEvent?.Invoke(base.IsCurrentValueValid);
	}

	protected override List<bool> GetDisableData()
	{
		List<bool> list = new List<bool>();
		foreach (DifficultyScriptableObject listDatum in _listData)
		{
			list.Add(listDatum.GetDisable());
		}
		return list;
	}

	protected override List<bool> GetLockData()
	{
		List<bool> list = new List<bool>();
		foreach (DifficultyScriptableObject listDatum in _listData)
		{
			list.Add(!listDatum.CheckRequirementUnlock());
		}
		return list;
	}
}
