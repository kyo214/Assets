using Dissonance.VAD;
using UnityEngine;

namespace Dissonance;

[HelpURL("https://placeholder-software.co.uk/dissonance/docs/Reference/Components/Voice-Proximity-Broadcast-Trigger/")]
public class VoiceProximityBroadcastTrigger : BaseProximityTrigger<RoomChannel>, IVoiceActivationListener, IVoiceBroadcastTrigger
{
	private class BroadcastGrid : Grid
	{
		private readonly VoiceProximityBroadcastTrigger _parent;

		public BroadcastGrid(VoiceProximityBroadcastTrigger parent)
			: base((BaseProximityTrigger<RoomChannel>)parent)
		{
			_parent = parent;
		}

		protected override RoomChannel CreateHandle(Vector3Int id, string name)
		{
			return base.Parent.Comms.RoomChannels.Open(new RoomName(name, suppress: true), positional: true, _parent.Priority);
		}

		protected override void CloseHandle(RoomChannel handle)
		{
			handle.Dispose();
		}
	}

	[SerializeField]
	private bool _roomExpanded = true;

	[SerializeField]
	private bool _metadataExpanded;

	[SerializeField]
	private bool _activationModeExpanded;

	[SerializeField]
	private bool _tokensExpanded;

	private bool _isVadSpeaking;

	private CommActivationMode? _previousMode;

	[SerializeField]
	private string _inputName;

	[SerializeField]
	private CommActivationMode _mode = CommActivationMode.VoiceActivation;

	[SerializeField]
	private bool _muted;

	[SerializeField]
	private ChannelPriority _prority;

	public string InputName
	{
		get
		{
			return _inputName;
		}
		set
		{
			_inputName = value;
		}
	}

	public CommActivationMode Mode
	{
		get
		{
			return _mode;
		}
		set
		{
			_mode = value;
		}
	}

	public bool IsMuted
	{
		get
		{
			return _muted;
		}
		set
		{
			if (value)
			{
				CloseChannels();
			}
			_muted = value;
		}
	}

	public bool IsTransmitting => base.ActiveChannelCount != 0;

	public ChannelPriority Priority
	{
		get
		{
			return _prority;
		}
		set
		{
			_prority = value;
			CloseChannels();
		}
	}

	public override bool CanTrigger
	{
		get
		{
			if (!IsMuted)
			{
				return base.CanTrigger;
			}
			return false;
		}
	}

	public void ToggleMute()
	{
		IsMuted = !IsMuted;
	}

	protected override bool IsUserActivated()
	{
		switch (Mode)
		{
		case CommActivationMode.VoiceActivation:
			return _isVadSpeaking;
		case CommActivationMode.PushToTalk:
			return Input.GetAxis(InputName) > 0.5f;
		case CommActivationMode.Open:
			return true;
		case CommActivationMode.None:
			return false;
		default:
			Log.Error("Unknown Activation Mode '{0}'", Mode);
			return false;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (base.Comms != null)
		{
			base.Comms.SubscribeToVoiceActivation(this);
		}
	}

	protected override void OnDisable()
	{
		if (base.Comms != null)
		{
			base.Comms.UnsubscribeFromVoiceActivation(this);
		}
		base.OnDisable();
	}

	protected override void OnDestroy()
	{
		if (base.Comms != null)
		{
			base.Comms.UnsubscribeFromVoiceActivation(this);
		}
		base.OnDestroy();
	}

	protected override void Update()
	{
		if (_mode != _previousMode)
		{
			CloseChannels();
		}
		_previousMode = _mode;
		base.Update();
	}

	protected override Grid CreateGrid()
	{
		return new BroadcastGrid(this);
	}

	void IVoiceActivationListener.VoiceActivationStart()
	{
		_isVadSpeaking = true;
	}

	void IVoiceActivationListener.VoiceActivationStop()
	{
		_isVadSpeaking = false;
	}
}
