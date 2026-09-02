using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGCalcSaverString
{
	private readonly BGCalcGraph graph;

	public BGCalcSaverString(BGCalcGraph graph)
	{
		this.graph = graph;
	}

	public string Save()
	{
		return JsonUtility.ToJson(new BGCalcGraphModel(graph));
	}
}
