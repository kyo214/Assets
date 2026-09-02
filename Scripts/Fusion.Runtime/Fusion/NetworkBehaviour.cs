#define DEBUG
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace Fusion;

[ScriptHelp(BackColor = EditorHeaderBackColor.Blue, Icon = EditorHeaderIcon.FusionBlue)]
[HelpURL("https://doc.photonengine.com/fusion/current/manual/network-object#networkbehaviour")]
public abstract class NetworkBehaviour : SimulationBehaviour, ISpawned, IDespawned
{
	public delegate int[] InterestGroupsCallback(Type type, NetworkBehaviour behaviour);

	public enum InterpolationDataSources
	{
		Auto = 0,
		Snapshots = 1,
		Predicted = 2,
		NoInterpolation = 3
	}

	public struct RawInterpolator
	{
		private int _offset;

		private NetworkBehaviour _behaviour;

		public int Offset => _offset;

		public NetworkBehaviour Behaviour => _behaviour;

		public RawInterpolator(NetworkBehaviour behaviour, int offset)
		{
			_offset = offset;
			_behaviour = behaviour;
		}

		public unsafe bool TryGetValues(out void* from, out void* to, out float alpha, bool? force = null)
		{
			if (_behaviour.GetInterpolationData(out var data, force))
			{
				from = data.From + _offset;
				to = data.To + _offset;
				alpha = data.Alpha;
				return true;
			}
			from = null;
			to = null;
			alpha = 0f;
			return false;
		}

		public unsafe bool TryGetStruct<T>(out T from, out T to, out float alpha, bool? force = null) where T : unmanaged
		{
			if (TryGetValues(out var from2, out var to2, out alpha, force))
			{
				from = *(T*)from2;
				to = *(T*)to2;
				return true;
			}
			from = default;
			to = default;
			alpha = 0f;
			return false;
		}

		public unsafe bool TryGetArray<T>(NetworkArray<T> @base, out NetworkArray<T> from, out NetworkArray<T> to, out float alpha, bool? force = null)
		{
			if (TryGetValues(out var from2, out var to2, out alpha, force))
			{
				from = @base.Remap(from2);
				to = @base.Remap(to2);
				return true;
			}
			from = default;
			to = default;
			alpha = 0f;
			return false;
		}

		public unsafe bool TryGetDictionary<K, V>(NetworkDictionary<K, V> @base, out NetworkDictionary<K, V> from, out NetworkDictionary<K, V> to, out float alpha, bool? force = null)
		{
			if (TryGetValues(out var from2, out var to2, out alpha, force))
			{
				from = @base.Remap(from2);
				to = @base.Remap(to2);
				return true;
			}
			from = default;
			to = default;
			alpha = 0f;
			return false;
		}

		public unsafe bool TryGetLinkedList<T>(NetworkLinkedList<T> @base, out NetworkLinkedList<T> from, out NetworkLinkedList<T> to, out float alpha, bool? force = null)
		{
			if (TryGetValues(out var from2, out var to2, out alpha, force))
			{
				from = @base.Remap(from2);
				to = @base.Remap(to2);
				return true;
			}
			from = default;
			to = default;
			alpha = 0f;
			return false;
		}
	}

	public struct Interpolator<T> where T : struct
	{
		public unsafe delegate T InterpDelegate(int* from, int* to, float t, float a);

		public unsafe delegate T ReadDelegate(int* ptr, float a);

		private float _accuracy;

		private int _offset;

		private InterpDelegate _interpDelegate;

		private ReadDelegate _readDelegate;

		private NetworkBehaviour _behaviour;

		public int Offset => _offset;

		public NetworkBehaviour Behaviour => _behaviour;

		public InterpDelegate InterpolationDelegate
		{
			get
			{
				return _interpDelegate;
			}
			set
			{
				_interpDelegate = value;
			}
		}

		public ReadDelegate ReaderDelegate => _readDelegate;

		public unsafe T Value
		{
			get
			{
				if (_behaviour.GetInterpolationData(out var data))
				{
					return _interpDelegate(data.From + _offset, data.To + _offset, data.Alpha, _accuracy);
				}
				return default;
			}
		}

		public unsafe T? TryValue
		{
			get
			{
				InterpolationData data;
				return _behaviour.GetInterpolationData(out data) ? _interpDelegate(data.From + _offset, data.To + _offset, data.Alpha, _accuracy) : default(T);
			}
		}

		public Interpolator(NetworkBehaviour behaviour, InterpDelegate interpDelegate, ReadDelegate readDelegate, int offset, Accuracy accuracy)
		{
			_offset = offset;
			_interpDelegate = interpDelegate;
			_readDelegate = readDelegate;
			_behaviour = behaviour;
			_accuracy = ((accuracy._value == 0f) ? 0f : accuracy.Value);
		}

		public bool TryGetValues(out T from, out T to, out float alpha, bool? force = null)
		{
			(T, T, float)? values = GetValues(force);
			if (!values.HasValue)
			{
				from = default;
				to = default;
				alpha = 0f;
				return false;
			}
			(from, to, alpha) = values.Value;
			return true;
		}

		public unsafe (T from, T to, float alpha)? GetValues(bool? force = null)
		{
			if (_behaviour.GetInterpolationData(out var data, force))
			{
				return (_readDelegate(data.From + _offset, _accuracy), _readDelegate(data.To + _offset, _accuracy), data.Alpha);
			}
			return null;
		}
	}

	[NonSerialized]
	public unsafe int* Ptr;

	[NonSerialized]
	public int ObjectIndex;

	[NonSerialized]
	public bool InvokeRpc;

	[NonSerialized]
	internal RpcInvokeData[] RpcCache;

	[EditorDisabled(false)]
	[SerializeField]
	internal int WordOffset;

	[EditorDisabled(false)]
	[SerializeField]
	internal int WordCount;

	[InlineHelp]
	[SerializeField]
	internal InterpolationDataSources _interpolationDataSource;

	[NonSerialized]
	internal NetworkBehaviourCallbacks DynamicCallbacks;

	public (int offset, int count) WordInfo => (offset: WordOffset, count: WordCount);

	public InterpolationDataSources InterpolationDataSource
	{
		get
		{
			return _interpolationDataSource;
		}
		set
		{
			_interpolationDataSource = value;
		}
	}

	public NetworkBehaviourId Id => new NetworkBehaviourId
	{
		Object = (BehaviourUtils.IsAlive(Object) ? Object.Id : default(NetworkId)),
		Behaviour = ObjectIndex
	};

	public virtual int? DynamicWordCount => null;

	public virtual bool InvokeOnChangedForInitialNonZeroValues => true;

	public int GetLocalAuthorityMask()
	{
		if (BehaviourUtils.IsNotAlive(Runner))
		{
			return 0;
		}
		return AuthorityMasks.Create(HasStateAuthority, HasInputAuthority);
	}

	public unsafe void CopyStateFrom(NetworkBehaviour source)
	{
		if (GetType() == source.GetType())
		{
			Native.MemCpy(Ptr, source.Ptr, WordCount * 4);
		}
	}

	public override void FixedUpdateNetwork()
	{
	}

	[Preserve]
	public virtual void CopyBackingFieldsToState(bool firstTime)
	{
	}

	[Preserve]
	public virtual void CopyStateToBackingFields()
	{
	}

	public virtual void Spawned()
	{
	}

	public virtual void Despawned(NetworkRunner runner, bool hasState)
	{
	}

	public bool GetInterpolationData(out InterpolationData data, bool? predicted = null)
	{
		return Object.Runner.GetInterpolationData(this, predicted ?? IsInterpolationDataPredicted(), out data);
	}

	public bool GetInterpolationData(out InterpolationData data, out bool predicted)
	{
		predicted = IsInterpolationDataPredicted();
		return Object.Runner.GetInterpolationData(this, predicted, out data);
	}

	public virtual bool IsInterpolationDataPredicted()
	{
		return _interpolationDataSource switch
		{
			InterpolationDataSources.Auto => !Object.IsProxy && Object.InSimulation, 
			InterpolationDataSources.Snapshots => Object.HasStateAuthority, 
			InterpolationDataSources.Predicted => true, 
			_ => false, 
		};
	}

	public void OnChangeClearAll()
	{
		DynamicCallbacks?.ClearAll();
	}

	public bool OnChangeRemove(NetworkBehaviourCallbackReference reference)
	{
		return DynamicCallbacks != null && DynamicCallbacks.RemoveCallback(reference);
	}

	public unsafe NetworkBehaviourCallbackReference OnChangeAdd<T>(int wordOffset, int wordCount, ChangedDelegate<T> callback, OnChangedTargets targets = OnChangedTargets.All) where T : NetworkBehaviour
	{
		if (!typeof(T).IsAssignableFrom(GetType()))
		{
			Log.Error(this, typeof(T).Name + " is not cast-able to " + GetType().Name);
			return default;
		}
		if (DynamicCallbacks == null)
		{
			DynamicCallbacks = new NetworkBehaviourCallbacks(NetworkBehaviourUtils.GetWordCount(this));
		}
		if (!NetworkBehaviourUtils.HasStaticCallbacks(GetType()) && DynamicCallbacks.Count == 0)
		{
			Assert.Check(!Object.CallbackBehaviours.Contains(this));
			Object.CallbackBehaviours.Add(this);
		}
		return DynamicCallbacks.RegisterCallback(wordOffset, wordCount, (int)targets, (NetworkBehaviour behaviour, int* old) =>
		{
			int* ptr = behaviour.Ptr;
			Changed<T> changed = new Changed<T>((T)behaviour, old);
			try
			{
				callback(changed);
			}
			catch (Exception exn)
			{
				Log.Exception(this, exn);
			}
			finally
			{
				behaviour.Ptr = ptr;
			}
			return changed.ShouldRescan;
		});
	}

	public NetworkBehaviourCallbackReference OnChangeAdd<T>(string propertyName, ChangedDelegate<T> callback, OnChangedTargets targetses = OnChangedTargets.All) where T : NetworkBehaviour
	{
		if (!NetworkBehaviourUtils.HasStaticWordCount(GetType()))
		{
			Log.Error(this, "On change callbacks can only be added on NetworkBehaviours which are weaved");
			return default;
		}
		PropertyInfo property = GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		if (property == null)
		{
			Log.Error(this, "Could not find property name " + propertyName);
			return default;
		}
		NetworkedWeavedAttribute customAttribute = property.GetCustomAttribute<NetworkedWeavedAttribute>();
		if (customAttribute == null)
		{
			Log.Error(this, "Property " + propertyName + " is not a [Networked] property");
			return default;
		}
		return OnChangeAdd(customAttribute.WordOffset, customAttribute.WordCount, callback, targetses);
	}

	public T? GetInput<T>() where T : unmanaged, INetworkInput
	{
		if (BehaviourUtils.IsAlive(Object) && Object.InputAuthority.IsValid && BehaviourUtils.IsAlive(Object.Runner))
		{
			return Object.Runner.GetInputForPlayer<T>(Object.InputAuthority);
		}
		return null;
	}

	public bool GetInput<T>(out T input) where T : unmanaged, INetworkInput
	{
		if (BehaviourUtils.IsAlive(Object) && Object.InputAuthority.IsValid && BehaviourUtils.IsAlive(Object.Runner))
		{
			return Object.Runner.TryGetInputForPlayer<T>(Object.InputAuthority, out input);
		}
		input = default;
		return false;
	}

	private unsafe static bool InterpolateBoolean(int* from, int* to, float t, float a)
	{
		return *from == 1;
	}

	private unsafe static int InterpolateInt32(int* from, int* to, float t, float a)
	{
		return (int)Mathf.Lerp(*from, *to, a);
	}

	private unsafe static float InterpolateSingle(int* from, int* to, float t, float a)
	{
		return Mathf.Lerp(ReadWriteUtilsForWeaver.ReadFloat(from, a), ReadWriteUtilsForWeaver.ReadFloat(to, a), t);
	}

	private unsafe static Vector2 InterpolateVector2(int* from, int* to, float t, float a)
	{
		return Vector2.Lerp(ReadWriteUtilsForWeaver.ReadVector2(from, a), ReadWriteUtilsForWeaver.ReadVector2(to, a), t);
	}

	private unsafe static Vector3 InterpolateVector3(int* from, int* to, float t, float a)
	{
		return Vector3.Lerp(ReadWriteUtilsForWeaver.ReadVector3(from, a), ReadWriteUtilsForWeaver.ReadVector3(to, a), t);
	}

	private unsafe static Quaternion InterpolateQuaternion(int* from, int* to, float t, float a)
	{
		return Quaternion.Slerp(ReadWriteUtilsForWeaver.ReadQuaternion(from, a), ReadWriteUtilsForWeaver.ReadQuaternion(to, a), t);
	}

	private static Interpolator<T>.InterpDelegate GetInterpolatorDelegate<T>() where T : struct
	{
		if (IsValidInterpolatorType(typeof(T)))
		{
			return (Interpolator<T>.InterpDelegate)Delegate.CreateDelegate(typeof(Interpolator<T>.InterpDelegate), typeof(NetworkBehaviour).GetMethod("Interpolate" + typeof(T).Name, BindingFlags.Static | BindingFlags.NonPublic));
		}
		throw new NotSupportedException("Can't interpolate " + typeof(T).FullName);
	}

	private static bool IsValidInterpolatorType(Type t)
	{
		return t == typeof(bool) || t == typeof(int) || t == typeof(float) || t == typeof(Vector2) || t == typeof(Vector3) || t == typeof(Quaternion);
	}

	public RawInterpolator GetInterpolator(string propertyName)
	{
		PropertyInfo property = GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		if (property == null)
		{
			Log.Error(this, "Could not find property name " + propertyName);
			return default;
		}
		NetworkedWeavedAttribute customAttribute = property.GetCustomAttribute<NetworkedWeavedAttribute>();
		if (customAttribute == null)
		{
			Log.Error(this, "Property " + propertyName + " is not a [Networked] property");
			return default;
		}
		return new RawInterpolator(this, customAttribute.WordOffset);
	}

	public Interpolator<T> GetInterpolator<T>(string propertyName) where T : struct
	{
		PropertyInfo property = GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		if (property == null)
		{
			Log.Error(this, "Could not find property name " + propertyName);
			return default;
		}
		NetworkedWeavedAttribute customAttribute = property.GetCustomAttribute<NetworkedWeavedAttribute>();
		if (customAttribute == null)
		{
			Log.Error(this, "Property " + propertyName + " is not a [Networked] property");
			return default;
		}
		if (property.PropertyType != typeof(T))
		{
			Log.Error(this, "Property " + propertyName + " is not a of type " + typeof(T).FullName + " property");
			return default;
		}
		AccuracyAttribute customAttribute2 = property.GetCustomAttribute<AccuracyAttribute>();
		Accuracy accuracy;
		if (customAttribute2 == null)
		{
			accuracy = AccuracyDefaults.Default;
		}
		else
		{
			int customHash = customAttribute2.CustomHash;
			accuracy = ((customHash != 0) ? Runner.Config.AccuracyDefaults.GetAccuracyOrThrow(Runner.Config.AccuracyDefaults.GetNameFromHash(customHash)) : ((Accuracy)customAttribute2.Accuracy));
		}
		Interpolator<T>.ReadDelegate readDelegate = (Interpolator<T>.ReadDelegate)Delegate.CreateDelegate(typeof(Interpolator<T>.ReadDelegate), typeof(ReadWriteUtilsForWeaver).GetMethod("Read" + typeof(T).Name));
		return new Interpolator<T>(this, GetInterpolatorDelegate<T>(), readDelegate, customAttribute.WordOffset, accuracy);
	}

	[NetworkSerializeMethod(MaxSize = 8)]
	public unsafe static int NetworkSerialize(NetworkRunner runner, NetworkBehaviour obj, byte* data)
	{
		if (BehaviourUtils.IsNull(obj))
		{
			*(NetworkId*)data = default;
			return 4;
		}
		NetworkObject networkObject = obj.Object;
		if ((object)networkObject != null)
		{
			_ = networkObject.Id;
			if (0 == 0)
			{
				*(NetworkId*)data = obj.Object.Id;
				((int*)data)[1] = obj.ObjectIndex;
				return 8;
			}
		}
		NetworkBehaviourUtils.NotifyNetworkWrapFailed(obj);
		*(NetworkId*)data = default;
		return 4;
	}

	[NetworkDeserializeMethod]
	public unsafe static int NetworkDeserialize(NetworkRunner runner, byte* data, ref NetworkBehaviour result)
	{
		NetworkBehaviourId wrapper = new NetworkBehaviourId
		{
			Object = *(NetworkId*)data
		};
		int num = 4;
		if (wrapper.Object == default(NetworkId))
		{
			result = null;
			return num;
		}
		wrapper.Behaviour = ((int*)data)[1];
		num += 4;
		if (runner.TryFindObject(wrapper.Object, out var obj))
		{
			if (wrapper.Behaviour >= 0 && wrapper.Behaviour < obj.NetworkedBehaviours.Length)
			{
				result = obj.NetworkedBehaviours[wrapper.Behaviour];
			}
			else
			{
				result = null;
				NetworkBehaviourUtils.NotifyNetworkUnwrapFailed(wrapper, typeof(NetworkBehaviour));
			}
		}
		else
		{
			result = null;
		}
		return num;
	}

	public static NetworkBehaviourId NetworkWrap(NetworkRunner runner, NetworkBehaviour obj)
	{
		if (BehaviourUtils.IsNull(obj))
		{
			return default;
		}
		return new NetworkBehaviourId
		{
			Object = (obj.Object?.Id ?? default(NetworkId)),
			Behaviour = obj.ObjectIndex
		};
	}

	public static NetworkBehaviour NetworkUnwrap(NetworkRunner runner, NetworkBehaviourId wrapper)
	{
		if (!wrapper.IsValid)
		{
			return null;
		}
		if (!runner.TryFindBehaviour(wrapper, out var behaviour))
		{
			NetworkBehaviourUtils.NotifyNetworkUnwrapFailed(wrapper, typeof(NetworkBehaviour));
		}
		return behaviour;
	}

	public static ref T MakeRef<T>() where T : unmanaged
	{
		throw new NotImplementedException("This is a special method that is meant to be used only for [Networked] properties inline initialization.");
	}

	public static ref T MakeRef<T>(T defaultValue) where T : unmanaged
	{
		throw new NotImplementedException("This is a special method that is meant to be used only for [Networked] properties inline initialization.");
	}

	public unsafe static T* MakePtr<T>() where T : unmanaged
	{
		throw new NotImplementedException("This is a special method that is meant to be used only for [Networked] properties inline initialization.");
	}

	public unsafe static T* MakePtr<T>(T defaultValue) where T : unmanaged
	{
		throw new NotImplementedException("This is a special method that is meant to be used only for [Networked] properties inline initialization.");
	}

	public static NetworkBehaviourUtils.ArrayInitializer<T> MakeInitializer<T>(T[] array)
	{
		throw new NotImplementedException("This is a special method that is meant to be used only for [Networked] properties inline initialization.");
	}

	public static NetworkBehaviourUtils.DictionaryInitializer<K, V> MakeInitializer<K, V>(Dictionary<K, V> dictionary)
	{
		throw new NotImplementedException("This is a special method that is meant to be used only for [Networked] properties inline initialization.");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator NetworkBehaviourId(NetworkBehaviour behaviour)
	{
		if (BehaviourUtils.IsNull(behaviour) || BehaviourUtils.IsNotAlive(behaviour.Runner))
		{
			return default;
		}
		return NetworkWrap(behaviour.Runner, behaviour);
	}

	protected internal static void InvokeWeavedCode()
	{
	}
}
