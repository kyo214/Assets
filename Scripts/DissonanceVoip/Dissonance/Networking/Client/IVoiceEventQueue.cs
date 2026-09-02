namespace Dissonance.Networking.Client;

internal interface IVoiceEventQueue
{
	void EnqueueStoppedSpeaking(string name);

	void EnqueueStartedSpeaking(string name);

	void EnqueueVoiceData(VoicePacket voicePacket);

	byte[] GetEventBuffer();
}
