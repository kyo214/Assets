using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGMultiThreadedLoader
{
	private class ActionsListRunner
	{
		private readonly List<Action> actions = new List<Action>();

		public bool HasActions => actions.Count > 0;

		public void AddAction(Action action)
		{
			actions.Add(action);
		}

		public void Go()
		{
			for (int i = 0; i < actions.Count; i++)
			{
				actions[i]();
			}
		}
	}

	private readonly ActionsListRunner[] loaders;

	private readonly ActionsListRunner mainThreadLoader;

	private readonly List<Exception> errorsList = new List<Exception>();

	private int currentLoader;

	public BGMultiThreadedLoader()
	{
		mainThreadLoader = new ActionsListRunner();
		int num = Mathf.Clamp(Environment.ProcessorCount, 1, 16);
		loaders = new ActionsListRunner[num];
		for (int i = 0; i < num; i++)
		{
			loaders[i] = new ActionsListRunner();
		}
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void AddException(Exception e)
	{
		errorsList.Add(e);
	}

	public void PrintExceptions()
	{
		if (errorsList.Count != 0)
		{
			for (int i = 0; i < errorsList.Count; i++)
			{
				Debug.LogException(errorsList[i]);
			}
		}
	}

	public void AddAction(Action action, bool runOnMainThread)
	{
		if (runOnMainThread)
		{
			mainThreadLoader.AddAction(action);
			return;
		}
		ActionsListRunner actionsListRunner = loaders[currentLoader];
		actionsListRunner.AddAction(action);
		currentLoader++;
		if (currentLoader == loaders.Length)
		{
			currentLoader = 0;
		}
	}

	public void Load()
	{
		Thread[] array = new Thread[loaders.Length];
		for (int i = 0; i < array.Length; i++)
		{
			ActionsListRunner actionsListRunner = loaders[i];
			if (actionsListRunner != null && actionsListRunner.HasActions)
			{
				Thread thread = new Thread(actionsListRunner.Go);
				thread.Start();
				array[i] = thread;
			}
		}
		mainThreadLoader.Go();
		for (int j = 0; j < array.Length; j++)
		{
			array[j]?.Join();
		}
		PrintExceptions();
	}
}
