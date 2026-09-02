using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGCalcLoaderString
{
	public void Load(BGCalcGraph graph, string json)
	{
		BGCalcGraphModel bGCalcGraphModel = JsonUtility.FromJson<BGCalcGraphModel>(json);
		bGCalcGraphModel.ToGraph(graph);
	}
}
