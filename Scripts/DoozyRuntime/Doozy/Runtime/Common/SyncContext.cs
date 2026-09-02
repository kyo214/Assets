using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Doozy.Runtime.Common;

public class SyncContext : SingletonBehaviour<SyncContext>
{
	public static TaskScheduler unityTaskScheduler { get; private set; }

	public static int unityThread { get; private set; }

	public static SynchronizationContext unitySynchronizationContext { get; private set; }

	public static Queue<Action> runInUpdate { get; } = new Queue<Action>();

	public static bool isOnUnityThread => unityThread == Thread.CurrentThread.ManagedThreadId;

	protected override void Awake()
	{
		base.Awake();
		unitySynchronizationContext = SynchronizationContext.Current;
		unityThread = Thread.CurrentThread.ManagedThreadId;
		unityTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
	}

	private void Update()
	{
		while (runInUpdate.Count > 0)
		{
			Action action = null;
			lock (runInUpdate)
			{
				if (runInUpdate.Count > 0)
				{
					action = runInUpdate.Dequeue();
				}
			}
			action?.Invoke();
		}
	}

	public static void Initialize()
	{
		if (!SingletonBehaviour<SyncContext>.applicationIsQuitting)
		{
			_ = SingletonBehaviour<SyncContext>.instance;
		}
	}

	public static void RunOnUnityThread(Action action)
	{
		Initialize();
		if (unityThread == Thread.CurrentThread.ManagedThreadId)
		{
			action();
			return;
		}
		lock (runInUpdate)
		{
			runInUpdate.Enqueue(action);
		}
	}
}
