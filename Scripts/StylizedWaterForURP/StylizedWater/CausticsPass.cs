using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace StylizedWater;

public class CausticsPass : ScriptableRenderPass
{
	private const string profilerTag = "Caustics Pass";

	public Material causticsMaterial;

	private static Mesh mesh;

	private float waterLevel;

	private const float BIAS = 0.1f;

	public CausticsPass(float waterLevel)
	{
		this.waterLevel = waterLevel;
	}

	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
	{
		Camera camera = renderingData.cameraData.camera;
		if (camera.cameraType != CameraType.Preview && (bool)causticsMaterial)
		{
			Matrix4x4 value = ((RenderSettings.sun != null) ? RenderSettings.sun.transform.localToWorldMatrix : Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(-45f, 45f, 0f), Vector3.one));
			causticsMaterial.SetMatrix("_MainLightDirection", value);
			CommandBuffer commandBuffer = CommandBufferPool.Get("Caustics Pass");
			if (!mesh)
			{
				mesh = GenerateQuad(1000f);
			}
			Vector3 position = camera.transform.position;
			position.y = ((camera.transform.position.y > waterLevel) ? waterLevel : (camera.transform.position.y - 0.1f));
			Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
			commandBuffer.DrawMesh(mesh, matrix, causticsMaterial, 0, 0);
			context.ExecuteCommandBuffer(commandBuffer);
			CommandBufferPool.Release(commandBuffer);
		}
	}

	private static Mesh GenerateQuad(float size)
	{
		Mesh mesh = new Mesh();
		size *= 0.5f;
		Vector3[] vertices = new Vector3[4]
		{
			new Vector3(0f - size, 0f, 0f - size),
			new Vector3(size, 0f, 0f - size),
			new Vector3(0f - size, 0f, size),
			new Vector3(size, 0f, size)
		};
		int[] triangles = new int[6] { 0, 2, 1, 2, 3, 1 };
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		return mesh;
	}
}
