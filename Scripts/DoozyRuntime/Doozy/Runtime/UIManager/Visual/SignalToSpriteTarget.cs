using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Signals;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Visual;

[AddComponentMenu("Signals/Signal To SpriteTarget")]
public class SignalToSpriteTarget : BaseStreamListener
{
	[SerializeField]
	private StreamId StreamId;

	[SerializeField]
	private ReactorSpriteTarget SpriteTarget;

	public StreamId streamId => StreamId;

	public ReactorSpriteTarget spriteTarget => SpriteTarget;

	public bool hasSpriteTarget => SpriteTarget != null;

	public SignalStream stream { get; private set; }

	public void FindTarget()
	{
		if (!(SpriteTarget != null))
		{
			SpriteTarget = ReactorSpriteTarget.FindTarget(base.gameObject);
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
		if (hasSpriteTarget && signal != null && signal.hasValue && !(signal.valueType != typeof(Sprite)))
		{
			SetSprite(signal.GetValueUnsafe<Sprite>());
		}
	}

	public void SetSprite(Sprite sprite)
	{
		if (SpriteTarget != null)
		{
			SpriteTarget.SetSprite(sprite);
		}
	}
}
