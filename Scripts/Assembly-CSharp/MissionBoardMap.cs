using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionBoardMap : MonoBehaviour
{
	public GameObject ImageMissionObject;

	public List<MissionMapPerRow> ListMissionMapPerRows = new List<MissionMapPerRow>();

	public List<MissionMapPerRow> ListMissionMapNavigation = new List<MissionMapPerRow>();

	public List<MissionSelection> AllMissionSelection = new List<MissionSelection>();

	public List<MissionPathDifficulty> ListMissionPathDifficulty = new List<MissionPathDifficulty>();

	public Localize TextScenarioDesc;

	public Localize TextScenarioLabel;

	public TMP_Text TextFieldScenarioDesc;

	public TMP_Text TextFieldScenarioLabel;

	public TMP_Text TextFieldModifierValue;

	public List<Localize> ListModifierLocalizeText = new List<Localize>();

	public List<Image> ListModifierIcon = new List<Image>();

	public List<SO_MissionMap> ListMissionEasyMap = new List<SO_MissionMap>();

	public List<SO_MissionMap> ListMissionEasyMapPhase0 = new List<SO_MissionMap>();

	public List<SO_MissionMap> ListMissionMiniMap = new List<SO_MissionMap>();

	public List<SO_MissionMap> ListMissionMiniMapPhase0 = new List<SO_MissionMap>();

	public List<SO_MissionMap> ListMissionMiniMapPhase1 = new List<SO_MissionMap>();

	public List<SO_MissionMap> ListMissionMiniMapPhase2 = new List<SO_MissionMap>();

	public List<SO_MissionMap> ListMissionNormalMap = new List<SO_MissionMap>();

	public List<SO_MissionMap> ListMissionBigMap = new List<SO_MissionMap>();

	public List<WeaponMapType> ListMeleeWeaponMap = new List<WeaponMapType>();

	public List<WeaponMapType> ListRangeWeaponMap = new List<WeaponMapType>();

	public List<WeaponMapType> ListSpecialWeaponMap = new List<WeaponMapType>();

	public List<WeaponMapType> ListUtiliyWeaponMap = new List<WeaponMapType>();

	private List<WeaponMapType> ListMeleeWeaponMapTemp = new List<WeaponMapType>();

	private List<WeaponMapType> ListRangeWeaponMapTemp = new List<WeaponMapType>();

	private List<WeaponMapType> ListSpecialWeaponMapTemp = new List<WeaponMapType>();

	public List<Animator> ListPlayerVote = new List<Animator>();

	public List<Image> ListPossibleLoot = new List<Image>();

	public int TotalMeleeWeaponOnAllNormalMap;

	public int TotalSpecialWeaponOnAllNormalMap;

	public bool isPinAndStringLineMoved;

	public List<float> ListXPositionScrollMapByPhase = new List<float>();

	public RectTransform MapTransform;

	private void Awake()
	{
		for (int i = 0; i < AllMissionSelection.Count; i++)
		{
			AllMissionSelection[i].idxMission = i;
		}
	}

	public void MovePinAndStringLine()
	{
		if (isPinAndStringLineMoved)
		{
			return;
		}
		isPinAndStringLineMoved = true;
		for (int i = 0; i < AllMissionSelection.Count; i++)
		{
			if (AllMissionSelection[i].StringLine.childCount > 0)
			{
				AllMissionSelection[i].StringLine.transform.SetParent(AllMissionSelection[i].transform.parent, worldPositionStays: true);
			}
		}
		for (int j = 0; j < AllMissionSelection.Count; j++)
		{
			AllMissionSelection[j].PinMap.transform.SetParent(AllMissionSelection[j].transform.parent, worldPositionStays: true);
		}
	}

	public void InitWeapon()
	{
		if (GameManagerPhoton.Instance.isInitializedRandomizeWeapon)
		{
			return;
		}
		List<SO_MissionMap> list = new List<SO_MissionMap>();
		List<SO_MissionMap> list2 = ListMissionMiniMap.ToList();
		List<SO_MissionMap> list3 = ListMissionMiniMap.ToList();
		List<SO_MissionMap> list4 = ListMissionMiniMap.ToList();
		foreach (MissionSelection item in AllMissionSelection)
		{
			SO_MissionMap missionData = item.MissionData;
			missionData.ListWeapon.Clear();
			if (missionData.Difficulty == 1 && !item.IsEasyMap)
			{
				list.Add(missionData);
				list2.Add(missionData);
				list3.Add(missionData);
				list4.Add(missionData);
			}
			else if (missionData.Difficulty == 1)
			{
				list.Add(missionData);
				list2.Add(missionData);
			}
			else if (missionData.Difficulty == 2)
			{
				if (!missionData.MissionObjective.IsSpawnEndlessHordeFromBeginning)
				{
					list.Add(missionData);
				}
				for (int i = 0; i < missionData.TotalMeleeWeapon; i++)
				{
					if (ListMeleeWeaponMapTemp.Count == 0)
					{
						foreach (WeaponMapType item2 in ListMeleeWeaponMap)
						{
							ListMeleeWeaponMapTemp.Add(item2);
						}
					}
					int index = Random.Range(0, ListMeleeWeaponMapTemp.Count);
					missionData.ListWeapon.Add(ListMeleeWeaponMapTemp[index]);
					ListMeleeWeaponMapTemp.RemoveAt(index);
				}
				if (missionData.ListTotalRangeWeapon.Count > 0)
				{
					for (int j = 0; j < missionData.ListTotalRangeWeapon[GameManagerPhoton.Instance.Difficulty].totalWeapon; j++)
					{
						if (ListRangeWeaponMapTemp.Count == 0)
						{
							foreach (WeaponMapType item3 in ListRangeWeaponMap)
							{
								ListRangeWeaponMapTemp.Add(item3);
							}
						}
						int index2 = Random.Range(0, ListRangeWeaponMapTemp.Count);
						missionData.ListWeapon.Add(ListRangeWeaponMapTemp[index2]);
						ListRangeWeaponMapTemp.RemoveAt(index2);
					}
				}
				for (int k = 0; k < missionData.TotalSpecialWeapon; k++)
				{
					if (ListSpecialWeaponMapTemp.Count == 0)
					{
						foreach (WeaponMapType item4 in ListSpecialWeaponMap)
						{
							ListSpecialWeaponMapTemp.Add(item4);
						}
					}
					int index3 = Random.Range(0, ListSpecialWeaponMapTemp.Count);
					missionData.ListWeapon.Add(ListSpecialWeaponMapTemp[index3]);
					ListSpecialWeaponMapTemp.RemoveAt(index3);
				}
			}
			if (missionData.MinSpecialWeapon < 1 || missionData.ListWeapon.Count >= missionData.MinSpecialWeapon)
			{
				continue;
			}
			for (int l = 0; l < missionData.MinSpecialWeapon - missionData.ListWeapon.Count; l++)
			{
				if (ListSpecialWeaponMapTemp.Count == 0)
				{
					foreach (WeaponMapType item5 in ListSpecialWeaponMap)
					{
						ListSpecialWeaponMapTemp.Add(item5);
					}
				}
				int index4 = Random.Range(0, ListSpecialWeaponMapTemp.Count);
				missionData.ListWeapon.Add(ListSpecialWeaponMapTemp[index4]);
				ListSpecialWeaponMapTemp.RemoveAt(index4);
			}
		}
		int totalMeleeWeaponOnAllNormalMap = TotalMeleeWeaponOnAllNormalMap;
		ListMeleeWeaponMapTemp.Clear();
		for (int m = 0; m < totalMeleeWeaponOnAllNormalMap; m++)
		{
			ListMeleeWeaponMapTemp.Add(ListMeleeWeaponMap[m % ListMeleeWeaponMap.Count]);
		}
		if (list3.Count > 0)
		{
			for (int n = 0; n < ListMeleeWeaponMapTemp.Count; n++)
			{
				int index5 = Random.Range(0, list3.Count);
				list3[index5].ListWeapon.Add(ListMeleeWeaponMapTemp[n]);
				list3.RemoveAt(index5);
			}
		}
		ListRangeWeaponMapTemp.Clear();
		foreach (SO_MissionMap item6 in list2)
		{
			item6.ListWeapon.Clear();
			for (int num = 0; num < item6.ListTotalRangeWeapon[GameManagerPhoton.Instance.Difficulty].totalWeapon; num++)
			{
				if (ListRangeWeaponMapTemp.Count == 0)
				{
					foreach (WeaponMapType item7 in ListRangeWeaponMap)
					{
						ListRangeWeaponMapTemp.Add(item7);
					}
				}
				int index6 = Random.Range(0, ListRangeWeaponMapTemp.Count);
				WeaponMapType weaponMapType = null;
				for (int num2 = 0; num2 < item6.ListWeapon.Count; num2++)
				{
					if (item6.ListWeapon[num2] == ListRangeWeaponMapTemp[index6])
					{
						weaponMapType = item6.ListWeapon[num2];
						ListRangeWeaponMapTemp.RemoveAt(index6);
						break;
					}
				}
				if (weaponMapType != null)
				{
					if (ListRangeWeaponMapTemp.Count == 0)
					{
						foreach (WeaponMapType item8 in ListRangeWeaponMap)
						{
							if (weaponMapType != item8)
							{
								ListRangeWeaponMapTemp.Add(item8);
							}
						}
					}
					index6 = Random.Range(0, ListRangeWeaponMapTemp.Count);
					item6.ListWeapon.Add(ListRangeWeaponMapTemp[index6]);
				}
				else
				{
					item6.ListWeapon.Add(ListRangeWeaponMapTemp[index6]);
				}
				ListRangeWeaponMapTemp.RemoveAt(index6);
			}
		}
		int totalSpecialWeaponOnAllNormalMap = TotalSpecialWeaponOnAllNormalMap;
		ListSpecialWeaponMapTemp.Clear();
		for (int num3 = 0; num3 < totalSpecialWeaponOnAllNormalMap; num3++)
		{
			ListSpecialWeaponMapTemp.Add(ListSpecialWeaponMap[num3 % ListSpecialWeaponMap.Count]);
		}
		if (list4.Count > 0)
		{
			for (int num4 = 0; num4 < ListSpecialWeaponMapTemp.Count; num4++)
			{
				int index7 = Random.Range(0, list3.Count);
				list4[index7].ListWeapon.Add(ListSpecialWeaponMapTemp[num4]);
				list4.RemoveAt(index7);
			}
		}
		for (int num5 = 0; num5 < ListUtiliyWeaponMap.Count; num5++)
		{
			int index8 = Random.Range(0, list.Count);
			list[index8].ListWeapon.Add(ListUtiliyWeaponMap[num5]);
			list.RemoveAt(index8);
		}
		GameManagerPhoton.Instance.isInitializedRandomizeWeapon = true;
	}

	public MissionSelection GetMissionSelection(int missionID)
	{
		foreach (MissionSelection item in AllMissionSelection)
		{
			if ((bool)item.MissionData && item.MissionData.MissionID == missionID)
			{
				return item;
			}
		}
		return null;
	}
}
