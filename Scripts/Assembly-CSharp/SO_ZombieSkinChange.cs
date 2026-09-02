using System;
using UnityEngine;
using _Modules.CharacterSkin;

[Serializable]
[CreateAssetMenu(fileName = "ZombieSkinChangeModifier", menuName = "WMO/ScriptableObjects/Game/ZombieSkinChangeModifier", order = 0)]
public class SO_ZombieSkinChange : SO_GameModifier
{
	[SerializeField]
	private bool _isForever;

	[SerializeField]
	private int _startMonth;

	[SerializeField]
	private int _startDay;

	[SerializeField]
	private int _startHour;

	[SerializeField]
	private int _endMonth;

	[SerializeField]
	private int _endDay;

	[SerializeField]
	private int _endHour;

	[SerializeField]
	private int _randomizeFrom100Percent = 10;

	[SerializeField]
	private int _idxSkin;

	[SerializeField]
	private bool _isActiveOnTestBuild;

	[SerializeField]
	private bool _isUsingGlobalTime;

	[SerializeField]
	private SkinScriptableObject _skinZombie;

	public int IdxSkin => _idxSkin;

	public override void Apply()
	{
		if ((_isForever || IsDateInRange() || (_isActiveOnTestBuild && GameModes.Instance.isDebug)) && !SkinManager.Instance.ListSkinZombieModifier.Contains(this))
		{
			SkinManager.Instance.ListSkinZombieModifier.Add(this);
			SkinManager.Instance.AddListEnemySkin(_skinZombie);
			_idxSkin = SkinManager.Instance.GetTotalEnemySkin() - 1;
		}
	}

	public bool IsDateInRange()
	{
		DateTime dateTime = new DateTime(DateTime.Now.Year, _startMonth, _startDay, _startHour, 0, 0);
		DateTime dateTime2 = new DateTime(DateTime.Now.Year, _endMonth, _endDay, _endHour, 0, 0);
		if (_isUsingGlobalTime)
		{
			if (GlobalOptionsManager.Instance.UtcDateTime >= dateTime)
			{
				return GlobalOptionsManager.Instance.PdtDateTime <= dateTime2;
			}
			return false;
		}
		if (GlobalOptionsManager.Instance.LocalDateTime >= dateTime)
		{
			return GlobalOptionsManager.Instance.LocalDateTime <= dateTime2;
		}
		return false;
	}

	public int GetPercentageShow()
	{
		return _randomizeFrom100Percent;
	}
}
