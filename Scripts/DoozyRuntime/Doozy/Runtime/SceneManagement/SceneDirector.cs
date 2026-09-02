using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.SceneManagement.Events;
using Doozy.Runtime.SceneManagement.ScriptableObjects;
using Doozy.Runtime.Signals;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Doozy.Runtime.SceneManagement;

[AddComponentMenu("Scene Management/Scene Director")]
public class SceneDirector : SingletonBehaviour<SceneDirector>
{
	public const string k_StreamCategory = "SceneManagement";

	public const string k_StreamName = "SceneDirector";

	[ClearOnReload]
	private static SignalStream s_stream;

	public bool DebugMode;

	[SerializeField]
	private ActiveSceneChangedEvent OnActiveSceneChanged;

	[SerializeField]
	private SceneLoadedEvent OnSceneLoaded;

	[SerializeField]
	private SceneUnloadedEvent OnSceneUnloaded;

	public static SignalStream stream => s_stream ?? (s_stream = SignalsService.GetStream("SceneManagement", "SceneDirector"));

	public static SceneManagementSettings settings => SingletonRuntimeScriptableObject<SceneManagementSettings>.instance;

	public bool debug => DebugMode | settings.DebugMode;

	public ActiveSceneChangedEvent onActiveSceneChanged => OnActiveSceneChanged ?? (OnActiveSceneChanged = new ActiveSceneChangedEvent());

	public SceneLoadedEvent onSceneLoaded => OnSceneLoaded ?? (OnSceneLoaded = new SceneLoadedEvent());

	public SceneUnloadedEvent onSceneUnloaded => OnSceneUnloaded ?? (OnSceneUnloaded = new SceneUnloadedEvent());

	private SignalReceiver receiver { get; set; }

	private void ProcessSignal(Signal signal)
	{
		if (signal.hasValue)
		{
			object valueAsObject = signal.valueAsObject;
			if (valueAsObject is SceneLoaderSignalData)
			{
				_ = (SceneLoaderSignalData)valueAsObject;
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		receiver = new SignalReceiver().SetOnSignalCallback(ProcessSignal);
		stream.ConnectReceiver(receiver);
	}

	private void OnEnable()
	{
		SceneManager.activeSceneChanged += ActiveSceneChanged;
		SceneManager.sceneLoaded += SceneLoaded;
		SceneManager.sceneUnloaded += SceneUnloaded;
	}

	private void OnDisable()
	{
		SceneManager.activeSceneChanged -= ActiveSceneChanged;
		SceneManager.sceneLoaded -= SceneLoaded;
		SceneManager.sceneUnloaded -= SceneUnloaded;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		stream.DisconnectReceiver(receiver);
	}

	private void ActiveSceneChanged(Scene current, Scene next)
	{
		onActiveSceneChanged?.Invoke(current, next);
		if (debug)
		{
			Log(ObjectNames.NicifyVariableName("ActiveSceneChanged") + " - Replaced Scene: " + current.name + " / Next Scene: " + next.name);
		}
	}

	private void SceneLoaded(Scene scene, LoadSceneMode mode)
	{
		onSceneLoaded?.Invoke(scene, mode);
		if (debug)
		{
			Log(string.Format("{0} - Scene: {1} / LoadSceneMode: {2}", ObjectNames.NicifyVariableName("SceneLoaded"), scene.name, mode));
		}
	}

	private void SceneUnloaded(Scene unloadedScene)
	{
		onSceneUnloaded?.Invoke(unloadedScene);
		if (debug)
		{
			Log(ObjectNames.NicifyVariableName("SceneUnloaded") + " - Scene: " + unloadedScene.name);
		}
	}

	public static SceneLoader LoadSceneAsync(int sceneBuildIndex, LoadSceneMode loadSceneMode)
	{
		if (SingletonBehaviour<SceneDirector>.instance.debug)
		{
			Log(string.Format("{0} - sceneBuildIndex: {1} / loadSceneMode: {2}", ObjectNames.NicifyVariableName("LoadSceneAsync"), sceneBuildIndex, loadSceneMode), SingletonBehaviour<SceneDirector>.instance);
		}
		return SceneLoader.GetLoader().SetSceneBuildIndex(sceneBuildIndex).SetLoadSceneBy(GetSceneBy.BuildIndex)
			.SetLoadSceneMode(loadSceneMode)
			.LoadSceneAsync();
	}

	public static SceneLoader LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode)
	{
		if (SingletonBehaviour<SceneDirector>.instance.debug)
		{
			Log(string.Format("{0} - sceneName: {1} / loadSceneMode: {2}", ObjectNames.NicifyVariableName("LoadSceneAsync"), sceneName, loadSceneMode), SingletonBehaviour<SceneDirector>.instance);
		}
		return SceneLoader.GetLoader().SetSceneName(sceneName).SetLoadSceneBy(GetSceneBy.Name)
			.SetLoadSceneMode(loadSceneMode)
			.LoadSceneAsync();
	}

	public static SceneLoader LoadSceneAsync(Scene scene, LoadSceneMode loadSceneMode)
	{
		if (SingletonBehaviour<SceneDirector>.instance.debug)
		{
			Log(string.Format("{0} - scene: {1} / loadSceneMode: {2}", ObjectNames.NicifyVariableName("LoadSceneAsync"), scene.name, loadSceneMode), SingletonBehaviour<SceneDirector>.instance);
		}
		return SceneLoader.GetLoader().LoadSceneAsync(scene, loadSceneMode);
	}

	public static AsyncOperation UnloadSceneAsync(Scene scene)
	{
		if (SingletonBehaviour<SceneDirector>.instance.debug)
		{
			Log(ObjectNames.NicifyVariableName("UnloadSceneAsync") + " - scene: " + scene.name, SingletonBehaviour<SceneDirector>.instance);
		}
		if (!scene.IsValid())
		{
			return null;
		}
		return SceneManager.UnloadSceneAsync(scene);
	}

	public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex)
	{
		if (SingletonBehaviour<SceneDirector>.instance.debug)
		{
			Log(string.Format("{0} - sceneBuildIndex: {1}", ObjectNames.NicifyVariableName("UnloadSceneAsync"), sceneBuildIndex), SingletonBehaviour<SceneDirector>.instance);
		}
		if (!SceneManager.GetSceneByBuildIndex(sceneBuildIndex).IsValid())
		{
			return null;
		}
		return SceneManager.UnloadSceneAsync(sceneBuildIndex);
	}

	public static AsyncOperation UnloadSceneAsync(string sceneName)
	{
		if (SingletonBehaviour<SceneDirector>.instance.debug)
		{
			Log(ObjectNames.NicifyVariableName("UnloadSceneAsync") + " - sceneName: " + sceneName, SingletonBehaviour<SceneDirector>.instance);
		}
		if (!SceneManager.GetSceneByName(sceneName).IsValid())
		{
			return null;
		}
		return SceneManager.UnloadSceneAsync(sceneName);
	}

	public static SceneDirector AddToScene(bool selectGameObjectAfterCreation = false)
	{
		return GameObjectUtils.AddToScene<SceneDirector>(isSingleton: true, selectGameObjectAfterCreation);
	}

	private void Log(string message)
	{
		Log("[" + base.name + "] " + message, this);
	}

	private static void Log(string message, Object context)
	{
		Debugger.Log("(SceneDirector) " + message, context);
	}
}
