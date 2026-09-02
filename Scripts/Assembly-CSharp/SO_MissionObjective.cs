using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionObjective", menuName = "WMO/ScriptableObjects/Mission/MissionObjective", order = 0)]
public class SO_MissionObjective : ScriptableObject
{
	[SerializeField]
	private int _id;

	[SerializeField]
	private string _code;

	[SerializeField]
	public Sprite IconSmall;

	[SerializeField]
	public Sprite IconBig;

	[SerializeField]
	public Sprite IconSticker;

	[SerializeField]
	private bool _isSpawnEndlessHordeFromBeginning;

	[SerializeField]
	private int _startingTableWave = 1;

	[SerializeField]
	private int _maxWave = -1;

	[SerializeField]
	private string _targetType = "";

	[SerializeField]
	private int _targetTypeID = -1;

	[SerializeField]
	private int _minTargetDestroy = -1;

	[SerializeField]
	private bool _isCountdownEndlessHordeEnable;

	[SerializeField]
	private List<int> _countdownTimerEndlessHordeByTotalPlayers = new List<int>();

	[SerializeField]
	private bool _isCarRepairingOnStart;

	[SerializeField]
	private int _timerCountdownCarRepairing;

	[SerializeField]
	private bool _isPVP;

	public int MissionKeyItem;

	[SerializeField]
	private float _delayHorde;

	[TermsPopup("")]
	public string MissionModeLocalization;

	[TermsPopup("")]
	public string MissionModeDescLocalization;

	[TermsPopup("")]
	public string MissionObjectiveLocalization;

	[TermsPopup("")]
	public List<string> DetailObjectiveLocalization = new List<string>();

	public int ID
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

	public string Code
	{
		get
		{
			return _code;
		}
		set
		{
			_code = value;
		}
	}

	public bool IsSpawnEndlessHordeFromBeginning => _isSpawnEndlessHordeFromBeginning;

	public int StartingTableWave => _startingTableWave;

	public int MaxWave => _maxWave;

	public string TargetType => _targetType;

	public int TargetTypeID => _targetTypeID;

	public int MinTargetDestroy => _minTargetDestroy;

	public bool IsCountdownEndlessHordeEnable => _isCountdownEndlessHordeEnable;

	public int TimerCountdownCarRepairing => _timerCountdownCarRepairing;

	public bool IsCarRepairingOnStart => _isCarRepairingOnStart;

	public bool IsPVP => _isPVP;

	public float DelayHorde => _delayHorde;

	public int GetCountdownTimerEndlessHorde(int totalPlayer)
	{
		if (totalPlayer <= _countdownTimerEndlessHordeByTotalPlayers.Count)
		{
			return _countdownTimerEndlessHordeByTotalPlayers[totalPlayer - 1];
		}
		return 0;
	}
}
