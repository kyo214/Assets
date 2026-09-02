using I2.Loc;
using UnityEngine;

namespace _Modules.Achievement.Scripts;

[CreateAssetMenu(fileName = "AchievementDataSO", menuName = "WMO/ScriptableObjects/Achievement/AchievementDataSO", order = 0)]
public class AchievementDataSO : ScriptableObject
{
	[SerializeField]
	private int _orderNumber;

	[SerializeField]
	private string _achievementId;

	[SerializeField]
	private bool _hiddenAchievement;

	[SerializeField]
	[TermsPopup("")]
	private string _achievementName;

	[SerializeField]
	[TermsPopup("")]
	private string _achievementDescription;

	[SerializeField]
	[TermsPopup("")]
	private string _achievementDescriptionHowToUnlock;

	[SerializeField]
	private Sprite _achievementAchievedSprite;

	[SerializeField]
	private Sprite _achievementUnachievedSprite;

	[SerializeField]
	private AchievementConditionUnlockData[] _unlockCondition;

	[SerializeField]
	private bool _isOneRun;

	[SerializeField]
	private string _statsID;

	[SerializeField]
	private AchievementDataSO[] _dependencies;

	public int OrderNumber
	{
		get
		{
			return _orderNumber;
		}
		set
		{
			_orderNumber = value;
		}
	}

	public string AchievementId
	{
		get
		{
			return _achievementId;
		}
		set
		{
			_achievementId = value;
		}
	}

	public bool HiddenAchievement
	{
		get
		{
			return _hiddenAchievement;
		}
		set
		{
			_hiddenAchievement = value;
		}
	}

	public string AchievementName
	{
		get
		{
			return _achievementName;
		}
		set
		{
			_achievementName = value;
		}
	}

	public string AchievementDescription
	{
		get
		{
			return _achievementDescription;
		}
		set
		{
			_achievementDescription = value;
		}
	}

	public string AchievementDescriptionHowToUnlock
	{
		get
		{
			return _achievementDescriptionHowToUnlock;
		}
		set
		{
			_achievementDescriptionHowToUnlock = value;
		}
	}

	public Sprite AchievementAchievedSprite
	{
		get
		{
			return _achievementAchievedSprite;
		}
		set
		{
			_achievementAchievedSprite = value;
		}
	}

	public Sprite AchievementUnachievedSprite
	{
		get
		{
			return _achievementUnachievedSprite;
		}
		set
		{
			_achievementUnachievedSprite = value;
		}
	}

	public AchievementConditionUnlockData[] UnlockCondition
	{
		get
		{
			return _unlockCondition;
		}
		set
		{
			_unlockCondition = value;
		}
	}

	public bool IsOneRun
	{
		get
		{
			return _isOneRun;
		}
		set
		{
			_isOneRun = value;
		}
	}

	public string StatsID
	{
		get
		{
			return _statsID;
		}
		set
		{
			_statsID = value;
		}
	}

	public AchievementDataSO[] Dependencies
	{
		get
		{
			return _dependencies;
		}
		set
		{
			_dependencies = value;
		}
	}
}
