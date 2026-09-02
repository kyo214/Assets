using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionModifier", menuName = "WMO/ScriptableObjects/Mission/MissionModifier", order = 0)]
public class SO_MissionModifierEffect : ScriptableObject
{
	public int ID;

	public string Category;

	public int DifficultyScore;

	public List<MissionMapModifierValue> Modifier = new List<MissionMapModifierValue>();

	public List<SO_MissionObjective> ChangeToOtherModifierIfThisObjective = new List<SO_MissionObjective>();

	public List<SO_MissionObjective> DisableModifierIfThisObjective = new List<SO_MissionObjective>();

	public SO_MissionModifierEffect OtherModifier;

	public Sprite spriteIcon;

	public Sprite spriteSticker;

	public bool isDisable;

	[TermsPopup("")]
	public string ModifierNameLocalization;

	public void Init()
	{
		for (int i = 0; i < Modifier.Count; i++)
		{
			Modifier[i].ModifierStatus.CurrentValue = Modifier[i].ModifierStatus.InitValue;
		}
	}

	public void SetValueByDifficulty(int difficulty)
	{
		for (int i = 0; i < Modifier.Count; i++)
		{
			if (difficulty < Modifier[i].valueByDifficulty.Count)
			{
				Modifier[i].SetValue(Modifier[i].valueByDifficulty[difficulty]);
			}
			else
			{
				Modifier[i].SetValue(Modifier[i].valueByDifficulty[Modifier[i].valueByDifficulty.Count - 1]);
			}
		}
	}
}
