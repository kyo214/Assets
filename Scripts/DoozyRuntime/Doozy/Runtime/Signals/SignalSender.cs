using UnityEngine;

namespace Doozy.Runtime.Signals;

[AddComponentMenu("Signals/Signal Sender")]
public class SignalSender : MonoBehaviour
{
	public SignalPayload Payload = new SignalPayload();

	public bool SendOnStart;

	public bool SendOnEnable;

	public bool SendOnDisable;

	public bool SendOnDestroy;

	protected virtual void Start()
	{
		if (SendOnStart)
		{
			SendSignal();
		}
	}

	protected virtual void OnEnable()
	{
		if (SendOnEnable)
		{
			SendSignal();
		}
	}

	protected virtual void OnDisable()
	{
		if (SendOnDisable)
		{
			SendSignal();
		}
	}

	protected virtual void OnDestroy()
	{
		if (SendOnDestroy)
		{
			SendSignal();
		}
	}

	public virtual void SendSignal()
	{
		Payload?.SendSignal();
	}
}
