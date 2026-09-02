using System;
using System.Collections.Generic;
using System.Threading;

namespace BansheeGz.BGDatabase;

public class BGMTService
{
	private class UpdateWorker
	{
		private readonly BlockingQueue<Action> queue = new BlockingQueue<Action>();

		public UpdateWorker()
		{
			Thread thread = new Thread(Run)
			{
				IsBackground = true
			};
			thread.Start();
		}

		public void Add(Action action)
		{
			queue.Enqueue(action);
		}

		private void Run()
		{
			while (true)
			{
				Action action = queue.Dequeue();
				action();
			}
		}
	}

	private class BlockingQueue<T>
	{
		private int count;

		private readonly Queue<T> queue = new Queue<T>();

		public T Dequeue()
		{
			lock (queue)
			{
				while (count <= 0)
				{
					Monitor.Wait(queue);
				}
				count--;
				return queue.Dequeue();
			}
		}

		public void Enqueue(T data)
		{
			lock (queue)
			{
				queue.Enqueue(data);
				count++;
				Monitor.Pulse(queue);
			}
		}
	}

	private BGMTRepo repo;

	private readonly UpdateWorker updateWorker;

	private BGMTRepo Repo
	{
		set
		{
			Interlocked.Exchange(ref repo, value);
		}
	}

	public BGMTRepo RepoReadOnly => repo;

	public BGMTService(bool multithreadedUpdates, BGMTRepo repo)
	{
		this.repo = repo;
		if (!multithreadedUpdates)
		{
			updateWorker = new UpdateWorker();
		}
	}

	public void Read(Action<BGMTRepo> transaction)
	{
		transaction(RepoReadOnly);
	}

	public void Write(Action<BGMTRepo> transaction, Action completedCallback = null)
	{
		if (updateWorker == null)
		{
			UpdateTask(transaction, completedCallback, async: false);
			return;
		}
		updateWorker.Add(() =>
		{
			UpdateTask(transaction, completedCallback, async: true);
		});
	}

	private void UpdateTask(Action<BGMTRepo> transaction, Action completedCallback, bool async)
	{
		BGMTRepo bGMTRepo = RepoReadOnly.ToWritableRepo();
		transaction(bGMTRepo);
		bGMTRepo.ForEachMeta((BGMTMeta meta) =>
		{
			meta.ApplyDelete();
		});
		Repo = bGMTRepo.ToReadOnlyRepo();
		if (completedCallback == null)
		{
			return;
		}
		if (async)
		{
			ThreadPool.QueueUserWorkItem((object state) =>
			{
				completedCallback();
			});
		}
		else
		{
			completedCallback();
		}
	}
}
