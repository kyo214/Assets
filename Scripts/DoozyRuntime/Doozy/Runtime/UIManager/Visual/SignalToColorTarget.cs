using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Signals;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Visual;

[AddComponentMenu("Signals/Signal To ColorTarget")]
public class SignalToColorTarget : BaseStreamListener
{
	[SerializeField]
	private StreamId StreamId;

	[SerializeField]
	private ReactorColorTarget ColorTarget;

	public StreamId streamId => StreamId;

	public ReactorColorTarget colorTarget => ColorTarget;

	public bool hasColorTarget => ColorTarget != null;

	public SignalStream stream { get; private set; }

	public void FindTarget()
	{
		if (!(ColorTarget != null))
		{
			ColorTarget = ReactorColorTarget.FindTarget(base.gameObject);
		}
	}

	private void Awake()
	{
		FindTarget();
	}

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
		if (hasColorTarget && signal != null && signal.hasValue && !(signal.valueType != typeof(Color)))
		{
			SetColor(signal.GetValueUnsafe<Color>());
		}
	}

	public void SetColor(Color color)
	{
		if (ColorTarget != null)
		{
			ColorTarget.SetColor(color);
		}
	}
}
