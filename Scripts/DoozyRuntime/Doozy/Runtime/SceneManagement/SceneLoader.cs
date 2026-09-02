using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.Global;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.SceneManagement.ScriptableObjects;
using Doozy.Runtime.Signals;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Doozy.Runtime.SceneManagement;

[AddComponentMenu("Scene Management/Scene Loader")]
public class SceneLoader : MonoBehaviour
{
	public enum State
	{
		Idle = 0,
		LoadScene = 1,
		Loading = 2,
		SceneLoaded = 3,
		ActivatingScene = 4
	}

	public const string k_StreamCategory = "SceneManagement";

	public const string k_StreamName = "SceneLoader";

	public const GetSceneBy k_DefaultGetSceneBy = GetSceneBy.Name;

	public const LoadSceneMode k_DefaultLoadSceneMode = LoadSceneMode.Single;

	public const bool k_DefaultAutoSceneActivation = true;

	public const bool k_DefaultPreventLoadingSameScene = false;

	public const bool k_DefaultSelfDestructAfterSceneLoaded = false;

	public const float k_DefaultSceneActivationDelay = 0.2f;

	public const int k_DefaultBuildIndex = 0;

	public const string k_DefaultSceneName = "";

	[ClearOnReload]
	private static SignalStream s_stream;

	public bool DebugMode;

	public ModyEvent OnLoadScene = new ModyEvent("OnLoadScene");

	public ModyEvent OnSceneLoaded = new ModyEvent("OnSceneLoaded");

	public ModyEvent OnSceneActivated = new ModyEvent("OnSceneActivated");

	public FloatEvent OnProgressChanged = new FloatEvent();

	[SerializeField]
	private bool AllowSceneActivation = true;

	[SerializeField]
	private bool PreventLoadingSameScene;

	public GetSceneBy GetSceneBy;

	public LoadSceneMode LoadSceneMode;

	[SerializeField]
	private List<Progressor> Progressors;

	public float SceneActivationDelay = 0.2f;

	public int SceneBuildIndex;

	public string SceneName = "";

	public bool SelfDestructAfterSceneLoaded;

	private State m_CurrentState;

	private bool m_LoadInProgress;

	private bool m_SceneLoadedAndReady;

	private bool m_ActivatingScene;

	private float m_SceneLoadedAndReadyTime;

	private float m_Progress;

	[ClearOnReload]
	public static HashSet<SceneLoader> database { get; } = new HashSet<SceneLoader>();

	public static SignalStream stream => s_stream ?? (s_stream = SignalsService.GetStream("SceneManagement", "SceneLoader"));

	public static SceneManagementSettings settings => SingletonRuntimeScriptableObject<SceneManagementSettings>.instance;

	public bool debug => DebugMode | settings.DebugMode;

	public AsyncOperation currentAsyncOperation { get; private set; }

	public bool allowSceneActivation
	{
		get
		{
			return AllowSceneActivation;
		}
		set
		{
			AllowSceneActivation = value;
		}
	}

	public bool preventLoadingSameScene
	{
		get
		{
			return PreventLoadingSameScene;
		}
		set
		{
			PreventLoadingSameScene = value;
		}
	}

	public List<Progressor> progressors => Progressors ?? (Progressors = new List<Progressor>());

	public float progress
	{
		get
		{
			return m_Progress;
		}
		private set
		{
			m_Progress = value;
			progressors.ForEach((Progressor p) =>
			{
				p.PlayToProgress(value);
			});
			OnProgressChanged?.Invoke(value);
		}
	}

	public State currentState
	{
		get
		{
			return m_CurrentState;
		}
		private set
		{
			bool num = m_CurrentState != value;
			m_CurrentState = value;
			if (num)
			{
				stream?.SendSignal(new SceneLoaderSignalData(this));
			}
		}
	}

	private void Awake()
	{
		database.Add(this);
	}

	private void OnEnable()
	{
		database.Remove(null);
		ResetProgress();
	}

	private void OnDestroy()
	{
		database.Remove(null);
		database.Remove(this);
	}

	private void Update()
	{
		if (currentAsyncOperation == null)
		{
			return;
		}
		float num = Mathf.Clamp01(currentAsyncOperation.progress / 0.9f);
		if (Math.Abs(progress - num) > 0.0001f)
		{
			progress = num;
		}
		if (debug && (!m_ActivatingScene & !m_SceneLoadedAndReady))
		{
			Log($"Load progress: {Mathf.Round(progress * 100f)}%");
		}
		if (!m_SceneLoadedAndReady & !m_ActivatingScene)
		{
			currentState = State.Loading;
		}
		if (!m_SceneLoadedAndReady && currentAsyncOperation.progress == 0.9f)
		{
			if (debug)
			{
				Log("Scene finished loading and is ready to be activated.");
			}
			OnSceneLoaded?.Execute();
			currentState = State.SceneLoaded;
			m_SceneLoadedAndReady = true;
			m_SceneLoadedAndReadyTime = Time.realtimeSinceStartup;
		}
		if (m_SceneLoadedAndReady && !m_ActivatingScene && AllowSceneActivation)
		{
			if (SceneActivationDelay < 0f)
			{
				SceneActivationDelay = 0f;
			}
			if (SceneActivationDelay >= 0f && Time.realtimeSinceStartup - m_SceneLoadedAndReadyTime > SceneActivationDelay)
			{
				ActivateLoadedScene();
			}
		}
		if (m_ActivatingScene)
		{
			currentState = State.ActivatingScene;
		}
		if (currentAsyncOperation.isDone)
		{
			if (debug)
			{
				Log("Loaded scene has been activated.");
			}
			OnSceneActivated?.Execute();
			m_LoadInProgress = false;
			currentAsyncOperation = null;
			currentState = State.Idle;
			if (SelfDestructAfterSceneLoaded)
			{
				Coroutiner.Start(SelfDestruct());
			}
		}
	}

	public static bool IsSceneLoaded(string sceneName)
	{
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			if (SceneManager.GetSceneAt(i).name == sceneName)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsSceneLoaded(int sceneBuildIndex)
	{
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			if (SceneManager.GetSceneAt(i).buildIndex == sceneBuildIndex)
			{
				return true;
			}
		}
		return false;
	}

	public SceneLoader ActivateLoadedScene()
	{
		if (currentAsyncOperation == null)
		{
			return this;
		}
		if (debug)
		{
			Log("Activating Scene...");
		}
		m_ActivatingScene = true;
		currentState = State.ActivatingScene;
		currentAsyncOperation.allowSceneActivation = true;
		return this;
	}

	public SceneLoader LoadScene()
	{
		switch (GetSceneBy)
		{
		case GetSceneBy.Name:
			if (preventLoadingSameScene && IsSceneLoaded(SceneName))
			{
				return this;
			}
			SceneManager.LoadScene(SceneName, LoadSceneMode);
			break;
		case GetSceneBy.BuildIndex:
			if (preventLoadingSameScene && IsSceneLoaded(SceneBuildIndex))
			{
				return this;
			}
			SceneManager.LoadScene(SceneBuildIndex, LoadSceneMode);
			break;
		}
		return this;
	}

	public SceneLoader LoadSceneAsync()
	{
		switch (GetSceneBy)
		{
		case GetSceneBy.Name:
			if (preventLoadingSameScene && IsSceneLoaded(SceneName))
			{
				return this;
			}
			LoadSceneAsync(SceneName, LoadSceneMode);
			break;
		case GetSceneBy.BuildIndex:
			if (preventLoadingSameScene && IsSceneLoaded(SceneBuildIndex))
			{
				return this;
			}
			LoadSceneAsync(SceneBuildIndex, LoadSceneMode);
			break;
		}
		return this;
	}

	public SceneLoader LoadSceneAsync(int sceneBuildIndex, LoadSceneMode mode)
	{
		if (preventLoadingSameScene && IsSceneLoaded(sceneBuildIndex))
		{
			return this;
		}
		currentAsyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
		StartSceneLoad();
		return this;
	}

	public SceneLoader LoadSceneAsync(string sceneName, LoadSceneMode mode)
	{
		if (preventLoadingSameScene && IsSceneLoaded(sceneName))
		{
			return this;
		}
		currentAsyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
		StartSceneLoad();
		return this;
	}

	public SceneLoader LoadSceneAsync(Scene scene, LoadSceneMode mode)
	{
		if (preventLoadingSameScene && IsSceneLoaded(scene.name))
		{
			return this;
		}
		currentAsyncOperation = SceneManager.LoadSceneAsync(scene.name, mode);
		StartSceneLoad();
		return this;
	}

	public SceneLoader LoadSceneAsyncAdditive(int sceneBuildIndex)
	{
		return LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);
	}

	public SceneLoader LoadSceneAsyncAdditive(string sceneName)
	{
		return LoadSceneAsync(sceneName, LoadSceneMode.Additive);
	}

	public SceneLoader LoadSceneAsyncAdditive(Scene scene)
	{
		return LoadSceneAsync(scene, LoadSceneMode.Additive);
	}

	public SceneLoader LoadSceneAsyncSingle(int sceneBuildIndex)
	{
		return LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Single);
	}

	public SceneLoader LoadSceneAsyncSingle(string sceneName)
	{
		return LoadSceneAsync(sceneName, LoadSceneMode.Single);
	}

	public SceneLoader LoadSceneAsyncSingle(Scene scene)
	{
		return LoadSceneAsync(scene, LoadSceneMode.Single);
	}

	public SceneLoader SetAllowSceneActivation(bool whenReadyAllowSceneActivation)
	{
		allowSceneActivation = whenReadyAllowSceneActivation;
		return this;
	}

	public SceneLoader SetLoadSceneBy(GetSceneBy getSceneBy)
	{
		GetSceneBy = getSceneBy;
		return this;
	}

	public SceneLoader SetLoadSceneMode(LoadSceneMode loadSceneMode)
	{
		LoadSceneMode = loadSceneMode;
		return this;
	}

	public SceneLoader AddProgressor(Progressor progressor)
	{
		if (progressor == null)
		{
			return this;
		}
		progressors.RemoveNulls();
		if (progressors.Contains(progressor))
		{
			return this;
		}
		progressors.Add(progressor);
		return this;
	}

	public SceneLoader RemoveProgressor(Progressor progressor)
	{
		if (progressor == null)
		{
			return this;
		}
		progressors.RemoveNulls();
		if (!progressors.Contains(progressor))
		{
			return this;
		}
		progressors.Remove(progressor);
		return this;
	}

	public SceneLoader ClearProgressors()
	{
		progressors.Clear();
		return this;
	}

	public SceneLoader SetSceneActivationDelay(float sceneActivationDelay)
	{
		SceneActivationDelay = sceneActivationDelay;
		return this;
	}

	public SceneLoader SetSceneBuildIndex(int sceneBuildIndex)
	{
		SceneBuildIndex = sceneBuildIndex;
		return this;
	}

	public SceneLoader SetSceneName(string sceneName)
	{
		SceneName = sceneName;
		return this;
	}

	public SceneLoader SetSelfDestructAfterSceneLoaded(bool selfDestruct)
	{
		SelfDestructAfterSceneLoaded = selfDestruct;
		return this;
	}

	private void ResetProgress()
	{
		progressors.RemoveNulls();
		progressors.ForEach((Progressor p) =>
		{
			p.SetProgressAtZero();
		});
		progress = 0f;
	}

	private void StartSceneLoad()
	{
		ResetProgress();
		OnLoadScene?.Execute();
		currentState = State.LoadScene;
		currentAsyncOperation.allowSceneActivation = false;
		m_LoadInProgress = true;
		m_SceneLoadedAndReady = false;
		m_ActivatingScene = false;
	}

	private IEnumerator AsynchronousLoad(string sceneName, LoadSceneMode mode)
	{
		ResetProgress();
		OnLoadScene?.Execute();
		currentAsyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
		if (currentAsyncOperation == null)
		{
			yield break;
		}
		currentAsyncOperation.allowSceneActivation = false;
		m_LoadInProgress = true;
		bool sceneLoadedAndReady = false;
		bool activatingScene = false;
		while (m_LoadInProgress)
		{
			progress = Mathf.Clamp01(currentAsyncOperation.progress / 0.9f);
			if (debug && !activatingScene)
			{
				Log($"Load progress: {Mathf.Round(progress * 100f)}%");
			}
			if (!sceneLoadedAndReady && currentAsyncOperation.progress == 0.9f)
			{
				if (debug)
				{
					Log("Scene is ready to be activated.");
				}
				OnSceneLoaded.Execute();
				sceneLoadedAndReady = true;
			}
			if (sceneLoadedAndReady && !activatingScene)
			{
				if (SceneActivationDelay < 0f)
				{
					SceneActivationDelay = 0f;
				}
				if (SceneActivationDelay > 0f)
				{
					yield return new WaitForSecondsRealtime(SceneActivationDelay);
				}
				if (AllowSceneActivation)
				{
					ActivateLoadedScene();
					activatingScene = true;
				}
			}
			if (currentAsyncOperation.isDone)
			{
				if (debug)
				{
					Log("Scene has been activated.");
				}
				m_LoadInProgress = false;
				if (SelfDestructAfterSceneLoaded)
				{
					Coroutiner.Start(SelfDestruct());
				}
			}
			yield return null;
		}
	}

	private IEnumerator AsynchronousLoad(int sceneBuildIndex, LoadSceneMode mode)
	{
		ResetProgress();
		OnLoadScene?.Execute();
		currentAsyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
		if (currentAsyncOperation == null)
		{
			yield break;
		}
		currentAsyncOperation.allowSceneActivation = false;
		m_LoadInProgress = true;
		bool sceneLoadedAndReady = false;
		bool activatingScene = false;
		while (m_LoadInProgress)
		{
			progress = Mathf.Clamp01(currentAsyncOperation.progress / 0.9f);
			if (debug && !activatingScene)
			{
				Log($"Load progress: {Mathf.Round(progress * 100f)}%");
			}
			if (!sceneLoadedAndReady && currentAsyncOperation.progress == 0.9f)
			{
				if (debug)
				{
					Log("Scene is ready to be activated.");
				}
				OnSceneLoaded?.Execute();
				sceneLoadedAndReady = true;
			}
			if (sceneLoadedAndReady && !activatingScene && AllowSceneActivation)
			{
				if (SceneActivationDelay < 0f)
				{
					SceneActivationDelay = 0f;
				}
				if (SceneActivationDelay > 0f)
				{
					yield return new WaitForSecondsRealtime(SceneActivationDelay);
				}
				ActivateLoadedScene();
				activatingScene = true;
			}
			if (currentAsyncOperation.isDone)
			{
				if (debug)
				{
					Log("[" + base.name + "] Scene has been activated.");
				}
				m_LoadInProgress = false;
				if (SelfDestructAfterSceneLoaded)
				{
					Coroutiner.Start(SelfDestruct());
				}
			}
			yield return null;
		}
	}

	private IEnumerator SelfDestruct()
	{
		yield return null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public static void ActivateLoadedScenes()
	{
		if (settings.DebugMode)
		{
			Log("Activate Loaded Scenes", null);
		}
		database.Remove(null);
		foreach (SceneLoader item in database)
		{
			item.ActivateLoadedScene();
		}
	}

	public static SceneLoader GetLoader(Transform parent = null)
	{
		SceneLoader sceneLoader = new GameObject("SceneLoader").AddComponent<SceneLoader>();
		if (parent != null)
		{
			sceneLoader.transform.SetParent(parent);
			return sceneLoader;
		}
		UnityEngine.Object.DontDestroyOnLoad(sceneLoader);
		return sceneLoader;
	}

	private void Log(string message)
	{
		Log("[" + base.name + "] " + message, this);
	}

	private static void Log(string message, UnityEngine.Object context)
	{
		Debugger.Log("(SceneLoader) " + message, context);
	}
}
