using System;
using System.Collections.Generic;

[Serializable]
public class MissionPathDifficulty
{
	public List<MissionSelection> Listmission = new List<MissionSelection>();

	public int totalDifficultyScore;
}
