using System.Collections.Generic;
using DG.Tweening;
using I2.Loc;
using TMPro;
using Toked;
using UnityEngine;
using UnityEngine.UI;

public class MissionSelection : MonoBehaviour
{
	[SerializeField]
	private GameObject selectedImageObject;

	public SO_MissionMap MissionData;

	public int idxMission;

	public Image MapImage;

	public Image PaperImage;

	private MissionLobbyManager missionLobbyManager;

	public GameObject Icon;

	public Image InactiveImage;

	public Button BtnMission;

	public Localize TextMapName;

	public TMP_Text TFMapName;

	public bool IsSelected;

	public bool IsLocked;

	public bool IsCleared;

	public bool IsMiniMap;

	public bool IsEasyMap;

	public bool IsNormalMap;

	public bool IsHide;

	public bool IsBoss;

	public List<Image> ListSkull = new List<Image>();

	public List<MissionSelection> ListReqMissionUnlocked = new List<MissionSelection>();

	public List<MissionSelection> ListMissionDisableAfterClear = new List<MissionSelection>();

	public List<Image> ListImageModifier = new List<Image>();

	public bool IsNeedOneReqOnly;

	public GameObject PaperDifficulty;

	public Image StickerObjective;

	public Image IconObjective;

	public Image IconCleared;

	public Sprite MapLockedSprite;

	public Transform StringLine;

	public Transform PinMap;

	public int Phase;

	public bool isWinGetCheckpoint;

	public bool isCheckpoint;

	public int minMissionModifier;

	public int totalMissionModifier;

	public int objectiveLevel;

	public bool isNeedToChangeIDMission;

	private void Start()
	{
		missionLobbyManager = MissionLobbyManager.Instance;
		SetUI();
	}

	public void SetUI(SO_MissionMap NewMission = null)
	{
		if (NewMission != null)
		{
			MissionData = NewMission;
		}
		if (!MissionData)
		{
			return;
		}
		for (int i = 0; i < ListSkull.Count; i++)
		{
			ListSkull[i].gameObject.SetActive(value: false);
			if (i < MissionData.Difficulty)
			{
				ListSkull[i].gameObject.SetActive(value: true);
			}
		}
		PaperDifficulty.SetActive(value: true);
		if (MissionData.IsLocked)
		{
			IconObjective.gameObject.SetActive(value: true);
			if ((bool)MissionData.MissionObjective)
			{
				IconObjective.sprite = MissionData.MissionObjective.IconBig;
			}
			TFMapName.text = "?????";
			MapImage.sprite = MapLockedSprite;
			StickerObjective.gameObject.SetActive(value: false);
		}
		else
		{
			TextMapName.SetTerm(MissionData.MapNameLocalization);
			MapImage.sprite = MissionData.MapImage;
			IconObjective.gameObject.SetActive(value: false);
			if (!IconCleared.gameObject.activeSelf)
			{
				StickerObjective.gameObject.SetActive(value: true);
			}
			if ((bool)MissionData.MissionObjective)
			{
				StickerObjective.sprite = MissionData.MissionObjective.IconSticker;
			}
		}
		for (int j = 0; j < ListImageModifier.Count; j++)
		{
			if (j < MissionData.ListModifier.Count)
			{
				ListImageModifier[j].sprite = MissionData.ListModifier[j].spriteSticker;
				ListImageModifier[j].enabled = true;
			}
			else
			{
				ListImageModifier[j].enabled = false;
			}
		}
	}

	public void ResetMissionData()
	{
		MissionData.IsBoss = IsBoss;
		if (GameManagerPhoton.Instance.isLoadMap)
		{
			IsCleared = GameManagerPhoton.Instance.ArrMissionCleared.Get(MissionData.MissionID - 1);
			IsLocked = GameManagerPhoton.Instance.ArrMissionLocked.Get(MissionData.MissionID - 1);
		}
		if (!GameManagerPhoton.Instance.isInitializedLockedMap)
		{
			MissionData.IsLocked = IsLocked;
			GameManagerPhoton.Instance.ArrMissionLocked.Set(MissionData.MissionID - 1, IsLocked);
		}
		MissionData.IsCleared = IsCleared;
		MissionData.IsHide = IsHide;
		MissionData.IsEasyMap = IsEasyMap;
	}

	public void SetMission()
	{
		if ((bool)MissionData && !MissionData.IsLocked)
		{
			AudioManager.PlaySFX("ui_confirm");
			missionLobbyManager.MissionDetailMap.ShowUI(MissionData, this);
		}
	}

	public void SetMissionGlobal()
	{
		foreach (MissionSelection item in missionLobbyManager.MissionBoard.AllMissionSelection)
		{
			if ((bool)item)
			{
				item.Icon.SetActive(value: false);
			}
		}
		MissionSelection missionSelection = missionLobbyManager.MissionBoard.GetMissionSelection(NetworkGameManager.Instance.Mission);
		if ((bool)missionSelection)
		{
			missionSelection.SetHighlight(isTurnOn: false, 0.2f);
		}
		if ((bool)MissionData)
		{
			NetworkGameManager.Instance.Mission = MissionData.MissionID;
			GameManager.Instance.gameManagerPhoton.Mission = (byte)MissionData.MissionID;
			GameManager.Instance.gameManagerPhoton.CurrentMission = MissionData;
		}
		if ((bool)LobbyManager.Instance && LobbyManager.Instance.LobbyState == LobbyManager.LobbyStateEnum.Car)
		{
			Icon.SetActive(value: true);
		}
		Icon.transform.DOKill();
		Icon.transform.DOScale(1.5f, 0f);
		Icon.transform.DOScale(1f, 0.1f);
		UIGameManager.Instance.SetMissionLocation(UIGameManager.Instance.missionLocationText, null, UIGameManager.Instance.missionLocationTextField);
		SetHighlight(isTurnOn: true, 0.2f);
	}

	public void OnHighlightMission()
	{
		if (BtnMission.enabled)
		{
			BtnMission.Select();
		}
	}

	public void SelectMission()
	{
		if (missionLobbyManager == null)
		{
			missionLobbyManager = MissionLobbyManager.Instance;
		}
		foreach (MissionSelection item in missionLobbyManager.MissionBoard.AllMissionSelection)
		{
			if ((bool)item)
			{
				item.IsSelected = false;
			}
		}
		int selectCol = missionLobbyManager.selectCol;
		int selectRow = missionLobbyManager.selectRow;
		if (missionLobbyManager.MissionBoard.ListMissionMapNavigation.Count == 0)
		{
			missionLobbyManager.SetMapNavigation();
		}
		if (missionLobbyManager.MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[selectCol].MissionData.MissionID != NetworkGameManager.Instance.Mission)
		{
			missionLobbyManager.MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[selectCol].SetHighlight(isTurnOn: false, 0.2f);
			missionLobbyManager.MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[selectCol].IsSelected = false;
		}
		if (missionLobbyManager.MissionBoard.ImageMissionObject != null)
		{
			missionLobbyManager.MissionBoard.ImageMissionObject.SetActive(value: false);
		}
		selectedImageObject.SetActive(value: true);
		missionLobbyManager.MissionBoard.ImageMissionObject = selectedImageObject;
		IsSelected = true;
		if (!missionLobbyManager.MissionBoard.ListMissionMapNavigation[missionLobbyManager.selectRow].missionSelect[missionLobbyManager.selectCol] || !missionLobbyManager.MissionBoard.ListMissionMapNavigation[missionLobbyManager.selectRow].missionSelect[missionLobbyManager.selectCol].IsSelected)
		{
			bool flag = false;
			for (int i = 0; i < missionLobbyManager.MissionBoard.ListMissionMapNavigation.Count; i++)
			{
				for (int j = 0; j < missionLobbyManager.MissionBoard.ListMissionMapNavigation[i].missionSelect.Count; j++)
				{
					if ((bool)missionLobbyManager.MissionBoard.ListMissionMapNavigation[i].missionSelect[j] && missionLobbyManager.MissionBoard.ListMissionMapNavigation[i].missionSelect[j].IsSelected)
					{
						missionLobbyManager.selectRow = i;
						missionLobbyManager.selectCol = j;
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		SetHighlight(isTurnOn: true, 0.2f);
		for (int k = 0; k < 6; k++)
		{
			missionLobbyManager.MissionBoard.ListPossibleLoot[k].sprite = null;
			missionLobbyManager.MissionBoard.ListPossibleLoot[k].color = new Color(1f, 1f, 1f, 0f);
		}
		if (!MissionData)
		{
			return;
		}
		if (MissionData.IsLocked)
		{
			missionLobbyManager.MissionBoard.TextFieldScenarioLabel.text = "--- ????? ---";
			missionLobbyManager.MissionBoard.TextFieldScenarioDesc.text = "?????";
			missionLobbyManager.MissionBoard.TextFieldModifierValue.text = "?????";
		}
		else
		{
			missionLobbyManager.MissionBoard.TextScenarioLabel.SetTerm(MissionData.MapNameLocalization);
			missionLobbyManager.MissionBoard.TextScenarioDesc.SetTerm(MissionData.MissionObjective.MissionModeDescLocalization);
			if (MissionData.MissionObjective.IsCountdownEndlessHordeEnable)
			{
				int num = 0;
				num = MissionData.MissionObjective.GetCountdownTimerEndlessHorde(NetworkGameManager.Instance.arrPlayerController.Count) / 60;
				missionLobbyManager.MissionBoard.TextFieldScenarioDesc.text = missionLobbyManager.MissionBoard.TextFieldScenarioDesc.text.Replace("(x)", num.ToString());
			}
			if (MissionData.MissionObjective.MinTargetDestroy > 0 && MissionData.MissionObjective.TargetType != "")
			{
				missionLobbyManager.MissionBoard.TextFieldScenarioDesc.text = missionLobbyManager.MissionBoard.TextFieldScenarioDesc.text.Replace("(x)", MissionData.MissionObjective.MinTargetDestroy.ToString());
			}
			missionLobbyManager.MissionBoard.TextFieldScenarioLabel.text = missionLobbyManager.MissionBoard.TextFieldScenarioLabel.text + " - " + LocalizationManager.GetTranslation(MissionData.MissionObjective.MissionModeLocalization);
			missionLobbyManager.MissionBoard.TextFieldModifierValue.text = "";
			for (int l = 0; l < MissionData.ListWeapon.Count; l++)
			{
				Image image = missionLobbyManager.MissionBoard.ListPossibleLoot[l];
				DataManager instance = DataManager.Instance;
				int weapon = (int)MissionData.ListWeapon[l].Weapon;
				image.sprite = instance.GetItemSprite(weapon.ToString());
				missionLobbyManager.MissionBoard.ListPossibleLoot[l].color = new Color(1f, 1f, 1f, 1f);
			}
		}
		for (int m = 0; m < missionLobbyManager.MissionBoard.ListModifierIcon.Count; m++)
		{
			if (m < MissionData.ListModifier.Count)
			{
				missionLobbyManager.MissionBoard.ListModifierIcon[m].gameObject.SetActive(value: true);
				missionLobbyManager.MissionBoard.ListModifierIcon[m].sprite = MissionData.ListModifier[m].spriteIcon;
				missionLobbyManager.MissionBoard.ListModifierLocalizeText[m].SetTerm(MissionData.ListModifier[m].ModifierNameLocalization);
			}
			else
			{
				missionLobbyManager.MissionBoard.ListModifierIcon[m].gameObject.SetActive(value: false);
			}
		}
	}

	public void SetHighlight(bool isTurnOn, float duration)
	{
		if (isTurnOn)
		{
			MapImage.DOKill();
			PaperImage.DOKill();
			IconObjective.DOKill();
			if (StickerObjective.gameObject.activeSelf)
			{
				StickerObjective.DOKill();
				StickerObjective.DOColor(new Color(1f, 1f, 1f), duration);
			}
			IconObjective.DOColor(new Color(1f, 1f, 1f), duration);
			MapImage.DOColor(new Color(1f, 1f, 1f), duration);
			PaperImage.DOColor(new Color(1f, 1f, 1f), duration);
		}
		else
		{
			MapImage.DOKill();
			PaperImage.DOKill();
			IconObjective.DOKill();
			if (StickerObjective.gameObject.activeSelf)
			{
				StickerObjective.DOKill();
				StickerObjective.DOColor(new Color(0.5f, 0.5f, 0.5f), duration);
			}
			IconObjective.DOColor(new Color(0.5f, 0.5f, 0.5f), duration);
			MapImage.DOColor(new Color(0.5f, 0.5f, 0.5f), duration);
			PaperImage.DOColor(new Color(0.5f, 0.5f, 0.5f), duration);
		}
	}
}
