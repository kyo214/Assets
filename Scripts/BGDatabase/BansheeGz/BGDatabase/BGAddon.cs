using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public abstract class BGAddon : BGConfigurableI, BGConfigurableBinaryI
{
	public class AddonDescriptor : BGAttributeWithManager
	{
	}

	private static readonly List<Type> AllAddonTypes = new List<Type>();

	private BGRepo repo;

	public static List<Type> AddonTypes
	{
		get
		{
			if (AllAddonTypes.Count != 0)
			{
				return AllAddonTypes;
			}
			List<Type> allSubTypes = BGUtil.GetAllSubTypes(typeof(BGAddon));
			foreach (Type item in allSubTypes)
			{
				AllAddonTypes.Add(item);
			}
			return AllAddonTypes;
		}
	}

	public virtual BGRepo Repo
	{
		get
		{
			return repo;
		}
		protected set
		{
			repo = value;
		}
	}

	public string Name => GetName(GetType());

	public virtual int OnMainDatabaseLoadOrder => 0;

	public static string GetName(Type type)
	{
		return BGUtil.GetAttribute<AddonDescriptor>(type).Name;
	}

	public static BGAddon Create(string type, string config)
	{
		BGAddon bGAddon = BGUtil.Create<BGAddon>(type, includePrivateConstructors: false, Array.Empty<object>());
		bGAddon.ConfigFromString(config);
		return bGAddon;
	}

	[Obsolete("This method is deprecated. Use FromBinary instead")]
	public static BGAddon Create(string type, ArraySegment<byte> config)
	{
		BGAddon bGAddon = BGUtil.Create<BGAddon>(type, includePrivateConstructors: false, Array.Empty<object>());
		bGAddon.ConfigFromBytes(config);
		return bGAddon;
	}

	internal static BGAddon FromBinary(BGBinaryReader binder)
	{
		int num = binder.ReadInt();
		if (num == 1)
		{
			string typeName = binder.ReadString();
			ArraySegment<byte> config = binder.ReadByteArray();
			BGAddon bGAddon = BGUtil.Create<BGAddon>(typeName, includePrivateConstructors: false, Array.Empty<object>());
			bGAddon.ConfigFromBytes(config);
			return bGAddon;
		}
		throw new BGException("Can not read addon from binary array: unsupported version $", num);
	}

	internal static void ToBinary(BGBinaryWriter builder, BGAddon addon)
	{
		builder.AddInt(1);
		builder.AddString(addon.GetType().AssemblyQualifiedName);
		builder.AddByteArray(addon.ConfigToBytes());
	}

	public virtual string ConfigToString()
	{
		return null;
	}

	public virtual void ConfigFromString(string config)
	{
	}

	public virtual byte[] ConfigToBytes()
	{
		return null;
	}

	public virtual void ConfigFromBytes(ArraySegment<byte> config)
	{
	}

	public void Init(BGRepo repo)
	{
		Repo = repo;
	}

	public abstract BGAddon CloneTo(BGRepo repo);

	public void CloneAndAddTo(BGRepo repo)
	{
		repo.Addons.Add(CloneTo(repo));
	}

	public virtual List<Type> GetRequiredAddons()
	{
		return null;
	}

	public void FireChange()
	{
		if (Repo != null && Repo.Events.On)
		{
			Repo.Events.FireAddonChange();
		}
	}

	public virtual void OnMainDatabaseLoad()
	{
	}

	public virtual void OnLoad()
	{
	}

	public virtual void OnDelete(BGRepo repo)
	{
	}

	public virtual void OnBeforeAdd(BGRepo repo)
	{
		List<Type> requiredAddons = GetRequiredAddons();
		if (requiredAddons == null || requiredAddons.Count == 0)
		{
			return;
		}
		foreach (Type item in requiredAddons)
		{
			if (!repo.Addons.Has(item))
			{
				throw new BGException("Addon $ is required, but it was not activated. Activate it first.", GetName(item));
			}
		}
	}

	public virtual void OnTransfer(BGRepo repo)
	{
	}
}
