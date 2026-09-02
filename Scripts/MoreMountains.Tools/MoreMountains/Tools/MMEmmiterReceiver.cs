using UnityEngine;

namespace MoreMountains.Tools;

public class MMEmmiterReceiver : MonoBehaviour
{
	public delegate void OnValueChangeDelegate();

	[MMInformation("This component lets you very easily have one property drive the value of another property. To do so, drag the object with the property you want to 'read' from into the Emitter Property slot, then select the component the property is on, and finally the property itself.Then drag the object with the property you want to 'write' to into the ReceiverProperty slot, and pick the property you want to drive with the emitter's value.", MMInformationAttribute.InformationType.Info, false)]
	public bool Emitting = true;

	[Header("Emitter")]
	[Tooltip("the property whose value you want to read and to have drive the ReceiverProperty's value")]
	public MMPropertyEmitter EmitterProperty;

	[Header("Receiver")]
	[Tooltip("the property whose value you want to be driven by the EmitterProperty's value")]
	public MMPropertyReceiver ReceiverProperty;

	public OnValueChangeDelegate OnValueChange;

	protected float _levelLastFrame;

	protected virtual void Awake()
	{
		EmitterProperty.Initialization(EmitterProperty.TargetComponent.gameObject);
		ReceiverProperty.Initialization(ReceiverProperty.TargetComponent.gameObject);
	}

	protected virtual void Update()
	{
		EmitValue();
	}

	protected virtual void EmitValue()
	{
		if (Emitting)
		{
			float level = EmitterProperty.GetLevel();
			if (level != _levelLastFrame)
			{
				OnValueChange?.Invoke();
				ReceiverProperty?.SetLevel(level);
			}
			_levelLastFrame = level;
		}
	}
}
