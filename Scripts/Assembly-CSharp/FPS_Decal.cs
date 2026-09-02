using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class FPS_Decal : MonoBehaviour
{
	public bool ScreenSpaceDecals = true;

	public float randomScalePercent = 50f;

	private MaterialPropertyBlock props;

	private MeshRenderer rend;

	private Vector3 startScale;

	private void Awake()
	{
		startScale = base.transform.localScale;
	}

	private void OnEnable()
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		if (component != null)
		{
			component.reflectionProbeUsage = ReflectionProbeUsage.Off;
			component.shadowCastingMode = ShadowCastingMode.Off;
			if (ScreenSpaceDecals)
			{
				component.sharedMaterial.DisableKeyword("USE_QUAD_DECAL");
				component.sharedMaterial.SetInt("_ZTest1", 5);
			}
			else
			{
				component.sharedMaterial.EnableKeyword("USE_QUAD_DECAL");
				component.sharedMaterial.SetInt("_ZTest1", 4);
			}
		}
		if (Application.isPlaying)
		{
			base.transform.localRotation = Quaternion.Euler(Random.Range(0, 360), 90f, 90f);
			float num = Random.Range(startScale.x - startScale.x * randomScalePercent * 0.01f, startScale.x + startScale.x * randomScalePercent * 0.01f);
			base.transform.localScale = new Vector3(num, ScreenSpaceDecals ? startScale.y : 0.001f, num);
		}
		if (Camera.main.depthTextureMode != DepthTextureMode.Depth)
		{
			Camera.main.depthTextureMode = DepthTextureMode.Depth;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = Matrix4x4.TRS(base.transform.TransformPoint(Vector3.zero), base.transform.rotation, base.transform.lossyScale);
		Gizmos.color = new Color(1f, 1f, 1f, 1f);
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
	}
}
