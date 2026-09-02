using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGMainThreadRunner : MonoBehaviour
{
	private static BGMainThreadRunner instance;

	private static volatile bool hasJobs;

	private static List<Action> jobs = new List<Action>(4);

	private static List<Action> jobsToRun = new List<Action>(4);

	private Thread mainThread;

	public static BGMainThreadRunner Instance => instance;

	public Thread MainThread => mainThread;

	public static void RunOnMainThread(Action action)
	{
		lock (jobs)
		{
			jobs.Add(action);
			hasJobs = true;
		}
	}

	public static void EnsureMainThread(string error)
	{
		if (instance == null || instance.mainThread == null || instance.mainThread == Thread.CurrentThread)
		{
			return;
		}
		throw new BGException(error);
	}

	private static void Initialize()
	{
		if (!(instance != null))
		{
			instance = new GameObject("BGMainThreadRunner")
			{
				hideFlags = (HideFlags.HideInHierarchy | HideFlags.HideInInspector)
			}.AddComponent<BGMainThreadRunner>();
			UnityEngine.Object.DontDestroyOnLoad(instance.gameObject);
		}
	}

	private void Start()
	{
		mainThread = Thread.CurrentThread;
	}

	private void Update()
	{
		if (!hasJobs)
		{
			return;
		}
		lock (jobs)
		{
			List<Action> list = jobs;
			List<Action> list2 = jobsToRun;
			jobsToRun = list;
			jobs = list2;
			hasJobs = false;
		}
		foreach (Action item in jobsToRun)
		{
			try
			{
				item();
			}
			catch (Exception)
			{
			}
		}
		jobsToRun.Clear();
	}
}
