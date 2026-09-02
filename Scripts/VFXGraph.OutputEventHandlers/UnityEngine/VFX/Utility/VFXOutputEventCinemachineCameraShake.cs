using Cinemachine;

namespace UnityEngine.VFX.Utility;

[ExecuteAlways]
[RequireComponent(typeof(VisualEffect))]
internal class VFXOutputEventCinemachineCameraShake : VFXOutputEventAbstractHandler
{
	public enum Space
	{
		Local = 0,
		World = 1
	}

	private static readonly int k_Position = Shader.PropertyToID("position");

	private static readonly int k_Velocity = Shader.PropertyToID("velocity");

	[Tooltip("The Cinemachine Impulse Source to use in order to send impulses.")]
	public CinemachineImpulseSource cinemachineImpulseSource;

	[Tooltip("The space in which the position and velocity attributes values are defined (local to the VFX, or world).")]
	public Space attributeSpace;

	public override bool canExecuteInEditor => true;

	public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
	{
		if (cinemachineImpulseSource != null)
		{
			Vector3 vector = eventAttribute.GetVector3(k_Position);
			Vector3 vector2 = eventAttribute.GetVector3(k_Velocity);
			if (attributeSpace == Space.Local)
			{
				vector = base.transform.localToWorldMatrix.MultiplyPoint(vector);
				vector2 = base.transform.localToWorldMatrix.MultiplyVector(vector2);
			}
			cinemachineImpulseSource.GenerateImpulseAt(vector, vector2);
		}
	}
}
