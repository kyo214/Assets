using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Modules.UIMission.Scripts;

public class UIMissionObjective : MonoBehaviour
{
	public TMP_Text TextMap;

	[SerializeField]
	private Image _objectiveImg;

	public List<Localize> ListTextObjective = new List<Localize>();

	public List<TMP_Text> ListTMPTextObjective = new List<TMP_Text>();

	public List<GameObject> CheckboxObjective = new List<GameObject>();

	public GameObject FrameEscape;

	public TextMeshProUGUI TextEscape;

	private int _totalObjective;

	[SerializeField]
	private List<UIIconModifier> _listIconModifier = new List<UIIconModifier>();

	public static UIMissionObjective Instance { get; private set; }

	[SerializeField]
	public List<UIIconModifier> ListIconModifier => _listIconModifier;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		if (NetworkGameManager.Instance.Mission - 1 >= 0)
		{
			SetUIMission();
		}
	}

	public void SetUIMapText()
	{
		if ((bool)GameManagerPhoton.Instance && (bool)GameManagerPhoton.Instance.CurrentMission)
		{
			TextMap.text = LocalizationManager.GetTranslation(GameManagerPhoton.Instance.CurrentMission.MapNameLocalization) + " - <color=yellow>" + LocalizationManager.GetTranslation(GameManagerPhoton.Instance.CurrentMission.MissionObjective.MissionModeLocalization) + "</color>";
		}
	}

	public void SetUIMission()
	{
		if (!GameManagerPhoton.Instance)
		{
			return;
		}
		SetUIMapText();
		if (ListTextObjective.Count <= 0 || GameManagerPhoton.Instance.CurrentMission.MissionObjective.DetailObjectiveLocalization.Count < 1)
		{
			return;
		}
		for (int i = 0; i < _listIconModifier.Count; i++)
		{
			_listIconModifier[i].gameObject.SetActive(value: false);
		}
		for (int j = 0; j < 3; j++)
		{
			if (j <= GameManagerPhoton.Instance.CurrentMission.MissionObjective.DetailObjectiveLocalization.Count - 1)
			{
				ListTextObjective[j].transform.parent.gameObject.SetActive(value: true);
				ListTextObjective[j].SetTerm(GameManagerPhoton.Instance.CurrentMission.MissionObjective.DetailObjectiveLocalization[j]);
				if (GameManagerPhoton.Instance.CurrentMission.MissionObjective.MinTargetDestroy > 0)
				{
					ListTMPTextObjective[j].text = ListTMPTextObjective[j].text + " (" + GameManagerPhoton.Instance.TargetDestroyed + "/" + GameManagerPhoton.Instance.CurrentMission.MissionObjective.MinTargetDestroy + ")";
				}
			}
			else
			{
				ListTextObjective[j].transform.parent.gameObject.SetActive(value: false);
			}
		}
		_objectiveImg.sprite = GameManagerPhoton.Instance.CurrentMission.MissionObjective.IconSticker;
		_totalObjective = GameManagerPhoton.Instance.CurrentMission.MissionObjective.DetailObjectiveLocalization.Count + 1;
		for (int k = 0; k < GameManagerPhoton.Instance.CurrentMission.ListModifier.Count; k++)
		{
			_listIconModifier[k].gameObject.SetActive(value: true);
			_listIconModifier[k].Init(GameManagerPhoton.Instance.CurrentMission.ListModifier[k]);
		}
	}

	public void SetAllObjectiveCleared()
	{
		foreach (GameObject item in CheckboxObjective)
		{
			item.SetActive(value: true);
		}
	}

	public void SetCheckboxAdditionalObjective(int idx)
	{
		CheckboxObjective[idx].SetActive(value: true);
	}

	public void SetCheckboxRetrieveKeyItem()
	{
		if (_totalObjective == 0)
		{
			_totalObjective = GameManagerPhoton.Instance.CurrentMission.MissionObjective.DetailObjectiveLocalization.Count + 1;
		}
		if (CheckboxObjective.Count >= 2)
		{
			CheckboxObjective[_totalObjective - 2].SetActive(value: true);
			FrameEscape.SetActive(value: true);
		}
	}
}
