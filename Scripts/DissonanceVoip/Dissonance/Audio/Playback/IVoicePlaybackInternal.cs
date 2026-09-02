using Dissonance.Networking;
using UnityEngine;

namespace Dissonance.Audio.Playback;

public interface IVoicePlaybackInternal : IRemoteChannelProvider, IVoicePlayback
{
	bool IsMuted { get; set; }

	new string PlayerName { get; set; }

	bool AllowPositionalPlayback { get; set; }

	CodecSettings CodecSettings { get; set; }

	float PlaybackVolume { get; set; }

	void Reset();

	void StartPlayback();

	void StopPlayback();

	void SetTransform(Vector3 position, Quaternion rotation);

	void ReceiveAudioPacket(VoicePacket packet);

	void ForceReset();

	void Setup(IPriorityManager priority, IVolumeProvider volume);
}
