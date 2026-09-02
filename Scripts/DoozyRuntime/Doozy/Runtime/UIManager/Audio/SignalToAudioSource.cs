using Doozy.Runtime.Signals;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Audio;

[AddComponentMenu("Signals/Signal To AudioSource")]
public class SignalToAudioSource : BaseStreamListener
{
	[SerializeField]
	private StreamId StreamId;

	[SerializeField]
	private AudioSource AudioSource;

	public StreamId streamId => StreamId;

	public AudioSource audioSource => AudioSource;

	public bool hasAudioSource => AudioSource != null;

	public SignalStream stream { get; private set; }

	private void OnEnable()
	{
		ConnectReceiver();
	}

	private void OnDisable()
	{
		DisconnectReceiver();
	}

	protected override void ConnectReceiver()
	{
		stream = SignalStream.Get(streamId.Category, streamId.Name).ConnectReceiver(base.receiver);
	}

	protected override void DisconnectReceiver()
	{
		stream.DisconnectReceiver(base.receiver);
	}

	protected override void ProcessSignal(Signal signal)
	{
		if (hasAudioSource && signal != null && signal.hasValue && !(signal.valueType != typeof(AudioClip)))
		{
			audioSource.Stop();
			audioSource.clip = signal.GetValueUnsafe<AudioClip>();
			if (audioSource.clip != null)
			{
				audioSource.Play();
			}
		}
	}
}
