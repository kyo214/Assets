using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.Tools;

public class MMSpawnAroundTester : MonoBehaviour
{
	public GameObject ObjectToInstantiate;

	public MMSpawnAroundProperties SpawnProperties;

	[Header("Debug")]
	public int DebugQuantity = 10000;

	[MMInspectorButton("DebugSpawn")]
	public bool DebugSpawnButton;

	[Header("Gizmos")]
	public bool DrawGizmos;

	public int GizmosQuantity = 1000;

	public float GizmosSize = 1f;

	protected GameObject _gameObject;

	public virtual void DebugSpawn()
	{
		for (int i = 0; i < DebugQuantity; i++)
		{
			Spawn();
		}
	}

	public virtual void Spawn()
	{
		_gameObject = Object.Instantiate(ObjectToInstantiate);
		SceneManager.MoveGameObjectToScene(_gameObject, base.gameObject.scene);
		MMSpawnAround.ApplySpawnAroundProperties(_gameObject, SpawnProperties, base.transform.position);
	}

	protected virtual void OnDrawGizmos()
	{
		if (DrawGizmos)
		{
			MMSpawnAround.DrawGizmos(SpawnProperties, base.transform.position, GizmosQuantity, GizmosSize, Color.gray);
		}
	}
}
