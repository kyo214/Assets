using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AddonDescriptor(Name = "Modding", ManagerType = "BansheeGz.BGDatabase.Editor.BGAddonManagerModding")]
public class BGAddonModding : BGAddon
{
	[Serializable]
	private class JsonConfig
	{
		public bool InBuildsOnly;

		public bool EnableCellProtection;

		public BGModdingRepoProtection repoProtection;
	}

	private interface ModdingSourceI
	{
		int Order { get; }

		BGRepoDelta[] Deltas { get; }

		[Obsolete("Use Deltas property to provide delta content")]
		void OnLoad(BGRepo repo);
	}

	public class ModdingSourceDefault : ModdingSourceI
	{
		public virtual int Order => 0;

		public virtual BGRepoDelta[] Deltas => null;

		[Obsolete("Use Deltas property to provide delta content")]
		public virtual void OnLoad(BGRepo repo)
		{
		}
	}

	private bool inBuildsOnly;

	private bool enableCellProtection;

	private bool disableCellProtectionOnMerge;

	private BGModdingRepoProtection repoProtection;

	private List<ModdingSourceI> sources;

	public bool InBuildsOnly
	{
		get
		{
			return inBuildsOnly;
		}
		set
		{
			if (inBuildsOnly != value)
			{
				inBuildsOnly = value;
				FireChange();
			}
		}
	}

	public bool EnableCellProtection
	{
		get
		{
			return enableCellProtection;
		}
		set
		{
			if (enableCellProtection != value)
			{
				enableCellProtection = value;
				FireChange();
			}
		}
	}

	public bool DisableCellProtectionOnMerge
	{
		get
		{
			return disableCellProtectionOnMerge;
		}
		set
		{
			if (disableCellProtectionOnMerge != value)
			{
				disableCellProtectionOnMerge = value;
				FireChange();
			}
		}
	}

	public BGModdingRepoProtection RepoProtection
	{
		get
		{
			if (!enableCellProtection)
			{
				return null;
			}
			if (repoProtection == null)
			{
				repoProtection = new BGModdingRepoProtection(Repo);
				AssignListeners();
			}
			return repoProtection;
		}
	}

	private List<ModdingSourceI> Sources
	{
		get
		{
			if (sources != null)
			{
				return sources;
			}
			sources = new List<ModdingSourceI>();
			List<Type> allImplementations = BGUtil.GetAllImplementations(typeof(ModdingSourceI));
			foreach (Type item2 in allImplementations)
			{
				try
				{
					if (Activator.CreateInstance(item2) is ModdingSourceI item)
					{
						sources.Add(item);
					}
				}
				catch (Exception exception)
				{
					Debug.Log("BGAddonModding: Can not create instance of type  " + item2.FullName + ", see an exception below");
					Debug.LogException(exception);
				}
			}
			sources.Sort((ModdingSourceI o1, ModdingSourceI o2) => o1.Order.CompareTo(o2.Order));
			return sources;
		}
	}

	private void RepoProtectionChanged()
	{
		FireChange();
	}

	public override void OnLoad()
	{
		if (!BGRepo.DefaultRepo(Repo) || (!Application.isPlaying && !BGUtil.IsAboutToStartInEditor) || (inBuildsOnly && Application.isEditor))
		{
			return;
		}
		List<ModdingSourceI> list = Sources;
		foreach (ModdingSourceI item in list)
		{
			try
			{
				BGRepoDelta[] deltas = item.Deltas;
				if (deltas != null && deltas.Length != 0)
				{
					BGRepoDelta[] array = deltas;
					for (int i = 0; i < array.Length; i++)
					{
						array[i]?.ApplyTo(Repo, repoProtection);
					}
				}
			}
			catch (Exception exception)
			{
				Debug.LogError("Can not load deltas from " + item.GetType().FullName + " class. See exception below");
				Debug.LogException(exception);
			}
		}
	}

	public override BGAddon CloneTo(BGRepo repo)
	{
		BGAddonModding bGAddonModding = new BGAddonModding
		{
			Repo = repo,
			inBuildsOnly = inBuildsOnly,
			enableCellProtection = enableCellProtection,
			repoProtection = repoProtection?.CloneTo(repo)
		};
		bGAddonModding.AssignListeners();
		return bGAddonModding;
	}

	private void AssignListeners()
	{
		if (repoProtection != null)
		{
			repoProtection.Changed -= RepoProtectionChanged;
			repoProtection.Changed += RepoProtectionChanged;
		}
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new JsonConfig
		{
			InBuildsOnly = inBuildsOnly,
			EnableCellProtection = enableCellProtection,
			repoProtection = RepoProtection
		});
	}

	public override void ConfigFromString(string config)
	{
		JsonConfig jsonConfig = JsonUtility.FromJson<JsonConfig>(config);
		inBuildsOnly = jsonConfig.InBuildsOnly;
		enableCellProtection = jsonConfig.EnableCellProtection;
		if (enableCellProtection)
		{
			repoProtection = jsonConfig.repoProtection;
			AssignListeners();
		}
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(5);
		int num = 2;
		bGBinaryWriter.AddInt(num);
		bGBinaryWriter.AddBool(inBuildsOnly);
		bGBinaryWriter.AddBool(enableCellProtection);
		bGBinaryWriter.AddBool(disableCellProtectionOnMerge);
		if (enableCellProtection)
		{
			RepoProtection.ConfigToBytes(bGBinaryWriter, num);
		}
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		switch (num)
		{
		case 1:
			inBuildsOnly = bGBinaryReader.ReadBool();
			break;
		case 2:
			inBuildsOnly = bGBinaryReader.ReadBool();
			enableCellProtection = bGBinaryReader.ReadBool();
			disableCellProtectionOnMerge = bGBinaryReader.ReadBool();
			if (enableCellProtection)
			{
				RepoProtection.ConfigFromBytes(bGBinaryReader, num);
			}
			break;
		default:
			throw new BGException("Unknown version: $", num);
		}
	}
}
