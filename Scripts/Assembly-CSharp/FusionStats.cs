using System;
using System.Collections.Generic;
using System.Reflection;
using Fusion;
using Fusion.StatsInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ScriptHelp(BackColor = EditorHeaderBackColor.Olive)]
[ExecuteAlways]
public class FusionStats : Fusion.Behaviour
{
	public enum StatCanvasTypes
	{
		Overlay = 0,
		GameObject = 1
	}

	public enum DefaultLayouts
	{
		Custom = 0,
		Left = 1,
		Right = 2,
		UpperLeft = 3,
		UpperRight = 4,
		Full = 5
	}

	private static Dictionary<NetworkRunner, List<FusionStats>> _statsForRunnerLookup = new Dictionary<NetworkRunner, List<FusionStats>>();

	private static Dictionary<string, FusionStats> _activeGuids = new Dictionary<string, FusionStats>();

	public const Simulation.Statistics.SimStatFlags DefaultSimStatsMask = Simulation.Statistics.SimStatFlags.ForwardSimCount | Simulation.Statistics.SimStatFlags.ResimCount | Simulation.Statistics.SimStatFlags.PacketSize;

	private const int SCREEN_SCALE_W = 1080;

	private const int SCREEN_SCALE_H = 1080;

	private const float TEXT_MARGIN = 0.25f;

	private const float TITLE_HEIGHT = 20f;

	private const int MARGIN = 6;

	private const int PAD = 10;

	private const string PLAY_TEXT = "PLAY";

	private const string PAUS_TEXT = "PAUSE";

	private const string SHOW_TEXT = "SHOW";

	private const string HIDE_TEXT = "HIDE";

	private const string CLER_TEXT = "CLEAR";

	private const string CNVS_TEXT = "CANVAS";

	private const string CLSE_TEXT = "CLOSE";

	private const string PLAY_ICON = "►";

	private const string PAUS_ICON = "װ";

	private const string HIDE_ICON = "▼";

	private const string SHOW_ICON = "▲";

	private const string CLER_ICON = "ᴓ";

	private const string CNVS_ICON = "ﬦ";

	private const string CLSE_ICON = "x";

	[InlineHelp]
	[Unit(Units.Seconds, 1.0, 0.0, DecimalPlaces = 2)]
	[MultiPropertyDrawersFix]
	public float RedrawInterval = 0.1f;

	[Header("Layout")]
	[InlineHelp]
	[SerializeField]
	private StatCanvasTypes _canvasType;

	[InlineHelp]
	[SerializeField]
	private bool _showButtonLabels = true;

	[InlineHelp]
	[SerializeField]
	[Range(0f, 200f)]
	[MultiPropertyDrawersFix]
	private int _maxHeaderHeight = 70;

	[InlineHelp]
	[DrawIf("_canvasType", 1.0, Hide = true)]
	[Range(0f, 20f)]
	[MultiPropertyDrawersFix]
	public float CanvasScale = 5f;

	[InlineHelp]
	[DrawIf("_canvasType", 1.0, Hide = true)]
	[Range(-10f, 10f)]
	[MultiPropertyDrawersFix]
	public float CanvasDistance;

	[InlineHelp]
	[SerializeField]
	[DrawIf("CanvasType", 1.0, Hide = true)]
	[NormalizedRect(true, 1f)]
	[MultiPropertyDrawersFix]
	private Rect _gameObjectRect = new Rect(0f, 0f, 0.3f, 1f);

	[InlineHelp]
	[SerializeField]
	[DrawIf("CanvasType", 0.0, Hide = true)]
	[NormalizedRect(true, 0f)]
	[MultiPropertyDrawersFix]
	private Rect _overlayRect = new Rect(0f, 0f, 0.3f, 1f);

	[Header("Fusion Graphs Layout")]
	[InlineHelp]
	[SerializeField]
	private FusionGraph.Layouts _defaultLayout;

	[InlineHelp]
	[SerializeField]
	private bool _noTextOverlap;

	[InlineHelp]
	[SerializeField]
	private bool _noGraphShader;

	[InlineHelp]
	[Range(0f, 16f)]
	[MultiPropertyDrawersFix]
	public int GraphColumnCount = 1;

	[InlineHelp]
	[SerializeField]
	[DrawIf("GraphColumnCount", 0.0)]
	[Range(30f, 1080f)]
	[MultiPropertyDrawersFix]
	private int _graphMaxWidth = 270;

	[Header("Network Object Stats")]
	[InlineHelp]
	[SerializeField]
	[WarnIf("ShowMissingNetObjWarning", "No NetworkObject found on this GameObject, nor parent. Object stats will be unavailable.")]
	private bool _enableObjectStats;

	[InlineHelp]
	[SerializeField]
	[DrawIf("EnableObjectStats")]
	private NetworkObject _object;

	[InlineHelp]
	[SerializeField]
	[DrawIf("EnableObjectStats")]
	[Range(0f, 200f)]
	[MultiPropertyDrawersFix]
	private int _objectTitleHeight = 48;

	[InlineHelp]
	[SerializeField]
	[DrawIf("EnableObjectStats")]
	[Range(0f, 200f)]
	[MultiPropertyDrawersFix]
	private int _objectIdsHeight = 60;

	[InlineHelp]
	[SerializeField]
	[DrawIf("EnableObjectStats")]
	[Range(0f, 200f)]
	[MultiPropertyDrawersFix]
	private int _objectMetersHeight = 90;

	[Header("Data")]
	[SerializeField]
	[InlineHelp]
	[EditorDisabled(false)]
	[MultiPropertyDrawersFix]
	private NetworkRunner _runner;

	[InlineHelp]
	public bool InitializeAllGraphs;

	[InlineHelp]
	[VersaMask(false, null)]
	[MultiPropertyDrawersFix]
	public SimulationModes ConnectTo = SimulationModes.Client;

	[InlineHelp]
	[SerializeField]
	[VersaMask(false, null)]
	[DrawIf("EnableObjectStats")]
	[MultiPropertyDrawersFix]
	private Simulation.Statistics.ObjStatFlags _includedObjStats;

	[InlineHelp]
	[SerializeField]
	[VersaMask(false, null)]
	[MultiPropertyDrawersFix]
	private Simulation.Statistics.NetStatFlags _includedNetStats;

	[InlineHelp]
	[SerializeField]
	[VersaMask(false, null)]
	[MultiPropertyDrawersFix]
	private Simulation.Statistics.SimStatFlags _includedSimStats;

	[Header("Life-Cycle")]
	[InlineHelp]
	[SerializeField]
	public bool AutoDestroy;

	[InlineHelp]
	[SerializeField]
	public bool EnforceSingle = true;

	[InlineHelp]
	[DrawIf("EnforceSingle")]
	[SerializeField]
	public string Guid;

	[Header("Customization")]
	[InlineHelp]
	[SerializeField]
	[DrawIf("IsNotPlaying", Hide = true)]
	[MultiPropertyDrawersFix]
	private bool _modifyColors;

	[InlineHelp]
	[SerializeField]
	[DrawIf("ShowColorControls", Hide = true)]
	private Color _graphColorGood = new Color(0.1f, 0.5f, 0.1f, 0.9f);

	[InlineHelp]
	[SerializeField]
	[DrawIf("ShowColorControls", Hide = true)]
	private Color _graphColorWarn = new Color(0.75f, 0.75f, 0.2f, 0.9f);

	[InlineHelp]
	[SerializeField]
	[DrawIf("ShowColorControls", Hide = true)]
	private Color _graphColorBad = new Color(0.9f, 0.2f, 0.2f, 0.9f);

	[InlineHelp]
	[SerializeField]
	[DrawIf("ShowColorControls", Hide = true)]
	private Color _graphColorFlag = new Color(0.8f, 0.75f, 0f, 1f);

	[InlineHelp]
	[SerializeField]
	[DrawIf("ShowColorControls", Hide = true)]
	private Color _fontColor = new Color(1f, 1f, 1f, 1f);

	[InlineHelp]
	[SerializeField]
	[DrawIf("ShowColorControls", Hide = true)]
	private Color PanelColor = new Color(0.3f, 0.3f, 0.3f, 1f);

	[InlineHelp]
	[SerializeField]
	[DrawIf("ShowColorControls", Hide = true)]
	private Color _simDataBackColor = new Color(0.1f, 0.08f, 0.08f, 1f);

	[InlineHelp]
	[SerializeField]
	[DrawIf("ShowColorControls", Hide = true)]
	private Color _netDataBackColor = new Color(0.15f, 0.14f, 0.09f, 1f);

	[InlineHelp]
	[SerializeField]
	[DrawIf("ShowColorControls", Hide = true)]
	private Color _objDataBackColor = new Color(0f, 0.2f, 0.4f, 1f);

	[SerializeField]
	[HideInInspector]
	private FusionGraph[] _simGraphs;

	[SerializeField]
	[HideInInspector]
	private FusionGraph[] _objGraphs;

	[SerializeField]
	[HideInInspector]
	private FusionGraph[] _netGraphs;

	[NonSerialized]
	private List<IFusionStatsView> _foundViews;

	[NonSerialized]
	private List<FusionGraph> _foundGraphs;

	[SerializeField]
	[HideInInspector]
	private Text _titleText;

	[SerializeField]
	[HideInInspector]
	private Text _clearIcon;

	[SerializeField]
	[HideInInspector]
	private Text _pauseIcon;

	[SerializeField]
	[HideInInspector]
	private Text _togglIcon;

	[SerializeField]
	[HideInInspector]
	private Text _closeIcon;

	[SerializeField]
	[HideInInspector]
	private Text _canvsIcon;

	[SerializeField]
	[HideInInspector]
	private Text _clearLabel;

	[SerializeField]
	[HideInInspector]
	private Text _pauseLabel;

	[SerializeField]
	[HideInInspector]
	private Text _togglLabel;

	[SerializeField]
	[HideInInspector]
	private Text _closeLabel;

	[SerializeField]
	[HideInInspector]
	private Text _canvsLabel;

	[SerializeField]
	[HideInInspector]
	private Text _objectNameText;

	[SerializeField]
	[HideInInspector]
	private GridLayoutGroup _graphGridLayoutGroup;

	[SerializeField]
	[HideInInspector]
	private Canvas _canvas;

	[SerializeField]
	[HideInInspector]
	private RectTransform _canvasRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _rootPanelRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _guidesRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _headerRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _statsPanelRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _graphsLayoutRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _titleRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _buttonsRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _objectTitlePanelRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _objectIdsGroupRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _objectMetersPanelRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _clientIdPanelRT;

	[SerializeField]
	[HideInInspector]
	private RectTransform _authorityPanelRT;

	[SerializeField]
	[HideInInspector]
	private Button _titleButton;

	[SerializeField]
	[HideInInspector]
	private Button _objctButton;

	[SerializeField]
	[HideInInspector]
	private Button _clearButton;

	[SerializeField]
	[HideInInspector]
	private Button _togglButton;

	[SerializeField]
	[HideInInspector]
	private Button _pauseButton;

	[SerializeField]
	[HideInInspector]
	private Button _closeButton;

	[SerializeField]
	[HideInInspector]
	private Button _canvsButton;

	private Font _font;

	private bool _hidden;

	private bool _paused;

	private int _layoutDirty;

	private bool _activeDirty;

	private double _currentDrawTime;

	private double _delayDrawUntil;

	private static bool? _newInputSystemFound;

	private string _previousObjectTitle;

	private float _lastLayoutUpdate;

	public static Simulation.Statistics.NetStatFlags DefaultNetStatsMask => Simulation.Statistics.NetStatFlags.RoundTripTime | Simulation.Statistics.NetStatFlags.SentPacketSizes | Simulation.Statistics.NetStatFlags.ReceivedPacketSizes;

	private bool ShowColorControls
	{
		get
		{
			if (!Application.isPlaying)
			{
				return _modifyColors;
			}
			return false;
		}
	}

	private bool IsNotPlaying => !Application.isPlaying;

	public StatCanvasTypes CanvasType
	{
		get
		{
			return _canvasType;
		}
		set
		{
			_canvasType = value;
			DirtyLayout(2);
		}
	}

	public bool ShowButtonLabels
	{
		get
		{
			return _showButtonLabels;
		}
		set
		{
			_showButtonLabels = value;
			DirtyLayout();
		}
	}

	public int MaxHeaderHeight
	{
		get
		{
			return _maxHeaderHeight;
		}
		set
		{
			_maxHeaderHeight = value;
			DirtyLayout();
		}
	}

	public Rect GameObjectRect
	{
		get
		{
			return _gameObjectRect;
		}
		set
		{
			_gameObjectRect = value;
			DirtyLayout();
		}
	}

	public Rect OverlayRect
	{
		get
		{
			return _overlayRect;
		}
		set
		{
			_overlayRect = value;
			DirtyLayout();
		}
	}

	public FusionGraph.Layouts DefaultLayout
	{
		get
		{
			return _defaultLayout;
		}
		set
		{
			_defaultLayout = value;
			DirtyLayout();
		}
	}

	public bool NoTextOverlap
	{
		get
		{
			return _noTextOverlap;
		}
		set
		{
			_noTextOverlap = value;
			DirtyLayout();
		}
	}

	public bool NoGraphShader
	{
		get
		{
			return _noGraphShader;
		}
		set
		{
			_noGraphShader = value;
			DirtyLayout();
		}
	}

	public int GraphMaxWidth
	{
		get
		{
			return _graphMaxWidth;
		}
		set
		{
			_graphMaxWidth = value;
			DirtyLayout();
		}
	}

	public bool EnableObjectStats
	{
		get
		{
			return _enableObjectStats;
		}
		set
		{
			_enableObjectStats = value;
			DirtyLayout();
		}
	}

	private bool ShowMissingNetObjWarning
	{
		get
		{
			if (_enableObjectStats)
			{
				return Object == null;
			}
			return false;
		}
	}

	public NetworkObject Object
	{
		get
		{
			if (_object == null)
			{
				_object = GetComponentInParent<NetworkObject>();
			}
			return _object;
		}
	}

	public int ObjectTitleHeight
	{
		get
		{
			return _objectTitleHeight;
		}
		set
		{
			_objectTitleHeight = value;
			DirtyLayout();
		}
	}

	public int ObjectIdsHeight
	{
		get
		{
			return _objectIdsHeight;
		}
		set
		{
			_objectIdsHeight = value;
			DirtyLayout();
		}
	}

	public int ObjectMetersHeight
	{
		get
		{
			return _objectMetersHeight;
		}
		set
		{
			_objectIdsHeight = value;
			DirtyLayout();
		}
	}

	public NetworkRunner Runner
	{
		get
		{
			if (!Application.isPlaying)
			{
				return null;
			}
			if ((bool)_runner)
			{
				if (!_runner.IsShutdown)
				{
					return _runner;
				}
				Runner = null;
			}
			if ((bool)Object)
			{
				NetworkRunner runner = _object.Runner;
				if ((bool)runner && (!EnforceSingle || (runner.Mode & ConnectTo) != 0))
				{
					Runner = runner;
					return _runner;
				}
			}
			FusionStatsUtilities.TryFindActiveRunner(this, out var runner2, ConnectTo);
			Runner = runner2;
			return runner2;
		}
		set
		{
			if (!(_runner == value))
			{
				DisassociateWithRunner(_runner);
				_runner = value;
				AssociateWithRunner(value);
				UpdateTitle();
			}
		}
	}

	public Simulation.Statistics.ObjStatFlags IncludedObjectStats
	{
		get
		{
			return _includedObjStats;
		}
		set
		{
			_includedObjStats = value;
			_activeDirty = true;
		}
	}

	public Simulation.Statistics.NetStatFlags IncludedNetStats
	{
		get
		{
			return _includedNetStats;
		}
		set
		{
			_includedNetStats = value;
			_activeDirty = true;
		}
	}

	public Simulation.Statistics.SimStatFlags IncludedSimStats
	{
		get
		{
			return _includedSimStats;
		}
		set
		{
			_includedSimStats = value;
			_activeDirty = true;
		}
	}

	public bool ModifyColors => _modifyColors;

	public Color FontColor => _fontColor;

	public Color GraphColorGood => _graphColorGood;

	public Color GraphColorWarn => _graphColorWarn;

	public Color GraphColorBad => _graphColorBad;

	public Color GraphColorFlag => _graphColorFlag;

	public Color SimDataBackColor => _simDataBackColor;

	public Color NetDataBackColor => _netDataBackColor;

	public Color ObjDataBackColor => _objDataBackColor;

	public Rect CurrentRect
	{
		get
		{
			if (_canvasType != StatCanvasTypes.GameObject)
			{
				return _overlayRect;
			}
			return _gameObjectRect;
		}
	}

	private Shader Shader => Resources.Load<Shader>("FusionGraphShader");

	public static bool NewInputSystemFound
	{
		get
		{
			if (!_newInputSystemFound.HasValue)
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				for (int i = 0; i < assemblies.Length; i++)
				{
					Type[] types = assemblies[i].GetTypes();
					for (int j = 0; j < types.Length; j++)
					{
						if (types[j].Namespace == "UnityEngine.InputSystem")
						{
							_newInputSystemFound = true;
							return true;
						}
					}
				}
				_newInputSystemFound = false;
				return false;
			}
			return _newInputSystemFound.Value;
		}
	}

	private bool _graphsAreMissing => _canvasRT == null;

	internal static FusionStats CreateInternal(NetworkRunner runner = null, DefaultLayouts layout = DefaultLayouts.Left, Simulation.Statistics.NetStatFlags? netStatsMask = null, Simulation.Statistics.SimStatFlags? simStatsMask = null)
	{
		return Create(null, runner, layout, layout, netStatsMask, simStatsMask);
	}

	public static FusionStats Create(Transform parent = null, NetworkRunner runner = null, DefaultLayouts? screenLayout = null, DefaultLayouts? objectLayout = null, Simulation.Statistics.NetStatFlags? netStatsMask = null, Simulation.Statistics.SimStatFlags? simStatsMask = null)
	{
		GameObject gameObject = new GameObject("FusionStats " + (runner ? runner.name : "null"));
		if ((bool)parent)
		{
			gameObject.transform.SetParent(parent);
		}
		FusionStats fusionStats = gameObject.AddComponent<FusionStats>();
		fusionStats.ResetInternal(null, netStatsMask, simStatsMask, objectLayout, screenLayout);
		fusionStats.Runner = runner;
		if (runner != null)
		{
			fusionStats.AutoDestroy = true;
		}
		return fusionStats;
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void ResetStatics()
	{
		_statsForRunnerLookup.Clear();
		_activeGuids.Clear();
		_newInputSystemFound = null;
	}

	private void UpdateTitle()
	{
		string text = (_runner ? _runner.name : "Disconnected");
		if ((bool)_titleText)
		{
			_titleText.text = text;
		}
	}

	private void DirtyLayout(int minimumRefreshes = 1)
	{
		if (_layoutDirty < minimumRefreshes)
		{
			_layoutDirty = minimumRefreshes;
		}
	}

	private void ResetInternal(bool? enableObjectStats = null, Simulation.Statistics.NetStatFlags? netStatsMask = null, Simulation.Statistics.SimStatFlags? simStatsMask = null, DefaultLayouts? objectLayout = null, DefaultLayouts? screenLayout = null)
	{
		Canvas componentInChildren = GetComponentInChildren<Canvas>();
		if ((bool)componentInChildren)
		{
			UnityEngine.Object.DestroyImmediate(componentInChildren.gameObject);
		}
		if (!TryGetComponent<FusionStatsBillboard>(out var _))
		{
			base.gameObject.AddComponent<FusionStatsBillboard>().UpdateLookAt();
		}
		bool flag = GetComponentInParent<NetworkObject>();
		if (enableObjectStats == true || ((enableObjectStats ?? true) & flag))
		{
			EnableObjectStats = true;
			_includedObjStats = Simulation.Statistics.ObjStatFlags.Buffer;
			_includedSimStats = simStatsMask.GetValueOrDefault();
			_includedNetStats = netStatsMask.GetValueOrDefault();
			_canvasType = StatCanvasTypes.GameObject;
			EnforceSingle = false;
			GraphColumnCount = 1;
		}
		else
		{
			GraphColumnCount = 0;
			if ((bool)base.transform.parent)
			{
				_canvasType = StatCanvasTypes.GameObject;
				EnforceSingle = false;
			}
			else
			{
				_canvasType = StatCanvasTypes.Overlay;
				EnforceSingle = true;
			}
			_includedSimStats = simStatsMask ?? (Simulation.Statistics.SimStatFlags.ForwardSimCount | Simulation.Statistics.SimStatFlags.ResimCount | Simulation.Statistics.SimStatFlags.PacketSize);
			_includedNetStats = netStatsMask ?? (Simulation.Statistics.NetStatFlags.RoundTripTime | Simulation.Statistics.NetStatFlags.SentPacketSizes | Simulation.Statistics.NetStatFlags.ReceivedPacketSizes);
		}
		int num = (flag ? 4 : 5);
		ApplyDefaultLayout((DefaultLayouts)(((int?)objectLayout) ?? num), StatCanvasTypes.GameObject);
		ApplyDefaultLayout(screenLayout ?? DefaultLayouts.Right, StatCanvasTypes.Overlay);
		Guid = System.Guid.NewGuid().ToString().Substring(0, 13);
		GenerateGraphs();
	}

	private void Awake()
	{
		if ((bool)_guidesRT)
		{
			UnityEngine.Object.Destroy(_guidesRT.gameObject);
		}
		if (Application.isPlaying)
		{
			_foundViews = new List<IFusionStatsView>();
			GetComponentsInChildren(includeInactive: true, _foundViews);
		}
		if (Guid == "")
		{
			Guid = System.Guid.NewGuid().ToString().Substring(0, 13);
		}
		if (EnforceSingle && Guid != null)
		{
			if (_activeGuids.ContainsKey(Guid))
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			_activeGuids.Add(Guid, this);
		}
		if (EnforceSingle && base.transform.parent == null && _canvasType == StatCanvasTypes.Overlay)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	private void Start()
	{
		if (Application.isPlaying)
		{
			Initialize();
			_activeDirty = true;
			_layoutDirty = 2;
		}
	}

	private void OnDestroy()
	{
		DisassociateWithRunner(_runner);
		if (Guid != null && _activeGuids.TryGetValue(Guid, out var value) && value == this)
		{
			_activeGuids.Remove(Guid);
		}
	}

	[BehaviourButtonAction("Destroy Graphs", null, "_canvasRT", ConditionFlags = BehaviourActionAttribute.ActionFlags.ShowAtNotRuntime)]
	private void DestroyGraphs()
	{
		if ((bool)_canvasRT)
		{
			UnityEngine.Object.DestroyImmediate(_canvasRT.gameObject);
		}
		_canvasRT = null;
	}

	private void Initialize()
	{
		if (Application.isPlaying && !NewInputSystemFound && UnityEngine.Object.FindObjectOfType<EventSystem>() == null)
		{
			GameObject gameObject = new GameObject("Event System");
			gameObject.AddComponent<EventSystem>();
			gameObject.AddComponent<StandaloneInputModule>();
			if (Application.isPlaying)
			{
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
		}
		if (!_canvasRT)
		{
			GenerateGraphs();
		}
		if (!_canvasRT)
		{
			return;
		}
		_togglButton?.onClick.RemoveListener(Toggle);
		_canvsButton?.onClick.RemoveListener(ToggleCanvasType);
		_clearButton?.onClick.RemoveListener(Clear);
		_pauseButton?.onClick.RemoveListener(Pause);
		_closeButton?.onClick.RemoveListener(Close);
		_titleButton?.onClick.RemoveListener(PingSelectFusionStats);
		_objctButton?.onClick.RemoveListener(PingSelectObject);
		_togglButton?.onClick.AddListener(Toggle);
		_canvsButton?.onClick.AddListener(ToggleCanvasType);
		_clearButton?.onClick.AddListener(Clear);
		_pauseButton?.onClick.AddListener(Pause);
		_closeButton?.onClick.AddListener(Close);
		_titleButton?.onClick.AddListener(PingSelectFusionStats);
		_objctButton?.onClick.AddListener(PingSelectObject);
		GetComponentsInChildren(includeInactive: true, _foundViews);
		foreach (IFusionStatsView foundView in _foundViews)
		{
			foundView.Initialize();
		}
		_layoutDirty = 1;
	}

	[BehaviourButtonAction("Generate Graphs", null, "_graphsAreMissing", ConditionFlags = BehaviourActionAttribute.ActionFlags.ShowAtNotRuntime)]
	private void GenerateGraphs()
	{
		Transform component = base.gameObject.GetComponent<Transform>();
		_canvasRT = component.CreateRectTransform("Stats Canvas");
		_canvas = _canvasRT.gameObject.AddComponent<Canvas>();
		_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		if ((bool)Runner && Runner.IsRunning)
		{
			RunnerVisibilityNode.AddVisibilityNodes(_canvasRT.gameObject, Runner);
		}
		CanvasScaler canvasScaler = _canvasRT.gameObject.AddComponent<CanvasScaler>();
		canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		canvasScaler.referenceResolution = new Vector2(1080f, 1080f);
		canvasScaler.matchWidthOrHeight = 0.4f;
		_canvasRT.gameObject.AddComponent<GraphicRaycaster>();
		_rootPanelRT = _canvasRT.CreateRectTransform("Root Panel");
		_headerRT = _rootPanelRT.CreateRectTransform("Header Panel").AddCircleSprite(PanelColor);
		_titleRT = _headerRT.CreateRectTransform("Runner Title").SetAnchors(0f, 1f, 0.75f, 1f).SetOffsets(6f, -6f, 0f, -6f);
		_titleButton = _titleRT.gameObject.AddComponent<Button>();
		_titleText = _titleRT.AddText(_runner ? _runner.name : "Disconnected", TextAnchor.UpperCenter, _fontColor);
		_titleText.raycastTarget = true;
		_buttonsRT = _headerRT.CreateRectTransform("Buttons").SetAnchors(0f, 1f, 0f, 0.75f).SetOffsets(6f, -6f, 6f, 0f);
		HorizontalLayoutGroup horizontalLayoutGroup = _buttonsRT.gameObject.AddComponent<HorizontalLayoutGroup>();
		horizontalLayoutGroup.childControlHeight = true;
		horizontalLayoutGroup.childControlWidth = true;
		horizontalLayoutGroup.spacing = 6f;
		_buttonsRT.MakeButton(ref _togglButton, "▼", "HIDE", out _togglIcon, out _togglLabel, Toggle);
		_buttonsRT.MakeButton(ref _canvsButton, "ﬦ", "CANVAS", out _canvsIcon, out _canvsLabel, ToggleCanvasType);
		_buttonsRT.MakeButton(ref _pauseButton, "װ", "PAUSE", out _pauseIcon, out _pauseLabel, Pause);
		_buttonsRT.MakeButton(ref _clearButton, "ᴓ", "CLEAR", out _clearIcon, out _clearLabel, Clear);
		_buttonsRT.MakeButton(ref _closeButton, "x", "CLOSE", out _closeIcon, out _closeLabel, Close);
		_togglIcon.rectTransform.anchorMax = new Vector2(1f, 0.85f);
		_statsPanelRT = _rootPanelRT.CreateRectTransform("Stats Panel").AddCircleSprite(PanelColor);
		_objectTitlePanelRT = _statsPanelRT.CreateRectTransform("Object Name Panel").ExpandTopAnchor(6f).AddCircleSprite(_objDataBackColor);
		_objctButton = _objectTitlePanelRT.gameObject.AddComponent<Button>();
		RectTransform rt = _objectTitlePanelRT.CreateRectTransform("Object Name").SetAnchors(0f, 1f, 0.15f, 0.85f).SetOffsets(10f, -10f, 0f, 0f);
		_objectNameText = rt.AddText("Object Name", TextAnchor.MiddleCenter, _fontColor);
		_objectNameText.alignByGeometry = false;
		_objectNameText.raycastTarget = false;
		_objectIdsGroupRT = FusionStatsObjectIds.Create(_statsPanelRT, this);
		_objectMetersPanelRT = _statsPanelRT.CreateRectTransform("Object Meters Layout").ExpandTopAnchor(6f).AddVerticalLayoutGroup(6f);
		FusionStatsMeterBar.Create(_objectMetersPanelRT, this, Simulation.Statistics.StatSourceTypes.NetworkObject, 0, 15f, 30f);
		FusionStatsMeterBar.Create(_objectMetersPanelRT, this, Simulation.Statistics.StatSourceTypes.NetworkObject, 1, 3f, 6f);
		_graphsLayoutRT = _statsPanelRT.CreateRectTransform("Graphs Layout").ExpandAnchor().SetOffsets(6f, 0f, 0f, 0f);
		_graphGridLayoutGroup = _graphsLayoutRT.AddGridlLayoutGroup(6f);
		_objGraphs = new FusionGraph[2];
		for (int i = 0; i < 2; i++)
		{
			if (InitializeAllGraphs || ((uint)(1 << i) & (uint)_includedObjStats) != 0)
			{
				CreateGraph(Simulation.Statistics.StatSourceTypes.NetworkObject, i, _graphsLayoutRT);
			}
		}
		_netGraphs = new FusionGraph[3];
		for (int j = 0; j < 3; j++)
		{
			if (InitializeAllGraphs || ((uint)(1 << j) & (uint)_includedNetStats) != 0)
			{
				CreateGraph(Simulation.Statistics.StatSourceTypes.NetConnection, j, _graphsLayoutRT);
			}
		}
		_simGraphs = new FusionGraph[16];
		for (int k = 0; k < 16; k++)
		{
			if (InitializeAllGraphs || ((uint)(1 << k) & (uint)_includedSimStats) != 0)
			{
				CreateGraph(Simulation.Statistics.StatSourceTypes.Simulation, k, _graphsLayoutRT);
			}
		}
		_activeDirty = true;
		_layoutDirty = 2;
	}

	private void AssociateWithRunner(NetworkRunner runner)
	{
		if (runner != null)
		{
			if (!_statsForRunnerLookup.TryGetValue(runner, out var value))
			{
				_statsForRunnerLookup.Add(runner, new List<FusionStats> { this });
			}
			else
			{
				value.Add(this);
			}
		}
	}

	private void DisassociateWithRunner(NetworkRunner runner)
	{
		if (runner != null && _statsForRunnerLookup.TryGetValue(runner, out var value) && value.Contains(this))
		{
			value.Remove(this);
		}
	}

	private void Pause()
	{
		if (!_runner || _runner.Simulation == null)
		{
			return;
		}
		_paused = !_paused;
		string text = (_paused ? "►" : "װ");
		string text2 = (_paused ? "PLAY" : "PAUSE");
		_pauseIcon.text = text;
		_pauseLabel.text = text2;
		if (!_statsForRunnerLookup.TryGetValue(_runner, out var value))
		{
			return;
		}
		bool flag = false;
		foreach (FusionStats item in value)
		{
			if (!item._paused)
			{
				flag = true;
				break;
			}
		}
		_runner.Simulation.Stats.Pause(!flag);
	}

	private void Toggle()
	{
		_hidden = !_hidden;
		_togglIcon.text = (_hidden ? "▲" : "▼");
		_togglLabel.text = (_hidden ? "SHOW" : "HIDE");
		_statsPanelRT.gameObject.SetActive(!_hidden);
		for (int i = 0; i < _simGraphs.Length; i++)
		{
			if ((bool)_simGraphs[i])
			{
				_simGraphs[i].gameObject.SetActive(!_hidden && ((uint)(1 << i) & (uint)_includedSimStats) != 0);
			}
		}
		for (int j = 0; j < _objGraphs.Length; j++)
		{
			if ((bool)_objGraphs[j])
			{
				_objGraphs[j].gameObject.SetActive(!_hidden && ((uint)(1 << j) & (uint)_includedObjStats) != 0);
			}
		}
		for (int k = 0; k < _netGraphs.Length; k++)
		{
			if ((bool)_netGraphs[k])
			{
				_netGraphs[k].gameObject.SetActive(!_hidden && ((uint)(1 << k) & (uint)_includedNetStats) != 0);
			}
		}
	}

	private void Clear()
	{
		if ((bool)_runner && _runner.Simulation != null)
		{
			_runner.Simulation.Stats.Clear();
		}
		for (int i = 0; i < _simGraphs.Length; i++)
		{
			if ((bool)_simGraphs[i])
			{
				_simGraphs[i].Clear();
			}
		}
		for (int j = 0; j < _objGraphs.Length; j++)
		{
			if ((bool)_objGraphs[j])
			{
				_objGraphs[j].Clear();
			}
		}
		for (int k = 0; k < _netGraphs.Length; k++)
		{
			if ((bool)_netGraphs[k])
			{
				_netGraphs[k].Clear();
			}
		}
	}

	private void ToggleCanvasType()
	{
		_canvasType = ((_canvasType != StatCanvasTypes.GameObject) ? StatCanvasTypes.GameObject : StatCanvasTypes.Overlay);
		_layoutDirty = 3;
		CalculateLayout();
	}

	private void Close()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void PingSelectObject()
	{
	}

	private void PingSelectFusionStats()
	{
	}

	private void LateUpdate()
	{
		NetworkRunner runner = Runner;
		bool flag = runner == null;
		if (AutoDestroy & flag)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if (_activeDirty)
		{
			ReapplyEnabled();
		}
		if (_layoutDirty > 0)
		{
			CalculateLayout();
		}
		if (!Application.isPlaying || flag || runner.IsShutdown || _paused)
		{
			return;
		}
		if (RedrawInterval > 0f)
		{
			double timeAsDouble = Time.timeAsDouble;
			if (timeAsDouble > _delayDrawUntil)
			{
				for (_currentDrawTime = timeAsDouble; _delayDrawUntil <= timeAsDouble; _delayDrawUntil += RedrawInterval)
				{
				}
			}
			if (timeAsDouble != _currentDrawTime)
			{
				return;
			}
		}
		if (EnableObjectStats)
		{
			RefreshObjectValues();
		}
		foreach (IFusionStatsView foundView in _foundViews)
		{
			if (foundView != null && foundView.isActiveAndEnabled)
			{
				foundView.Refresh();
			}
		}
	}

	private void RefreshObjectValues()
	{
		NetworkObject networkObject = Object;
		if (!(networkObject == null))
		{
			string text = networkObject.name;
			if (_previousObjectTitle != text)
			{
				_objectNameText.text = text;
				_previousObjectTitle = text;
			}
		}
	}

	public FusionGraph CreateGraph(Simulation.Statistics.StatSourceTypes type, int statId, RectTransform parentRT)
	{
		FusionGraph fusionGraph = FusionGraph.Create(this, type, statId, parentRT);
		switch (type)
		{
		case Simulation.Statistics.StatSourceTypes.Simulation:
			_simGraphs[statId] = fusionGraph;
			if (((uint)_includedSimStats & (uint)(1 << statId)) == 0)
			{
				fusionGraph.gameObject.SetActive(value: false);
			}
			break;
		case Simulation.Statistics.StatSourceTypes.NetworkObject:
			_objGraphs[statId] = fusionGraph;
			if (((uint)_includedObjStats & (uint)(1 << statId)) == 0)
			{
				fusionGraph.gameObject.SetActive(value: false);
			}
			break;
		default:
			_netGraphs[statId] = fusionGraph;
			if (((uint)_includedNetStats & (uint)(1 << statId)) == 0)
			{
				fusionGraph.gameObject.SetActive(value: false);
			}
			break;
		}
		return fusionGraph;
	}

	private void ReapplyEnabled()
	{
		_activeDirty = false;
		if (_simGraphs == null || _simGraphs.Length < 0 || _graphsLayoutRT == null)
		{
			return;
		}
		for (int i = 0; i < _simGraphs.Length; i++)
		{
			FusionGraph fusionGraph = _simGraphs[i];
			bool flag = ((uint)(1 << i) & (uint)_includedSimStats) != 0;
			if (fusionGraph == null)
			{
				if (!flag)
				{
					continue;
				}
				fusionGraph = CreateGraph(Simulation.Statistics.StatSourceTypes.Simulation, i, _graphsLayoutRT);
				_simGraphs[i] = fusionGraph;
			}
			fusionGraph.gameObject.SetActive(flag);
		}
		for (int j = 0; j < _objGraphs.Length; j++)
		{
			FusionGraph fusionGraph2 = _objGraphs[j];
			bool flag2 = _enableObjectStats && ((uint)(1 << j) & (uint)_includedObjStats) != 0;
			if (fusionGraph2 == null)
			{
				if (!flag2)
				{
					continue;
				}
				fusionGraph2 = CreateGraph(Simulation.Statistics.StatSourceTypes.NetworkObject, j, _graphsLayoutRT);
				_objGraphs[j] = fusionGraph2;
			}
			if (_objGraphs[j] != null)
			{
				fusionGraph2.gameObject.SetActive(flag2);
			}
		}
		for (int k = 0; k < _netGraphs.Length; k++)
		{
			FusionGraph fusionGraph3 = _netGraphs[k];
			bool flag3 = ((uint)(1 << k) & (uint)_includedNetStats) != 0;
			if (fusionGraph3 == null)
			{
				if (!flag3)
				{
					continue;
				}
				fusionGraph3 = CreateGraph(Simulation.Statistics.StatSourceTypes.NetConnection, k, _graphsLayoutRT);
				_netGraphs[k] = fusionGraph3;
			}
			if (_netGraphs[k] != null)
			{
				fusionGraph3.gameObject.SetActive(flag3);
			}
		}
	}

	private void CalculateLayout()
	{
		if (_rootPanelRT == null || _graphsLayoutRT == null)
		{
			return;
		}
		if (_foundGraphs == null)
		{
			_foundGraphs = new List<FusionGraph>(_graphsLayoutRT.GetComponentsInChildren<FusionGraph>(includeInactive: false));
		}
		else
		{
			GetComponentsInChildren(includeInactive: false, _foundGraphs);
		}
		float time = Time.time;
		if (_lastLayoutUpdate < time)
		{
			_layoutDirty--;
			_lastLayoutUpdate = time;
		}
		if (_layoutDirty <= 0)
		{
			_ = _canvas.enabled;
		}
		if (!_rootPanelRT)
		{
			return;
		}
		float num = Math.Min(_maxHeaderHeight, _rootPanelRT.rect.width / 4f);
		if (_canvasType == StatCanvasTypes.GameObject)
		{
			_canvas.renderMode = RenderMode.WorldSpace;
			float num2 = CanvasScale / 1080f;
			_canvasRT.localScale = new Vector3(num2, num2, num2);
			_canvasRT.sizeDelta = new Vector2(1024f, 1024f);
			_canvasRT.localPosition = new Vector3(0f, 0f, CanvasDistance);
			if (!_canvasRT.GetComponent<FusionStatsBillboard>())
			{
				_canvasRT.localRotation = default;
			}
		}
		else
		{
			_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		}
		_objectTitlePanelRT.gameObject.SetActive(_enableObjectStats);
		_objectIdsGroupRT.gameObject.SetActive(_enableObjectStats);
		_objectMetersPanelRT.gameObject.SetActive(_enableObjectStats);
		Vector2 vector = ((!_showButtonLabels) ? new Vector2(0f, 0f) : new Vector2(0f, 0.0875f));
		_togglIcon.rectTransform.anchorMin = vector + new Vector2(0f, 0.15f);
		_canvsIcon.rectTransform.anchorMin = vector;
		_clearIcon.rectTransform.anchorMin = vector;
		_pauseIcon.rectTransform.anchorMin = vector;
		_closeIcon.rectTransform.anchorMin = vector;
		_togglLabel.gameObject.SetActive(_showButtonLabels);
		_canvsLabel.gameObject.SetActive(_showButtonLabels);
		_clearLabel.gameObject.SetActive(_showButtonLabels);
		_pauseLabel.gameObject.SetActive(_showButtonLabels);
		_closeLabel.gameObject.SetActive(_showButtonLabels);
		Rect currentRect = CurrentRect;
		_rootPanelRT.anchorMax = new Vector2(currentRect.xMax, currentRect.yMax);
		_rootPanelRT.anchorMin = new Vector2(currentRect.xMin, currentRect.yMin);
		_rootPanelRT.sizeDelta = new Vector2(0f, 0f);
		_rootPanelRT.pivot = new Vector2(0.5f, 0.5f);
		_rootPanelRT.anchoredPosition3D = default;
		_headerRT.anchorMin = new Vector2(0f, 1f);
		_headerRT.anchorMax = new Vector2(1f, 1f);
		_headerRT.pivot = new Vector2(0.5f, 1f);
		_headerRT.anchoredPosition3D = default;
		_headerRT.sizeDelta = new Vector2(0f, num);
		_objectTitlePanelRT.offsetMax = new Vector2(-6f, -6f);
		_objectTitlePanelRT.offsetMin = new Vector2(6f, -ObjectTitleHeight);
		_objectIdsGroupRT.offsetMax = new Vector2(-6f, -(ObjectTitleHeight + 6));
		_objectIdsGroupRT.offsetMin = new Vector2(6f, -(ObjectTitleHeight + ObjectIdsHeight));
		_objectMetersPanelRT.offsetMax = new Vector2(-6f, -(ObjectTitleHeight + ObjectIdsHeight + 6));
		_objectMetersPanelRT.offsetMin = new Vector2(6f, -(ObjectTitleHeight + ObjectIdsHeight + ObjectMetersHeight));
		_objectTitlePanelRT.gameObject.SetActive(EnableObjectStats && ObjectTitleHeight > 0);
		_objectIdsGroupRT.gameObject.SetActive(EnableObjectStats && ObjectIdsHeight > 0);
		_objectMetersPanelRT.gameObject.SetActive(EnableObjectStats && ObjectMetersHeight > 0);
		_statsPanelRT.ExpandAnchor().SetOffsets(0f, 0f, 0f, 0f - num);
		if (_enableObjectStats && _statsPanelRT.rect.height < (float)(ObjectTitleHeight + ObjectIdsHeight + ObjectMetersHeight))
		{
			_statsPanelRT.offsetMin = new Vector2(0f, _statsPanelRT.rect.height - (float)(ObjectTitleHeight + ObjectIdsHeight + ObjectMetersHeight + 6));
		}
		int num3 = ((GraphColumnCount > 0) ? GraphColumnCount : ((int)(_graphsLayoutRT.rect.width / (float)(_graphMaxWidth + 6))));
		if (num3 < 1)
		{
			num3 = 1;
		}
		int num4 = (int)Math.Ceiling((double)_foundGraphs.Count / (double)num3);
		if (num4 < 1)
		{
			num4 = 1;
		}
		if (num4 == 1)
		{
			num3 = _foundGraphs.Count;
		}
		_graphGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		_graphGridLayoutGroup.constraintCount = num3;
		float x = _graphsLayoutRT.rect.width / (float)num3 - 6f;
		float y = _graphsLayoutRT.rect.height / (float)num4 - 6f;
		_graphGridLayoutGroup.cellSize = new Vector2(x, y);
		_graphsLayoutRT.offsetMax = new Vector2(0f, _enableObjectStats ? (-(ObjectTitleHeight + ObjectIdsHeight + ObjectMetersHeight + 6)) : (-6));
		if (_foundViews == null)
		{
			_foundViews = new List<IFusionStatsView>(GetComponentsInChildren<IFusionStatsView>(includeInactive: false));
		}
		else
		{
			GetComponentsInChildren(includeInactive: false, _foundViews);
		}
		if (_objGraphs != null)
		{
			FusionGraph[] objGraphs = _objGraphs;
			foreach (FusionGraph fusionGraph in objGraphs)
			{
				if ((bool)fusionGraph)
				{
					fusionGraph.gameObject.SetActive(((uint)_includedObjStats & (uint)(1 << fusionGraph.StatId)) != 0 && _enableObjectStats);
				}
			}
		}
		for (int j = 0; j < _foundViews.Count; j++)
		{
			IFusionStatsView fusionStatsView = _foundViews[j];
			if (fusionStatsView != null && fusionStatsView.isActiveAndEnabled)
			{
				fusionStatsView.CalculateLayout();
				fusionStatsView.transform.localRotation = default;
				fusionStatsView.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
	}

	private void ApplyDefaultLayout(DefaultLayouts defaults, StatCanvasTypes? applyForCanvasType = null)
	{
		bool flag = !applyForCanvasType.HasValue || applyForCanvasType.Value == StatCanvasTypes.GameObject;
		bool flag2 = !applyForCanvasType.HasValue || applyForCanvasType.Value == StatCanvasTypes.Overlay;
		if (defaults != DefaultLayouts.Custom)
		{
			bool flag3 = Screen.height > Screen.width;
			Rect rect;
			Rect overlayRect;
			switch (defaults)
			{
			case DefaultLayouts.Left:
				rect = Rect.MinMaxRect(0f, 0f, 0.3f, 1f);
				overlayRect = rect;
				break;
			case DefaultLayouts.Right:
				rect = Rect.MinMaxRect(0.7f, 0f, 1f, 1f);
				overlayRect = rect;
				break;
			case DefaultLayouts.UpperLeft:
				rect = Rect.MinMaxRect(0f, 0.5f, 0.3f, 1f);
				overlayRect = (flag3 ? Rect.MinMaxRect(0f, 0.7f, 0.3f, 1f) : rect);
				break;
			case DefaultLayouts.UpperRight:
				rect = Rect.MinMaxRect(0.7f, 0.5f, 1f, 1f);
				overlayRect = (flag3 ? Rect.MinMaxRect(0.7f, 0.7f, 1f, 1f) : rect);
				break;
			case DefaultLayouts.Full:
				rect = Rect.MinMaxRect(0f, 0f, 1f, 1f);
				overlayRect = rect;
				break;
			default:
				rect = Rect.MinMaxRect(0f, 0.5f, 0.3f, 1f);
				overlayRect = rect;
				break;
			}
			if (flag)
			{
				GameObjectRect = rect;
			}
			if (flag2)
			{
				OverlayRect = overlayRect;
			}
			_layoutDirty++;
		}
	}
}
