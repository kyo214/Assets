namespace UnityEngine.VFX.Utility;

[RequireComponent(typeof(Rigidbody))]
internal class VFXOutputEventPrefabAttributeRigidBodyVelocityHandler : VFXOutputEventPrefabAttributeAbstractHandler
{
	public enum Space
	{
		Local = 0,
		World = 1
	}

	private Rigidbody m_RigidBody;

	public Space attributeSpace;

	private static readonly int k_Velocity = Shader.PropertyToID("velocity");

	public override void OnVFXEventAttribute(VFXEventAttribute eventAttribute, VisualEffect visualEffect)
	{
		Vector3 vector = eventAttribute.GetVector3(k_Velocity);
		if (attributeSpace == Space.Local)
		{
			vector = visualEffect.transform.localToWorldMatrix.MultiplyVector(vector);
		}
		if (TryGetComponent<Rigidbody>(out m_RigidBody))
		{
			m_RigidBody.WakeUp();
			m_RigidBody.velocity = vector;
		}
	}
}
