using System;

namespace Dissonance.Audio.Playback;

public interface IAudioOutputSubscriber
{
	void OnAudioPlayback(ArraySegment<float> data, bool complete);
}
