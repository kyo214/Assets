using UnityEngine;

namespace MoreMountains.Tools;

[MMRequiresConstantRepaint]
public class MMRadioBroadcaster : MMMonoBehaviour
{
	public delegate void OnValueChangeDelegate();

	[Header("Source")]
	public MMPropertyEmitter Emitter;

	[Header("Destinations")]
	public MMRadioReceiver[] Receivers;

	[Header("Channel Broadcasting")]
	public bool BroadcastOnChannel = true;

	[MMCondition("BroadcastOnChannel", true)]
	public int Channel;

	[MMCondition("BroadcastOnChannel", true)]
	public bool OnlyBroadcastOnValueChange = true;

	public OnValueChangeDelegate OnValueChange;

	protected float _levelLastFrame;

	protected virtual void Awake()
	{
		Emitter.Initialization(base.gameObject);
	}

	protected virtual void Update()
	{
		ProcessBroadcast();
	}

	protected virtual void ProcessBroadcast()
	{
		if (Emitter == null)
		{
			return;
		}
		float level = Emitter.GetLevel();
		if (level != _levelLastFrame)
		{
			OnValueChange?.Invoke();
			MMRadioReceiver[] receivers = Receivers;
			for (int i = 0; i < receivers.Length; i++)
			{
				receivers[i]?.SetLevel(level);
			}
			if (BroadcastOnChannel)
			{
				MMRadioLevelEvent.Trigger(Channel, level);
			}
		}
		_levelLastFrame = level;
	}
}
