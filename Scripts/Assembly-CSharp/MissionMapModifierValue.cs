using System;
using System.Collections.Generic;

[Serializable]
public class MissionMapModifierValue
{
	public SO_MissionModifierStatus ModifierStatus;

	public List<float> valueByDifficulty = new List<float>();

	public void SetValue(float value)
	{
		ModifierStatus.CurrentValue = value;
	}
}
