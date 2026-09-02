#define DEBUG
#define ENABLE_PROFILER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Fusion;

internal class SimulationBehaviourUpdater
{
	internal class BehaviourList : ILogBuilder
	{
		public Type Type;

		public SimulationModes Modes;

		public SimulationStages Stages;

		public SimulationBehaviour Head;

		public SimulationBehaviour Tail;

		public int LockCount;

		public List<SimulationBehaviour> PendingRemovals;

		public void AddAfter(SimulationBehaviour item, SimulationBehaviour after)
		{
			Assert.Check(IsInList(after));
			Assert.Check(!IsInList(item));
			Assert.Check((item.Flags & SimulationBehaviourFlags.PendingRemoval) == 0);
			if (BehaviourUtils.IsSame(after, Tail))
			{
				AddLast(item);
			}
			else
			{
				Assert.Check(BehaviourUtils.IsNotNull(after.Next));
				item.Next = after.Next;
				item.Prev = after;
				after.Next.Prev = item;
				after.Next = item;
			}
			Assert.Check(IsInList(after));
			Assert.Check(IsInList(item));
		}

		public void AddFirst(SimulationBehaviour item)
		{
			Assert.Check(!IsInList(item));
			item.Next = Head;
			item.Prev = null;
			if (BehaviourUtils.IsNotNull(Head))
			{
				Head.Prev = item;
				Head = item;
			}
			else
			{
				Head = item;
				Tail = item;
			}
		}

		public void AddLast(SimulationBehaviour item)
		{
			Assert.Check(BehaviourUtils.IsNull(item.Prev));
			Assert.Check(BehaviourUtils.IsNull(item.Next));
			Assert.Check(!IsInList(item));
			Assert.Check((item.Flags & SimulationBehaviourFlags.PendingRemoval) == 0);
			item.Next = null;
			item.Prev = Tail;
			if (BehaviourUtils.IsNotNull(Tail))
			{
				Tail.Next = item;
				Tail = item;
			}
			else
			{
				Head = item;
				Tail = item;
			}
		}

		public void RemoveAllPending()
		{
			Assert.Check(LockCount == 0);
			if (PendingRemovals == null || PendingRemovals.Count == 0)
			{
				return;
			}
			foreach (SimulationBehaviour pendingRemoval in PendingRemovals)
			{
				Remove(pendingRemoval);
			}
			PendingRemovals.Clear();
		}

		public void PendingRemove(SimulationBehaviour item)
		{
			Assert.Check(IsInList(item));
			Assert.Check(LockCount > 0);
			if (PendingRemovals == null)
			{
				PendingRemovals = new List<SimulationBehaviour>();
			}
			PendingRemovals.Add(item);
		}

		public void Remove(SimulationBehaviour item)
		{
			if (IsInList(item))
			{
				if (BehaviourUtils.IsNotNull(item.Prev))
				{
					item.Prev.Next = item.Next;
				}
				if (BehaviourUtils.IsNotNull(item.Next))
				{
					item.Next.Prev = item.Prev;
				}
				if (BehaviourUtils.IsSame(item, Tail))
				{
					Tail = item.Prev;
				}
				if (BehaviourUtils.IsSame(item, Head))
				{
					Head = item.Next;
				}
				item.Prev = null;
				item.Next = null;
				item.Flags &= ~SimulationBehaviourFlags.PendingRemoval;
			}
		}

		public bool IsInList(SimulationBehaviour item)
		{
			SimulationBehaviour simulationBehaviour = Head;
			while (BehaviourUtils.IsNotNull(simulationBehaviour))
			{
				if (BehaviourUtils.IsSame(simulationBehaviour, item))
				{
					return true;
				}
				simulationBehaviour = simulationBehaviour.Next;
			}
			return false;
		}

		void ILogBuilder.BuildLogMessage(StringBuilder builder, string message, in LogOptions options)
		{
			builder.Append(message);
			builder.Append(" [Type: ").Append(Type.Name).Append(", List: ");
			SimulationBehaviour simulationBehaviour = Head;
			while (!BehaviourUtils.IsNull(simulationBehaviour))
			{
				if (!BehaviourUtils.IsSame(simulationBehaviour, Head))
				{
					builder.Append("->");
				}
				if (!simulationBehaviour.CanReceiveCallback)
				{
					builder.Append("[x]");
				}
				builder.Append(BehaviourUtils.GetName(simulationBehaviour));
				simulationBehaviour = simulationBehaviour.Next;
			}
			builder.Append("]");
		}
	}

	private readonly Dictionary<Type, BehaviourList> _byTypeLookup;

	private readonly Dictionary<Type, (SimulationBehaviour[], Type[])> _byTypeHierarchy;

	private readonly List<BehaviourList> _inOrderList;

	private readonly Dictionary<Type, List<BehaviourList>> _inOrderByInterfaceList;

	private readonly HashSet<Type> _behavioursChecked;

	private readonly List<SimulationBehaviour> _pendingRemovals;

	private static Type[] CallbackInterfacesDefualts => new Type[21]
	{
		typeof(IBeforeTick),
		typeof(IAfterTick),
		typeof(IBeforeAllTicks),
		typeof(IAfterAllTicks),
		typeof(IBeforePhysicsStep),
		typeof(IAfterPhysicsStep),
		typeof(IAfterPhysicsSyncTransforms2D),
		typeof(IAfterPhysicsSyncTransforms3D),
		typeof(IBeforeHitboxRegistration),
		typeof(IPlayerJoined),
		typeof(IPlayerLeft),
		typeof(IBeforeUpdate),
		typeof(IAfterUpdate),
		typeof(ISceneLoadDone),
		typeof(ISceneLoadStart),
		typeof(IAfterClientPredictionReset),
		typeof(IBeforeClientPredictionReset),
		typeof(IBeforeCopyPreviousState),
		typeof(IBeforeUpdateRemotePrefabs),
		typeof(IAfterUpdateRemotePrefabs),
		typeof(IAfterHostMigration)
	};

	public SimulationBehaviourUpdater()
	{
		_byTypeLookup = new Dictionary<Type, BehaviourList>();
		_byTypeHierarchy = new Dictionary<Type, (SimulationBehaviour[], Type[])>();
		_inOrderList = new List<BehaviourList>();
		_inOrderByInterfaceList = new Dictionary<Type, List<BehaviourList>>();
		_behavioursChecked = new HashSet<Type>();
		_pendingRemovals = new List<SimulationBehaviour>();
	}

	public void BuildTypeOrder(Type[] customCallbackInterfaces)
	{
		if (customCallbackInterfaces != null)
		{
			Assert.Always(customCallbackInterfaces.All((Type x) => x.IsInterface), "All types provided as custom callback interfaces must be interfaces.");
		}
		else
		{
			customCallbackInterfaces = new Type[0];
		}
		_inOrderList.Clear();
		_byTypeLookup.Clear();
		OrderNode[] array = new OrderSorter().Run();
		foreach (OrderNode orderNode in array)
		{
			Type type = orderNode.Type;
			if (!type.IsAbstract && typeof(SimulationBehaviour).IsAssignableFrom(type))
			{
				AddType(type, orderNode.SimFlags);
			}
		}
		foreach (Type item in CallbackInterfacesDefualts.Concat(customCallbackInterfaces))
		{
			List<BehaviourList> list = new List<BehaviourList>();
			for (int num2 = 0; num2 < _inOrderList.Count; num2++)
			{
				BehaviourList behaviourList = _inOrderList[num2];
				if (item.IsAssignableFrom(behaviourList.Type))
				{
					list.Add(behaviourList);
				}
			}
			_inOrderByInterfaceList.Add(item, list);
		}
	}

	public void InvokeRender()
	{
		try
		{
			int count = _inOrderList.Count;
			for (int i = 0; i < count; i++)
			{
				try
				{
					BehaviourList behaviourList = _inOrderList[i];
					SimulationBehaviour simulationBehaviour = behaviourList.Head;
					while (BehaviourUtils.IsNotNull(simulationBehaviour))
					{
						if (simulationBehaviour.CanReceiveCallback)
						{
							simulationBehaviour.Render();
						}
						simulationBehaviour = simulationBehaviour.Next;
					}
				}
				catch (Exception exn)
				{
					Log.Exception(exn);
				}
			}
		}
		finally
		{
		}
	}

	public int GetCallbackCount(Type type)
	{
		return _inOrderByInterfaceList[type].Count;
	}

	[Obsolete]
	public SimulationBehaviour GetCallbackHead(Type type, int index)
	{
		return _inOrderByInterfaceList[type][index].Head;
	}

	public SimulationBehaviourListScope GetCallbackHead(Type type, int index, out SimulationBehaviour head)
	{
		BehaviourList behaviourList = _inOrderByInterfaceList[type][index];
		head = behaviourList.Head;
		return new SimulationBehaviourListScope(behaviourList);
	}

	public void InvokeFixedUpdateNetwork(SimulationStages stage, SimulationModes mode)
	{
		EngineProfiler.Begin("SimulationBehaviourUpdater.InvokeFixedUpdateNetwork");
		int count = _inOrderList.Count;
		for (int i = 0; i < count; i++)
		{
			try
			{
				BehaviourList behaviourList = _inOrderList[i];
				if ((behaviourList.Modes & mode) != mode || (behaviourList.Stages & stage) != stage)
				{
					continue;
				}
				SimulationBehaviour simulationBehaviour = behaviourList.Head;
				Assert.Check(behaviourList.LockCount == 0);
				behaviourList.LockCount++;
				try
				{
					while (BehaviourUtils.IsNotNull(simulationBehaviour))
					{
						SimulationBehaviour next = simulationBehaviour.Next;
						if ((simulationBehaviour.Flags & SimulationBehaviourFlags.SkipNextUpdate) != 0)
						{
							simulationBehaviour.Flags &= ~SimulationBehaviourFlags.SkipNextUpdate;
						}
						else if (simulationBehaviour.CanReceiveCallback && (BehaviourUtils.IsNull(simulationBehaviour.Object) || simulationBehaviour.Object.InSimulation))
						{
							simulationBehaviour.FixedUpdateNetwork();
						}
						simulationBehaviour = next;
					}
				}
				finally
				{
					if (--behaviourList.LockCount == 0)
					{
						behaviourList.RemoveAllPending();
					}
				}
			}
			catch (Exception exn)
			{
				Log.Exception(exn);
			}
		}
		EngineProfiler.End();
	}

	public void RemoveObject(NetworkRunner runner, NetworkObject obj)
	{
		Assert.Check(obj.InSimulation);
		Assert.Check(BehaviourUtils.IsSameNotNull(obj.Runner, runner));
		obj.InSimulation = false;
		for (int i = 0; i < obj.NetworkedBehaviours.Length; i++)
		{
			Assert.Check(BehaviourUtils.IsSame(obj.NetworkedBehaviours[i].Object, obj));
			Assert.Check(BehaviourUtils.IsSame(obj.NetworkedBehaviours[i].Runner, runner));
			RemoveBehaviour(obj.NetworkedBehaviours[i]);
		}
		for (int j = 0; j < obj.SimulationBehaviours.Length; j++)
		{
			Assert.Check(BehaviourUtils.IsSame(obj.SimulationBehaviours[j].Object, obj));
			Assert.Check(BehaviourUtils.IsSame(obj.SimulationBehaviours[j].Runner, runner));
			RemoveBehaviour(obj.SimulationBehaviours[j]);
		}
	}

	public void AddObject(NetworkRunner runner, NetworkObject obj, bool skipFirstcall)
	{
		Assert.Check(!obj.InSimulation);
		Assert.Check(BehaviourUtils.IsSameNotNull(obj.Runner, runner));
		obj.InSimulation = true;
		for (int i = 0; i < obj.NetworkedBehaviours.Length; i++)
		{
			Assert.Check(BehaviourUtils.IsSame(obj.NetworkedBehaviours[i].Object, obj));
			Assert.Check(BehaviourUtils.IsSame(obj.NetworkedBehaviours[i].Runner, runner));
			AddBehaviour(obj.NetworkedBehaviours[i], skipFirstcall);
		}
		for (int j = 0; j < obj.SimulationBehaviours.Length; j++)
		{
			Assert.Check(BehaviourUtils.IsSame(obj.SimulationBehaviours[j].Object, obj));
			Assert.Check(BehaviourUtils.IsSame(obj.SimulationBehaviours[j].Runner, runner));
			AddBehaviour(obj.SimulationBehaviours[j], skipFirstcall);
		}
	}

	public void AddBehaviour(SimulationBehaviour behaviour, bool skipFirstCall)
	{
		CheckSimulationBehaviourForNetworkedAttribute(behaviour.GetType());
		if (skipFirstCall)
		{
			behaviour.Flags |= SimulationBehaviourFlags.SkipNextUpdate;
		}
		else
		{
			behaviour.Flags &= ~SimulationBehaviourFlags.SkipNextUpdate;
		}
		BehaviourList behaviourList = FindList(behaviour.GetType());
		if (behaviourList.IsInList(behaviour))
		{
			return;
		}
		SimulationBehaviour result = behaviourList.Head;
		if (BehaviourUtils.IsNotNull(behaviour.Object))
		{
			SimulationBehaviour result2 = null;
			if (FindFirstWithValidObject(result, out result))
			{
				while (FindFirstWithValidObject(result.Next, out result2))
				{
					if (result.Object.Id == behaviour.Object.Id)
					{
						behaviourList.AddAfter(behaviour, result);
						return;
					}
					if (result.Object.Id.Raw < behaviour.Object.Id.Raw && behaviour.Object.Id.Raw <= result2.Object.Id.Raw)
					{
						behaviourList.AddAfter(behaviour, result);
						return;
					}
					result = result2;
				}
			}
			Assert.Check(BehaviourUtils.IsNull(result) || BehaviourUtils.IsNull(result2));
			behaviourList.AddLast(behaviour);
			return;
		}
		string name = behaviour.GetType().Name;
		while (BehaviourUtils.IsNotNull(result) && BehaviourUtils.IsNotNull(result.Next) && !BehaviourUtils.IsNotNull(result.Object))
		{
			if (BehaviourUtils.IsNotNull(result.Next.Object))
			{
				behaviourList.AddAfter(behaviour, result);
				return;
			}
			int num = string.CompareOrdinal(result.GetType().Name, name);
			int num2 = string.CompareOrdinal(name, result.Next.GetType().Name);
			if (num < 0 && num2 <= 0)
			{
				behaviourList.AddAfter(behaviour, result);
				return;
			}
			result = result.Next;
		}
		if (BehaviourUtils.IsNull(result) || BehaviourUtils.IsNull(result.Next))
		{
			behaviourList.AddFirst(behaviour);
		}
	}

	private bool FindFirstWithValidObject(SimulationBehaviour behaviour, out SimulationBehaviour result)
	{
		SimulationBehaviour simulationBehaviour = behaviour;
		while (BehaviourUtils.IsNotNull(simulationBehaviour))
		{
			if (BehaviourUtils.IsNull(simulationBehaviour.Object))
			{
				Assert.Check(!simulationBehaviour.CanReceiveCallback);
				simulationBehaviour = simulationBehaviour.Next;
				continue;
			}
			result = simulationBehaviour;
			return true;
		}
		result = null;
		return false;
	}

	private void CheckSimulationBehaviourForNetworkedAttribute(Type type)
	{
		if (_behavioursChecked.Contains(type))
		{
			return;
		}
		_behavioursChecked.Add(type);
		if (typeof(NetworkBehaviour).IsAssignableFrom(type))
		{
			return;
		}
		PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		foreach (PropertyInfo propertyInfo in properties)
		{
			Attribute customAttribute = propertyInfo.GetCustomAttribute(typeof(NetworkedAttribute));
			if (customAttribute != null)
			{
				Log.Error("[Networked] attribute found on property " + propertyInfo.Name + " on " + type.FullName + ". [Networked] properties are only supported on types inheriting from NetworkBehaviour.");
			}
		}
		MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		foreach (MethodInfo methodInfo in methods)
		{
			Attribute customAttribute2 = methodInfo.GetCustomAttribute(typeof(RpcAttribute));
			if (customAttribute2 != null)
			{
				Log.Error("[Rpc] attribute found on method " + methodInfo.Name + " on " + type.FullName + ". [Rpc] methods are only supported on types inheriting from NetworkBehaviour.");
			}
		}
	}

	public void RemoveBehaviour(SimulationBehaviour behaviour)
	{
		BehaviourList behaviourList = FindList(behaviour.GetType());
		Assert.Check((behaviour.Flags & SimulationBehaviourFlags.PendingRemoval) == 0);
		if (behaviourList.LockCount > 0)
		{
			behaviour.Flags |= SimulationBehaviourFlags.PendingRemoval;
			behaviourList.PendingRemove(behaviour);
		}
		else
		{
			behaviourList.Remove(behaviour);
		}
	}

	public SimulationBehaviour[] GetTypeHeads(Type type)
	{
		if (!_byTypeHierarchy.TryGetValue(type, out var value))
		{
			List<Type> list = new List<Type>();
			for (int i = 0; i < _inOrderList.Count; i++)
			{
				if (type.IsAssignableFrom(_inOrderList[i].Type))
				{
					list.Add(_inOrderList[i].Type);
				}
			}
			Dictionary<Type, (SimulationBehaviour[], Type[])> byTypeHierarchy = _byTypeHierarchy;
			value = (new SimulationBehaviour[list.Count], list.ToArray());
			byTypeHierarchy.Add(type, value);
		}
		var (array, array2) = value;
		Assert.Check(array.Length == array2.Length);
		for (int j = 0; j < array2.Length; j++)
		{
			array[j] = FindList(array2[j]).Head;
		}
		return array;
	}

	private void AddType(Type type, (SimulationModes, SimulationStages) attr)
	{
		if (typeof(NetworkBehaviour).IsAssignableFrom(type))
		{
			NetworkBehaviourUtils.RegisterRpcInvokeDelegates(type);
			NetworkBehaviourUtils.RegisterStaticCallbacks(type);
			NetworkBehaviourUtils.RegisterInterestGroups(type);
		}
		else if (typeof(SimulationBehaviour).IsAssignableFrom(type))
		{
			NetworkBehaviourUtils.RegisterRpcInvokeDelegates(type);
		}
		(SimulationModes, SimulationStages) tuple = attr;
		SimulationModes item = tuple.Item1;
		SimulationStages item2 = tuple.Item2;
		BehaviourList behaviourList = new BehaviourList();
		behaviourList.Type = type;
		behaviourList.Modes = item;
		behaviourList.Stages = item2;
		_byTypeLookup.Add(type, behaviourList);
		_inOrderList.Add(behaviourList);
	}

	private BehaviourList FindList(Type type)
	{
		if (_byTypeLookup.TryGetValue(type, out var value))
		{
			return value;
		}
		Type key = type;
		while (typeof(SimulationBehaviour).IsAssignableFrom(type))
		{
			if (_byTypeLookup.TryGetValue(type, out value))
			{
				_byTypeLookup.Add(key, value);
				return value;
			}
			type = type.BaseType;
		}
		throw new InvalidOperationException(string.Format("{0} or any of its base-classes found in _byTypeLookup: {1}", type, string.Join(", ", _byTypeLookup.Select((KeyValuePair<Type, BehaviourList> x) => x.Key.ToString()).ToString())));
	}
}
