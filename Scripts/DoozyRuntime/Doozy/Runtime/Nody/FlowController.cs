using System;
using System.Collections;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Nody;

[AddComponentMenu("Nody/Flow Controller")]
public class FlowController : MonoBehaviour, IUseMultiplayerInfo
{
	[SerializeField]
	private bool DontDestroyOnSceneChange;

	[SerializeField]
	private FlowGraph Flow;

	[SerializeField]
	private FlowType FlowType;

	[SerializeField]
	private ControllerBehaviour OnEnableBehaviour = ControllerBehaviour.StartFlow;

	[SerializeField]
	private ControllerBehaviour OnDisableBehaviour = ControllerBehaviour.StopFlow;

	[SerializeField]
	private UnityEvent OnStart = new UnityEvent();

	[SerializeField]
	private UnityEvent OnStop = new UnityEvent();

	[SerializeField]
	private UnityEvent OnPause = new UnityEvent();

	[SerializeField]
	private UnityEvent OnResume = new UnityEvent();

	[SerializeField]
	private UnityEvent OnBackFlow = new UnityEvent();

	[SerializeField]
	private MultiplayerInfo MultiplayerInfo;

	public static UIManagerInputSettings inputSettings => SingletonRuntimeScriptableObject<UIManagerInputSettings>.instance;

	public static bool multiplayerMode => inputSettings.multiplayerMode;

	public bool dontDestroyOnSceneChange
	{
		get
		{
			return DontDestroyOnSceneChange;
		}
		set
		{
			DontDestroyOnSceneChange = value;
		}
	}

	public FlowGraph flow => Flow;

	public FlowType flowType => FlowType;

	public ControllerBehaviour onEnableBehaviour
	{
		get
		{
			return OnEnableBehaviour;
		}
		set
		{
			OnEnableBehaviour = value;
		}
	}

	public ControllerBehaviour onDisableBehaviour
	{
		get
		{
			return OnDisableBehaviour;
		}
		set
		{
			OnDisableBehaviour = value;
		}
	}

	public UnityEvent onStart => OnStart ?? (OnStart = new UnityEvent());

	public UnityEvent onStop => OnStop ?? (OnStop = new UnityEvent());

	public UnityEvent onPause => OnPause ?? (OnPause = new UnityEvent());

	public UnityEvent onResume => OnResume ?? (OnResume = new UnityEvent());

	public UnityEvent onBackFlow => OnBackFlow ?? (OnBackFlow = new UnityEvent());

	public MultiplayerInfo multiplayerInfo => MultiplayerInfo;

	public bool hasMultiplayerInfo => multiplayerInfo != null;

	public int playerIndex
	{
		get
		{
			if (!(multiplayerMode & hasMultiplayerInfo))
			{
				return inputSettings.defaultPlayerIndex;
			}
			return multiplayerInfo.playerIndex;
		}
	}

	public bool initialized { get; private set; }

	public bool isValid
	{
		get
		{
			if (initialized && Flow != null)
			{
				return Flow.controller == this;
			}
			return false;
		}
	}

	public void SetMultiplayerInfo(MultiplayerInfo reference)
	{
		MultiplayerInfo = reference;
	}

	protected virtual void Awake()
	{
		if (Application.isPlaying)
		{
			if (dontDestroyOnSceneChange & (base.transform.parent != null))
			{
				Debug.LogWarning("[FlowController] - " + base.name + " is set to 'Don't destroy controller on scene change' but it has a parent. For this to work, the controller must be a root object in the scene hierarchy (it must not have a parent).");
			}
			if (dontDestroyOnSceneChange)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			BackButton.Initialize();
			initialized = false;
		}
	}

	protected virtual IEnumerator Start()
	{
		if (Application.isPlaying)
		{
			yield return null;
			SetFlowGraph(Flow);
		}
	}

	protected virtual void OnEnable()
	{
		if (Application.isPlaying)
		{
			RunBehavior(onEnableBehaviour);
		}
	}

	protected virtual void OnDisable()
	{
		if (Application.isPlaying)
		{
			RunBehavior(onDisableBehaviour);
		}
	}

	protected virtual void RunBehavior(ControllerBehaviour behaviour)
	{
		switch (behaviour)
		{
		case ControllerBehaviour.StartFlow:
			StartFlow();
			break;
		case ControllerBehaviour.RestartFlow:
			RestartFlow();
			break;
		case ControllerBehaviour.StopFlow:
			StopFlow();
			break;
		case ControllerBehaviour.PauseFlow:
			PauseFlow();
			break;
		case ControllerBehaviour.ResumeFlow:
			ResumeFlow();
			break;
		default:
			throw new ArgumentOutOfRangeException("behaviour", behaviour, null);
		case ControllerBehaviour.Disabled:
			break;
		}
	}

	private void Update()
	{
		if (isValid)
		{
			Flow.Update();
		}
	}

	private void FixedUpdate()
	{
		if (isValid)
		{
			Flow.FixedUpdate();
		}
	}

	private void LateUpdate()
	{
		if (isValid)
		{
			Flow.LateUpdate();
		}
	}

	public void SetFlowGraph(FlowGraph graph)
	{
		if (isValid)
		{
			StopFlow();
			Flow.OnStart.RemoveListener(OnStartFlow);
			Flow.OnStop.RemoveListener(OnStopFlow);
			Flow.OnPause.RemoveListener(OnPauseFlow);
			Flow.OnResume.RemoveListener(OnResumeFlow);
			Flow.OnBackFlow.RemoveListener(OnBackFlowTriggered);
			Flow.controller = null;
			Flow = null;
		}
		base.enabled = graph != null;
		if (!(graph == null))
		{
			Flow = ((flowType == FlowType.Local) ? graph.Clone() : graph);
			Flow.OnStart.AddListener(OnStartFlow);
			Flow.OnStop.AddListener(OnStopFlow);
			Flow.OnPause.AddListener(OnPauseFlow);
			Flow.OnResume.AddListener(OnResumeFlow);
			Flow.OnBackFlow.AddListener(OnBackFlowTriggered);
			StartCoroutine(InitializeFlow());
		}
	}

	public void SetActiveNode(FlowNode node, FlowPort fromPort = null)
	{
		if (isValid && !(node == null) && Flow.ContainsNode(node))
		{
			Flow.SetActiveNode(node, fromPort);
		}
	}

	public void SetActiveNodeById(string nodeId)
	{
		if (isValid && !nodeId.IsNullOrEmpty() && Flow.ContainsNodeById(nodeId))
		{
			Flow.SetActiveNodeByNodeId(nodeId);
		}
	}

	public void SetActiveNodeByName(string nodeName)
	{
		if (isValid && !nodeName.IsNullOrEmpty() && Flow.ContainsNodeByName(nodeName))
		{
			Flow.SetActiveNodeByNodeName(nodeName);
		}
	}

	public void ResetFlow()
	{
		if (isValid)
		{
			Flow.ResetGraph();
		}
	}

	public void RestartFlow()
	{
		if (isValid)
		{
			Flow.Restart();
		}
	}

	public void StartFlow()
	{
		if (isValid)
		{
			Flow.Start();
		}
	}

	public void StopFlow()
	{
		if (isValid)
		{
			Flow.Stop();
		}
	}

	public void PauseFlow()
	{
		if (isValid)
		{
			Flow.Pause();
		}
	}

	public void ResumeFlow()
	{
		if (isValid)
		{
			Flow.Resume();
		}
	}

	protected virtual void OnStartFlow()
	{
		onStart?.Invoke();
	}

	protected virtual void OnStopFlow()
	{
		onStop?.Invoke();
	}

	protected virtual void OnPauseFlow()
	{
		onPause?.Invoke();
	}

	protected virtual void OnResumeFlow()
	{
		onResume?.Invoke();
	}

	protected virtual void OnBackFlowTriggered()
	{
		onBackFlow?.Invoke();
	}

	private IEnumerator InitializeFlow()
	{
		yield return null;
		yield return new WaitForEndOfFrame();
		if (!(Flow == null))
		{
			initialized = true;
			Flow.controller = this;
			StartFlow();
		}
	}
}
