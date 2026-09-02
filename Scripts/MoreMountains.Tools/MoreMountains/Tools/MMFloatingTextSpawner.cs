using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.Tools;

public class MMFloatingTextSpawner : MMMonoBehaviour
{
	public enum PoolerModes
	{
		Simple = 0,
		Multiple = 1
	}

	public enum AlignmentModes
	{
		Fixed = 0,
		MatchInitialDirection = 1,
		MatchMovementDirection = 2
	}

	[MMInspectorGroup("General Settings", true, 10)]
	[Tooltip("the channel to listen for events on. this will have to be matched in the feedbacks trying to command this spawner")]
	public int Channel;

	[Tooltip("whether or not this spawner can spawn at this time")]
	public bool CanSpawn = true;

	[Tooltip("whether or not this spawner should spawn objects on unscaled time")]
	public bool UseUnscaledTime;

	[MMInspectorGroup("Pooler", true, 24)]
	[Tooltip("the selected pooler mode (single prefab or multiple ones)")]
	public PoolerModes PoolerMode;

	[Tooltip("the prefab to spawn (ignored if in multiple mode)")]
	public MMFloatingText PooledSimpleMMFloatingText;

	[Tooltip("the prefabs to spawn (ignored if in simple mode)")]
	public List<MMFloatingText> PooledMultipleMMFloatingText;

	[Tooltip("the amount of objects to pool to avoid having to instantiate them at runtime. Should be bigger than the max amount of texts you plan on having on screen at any given moment")]
	public int PoolSize = 20;

	[Tooltip("whether or not to nest the waiting pools")]
	public bool NestWaitingPool = true;

	[Tooltip("whether or not to mutualize the waiting pools")]
	public bool MutualizeWaitingPools = true;

	[Tooltip("whether or not the text pool can expand if the pool is empty")]
	public bool PoolCanExpand = true;

	[MMInspectorGroup("Spawn Settings", true, 14)]
	[Tooltip("the random min and max lifetime duration for the spawned texts (in seconds)")]
	[MMVector(new string[] { "Min", "Max" })]
	public Vector2 Lifetime = Vector2.one;

	[Header("Spawn Position Offset")]
	[Tooltip("the random min position at which to spawn the text, relative to its intended spawn position")]
	public Vector3 SpawnOffsetMin = Vector3.zero;

	[Tooltip("the random max position at which to spawn the text, relative to its intended spawn position")]
	public Vector3 SpawnOffsetMax = Vector3.zero;

	[MMInspectorGroup("Animate Position", true, 15)]
	[Header("Movement")]
	[Tooltip("whether or not to animate the movement of spawned texts")]
	public bool AnimateMovement = true;

	[Tooltip("whether or not to animate the X movement of spawned texts")]
	public bool AnimateX;

	[Tooltip("the value to which the x movement curve's zero should be remapped to")]
	[MMCondition("AnimateX", true)]
	public Vector2 RemapXZero = Vector2.zero;

	[Tooltip("the value to which the x movement curve's one should be remapped to")]
	[MMCondition("AnimateX", true)]
	public Vector2 RemapXOne = Vector2.one;

	[Tooltip("the curve on which to animate the x movement")]
	[MMCondition("AnimateX", true)]
	public AnimationCurve AnimateXCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	[Tooltip("whether or not to animate the Y movement of spawned texts")]
	public bool AnimateY = true;

	[Tooltip("the value to which the y movement curve's zero should be remapped to")]
	[MMCondition("AnimateY", true)]
	public Vector2 RemapYZero = Vector2.zero;

	[Tooltip("the value to which the y movement curve's one should be remapped to")]
	[MMCondition("AnimateY", true)]
	public Vector2 RemapYOne = new Vector2(5f, 5f);

	[Tooltip("the curve on which to animate the y movement")]
	[MMCondition("AnimateY", true)]
	public AnimationCurve AnimateYCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	[Tooltip("whether or not to animate the Z movement of spawned texts")]
	public bool AnimateZ;

	[Tooltip("the value to which the z movement curve's zero should be remapped to")]
	[MMCondition("AnimateZ", true)]
	public Vector2 RemapZZero = Vector2.zero;

	[Tooltip("the value to which the z movement curve's one should be remapped to")]
	[MMCondition("AnimateZ", true)]
	public Vector2 RemapZOne = Vector2.one;

	[Tooltip("the curve on which to animate the z movement")]
	[MMCondition("AnimateZ", true)]
	public AnimationCurve AnimateZCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	[MMInspectorGroup("Facing Directions", true, 16)]
	[Header("Alignment")]
	[Tooltip("the selected alignment mode (whether the spawned text should have a fixed alignment, orient to match the initial spawn direction, or its movement curve)")]
	public AlignmentModes AlignmentMode;

	[Tooltip("when in fixed mode, the direction in which to keep the spawned texts")]
	[MMEnumCondition("AlignmentMode", new int[] { 0 })]
	public Vector3 FixedAlignment = Vector3.up;

	[Header("Billboard")]
	[Tooltip("whether or not spawned texts should always face the camera")]
	public bool AlwaysFaceCamera;

	[Tooltip("whether or not this spawner should automatically grab the main camera on start")]
	[MMCondition("AlwaysFaceCamera", true)]
	public bool AutoGrabMainCameraOnStart = true;

	[Tooltip("if not in auto grab mode, the camera to use for billboards")]
	[MMCondition("AlwaysFaceCamera", true)]
	public Camera TargetCamera;

	[MMInspectorGroup("Animate Scale", true, 46)]
	[Tooltip("whether or not to animate the scale of spawned texts")]
	public bool AnimateScale = true;

	[Tooltip("the value to which the scale curve's zero should be remapped to")]
	[MMCondition("AnimateScale", true)]
	public Vector2 RemapScaleZero = Vector2.zero;

	[Tooltip("the value to which the scale curve's one should be remapped to")]
	[MMCondition("AnimateScale", true)]
	public Vector2 RemapScaleOne = Vector2.one;

	[Tooltip("the curve on which to animate the scale")]
	[MMCondition("AnimateScale", true)]
	public AnimationCurve AnimateScaleCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.15f, 1f), new Keyframe(0.85f, 1f), new Keyframe(1f, 0f));

	[MMInspectorGroup("Animate Color", true, 55)]
	[Tooltip("whether or not to animate the spawned text's color over time")]
	public bool AnimateColor;

	[Tooltip("the gradient over which to animate the spawned text's color over time")]
	[GradientUsage(true)]
	public Gradient AnimateColorGradient = new Gradient();

	[MMInspectorGroup("Animate Opacity", true, 45)]
	[Tooltip("whether or not to animate the opacity of the spawned texts")]
	public bool AnimateOpacity = true;

	[Tooltip("the value to which the opacity curve's zero should be remapped to")]
	[MMCondition("AnimateOpacity", true)]
	public Vector2 RemapOpacityZero = Vector2.zero;

	[Tooltip("the value to which the opacity curve's one should be remapped to")]
	[MMCondition("AnimateOpacity", true)]
	public Vector2 RemapOpacityOne = Vector2.one;

	[Tooltip("the curve on which to animate the opacity")]
	[MMCondition("AnimateOpacity", true)]
	public AnimationCurve AnimateOpacityCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.2f, 1f), new Keyframe(0.8f, 1f), new Keyframe(1f, 0f));

	[MMInspectorGroup("Intensity Multipliers", true, 45)]
	[Tooltip("whether or not the intensity multiplier should impact lifetime")]
	public bool IntensityImpactsLifetime;

	[Tooltip("when getting an intensity multiplier, the value by which to multiply the lifetime")]
	[MMCondition("IntensityImpactsLifetime", true)]
	public float IntensityLifetimeMultiplier = 1f;

	[Tooltip("whether or not the intensity multiplier should impact movement")]
	public bool IntensityImpactsMovement;

	[Tooltip("when getting an intensity multiplier, the value by which to multiply the movement values")]
	[MMCondition("IntensityImpactsMovement", true)]
	public float IntensityMovementMultiplier = 1f;

	[Tooltip("whether or not the intensity multiplier should impact scale")]
	public bool IntensityImpactsScale;

	[Tooltip("when getting an intensity multiplier, the value by which to multiply the scale values")]
	[MMCondition("IntensityImpactsScale", true)]
	public float IntensityScaleMultiplier = 1f;

	[MMInspectorGroup("Debug", true, 12)]
	[Tooltip("a random value to display when pressing the TestSpawnOne button")]
	public Vector2Int DebugRandomValue = new Vector2Int(100, 500);

	[Tooltip("the min and max bounds within which to pick a value to output when pressing the TestSpawnMany button")]
	[MMVector(new string[] { "Min", "Max" })]
	public Vector2 DebugInterval = new Vector2(0.3f, 0.5f);

	[Tooltip("a button used to test the spawn of one text")]
	[MMInspectorButton("TestSpawnOne")]
	public bool TestSpawnOneBtn;

	[Tooltip("a button used to start/stop the spawn of texts at regular intervals")]
	[MMInspectorButton("TestSpawnMany")]
	public bool TestSpawnManyBtn;

	protected MMObjectPooler _pooler;

	protected MMFloatingText _floatingText;

	protected Coroutine _testSpawnCoroutine;

	protected float _lifetime;

	protected float _speed;

	protected Vector3 _spawnOffset;

	protected Vector3 _direction;

	protected Gradient _colorGradient;

	protected bool _animateColor;

	protected virtual void Start()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		InstantiateObjectPool();
		GrabMainCamera();
	}

	protected virtual void InstantiateObjectPool()
	{
		if (_pooler == null)
		{
			if (PoolerMode == PoolerModes.Simple)
			{
				InstantiateSimplePool();
			}
			else
			{
				InstantiateMultiplePool();
			}
		}
	}

	protected virtual void InstantiateSimplePool()
	{
		if (PooledSimpleMMFloatingText == null)
		{
			Debug.LogError(base.name + " : no PooledSimpleMMFloatingText prefab has been set.");
			return;
		}
		GameObject obj = new GameObject();
		SceneManager.MoveGameObjectToScene(obj, base.gameObject.scene);
		obj.name = PooledSimpleMMFloatingText.name + "_Pooler";
		obj.transform.SetParent(base.transform);
		MMSimpleObjectPooler mMSimpleObjectPooler = obj.AddComponent<MMSimpleObjectPooler>();
		mMSimpleObjectPooler.PoolSize = PoolSize;
		mMSimpleObjectPooler.GameObjectToPool = PooledSimpleMMFloatingText.gameObject;
		mMSimpleObjectPooler.NestWaitingPool = NestWaitingPool;
		mMSimpleObjectPooler.MutualizeWaitingPools = MutualizeWaitingPools;
		mMSimpleObjectPooler.PoolCanExpand = PoolCanExpand;
		mMSimpleObjectPooler.FillObjectPool();
		_pooler = mMSimpleObjectPooler;
	}

	protected virtual void InstantiateMultiplePool()
	{
		GameObject obj = new GameObject();
		SceneManager.MoveGameObjectToScene(obj, base.gameObject.scene);
		obj.name = base.name + "_Pooler";
		obj.transform.SetParent(base.transform);
		MMMultipleObjectPooler mMMultipleObjectPooler = obj.AddComponent<MMMultipleObjectPooler>();
		mMMultipleObjectPooler.Pool = new List<MMMultipleObjectPoolerObject>();
		foreach (MMFloatingText item in PooledMultipleMMFloatingText)
		{
			MMMultipleObjectPoolerObject mMMultipleObjectPoolerObject = new MMMultipleObjectPoolerObject();
			mMMultipleObjectPoolerObject.GameObjectToPool = item.gameObject;
			mMMultipleObjectPoolerObject.PoolCanExpand = PoolCanExpand;
			mMMultipleObjectPoolerObject.PoolSize = PoolSize;
			mMMultipleObjectPoolerObject.Enabled = true;
			mMMultipleObjectPooler.Pool.Add(mMMultipleObjectPoolerObject);
		}
		mMMultipleObjectPooler.NestWaitingPool = NestWaitingPool;
		mMMultipleObjectPooler.MutualizeWaitingPools = MutualizeWaitingPools;
		mMMultipleObjectPooler.FillObjectPool();
		_pooler = mMMultipleObjectPooler;
	}

	protected virtual void GrabMainCamera()
	{
		if (AutoGrabMainCameraOnStart)
		{
			TargetCamera = Camera.main;
		}
	}

	protected virtual void Spawn(string value, Vector3 position, Vector3 direction, float intensity = 1f, bool forceLifetime = false, float lifetime = 1f, bool forceColor = false, Gradient animateColorGradient = null)
	{
		if (CanSpawn)
		{
			_direction = ((direction != Vector3.zero) ? (direction + base.transform.up) : base.transform.up);
			base.transform.position = position;
			GameObject pooledGameObject = _pooler.GetPooledGameObject();
			float num = (IntensityImpactsLifetime ? (intensity * IntensityLifetimeMultiplier) : 1f);
			float num2 = (IntensityImpactsMovement ? (intensity * IntensityMovementMultiplier) : 1f);
			float num3 = (IntensityImpactsScale ? (intensity * IntensityScaleMultiplier) : 1f);
			_lifetime = Random.Range(Lifetime.x, Lifetime.y) * num;
			_spawnOffset = MMMaths.RandomVector3(SpawnOffsetMin, SpawnOffsetMax);
			_animateColor = AnimateColor;
			_colorGradient = AnimateColorGradient;
			float remapXZero = Random.Range(RemapXZero.x, RemapXZero.y);
			float remapXOne = Random.Range(RemapXOne.x, RemapXOne.y) * num2;
			float remapYZero = Random.Range(RemapYZero.x, RemapYZero.y);
			float remapYOne = Random.Range(RemapYOne.x, RemapYOne.y) * num2;
			float remapZZero = Random.Range(RemapZZero.x, RemapZZero.y);
			float remapZOne = Random.Range(RemapZOne.x, RemapZOne.y) * num2;
			float remapOpacityZero = Random.Range(RemapOpacityZero.x, RemapOpacityZero.y);
			float remapOpacityOne = Random.Range(RemapOpacityOne.x, RemapOpacityOne.y);
			float remapScaleZero = Random.Range(RemapScaleZero.x, RemapOpacityZero.y);
			float remapScaleOne = Random.Range(RemapScaleOne.x, RemapScaleOne.y) * num3;
			if (forceLifetime)
			{
				_lifetime = lifetime;
			}
			if (forceColor)
			{
				_animateColor = true;
				_colorGradient = animateColorGradient;
			}
			if (!(pooledGameObject == null))
			{
				pooledGameObject.gameObject.SetActive(value: true);
				pooledGameObject.gameObject.MMGetComponentNoAlloc<MMPoolableObject>().TriggerOnSpawnComplete();
				pooledGameObject.transform.position = base.transform.position + _spawnOffset;
				_floatingText = pooledGameObject.MMGetComponentNoAlloc<MMFloatingText>();
				_floatingText.SetUseUnscaledTime(UseUnscaledTime, resetStartedAt: true);
				_floatingText.ResetPosition();
				_floatingText.SetProperties(value, _lifetime, _direction, AnimateMovement, AlignmentMode, FixedAlignment, AlwaysFaceCamera, TargetCamera, AnimateX, AnimateXCurve, remapXZero, remapXOne, AnimateY, AnimateYCurve, remapYZero, remapYOne, AnimateZ, AnimateZCurve, remapZZero, remapZOne, AnimateOpacity, AnimateOpacityCurve, remapOpacityZero, remapOpacityOne, AnimateScale, AnimateScaleCurve, remapScaleZero, remapScaleOne, _animateColor, _colorGradient);
			}
		}
	}

	public virtual void OnMMFloatingTextSpawnEvent(int channel, Vector3 spawnPosition, string value, Vector3 direction, float intensity, bool forceLifetime = false, float lifetime = 1f, bool forceColor = false, Gradient animateColorGradient = null, bool useUnscaledTime = false)
	{
		if (channel == Channel)
		{
			UseUnscaledTime = useUnscaledTime;
			Spawn(value, spawnPosition, direction, intensity, forceLifetime, lifetime, forceColor, animateColorGradient);
		}
	}

	protected virtual void OnEnable()
	{
		MMFloatingTextSpawnEvent.Register(OnMMFloatingTextSpawnEvent);
	}

	protected virtual void OnDisable()
	{
		MMFloatingTextSpawnEvent.Unregister(OnMMFloatingTextSpawnEvent);
	}

	protected virtual void TestSpawnOne()
	{
		string value = Random.Range(DebugRandomValue.x, DebugRandomValue.y).ToString();
		Spawn(value, base.transform.position, Vector3.zero);
	}

	protected virtual void TestSpawnMany()
	{
		if (_testSpawnCoroutine == null)
		{
			_testSpawnCoroutine = StartCoroutine(TestSpawnManyCo());
			return;
		}
		StopCoroutine(_testSpawnCoroutine);
		_testSpawnCoroutine = null;
	}

	protected virtual IEnumerator TestSpawnManyCo()
	{
		float lastSpawnAt = Time.time;
		float interval = Random.Range(DebugInterval.x, DebugInterval.y);
		while (true)
		{
			if (Time.time - lastSpawnAt > interval)
			{
				TestSpawnOne();
				lastSpawnAt = Time.time;
				interval = Random.Range(DebugInterval.x, DebugInterval.y);
			}
			yield return null;
		}
	}
}
