namespace UnityEngine.VFX.Utility;

[RequireComponent(typeof(Light))]
internal class VFXOutputEventPrefabAttributeLightHandler : VFXOutputEventPrefabAttributeAbstractHandler
{
	public float brightnessScale = 1f;

	private static readonly int k_Color = Shader.PropertyToID("color");

	public override void OnVFXEventAttribute(VFXEventAttribute eventAttribute, VisualEffect visualEffect)
	{
		Vector3 vector = eventAttribute.GetVector3(k_Color);
		float magnitude = vector.magnitude;
		Color color = new Color(vector.x, vector.y, vector.z) / magnitude;
		magnitude *= brightnessScale;
		Light component = GetComponent<Light>();
		component.color = color;
		component.intensity = magnitude;
	}
}
