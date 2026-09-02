using System;
using UnityEngine;

namespace DestroyIt;

[RequireComponent(typeof(Destructible))]
public class WhenDestroyedResetTree : MonoBehaviour
{
	[Tooltip("How many seconds to wait before resetting the tree.")]
	public float resetAfterSeconds = 30f;

	private Destructible _destObj;

	private void Start()
	{
		_destObj = base.gameObject.GetComponent<Destructible>();
		if (_destObj != null)
		{
			_destObj.DestroyedEvent += OnDestroyed;
		}
	}

	private void OnDisable()
	{
		if (!(_destObj == null))
		{
			_destObj.DestroyedEvent -= OnDestroyed;
		}
	}

	private void OnDestroyed()
	{
		Debug.Log($"{_destObj.name} was destroyed at world coordinates: {_destObj.transform.position}");
		TerrainTree terrainTree = Terrain.activeTerrain.ClosestTreeToPoint(base.transform.position);
		TreeReset item = new TreeReset
		{
			prototypeIndex = terrainTree.TreeInstance.prototypeIndex,
			position = terrainTree.TreeInstance.position,
			color = terrainTree.TreeInstance.color,
			heightScale = terrainTree.TreeInstance.heightScale,
			widthScale = terrainTree.TreeInstance.widthScale,
			resetTime = DateTime.Now.AddSeconds(resetAfterSeconds)
		};
		TreeManager.Instance.treesToReset.Add(item);
	}
}
