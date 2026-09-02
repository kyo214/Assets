namespace UnityEngine.VFX.Utility;

[ExecuteAlways]
[RequireComponent(typeof(VisualEffect))]
internal class VFXOutputEventRigidBody : VFXOutputEventAbstractHandler
{
	public enum Space
	{
		Local = 0,
		World = 1
	}

	public enum RigidBodyEventType
	{
		Impulse = 0,
		Explosion = 1,
		VelocityChange = 2
	}

	private static readonly int k_Position = Shader.PropertyToID("position");

	private static readonly int k_Size = Shader.PropertyToID("size");

	private static readonly int k_Velocity = Shader.PropertyToID("velocity");

	[Tooltip("The Rigid body to apply a force on.")]
	public Rigidbody rigidBody;

	[Tooltip("The Space VFX Attributes values are expressed.")]
	public Space attributeSpace;

	[Tooltip("Type of Instantaneous Force to apply on the RigidBody upon event:\n - Impulse using the Velocity attribute \n - Explosion at given Position attribute, using the Size for radius and the magnitude of Velocity Attribute for intensity\n - Velocity Change using Velocity Attribute")]
	public RigidBodyEventType eventType;

	public override bool canExecuteInEditor => false;

	public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
	{
		if (!(rigidBody == null))
		{
			Vector3 vector = eventAttribute.GetVector3(k_Position);
			float num = eventAttribute.GetFloat(k_Size);
			Vector3 vector2 = eventAttribute.GetVector3(k_Velocity);
			if (attributeSpace == Space.Local)
			{
				vector = base.transform.localToWorldMatrix.MultiplyPoint(vector);
				vector2 = base.transform.localToWorldMatrix.MultiplyVector(vector2);
				num = base.transform.localToWorldMatrix.MultiplyVector(Vector3.right * num).magnitude;
			}
			switch (eventType)
			{
			case RigidBodyEventType.Impulse:
				rigidBody.AddForce(vector2, ForceMode.Impulse);
				break;
			case RigidBodyEventType.Explosion:
				rigidBody.AddExplosionForce(vector2.magnitude, vector, num);
				break;
			case RigidBodyEventType.VelocityChange:
				rigidBody.AddForce(vector2, ForceMode.VelocityChange);
				break;
			}
		}
	}
}
