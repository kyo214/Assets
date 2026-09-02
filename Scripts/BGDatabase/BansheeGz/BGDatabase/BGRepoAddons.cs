using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGRepoAddons
{
	private readonly Dictionary<Type, BGAddon> type2Addon = new Dictionary<Type, BGAddon>();

	private readonly BGRepo repo;

	public int Count => type2Addon.Count;

	public List<BGAddon> Addons => new List<BGAddon>(type2Addon.Values);

	public BGRepoAddons(BGRepo repo)
	{
		this.repo = repo;
	}

	public T Get<T>() where T : BGAddon
	{
		return (T)BGUtil.Get(type2Addon, typeof(T));
	}

	public BGAddon Get(Type addonType)
	{
		return BGUtil.Get(type2Addon, addonType);
	}

	public BGAddon Get(string type)
	{
		foreach (KeyValuePair<Type, BGAddon> item in type2Addon)
		{
			if (type.Equals(item.Key.FullName))
			{
				return item.Value;
			}
		}
		return null;
	}

	public bool Has<T>() where T : BGAddon
	{
		return type2Addon.ContainsKey(typeof(T));
	}

	public bool Has(Type type)
	{
		return type2Addon.ContainsKey(type);
	}

	public bool Has(string typeFullName)
	{
		foreach (BGAddon addon in Addons)
		{
			if (string.Equals(addon.GetType().FullName, typeFullName))
			{
				return true;
			}
		}
		return false;
	}

	public void Add(BGAddon addon)
	{
		addon.Init(repo);
		type2Addon[addon.GetType()] = addon;
		repo.Events.FireAddonChange();
	}

	public void Remove(Type type)
	{
		if (type2Addon.TryGetValue(type, out var value))
		{
			value.OnDelete(repo);
			type2Addon.Remove(type);
			repo.Events.FireAddonChange();
		}
	}

	public void Remove<T>() where T : BGAddon
	{
		Remove(typeof(T));
	}

	public void Clear()
	{
		if (type2Addon.Count != 0)
		{
			type2Addon.Clear();
			repo.Events.FireAddonChange();
		}
	}

	public void AddFrom(BGRepoAddons addons)
	{
		addons.ForEachAddon((BGAddon addon) =>
		{
			addon.CloneAndAddTo(repo);
		});
		repo.Events.FireAddonChange();
	}

	public void ForEachAddon(Action<BGAddon> action)
	{
		foreach (KeyValuePair<Type, BGAddon> item in type2Addon)
		{
			action(item.Value);
		}
	}
}
