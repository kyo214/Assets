using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback allows you to instantiate the object specified in its inspector, at the feedback's position (plus an optional offset). You can also optionally (and automatically) create an object pool at initialization to save on performance. In that case you'll need to specify a pool size (usually the maximum amount of these instantiated objects you plan on having in your scene at each given time).")]
[FeedbackPath("GameObject/Instantiate Object")]
public class MMFeedbackInstantiateObject : MMFeedback
{
	public enum PositionModes
	{
		FeedbackPosition = 0,
		Transform = 1,
		WorldPosition = 2,
		Script = 3
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Instantiate Object")]
	[Tooltip("the object to instantiate")]
	[FormerlySerializedAs("VfxToInstantiate")]
	public GameObject GameObjectToInstantiate;

	[Header("Position")]
	[Tooltip("the chosen way to position the object")]
	public PositionModes PositionMode;

	[Tooltip("the chosen way to position the object")]
	public bool AlsoApplyRotation;

	[Tooltip("the chosen way to position the object")]
	public bool AlsoApplyScale;

	[Tooltip("the transform at which to instantiate the object")]
	[MMFEnumCondition("PositionMode", new int[] { 1 })]
	public Transform TargetTransform;

	[Tooltip("the transform at which to instantiate the object")]
	[MMFEnumCondition("PositionMode", new int[] { 2 })]
	public Vector3 TargetPosition;

	[Tooltip("the position offset at which to instantiate the object")]
	[FormerlySerializedAs("VfxPositionOffset")]
	public Vector3 PositionOffset;

	[Header("Object Pool")]
	[Tooltip("whether or not we should create automatically an object pool for this object")]
	[FormerlySerializedAs("VfxCreateObjectPool")]
	public bool CreateObjectPool;

	[Tooltip("the initial and planned size of this object pool")]
	[MMFCondition("CreateObjectPool", true)]
	[FormerlySerializedAs("VfxObjectPoolSize")]
	public int ObjectPoolSize = 5;

	[Tooltip("whether or not to create a new pool even if one already exists for that same prefab")]
	[MMFCondition("CreateObjectPool", true)]
	public bool MutualizePools;

	protected MMMiniObjectPooler _objectPooler;

	protected GameObject _newGameObject;

	protected bool _poolCreatedOrFound;

	protected override void CustomInitialization(GameObject owner)
	{
		base.CustomInitialization(owner);
		if (Active && CreateObjectPool && !_poolCreatedOrFound)
		{
			if (_objectPooler != null)
			{
				_objectPooler.DestroyObjectPool();
				Object.Destroy(_objectPooler.gameObject);
			}
			GameObject gameObject = new GameObject();
			gameObject.name = base.name + "_ObjectPooler";
			_objectPooler = gameObject.AddComponent<MMMiniObjectPooler>();
			_objectPooler.GameObjectToPool = GameObjectToInstantiate;
			_objectPooler.PoolSize = ObjectPoolSize;
			_objectPooler.transform.SetParent(base.transform);
			_objectPooler.MutualizeWaitingPools = MutualizePools;
			_objectPooler.FillObjectPool();
			if (owner != null && gameObject.transform.parent == null)
			{
				SceneManager.MoveGameObjectToScene(gameObject, owner.scene);
			}
			_poolCreatedOrFound = true;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized || GameObjectToInstantiate == null)
		{
			return;
		}
		if (_objectPooler != null)
		{
			_newGameObject = _objectPooler.GetPooledGameObject();
			if (_newGameObject != null)
			{
				PositionObject(position);
				_newGameObject.SetActive(value: true);
			}
		}
		else
		{
			_newGameObject = Object.Instantiate(GameObjectToInstantiate);
			if (_newGameObject != null)
			{
				SceneManager.MoveGameObjectToScene(_newGameObject, base.gameObject.scene);
				PositionObject(position);
			}
		}
	}

	protected virtual void PositionObject(Vector3 position)
	{
		_newGameObject.transform.position = GetPosition(position);
		if (AlsoApplyRotation)
		{
			_newGameObject.transform.rotation = GetRotation();
		}
		if (AlsoApplyScale)
		{
			_newGameObject.transform.localScale = GetScale();
		}
	}

	protected virtual Vector3 GetPosition(Vector3 position)
	{
		return PositionMode switch
		{
			PositionModes.FeedbackPosition => base.transform.position + PositionOffset, 
			PositionModes.Transform => TargetTransform.position + PositionOffset, 
			PositionModes.WorldPosition => TargetPosition + PositionOffset, 
			PositionModes.Script => position + PositionOffset, 
			_ => position + PositionOffset, 
		};
	}

	protected virtual Quaternion GetRotation()
	{
		return PositionMode switch
		{
			PositionModes.FeedbackPosition => base.transform.rotation, 
			PositionModes.Transform => TargetTransform.rotation, 
			PositionModes.WorldPosition => Quaternion.identity, 
			PositionModes.Script => base.transform.rotation, 
			_ => base.transform.rotation, 
		};
	}

	protected virtual Vector3 GetScale()
	{
		return PositionMode switch
		{
			PositionModes.FeedbackPosition => base.transform.localScale, 
			PositionModes.Transform => TargetTransform.localScale, 
			PositionModes.WorldPosition => base.transform.localScale, 
			PositionModes.Script => base.transform.localScale, 
			_ => base.transform.localScale, 
		};
	}
}
