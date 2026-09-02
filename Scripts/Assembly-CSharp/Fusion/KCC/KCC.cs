using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace Fusion.KCC;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[OrderBefore(new Type[] { typeof(HitboxManager) })]
[NetworkBehaviourWeaved(0)]
public sealed class KCC : NetworkAreaOfInterestBehaviour, IBeforeAllTicks, IAfterTick
{
	private const int CACHE_SIZE = 64;

	private const int HISTORY_SIZE = 60;

	public Func<KCC, Collider, bool> ResolveCollision;

	[SerializeField]
	private KCCSettings _settings = new KCCSettings();

	private EKCCDriver _driver;

	private Transform _transform;

	private Rigidbody _rigidbody;

	private bool _isFixed;

	private bool _isSpawned;

	private bool _isInitialized;

	private bool _hasManualUpdate;

	private bool _hasInputAuthority;

	private bool _hasStateAuthority;

	private KCCUpdater _updater;

	private KCCDebug _debug = new KCCDebug();

	private KCCStatistics _statistics = new KCCStatistics();

	private KCCCollider _collider = new KCCCollider();

	private KCCData _fixedData = new KCCData();

	private KCCData _renderData = new KCCData();

	private KCCData[] _historyData = new KCCData[60];

	private KCCTransientData _transientData = new KCCTransientData();

	private KCCSettings _defaultSettings = new KCCSettings();

	private KCCSettings _runtimeSettings = new KCCSettings();

	private KCCOverlapInfo _extendedOverlapInfo = new KCCOverlapInfo(64);

	private KCCOverlapInfo _sharedOverlapInfo = new KCCOverlapInfo(64);

	private KCCOverlapInfo _trackOverlapInfo = new KCCOverlapInfo(64);

	private KCCRaycastInfo _raycastInfo = new KCCRaycastInfo(64);

	private List<Collider> _childColliders = new List<Collider>();

	private RaycastHit[] _raycastHits = new RaycastHit[64];

	private Collider[] _hitColliders = new Collider[64];

	private Collider[] _addColliders = new Collider[64];

	private Collider[] _removeColliders = new Collider[64];

	private KCCCollision[] _removeCollisions = new KCCCollision[64];

	private KCCResolver _resolver = new KCCResolver(64);

	private EKCCStage _activeStage;

	private EKCCFeatures _activeFeatures;

	private IKCCProcessor[] _stageProcessors = new IKCCProcessor[64];

	private int _stageProcessorIndex;

	private IKCCProcessor[] _cachedProcessors = new IKCCProcessor[64];

	private EKCCStages[] _cachedProcessorStages = new EKCCStages[64];

	private int _cachedProcessorCount;

	private List<IKCCProcessor> _localProcessors = new List<IKCCProcessor>();

	private ReadOnlyCollection<IKCCProcessor> _localROProcessors;

	private float _lastRenderTime;

	private Vector3 _lastRenderPosition;

	private int _lastRenderInitialization;

	private int _lastFixedInitialization;

	private Vector3 _lastAntiJitterPosition;

	private Vector3 _predictionError;

	private static readonly Action<IKCCProcessor, KCC, KCCData> _setInputProperties = (IKCCProcessor processor, KCC kcc, KCCData data) =>
	{
		processor.SetInputProperties(kcc, data);
	};

	private static readonly Action<IKCCProcessor, KCC, KCCData> _setDynamicVelocity = (IKCCProcessor processor, KCC kcc, KCCData data) =>
	{
		processor.SetDynamicVelocity(kcc, data);
	};

	private static readonly Action<IKCCProcessor, KCC, KCCData> _setKinematicDirection = (IKCCProcessor processor, KCC kcc, KCCData data) =>
	{
		processor.SetKinematicDirection(kcc, data);
	};

	private static readonly Action<IKCCProcessor, KCC, KCCData> _setKinematicTangent = (IKCCProcessor processor, KCC kcc, KCCData data) =>
	{
		processor.SetKinematicTangent(kcc, data);
	};

	private static readonly Action<IKCCProcessor, KCC, KCCData> _setKinematicSpeed = (IKCCProcessor processor, KCC kcc, KCCData data) =>
	{
		processor.SetKinematicSpeed(kcc, data);
	};

	private static readonly Action<IKCCProcessor, KCC, KCCData> _setKinematicVelocity = (IKCCProcessor processor, KCC kcc, KCCData data) =>
	{
		processor.SetKinematicVelocity(kcc, data);
	};

	private static readonly Action<IKCCProcessor, KCC, KCCData> _processPhysicsQuery = (IKCCProcessor processor, KCC kcc, KCCData data) =>
	{
		processor.ProcessPhysicsQuery(kcc, data);
	};

	private static readonly Action<IKCCProcessor, KCC, KCCData> _onStay = (IKCCProcessor processor, KCC kcc, KCCData data) =>
	{
		processor.OnStay(kcc, data);
	};

	private static readonly Action<IKCCProcessor, KCC, KCCData> _onInterpolate = (IKCCProcessor processor, KCC kcc, KCCData data) =>
	{
		processor.OnInterpolate(kcc, data);
	};

	private static readonly Action<IKCCProcessor, KCC, KCCData, object> _processUserLogic = (IKCCProcessor processor, KCC kcc, KCCData data, object userData) =>
	{
		processor.ProcessUserLogic(kcc, data, userData);
	};

	private KCCNetworkContext _networkContext;

	private IKCCNetworkProperty[] _networkProperties;

	private KCCNetworkProperties _defaultProperties;

	private static float _defaultPositionReadAccuracy = float.NaN;

	private static Changed<KCC> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<KCC> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<KCC> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	public EKCCDriver Driver => _driver;

	public KCCDebug Debug => _debug;

	public KCCStatistics Statistics => _statistics;

	public KCCData Data
	{
		get
		{
			if (!IsInFixedUpdate)
			{
				return _renderData;
			}
			return _fixedData;
		}
	}

	public KCCData FixedData => _fixedData;

	public KCCData RenderData => _renderData;

	public KCCTransientData TransientData => _transientData;

	public KCCSettings Settings => _settings;

	public CapsuleCollider Collider => _collider.Collider;

	public EKCCStage ActiveStage => _activeStage;

	public EKCCFeatures ActiveFeatures => _activeFeatures;

	public bool HasManualUpdate => _hasManualUpdate;

	public new bool HasInputAuthority => _hasInputAuthority;

	public new bool HasStateAuthority => _hasStateAuthority;

	public bool HasAnyAuthority
	{
		get
		{
			if (!_hasInputAuthority)
			{
				return _hasStateAuthority;
			}
			return true;
		}
	}

	public new bool IsProxy
	{
		get
		{
			if (!_hasInputAuthority)
			{
				return !_hasStateAuthority;
			}
			return false;
		}
	}

	public bool IsInFixedUpdate
	{
		get
		{
			if (!_isFixed && (_driver != EKCCDriver.Fusion || Runner.Stage == (SimulationStages)0))
			{
				if (_driver == EKCCDriver.Unity)
				{
					return Time.inFixedTimeStep;
				}
				return false;
			}
			return true;
		}
	}

	public Vector3 PredictionError => _predictionError;

	public ReadOnlyCollection<IKCCProcessor> LocalProcessors => _localROProcessors;

	public override int? DynamicWordCount => GetNetworkDataWordCount();

	public override bool InvokeOnChangedForInitialNonZeroValues => false;

	public override int PositionWordOffset => 0;

	public event Action<KCC> OnInitialize;

	public event Action<KCC> OnDeinitialize;

	public event Action<KCC, KCCCollision> OnCollisionEnter;

	public event Action<KCC, KCCCollision> OnCollisionExit;

	public void SetInputDirection(Vector3 direction)
	{
		if (HasAnyAuthority)
		{
			if (direction.sqrMagnitude > 1f)
			{
				direction.Normalize();
			}
			_renderData.InputDirection = direction;
			if (IsInFixedUpdate)
			{
				_fixedData.InputDirection = direction;
			}
		}
	}

	public void AddLookRotation(float pitchDelta, float yawDelta)
	{
		if (!HasAnyAuthority)
		{
			return;
		}
		KCCData kCCData = _renderData;
		if (pitchDelta != 0f)
		{
			kCCData.LookPitch = Mathf.Clamp(kCCData.LookPitch + pitchDelta, -90f, 90f);
		}
		if (yawDelta != 0f)
		{
			float num;
			for (num = kCCData.LookYaw + yawDelta; num > 180f; num -= 360f)
			{
			}
			for (; num < -180f; num += 360f)
			{
			}
			kCCData.LookYaw = num;
		}
		if (IsInFixedUpdate)
		{
			kCCData = _fixedData;
			if (pitchDelta != 0f)
			{
				kCCData.LookPitch = Mathf.Clamp(kCCData.LookPitch + pitchDelta, -90f, 90f);
			}
			if (yawDelta != 0f)
			{
				float num2;
				for (num2 = kCCData.LookYaw + yawDelta; num2 > 180f; num2 -= 360f)
				{
				}
				for (; num2 < -180f; num2 += 360f)
				{
				}
				kCCData.LookYaw = num2;
			}
		}
		SynchronizeTransform(kCCData, synchronizePosition: false, synchronizeRotation: true, useAntiJitter: false);
	}

	public void AddLookRotation(Vector2 lookRotationDelta)
	{
		AddLookRotation(lookRotationDelta.x, lookRotationDelta.y);
	}

	public void SetLookRotation(float pitch, float yaw)
	{
		if (HasAnyAuthority)
		{
			KCCUtility.ClampLookRotationAngles(ref pitch, ref yaw);
			KCCData kCCData = _renderData;
			kCCData.LookPitch = pitch;
			kCCData.LookYaw = yaw;
			if (IsInFixedUpdate)
			{
				kCCData = _fixedData;
				kCCData.LookPitch = pitch;
				kCCData.LookYaw = yaw;
			}
			SynchronizeTransform(kCCData, synchronizePosition: false, synchronizeRotation: true, useAntiJitter: false);
		}
	}

	public void SetLookRotation(Vector2 lookRotation)
	{
		SetLookRotation(lookRotation.x, lookRotation.y);
	}

	public void SetLookRotation(Quaternion lookRotation, bool preservePitch = false, bool preserveYaw = false)
	{
		if (!HasAnyAuthority)
		{
			return;
		}
		KCCData kCCData = _renderData;
		KCCUtility.GetLookRotationAngles(lookRotation, out var pitch, out var yaw);
		if (!preservePitch)
		{
			kCCData.LookPitch = pitch;
		}
		if (!preserveYaw)
		{
			kCCData.LookYaw = yaw;
		}
		if (IsInFixedUpdate)
		{
			kCCData = _fixedData;
			if (!preservePitch)
			{
				kCCData.LookPitch = pitch;
			}
			if (!preserveYaw)
			{
				kCCData.LookYaw = yaw;
			}
		}
		SynchronizeTransform(kCCData, synchronizePosition: false, synchronizeRotation: true, useAntiJitter: false);
	}

	public void Jump(Vector3 impulse)
	{
		if (HasAnyAuthority)
		{
			_renderData.JumpImpulse += impulse;
			if (IsInFixedUpdate)
			{
				_fixedData.JumpImpulse += impulse;
			}
		}
	}

	public void AddExternalVelocity(Vector3 velocity)
	{
		if (HasAnyAuthority)
		{
			_renderData.ExternalVelocity += velocity;
			if (IsInFixedUpdate)
			{
				_fixedData.ExternalVelocity += velocity;
			}
		}
	}

	public void SetExternalVelocity(Vector3 velocity)
	{
		if (HasAnyAuthority)
		{
			_renderData.ExternalVelocity = velocity;
			if (IsInFixedUpdate)
			{
				_fixedData.ExternalVelocity = velocity;
			}
			_transientData.ExternalVelocity = default;
		}
	}

	public void AddExternalAcceleration(Vector3 acceleration)
	{
		if (HasAnyAuthority)
		{
			_renderData.ExternalAcceleration += acceleration;
			if (IsInFixedUpdate)
			{
				_fixedData.ExternalAcceleration += acceleration;
			}
		}
	}

	public void SetExternalAcceleration(Vector3 acceleration)
	{
		if (HasAnyAuthority)
		{
			_renderData.ExternalAcceleration = acceleration;
			if (IsInFixedUpdate)
			{
				_fixedData.ExternalAcceleration = acceleration;
			}
			_transientData.ExternalAcceleration = default;
		}
	}

	public void AddExternalImpulse(Vector3 impulse)
	{
		if (HasAnyAuthority)
		{
			_renderData.ExternalImpulse += impulse;
			if (IsInFixedUpdate)
			{
				_fixedData.ExternalImpulse += impulse;
			}
		}
	}

	public void SetExternalImpulse(Vector3 impulse)
	{
		if (HasAnyAuthority)
		{
			_renderData.ExternalImpulse = impulse;
			if (IsInFixedUpdate)
			{
				_fixedData.ExternalImpulse = impulse;
			}
			_transientData.ExternalImpulse = default;
		}
	}

	public void AddExternalForce(Vector3 force)
	{
		if (HasAnyAuthority)
		{
			_renderData.ExternalForce += force;
			if (IsInFixedUpdate)
			{
				_fixedData.ExternalForce += force;
			}
		}
	}

	public void SetExternalForce(Vector3 force)
	{
		if (HasAnyAuthority)
		{
			_renderData.ExternalForce = force;
			if (IsInFixedUpdate)
			{
				_fixedData.ExternalForce = force;
			}
			_transientData.ExternalForce = default;
		}
	}

	public void AddExternalDelta(Vector3 delta)
	{
		if (HasAnyAuthority)
		{
			_renderData.ExternalDelta += delta;
			if (IsInFixedUpdate)
			{
				_fixedData.ExternalDelta += delta;
			}
		}
	}

	public void SetExternalDelta(Vector3 delta)
	{
		if (HasAnyAuthority)
		{
			_renderData.ExternalDelta = delta;
			if (IsInFixedUpdate)
			{
				_fixedData.ExternalDelta = delta;
			}
		}
	}

	public void SetDynamicVelocity(Vector3 velocity)
	{
		if (HasAnyAuthority)
		{
			_renderData.DynamicVelocity = velocity;
			if (IsInFixedUpdate)
			{
				_fixedData.DynamicVelocity = velocity;
			}
		}
	}

	public void SetKinematicVelocity(Vector3 velocity)
	{
		if (HasAnyAuthority)
		{
			_renderData.KinematicVelocity = velocity;
			if (IsInFixedUpdate)
			{
				_fixedData.KinematicVelocity = velocity;
			}
		}
	}

	public void SetPosition(Vector3 position)
	{
		if (HasAnyAuthority)
		{
			KCCData kCCData = _renderData;
			kCCData.BasePosition = position;
			kCCData.DesiredPosition = position;
			kCCData.TargetPosition = position;
			kCCData.HasTeleported = true;
			kCCData.IsSteppingUp = false;
			kCCData.IsSnappingToGround = false;
			if (IsInFixedUpdate)
			{
				kCCData = _fixedData;
				kCCData.BasePosition = position;
				kCCData.DesiredPosition = position;
				kCCData.TargetPosition = position;
				kCCData.HasTeleported = true;
				kCCData.IsSteppingUp = false;
				kCCData.IsSnappingToGround = false;
			}
			SynchronizeTransform(kCCData, synchronizePosition: true, synchronizeRotation: false, useAntiJitter: false);
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void TeleportRPC(Vector3 position, float lookPitch, float lookYaw)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 2) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void Fusion.KCC.KCC::TeleportRPC(UnityEngine.Vector3,System.Single,System.Single)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 12;
				num += 4;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void Fusion.KCC.KCC::TeleportRPC(UnityEngine.Vector3,System.Single,System.Single)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 1), data);
					ReadWriteUtilsForWeaver.WriteVector3((int*)(data + num2), 999.99994f, position);
					num2 += 12;
					ReadWriteUtilsForWeaver.WriteFloat((int*)(data + num2), 999.99994f, lookPitch);
					num2 += 4;
					ReadWriteUtilsForWeaver.WriteFloat((int*)(data + num2), 999.99994f, lookYaw);
					num2 += 4;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		if (!_settings.AllowClientTeleports)
		{
			throw new InvalidOperationException();
		}
		KCCUtility.ClampLookRotationAngles(ref lookPitch, ref lookYaw);
		_renderData.BasePosition = position;
		_renderData.DesiredPosition = position;
		_renderData.TargetPosition = position;
		_renderData.HasTeleported = true;
		_renderData.IsSteppingUp = false;
		_renderData.IsSnappingToGround = false;
		_renderData.LookPitch = lookPitch;
		_renderData.LookYaw = lookYaw;
		_fixedData.BasePosition = position;
		_fixedData.DesiredPosition = position;
		_fixedData.TargetPosition = position;
		_fixedData.HasTeleported = true;
		_fixedData.IsSteppingUp = false;
		_fixedData.IsSnappingToGround = false;
		_fixedData.LookPitch = lookPitch;
		_fixedData.LookYaw = lookYaw;
		SynchronizeTransform(_fixedData, synchronizePosition: true, synchronizeRotation: true, useAntiJitter: false);
	}

	public void SynchronizeTransform(bool synchronizePosition, bool synchronizeRotation)
	{
		SynchronizeTransform(Data, synchronizePosition, synchronizeRotation, !IsInFixedUpdate && !IsProxy);
	}

	public void SetShape(EKCCShape shape, float radius = 0f, float height = 0f)
	{
		if (HasAnyAuthority)
		{
			_settings.Shape = shape;
			if (radius > 0f)
			{
				_settings.Radius = radius;
			}
			if (height > 0f)
			{
				_settings.Height = height;
			}
			RefreshCollider();
		}
	}

	public void SetTrigger(bool isTrigger)
	{
		if (HasAnyAuthority)
		{
			_settings.IsTrigger = isTrigger;
			RefreshCollider();
		}
	}

	public void SetRadius(float radius)
	{
		if (!(radius <= 0f) && HasAnyAuthority)
		{
			_settings.Radius = radius;
			RefreshCollider();
		}
	}

	public void SetHeight(float height)
	{
		if (!(height <= 0f) && HasAnyAuthority)
		{
			_settings.Height = height;
			RefreshCollider();
		}
	}

	public void SetMass(float mass)
	{
		if (!(mass <= 0f) && HasAnyAuthority)
		{
			_settings.Mass = mass;
		}
	}

	public void SetLayer(int layer)
	{
		if (HasAnyAuthority)
		{
			_settings.ColliderLayer = layer;
			RefreshCollider();
		}
	}

	public void SetLayerMask(LayerMask layerMask)
	{
		if (HasAnyAuthority)
		{
			_settings.CollisionLayerMask = layerMask;
		}
	}

	public void SetIgnoreCollider(Collider ignoreCollider, bool ignore)
	{
		if (ignoreCollider == null || !HasAnyAuthority)
		{
			return;
		}
		KCCData data = Data;
		if (ignore)
		{
			if (!data.Ignores.HasCollider(ignoreCollider))
			{
				NetworkObject componentNoAlloc = ignoreCollider.GetComponentNoAlloc<NetworkObject>();
				if (componentNoAlloc == null)
				{
					UnityEngine.Debug.LogError("Collider " + ignoreCollider.name + " doesn't have NetworkObject component! Ignoring.", ignoreCollider.gameObject);
				}
				else if ((object)ignoreCollider.gameObject.GetComponentNoAlloc<Collider>() != ignoreCollider)
				{
					UnityEngine.Debug.LogError("Object " + ignoreCollider.name + " has multiple Collider components, this is not allowed for ignored colliders! Ignoring.", ignoreCollider.gameObject);
				}
				else
				{
					data.Ignores.Add(componentNoAlloc, ignoreCollider, checkExisting: false);
				}
			}
		}
		else
		{
			data.Ignores.Remove(ignoreCollider);
		}
	}

	public void RefreshChildColliders()
	{
		_childColliders.Clear();
		GetComponentsInChildren(includeInactive: true, _childColliders);
		int num = 0;
		int num2 = _childColliders.Count - 1;
		while (num <= num2)
		{
			Collider collider = _childColliders[num];
			if (collider.isTrigger || collider == _collider.Collider)
			{
				_childColliders[num] = _childColliders[num2];
				_childColliders.RemoveAt(num2);
				num2--;
			}
			else
			{
				num++;
			}
		}
	}

	public bool IsValidHitCollider(Collider hitCollider)
	{
		return IsValidHitCollider(Data, hitCollider);
	}

	public bool HasModifier<T>() where T : class
	{
		return Data.Modifiers.HasProvider<T>();
	}

	public bool HasModifier<T>(T provider) where T : Component, IKCCInteractionProvider
	{
		if (provider == null)
		{
			return false;
		}
		return Data.Modifiers.HasProvider(provider);
	}

	public T GetModifier<T>() where T : class
	{
		return Data.Modifiers.GetProvider<T>();
	}

	public void GetModifiers<T>(List<T> providers) where T : class
	{
		Data.Modifiers.GetProviders(providers);
	}

	public List<T> GetModifiers<T>() where T : class
	{
		List<T> list = new List<T>();
		GetModifiers(list);
		return list;
	}

	public void AddModifier<T>(T provider) where T : Component, IKCCInteractionProvider
	{
		if (provider == null || !HasAnyAuthority)
		{
			return;
		}
		KCCData data = Data;
		if (data.Modifiers.HasProvider(provider))
		{
			return;
		}
		NetworkObject componentNoAlloc = provider.GetComponentNoAlloc<NetworkObject>();
		if (componentNoAlloc == null)
		{
			UnityEngine.Debug.LogError("Interaction provider " + provider.name + " doesn't have NetworkObject component! Ignoring.", provider.gameObject);
		}
		else if (provider.gameObject.GetComponentNoAlloc<IKCCInteractionProvider>() != provider)
		{
			UnityEngine.Debug.LogError("Object " + provider.name + " has multiple IKCCInteractionProvider components, this is not allowed for custom modifiers! Ignoring.", provider.gameObject);
		}
		else if (provider.CanStartInteraction(this, data))
		{
			KCCModifier kCCModifier = data.Modifiers.Add(componentNoAlloc, provider);
			if (kCCModifier.Processor != null)
			{
				OnProcessorAdded(data, kCCModifier.Processor);
			}
		}
	}

	public void RemoveModifier<T>(T provider) where T : Component, IKCCInteractionProvider
	{
		if (provider == null || !HasAnyAuthority)
		{
			return;
		}
		KCCData data = Data;
		KCCModifier kCCModifier = data.Modifiers.Find(provider);
		if (kCCModifier != null && provider.CanStopInteraction(this, data))
		{
			IKCCProcessor processor = kCCModifier.Processor;
			data.Modifiers.Remove(kCCModifier);
			if (processor != null)
			{
				OnProcessorRemoved(data, processor);
			}
		}
	}

	public bool HasInteraction<T>() where T : class
	{
		KCCData data = Data;
		if (data.Modifiers.HasProvider<T>())
		{
			return true;
		}
		if (data.Collisions.HasProvider<T>())
		{
			return true;
		}
		return false;
	}

	public bool HasInteraction<T>(T provider) where T : Component, IKCCInteractionProvider
	{
		if (provider == null)
		{
			return false;
		}
		KCCData data = Data;
		if (data.Modifiers.HasProvider(provider))
		{
			return true;
		}
		if (data.Collisions.HasProvider(provider))
		{
			return true;
		}
		return false;
	}

	public T GetInteraction<T>() where T : class
	{
		KCCData data = Data;
		T provider = data.Modifiers.GetProvider<T>();
		if (provider != null)
		{
			return provider;
		}
		provider = data.Collisions.GetProvider<T>();
		if (provider != null)
		{
			return provider;
		}
		return null;
	}

	public void GetInteractions<T>(List<T> providers) where T : class
	{
		providers.Clear();
		KCCData data = Data;
		data.Modifiers.GetProviders(providers, clearList: false);
		data.Collisions.GetProviders(providers, clearList: false);
	}

	public List<T> GetInteractions<T>() where T : class
	{
		List<T> list = new List<T>();
		GetInteractions(list);
		return list;
	}

	public bool HasProcessor<T>() where T : class
	{
		KCCData data = Data;
		if (data.Modifiers.HasProcessor<T>())
		{
			return true;
		}
		if (data.Collisions.HasProcessor<T>())
		{
			return true;
		}
		List<IKCCProcessor> localProcessors = _localProcessors;
		int i = 0;
		for (int count = localProcessors.Count; i < count; i++)
		{
			if (localProcessors[i] is T)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasProcessor<T>(T processor) where T : Component, IKCCProcessor
	{
		if (processor == null)
		{
			return false;
		}
		KCCData data = Data;
		if (data.Modifiers.HasProcessor(processor))
		{
			return true;
		}
		if (data.Collisions.HasProcessor(processor))
		{
			return true;
		}
		List<IKCCProcessor> localProcessors = _localProcessors;
		int i = 0;
		for (int count = localProcessors.Count; i < count; i++)
		{
			if (localProcessors[i] == processor)
			{
				return true;
			}
		}
		return false;
	}

	public T GetProcessor<T>() where T : class
	{
		KCCData data = Data;
		T processor = data.Modifiers.GetProcessor<T>();
		if (processor != null)
		{
			return processor;
		}
		processor = data.Collisions.GetProcessor<T>();
		if (processor != null)
		{
			return processor;
		}
		List<IKCCProcessor> localProcessors = _localProcessors;
		int i = 0;
		for (int count = localProcessors.Count; i < count; i++)
		{
			if (localProcessors[i] is T result)
			{
				return result;
			}
		}
		return null;
	}

	public void GetProcessors<T>(List<T> processors, bool sortByPriority = false) where T : class
	{
		processors.Clear();
		KCCData data = Data;
		data.Modifiers.GetProcessors(processors, clearList: false);
		data.Collisions.GetProcessors(processors, clearList: false);
		List<IKCCProcessor> localProcessors = _localProcessors;
		int i = 0;
		for (int count = localProcessors.Count; i < count; i++)
		{
			if (localProcessors[i] is T item)
			{
				processors.Add(item);
			}
		}
		if (sortByPriority)
		{
			SortProcessors(processors);
		}
	}

	public List<T> GetProcessors<T>(bool sortByPriority = false) where T : class
	{
		List<T> list = new List<T>();
		GetProcessors(list, sortByPriority);
		return list;
	}

	public void AddLocalProcessor(IKCCProcessor processor)
	{
		if (processor == null)
		{
			throw new ArgumentNullException("processor");
		}
		if (_driver == EKCCDriver.None)
		{
			throw new InvalidOperationException("KCC must be initialized first!");
		}
		if (_localProcessors.Contains(processor))
		{
			return;
		}
		_localProcessors.Add(processor);
		if (!HasAnyAuthority)
		{
			return;
		}
		try
		{
			processor.OnEnter(this, Data);
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogException(exception);
		}
	}

	public void RemoveLocalProcessor(IKCCProcessor processor)
	{
		if (!_localProcessors.Remove(processor) || !HasAnyAuthority || processor == null)
		{
			return;
		}
		try
		{
			processor.OnExit(this, Data);
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogException(exception);
		}
	}

	public bool HasPendingProcessor(IKCCProcessor processor)
	{
		if (_activeStage == EKCCStage.None)
		{
			throw new InvalidOperationException("Querying processor execution is allowed only during stage execution!");
		}
		IKCCProcessor[] stageProcessors = _stageProcessors;
		int i = _stageProcessorIndex + 1;
		for (int cachedProcessorCount = _cachedProcessorCount; i < cachedProcessorCount; i++)
		{
			if (stageProcessors[i] == processor)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasPendingProcessor<T>() where T : class
	{
		if (_activeStage == EKCCStage.None)
		{
			throw new InvalidOperationException("Querying processor execution is allowed only during stage execution!");
		}
		IKCCProcessor[] stageProcessors = _stageProcessors;
		int i = _stageProcessorIndex + 1;
		for (int cachedProcessorCount = _cachedProcessorCount; i < cachedProcessorCount; i++)
		{
			if (stageProcessors[i] is T)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasExecutedProcessor(IKCCProcessor processor)
	{
		if (_activeStage == EKCCStage.None)
		{
			throw new InvalidOperationException("Querying processor execution is allowed only during stage execution!");
		}
		IKCCProcessor[] stageProcessors = _stageProcessors;
		for (int num = _stageProcessorIndex; num >= 0; num--)
		{
			if (stageProcessors[num] == processor)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasExecutedProcessor<T>() where T : class
	{
		if (_activeStage == EKCCStage.None)
		{
			throw new InvalidOperationException("Querying processor execution is allowed only during stage execution!");
		}
		IKCCProcessor[] stageProcessors = _stageProcessors;
		for (int num = _stageProcessorIndex; num >= 0; num--)
		{
			if (stageProcessors[num] is T)
			{
				return true;
			}
		}
		return false;
	}

	public void SuppressProcessor(IKCCProcessor processor)
	{
		if (_activeStage == EKCCStage.None)
		{
			throw new InvalidOperationException("Suppressing processor execution is allowed only during stage execution!");
		}
		IKCCProcessor[] stageProcessors = _stageProcessors;
		int i = _stageProcessorIndex + 1;
		for (int cachedProcessorCount = _cachedProcessorCount; i < cachedProcessorCount; i++)
		{
			if (stageProcessors[i] == processor)
			{
				stageProcessors[i] = null;
				break;
			}
		}
	}

	public void SuppressProcessors<T>() where T : class
	{
		if (_activeStage == EKCCStage.None)
		{
			throw new InvalidOperationException("Suppressing processor execution is allowed only during stage execution!");
		}
		IKCCProcessor[] stageProcessors = _stageProcessors;
		int i = _stageProcessorIndex + 1;
		for (int cachedProcessorCount = _cachedProcessorCount; i < cachedProcessorCount; i++)
		{
			if (stageProcessors[i] is T)
			{
				stageProcessors[i] = null;
			}
		}
	}

	public void ProcessUserLogic(object userData)
	{
		if (_driver != EKCCDriver.None)
		{
			if (_activeStage != EKCCStage.None)
			{
				throw new InvalidOperationException("User stage is not allowed during other stage execution!");
			}
			KCCData data;
			if (IsInFixedUpdate)
			{
				data = _fixedData;
				InitializeFixedUpdate(forceReinitialize: false);
			}
			else
			{
				data = _renderData;
				InitializeRenderUpdate();
			}
			CacheProcessors(data);
			ProcessStage(EKCCStage.ProcessUserLogic, data, _processUserLogic, userData);
		}
	}

	public bool HasActiveFeature(EKCCFeature feature)
	{
		return _activeFeatures.Has(feature);
	}

	public void EnforceFeature(EKCCFeature feature)
	{
		if (_activeStage != EKCCStage.SetInputProperties)
		{
			throw new InvalidOperationException("Enforcing features is allowed only during SetInputProperties stage!");
		}
		_activeFeatures |= (EKCCFeatures)(1 << (int)feature);
	}

	public void SuppressFeature(EKCCFeature feature)
	{
		if (_activeStage != EKCCStage.SetInputProperties)
		{
			throw new InvalidOperationException("Suppressing features is allowed only during SetInputProperties stage!");
		}
		_activeFeatures &= (EKCCFeatures)(~(1 << (int)feature));
	}

	public KCCData GetHistory(int tick)
	{
		if (tick < 0)
		{
			return null;
		}
		KCCData kCCData = _historyData[tick % 60];
		if (kCCData != null && kCCData.Tick == tick)
		{
			return kCCData;
		}
		return null;
	}

	public void SetManualUpdate(bool hasManualUpdate)
	{
		_hasManualUpdate = hasManualUpdate;
		RefreshUpdater();
	}

	public void Initialize(EKCCDriver driver)
	{
		switch (driver)
		{
		case EKCCDriver.None:
			Deinitialize();
			return;
		case EKCCDriver.Fusion:
			if (!_isSpawned)
			{
				throw new InvalidOperationException("KCC cannot be explicitly initialized with Fusion driver before KCC.Spawned()!");
			}
			break;
		}
		if (!_isInitialized)
		{
			_defaultSettings.CopyFromOther(_settings);
			_isInitialized = true;
		}
		if (_driver == driver)
		{
			return;
		}
		bool hasManualUpdate = _hasManualUpdate;
		SetDefaults();
		_driver = driver;
		KCCUtility.GetLookRotationAngles(_transform.rotation, out var pitch, out var yaw);
		_fixedData = new KCCData();
		_fixedData.BasePosition = _transform.position;
		_fixedData.DesiredPosition = _transform.position;
		_fixedData.TargetPosition = _transform.position;
		_fixedData.LookPitch = pitch;
		_fixedData.LookYaw = yaw;
		InitializeFixedUpdate(forceReinitialize: true);
		if (_driver == EKCCDriver.Fusion && !_hasStateAuthority)
		{
			ReadNetworkData();
			SynchronizeTransform(_fixedData, synchronizePosition: true, synchronizeRotation: true, useAntiJitter: false);
		}
		_renderData = new KCCData();
		_renderData.CopyFromOther(_fixedData);
		_lastRenderPosition = _renderData.TargetPosition;
		_lastAntiJitterPosition = _renderData.TargetPosition;
		_transientData = new KCCTransientData();
		SetManualUpdate(hasManualUpdate);
		RefreshCollider();
		RefreshChildColliders();
		BaseKCCProcessor[] processors = _settings.Processors;
		if (processors != null)
		{
			int i = 0;
			for (int num = processors.Length; i < num; i++)
			{
				AddLocalProcessor(processors[i]);
			}
		}
		if (OnInitialize != null)
		{
			try
			{
				OnInitialize(this);
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
		}
		if (IsInFixedUpdate)
		{
			_renderData.CopyFromOther(_fixedData);
		}
		else
		{
			_fixedData.CopyFromOther(_renderData);
		}
		if (_driver == EKCCDriver.Fusion && HasAnyAuthority)
		{
			WriteNetworkData();
		}
	}

	public void Deinitialize()
	{
		_isInitialized = false;
		if (OnDeinitialize != null)
		{
			try
			{
				OnDeinitialize(this);
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
		}
		ForceRemoveAllCollisions(_fixedData);
		ForceRemoveAllModifiers(_fixedData);
		while (_localProcessors.Count > 0)
		{
			RemoveLocalProcessor(_localProcessors[_localProcessors.Count - 1]);
		}
		SetDefaults();
	}

	public void ManualFixedUpdate()
	{
		if (_driver != EKCCDriver.None)
		{
			if (!_hasManualUpdate)
			{
				throw new InvalidOperationException("Manual update is not set!");
			}
			OnFixedUpdateInternal();
		}
	}

	public void ManualRenderUpdate()
	{
		if (_driver != EKCCDriver.None)
		{
			if (!_hasManualUpdate)
			{
				throw new InvalidOperationException("Manual update is not set!");
			}
			OnRenderUpdateInternal();
		}
	}

	public void Interpolate(float alpha = -1f)
	{
		if (_driver != EKCCDriver.None)
		{
			KCCData data = Data;
			InterpolateNetworkData(alpha);
			CacheProcessors(data);
			ProcessStage(EKCCStage.OnInterpolate, data, _onInterpolate);
			SynchronizeTransform(data, synchronizePosition: true, synchronizeRotation: true, !IsInFixedUpdate && !IsProxy);
		}
	}

	private void Awake()
	{
		_transform = base.transform;
		_rigidbody = GetComponent<Rigidbody>();
		_localROProcessors = new ReadOnlyCollection<IKCCProcessor>(_localProcessors);
		if (_rigidbody == null)
		{
			throw new NullReferenceException("GameObject " + base.name + " has missing Rigidbody component!");
		}
		_rigidbody.isKinematic = true;
	}

	private void OnDestroy()
	{
		_isFixed = false;
		_isSpawned = false;
		_isInitialized = false;
		SetDefaults();
		OnInitialize = null;
		OnDeinitialize = null;
		OnCollisionEnter = null;
		OnCollisionExit = null;
		ResolveCollision = null;
	}

	public override void Spawned()
	{
		_isFixed = true;
		_isSpawned = true;
		if (_driver != EKCCDriver.Fusion)
		{
			Initialize(EKCCDriver.Fusion);
		}
	}

	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		_isFixed = false;
		_isSpawned = false;
		Deinitialize();
		OnInitialize = null;
		OnDeinitialize = null;
		OnCollisionEnter = null;
		OnCollisionExit = null;
		ResolveCollision = null;
	}

	public override void FixedUpdateNetwork()
	{
		if (_driver == EKCCDriver.Fusion && !_hasManualUpdate)
		{
			OnFixedUpdateInternal();
		}
	}

	public override void Render()
	{
		_isFixed = false;
		if (_driver == EKCCDriver.Fusion && !_hasManualUpdate)
		{
			OnRenderUpdateInternal();
		}
	}

	void IBeforeAllTicks.BeforeAllTicks(bool resimulation, int tickCount)
	{
		if (_driver != EKCCDriver.Fusion)
		{
			return;
		}
		_isFixed = true;
		if (!resimulation)
		{
			return;
		}
		_lastRenderInitialization = 0;
		_lastFixedInitialization = 0;
		_hasInputAuthority = Object.HasInputAuthority;
		_hasStateAuthority = Object.HasStateAuthority;
		KCCData kCCData = null;
		if (_hasInputAuthority)
		{
			int num = Runner.Simulation.Tick;
			kCCData = _historyData[num % 60];
			if (kCCData != null && kCCData.Tick == num)
			{
				_fixedData.CopyFromOther(kCCData);
				_fixedData.Frame = Time.frameCount;
			}
		}
		ReadNetworkData();
		if (kCCData != null)
		{
			RestoreHistoryData(kCCData);
		}
		RefreshCollider();
		SynchronizeTransform(_fixedData, synchronizePosition: true, synchronizeRotation: true, useAntiJitter: false);
	}

	void IAfterTick.AfterTick()
	{
		if (_driver == EKCCDriver.Fusion)
		{
			if (HasAnyAuthority)
			{
				PublishFixedData();
				WriteNetworkData();
			}
			if (Runner.IsLastTick)
			{
				_isFixed = false;
			}
		}
	}

	private void InitializeFixedUpdate(bool forceReinitialize)
	{
		if (_driver == EKCCDriver.None)
		{
			return;
		}
		int num = ((_driver == EKCCDriver.Fusion) ? ((int)Runner.Simulation.Tick) : Mathf.RoundToInt(Time.fixedUnscaledTime / Time.fixedUnscaledDeltaTime));
		if (num == _lastFixedInitialization && !forceReinitialize)
		{
			return;
		}
		_lastFixedInitialization = num;
		_debug.Reset();
		_statistics.Reset();
		_fixedData.Frame = Time.frameCount;
		_fixedData.Tick = num;
		_fixedData.Alpha = 1f;
		if (_driver == EKCCDriver.Fusion)
		{
			_fixedData.Time = Runner.SimulationTime;
			_fixedData.DeltaTime = Runner.Simulation.DeltaTime;
			_hasInputAuthority = _isSpawned && Object.HasInputAuthority;
			_hasStateAuthority = _isSpawned && Object.HasStateAuthority;
		}
		else
		{
			if (_driver != EKCCDriver.Unity)
			{
				throw new NotSupportedException(_driver.ToString());
			}
			_fixedData.Time = Time.fixedTime;
			_fixedData.DeltaTime = Time.fixedDeltaTime;
			_hasInputAuthority = true;
			_hasStateAuthority = true;
		}
		_fixedData.UnscaledDeltaTime = _fixedData.DeltaTime;
	}

	private void OnFixedUpdateInternal()
	{
		if (_driver == EKCCDriver.None)
		{
			return;
		}
		if (!IsInFixedUpdate)
		{
			throw new InvalidOperationException();
		}
		InitializeFixedUpdate(forceReinitialize: false);
		RefreshCollider();
		if (!HasAnyAuthority)
		{
			_debug.FixedUpdate(this);
			return;
		}
		_transientData.Store(this, _fixedData);
		bool isFixed = _isFixed;
		_isFixed = true;
		Move(_fixedData);
		_isFixed = isFixed;
		_transientData.Restore(this, _fixedData);
		if (_driver == EKCCDriver.Unity)
		{
			PublishFixedData();
		}
		SynchronizeTransform(_fixedData, synchronizePosition: true, synchronizeRotation: true, useAntiJitter: false);
		_debug.FixedUpdate(this);
	}

	private void InitializeRenderUpdate()
	{
		if (_driver == EKCCDriver.None)
		{
			return;
		}
		int frameCount = Time.frameCount;
		if (frameCount == _lastRenderInitialization)
		{
			return;
		}
		_lastRenderInitialization = frameCount;
		_debug.Reset();
		_statistics.Reset();
		_renderData.Frame = frameCount;
		float num = _renderData.Time;
		if (_driver == EKCCDriver.Fusion)
		{
			_renderData.Tick = Runner.Simulation.Tick;
			_renderData.Alpha = Runner.Simulation.StateAlpha;
			_renderData.Time = Runner.SimulationTime + Runner.Simulation.StateAlpha * Runner.DeltaTime;
		}
		else
		{
			if (_driver != EKCCDriver.Unity)
			{
				throw new NotSupportedException(_driver.ToString());
			}
			_renderData.Tick = Mathf.RoundToInt(Time.fixedUnscaledTime / Time.fixedUnscaledDeltaTime);
			_renderData.Alpha = Mathf.Clamp01((Time.time - _fixedData.Time) / Time.fixedDeltaTime);
			_renderData.Time = Time.time;
		}
		if (_settings.RenderBehavior == EKCCRenderBehavior.Interpolate)
		{
			_renderData.Tick--;
			_renderData.Time -= _fixedData.DeltaTime;
			if (_renderData.Frame == _fixedData.Frame)
			{
				num -= _fixedData.DeltaTime;
			}
		}
		_renderData.DeltaTime = _renderData.Time - num;
		_renderData.UnscaledDeltaTime = _renderData.DeltaTime;
	}

	private void OnRenderUpdateInternal()
	{
		_isFixed = false;
		if (_driver == EKCCDriver.None)
		{
			return;
		}
		if (IsInFixedUpdate)
		{
			throw new InvalidOperationException();
		}
		InitializeRenderUpdate();
		if (!HasAnyAuthority)
		{
			InterpolateNetworkData();
			CacheProcessors(_renderData);
			ProcessStage(EKCCStage.OnInterpolate, _renderData, _onInterpolate);
			SynchronizeTransform(_renderData, synchronizePosition: true, synchronizeRotation: true, useAntiJitter: false);
			_debug.RenderUpdate(this);
			return;
		}
		UpdatePredictionCorrection();
		if (_debug.ShowPath)
		{
			if (_renderData.Frame == _fixedData.Frame)
			{
				UnityEngine.Debug.DrawLine(_fixedData.TargetPosition, _renderData.TargetPosition, KCCDebug.FixedToRenderPathColor, _debug.DisplayTime);
			}
			else
			{
				UnityEngine.Debug.DrawLine(_lastRenderPosition, _renderData.TargetPosition, KCCDebug.PredictionCorrectionColor, _debug.DisplayTime);
			}
		}
		if (_settings.RenderBehavior == EKCCRenderBehavior.Predict)
		{
			if (_renderData.DeltaTime < 5E-05f)
			{
				Vector3 vector = _renderData.DesiredVelocity;
				if (_renderData.RealVelocity.sqrMagnitude <= vector.sqrMagnitude)
				{
					vector = _renderData.RealVelocity;
				}
				_renderData.BasePosition = _renderData.TargetPosition;
				_renderData.DesiredPosition = _renderData.BasePosition + vector * _renderData.DeltaTime;
				_renderData.TargetPosition = _renderData.DesiredPosition;
			}
			else
			{
				_transientData.Store(this, _renderData);
				Move(_renderData);
				_transientData.Restore(this, _renderData);
			}
		}
		else if (_settings.RenderBehavior == EKCCRenderBehavior.Interpolate)
		{
			_activeStage = EKCCStage.None;
			_activeFeatures = _settings.Features;
			CacheProcessors(_renderData);
			SetInputProperties(_renderData);
			KCCData fixedData = _fixedData;
			if (!fixedData.HasTeleported)
			{
				KCCData history = GetHistory(fixedData.Tick - 1);
				if (history != null)
				{
					float alpha = _renderData.Alpha;
					_renderData.BasePosition = Vector3.Lerp(history.BasePosition, fixedData.BasePosition, alpha) + _predictionError;
					_renderData.DesiredPosition = Vector3.Lerp(history.DesiredPosition, fixedData.DesiredPosition, alpha) + _predictionError;
					_renderData.TargetPosition = Vector3.Lerp(history.TargetPosition, fixedData.TargetPosition, alpha) + _predictionError;
					_renderData.LookPitch = Mathf.Lerp(history.LookPitch, fixedData.LookPitch, alpha);
					_renderData.LookYaw = KCCMathUtility.InterpolateRange(history.LookYaw, fixedData.LookYaw, -180f, 180f, alpha);
					_renderData.RealSpeed = Mathf.Lerp(history.RealSpeed, fixedData.RealSpeed, alpha);
					_renderData.RealVelocity = Vector3.Lerp(history.RealVelocity, fixedData.RealVelocity, alpha);
				}
			}
			ProcessStage(EKCCStage.OnInterpolate, _renderData, _onInterpolate);
		}
		SynchronizeTransform(_renderData, synchronizePosition: true, synchronizeRotation: true, useAntiJitter: true);
		_lastRenderPosition = _renderData.TargetPosition;
		_lastRenderTime = _renderData.Time;
		_debug.RenderUpdate(this);
	}

	private void UpdatePredictionCorrection()
	{
		if (!_activeFeatures.Has(EKCCFeature.PredictionCorrection))
		{
			_predictionError = default;
		}
		else if (_renderData.Frame == _fixedData.Frame)
		{
			KCCData kCCData = GetHistory(_renderData.Tick);
			if (kCCData == null || !(_lastRenderTime <= kCCData.Time))
			{
				return;
			}
			for (int i = 0; i < 5; i++)
			{
				KCCData history = GetHistory(kCCData.Tick - 1);
				if (history == null)
				{
					_predictionError = Vector3.Lerp(_predictionError, Vector3.zero, 20f * Time.deltaTime);
					break;
				}
				if (_lastRenderTime >= history.Time)
				{
					if (kCCData.HasTeleported || history.HasTeleported)
					{
						_predictionError = default;
						break;
					}
					float t = (_lastRenderTime - history.Time) / (kCCData.Time - history.Time);
					Vector3 vector = Vector3.Lerp(history.TargetPosition, kCCData.TargetPosition, t);
					if (_debug.ShowPath)
					{
						UnityEngine.Debug.DrawLine(vector, _lastRenderPosition, KCCDebug.PredictionErrorColor, _debug.DisplayTime);
					}
					_predictionError = _lastRenderPosition - vector;
					if (_predictionError.sqrMagnitude >= 4f)
					{
						_predictionError = default;
					}
					_predictionError = Vector3.Lerp(_predictionError, Vector3.zero, 20f * Time.deltaTime);
					_renderData.BasePosition += _predictionError;
					_renderData.DesiredPosition += _predictionError;
					_renderData.TargetPosition += _predictionError;
					break;
				}
				kCCData = history;
			}
		}
		else
		{
			_renderData.BasePosition -= _predictionError;
			_renderData.DesiredPosition -= _predictionError;
			_renderData.TargetPosition -= _predictionError;
			_predictionError = Vector3.Lerp(_predictionError, Vector3.zero, 30f * Time.deltaTime);
			_renderData.BasePosition += _predictionError;
			_renderData.DesiredPosition += _predictionError;
			_renderData.TargetPosition += _predictionError;
		}
	}

	private void Move(KCCData data)
	{
		_activeStage = EKCCStage.None;
		_activeFeatures = _settings.Features;
		float time = data.Time;
		float deltaTime = data.DeltaTime;
		Vector3 targetPosition = data.TargetPosition;
		Vector3 targetPosition2 = data.TargetPosition;
		bool isGrounded = data.IsGrounded;
		bool isSteppingUp = data.IsSteppingUp;
		bool isSnappingToGround = data.IsSnappingToGround;
		data.BasePosition = targetPosition;
		data.DesiredPosition = targetPosition2;
		if (_settings.Shape == EKCCShape.None)
		{
			ForceRemoveAllCollisions(data);
			ForceRemoveAllHits(data);
			return;
		}
		CacheProcessors(data);
		SetInputProperties(data);
		targetPosition = data.BasePosition;
		deltaTime = data.DeltaTime;
		ProcessStage(EKCCStage.SetDynamicVelocity, data, _setDynamicVelocity);
		ProcessStage(EKCCStage.SetKinematicDirection, data, _setKinematicDirection);
		ProcessStage(EKCCStage.SetKinematicTangent, data, _setKinematicTangent);
		ProcessStage(EKCCStage.SetKinematicSpeed, data, _setKinematicSpeed);
		ProcessStage(EKCCStage.SetKinematicVelocity, data, _setKinematicVelocity);
		ForceRemoveAllHits(data);
		float num = Mathf.Clamp01(deltaTime);
		Vector3 vector = data.DesiredVelocity * num + data.ExternalDelta;
		targetPosition2 = data.BasePosition + vector;
		if (!data.HasTeleported)
		{
			data.TargetPosition = data.BasePosition;
		}
		data.ExternalDelta = default;
		bool flag = false;
		float num2 = Mathf.Clamp(_settings.CCDRadiusMultiplier, 0.25f, 0.75f);
		float num3 = _settings.Radius * (num2 + 0.1f);
		float num4 = _settings.Radius * num2;
		while (!flag && !data.HasTeleported)
		{
			data.BasePosition = data.TargetPosition;
			float num5 = num;
			Vector3 vector2 = vector;
			if (_activeFeatures.Has(EKCCFeature.CCD))
			{
				float magnitude = vector2.magnitude;
				if (magnitude > num3)
				{
					float num6 = num4 / magnitude;
					num5 *= num6;
					vector2 *= num6;
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			num -= num5;
			vector -= vector2;
			if (num <= 0f)
			{
				num = 0f;
			}
			data.Time = time - num;
			data.DeltaTime = num5;
			data.DesiredPosition = data.BasePosition + vector2;
			data.TargetPosition = data.DesiredPosition;
			ProcessPhysicsQuery(data);
			UpdateCollisions(data);
			if (data.HasTeleported)
			{
				UpdateHits(data, null, EKCCHitsOverlapQuery.New);
				UpdateCollisions(data);
			}
			if (flag && !data.ExternalDelta.IsZero())
			{
				vector += data.ExternalDelta;
				data.ExternalDelta = default;
				flag = false;
			}
		}
		data.Time = time;
		data.DeltaTime = deltaTime;
		data.BasePosition = targetPosition;
		data.DesiredPosition = targetPosition2;
		data.WasGrounded = isGrounded;
		data.WasSteppingUp = isSteppingUp;
		data.WasSnappingToGround = isSnappingToGround;
		bool hasTeleported = data.HasTeleported;
		if (!hasTeleported)
		{
			data.RealVelocity = (data.TargetPosition - data.BasePosition) / data.DeltaTime;
			data.RealSpeed = data.RealVelocity.magnitude;
		}
		ProcessStage(EKCCStage.OnStay, data, _onStay);
		if (!hasTeleported && data.HasTeleported)
		{
			UpdateHits(data, null, EKCCHitsOverlapQuery.New);
			UpdateCollisions(data);
		}
		_activeStage = EKCCStage.None;
	}

	private void SetInputProperties(KCCData data)
	{
		data.Gravity = Physics.gravity;
		data.HasJumped = false;
		data.HasTeleported = false;
		data.MaxGroundAngle = 75f;
		data.MaxWallAngle = 5f;
		data.MaxHangAngle = 30f;
		data.MaxMoveSteps = _settings.MaxMoveSteps;
		ProcessStage(EKCCStage.SetInputProperties, data, _setInputProperties);
	}

	private void ProcessPhysicsQuery(KCCData data)
	{
		data.WasGrounded = data.IsGrounded;
		data.WasSteppingUp = data.IsSteppingUp;
		data.WasSnappingToGround = data.IsSnappingToGround;
		data.IsGrounded = false;
		data.IsSteppingUp = false;
		data.IsSnappingToGround = false;
		data.GroundNormal = default;
		data.GroundTangent = default;
		data.GroundPosition = default;
		data.GroundDistance = 0f;
		data.GroundAngle = 0f;
		ForceRemoveAllHits(data);
		if ((int)_settings.CollisionLayerMask != 0 && _collider.IsSpawned)
		{
			float extent = _settings.Radius;
			EKCCHitsOverlapQuery overlapQuery = EKCCHitsOverlapQuery.Default;
			if (_settings.ForceSingleOverlapQuery)
			{
				extent = _settings.Extent;
				overlapQuery = EKCCHitsOverlapQuery.Reuse;
			}
			OverlapCapsule(_extendedOverlapInfo, data, data.TargetPosition, _settings.Radius, _settings.Height, extent, _settings.CollisionLayerMask, QueryTriggerInteraction.Collide);
			if (_settings.SuppressConvexMeshColliders)
			{
				_extendedOverlapInfo.ToggleConvexMeshColliders(convex: false);
			}
			data.TargetPosition = DepenetrateColliders(_extendedOverlapInfo, data, data.BasePosition, data.TargetPosition, !data.HasJumped, data.MaxMoveSteps, 3);
			if (data.HasJumped)
			{
				data.IsGrounded = false;
			}
			if (data.IsGrounded)
			{
				CalculateGroundProperties(data);
			}
			CheckTriggersPenetration(_extendedOverlapInfo, data);
			if (!data.HasJumped)
			{
				TryStepUp(_extendedOverlapInfo, data);
			}
			if (!data.IsGrounded && data.WasGrounded && !data.HasJumped && !data.IsSteppingUp && !data.WasSteppingUp)
			{
				TrySnapToGround(data);
			}
			if (_settings.SuppressConvexMeshColliders)
			{
				_extendedOverlapInfo.ToggleConvexMeshColliders(convex: true);
			}
			UpdateHits(data, _extendedOverlapInfo, overlapQuery);
		}
		ProcessStage(EKCCStage.ProcessPhysicsQuery, data, _processPhysicsQuery);
	}

	private Vector3 DepenetrateColliders(KCCOverlapInfo overlapInfo, KCCData data, Vector3 basePosition, Vector3 targetPosition, bool probeGrounding, int maxSteps, int resolverIterations)
	{
		if (overlapInfo.ColliderHitCount == 0)
		{
			return targetPosition;
		}
		if (overlapInfo.ColliderHitCount == 1)
		{
			return DepenetrateSingle(overlapInfo, data, basePosition, targetPosition, probeGrounding, maxSteps);
		}
		return DepenetrateMultiple(overlapInfo, data, basePosition, targetPosition, probeGrounding, maxSteps, resolverIterations);
	}

	private Vector3 DepenetrateSingle(KCCOverlapInfo overlapInfo, KCCData data, Vector3 basePosition, Vector3 targetPosition, bool probeGrounding, int maxSteps)
	{
		float num = Mathf.Cos(Mathf.Clamp(data.MaxGroundAngle, 0f, 90f) * (MathF.PI / 180f));
		float num2 = 0f - Mathf.Cos(Mathf.Clamp(90f - data.MaxWallAngle, 0f, 90f) * (MathF.PI / 180f));
		float num3 = 0f - Mathf.Cos(Mathf.Clamp(90f - data.MaxHangAngle, 0f, 90f) * (MathF.PI / 180f));
		Vector3 vector = Vector3.up;
		float num4 = 0f;
		KCCOverlapHit kCCOverlapHit = overlapInfo.ColliderHits[0];
		kCCOverlapHit.UpDirectionDot = float.MinValue;
		KCCUtility.GetPositionAndRotation(kCCOverlapHit.Transform, out kCCOverlapHit.CachedPosition, out kCCOverlapHit.CachedRotation);
		if (maxSteps > 1)
		{
			float num5 = 0.001f;
			float num6 = Vector3.Distance(basePosition, targetPosition);
			if (num6 < (float)maxSteps * num5)
			{
				maxSteps = Mathf.Max(1, (int)(num6 / num5));
			}
		}
		if (maxSteps <= 1)
		{
			kCCOverlapHit.HasPenetration = Physics.ComputePenetration(_collider.Collider, targetPosition, Quaternion.identity, kCCOverlapHit.Collider, kCCOverlapHit.CachedPosition, kCCOverlapHit.CachedRotation, out var direction, out var distance);
			if (kCCOverlapHit.HasPenetration)
			{
				kCCOverlapHit.IsWithinExtent = true;
				if (distance > kCCOverlapHit.MaxPenetration)
				{
					kCCOverlapHit.MaxPenetration = distance;
				}
				float num7 = Vector3.Dot(direction, Vector3.up);
				if (num7 > kCCOverlapHit.UpDirectionDot)
				{
					kCCOverlapHit.UpDirectionDot = num7;
					if (num7 >= num)
					{
						kCCOverlapHit.CollisionType = ECollisionType.Ground;
						data.IsGrounded = true;
						vector = direction;
					}
					else if (num7 > 0f - num2)
					{
						kCCOverlapHit.CollisionType = ECollisionType.Slope;
					}
					else if (num7 >= num2)
					{
						kCCOverlapHit.CollisionType = ECollisionType.Wall;
					}
					else if (num7 >= num3)
					{
						kCCOverlapHit.CollisionType = ECollisionType.Hang;
					}
					else
					{
						kCCOverlapHit.CollisionType = ECollisionType.Top;
					}
				}
				if (num7 > 0f && num7 < num && distance >= 1E-06f && data.DynamicVelocity.y <= 0f && Vector3.Dot((targetPosition - basePosition).OnlyXZ(), direction.OnlyXZ()) < 0f)
				{
					KCCPhysicsUtility.ProjectVerticalPenetration(ref direction, ref distance);
				}
				targetPosition += direction * distance;
			}
		}
		else
		{
			Vector3 vector2 = (targetPosition - basePosition) / maxSteps;
			Vector3 vector3 = basePosition;
			int num8 = maxSteps;
			while (num8 > 0)
			{
				num8--;
				vector3 += vector2;
				kCCOverlapHit.HasPenetration = Physics.ComputePenetration(_collider.Collider, vector3, Quaternion.identity, kCCOverlapHit.Collider, kCCOverlapHit.CachedPosition, kCCOverlapHit.CachedRotation, out var direction2, out var distance2);
				if (!kCCOverlapHit.HasPenetration)
				{
					continue;
				}
				kCCOverlapHit.IsWithinExtent = true;
				if (distance2 > kCCOverlapHit.MaxPenetration)
				{
					kCCOverlapHit.MaxPenetration = distance2;
				}
				float num9 = Vector3.Dot(direction2, Vector3.up);
				if (num9 > kCCOverlapHit.UpDirectionDot)
				{
					kCCOverlapHit.UpDirectionDot = num9;
					if (num9 >= num)
					{
						kCCOverlapHit.CollisionType = ECollisionType.Ground;
						data.IsGrounded = true;
						vector = direction2;
					}
					else if (num9 > 0f - num2)
					{
						kCCOverlapHit.CollisionType = ECollisionType.Slope;
					}
					else if (num9 >= num2)
					{
						kCCOverlapHit.CollisionType = ECollisionType.Wall;
					}
					else if (num9 >= num3)
					{
						kCCOverlapHit.CollisionType = ECollisionType.Hang;
					}
					else
					{
						kCCOverlapHit.CollisionType = ECollisionType.Top;
					}
				}
				if (num9 > 0f && num9 < num && distance2 >= 1E-06f && data.DynamicVelocity.y <= 0f && Vector3.Dot(vector2.OnlyXZ(), direction2.OnlyXZ()) < 0f)
				{
					KCCPhysicsUtility.ProjectVerticalPenetration(ref direction2, ref distance2);
				}
				vector3 += direction2 * distance2;
			}
			targetPosition = vector3;
		}
		if (kCCOverlapHit.UpDirectionDot == float.MinValue)
		{
			kCCOverlapHit.UpDirectionDot = 0f;
		}
		if (probeGrounding && !data.IsGrounded)
		{
			if (KCCPhysicsUtility.CheckGround(_collider.Collider, targetPosition, kCCOverlapHit.Collider, kCCOverlapHit.CachedPosition, kCCOverlapHit.CachedRotation, _settings.Radius, _settings.Height, _settings.Extent, num, out var groundNormal, out var groundDistance, out var isWithinExtent))
			{
				data.IsGrounded = true;
				vector = groundNormal;
				num4 = groundDistance;
				kCCOverlapHit.IsWithinExtent = true;
				kCCOverlapHit.CollisionType = ECollisionType.Ground;
			}
			else if (isWithinExtent)
			{
				kCCOverlapHit.IsWithinExtent = true;
				if (kCCOverlapHit.CollisionType == ECollisionType.None)
				{
					kCCOverlapHit.CollisionType = ECollisionType.Slope;
				}
			}
		}
		if (data.IsGrounded)
		{
			data.GroundNormal = vector;
			data.GroundAngle = Vector3.Angle(vector, Vector3.up);
			data.GroundPosition = targetPosition + new Vector3(0f, _settings.Radius, 0f) - vector * (_settings.Radius + num4);
			data.GroundDistance = num4;
		}
		return targetPosition;
	}

	private Vector3 DepenetrateMultiple(KCCOverlapInfo overlapInfo, KCCData data, Vector3 basePosition, Vector3 targetPosition, bool probeGrounding, int maxSteps, int resolverIterations)
	{
		float num = Mathf.Cos(Mathf.Clamp(data.MaxGroundAngle, 0f, 90f) * (MathF.PI / 180f));
		float num2 = 0f - Mathf.Cos(Mathf.Clamp(90f - data.MaxWallAngle, 0f, 90f) * (MathF.PI / 180f));
		float num3 = 0f - Mathf.Cos(Mathf.Clamp(90f - data.MaxHangAngle, 0f, 90f) * (MathF.PI / 180f));
		float num4 = 0f;
		float num5 = 0f;
		Vector3 other = default;
		Vector3 vector = default;
		Vector3 lhs = (targetPosition - basePosition).OnlyXZ();
		for (int i = 0; i < overlapInfo.ColliderHitCount; i++)
		{
			KCCOverlapHit kCCOverlapHit = overlapInfo.ColliderHits[i];
			kCCOverlapHit.UpDirectionDot = float.MinValue;
			KCCUtility.GetPositionAndRotation(kCCOverlapHit.Transform, out kCCOverlapHit.CachedPosition, out kCCOverlapHit.CachedRotation);
		}
		if (maxSteps > 1)
		{
			float num6 = 0.001f;
			float num7 = Vector3.Distance(basePosition, targetPosition);
			if (num7 < (float)maxSteps * num6)
			{
				maxSteps = Mathf.Max(1, (int)(num7 / num6));
			}
		}
		if (maxSteps <= 1)
		{
			_resolver.Reset();
			for (int j = 0; j < overlapInfo.ColliderHitCount; j++)
			{
				KCCOverlapHit kCCOverlapHit2 = overlapInfo.ColliderHits[j];
				kCCOverlapHit2.HasPenetration = Physics.ComputePenetration(_collider.Collider, targetPosition, Quaternion.identity, kCCOverlapHit2.Collider, kCCOverlapHit2.CachedPosition, kCCOverlapHit2.CachedRotation, out var direction, out var distance);
				if (!kCCOverlapHit2.HasPenetration)
				{
					continue;
				}
				kCCOverlapHit2.IsWithinExtent = true;
				if (distance > kCCOverlapHit2.MaxPenetration)
				{
					kCCOverlapHit2.MaxPenetration = distance;
				}
				float num8 = Vector3.Dot(direction, Vector3.up);
				if (num8 > kCCOverlapHit2.UpDirectionDot)
				{
					kCCOverlapHit2.UpDirectionDot = num8;
					if (num8 >= num)
					{
						kCCOverlapHit2.CollisionType = ECollisionType.Ground;
						data.IsGrounded = true;
						if (num8 >= num5)
						{
							num5 = num8;
							other = direction;
						}
						vector += direction * num8;
					}
					else if (num8 > 0f - num2)
					{
						kCCOverlapHit2.CollisionType = ECollisionType.Slope;
					}
					else if (num8 >= num2)
					{
						kCCOverlapHit2.CollisionType = ECollisionType.Wall;
					}
					else if (num8 >= num3)
					{
						kCCOverlapHit2.CollisionType = ECollisionType.Hang;
					}
					else
					{
						kCCOverlapHit2.CollisionType = ECollisionType.Top;
					}
				}
				if (num8 > 0f && num8 < num && distance >= 1E-06f && data.DynamicVelocity.y <= 0f && Vector3.Dot(lhs, direction.OnlyXZ()) < 0f)
				{
					KCCPhysicsUtility.ProjectVerticalPenetration(ref direction, ref distance);
				}
				_resolver.AddCorrection(direction, distance);
			}
			int num9 = Mathf.Max(0, resolverIterations);
			float num10 = 1f - (float)Mathf.Min(num9, 2) * 0.25f;
			if (_resolver.Size == 2)
			{
				_resolver.GetCorrection(0, out var direction2);
				_resolver.GetCorrection(1, out var direction3);
				if (Vector3.Dot(direction2, direction3) >= 0f)
				{
					targetPosition += _resolver.CalculateMinMax() * num10;
				}
				else
				{
					targetPosition += _resolver.CalculateBinary() * num10;
				}
			}
			else
			{
				targetPosition += _resolver.CalculateGradientDescent(12, 0.0001f) * num10;
			}
			while (num9 > 0)
			{
				num9--;
				_resolver.Reset();
				for (int k = 0; k < overlapInfo.ColliderHitCount; k++)
				{
					KCCOverlapHit kCCOverlapHit3 = overlapInfo.ColliderHits[k];
					if (!Physics.ComputePenetration(_collider.Collider, targetPosition, Quaternion.identity, kCCOverlapHit3.Collider, kCCOverlapHit3.CachedPosition, kCCOverlapHit3.CachedRotation, out var direction4, out var distance2))
					{
						continue;
					}
					kCCOverlapHit3.IsWithinExtent = true;
					kCCOverlapHit3.HasPenetration = true;
					if (distance2 > kCCOverlapHit3.MaxPenetration)
					{
						kCCOverlapHit3.MaxPenetration = distance2;
					}
					float num11 = Vector3.Dot(direction4, Vector3.up);
					if (num11 > kCCOverlapHit3.UpDirectionDot)
					{
						kCCOverlapHit3.UpDirectionDot = num11;
						if (num11 >= num)
						{
							kCCOverlapHit3.CollisionType = ECollisionType.Ground;
							data.IsGrounded = true;
							if (num11 >= num5)
							{
								num5 = num11;
								other = direction4;
							}
							vector += direction4 * num11;
						}
						else if (num11 > 0f - num2)
						{
							kCCOverlapHit3.CollisionType = ECollisionType.Slope;
						}
						else if (num11 >= num2)
						{
							kCCOverlapHit3.CollisionType = ECollisionType.Wall;
						}
						else if (num11 >= num3)
						{
							kCCOverlapHit3.CollisionType = ECollisionType.Hang;
						}
						else
						{
							kCCOverlapHit3.CollisionType = ECollisionType.Top;
						}
					}
					if (num11 > 0f && num11 < num && distance2 >= 1E-06f && data.DynamicVelocity.y <= 0f && Vector3.Dot(lhs, direction4.OnlyXZ()) < 0f)
					{
						KCCPhysicsUtility.ProjectVerticalPenetration(ref direction4, ref distance2);
					}
					_resolver.AddCorrection(direction4, distance2);
				}
				if (_resolver.Size == 0)
				{
					break;
				}
				switch (num9)
				{
				case 0:
					if (_resolver.Size == 2)
					{
						_resolver.GetCorrection(0, out var direction5);
						_resolver.GetCorrection(1, out var direction6);
						if (Vector3.Dot(direction5, direction6) >= 0f)
						{
							targetPosition += _resolver.CalculateGradientDescent(12, 0.0001f);
						}
						else
						{
							targetPosition += _resolver.CalculateBinary();
						}
					}
					else
					{
						targetPosition += _resolver.CalculateGradientDescent(12, 0.0001f);
					}
					break;
				case 1:
					targetPosition += _resolver.CalculateMinMax() * 0.75f;
					break;
				default:
					targetPosition += _resolver.CalculateMinMax() * 0.5f;
					break;
				}
			}
		}
		else
		{
			Vector3 vector2 = (targetPosition - basePosition) / maxSteps;
			Vector3 vector3 = basePosition;
			int num12 = maxSteps;
			while (num12 > 1)
			{
				num12--;
				vector3 += vector2;
				_resolver.Reset();
				for (int l = 0; l < overlapInfo.ColliderHitCount; l++)
				{
					KCCOverlapHit kCCOverlapHit4 = overlapInfo.ColliderHits[l];
					kCCOverlapHit4.HasPenetration = Physics.ComputePenetration(_collider.Collider, vector3, Quaternion.identity, kCCOverlapHit4.Collider, kCCOverlapHit4.CachedPosition, kCCOverlapHit4.CachedRotation, out var direction7, out var distance3);
					if (!kCCOverlapHit4.HasPenetration)
					{
						continue;
					}
					kCCOverlapHit4.IsWithinExtent = true;
					if (distance3 > kCCOverlapHit4.MaxPenetration)
					{
						kCCOverlapHit4.MaxPenetration = distance3;
					}
					float num13 = Vector3.Dot(direction7, Vector3.up);
					if (num13 > kCCOverlapHit4.UpDirectionDot)
					{
						kCCOverlapHit4.UpDirectionDot = num13;
						if (num13 >= num)
						{
							kCCOverlapHit4.CollisionType = ECollisionType.Ground;
							data.IsGrounded = true;
							if (num13 >= num5)
							{
								num5 = num13;
								other = direction7;
							}
							vector += direction7 * num13;
						}
						else if (num13 > 0f - num2)
						{
							kCCOverlapHit4.CollisionType = ECollisionType.Slope;
						}
						else if (num13 >= num2)
						{
							kCCOverlapHit4.CollisionType = ECollisionType.Wall;
						}
						else if (num13 >= num3)
						{
							kCCOverlapHit4.CollisionType = ECollisionType.Hang;
						}
						else
						{
							kCCOverlapHit4.CollisionType = ECollisionType.Top;
						}
					}
					if (num13 > 0f && num13 < num && distance3 >= 1E-06f && data.DynamicVelocity.y <= 0f && Vector3.Dot(vector2.OnlyXZ(), direction7.OnlyXZ()) < 0f)
					{
						KCCPhysicsUtility.ProjectVerticalPenetration(ref direction7, ref distance3);
					}
					_resolver.AddCorrection(direction7, distance3);
				}
				if (_resolver.Size == 2)
				{
					_resolver.GetCorrection(0, out var direction8);
					_resolver.GetCorrection(1, out var direction9);
					if (Vector3.Dot(direction8, direction9) >= 0f)
					{
						vector3 += _resolver.CalculateMinMax();
					}
					else
					{
						vector3 += _resolver.CalculateBinary();
					}
				}
				else
				{
					vector3 += _resolver.CalculateMinMax();
				}
			}
			num12--;
			vector3 += vector2;
			_resolver.Reset();
			for (int m = 0; m < overlapInfo.ColliderHitCount; m++)
			{
				KCCOverlapHit kCCOverlapHit5 = overlapInfo.ColliderHits[m];
				kCCOverlapHit5.HasPenetration = Physics.ComputePenetration(_collider.Collider, vector3, Quaternion.identity, kCCOverlapHit5.Collider, kCCOverlapHit5.CachedPosition, kCCOverlapHit5.CachedRotation, out var direction10, out var distance4);
				if (!kCCOverlapHit5.HasPenetration)
				{
					continue;
				}
				kCCOverlapHit5.IsWithinExtent = true;
				if (distance4 > kCCOverlapHit5.MaxPenetration)
				{
					kCCOverlapHit5.MaxPenetration = distance4;
				}
				float num14 = Vector3.Dot(direction10, Vector3.up);
				if (num14 > kCCOverlapHit5.UpDirectionDot)
				{
					kCCOverlapHit5.UpDirectionDot = num14;
					if (num14 >= num)
					{
						kCCOverlapHit5.CollisionType = ECollisionType.Ground;
						data.IsGrounded = true;
						if (num14 >= num5)
						{
							num5 = num14;
							other = direction10;
						}
						vector += direction10 * num14;
					}
					else if (num14 > 0f - num2)
					{
						kCCOverlapHit5.CollisionType = ECollisionType.Slope;
					}
					else if (num14 >= num2)
					{
						kCCOverlapHit5.CollisionType = ECollisionType.Wall;
					}
					else if (num14 >= num3)
					{
						kCCOverlapHit5.CollisionType = ECollisionType.Hang;
					}
					else
					{
						kCCOverlapHit5.CollisionType = ECollisionType.Top;
					}
				}
				if (num14 > 0f && num14 < num && distance4 >= 1E-06f && data.DynamicVelocity.y <= 0f && Vector3.Dot(vector2.OnlyXZ(), direction10.OnlyXZ()) < 0f)
				{
					KCCPhysicsUtility.ProjectVerticalPenetration(ref direction10, ref distance4);
				}
				_resolver.AddCorrection(direction10, distance4);
			}
			if (_resolver.Size == 2)
			{
				_resolver.GetCorrection(0, out var direction11);
				_resolver.GetCorrection(1, out var direction12);
				if (Vector3.Dot(direction11, direction12) >= 0f)
				{
					vector3 += _resolver.CalculateMinMax();
				}
				else
				{
					vector3 += _resolver.CalculateBinary();
				}
			}
			else
			{
				vector3 += _resolver.CalculateGradientDescent(12, 0.0001f);
			}
			targetPosition = vector3;
		}
		for (int n = 0; n < overlapInfo.ColliderHitCount; n++)
		{
			KCCOverlapHit kCCOverlapHit6 = overlapInfo.ColliderHits[n];
			if (kCCOverlapHit6.UpDirectionDot == float.MinValue)
			{
				kCCOverlapHit6.UpDirectionDot = 0f;
			}
		}
		if (probeGrounding && !data.IsGrounded)
		{
			Vector3 vector4 = Vector3.up;
			float num15 = 1000f;
			for (int num16 = 0; num16 < overlapInfo.ColliderHitCount; num16++)
			{
				KCCOverlapHit kCCOverlapHit7 = overlapInfo.ColliderHits[num16];
				if (KCCPhysicsUtility.CheckGround(_collider.Collider, targetPosition, kCCOverlapHit7.Collider, kCCOverlapHit7.CachedPosition, kCCOverlapHit7.CachedRotation, _settings.Radius, _settings.Height, _settings.Extent, num, out var groundNormal, out var groundDistance, out var isWithinExtent))
				{
					data.IsGrounded = true;
					if (groundDistance < num15)
					{
						vector4 = groundNormal;
						num15 = groundDistance;
					}
					kCCOverlapHit7.IsWithinExtent = true;
					kCCOverlapHit7.CollisionType = ECollisionType.Ground;
				}
				else if (isWithinExtent)
				{
					kCCOverlapHit7.IsWithinExtent = true;
					if (kCCOverlapHit7.CollisionType == ECollisionType.None)
					{
						kCCOverlapHit7.CollisionType = ECollisionType.Slope;
					}
				}
			}
			if (data.IsGrounded)
			{
				other = vector4;
				vector = vector4;
				num4 = num15;
			}
		}
		if (data.IsGrounded)
		{
			if (!vector.IsEqual(other))
			{
				vector.Normalize();
			}
			data.GroundNormal = vector;
			data.GroundAngle = Vector3.Angle(data.GroundNormal, Vector3.up);
			data.GroundPosition = targetPosition + new Vector3(0f, _settings.Radius, 0f) - data.GroundNormal * (_settings.Radius + num4);
			data.GroundDistance = num4;
		}
		return targetPosition;
	}

	private void CheckTriggersPenetration(KCCOverlapInfo overlapInfo, KCCData data)
	{
		for (int i = 0; i < overlapInfo.TriggerHitCount; i++)
		{
			KCCOverlapHit kCCOverlapHit = overlapInfo.TriggerHits[i];
			KCCUtility.GetPositionAndRotation(kCCOverlapHit.Transform, out kCCOverlapHit.CachedPosition, out kCCOverlapHit.CachedRotation);
			kCCOverlapHit.CollisionType = ((kCCOverlapHit.IsWithinExtent = (kCCOverlapHit.HasPenetration = Physics.ComputePenetration(_collider.Collider, data.TargetPosition, Quaternion.identity, kCCOverlapHit.Collider, kCCOverlapHit.CachedPosition, kCCOverlapHit.CachedRotation, out var _, out var distance))) ? ECollisionType.Trigger : ECollisionType.None);
			if (distance > kCCOverlapHit.MaxPenetration)
			{
				kCCOverlapHit.MaxPenetration = distance;
			}
		}
	}

	private void TryStepUp(KCCOverlapInfo overlapInfo, KCCData data)
	{
		if (!_activeFeatures.Has(EKCCFeature.StepUp) || _settings.StepHeight <= 0f)
		{
			return;
		}
		data.IsSteppingUp = false;
		if (data.HasJumped || data.HasTeleported)
		{
			return;
		}
		if (IsTouchingSlopeOrWallOrHang(overlapInfo))
		{
			data.IsSteppingUp = true;
		}
		else
		{
			float magnitude = (data.DesiredPosition - data.BasePosition).magnitude;
			if (magnitude > 0.001f && (data.TargetPosition - data.BasePosition).magnitude / magnitude < 0.5f)
			{
				data.IsSteppingUp = true;
			}
		}
		if (!data.IsSteppingUp)
		{
			return;
		}
		Vector3 basePosition = data.BasePosition;
		Vector3 desiredPosition = data.DesiredPosition;
		Vector3 vector = data.TargetPosition;
		Vector3 vector2 = desiredPosition - basePosition;
		Vector3 vector3 = Vector3.Normalize(vector2);
		if (vector3.IsZero())
		{
			data.IsSteppingUp = false;
			return;
		}
		if (Vector3.Dot(vector3, Vector3.down) >= 0.9f)
		{
			data.IsSteppingUp = false;
			return;
		}
		Vector3 vector4 = vector - desiredPosition;
		float magnitude2 = vector4.magnitude;
		Vector3 vector5 = ((magnitude2 > 0.001f) ? (vector4 / magnitude2) : (-vector3));
		if (Vector3.Dot(vector3, vector5) >= 0f)
		{
			data.IsSteppingUp = false;
			return;
		}
		if (!vector5.IsZero())
		{
			Ray ray = new Ray(basePosition - vector2 * 2f, vector3);
			if (new Plane(vector5, vector).Raycast(ray, out var enter))
			{
				vector = ray.GetPoint(enter);
			}
		}
		float num = _settings.Radius - _settings.Extent;
		Vector3 vector6 = vector + new Vector3(0f, _settings.StepHeight, 0f);
		if (OverlapCapsule(_sharedOverlapInfo, data, vector6, num, _settings.Height, 0f, _settings.CollisionLayerMask, QueryTriggerInteraction.Ignore) && _sharedOverlapInfo.ColliderHitCount > 0)
		{
			data.IsSteppingUp = false;
			return;
		}
		Vector3 vector7 = Vector3.Normalize(vector3.OnlyXZ());
		Vector3 vector8 = Vector3.Normalize(-vector5.OnlyXZ());
		if (Vector3.Dot(vector7, vector8) < 0.1f)
		{
			data.IsSteppingUp = false;
			return;
		}
		Vector3 vector9 = Vector3.Normalize(vector7 + vector8);
		vector6 += vector9 * _settings.StepDepth;
		if (OverlapCapsule(_sharedOverlapInfo, data, vector6, num, _settings.Height, 0f, _settings.CollisionLayerMask, QueryTriggerInteraction.Ignore) && _sharedOverlapInfo.ColliderHitCount > 0)
		{
			data.IsSteppingUp = false;
			return;
		}
		float num2 = _settings.StepHeight;
		if (SphereCast(_raycastInfo, data, vector6 + new Vector3(0f, num, 0f), Vector3.down, num2, num, _settings.CollisionLayerMask, QueryTriggerInteraction.Ignore) && _raycastInfo.ColliderHitCount > 0)
		{
			Vector3 vector10 = new Vector3(0f, float.MinValue, 0f);
			int i = 0;
			for (int colliderHitCount = _raycastInfo.ColliderHitCount; i < colliderHitCount; i++)
			{
				RaycastHit raycastHit = _raycastInfo.ColliderHits[i].RaycastHit;
				if (raycastHit.point.y > vector10.y)
				{
					vector10 = raycastHit.point;
				}
			}
			num2 = Mathf.Clamp(num2 - (vector6.y - vector10.y) - _settings.Extent, 0f, _settings.StepHeight);
		}
		float num3 = Vector3.Distance(basePosition, desiredPosition);
		float num4 = Vector3.Distance(basePosition, vector);
		float num5 = Mathf.Clamp((num3 - num4) * _settings.StepSpeed, 0f, num2);
		num5 *= Mathf.Clamp01(Vector3.Dot(vector3, -vector5));
		data.TargetPosition = vector + new Vector3(0f, num5, 0f);
		data.IsGrounded = true;
		data.GroundNormal = Vector3.up;
		data.GroundDistance = _settings.Extent;
		data.GroundPosition = data.TargetPosition;
		data.GroundTangent = data.TransformDirection;
		static bool IsTouchingSlopeOrWallOrHang(KCCOverlapInfo kCCOverlapInfo)
		{
			for (int j = 0; j < kCCOverlapInfo.ColliderHitCount; j++)
			{
				KCCOverlapHit kCCOverlapHit = kCCOverlapInfo.ColliderHits[j];
				if (kCCOverlapHit.IsWithinExtent && (kCCOverlapHit.CollisionType == ECollisionType.Slope || kCCOverlapHit.CollisionType == ECollisionType.Wall || kCCOverlapHit.CollisionType == ECollisionType.Hang))
				{
					return true;
				}
			}
			return false;
		}
	}

	private void TrySnapToGround(KCCData data)
	{
		if (!_activeFeatures.Has(EKCCFeature.SnapToGround) || _settings.GroundSnapDistance <= 0f || data.DynamicVelocity.y > 0f)
		{
			return;
		}
		float groundSnapDistance = _settings.GroundSnapDistance;
		float num = _settings.Radius * 0.25f;
		int num2 = Mathf.CeilToInt(groundSnapDistance / num);
		float num3 = groundSnapDistance / (float)num2;
		OverlapCapsule(_sharedOverlapInfo, data, data.TargetPosition - new Vector3(0f, _settings.GroundSnapDistance, 0f), _settings.Radius, _settings.Height + _settings.GroundSnapDistance, _settings.Radius, _settings.CollisionLayerMask, QueryTriggerInteraction.Ignore);
		if (_sharedOverlapInfo.ColliderHitCount == 0)
		{
			return;
		}
		if (_settings.SuppressConvexMeshColliders)
		{
			_sharedOverlapInfo.ToggleConvexMeshColliders(convex: false);
		}
		Vector3 vector = data.TargetPosition;
		Vector3 vector2 = new Vector3(0f, 0f - num3, 0f);
		for (int i = 0; i < num2; i++)
		{
			vector = DepenetrateColliders(_sharedOverlapInfo, data, vector, vector + vector2, probeGrounding: false, 0, 0);
			if (data.IsGrounded)
			{
				float num4 = _settings.GroundSnapSpeed * data.UnscaledDeltaTime;
				Vector3 vector3 = vector - data.TargetPosition;
				if (!data.WasSnappingToGround)
				{
					num4 *= 0.5f;
				}
				Vector3 vector4 = ((!(vector3.sqrMagnitude <= num4 * num4)) ? (data.TargetPosition + vector3.normalized * num4) : vector);
				_debug.DrawGroundSnapping(data.TargetPosition, vector, vector4, IsInFixedUpdate);
				data.TargetPosition = vector4;
				data.GroundDistance = Mathf.Max(0f, vector4.y - vector.y);
				data.IsSnappingToGround = true;
				CalculateGroundProperties(data);
				break;
			}
		}
		if (_settings.SuppressConvexMeshColliders)
		{
			_sharedOverlapInfo.ToggleConvexMeshColliders(convex: true);
		}
	}

	private static void CalculateGroundProperties(KCCData data)
	{
		Vector3 projectedVector2;
		if (KCCPhysicsUtility.ProjectOnGround(data.GroundNormal, data.GroundNormal.OnlyXZ(), out var projectedVector))
		{
			data.GroundTangent = projectedVector.normalized;
		}
		else if (KCCPhysicsUtility.ProjectOnGround(data.GroundNormal, data.DesiredVelocity.OnlyXZ(), out projectedVector2))
		{
			data.GroundTangent = projectedVector2.normalized;
		}
		else
		{
			data.GroundTangent = data.TransformDirection;
		}
	}

	private bool OverlapCapsule(KCCOverlapInfo overlapInfo, KCCData data, Vector3 position, float radius, float height, float extent, LayerMask layerMask, QueryTriggerInteraction triggerInteraction)
	{
		overlapInfo.Reset(deep: false);
		overlapInfo.Position = position;
		overlapInfo.Radius = radius;
		overlapInfo.Height = height;
		overlapInfo.Extent = extent;
		overlapInfo.LayerMask = layerMask;
		overlapInfo.TriggerInteraction = triggerInteraction;
		Vector3 point = position + new Vector3(0f, height - radius, 0f);
		Vector3 point2 = position + new Vector3(0f, radius, 0f);
		Collider[] hitColliders = _hitColliders;
		int num = GetPhysicsScene().OverlapCapsule(point2, point, radius + extent, hitColliders, layerMask, triggerInteraction);
		for (int i = 0; i < num; i++)
		{
			Collider collider = hitColliders[i];
			if (IsValidHitCollider(data, collider))
			{
				overlapInfo.AddHit(collider);
			}
		}
		_statistics.OverlapQueries++;
		return overlapInfo.AllHitCount > 0;
	}

	private bool Raycast(KCCRaycastInfo raycastInfo, KCCData data, Vector3 origin, Vector3 direction, float maxDistance, LayerMask layerMask, QueryTriggerInteraction triggerInteraction)
	{
		raycastInfo.Reset(deep: false);
		raycastInfo.Origin = origin;
		raycastInfo.Direction = direction;
		raycastInfo.MaxDistance = maxDistance;
		raycastInfo.LayerMask = layerMask;
		raycastInfo.TriggerInteraction = triggerInteraction;
		RaycastHit[] raycastHits = _raycastHits;
		int num = GetPhysicsScene().Raycast(origin, direction, raycastHits, maxDistance, layerMask, triggerInteraction);
		for (int i = 0; i < num; i++)
		{
			RaycastHit raycastHit = raycastHits[i];
			if (IsValidHitCollider(data, raycastHit.collider))
			{
				raycastInfo.AddHit(raycastHit);
			}
		}
		_statistics.RaycastQueries++;
		return raycastInfo.AllHitCount > 0;
	}

	private bool SphereCast(KCCRaycastInfo raycastInfo, KCCData data, Vector3 origin, Vector3 direction, float maxDistance, float radius, LayerMask layerMask, QueryTriggerInteraction triggerInteraction)
	{
		raycastInfo.Reset(deep: false);
		raycastInfo.Origin = origin;
		raycastInfo.Direction = direction;
		raycastInfo.MaxDistance = maxDistance;
		raycastInfo.Radius = radius;
		raycastInfo.LayerMask = layerMask;
		raycastInfo.TriggerInteraction = triggerInteraction;
		RaycastHit[] raycastHits = _raycastHits;
		int num = GetPhysicsScene().SphereCast(origin, radius, direction, raycastHits, maxDistance, layerMask, triggerInteraction);
		for (int i = 0; i < num; i++)
		{
			RaycastHit raycastHit = raycastHits[i];
			if (IsValidHitCollider(data, raycastHit.collider))
			{
				raycastInfo.AddHit(raycastHit);
			}
		}
		_statistics.ShapecastQueries++;
		return raycastInfo.AllHitCount > 0;
	}

	private void UpdateHits(KCCData data, KCCOverlapInfo extendedOverlapInfo, EKCCHitsOverlapQuery overlapQuery)
	{
		if (overlapQuery switch
		{
			EKCCHitsOverlapQuery.Default => extendedOverlapInfo?.AllWithinExtent() ?? false, 
			EKCCHitsOverlapQuery.Reuse => extendedOverlapInfo != null, 
			EKCCHitsOverlapQuery.New => false, 
			_ => throw new NotImplementedException("overlapQuery"), 
		})
		{
			_trackOverlapInfo.CopyFromOther(extendedOverlapInfo);
		}
		else
		{
			OverlapCapsule(_trackOverlapInfo, data, data.TargetPosition, _settings.Radius, _settings.Height, _settings.Extent, _settings.CollisionLayerMask, QueryTriggerInteraction.Collide);
			if (extendedOverlapInfo != null)
			{
				for (int i = 0; i < _trackOverlapInfo.AllHitCount; i++)
				{
					KCCOverlapHit kCCOverlapHit = _trackOverlapInfo.AllHits[i];
					for (int j = 0; j < extendedOverlapInfo.AllHitCount; j++)
					{
						KCCOverlapHit kCCOverlapHit2 = extendedOverlapInfo.AllHits[j];
						if ((object)kCCOverlapHit.Collider == kCCOverlapHit2.Collider)
						{
							kCCOverlapHit.CopyFromOther(kCCOverlapHit2);
						}
					}
				}
			}
		}
		data.Hits.Clear();
		int k = 0;
		for (int allHitCount = _trackOverlapInfo.AllHitCount; k < allHitCount; k++)
		{
			data.Hits.Add(_trackOverlapInfo.AllHits[k]);
		}
	}

	private void ForceRemoveAllHits(KCCData data)
	{
		_trackOverlapInfo.Reset(deep: false);
		data.Hits.Clear();
	}

	private void UpdateCollisions(KCCData data)
	{
		int num = 0;
		int num2 = 0;
		List<KCCCollision> all = data.Collisions.All;
		int i = 0;
		for (int count = all.Count; i < count; i++)
		{
			KCCCollision kCCCollision = all[i];
			_removeColliders[num2] = kCCCollision.Collider;
			_removeCollisions[num2] = kCCCollision;
			num2++;
		}
		KCCOverlapHit[] allHits = _trackOverlapInfo.AllHits;
		int j = 0;
		for (int allHitCount = _trackOverlapInfo.AllHitCount; j < allHitCount; j++)
		{
			Collider collider = allHits[j].Collider;
			bool flag = false;
			for (int k = 0; k < num2; k++)
			{
				if ((object)_removeColliders[k] == collider)
				{
					flag = true;
					num2--;
					_removeColliders[k] = _removeColliders[num2];
					_removeCollisions[k] = _removeCollisions[num2];
					break;
				}
			}
			if (!flag)
			{
				_addColliders[num] = collider;
				num++;
			}
		}
		for (int l = 0; l < num2; l++)
		{
			RemoveCollision(data, _removeCollisions[l], forceRemove: false);
		}
		for (int m = 0; m < num; m++)
		{
			AddCollision(data, _addColliders[m]);
		}
	}

	private void AddCollision(KCCData data, Collider collisionCollider)
	{
		GameObject gameObject = collisionCollider.gameObject;
		NetworkObject componentNoAlloc = gameObject.GetComponentNoAlloc<NetworkObject>();
		if (componentNoAlloc == null)
		{
			return;
		}
		IKCCInteractionProvider componentNoAlloc2 = gameObject.GetComponentNoAlloc<IKCCInteractionProvider>();
		if (componentNoAlloc2 != null && !componentNoAlloc2.CanStartInteraction(this, data))
		{
			return;
		}
		KCCCollision kCCCollision = data.Collisions.Add(componentNoAlloc, componentNoAlloc2, collisionCollider);
		if (kCCCollision.Processor != null)
		{
			OnProcessorAdded(data, kCCCollision.Processor);
		}
		if (OnCollisionEnter == null)
		{
			return;
		}
		try
		{
			OnCollisionEnter(this, kCCCollision);
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogException(exception);
		}
	}

	private void RemoveCollision(KCCData data, KCCCollision collision, bool forceRemove)
	{
		bool flag = true;
		IKCCInteractionProvider provider = collision.Provider;
		if (provider != null)
		{
			flag = provider.CanStopInteraction(this, data);
		}
		if (!flag && !forceRemove)
		{
			return;
		}
		if (OnCollisionExit != null)
		{
			try
			{
				OnCollisionExit(this, collision);
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
		}
		if (collision.Processor != null)
		{
			OnProcessorRemoved(data, collision.Processor);
		}
		data.Collisions.Remove(collision);
	}

	private void ForceRemoveAllCollisions(KCCData data)
	{
		List<KCCCollision> all = data.Collisions.All;
		while (all.Count > 0)
		{
			RemoveCollision(data, all[all.Count - 1], forceRemove: true);
		}
	}

	private void RemoveModifier(KCCData data, KCCModifier modifier, bool forceRemove)
	{
		bool flag = true;
		IKCCInteractionProvider provider = modifier.Provider;
		if (provider != null)
		{
			flag = provider.CanStopInteraction(this, data);
		}
		if (flag || forceRemove)
		{
			IKCCProcessor processor = modifier.Processor;
			if (data.Modifiers.Remove(modifier) && processor != null)
			{
				OnProcessorRemoved(data, processor);
			}
		}
	}

	private void ForceRemoveAllModifiers(KCCData data)
	{
		List<KCCModifier> all = data.Modifiers.All;
		while (all.Count > 0)
		{
			RemoveModifier(data, all[all.Count - 1], forceRemove: true);
		}
	}

	private void OnProcessorAdded(KCCData data, IKCCProcessor processor)
	{
		try
		{
			processor.OnEnter(this, data);
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogException(exception);
		}
	}

	private void OnProcessorRemoved(KCCData data, IKCCProcessor processor)
	{
		if (_activeStage != EKCCStage.None)
		{
			SuppressProcessor(processor);
		}
		IKCCProcessor[] cachedProcessors = _cachedProcessors;
		int i = 0;
		for (int cachedProcessorCount = _cachedProcessorCount; i < cachedProcessorCount; i++)
		{
			if (cachedProcessors[i] == processor)
			{
				cachedProcessors[i] = null;
				break;
			}
		}
		try
		{
			processor.OnExit(this, data);
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogException(exception);
		}
	}

	private void SynchronizeTransform(KCCData data, bool synchronizePosition, bool synchronizeRotation, bool useAntiJitter)
	{
		if (synchronizePosition)
		{
			_rigidbody.position = data.TargetPosition;
			Vector3 vector = data.TargetPosition;
			if (useAntiJitter && _activeFeatures.Has(EKCCFeature.AntiJitter) && !_settings.AntiJitterDistance.IsZero())
			{
				Vector3 vector2 = vector - _lastAntiJitterPosition;
				if (vector2.sqrMagnitude < _settings.TeleportThreshold)
				{
					vector = _lastAntiJitterPosition;
					float num = Mathf.Abs(vector2.y);
					if (num > 1E-06f)
					{
						vector.y += vector2.y * Mathf.Clamp01((num - _settings.AntiJitterDistance.y) / num);
					}
					Vector3 vector3 = vector2.OnlyXZ();
					float num2 = Vector3.Magnitude(vector3);
					if (num2 > 1E-06f)
					{
						vector += vector3 * Mathf.Clamp01((num2 - _settings.AntiJitterDistance.x) / num2);
					}
				}
				_lastAntiJitterPosition = vector;
			}
			if (synchronizeRotation)
			{
				_transform.SetPositionAndRotation(vector, data.TransformRotation);
			}
			else
			{
				_transform.position = vector;
			}
		}
		else if (synchronizeRotation)
		{
			_transform.rotation = data.TransformRotation;
		}
	}

	private PhysicsScene GetPhysicsScene()
	{
		if (_driver == EKCCDriver.Fusion)
		{
			return Runner.GetPhysicsScene();
		}
		Scene activeScene = SceneManager.GetActiveScene();
		if (activeScene.IsValid())
		{
			PhysicsScene physicsScene = activeScene.GetPhysicsScene();
			if (physicsScene.IsValid())
			{
				return physicsScene;
			}
		}
		return Physics.defaultPhysicsScene;
	}

	private bool IsValidHitCollider(KCCData data, Collider hitCollider)
	{
		if (hitCollider == _collider.Collider)
		{
			return false;
		}
		int i = 0;
		for (int count = _childColliders.Count; i < count; i++)
		{
			if (hitCollider == _childColliders[i])
			{
				return false;
			}
		}
		List<KCCIgnore> all = data.Ignores.All;
		int j = 0;
		for (int count2 = all.Count; j < count2; j++)
		{
			if (hitCollider == all[j].Collider)
			{
				return false;
			}
		}
		if (ResolveCollision != null)
		{
			try
			{
				return ResolveCollision(this, hitCollider);
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
		}
		return true;
	}

	private void RefreshCollider()
	{
		if (_settings.Shape == EKCCShape.None || _settings.Shape == EKCCShape.Void || (!_settings.SpawnColliderOnProxy && !HasAnyAuthority))
		{
			_collider.Destroy();
			return;
		}
		_settings.Radius = Mathf.Max(0.01f, _settings.Radius);
		_settings.Height = Mathf.Max(_settings.Radius * 2f, _settings.Height);
		_collider.Update(_transform, _settings);
	}

	private void RefreshUpdater()
	{
		if (_driver == EKCCDriver.Unity && !_hasManualUpdate)
		{
			if (_updater == null)
			{
				_updater = base.gameObject.AddComponent<KCCUpdater>();
				_updater.Initialize(OnFixedUpdateInternal, OnRenderUpdateInternal);
			}
		}
		else
		{
			DestroyUpdater();
		}
	}

	private void DestroyUpdater()
	{
		if (_updater != null)
		{
			_updater.Deinitialize();
			UnityEngine.Object.Destroy(_updater);
		}
		_updater = null;
	}

	private void SetDefaults()
	{
		DestroyUpdater();
		_debug.SetDefaults();
		_fixedData.Clear();
		_renderData.Clear();
		_historyData.Clear();
		_transientData.Clear();
		_extendedOverlapInfo.Reset(deep: true);
		_sharedOverlapInfo.Reset(deep: true);
		_trackOverlapInfo.Reset(deep: true);
		_raycastInfo.Reset(deep: true);
		_childColliders.Clear();
		_raycastHits.Clear();
		_hitColliders.Clear();
		_addColliders.Clear();
		_removeColliders.Clear();
		_removeCollisions.Clear();
		_stageProcessors.Clear();
		_cachedProcessors.Clear();
		_cachedProcessorStages.Clear();
		_localProcessors.Clear();
		_cachedProcessorCount = 0;
		_collider.Destroy();
		_rigidbody.isKinematic = true;
		_settings.CopyFromOther(_defaultSettings);
		_driver = EKCCDriver.None;
		_activeStage = EKCCStage.None;
		_activeFeatures = EKCCFeatures.None;
		_hasManualUpdate = false;
		_hasInputAuthority = false;
		_hasStateAuthority = false;
		_lastRenderTime = 0f;
		_lastRenderPosition = default;
		_lastRenderInitialization = 0;
		_lastFixedInitialization = 0;
		_lastAntiJitterPosition = default;
		_predictionError = default;
	}

	private void ProcessStage(EKCCStage stage, KCCData data, Action<IKCCProcessor, KCC, KCCData> method)
	{
		_activeStage = stage;
		bool flag = _debug.TraceStage == stage;
		if (flag)
		{
			_debug.ProcessorsStack.Clear();
		}
		Array.Copy(_cachedProcessors, _stageProcessors, _cachedProcessorCount);
		for (_stageProcessorIndex = 0; _stageProcessorIndex < _cachedProcessorCount; _stageProcessorIndex++)
		{
			if (_cachedProcessorStages[_stageProcessorIndex].Has(stage))
			{
				IKCCProcessor iKCCProcessor = _stageProcessors[_stageProcessorIndex];
				if (iKCCProcessor != null)
				{
					try
					{
						method(iKCCProcessor, this, data);
					}
					catch (Exception exception)
					{
						UnityEngine.Debug.LogException(exception);
					}
					if (flag)
					{
						_debug.ProcessorsStack.Add(iKCCProcessor);
					}
				}
			}
		}
		_activeStage = EKCCStage.None;
	}

	private void ProcessStage<T>(EKCCStage stage, KCCData data, Action<IKCCProcessor, KCC, KCCData, T> method, T userData)
	{
		_activeStage = stage;
		bool flag = _debug.TraceStage == stage;
		if (flag)
		{
			_debug.ProcessorsStack.Clear();
		}
		Array.Copy(_cachedProcessors, _stageProcessors, _cachedProcessorCount);
		for (_stageProcessorIndex = 0; _stageProcessorIndex < _cachedProcessorCount; _stageProcessorIndex++)
		{
			if (_cachedProcessorStages[_stageProcessorIndex].Has(stage))
			{
				IKCCProcessor iKCCProcessor = _stageProcessors[_stageProcessorIndex];
				if (iKCCProcessor != null)
				{
					try
					{
						method(iKCCProcessor, this, data, userData);
					}
					catch (Exception exception)
					{
						UnityEngine.Debug.LogException(exception);
					}
					if (flag)
					{
						_debug.ProcessorsStack.Add(iKCCProcessor);
					}
				}
			}
		}
		_activeStage = EKCCStage.None;
	}

	private void CacheProcessors(KCCData data)
	{
		_cachedProcessorCount = 0;
		List<IKCCProcessor> localProcessors = _localProcessors;
		int i = 0;
		for (int count = localProcessors.Count; i < count; i++)
		{
			IKCCProcessor iKCCProcessor = localProcessors[i];
			if (iKCCProcessor != null)
			{
				_cachedProcessors[_cachedProcessorCount] = iKCCProcessor;
				_cachedProcessorCount++;
			}
		}
		List<KCCModifier> all = data.Modifiers.All;
		int j = 0;
		for (int count2 = all.Count; j < count2; j++)
		{
			IKCCProcessor processor = all[j].Processor;
			if (processor != null)
			{
				_cachedProcessors[_cachedProcessorCount] = processor;
				_cachedProcessorCount++;
			}
		}
		List<KCCCollision> all2 = data.Collisions.All;
		int k = 0;
		for (int count3 = all2.Count; k < count3; k++)
		{
			IKCCProcessor processor2 = all2[k].Processor;
			if (processor2 != null)
			{
				_cachedProcessors[_cachedProcessorCount] = processor2;
				_cachedProcessorCount++;
			}
		}
		SortProcessors(_cachedProcessors, _cachedProcessorCount);
		for (int l = 0; l < _cachedProcessorCount; l++)
		{
			_cachedProcessorStages[l] = _cachedProcessors[l].GetValidStages(this, data);
		}
	}

	private void PublishFixedData()
	{
		_renderData.CopyFromOther(_fixedData);
		KCCData kCCData = _historyData[_fixedData.Tick % 60];
		if (kCCData == null)
		{
			kCCData = new KCCData();
			_historyData[_fixedData.Tick % 60] = kCCData;
		}
		kCCData.CopyFromOther(_fixedData);
	}

	private static void SortProcessors(IKCCProcessor[] processors, int count)
	{
		if (count <= 1)
		{
			return;
		}
		bool flag = false;
		while (!flag)
		{
			flag = true;
			int num = 0;
			int i = 1;
			IKCCProcessor iKCCProcessor = processors[num];
			for (; i < count; i++)
			{
				IKCCProcessor iKCCProcessor2 = processors[i];
				if (iKCCProcessor.Priority >= iKCCProcessor2.Priority)
				{
					iKCCProcessor = iKCCProcessor2;
				}
				else
				{
					processors[num] = iKCCProcessor2;
					processors[i] = iKCCProcessor;
					flag = false;
				}
				num++;
			}
		}
	}

	private static void SortProcessors<T>(IList<T> processors) where T : class
	{
		int count = processors.Count;
		if (count <= 1)
		{
			return;
		}
		bool flag = false;
		while (!flag)
		{
			flag = true;
			int num = 0;
			int i = 1;
			T val = processors[num];
			IKCCProcessor iKCCProcessor = (IKCCProcessor)val;
			for (; i < count; i++)
			{
				T val2 = processors[i];
				IKCCProcessor iKCCProcessor2 = (IKCCProcessor)val2;
				if (iKCCProcessor.Priority >= iKCCProcessor2.Priority)
				{
					val = val2;
					iKCCProcessor = iKCCProcessor2;
				}
				else
				{
					processors[num] = val2;
					processors[i] = val;
					flag = false;
				}
				num++;
			}
		}
	}

	public unsafe Vector3 ReadNetworkPosition(int* ptr)
	{
		return _defaultProperties.ReadPosition(ptr);
	}

	private int GetNetworkDataWordCount()
	{
		InitializeNetworkProperties();
		int num = 0;
		int i = 0;
		for (int num2 = _networkProperties.Length; i < num2; i++)
		{
			IKCCNetworkProperty iKCCNetworkProperty = _networkProperties[i];
			num += iKCCNetworkProperty.WordCount;
		}
		return num;
	}

	private unsafe void ReadNetworkData()
	{
		_networkContext.Data = _fixedData;
		int* ptr = Ptr;
		int i = 0;
		for (int num = _networkProperties.Length; i < num; i++)
		{
			IKCCNetworkProperty iKCCNetworkProperty = _networkProperties[i];
			iKCCNetworkProperty.Read(ptr);
			ptr += iKCCNetworkProperty.WordCount;
		}
	}

	private unsafe void WriteNetworkData()
	{
		_networkContext.Data = _fixedData;
		int* ptr = Ptr;
		int i = 0;
		for (int num = _networkProperties.Length; i < num; i++)
		{
			IKCCNetworkProperty iKCCNetworkProperty = _networkProperties[i];
			iKCCNetworkProperty.Write(ptr);
			ptr += iKCCNetworkProperty.WordCount;
		}
	}

	private unsafe void InterpolateNetworkData(float alpha = -1f)
	{
		if (_driver != EKCCDriver.Fusion || !GetInterpolationData(out var data))
		{
			return;
		}
		if (alpha >= 0f && alpha <= 1f)
		{
			data.Alpha = alpha;
		}
		int num = (int)data.ToTick - (int)data.FromTick;
		float num2 = (float)(int)data.FromTick + data.Alpha * (float)num;
		int* ptr = data.From;
		int* to = data.To;
		_networkContext.Data = _fixedData;
		_fixedData.Frame = Time.frameCount;
		_fixedData.Tick = Mathf.RoundToInt(num2);
		_fixedData.Alpha = data.Alpha;
		_fixedData.DeltaTime = Runner.DeltaTime;
		_fixedData.UnscaledDeltaTime = _fixedData.DeltaTime;
		_fixedData.Time = num2 * _fixedData.DeltaTime;
		int i = 0;
		for (int num3 = _networkProperties.Length; i < num3; i++)
		{
			IKCCNetworkProperty iKCCNetworkProperty = _networkProperties[i];
			iKCCNetworkProperty.Interpolate(data);
			data.From += iKCCNetworkProperty.WordCount;
			data.To += iKCCNetworkProperty.WordCount;
		}
		if (num > 0)
		{
			Vector3 vector = KCCNetworkUtility.ReadVector3(ptr, _defaultPositionReadAccuracy);
			Vector3 vector2 = KCCNetworkUtility.ReadVector3(to, _defaultPositionReadAccuracy);
			Vector3 vector3 = vector2 - vector;
			if (vector3.sqrMagnitude > _settings.TeleportThreshold * _settings.TeleportThreshold * (float)num * (float)num)
			{
				_fixedData.TargetPosition = vector2;
				_fixedData.RealVelocity = Vector3.zero;
				_fixedData.RealSpeed = 0f;
			}
			else
			{
				_fixedData.RealVelocity = vector3 / (_fixedData.DeltaTime * (float)num);
				_fixedData.RealSpeed = _fixedData.RealVelocity.magnitude;
			}
		}
		_renderData.CopyFromOther(_fixedData);
	}

	private void RestoreHistoryData(KCCData historyData)
	{
		if (_fixedData.IsGrounded)
		{
			_fixedData.IsGrounded = historyData.IsGrounded;
		}
	}

	private void InitializeNetworkProperties()
	{
		if (_defaultPositionReadAccuracy.IsNaN())
		{
			_defaultPositionReadAccuracy = new Accuracy("Position").Value;
		}
		if (_networkContext == null)
		{
			_networkContext = new KCCNetworkContext();
			_networkContext.KCC = this;
			_networkContext.Settings = _settings;
			_defaultProperties = new KCCNetworkProperties(_networkContext, _settings.PositionAccuracy, _settings.RotationAccuracy);
			List<IKCCNetworkProperty> list = new List<IKCCNetworkProperty>(32);
			list.Add(_defaultProperties);
			if (_settings.MaxNetworkedCollisions > 0)
			{
				list.Add(new KCCNetworkCollisions(_networkContext, _settings.MaxNetworkedCollisions));
			}
			if (_settings.MaxNetworkedModifiers > 0)
			{
				list.Add(new KCCNetworkModifiers(_networkContext, _settings.MaxNetworkedModifiers));
			}
			if (_settings.MaxNetworkedIgnores > 0)
			{
				list.Add(new KCCNetworkIgnores(_networkContext, _settings.MaxNetworkedIgnores));
			}
			_networkProperties = list.ToArray();
		}
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
		base.CopyBackingFieldsToState(P_0);
	}

	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	[NetworkRpcWeavedInvoker(1, 2, 1)]
	[Preserve]
	protected unsafe static void TeleportRPC_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		Vector3 vector = ReadWriteUtilsForWeaver.ReadVector3((int*)(data + num), 0.001f);
		num += 12;
		Vector3 position = vector;
		float num2 = (float)(*(int*)(data + num)) * 0.001f;
		num += 4;
		float lookPitch = num2;
		float num3 = (float)(*(int*)(data + num)) * 0.001f;
		num += 4;
		float lookYaw = num3;
		behaviour.InvokeRpc = true;
		((KCC)behaviour).TeleportRPC(position, lookPitch, lookYaw);
	}
}
