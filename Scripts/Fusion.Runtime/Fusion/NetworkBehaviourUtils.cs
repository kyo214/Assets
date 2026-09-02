#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Fusion;

public static class NetworkBehaviourUtils
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ArrayInitializer<T>
	{
		public static implicit operator NetworkArray<T>(ArrayInitializer<T> arr)
		{
			throw new NotImplementedException("This is a special method that is meant to be used only for [Networked] properties inline initialization.");
		}

		public static implicit operator NetworkLinkedList<T>(ArrayInitializer<T> arr)
		{
			throw new NotImplementedException("This is a special method that is meant to be used only for [Networked] properties inline initialization.");
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct DictionaryInitializer<K, V>
	{
		public static implicit operator NetworkDictionary<K, V>(DictionaryInitializer<K, V> arr)
		{
			throw new NotImplementedException("This is a special method that is meant to be used only for [Networked] properties inline initialization.");
		}
	}

	private static Dictionary<string, int> _interestGroups2Keys = new Dictionary<string, int>();

	private static Dictionary<int, string> _interestKeys2Groups = new Dictionary<int, string>();

	private static Dictionary<Type, NetworkBehaviour.InterestGroupsCallback> _interestGroups = new Dictionary<Type, NetworkBehaviour.InterestGroupsCallback>();

	private static Dictionary<Type, int> _wordCounts = new Dictionary<Type, int>();

	private static Dictionary<Type, RpcInvokeData[]> _invokerDelegates = new Dictionary<Type, RpcInvokeData[]>();

	private static Dictionary<Type, NetworkBehaviourCallbacks> _staticCallbacks = new Dictionary<Type, NetworkBehaviourCallbacks>();

	private static SortedList<string, RpcStaticInvokeDelegate> _staticInvokers = new SortedList<string, RpcStaticInvokeDelegate>();

	public static bool InvokeRpc = false;

	internal static int InterestGroupKeysMax => _interestGroups2Keys.Count;

	internal static void ResetStatics()
	{
		InvokeRpc = false;
		_interestGroups2Keys.Clear();
		_interestKeys2Groups.Clear();
		_interestGroups.Clear();
		_wordCounts.Clear();
		_invokerDelegates.Clear();
		_staticCallbacks.Clear();
		_staticInvokers.Clear();
	}

	public static bool HasStaticCallbacks(Type type)
	{
		NetworkBehaviourCallbacks value;
		return _staticCallbacks.TryGetValue(type, out value);
	}

	public static bool GetStaticCallbacks(Type type, out NetworkBehaviourCallbacks nbc)
	{
		return _staticCallbacks.TryGetValue(type, out nbc);
	}

	public static NetworkBehaviourCallbacks GetStaticCallbacks(Type type)
	{
		NetworkBehaviourCallbacks value;
		return _staticCallbacks.TryGetValue(type, out value) ? value : null;
	}

	public static IEnumerable<string> GetAllInterestGroupOnObject(NetworkObject obj)
	{
		NetworkBehaviour[] networkedBehaviours = obj.NetworkedBehaviours;
		foreach (NetworkBehaviour behaviour in networkedBehaviours)
		{
			Type currentType = behaviour.GetType();
			while (currentType != typeof(NetworkBehaviour))
			{
				PropertyInfo[] properties = currentType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (PropertyInfo prop in properties)
				{
					if (!(prop.DeclaringType != currentType))
					{
						NetworkedAttribute attr = prop.GetCustomAttribute<NetworkedAttribute>();
						NetworkedWeavedAttribute attrWeaved = prop.GetCustomAttribute<NetworkedWeavedAttribute>();
						if (attr?.Group != null && attrWeaved != null && attrWeaved.WordCount > 0)
						{
							yield return attr.Group;
						}
					}
				}
				currentType = currentType.BaseType;
				Assert.Check(typeof(NetworkBehaviour).IsAssignableFrom(currentType));
			}
		}
	}

	internal static string TryGetInterestGroupFromKey(int key)
	{
		string value;
		return _interestKeys2Groups.TryGetValue(key, out value) ? value : null;
	}

	internal static int GetnterestGroupKeyFromGroup(string group)
	{
		return _interestGroups2Keys[group];
	}

	internal static bool TryGetInterestGroupKeyFromGroup(string group, out int key)
	{
		return _interestGroups2Keys.TryGetValue(group, out key);
	}

	internal static int GetOrAddInterestGroupKey(string group)
	{
		if (!_interestGroups2Keys.TryGetValue(group, out var value))
		{
			_interestGroups2Keys.Add(group, value = _interestGroups2Keys.Count + 1);
			_interestKeys2Groups.Add(value, group);
			Log.Debug($"Interest Group '{group}' assigned key '{value}'");
		}
		return value;
	}

	internal static bool TryGetInterestGroupProvider(Type type, out NetworkBehaviour.InterestGroupsCallback provider)
	{
		return _interestGroups.TryGetValue(type, out provider);
	}

	internal static void RegisterInterestGroups(Type type)
	{
		if (typeof(NetworkBehaviour) == type || !typeof(NetworkBehaviour).IsAssignableFrom(type) || _interestGroups.ContainsKey(type))
		{
			return;
		}
		MethodInfo method = type.GetMethod("InterestGroupsProvider", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		if (method != null)
		{
			_interestGroups.Add(type, (NetworkBehaviour.InterestGroupsCallback)Delegate.CreateDelegate(typeof(NetworkBehaviour.InterestGroupsCallback), method));
			return;
		}
		if (!HasStaticWordCount(type))
		{
			_interestGroups.Add(type, null);
			return;
		}
		int[] groups = new int[GetStaticWordCount(type)];
		bool flag = false;
		HashSet<int> hashSet = new HashSet<int>();
		Type type2 = type;
		while (type2 != typeof(NetworkBehaviour))
		{
			PropertyInfo[] properties = type2.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.DeclaringType != type2)
				{
					continue;
				}
				NetworkedAttribute customAttribute = propertyInfo.GetCustomAttribute<NetworkedAttribute>();
				NetworkedWeavedAttribute customAttribute2 = propertyInfo.GetCustomAttribute<NetworkedWeavedAttribute>();
				if (customAttribute != null && customAttribute.Group != null && customAttribute2 != null && customAttribute2.WordCount > 0)
				{
					flag = true;
					int orAddInterestGroupKey = GetOrAddInterestGroupKey(customAttribute.Group);
					for (int j = 0; j < customAttribute2.WordCount; j++)
					{
						Assert.Check(groups[customAttribute2.WordOffset + j] == 0);
						groups[customAttribute2.WordOffset + j] = orAddInterestGroupKey;
						hashSet.Add(orAddInterestGroupKey);
					}
				}
			}
			type2 = type2.BaseType;
			Assert.Check(typeof(NetworkBehaviour).IsAssignableFrom(type2));
		}
		if (flag)
		{
			Log.Debug(type.FullName + " InterestGroups: " + string.Join(", ", hashSet.Select(TryGetInterestGroupFromKey)));
			_interestGroups.Add(type, (Type type3, NetworkBehaviour __) => groups);
		}
		else
		{
			_interestGroups.Add(type, null);
		}
	}

	public static void RegisterStaticCallbacks(Type type)
	{
		if (typeof(NetworkBehaviour) == type)
		{
			return;
		}
		if (!typeof(NetworkBehaviour).IsAssignableFrom(type))
		{
			Assert.Fail();
		}
		else
		{
			if (!HasStaticWordCount(type) || _staticCallbacks.ContainsKey(type))
			{
				return;
			}
			NetworkBehaviourCallbacks networkBehaviourCallbacks = new NetworkBehaviourCallbacks(GetStaticWordCount(type));
			bool flag = false;
			Type type2 = type;
			while (type2 != typeof(NetworkBehaviour))
			{
				PropertyInfo[] properties = type2.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (PropertyInfo propertyInfo in properties)
				{
					if (propertyInfo.DeclaringType != type2)
					{
						continue;
					}
					NetworkedAttribute customAttribute = propertyInfo.GetCustomAttribute<NetworkedAttribute>();
					NetworkedWeavedAttribute customAttribute2 = propertyInfo.GetCustomAttribute<NetworkedWeavedAttribute>();
					if (customAttribute == null || customAttribute2 == null || customAttribute.OnChanged == null)
					{
						continue;
					}
					MethodInfo method = type2.GetMethod(customAttribute.OnChanged, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
					if (method != null)
					{
						flag = true;
						Type type3 = method.GetParameters()[0].ParameterType.GetGenericArguments()[0];
						MethodInfo method2 = typeof(NetworkBehaviourCallbacks<>).MakeGenericType(type3).GetMethod("RegisterCallback");
						Type type4 = typeof(ChangedDelegate<>).MakeGenericType(type3);
						Delegate obj = Delegate.CreateDelegate(type4, method);
						method2.Invoke(null, new object[5]
						{
							networkBehaviourCallbacks,
							customAttribute2.WordOffset,
							customAttribute2.WordCount,
							(int)customAttribute.OnChangedTargets,
							obj
						});
						Log.Debug("OnChange: " + customAttribute.OnChanged + " registered for " + type2.FullName + "." + propertyInfo.Name + " (" + type.FullName + ")");
					}
					else
					{
						MethodInfo method3 = type2.GetMethod(customAttribute.OnChanged, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
						if (method3 == null)
						{
							Log.Error("OnChange: " + customAttribute.OnChanged + " not found, set on " + type2.FullName + "." + propertyInfo.Name + " (" + type.FullName + ")");
						}
						else
						{
							Log.Error("OnChange: " + customAttribute.OnChanged + " is not static, set on " + type2.FullName + "." + propertyInfo.Name + " (" + type.FullName + ")");
						}
					}
				}
				type2 = type2.BaseType;
				Assert.Check(typeof(NetworkBehaviour).IsAssignableFrom(type2));
			}
			if (flag)
			{
				_staticCallbacks.Add(type, networkBehaviourCallbacks);
			}
		}
	}

	private static NetworkBehaviourWeavedAttribute GetWeavedAttributeOrThrow(Type type)
	{
		try
		{
			return type.GetCustomAttributeOrThrow<NetworkBehaviourWeavedAttribute>(inherit: false);
		}
		catch (ArgumentOutOfRangeException)
		{
			throw new InvalidOperationException(string.Format("Type {0} has not been weaved. Has the assembly {1} been added to {2}?", type, type.Assembly.GetName().Name, "NetworkProjectConfig"));
		}
	}

	public static int GetWordCount(NetworkBehaviour behaviour)
	{
		int? dynamicWordCount = behaviour.DynamicWordCount;
		if (dynamicWordCount.HasValue)
		{
			Assert.Check<string, int, BehaviourUtils.DumpDeferred>(dynamicWordCount.Value >= 0, "DynamicWordCount returned a negative value", dynamicWordCount.Value, BehaviourUtils.GetDump(behaviour));
			return dynamicWordCount.Value;
		}
		int staticWordCount = GetStaticWordCount(behaviour.GetType());
		Assert.Check<string, int, BehaviourUtils.DumpDeferred>(staticWordCount >= 0, "GetStaticWordCount returned a negative value", staticWordCount, BehaviourUtils.GetDump(behaviour));
		return staticWordCount;
	}

	public static bool HasStaticWordCount(Type type)
	{
		Assert.Check(typeof(NetworkBehaviour).IsAssignableFrom(type));
		return GetWeavedAttributeOrThrow(type).WordCount >= 0;
	}

	public static int GetStaticWordCount(Type type)
	{
		Assert.Check(typeof(NetworkBehaviour).IsAssignableFrom(type));
		if (!_wordCounts.TryGetValue(type, out var value))
		{
			NetworkBehaviourWeavedAttribute weavedAttributeOrThrow = GetWeavedAttributeOrThrow(type);
			Assert.Check(weavedAttributeOrThrow.WordCount >= 0);
			_wordCounts.Add(type, value = weavedAttributeOrThrow.WordCount);
		}
		return value;
	}

	public static bool ShouldRegisterRpcInvokeDelegates(Type type)
	{
		return !_invokerDelegates.ContainsKey(type);
	}

	public static void RegisterRpcInvokeDelegates(Type type)
	{
		if (!ShouldRegisterRpcInvokeDelegates(type))
		{
			return;
		}
		List<RpcInvokeData> list = new List<RpcInvokeData>();
		list.Add(default);
		MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		MethodInfo[] array = methods;
		foreach (MethodInfo methodInfo in array)
		{
			object[] customAttributes = methodInfo.GetCustomAttributes(typeof(NetworkRpcWeavedInvokerAttribute), inherit: false);
			if (customAttributes.Length != 0 && customAttributes[0] is NetworkRpcWeavedInvokerAttribute networkRpcWeavedInvokerAttribute)
			{
				list.Add(new RpcInvokeData
				{
					Key = networkRpcWeavedInvokerAttribute.Key,
					Sources = networkRpcWeavedInvokerAttribute.Sources,
					Targets = networkRpcWeavedInvokerAttribute.Targets,
					Delegate = (RpcInvokeDelegate)Delegate.CreateDelegate(typeof(RpcInvokeDelegate), methodInfo)
				});
			}
			if (methodInfo.DeclaringType == type)
			{
				object[] customAttributes2 = methodInfo.GetCustomAttributes(typeof(NetworkRpcStaticWeavedInvokerAttribute), inherit: false);
				if (customAttributes2.Length != 0 && customAttributes2[0] is NetworkRpcStaticWeavedInvokerAttribute networkRpcStaticWeavedInvokerAttribute)
				{
					_staticInvokers.Add(networkRpcStaticWeavedInvokerAttribute.Key, (RpcStaticInvokeDelegate)Delegate.CreateDelegate(typeof(RpcStaticInvokeDelegate), methodInfo));
				}
			}
		}
		list.Sort((RpcInvokeData a, RpcInvokeData b) => a.Key.CompareTo(b.Key));
		_invokerDelegates?.Add(type, list.ToArray());
	}

	public static bool TryGetRpcInvokeDelegateArray(Type type, out RpcInvokeData[] delegates)
	{
		return _invokerDelegates.TryGetValue(type, out delegates);
	}

	public static int GetRpcStaticIndexOrThrow(string key)
	{
		int num = _staticInvokers.IndexOfKey(key);
		if (num < 0)
		{
			throw new ArgumentOutOfRangeException("Static RPC not found: " + key);
		}
		return num;
	}

	public static bool TryGetRpcStaticInvokeDelegate(int index, out RpcStaticInvokeDelegate del)
	{
		if (index >= 0 && index < _staticInvokers.Count)
		{
			del = _staticInvokers.Values[index];
			return true;
		}
		del = null;
		return false;
	}

	public static void NotifyRpcPayloadSizeExceeded(string rpc, int size)
	{
		Log.Error($"{rpc}: payload is too large ({size} bytes). Max allowed: {512} bytes)");
	}

	public static void NotifyRpcTargetUnreachable(PlayerRef player, string rpc)
	{
		Log.Error($"{rpc}: target {player} not reachable.");
	}

	public static void NotifyLocalSimulationNotAllowedToSendRpc(string rpc, NetworkObject obj, int sources)
	{
		Log.Error(rpc + ": Local simulation is not allowed to send this RPC on " + obj.Name + ".");
	}

	public static void NotifyLocalTargetedRpcCulled(PlayerRef player, string methodName)
	{
		Log.Warn($"{methodName} culled for target {player}: player is local and InvokeLocal is set to false");
	}

	public static void ThrowIfBehaviourNotInitialized(NetworkBehaviour behaviour)
	{
		if (BehaviourUtils.IsNotAlive(behaviour.Object))
		{
			throw new InvalidOperationException("Behaviour not initialized: Object not set.");
		}
		if (BehaviourUtils.IsNotAlive(behaviour.Runner))
		{
			throw new InvalidOperationException("Behaviour not initialized: Runner not set.");
		}
	}

	public static void NotifyNetworkWrapFailed<T>(T value)
	{
		Log.Warn($"Failed to wrap {value}");
	}

	public static void NotifyNetworkWrapFailed<T>(T value, Type wrapperType)
	{
		Log.Warn($"Failed to wrap {value} as {wrapperType}");
	}

	public static void NotifyNetworkUnwrapFailed<T>(T wrapper, Type valueType)
	{
		Log.Warn($"Failed to unwrap {wrapper} to {valueType}");
	}

	public static void InitializeNetworkArray<T>(NetworkArray<T> networkArray, T[] sourceArray, string name) where T : unmanaged
	{
		int num = ((sourceArray != null) ? sourceArray.Length : 0);
		if (num != 0)
		{
			if (networkArray.Length < num)
			{
				Log.Error($"Source array is too long for {name} with capacity of {networkArray.Length}: {num}. Ignoring extra elements.");
				num = networkArray.Length;
			}
			networkArray.CopyFrom(sourceArray, 0, num);
		}
	}

	public static void CopyFromNetworkArray<T>(NetworkArray<T> networkArray, ref T[] dstArray) where T : unmanaged
	{
		if (dstArray?.Length != networkArray.Length)
		{
			dstArray = new T[networkArray.Length];
		}
		networkArray.CopyTo(dstArray);
	}

	public static void InitializeNetworkList<T>(NetworkLinkedList<T> networkList, T[] sourceArray, string name) where T : unmanaged
	{
		int num = ((sourceArray != null) ? sourceArray.Length : 0);
		if (num != 0)
		{
			if (networkList.Capacity < num)
			{
				Log.Error($"Source array is too long for {name} with capacity of {networkList.Capacity}: {num}. Ignoring extra elements.");
				num = networkList.Capacity;
			}
			networkList.Clear();
			for (int i = 0; i < num; i++)
			{
				networkList.Add(sourceArray[i]);
			}
		}
	}

	public static void CopyFromNetworkList<T>(NetworkLinkedList<T> networkList, ref T[] dstArray) where T : unmanaged
	{
		if (dstArray?.Length != networkList.Count)
		{
			dstArray = new T[networkList.Count];
		}
		int num = 0;
		foreach (T item in networkList)
		{
			dstArray[num++] = item;
		}
	}

	public static void InitializeNetworkDictionary<D, K, V>(NetworkDictionary<K, V> networkDictionary, D dictionary, string name) where D : IDictionary<K, V> where K : unmanaged where V : unmanaged
	{
		int num = dictionary?.Count ?? 0;
		if (num == 0)
		{
			return;
		}
		if (num > networkDictionary.Capacity)
		{
			Log.Error($"Source dictionary is too long for {name} with capacity of {networkDictionary.Capacity}: {num}. Ignoring extra elements.");
			num = networkDictionary.Capacity;
		}
		networkDictionary.Clear();
		foreach (KeyValuePair<K, V> item in dictionary)
		{
			if (--num < 0)
			{
				break;
			}
			networkDictionary.Add(item.Key, item.Value);
		}
	}

	public static void CopyFromNetworkDictionary<D, K, V>(NetworkDictionary<K, V> networkDictionary, ref D dictionary) where D : IDictionary<K, V>, new() where K : unmanaged where V : unmanaged
	{
		if (dictionary == null)
		{
			dictionary = new D();
		}
		else
		{
			dictionary.Clear();
		}
		foreach (KeyValuePair<K, V> item in networkDictionary)
		{
			dictionary.Add(item.Key, item.Value);
		}
	}

	public static SerializableDictionary<K, V> MakeSerializableDictionary<K, V>(Dictionary<K, V> dictionary) where K : unmanaged where V : unmanaged
	{
		return SerializableDictionary<K, V>.Wrap(dictionary);
	}
}
