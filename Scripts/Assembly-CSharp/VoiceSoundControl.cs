using Dissonance.Audio.Playback;
using UnityEngine;
using UnityEngine.Audio;

public class VoiceSoundControl : MonoBehaviour
{
	public PlayerController Player;

	public VoicePlayback VoicePlayback;

	public AudioSource AudioSource;

	[Header("Normal Mixer Group")]
	[SerializeField]
	private AudioMixerGroup MixerSpeakSameRoom;

	[SerializeField]
	private AudioMixerGroup MixerSpeakDifferentRoom;

	[SerializeField]
	private AudioMixerGroup MixerRadioHT;

	[Header("Slow Motion Mixer Group")]
	[SerializeField]
	private AudioMixerGroup MixerSpeakSameRoomSlowMotion;

	[SerializeField]
	private AudioMixerGroup MixerSpeakDifferentRoomSlowMotion;

	[SerializeField]
	private AudioMixerGroup MixerRadioHTSlowMotion;

	[Header("Filter Effects")]
	[SerializeField]
	private AudioDistortionFilter DistortionFilter;

	[SerializeField]
	private AudioLowPassFilter LowPassFilter;

	private bool _initalizedSetParent;

	private Transform _originParent;

	private bool _isSlowMotion;

	private void Start()
	{
		if ((bool)VoiceChatGlobalController.Instance)
		{
			VoiceChatGlobalController.Instance.ListVoiceSound.Add(this);
		}
	}

	public void SetToOriginParent()
	{
		_initalizedSetParent = false;
		base.transform.SetParent(_originParent);
	}

	public void IsInitSetParent(Transform audioListenerTransform, PlayerController player)
	{
		if (!_initalizedSetParent)
		{
			Player = player;
			_initalizedSetParent = true;
			base.transform.localPosition = Vector3.zero;
			base.transform.localEulerAngles = Vector3.zero;
			_originParent = base.transform.parent;
			base.transform.SetParent(audioListenerTransform);
		}
	}

	public void EnableSlowMotionEffect()
	{
		_isSlowMotion = true;
	}

	public void DisableSlowMotionEffect()
	{
		_isSlowMotion = false;
	}

	public void SetSlowMotionEffect(bool enable)
	{
		if (enable)
		{
			EnableSlowMotionEffect();
		}
		else
		{
			DisableSlowMotionEffect();
		}
	}

	public void ChangeAudioMixer(AudioMixerGroup audioMixerGroup)
	{
		AudioSource.outputAudioMixerGroup = audioMixerGroup;
	}

	public void SetSameRoomAudioMixer()
	{
		AudioSource.outputAudioMixerGroup = (_isSlowMotion ? MixerSpeakSameRoomSlowMotion : MixerSpeakSameRoom);
	}

	public void SetDifferentRoomAudioMixer()
	{
		AudioSource.outputAudioMixerGroup = (_isSlowMotion ? MixerSpeakDifferentRoomSlowMotion : MixerSpeakDifferentRoom);
	}

	public void SetHtAudioMixer()
	{
		AudioSource.spatialBlend = 0f;
		AudioSource.maxDistance = 500f;
		AudioSource.outputAudioMixerGroup = (_isSlowMotion ? MixerRadioHTSlowMotion : MixerRadioHT);
	}
}
