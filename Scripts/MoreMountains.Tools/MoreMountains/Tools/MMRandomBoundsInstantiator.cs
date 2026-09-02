using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.Tools;

[RequireComponent(typeof(Collider))]
public class MMRandomBoundsInstantiator : MonoBehaviour
{
	public enum StartModes
	{
		Awake = 0,
		Start = 1,
		None = 2
	}

	public enum ScaleModes
	{
		Uniform = 0,
		Vector3 = 1
	}

	[Header("Random instantiation")]
	public StartModes StartMode;

	public string InstantiatedObjectName = "RandomInstantiated";

	public bool ParentInstantiatedToThisObject = true;

	public bool DestroyPreviouslyInstantiatedObjects = true;

	[Header("Spawn")]
	public List<GameObject> RandomPool;

	[MMVector(new string[] { "Min", "Max" })]
	public Vector2Int Quantity = new Vector2Int(1, 1);

	[Header("Scale")]
	public ScaleModes ScaleMode;

	[MMEnumCondition("ScaleMode", new int[] { 0 })]
	public float MinScale = 1f;

	[MMEnumCondition("ScaleMode", new int[] { 0 })]
	public float MaxScale = 1f;

	[MMEnumCondition("ScaleMode", new int[] { 1 })]
	public Vector3 MinVectorScale = Vector3.one;

	[MMEnumCondition("ScaleMode", new int[] { 1 })]
	public Vector3 MaxVectorScale = Vector3.one;

	[Header("Test")]
	[MMInspectorButton("Instantiate")]
	public bool InstantiateButton;

	protected Collider _collider;

	protected List<GameObject> _instantiatedGameObjects;

	protected Vector3 _newScale = Vector3.zero;

	protected virtual void Awake()
	{
		_collider = base.gameObject.GetComponent<Collider>();
		if (StartMode == StartModes.Awake)
		{
			Instantiate();
		}
	}

	protected virtual void Start()
	{
		if (StartMode == StartModes.Start)
		{
			Instantiate();
		}
	}

	protected virtual void Instantiate()
	{
		if (_instantiatedGameObjects == null)
		{
			_instantiatedGameObjects = new List<GameObject>();
		}
		if (DestroyPreviouslyInstantiatedObjects)
		{
			foreach (GameObject instantiatedGameObject in _instantiatedGameObjects)
			{
				Object.DestroyImmediate(instantiatedGameObject);
			}
			_instantiatedGameObjects.Clear();
		}
		int num = Random.Range(Quantity.x, Quantity.y);
		for (int i = 0; i < num; i++)
		{
			InstantiateRandomObject();
		}
	}

	public virtual void InstantiateRandomObject()
	{
		if (RandomPool.Count != 0)
		{
			int index = Random.Range(0, RandomPool.Count);
			GameObject gameObject = Object.Instantiate(RandomPool[index], base.transform.position, base.transform.rotation);
			SceneManager.MoveGameObjectToScene(gameObject.gameObject, base.gameObject.scene);
			gameObject.transform.position = MMBoundsExtensions.MMRandomPointInBounds(_collider.bounds);
			gameObject.transform.position = _collider.ClosestPoint(gameObject.transform.position);
			gameObject.name = InstantiatedObjectName;
			if (ParentInstantiatedToThisObject)
			{
				gameObject.transform.SetParent(base.transform);
			}
			switch (ScaleMode)
			{
			case ScaleModes.Uniform:
			{
				float num = Random.Range(MinScale, MaxScale);
				gameObject.transform.localScale = Vector3.one * num;
				break;
			}
			case ScaleModes.Vector3:
				_newScale = MMMaths.RandomVector3(MinVectorScale, MaxVectorScale);
				gameObject.transform.localScale = _newScale;
				break;
			}
			_instantiatedGameObjects.Add(gameObject);
		}
	}
}
