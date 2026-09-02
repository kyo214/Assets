using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkDebugStart))]
[AddComponentMenu("Fusion/Network Debug Start GUI")]
[ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
public class NetworkDebugStartGUI : Fusion.Behaviour
{
	[InlineHelp]
	public bool EnableHotkeys;

	[InlineHelp]
	public GUISkin BaseSkin;

	private NetworkDebugStart _networkDebugStart;

	private string _clientCount;

	private bool _isMultiplePeerMode;

	private Dictionary<NetworkDebugStart.Stage, string> _nicifiedStageNames;

	protected virtual void OnValidate()
	{
		ValidateClientCount();
	}

	protected void ValidateClientCount()
	{
		if (_clientCount == null)
		{
			_clientCount = "1";
		}
		else
		{
			_clientCount = Regex.Replace(_clientCount, "[^0-9]", "");
		}
	}

	protected int GetClientCount()
	{
		try
		{
			return Convert.ToInt32(_clientCount);
		}
		catch
		{
			return 0;
		}
	}

	protected virtual void Awake()
	{
		_nicifiedStageNames = ConvertEnumToNicifiedNameLookup<NetworkDebugStart.Stage>("Fusion Status: ");
		_networkDebugStart = EnsureNetworkDebugStartExists();
		_clientCount = _networkDebugStart.AutoClients.ToString();
		ValidateClientCount();
	}

	protected virtual void Start()
	{
		_isMultiplePeerMode = NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple;
	}

	protected NetworkDebugStart EnsureNetworkDebugStartExists()
	{
		if ((bool)_networkDebugStart && _networkDebugStart.gameObject == base.gameObject)
		{
			return _networkDebugStart;
		}
		if (TryGetBehaviour<NetworkDebugStart>(out var behaviour))
		{
			_networkDebugStart = behaviour;
			return behaviour;
		}
		_networkDebugStart = AddBehaviour<NetworkDebugStart>();
		return _networkDebugStart;
	}

	private void Update()
	{
		NetworkDebugStart networkDebugStart = EnsureNetworkDebugStartExists();
		if (networkDebugStart.StartMode != NetworkDebugStart.StartModes.UserInterface || networkDebugStart.CurrentStage != NetworkDebugStart.Stage.Disconnected || !EnableHotkeys)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.I))
		{
			_networkDebugStart.StartSinglePlayer();
		}
		if (Input.GetKeyDown(KeyCode.H))
		{
			if (_isMultiplePeerMode)
			{
				StartHostWithClients(_networkDebugStart);
			}
			else
			{
				_networkDebugStart.StartHost();
			}
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			if (_isMultiplePeerMode)
			{
				StartServerWithClients(_networkDebugStart);
			}
			else
			{
				_networkDebugStart.StartServer();
			}
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			if (_isMultiplePeerMode)
			{
				StartMultipleClients(networkDebugStart);
			}
			else
			{
				networkDebugStart.StartClient();
			}
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			if (_isMultiplePeerMode)
			{
				StartMultipleAutoClients(networkDebugStart);
			}
			else
			{
				networkDebugStart.StartAutoClient();
			}
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			if (_isMultiplePeerMode)
			{
				StartMultipleSharedClients(networkDebugStart);
			}
			else
			{
				networkDebugStart.StartSharedClient();
			}
		}
	}

	protected virtual void OnGUI()
	{
		NetworkDebugStart networkDebugStart = EnsureNetworkDebugStartExists();
		if (networkDebugStart.StartMode != NetworkDebugStart.StartModes.UserInterface)
		{
			return;
		}
		NetworkDebugStart.Stage currentStage = networkDebugStart.CurrentStage;
		if (networkDebugStart.AutoHideGUI && currentStage == NetworkDebugStart.Stage.AllConnected)
		{
			return;
		}
		GUISkin skin = GUI.skin;
		GUI.skin = FusionScalableIMGUI.GetScaledSkin(BaseSkin, out var height, out var width, out var _, out var margin, out var boxLeft);
		GUILayout.BeginArea(new Rect(boxLeft, margin, width, Screen.height));
		GUILayout.BeginVertical(GUI.skin.window);
		GUILayout.BeginHorizontal(GUILayout.Height(height));
		GUILayout.Label(_nicifiedStageNames.TryGetValue(networkDebugStart.CurrentStage, out var value) ? value : "Unrecognized Stage", new GUIStyle(GUI.skin.label)
		{
			fontSize = (int)((float)GUI.skin.label.fontSize * 0.8f),
			alignment = TextAnchor.UpperLeft
		});
		if (!networkDebugStart.AutoHideGUI && networkDebugStart.CurrentStage == NetworkDebugStart.Stage.AllConnected && GUILayout.Button("X", GUILayout.ExpandHeight(expand: true), GUILayout.Width(height)))
		{
			networkDebugStart.AutoHideGUI = true;
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.BeginVertical(GUI.skin.window);
		if (currentStage == NetworkDebugStart.Stage.Disconnected)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Room:", GUILayout.Height(height), GUILayout.Width(width * 0.33f));
			networkDebugStart.DefaultRoomName = GUILayout.TextField(networkDebugStart.DefaultRoomName, 25, GUILayout.Height(height));
			GUILayout.EndHorizontal();
			if (GUILayout.Button(EnableHotkeys ? "Start Single Player (I)" : "Start Single Player", GUILayout.Height(height)))
			{
				networkDebugStart.StartSinglePlayer();
			}
			if (GUILayout.Button(EnableHotkeys ? "Start Shared Client (P)" : "Start Shared Client", GUILayout.Height(height)))
			{
				if (_isMultiplePeerMode)
				{
					StartMultipleSharedClients(networkDebugStart);
				}
				else
				{
					networkDebugStart.StartSharedClient();
				}
			}
			if (GUILayout.Button(EnableHotkeys ? "Start Server (S)" : "Start Server", GUILayout.Height(height)))
			{
				if (_isMultiplePeerMode)
				{
					StartServerWithClients(networkDebugStart);
				}
				else
				{
					networkDebugStart.StartServer();
				}
			}
			if (GUILayout.Button(EnableHotkeys ? "Start Host (H)" : "Start Host", GUILayout.Height(height)))
			{
				if (_isMultiplePeerMode)
				{
					StartHostWithClients(networkDebugStart);
				}
				else
				{
					networkDebugStart.StartHost();
				}
			}
			if (GUILayout.Button(EnableHotkeys ? "Start Client (C)" : "Start Client", GUILayout.Height(height)))
			{
				if (_isMultiplePeerMode)
				{
					StartMultipleClients(networkDebugStart);
				}
				else
				{
					networkDebugStart.StartClient();
				}
			}
			if (GUILayout.Button(EnableHotkeys ? "Start Auto Host Or Client (A)" : "Start Auto Host Or Client", GUILayout.Height(height)))
			{
				if (_isMultiplePeerMode)
				{
					StartMultipleAutoClients(networkDebugStart);
				}
				else
				{
					networkDebugStart.StartAutoClient();
				}
			}
			if (_isMultiplePeerMode)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Client Count:", GUILayout.Height(height));
				GUILayout.Label("", GUILayout.Width(4f));
				string text = GUILayout.TextField(_clientCount, 10, GUILayout.Width(width * 0.25f), GUILayout.Height(height));
				if (_clientCount != text)
				{
					_clientCount = text;
					ValidateClientCount();
				}
				GUILayout.EndHorizontal();
			}
		}
		else if (GUILayout.Button("Shutdown", GUILayout.Height(height)))
		{
			_networkDebugStart.ShutdownAll();
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
		GUI.skin = skin;
	}

	private void StartHostWithClients(NetworkDebugStart nds)
	{
		int clientCount;
		try
		{
			clientCount = Convert.ToInt32(_clientCount);
		}
		catch
		{
			clientCount = 0;
		}
		nds.StartHostPlusClients(clientCount);
	}

	private void StartServerWithClients(NetworkDebugStart nds)
	{
		int clientCount;
		try
		{
			clientCount = Convert.ToInt32(_clientCount);
		}
		catch
		{
			clientCount = 0;
		}
		nds.StartServerPlusClients(clientCount);
	}

	private void StartMultipleClients(NetworkDebugStart nds)
	{
		int clientCount;
		try
		{
			clientCount = Convert.ToInt32(_clientCount);
		}
		catch
		{
			clientCount = 0;
		}
		nds.StartMultipleClients(clientCount);
	}

	private void StartMultipleAutoClients(NetworkDebugStart nds)
	{
		int.TryParse(_clientCount, out var result);
		nds.StartMultipleAutoClients(result);
	}

	private void StartMultipleSharedClients(NetworkDebugStart nds)
	{
		int clientCount;
		try
		{
			clientCount = Convert.ToInt32(_clientCount);
		}
		catch
		{
			clientCount = 0;
		}
		nds.StartMultipleSharedClients(clientCount);
	}

	public static Dictionary<T, string> ConvertEnumToNicifiedNameLookup<T>(string prefix = null, Dictionary<T, string> nonalloc = null) where T : Enum
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (nonalloc == null)
		{
			nonalloc = new Dictionary<T, string>();
		}
		else
		{
			nonalloc.Clear();
		}
		string[] names = Enum.GetNames(typeof(T));
		Array values = Enum.GetValues(typeof(T));
		int i = 0;
		for (int num = names.Length; i < num; i++)
		{
			stringBuilder.Clear();
			if (prefix != null)
			{
				stringBuilder.Append(prefix);
			}
			string text = names[i];
			for (int j = 0; j < text.Length; j++)
			{
				if (char.IsUpper(text[j]) && j != 0)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(text[j]);
			}
			nonalloc.Add((T)values.GetValue(i), stringBuilder.ToString());
		}
		return nonalloc;
	}
}
