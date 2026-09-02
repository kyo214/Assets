using DarkTonic.MasterAudio.Multiplayer;
using UnityEngine;

namespace DarkTonic.MasterAudio;

public class MechanimStateCustomEvents : StateMachineBehaviour
{
	[Tooltip("Select for event to re-fire each time animation loops without exiting state")]
	[Header("Retrigger Events Each Time Anim Loops w/o Exiting State")]
	public bool RetriggerWhenStateLoops;

	[Header("Select For Events To Be Received By All Connected Players")]
	public bool MultiplayerBroadcast;

	[Tooltip("Fire A Custom Event When State Is Entered")]
	[Header("Enter Custom Event")]
	public bool fireEnterEvent;

	[MasterCustomEvent]
	public string enterCustomEvent = "[None]";

	[Tooltip("Fire a Custom Event when state is Exited")]
	[Header("Exit Custom Event")]
	public bool fireExitEvent;

	[MasterCustomEvent]
	public string exitCustomEvent = "[None]";

	[Tooltip("Fire a Custom Event timed to the animation state's normalized time.  Normalized time is simply the length in time of the animation.  Time is represented as a float from 0f - 1f.  0f is the beginning, .5f is the middle, 1f is the end...etc.etc.  Select a Start time from 0 - 1.")]
	[Header("Fire Custom EventTimed to Animation")]
	public bool fireAnimTimeEvent;

	[Tooltip("This value will be compared to the normalizedTime of the animation you are playing. NormalizedTime is represented as a float so 0 is the beginning, 1 is the end and .5f would be the middle etc.")]
	[Range(0f, 1f)]
	public float whenToFireEvent;

	[MasterCustomEvent]
	public string timedCustomEvent = "[None]";

	[Tooltip("Fire a Custom Event with timed to the animation.  This allows you to time your Custom Events to the actions in you animation. Select the number of Custom Events to be fired, up to 4. Then set the time you want each Custom Event to fire with each subsequent time greater than the previous time.")]
	[Header("Fire Multiple Custom Events Timed to Anim")]
	public bool fireMultiAnimTimeEvent;

	[Range(0f, 4f)]
	public int numOfMultiEventsToFire;

	[Tooltip("This value will be compared to the normalizedTime of the animation you are playing. NormalizedTime is represented as a float so 0 is the beginning, 1 is the end and .5f would be the middle etc.")]
	[Range(0f, 1f)]
	public float whenToFireMultiEvent1;

	[Tooltip("This value will be compared to the normalizedTime of the animation you are playing. NormalizedTime is represented as a float so 0 is the beginning, 1 is the end and .5f would be the middle etc.")]
	[Range(0f, 1f)]
	public float whenToFireMultiEvent2;

	[Tooltip("This value will be compared to the normalizedTime of the animation you are playing. NormalizedTime is represented as a float so 0 is the beginning, 1 is the end and .5f would be the middle etc.")]
	[Range(0f, 1f)]
	public float whenToFireMultiEvent3;

	[Tooltip("This value will be compared to the normalizedTime of the animation you are playing. NormalizedTime is represented as a float so 0 is the beginning, 1 is the end and .5f would be the middle etc.")]
	[Range(0f, 1f)]
	public float whenToFireMultiEvent4;

	[MasterCustomEvent]
	public string MultiTimedEvent = "[None]";

	private bool _playMultiEvent1 = true;

	private bool _playMultiEvent2 = true;

	private bool _playMultiEvent3 = true;

	private bool _playMultiEvent4 = true;

	private bool _fireTimedEvent = true;

	private Transform _actorTrans;

	private int _lastRepetition = -1;

	private bool CanTransmitToOtherPlayers
	{
		get
		{
			if (MultiplayerBroadcast)
			{
				return MasterAudioMultiplayerAdapter.CanSendRPCs;
			}
			return false;
		}
	}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_lastRepetition = 0;
		_actorTrans = ActorTrans(animator);
		if (fireEnterEvent && !(enterCustomEvent == "[None]") && !string.IsNullOrEmpty(enterCustomEvent))
		{
			if (CanTransmitToOtherPlayers)
			{
				MasterAudioMultiplayerAdapter.FireCustomEvent(enterCustomEvent, _actorTrans);
			}
			else
			{
				MasterAudio.FireCustomEvent(enterCustomEvent, _actorTrans);
			}
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		int num = (int)stateInfo.normalizedTime;
		float num2 = stateInfo.normalizedTime - (float)num;
		if (fireAnimTimeEvent)
		{
			if (!_fireTimedEvent && RetriggerWhenStateLoops && _lastRepetition >= 0 && num > _lastRepetition)
			{
				_fireTimedEvent = true;
			}
			if (_fireTimedEvent && num2 > whenToFireEvent)
			{
				_fireTimedEvent = false;
				if (CanTransmitToOtherPlayers)
				{
					MasterAudioMultiplayerAdapter.FireCustomEvent(timedCustomEvent, _actorTrans);
				}
				else
				{
					MasterAudio.FireCustomEvent(timedCustomEvent, _actorTrans);
				}
			}
		}
		if (fireMultiAnimTimeEvent)
		{
			if (RetriggerWhenStateLoops)
			{
				if (!_playMultiEvent1 && _lastRepetition >= 0 && num > _lastRepetition)
				{
					_playMultiEvent1 = true;
				}
				if (!_playMultiEvent2 && _lastRepetition >= 0 && num > _lastRepetition)
				{
					_playMultiEvent2 = true;
				}
				if (!_playMultiEvent3 && _lastRepetition >= 0 && num > _lastRepetition)
				{
					_playMultiEvent3 = true;
				}
				if (!_playMultiEvent4 && _lastRepetition >= 0 && num > _lastRepetition)
				{
					_playMultiEvent4 = true;
				}
			}
			if (_playMultiEvent1 && !(num2 < whenToFireMultiEvent1) && numOfMultiEventsToFire >= 1)
			{
				_playMultiEvent1 = false;
				if (CanTransmitToOtherPlayers)
				{
					MasterAudioMultiplayerAdapter.FireCustomEvent(MultiTimedEvent, _actorTrans);
				}
				else
				{
					MasterAudio.FireCustomEvent(MultiTimedEvent, _actorTrans);
				}
			}
			if (_playMultiEvent2 && !(num2 < whenToFireMultiEvent2) && numOfMultiEventsToFire >= 2)
			{
				_playMultiEvent2 = false;
				if (CanTransmitToOtherPlayers)
				{
					MasterAudioMultiplayerAdapter.FireCustomEvent(MultiTimedEvent, _actorTrans);
				}
				else
				{
					MasterAudio.FireCustomEvent(MultiTimedEvent, _actorTrans);
				}
			}
			if (_playMultiEvent3 && !(num2 < whenToFireMultiEvent3) && numOfMultiEventsToFire >= 3)
			{
				_playMultiEvent3 = false;
				if (CanTransmitToOtherPlayers)
				{
					MasterAudioMultiplayerAdapter.FireCustomEvent(MultiTimedEvent, _actorTrans);
				}
				else
				{
					MasterAudio.FireCustomEvent(MultiTimedEvent, _actorTrans);
				}
			}
			if (_playMultiEvent4 && !(num2 < whenToFireMultiEvent4) && numOfMultiEventsToFire >= 4)
			{
				_playMultiEvent4 = false;
				if (CanTransmitToOtherPlayers)
				{
					MasterAudioMultiplayerAdapter.FireCustomEvent(MultiTimedEvent, _actorTrans);
				}
				else
				{
					MasterAudio.FireCustomEvent(MultiTimedEvent, _actorTrans);
				}
			}
		}
		_lastRepetition = num;
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (fireExitEvent && exitCustomEvent != "[None]" && !string.IsNullOrEmpty(exitCustomEvent))
		{
			if (CanTransmitToOtherPlayers)
			{
				MasterAudioMultiplayerAdapter.FireCustomEvent(exitCustomEvent, _actorTrans);
			}
			else
			{
				MasterAudio.FireCustomEvent(exitCustomEvent, _actorTrans);
			}
		}
		if (fireMultiAnimTimeEvent)
		{
			_playMultiEvent1 = true;
			_playMultiEvent2 = true;
			_playMultiEvent3 = true;
			_playMultiEvent4 = true;
		}
		if (fireAnimTimeEvent)
		{
			_fireTimedEvent = true;
		}
	}

	private Transform ActorTrans(Animator anim)
	{
		if (_actorTrans != null)
		{
			return _actorTrans;
		}
		_actorTrans = anim.transform;
		return _actorTrans;
	}
}
