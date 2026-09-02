using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using I2.Loc;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using _Modules.GameSystem.BaseScripts.Difficulty;

public class MissionLobbyManager : MonoBehaviour
{
	public class MissionMapComparer : IEqualityComparer<SO_MissionMap>
	{
		public bool Equals(SO_MissionMap x, SO_MissionMap y)
		{
			if (x != null && y != null)
			{
				return x.MissionID == y.MissionID;
			}
			return false;
		}

		public int GetHashCode(SO_MissionMap obj)
		{
			return obj.MissionID.GetHashCode();
		}
	}

	public UIView UIMenu;

	public Localize txtReady;

	public MissionBriefing missionBrief;

	public PlayerInputActions input;

	public int selectCol;

	public int selectRow;

	public bool isInputMovePressed;

	public MissionBoardMap MissionBoard;

	public MissionBoardMap MissionBoardDemo;

	public MissionBoardMap MissionBoardExpo;

	public MissionDetail MissionDetailMap;

	public bool initializedMap;

	public List<MapDesignSelection> ListMapDesign = new List<MapDesignSelection>();

	public GameObject MapDesign;

	[SerializeField]
	private bool _isFirstHide;

	public Material MatGrayscale;

	[SerializeField]
	private Localize TextDifficulty;

	public static MissionLobbyManager Instance { get; private set; }

	public void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
		if ((bool)GameModes.Instance)
		{
			if (GameModes.Instance.isEvent)
			{
				MissionBoard.gameObject.SetActive(value: false);
				MissionBoardExpo.gameObject.SetActive(value: true);
				MissionBoard = MissionBoardExpo;
			}
			else
			{
				MissionBoardExpo.gameObject.SetActive(value: false);
				if (GameModes.Instance.isDemo)
				{
					MissionBoard.gameObject.SetActive(value: false);
					MissionBoardDemo.gameObject.SetActive(value: true);
					MissionBoard = MissionBoardDemo;
				}
				else
				{
					MissionBoardDemo.gameObject.SetActive(value: false);
				}
			}
		}
		if (!GameManagerPhoton.Instance)
		{
			return;
		}
		for (int i = 0; i < GameManagerPhoton.Instance.ListMission.Count; i++)
		{
			if (i < MissionBoard.AllMissionSelection.Count)
			{
				MissionBoard.AllMissionSelection[i].SetUI(GameManagerPhoton.Instance.ListMission[i]);
			}
		}
	}

	private void Start()
	{
		UIMenu = GetComponent<UIView>();
		TextDifficulty.SetTerm(DataManager.Instance.Get<DifficultyScriptableObjectLibrary>()?.GetData(GameModes.Instance.GetDifficultyData().DifficultySetting).DifficultyLocalization);
	}

	public void InitMap(bool isSetClearNotNullMission = true, bool isCheckingClearedMission = false, int lifeSeed = 0)
	{
		if (lifeSeed == 0)
		{
			lifeSeed = GameManagerPhoton.Instance.Life;
		}
		int seed = GameManagerPhoton.Instance.Seed;
		if (seed == 0)
		{
			seed = GlobalOptionsManager.Instance.seed;
		}
		UnityEngine.Random.InitState(seed);
		Debug.Log("--cek Init Map seed = " + seed + " lifeSeed = " + lifeSeed);
		if (GlobalSaveData.instance.gameData != null)
		{
			for (int num = MissionBoard.ListMissionMiniMap.Count - 1; num >= 0; num--)
			{
				if (MissionBoard.ListMissionMiniMap[num] != null && MissionBoard.ListMissionMiniMap[num].buildVersion > int.Parse(GlobalSaveData.instance.gameData.GameVersion))
				{
					MissionBoard.ListMissionMiniMap.RemoveAt(num);
				}
			}
			for (int num2 = MissionBoard.ListMissionNormalMap.Count - 1; num2 >= 0; num2--)
			{
				if (MissionBoard.ListMissionNormalMap[num2].buildVersion > int.Parse(GlobalSaveData.instance.gameData.GameVersion))
				{
					MissionBoard.ListMissionNormalMap.RemoveAt(num2);
				}
			}
		}
		GameManagerPhoton.Instance.ListMission.Clear();
		MissionBoard.ListMissionEasyMapPhase0.Clear();
		MissionBoard.ListMissionEasyMapPhase0 = MissionBoard.ListMissionEasyMap.ToList();
		MissionBoard.ListMissionMiniMapPhase0.Clear();
		MissionBoard.ListMissionMiniMapPhase0 = MissionBoard.ListMissionMiniMap.ToList();
		int num3 = MissionBoard.AllMissionSelection.Count((MissionSelection item) => item.IsEasyMap);
		for (int num4 = 0; num4 < num3; num4++)
		{
			int index = UnityEngine.Random.Range(0, MissionBoard.ListMissionEasyMapPhase0.Count);
			MissionBoard.ListMissionMiniMapPhase0.Add(MissionBoard.ListMissionEasyMap[index]);
			MissionBoard.ListMissionEasyMapPhase0.RemoveAt(index);
		}
		List<SO_MissionMap> list = GlobalMissionManager.Instance.ListAllMission.ToList();
		if (GlobalSaveData.instance?.gameData != null)
		{
			for (int num5 = list.Count - 1; num5 >= 0; num5--)
			{
				if (list[num5] != null && list[num5].buildVersion > int.Parse(GlobalSaveData.instance.gameData.GameVersion))
				{
					list.RemoveAt(num5);
				}
			}
		}
		int num6 = list.Count + 1;
		UnityEngine.Random.InitState(seed + lifeSeed);
		foreach (SO_MissionMap item in MissionBoard.ListMissionMiniMapPhase1)
		{
			if (item != null && item.isInstantiate)
			{
				UnityEngine.Object.Destroy(item);
			}
		}
		MissionBoard.ListMissionMiniMapPhase1.Clear();
		foreach (SO_MissionMap item2 in MissionBoard.ListMissionMiniMapPhase2)
		{
			if (item2 != null && item2.isInstantiate)
			{
				UnityEngine.Object.Destroy(item2);
			}
		}
		MissionBoard.ListMissionMiniMapPhase2.Clear();
		foreach (SO_MissionMap item3 in MissionBoard.ListMissionMiniMapPhase0.ToList())
		{
			SO_MissionMap sO_MissionMap = UnityEngine.Object.Instantiate(item3);
			sO_MissionMap.MissionIDByMap = item3.MissionIDByMap;
			sO_MissionMap.MissionID = num6;
			sO_MissionMap.isInstantiate = true;
			MissionBoard.ListMissionMiniMapPhase1.Add(sO_MissionMap);
			num6++;
		}
		foreach (SO_MissionMap item4 in MissionBoard.ListMissionMiniMapPhase1.ToList())
		{
			SO_MissionMap sO_MissionMap2 = UnityEngine.Object.Instantiate(item4);
			sO_MissionMap2.MissionIDByMap = item4.MissionIDByMap;
			sO_MissionMap2.MissionID = num6;
			sO_MissionMap2.isInstantiate = true;
			MissionBoard.ListMissionMiniMapPhase2.Add(sO_MissionMap2);
			num6++;
		}
		for (int num7 = 0; num7 < 3; num7++)
		{
			UnityEngine.Random.InitState(seed + num7 * lifeSeed);
			GenerateMap(num7, isSetClearNotNullMission);
			UnityEngine.Random.InitState(seed);
			RandomizeObjective(num7);
			UnityEngine.Random.InitState(seed);
			RandomizeModifierMapSelectionByPath(num7);
		}
		UnityEngine.Random.InitState(seed);
		foreach (MissionSelection item5 in MissionBoard.AllMissionSelection)
		{
			if (item5.isNeedToChangeIDMission)
			{
				SO_MissionMap sO_MissionMap3 = UnityEngine.Object.Instantiate(item5.MissionData);
				sO_MissionMap3.MissionIDByMap = item5.MissionData.MissionIDByMap;
				sO_MissionMap3.MissionID = num6;
				sO_MissionMap3.IsLocked = true;
				sO_MissionMap3.isInstantiate = true;
				item5.MissionData = sO_MissionMap3;
				item5.ResetMissionData();
				item5.SetUI();
				num6++;
			}
			if (item5.IsHide)
			{
				item5.GetComponent<Button>().enabled = false;
				item5.MapImage.gameObject.SetActive(value: false);
				item5.InactiveImage.gameObject.SetActive(value: false);
				item5.IconCleared.gameObject.SetActive(value: false);
			}
			if ((GameManagerPhoton.Instance.isLoadMap | isCheckingClearedMission) && item5.IsCleared)
			{
				item5.GetComponent<Button>().enabled = false;
				item5.MissionData.IsHide = false;
				item5.MissionData.IsCleared = true;
				item5.IconCleared.gameObject.SetActive(value: true);
				item5.MapImage.sprite = item5.MissionData.MapImage;
				item5.MapImage.material = MatGrayscale;
				item5.TextMapName.SetTerm(item5.MissionData.MapNameLocalization);
				if ((bool)item5.MissionData.MissionObjective)
				{
					item5.StickerObjective.gameObject.SetActive(value: true);
					item5.StickerObjective.sprite = item5.MissionData.MissionObjective.IconSticker;
				}
				if (NetworkGameManager.Instance.isServer)
				{
					if (item5.isWinGetCheckpoint)
					{
						GameManagerPhoton.Instance.Phase = (byte)(item5.Phase + 1);
					}
					else if (GameManagerPhoton.Instance.Phase <= item5.Phase)
					{
						GameManagerPhoton.Instance.Phase = (byte)item5.Phase;
					}
				}
			}
			GameManagerPhoton.Instance.ListMission.Add(item5.MissionData);
			item5.MissionData.ListRequiredMapToUnlock.Clear();
			item5.MissionData.ListPossibleMapToUnlock.Clear();
		}
		foreach (MissionSelection item6 in MissionBoard.AllMissionSelection)
		{
			if (item6.isCheckpoint && item6.Phase == GameManagerPhoton.Instance.Phase && !item6.MissionData.IsCleared)
			{
				item6.GetComponent<Button>().enabled = true;
				item6.IconObjective.gameObject.SetActive(value: false);
				Debug.Log(item6.name);
				item6.IsLocked = false;
				item6.MissionData.IsLocked = false;
				item6.MissionData.IsHide = false;
				item6.MapImage.gameObject.SetActive(value: true);
				item6.InactiveImage.gameObject.SetActive(value: false);
				item6.IconCleared.gameObject.SetActive(value: false);
				item6.MapImage.sprite = item6.MissionData.MapImage;
				item6.TextMapName.SetTerm(item6.MissionData.MapNameLocalization);
				if ((bool)item6.MissionData.MissionObjective)
				{
					item6.StickerObjective.gameObject.SetActive(value: true);
					item6.StickerObjective.sprite = item6.MissionData.MissionObjective.IconSticker;
				}
			}
			foreach (MissionSelection item7 in item6.ListReqMissionUnlocked)
			{
				MissionSelection missionReq = item7;
				item6.MissionData.ListRequiredMapToUnlock.Add(missionReq.MissionData);
				AddPossibleMapUnlock(item6.MissionData);
				void AddPossibleMapUnlock(SO_MissionMap missionMap)
				{
					if ((bool)missionReq.MissionData && !missionReq.MissionData.ListPossibleMapToUnlock.Contains(missionMap))
					{
						missionReq.MissionData.ListPossibleMapToUnlock.Add(missionMap);
					}
				}
			}
		}
		SetDisableMap();
		UnityEngine.Random.InitState(seed + GameManagerPhoton.Instance.Life);
		foreach (MissionSelection item8 in MissionBoard.AllMissionSelection)
		{
			item8.MissionData.PlayerSpawningIdx = UnityEngine.Random.Range(0, item8.MissionData.TotalPlayerSpawningPosition);
			if (item8.MissionData.MissionObjective.IsSpawnEndlessHordeFromBeginning)
			{
				item8.MissionData.PlayerSpawningIdx = 0;
			}
		}
		UnityEngine.Random.InitState(seed);
		MissionBoard.InitWeapon();
		GameManagerPhoton.Instance.isInitializedLockedMap = true;
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
		initializedMap = true;
		GameManagerPhoton.Instance.isLoadMap = false;
	}

	private void GenerateMap(int Phase, bool isSetClearNotNullMission = true)
	{
		int num = 0;
		List<SO_MissionMap> list = MissionBoard.ListMissionNormalMap.ToList();
		List<SO_MissionMap> list2 = new List<SO_MissionMap>();
		foreach (MissionSelection item in MissionBoard.AllMissionSelection)
		{
			if (item.Phase != Phase)
			{
				continue;
			}
			if (item.IsEasyMap)
			{
				num++;
				int index = UnityEngine.Random.Range(0, MissionBoard.ListMissionEasyMapPhase0.Count);
				UnityEngine.Random.Range(0, GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv1.Count);
				if (item.MissionData == null)
				{
					item.MissionData = MissionBoard.ListMissionEasyMapPhase0[index];
					item.ResetMissionData();
					item.SetUI();
				}
				else
				{
					if (isSetClearNotNullMission)
					{
						item.ResetMissionData();
					}
					item.SetUI();
				}
				MissionBoard.ListMissionEasyMapPhase0.RemoveAt(index);
			}
			else if (item.IsMiniMap)
			{
				int index2 = UnityEngine.Random.Range(0, MissionBoard.ListMissionMiniMapPhase0.Count);
				if (Phase == 1)
				{
					index2 = UnityEngine.Random.Range(0, MissionBoard.ListMissionMiniMapPhase1.Count);
				}
				if (Phase == 2)
				{
					index2 = UnityEngine.Random.Range(0, MissionBoard.ListMissionMiniMapPhase2.Count);
				}
				if (item.MissionData == null)
				{
					switch (Phase)
					{
					case 0:
						item.MissionData = MissionBoard.ListMissionMiniMapPhase0[index2];
						break;
					case 1:
						item.MissionData = MissionBoard.ListMissionMiniMapPhase1[index2];
						break;
					default:
						item.MissionData = MissionBoard.ListMissionMiniMapPhase2[index2];
						break;
					}
					item.IsLocked = true;
					item.ResetMissionData();
					item.SetUI();
				}
				else
				{
					if (isSetClearNotNullMission)
					{
						item.ResetMissionData();
					}
					item.SetUI();
				}
				switch (Phase)
				{
				case 0:
					MissionBoard.ListMissionMiniMapPhase0.RemoveAt(index2);
					break;
				case 1:
					MissionBoard.ListMissionMiniMapPhase1.RemoveAt(index2);
					break;
				default:
					MissionBoard.ListMissionMiniMapPhase2.RemoveAt(index2);
					break;
				}
			}
			else if (item.IsNormalMap)
			{
				for (int num2 = list.Count - 1; num2 >= 0; num2--)
				{
					bool flag = false;
					foreach (SO_MissionMap item2 in list[num2].ListCombinationMapFrom)
					{
						if (item.ListReqMissionUnlocked.Count <= 0)
						{
							continue;
						}
						if (item.ListReqMissionUnlocked[0].MissionData.ListCombinationMapFrom.Count > 0)
						{
							if (item.ListReqMissionUnlocked[0].MissionData.ListCombinationMapFrom[0].MissionIDByMap == item2.MissionIDByMap)
							{
								flag = true;
							}
						}
						else if (item2.MissionIDByMap == item.ListReqMissionUnlocked[0].MissionData.MissionIDByMap)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						foreach (SO_MissionMap item3 in list2)
						{
							if (item3.MissionIDByMap == list[num2].MissionIDByMap)
							{
								flag = true;
							}
						}
					}
					if (flag)
					{
						list.RemoveAt(num2);
					}
				}
				if (list.Count == 0)
				{
					list = MissionBoard.ListMissionNormalMap.ToList();
					for (int num3 = list.Count - 1; num3 >= 0; num3--)
					{
						bool flag2 = false;
						foreach (SO_MissionMap item4 in list2)
						{
							if (item4.MissionIDByMap == list[num3].MissionIDByMap)
							{
								flag2 = true;
							}
						}
						if (flag2)
						{
							list.RemoveAt(num3);
						}
					}
				}
				int index3 = UnityEngine.Random.Range(0, list.Count);
				if (item.MissionData == null)
				{
					list2.Add(list[index3]);
					item.MissionData = list[index3];
					item.IsLocked = true;
					item.ResetMissionData();
					item.SetUI();
					list.RemoveAt(index3);
				}
				else
				{
					if (isSetClearNotNullMission)
					{
						item.ResetMissionData();
					}
					item.SetUI();
				}
			}
			else if (!item.IsBoss)
			{
				int num4 = UnityEngine.Random.Range(0, MissionBoard.ListMissionBigMap.Count);
				if (item.MissionData == null)
				{
					item.MissionData = MissionBoard.ListMissionBigMap[num4];
					item.IsLocked = true;
					item.ResetMissionData();
					item.SetUI();
					if (num4 < MissionBoard.ListMissionBigMap.Count)
					{
						MissionBoard.ListMissionBigMap.RemoveAt(num4);
					}
				}
				else
				{
					if (isSetClearNotNullMission)
					{
						item.ResetMissionData();
					}
					item.SetUI();
				}
			}
			else
			{
				if (isSetClearNotNullMission)
				{
					item.ResetMissionData();
				}
				item.SetUI();
			}
			item.MissionData.ListModifier.Clear();
		}
	}

	private void RandomizeObjective(int Phase)
	{
		List<SO_MissionObjective> list = GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv1.ToList();
		List<SO_MissionObjective> list2 = GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv2.ToList();
		List<SO_MissionObjective> list3 = GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv3.ToList();
		List<SO_MissionObjective> list4 = GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv4.ToList();
		foreach (MissionSelection item in MissionBoard.AllMissionSelection)
		{
			if (item.Phase == Phase && !item.MissionData.IsFixedMissionObjective)
			{
				if (item.objectiveLevel == 0)
				{
					int index = UnityEngine.Random.Range(0, list.Count);
					item.MissionData.MissionObjective = list[index];
					list.RemoveAt(index);
				}
				else if (item.objectiveLevel == 1)
				{
					int index2 = UnityEngine.Random.Range(0, list2.Count);
					item.MissionData.MissionObjective = list2[index2];
					list2.RemoveAt(index2);
				}
				else if (item.objectiveLevel == 2)
				{
					int index3 = UnityEngine.Random.Range(0, list3.Count);
					item.MissionData.MissionObjective = list3[index3];
					list3.RemoveAt(index3);
				}
				else if (item.objectiveLevel == 3)
				{
					int index4 = UnityEngine.Random.Range(0, list4.Count);
					item.MissionData.MissionObjective = list4[index4];
					list4.RemoveAt(index4);
				}
				if (list.Count == 0)
				{
					list = GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv1.ToList();
				}
				if (list2.Count == 0)
				{
					list2 = GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv2.ToList();
				}
				if (list3.Count == 0)
				{
					list3 = GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv3.ToList();
				}
				if (list4.Count == 0)
				{
					list4 = GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv4.ToList();
				}
				item.SetUI();
			}
		}
	}

	private void RandomizeModifierMapSelectionByPath(int phase)
	{
		List<SO_MissionModifierEffect> list = GlobalMissionManager.Instance.ListAllMissionModifier.ToList();
		foreach (MissionPathDifficulty item in MissionBoard.ListMissionPathDifficulty)
		{
			int num = 0;
			for (int i = 0; i < item.Listmission.Count; i++)
			{
				MissionSelection missionSelection = item.Listmission[i];
				if (!missionSelection)
				{
					continue;
				}
				int num2 = UnityEngine.Random.Range(missionSelection.minMissionModifier, item.totalDifficultyScore - num + 1);
				if (i == item.Listmission.Count - 1)
				{
					num2 = item.totalDifficultyScore - num;
				}
				if (missionSelection.minMissionModifier >= 1 && num2 <= 0)
				{
					num2 = missionSelection.minMissionModifier;
				}
				missionSelection.totalMissionModifier = 0;
				if (missionSelection.Phase != phase)
				{
					continue;
				}
				missionSelection.MissionData.ListModifier.Clear();
				if (item.totalDifficultyScore > 0)
				{
					if (missionSelection.totalMissionModifier >= 2)
					{
						continue;
					}
					for (int num3 = list.Count - 1; num3 >= 0; num3--)
					{
						if (list[num3].isDisable)
						{
							list.Remove(list[num3]);
						}
					}
					int num4 = 0;
					for (int j = 0; j < missionSelection.ListImageModifier.Count; j++)
					{
						if (num4 < num2)
						{
							int rdmDifficulty = UnityEngine.Random.Range(1, num2 + 1);
							if (rdmDifficulty > 1)
							{
								rdmDifficulty = UnityEngine.Random.Range(num2 - 1, num2 + 1);
							}
							if (j == missionSelection.ListImageModifier.Count - 1)
							{
								rdmDifficulty = num2 - num4;
							}
							for (int num5 = list.Count - 1; num5 >= 0; num5--)
							{
								if (list[num5].DifficultyScore != rdmDifficulty)
								{
									list.RemoveAt(num5);
								}
							}
							for (int num6 = j - 1; num6 >= 0; num6--)
							{
								list.Remove(missionSelection.MissionData.ListModifier[num6]);
							}
							if (list.Count == 0)
							{
								list = GlobalMissionManager.Instance.ListAllMissionModifier.Where((SO_MissionModifierEffect item) => item.DifficultyScore == rdmDifficulty && !item.isDisable).ToList();
								for (int num7 = j - 1; num7 >= 0; num7--)
								{
									list.Remove(missionSelection.MissionData.ListModifier[num7]);
								}
							}
							int index = UnityEngine.Random.Range(0, list.Count);
							bool flag = true;
							while (flag)
							{
								flag = false;
								for (int num8 = 0; num8 < list[index].DisableModifierIfThisObjective.Count; num8++)
								{
									if (list[index].DisableModifierIfThisObjective[num8] == missionSelection.MissionData.MissionObjective)
									{
										flag = true;
										list.RemoveAt(index);
										if (list.Count == 0)
										{
											list = GlobalMissionManager.Instance.ListAllMissionModifier.ToList();
										}
										index = UnityEngine.Random.Range(0, list.Count);
										break;
									}
								}
							}
							flag = false;
							for (int num9 = 0; num9 < list[index].ChangeToOtherModifierIfThisObjective.Count; num9++)
							{
								if (list[index].ChangeToOtherModifierIfThisObjective[num9] == missionSelection.MissionData.MissionObjective)
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								SetModifier(missionSelection, list[index].OtherModifier, j);
							}
							else
							{
								SetModifier(missionSelection, list[index], j);
								list.RemoveAt(index);
								if (list.Count == 0)
								{
									list = GlobalMissionManager.Instance.ListAllMissionModifier.ToList();
								}
							}
							missionSelection.ListImageModifier[j].enabled = true;
							int num10 = num;
							List<SO_MissionModifierEffect> listModifier = missionSelection.MissionData.ListModifier;
							num = num10 + listModifier[listModifier.Count - 1].DifficultyScore;
							int num11 = num4;
							List<SO_MissionModifierEffect> listModifier2 = missionSelection.MissionData.ListModifier;
							num4 = num11 + listModifier2[listModifier2.Count - 1].DifficultyScore;
							missionSelection.totalMissionModifier++;
						}
						else
						{
							missionSelection.ListImageModifier[j].enabled = false;
						}
					}
				}
				else
				{
					DisableModifier(missionSelection);
				}
			}
		}
	}

	private void SetModifier(MissionSelection missionSelect, SO_MissionModifierEffect modifierEffect, int idxListModifier)
	{
		missionSelect.MissionData.ListModifier.Add(modifierEffect);
		missionSelect.ListImageModifier[idxListModifier].sprite = modifierEffect.spriteSticker;
	}

	private void DisableModifier(MissionSelection missionSelect)
	{
		missionSelect.MissionData.ListModifier.Clear();
		for (int i = 0; i < missionSelect.ListImageModifier.Count; i++)
		{
			missionSelect.ListImageModifier[i].enabled = false;
		}
	}

	public void OnInputMove(InputAction.CallbackContext value)
	{
		if (MissionDetailMap.IsVisible)
		{
			return;
		}
		if (value.ReadValue<Vector2>().x < -0.5f)
		{
			if (isInputMovePressed)
			{
				return;
			}
			isInputMovePressed = true;
			AudioManager.PlaySFX("ui_select");
			if (missionBrief.enabled)
			{
				missionBrief.changePage(IncreasePage: false);
			}
			else
			{
				DisableMap();
				for (int num = selectCol - 1; num >= 0; num--)
				{
					if (!MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[num] || !MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[num].MissionData.IsCleared)
					{
						selectCol = num;
						break;
					}
				}
			}
			MissionSelect(-1);
			MissionBoard.MapTransform.DOKill();
			MissionBoard.MapTransform.DOAnchorPosX(MissionBoard.ListXPositionScrollMapByPhase[MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[selectCol].Phase], 0.5f);
		}
		else if (value.ReadValue<Vector2>().x > 0.5f)
		{
			if (isInputMovePressed)
			{
				return;
			}
			isInputMovePressed = true;
			AudioManager.PlaySFX("ui_select");
			if (missionBrief.enabled)
			{
				missionBrief.changePage(IncreasePage: true);
			}
			else
			{
				DisableMap();
				for (int i = selectCol + 1; i < MissionBoard.ListMissionMapNavigation[selectRow].missionSelect.Count; i++)
				{
					if (!MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[i] || !MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[i].MissionData.IsCleared)
					{
						selectCol = i;
						break;
					}
				}
			}
			MissionSelect(1);
			MissionBoard.MapTransform.DOKill();
			MissionBoard.MapTransform.DOAnchorPosX(MissionBoard.ListXPositionScrollMapByPhase[MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[selectCol].Phase], 0.5f);
		}
		else if (value.ReadValue<Vector2>().y < -0.5f)
		{
			if (isInputMovePressed)
			{
				return;
			}
			isInputMovePressed = true;
			AudioManager.PlaySFX("ui_select");
			DisableMap();
			int num2 = selectCol;
			for (int j = selectRow + 1; j < MissionBoard.ListMissionMapNavigation.Count; j++)
			{
				if (num2 >= MissionBoard.ListMissionMapNavigation[j].missionSelect.Count)
				{
					num2 = MissionBoard.ListMissionMapNavigation[j].missionSelect.Count - 1;
				}
				if ((bool)MissionBoard.ListMissionMapNavigation[j].missionSelect[num2] && !MissionBoard.ListMissionMapNavigation[j].missionSelect[num2].MissionData.IsCleared)
				{
					selectCol = num2;
					selectRow = j;
					break;
				}
			}
			MissionSelect(0, -1);
		}
		else if (value.ReadValue<Vector2>().y > 0.5f)
		{
			if (isInputMovePressed)
			{
				return;
			}
			isInputMovePressed = true;
			AudioManager.PlaySFX("ui_select");
			DisableMap();
			int num3 = selectCol;
			for (int num4 = selectRow - 1; num4 >= 0; num4--)
			{
				if (num3 >= MissionBoard.ListMissionMapNavigation[num4].missionSelect.Count)
				{
					num3 = MissionBoard.ListMissionMapNavigation[num4].missionSelect.Count - 1;
				}
				if ((bool)MissionBoard.ListMissionMapNavigation[num4].missionSelect[num3] && !MissionBoard.ListMissionMapNavigation[num4].missionSelect[num3].MissionData.IsCleared)
				{
					selectRow = num4;
					selectCol = num3;
					break;
				}
			}
			MissionSelect(0, 1);
		}
		else
		{
			isInputMovePressed = false;
		}
	}

	public void MissionSelect(int directionX = 0, int directionY = 0)
	{
		if ((bool)MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[selectCol] && !MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[selectCol].IsCleared)
		{
			MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[selectCol].BtnMission.Select();
			MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[selectCol].SetHighlight(isTurnOn: true, 0.2f);
			MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[selectCol].IsSelected = true;
			return;
		}
		int index = selectCol;
		int index2 = selectRow;
		bool flag = false;
		if (directionX < 0)
		{
			for (int num = selectCol; num >= 0; num--)
			{
				if ((bool)MissionBoard.ListMissionMapNavigation[index2].missionSelect[num] && !MissionBoard.ListMissionMapNavigation[index2].missionSelect[num].IsCleared)
				{
					flag = true;
					index = num;
					break;
				}
			}
		}
		else if (directionX > 0)
		{
			for (int i = selectCol; i < MissionBoard.ListMissionMapNavigation[index2].missionSelect.Count; i++)
			{
				if ((bool)MissionBoard.ListMissionMapNavigation[index2].missionSelect[i] && !MissionBoard.ListMissionMapNavigation[index2].missionSelect[i].IsCleared)
				{
					flag = true;
					index = i;
					break;
				}
			}
		}
		if (!flag)
		{
			for (int j = 0; j < MissionBoard.ListMissionMapNavigation.Count; j++)
			{
				if ((bool)MissionBoard.ListMissionMapNavigation[j].missionSelect[index] && !MissionBoard.ListMissionMapNavigation[j].missionSelect[index].IsCleared)
				{
					index2 = j;
					break;
				}
			}
		}
		if ((bool)MissionBoard.ListMissionMapNavigation[index2].missionSelect[index])
		{
			selectCol = index;
			selectRow = index2;
			MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[selectCol].BtnMission.Select();
			MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[selectCol].SetHighlight(isTurnOn: true, 0.2f);
			MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[selectCol].IsSelected = true;
		}
	}

	public void DisableMap()
	{
		if ((bool)MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[selectCol] && MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[selectCol].MissionData.MissionID != NetworkGameManager.Instance.Mission)
		{
			MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[selectCol].SetHighlight(isTurnOn: false, 0.2f);
			MissionBoard.ListMissionMapNavigation[selectRow].missionSelect[selectCol].IsSelected = false;
		}
	}

	public void ReadyBtnClicked(UIButton button)
	{
		if (NetworkGameManager.Instance.ownPlayer.network.GetReadyLobby())
		{
			txtReady.SetTerm("Menu/Ready");
		}
		else
		{
			UniTaskUtil.DelayedCall(this, 0.1f, BackBtnClicked).Forget();
			UniTaskUtil.DelayedCall(this, 0.2f, () =>
			{
				txtReady.SetTerm("Menu/NotReady");
			}).Forget();
		}
		NetworkGameManager.Instance.ownPlayer.network.SetPlayerReady(!NetworkGameManager.Instance.ownPlayer.network.GetReadyLobby());
	}

	public void BackBtnClicked()
	{
		UIMenu.Hide();
		if (!UIGameManager.Instance.isUIInvisible)
		{
			UIGameManager.Instance.uiInGame.Show();
		}
		NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
		NetworkGameManager.Instance.ownPlayer.functionItemCollision = "";
		ChatSystem.Instance.ItemCommand.SetActive(value: false);
		EventSystem.current.SetSelectedGameObject(null);
	}

	public void OnEnable()
	{
		input = new PlayerInputActions();
	}

	public void OnShow()
	{
		SetUIMission();
		if (input == null)
		{
			input = new PlayerInputActions();
		}
		input.Player.Enable();
		SetMapNavigation();
		input.Player.NavigateUI.performed += OnInputMove;
		input.Player.Map.performed += OnShowHideBriefing;
		bool flag = false;
		for (int i = 0; i < MissionBoard.ListMissionMapNavigation.Count; i++)
		{
			for (int j = 0; j < MissionBoard.ListMissionMapNavigation[i].missionSelect.Count; j++)
			{
				if ((bool)MissionBoard.ListMissionMapNavigation[i].missionSelect[j] && MissionBoard.ListMissionMapNavigation[i].missionSelect[j].IsSelected)
				{
					flag = true;
					selectRow = i;
					selectCol = j;
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		if ((bool)MissionBoard.GetMissionSelection(GameManager.Instance.gameManagerPhoton.Mission))
		{
			MissionSelection missionSelection = MissionBoard.GetMissionSelection(GameManager.Instance.gameManagerPhoton.Mission);
			missionSelection.IsSelected = true;
			missionSelection.SetMissionGlobal();
			missionSelection.BtnMission.Select();
			missionSelection.SetHighlight(isTurnOn: true, 0f);
		}
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerController)
		{
			if (item.network.GetIDX() > 0 && item.network.playerPhoton.MissionVote > 0)
			{
				MissionBoard.ListPlayerVote[item.network.GetIDX() - 1].gameObject.SetActive(value: true);
				MissionBoard.ListPlayerVote[item.network.GetIDX() - 1].Play(item.data.PlayerSkinData.GetPlayerAvatarSkin());
				MissionBoard.ListPlayerVote[item.network.GetIDX() - 1].transform.parent.position = MissionBoard.GetMissionSelection(item.network.playerPhoton.MissionVote).transform.position;
			}
		}
		MissionBoard.MovePinAndStringLine();
		SetDisableMap();
		MissionBoard.MapTransform.DOKill();
		MissionBoard.MapTransform.DOAnchorPosX(MissionBoard.ListXPositionScrollMapByPhase[GameManagerPhoton.Instance.Phase], 0.5f);
	}

	public void SetMapNavigation()
	{
		MissionBoard.ListMissionMapNavigation.Clear();
		int num = 0;
		for (int i = 0; i < MissionBoard.ListMissionMapPerRows.Count; i++)
		{
			MissionBoard.ListMissionMapNavigation.Add(new MissionMapPerRow());
			for (int j = 0; j < MissionBoard.ListMissionMapPerRows[i].missionSelect.Count; j++)
			{
				if (MissionBoard.ListMissionMapPerRows[i].missionSelect[j] == null || ((bool)MissionBoard.ListMissionMapPerRows[i].missionSelect[j].MissionData && !MissionBoard.ListMissionMapPerRows[i].missionSelect[j].MissionData.IsHide))
				{
					if ((bool)MissionBoard.ListMissionMapPerRows[i].missionSelect[j])
					{
						MissionBoard.ListMissionMapPerRows[i].missionSelect[j].SetHighlight(isTurnOn: false, 0f);
					}
					MissionBoard.ListMissionMapNavigation[num].missionSelect.Add(MissionBoard.ListMissionMapPerRows[i].missionSelect[j]);
				}
			}
			if (MissionBoard.ListMissionMapNavigation[num].missionSelect.Count == 0)
			{
				MissionBoard.ListMissionMapNavigation.RemoveAt(num);
				num--;
			}
			num++;
		}
	}

	public void OnHide()
	{
		if (input != null)
		{
			input.Player.NavigateUI.performed -= OnInputMove;
			input.Player.Map.performed -= OnShowHideBriefing;
		}
		if (!_isFirstHide)
		{
			AudioManager.PlaySFX("ui_cancel");
		}
		_isFirstHide = false;
		if (UIGameManager.Instance.uiObjective != null)
		{
			UIGameManager.Instance.uiObjective.SetActive(value: true);
		}
		if (input != null)
		{
			input.Player.Disable();
		}
	}

	public void OnShowHideBriefing(InputAction.CallbackContext value)
	{
		if (!missionBrief.enabled)
		{
			ShowMissionBriefing();
			return;
		}
		AudioManager.PlaySFX("ui_cancel");
		missionBrief.enabled = false;
	}

	public void ShowMissionBriefing()
	{
		AudioManager.PlaySFX("ui_confirm");
		missionBrief.enabled = true;
	}

	public void SetUIMissionClear(int newIdxMissionClear = -1)
	{
		SetUIMission(newIdxMissionClear);
		NetworkGameManager.Instance.Mission = GameManager.Instance.gameManagerPhoton.Mission;
		UIGameManager.Instance.SetMissionLocation(UIGameManager.Instance.missionLocationText, null, UIGameManager.Instance.missionLocationTextField);
	}

	public void SetUIMission(int newIdxMissionClear = -1)
	{
		for (int num = GameManagerPhoton.Instance.ArrMissionCleared.Length - 1; num >= 0; num--)
		{
			MissionSelection missionSelection = MissionBoard.GetMissionSelection(num + 1);
			if (missionSelection != null && (bool)missionSelection.MissionData)
			{
				if (GameManagerPhoton.Instance.ArrMissionLocked.Get(num))
				{
					missionSelection.MissionData.IsLocked = true;
				}
				else
				{
					missionSelection.MissionData.IsLocked = false;
				}
				if (GameManagerPhoton.Instance.ArrMissionCleared.Get(num) || newIdxMissionClear == num)
				{
					missionSelection.GetComponent<Button>().enabled = false;
					missionSelection.IconCleared.gameObject.SetActive(value: true);
					missionSelection.MapImage.material = MatGrayscale;
					missionSelection.MissionData.IsCleared = true;
					missionSelection.MissionData.IsHide = false;
					missionSelection.MapImage.sprite = missionSelection.MissionData.MapImage;
					missionSelection.MapImage.material = MatGrayscale;
					missionSelection.TextMapName.SetTerm(missionSelection.MissionData.MapNameLocalization);
					if ((bool)missionSelection.MissionData.MissionObjective)
					{
						missionSelection.StickerObjective.gameObject.SetActive(value: true);
						missionSelection.StickerObjective.sprite = missionSelection.MissionData.MissionObjective.IconSticker;
					}
				}
			}
		}
		foreach (MissionSelection item in MissionBoard.AllMissionSelection)
		{
			if (item.ListMissionDisableAfterClear.Count > 0 && (bool)item.MissionData && item.MissionData.IsCleared)
			{
				foreach (MissionSelection item2 in item.ListMissionDisableAfterClear)
				{
					item2.InactiveImage.gameObject.SetActive(value: true);
					item2.IconCleared.gameObject.SetActive(value: false);
					item2.MapImage.gameObject.SetActive(value: false);
					item2.GetComponent<Button>().enabled = false;
					foreach (MissionMapPerRow listMissionMapPerRow in MissionBoard.ListMissionMapPerRows)
					{
						for (int i = 0; i < listMissionMapPerRow.missionSelect.Count; i++)
						{
							if ((bool)listMissionMapPerRow.missionSelect[i] && (bool)item2.MissionData && item2.MissionData.MissionID == listMissionMapPerRow.missionSelect[i].MissionData.MissionID)
							{
								listMissionMapPerRow.missionSelect[i] = null;
							}
						}
					}
				}
			}
			if (item.ListReqMissionUnlocked.Count <= 0 || !item.MissionData || item.MissionData.IsCleared)
			{
				continue;
			}
			bool flag = false;
			foreach (MissionSelection item3 in item.ListReqMissionUnlocked)
			{
				if (item3.MissionData != null)
				{
					if (GameManagerPhoton.Instance.ArrMissionCleared.Get(item3.MissionData.MissionID - 1) && item.IsNeedOneReqOnly)
					{
						flag = false;
						item.MissionData.IsHide = false;
						item.GetComponent<Button>().enabled = true;
						item.MapImage.gameObject.SetActive(value: true);
						item.InactiveImage.gameObject.SetActive(value: false);
						item.IconCleared.gameObject.SetActive(value: false);
						break;
					}
					if (!GameManagerPhoton.Instance.ArrMissionCleared.Get(item3.MissionData.MissionID - 1))
					{
						flag = true;
					}
					if (!flag && item.MissionData.IsHide)
					{
						item.MissionData.IsHide = false;
						item.GetComponent<Button>().enabled = true;
						item.MapImage.gameObject.SetActive(value: true);
						item.InactiveImage.gameObject.SetActive(value: false);
						item.IconCleared.gameObject.SetActive(value: false);
					}
				}
			}
			if (!flag && GameManagerPhoton.Instance.isInitializedLockedMap)
			{
				item.MissionData.IsLocked = false;
				GameManagerPhoton.Instance.ArrMissionLocked.Set(item.MissionData.MissionID - 1, value: false);
			}
			if (!item.MissionData.IsLocked)
			{
				item.SetUI();
				item.MissionData.IsHide = false;
			}
			if (item.MissionData.IsHide)
			{
				item.GetComponent<Button>().enabled = false;
				item.MapImage.gameObject.SetActive(value: false);
				item.InactiveImage.gameObject.SetActive(value: false);
				item.IconCleared.gameObject.SetActive(value: false);
			}
		}
	}

	public void RandomizeIdxSpawnPlayer()
	{
		UnityEngine.Random.InitState(GameManagerPhoton.Instance.Seed + GameManagerPhoton.Instance.Life);
		foreach (MissionSelection item in MissionBoard.AllMissionSelection)
		{
			item.MissionData.PlayerSpawningIdx = UnityEngine.Random.Range(0, item.MissionData.TotalPlayerSpawningPosition);
			if (item.MissionData.MissionObjective.IsSpawnEndlessHordeFromBeginning)
			{
				item.MissionData.PlayerSpawningIdx = 0;
			}
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	private void SetDisableMap()
	{
		foreach (MissionSelection item in MissionBoard.AllMissionSelection)
		{
			if (item.ListMissionDisableAfterClear.Count <= 0 || !item.MissionData || !item.MissionData.IsCleared)
			{
				continue;
			}
			foreach (MissionSelection item2 in item.ListMissionDisableAfterClear)
			{
				item2.MapImage.gameObject.SetActive(value: false);
				item2.InactiveImage.gameObject.SetActive(value: true);
				item2.IconCleared.gameObject.SetActive(value: false);
				item2.GetComponent<Button>().enabled = false;
				item2.StringLine.gameObject.SetActive(value: false);
				foreach (MissionMapPerRow listMissionMapPerRow in MissionBoard.ListMissionMapPerRows)
				{
					for (int i = 0; i < listMissionMapPerRow.missionSelect.Count; i++)
					{
						if ((bool)listMissionMapPerRow.missionSelect[i] && item2.MissionData.MissionID == listMissionMapPerRow.missionSelect[i].MissionData.MissionID)
						{
							listMissionMapPerRow.missionSelect[i] = null;
						}
					}
				}
			}
		}
	}

	private void OnDestroy()
	{
		if (!UIMenu.isHidden)
		{
			OnHide();
		}
		DOTween.Kill(this);
		if (Instance == this)
		{
			Instance = null;
		}
	}
}
