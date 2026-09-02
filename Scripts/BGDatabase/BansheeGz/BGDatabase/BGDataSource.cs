using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public abstract class BGDataSource : BGConfigurableI
{
	public enum ActionsTypeEnum : byte
	{
		All = 0,
		ImportOnly = 1,
		ExportOnly = 2,
		NoActions = 3
	}

	public class Descriptor : BGAttribute
	{
		public bool SupportSettings = true;
	}

	private static readonly List<Type> dataSources = new List<Type>();

	private ActionsTypeEnum actionsType;

	public ActionsTypeEnum ActionsType
	{
		get
		{
			return actionsType;
		}
		set
		{
			actionsType = value;
		}
	}

	public static List<Type> DataSources
	{
		get
		{
			if (dataSources.Count == 0)
			{
				dataSources.AddRange(BGUtil.GetAllSubTypes(typeof(BGDataSource)));
			}
			return dataSources;
		}
	}

	public virtual bool IsExportAllowed
	{
		get
		{
			if (actionsType != ActionsTypeEnum.All)
			{
				return actionsType == ActionsTypeEnum.ExportOnly;
			}
			return true;
		}
	}

	public virtual bool IsImportAllowed
	{
		get
		{
			if (actionsType != ActionsTypeEnum.All)
			{
				return actionsType == ActionsTypeEnum.ImportOnly;
			}
			return true;
		}
	}

	public abstract string Error { get; }

	public virtual bool RequireMergeSettings => true;

	public abstract string ConfigToString();

	public abstract void ConfigFromString(string config);
}
