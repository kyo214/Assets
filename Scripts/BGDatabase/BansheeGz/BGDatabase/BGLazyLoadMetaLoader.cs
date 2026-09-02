using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGLazyLoadMetaLoader
{
	private readonly BGMetaEntity meta;

	private readonly List<Action> loadActions = new List<Action>();

	private string error;

	public BGLazyLoadMetaLoader(BGMetaEntity meta)
	{
		this.meta = meta;
	}

	public void AddAction(Action action)
	{
		loadActions.Add(action);
	}

	public void Load()
	{
		if (error != null)
		{
			throw new Exception(error);
		}
		if (loadActions.Count == 0)
		{
			throw new Exception("Can not load, cause load is already executed!");
		}
		try
		{
			meta.Repo.Events.WithEventsDisabled(() =>
			{
				foreach (Action loadAction in loadActions)
				{
					loadAction();
				}
			});
		}
		catch (Exception ex)
		{
			error = ex.Message ?? ("unknown error while lazy loading data: " + ex.GetType().FullName);
			Debug.LogException(ex);
			throw;
		}
		loadActions.Clear();
	}
}
