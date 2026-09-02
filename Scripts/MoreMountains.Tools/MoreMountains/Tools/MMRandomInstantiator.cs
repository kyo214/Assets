using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.Tools;

public class MMRandomInstantiator : MonoBehaviour
{
	public enum StartModes
	{
		Awake = 0,
		Start = 1,
		None = 2
	}

	[Header("Random instantiation")]
	public StartModes StartMode;

	public string InstantiatedObjectName = "RandomInstantiated";

	public bool ParentInstantiatedToThisObject = true;

	public bool DestroyPreviouslyInstantiatedObject = true;

	public List<GameObject> RandomPool;

	[Header("Test")]
	[MMInspectorButton("InstantiateRandomObject")]
	public bool InstantiateButton;

	protected GameObject _instantiatedGameObject;

	protected virtual void Awake()
	{
		if (StartMode == StartModes.Awake)
		{
			InstantiateRandomObject();
		}
	}

	protected virtual void Start()
	{
		if (StartMode == StartModes.Start)
		{
			InstantiateRandomObject();
		}
	}

	public virtual void InstantiateRandomObject()
	{
		if (RandomPool.Count != 0)
		{
			if (DestroyPreviouslyInstantiatedObject && _instantiatedGameObject != null)
			{
				Object.DestroyImmediate(_instantiatedGameObject);
			}
			int index = Random.Range(0, RandomPool.Count);
			_instantiatedGameObject = Object.Instantiate(RandomPool[index], base.transform.position, base.transform.rotation);
			SceneManager.MoveGameObjectToScene(_instantiatedGameObject, base.gameObject.scene);
			_instantiatedGameObject.name = InstantiatedObjectName;
			if (ParentInstantiatedToThisObject)
			{
				_instantiatedGameObject.transform.SetParent(base.transform);
			}
		}
	}
}
