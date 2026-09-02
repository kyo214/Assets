using System.Collections.Generic;

namespace Fusion.KCC;

public abstract class KCCInteractions<TInteraction> where TInteraction : KCCInteraction<TInteraction>, new()
{
	public readonly List<TInteraction> All = new List<TInteraction>();

	private static readonly KCCFastStack<TInteraction> _pool = new KCCFastStack<TInteraction>(256, createInstances: true);

	public int Count => All.Count;

	public bool HasProvider<T>() where T : class
	{
		int i = 0;
		for (int count = All.Count; i < count; i++)
		{
			if (All[i].Provider is T)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasProvider(IKCCInteractionProvider provider)
	{
		int i = 0;
		for (int count = All.Count; i < count; i++)
		{
			if (All[i].Provider == provider)
			{
				return true;
			}
		}
		return false;
	}

	public T GetProvider<T>() where T : class
	{
		int i = 0;
		for (int count = All.Count; i < count; i++)
		{
			if (All[i].Provider is T result)
			{
				return result;
			}
		}
		return null;
	}

	public void GetProviders<T>(List<T> providers, bool clearList = true) where T : class
	{
		if (clearList)
		{
			providers.Clear();
		}
		int i = 0;
		for (int count = All.Count; i < count; i++)
		{
			if (All[i].Provider is T item)
			{
				providers.Add(item);
			}
		}
	}

	public TInteraction Find(IKCCInteractionProvider provider)
	{
		int index;
		return Find(provider, out index);
	}

	public TInteraction Add(NetworkObject networkObject, IKCCInteractionProvider provider)
	{
		return AddInternal(networkObject, provider, invokeInitialize: true);
	}

	public bool Add(NetworkObject networkObject, KCCNetworkID networkID)
	{
		if (networkObject == null)
		{
			return false;
		}
		IKCCInteractionProvider componentNoAlloc = networkObject.GetComponentNoAlloc<IKCCInteractionProvider>();
		TInteraction val = _pool.PopOrCreate();
		val.NetworkID = networkID;
		val.NetworkObject = networkObject;
		val.Provider = componentNoAlloc;
		val.Initialize();
		All.Add(val);
		return true;
	}

	public bool Remove(TInteraction interaction)
	{
		int i = 0;
		for (int count = All.Count; i < count; i++)
		{
			if (All[i] == interaction)
			{
				All.RemoveAt(i);
				ReturnToPool(interaction);
				return true;
			}
		}
		return false;
	}

	public void CopyFromOther<T>(T other) where T : KCCInteractions<TInteraction>
	{
		int count = All.Count;
		int count2 = other.All.Count;
		if (count == count2)
		{
			if (count != 0)
			{
				for (int i = 0; i < count; i++)
				{
					TInteraction val = All[i];
					TInteraction val2 = other.All[i];
					val.NetworkID = val2.NetworkID;
					val.NetworkObject = val2.NetworkObject;
					val.Provider = val2.Provider;
					val.CopyFromOther(val2);
				}
			}
		}
		else
		{
			Clear();
			for (int j = 0; j < count2; j++)
			{
				TInteraction val3 = other.All[j];
				TInteraction val4 = _pool.PopOrCreate();
				val4.NetworkID = val3.NetworkID;
				val4.NetworkObject = val3.NetworkObject;
				val4.Provider = val3.Provider;
				val4.CopyFromOther(val3);
				All.Add(val4);
			}
		}
	}

	public void Clear()
	{
		int i = 0;
		for (int count = All.Count; i < count; i++)
		{
			ReturnToPool(All[i]);
		}
		All.Clear();
	}

	protected TInteraction AddInternal(NetworkObject networkObject, IKCCInteractionProvider provider, bool invokeInitialize)
	{
		TInteraction val = _pool.PopOrCreate();
		val.NetworkID = KCCNetworkID.GetNetworkID(networkObject);
		val.NetworkObject = networkObject;
		val.Provider = provider;
		if (invokeInitialize)
		{
			val.Initialize();
		}
		All.Add(val);
		return val;
	}

	protected void AddInternal(TInteraction interaction, NetworkObject networkObject, IKCCInteractionProvider provider, bool invokeInitialize)
	{
		interaction.NetworkID = KCCNetworkID.GetNetworkID(networkObject);
		interaction.NetworkObject = networkObject;
		interaction.Provider = provider;
		if (invokeInitialize)
		{
			interaction.Initialize();
		}
		All.Add(interaction);
	}

	protected TInteraction Find(IKCCInteractionProvider provider, out int index)
	{
		int i = 0;
		for (int count = All.Count; i < count; i++)
		{
			TInteraction val = All[i];
			if (val.Provider == provider)
			{
				index = i;
				return val;
			}
		}
		index = -1;
		return null;
	}

	protected static TInteraction GetFromPool()
	{
		return _pool.PopOrCreate();
	}

	private static void ReturnToPool(TInteraction interaction)
	{
		interaction.Deinitialize();
		interaction.NetworkID = default;
		interaction.NetworkObject = null;
		interaction.Provider = null;
		_pool.Push(interaction);
	}
}
