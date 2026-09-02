using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Lux_SRP_GrassDisplacement;

public class GrassDisplacementPass : ScriptableRenderPass
{
	private const string ProfilerTag = "Render Lux Grass Displacement FX";

	private static ProfilingSampler m_ProfilingSampler = new ProfilingSampler("Render Lux Grass Displacement FX");

	private ShaderTagId m_GrassDisplacementFXShaderTag = new ShaderTagId("LuxGrassDisplacementFX");

	private SinglePassStereoMode m_StereoRenderingMode;

	private Color m_ClearColor = new Color(127f / 255f, 127f / 255f, 1f, 1f);

	private RTHandle m_GrassDisplacementFX;

	private Matrix4x4 projectionMatrix;

	private Matrix4x4 worldToCameraMatrix;

	public float m_Size = 20f;

	public int m_Resolution = 256;

	public bool m_ShiftRenderTex;

	private float stepSize;

	private float oneOverStepSize;

	private Vector4 posSize = Vector4.zero;

	private static readonly int DisplacementTexPosSizePID = Shader.PropertyToID("_Lux_DisplacementPosition");

	private static readonly int _Lux_DisplacementRT = Shader.PropertyToID("_Lux_DisplacementRT");

	private RenderTextureDescriptor descriptor;

	private FilteringSettings transparentFilterSettings { get; set; }

	public void Setup(in RenderingData renderingData)
	{
		descriptor = new RenderTextureDescriptor(m_Resolution, m_Resolution);
		descriptor.depthBufferBits = 0;
		descriptor.colorFormat = RenderTextureFormat.Default;
		descriptor.dimension = TextureDimension.Tex2D;
		RenderingUtils.ReAllocateIfNeeded(ref m_GrassDisplacementFX, in descriptor, FilterMode.Point, TextureWrapMode.Repeat, isShadowMap: false, 1, 0f, "_Lux_DisplacementRT");
		Shader.SetGlobalTexture(_Lux_DisplacementRT, m_GrassDisplacementFX.rt);
		stepSize = m_Size / (float)m_Resolution;
		oneOverStepSize = 1f / stepSize;
		float num = m_Size * 0.5f;
		projectionMatrix = Matrix4x4.Ortho(0f - num, num, 0f - num, num, 0.1f, 80f);
		projectionMatrix = GL.GetGPUProjectionMatrix(projectionMatrix, renderIntoTexture: false);
		worldToCameraMatrix.SetRow(0, new Vector4(1f, 0f, 0f, 0f));
		worldToCameraMatrix.SetRow(1, new Vector4(0f, 0f, 1f, 0f));
		worldToCameraMatrix.SetRow(2, new Vector4(0f, 1f, 0f, 0f));
		worldToCameraMatrix.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
		transparentFilterSettings = new FilteringSettings(RenderQueueRange.transparent);
	}

	public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
	{
		ConfigureTarget(m_GrassDisplacementFX);
		ConfigureClear(ClearFlag.Color, m_ClearColor);
	}

	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
	{
		CommandBuffer commandBuffer = CommandBufferPool.Get();
		using (new ProfilingScope(commandBuffer, m_ProfilingSampler))
		{
			commandBuffer.Clear();
			DrawingSettings drawingSettings = CreateDrawingSettings(m_GrassDisplacementFXShaderTag, ref renderingData, SortingCriteria.CommonTransparent);
			FilteringSettings filteringSettings = transparentFilterSettings;
			Camera camera = renderingData.cameraData.camera;
			Transform transform = camera.transform;
			Vector3 position = transform.position;
			Vector3 forward = transform.forward;
			if (m_ShiftRenderTex)
			{
				Vector2 vector = new Vector2(forward.x, forward.z);
				vector.Normalize();
				position.x += vector.x * m_Size * 0.33f;
				position.z += vector.y * m_Size * 0.33f;
			}
			Matrix4x4 view = camera.worldToCameraMatrix;
			Matrix4x4 proj = camera.projectionMatrix;
			Vector2 zero = Vector2.zero;
			zero.x = Mathf.Floor(position.x * oneOverStepSize) * stepSize;
			zero.y = Mathf.Floor(position.z * oneOverStepSize) * stepSize;
			worldToCameraMatrix.SetColumn(3, new Vector4(0f - zero.x, 0f - zero.y, 0f - position.y - 40f, 1f));
			commandBuffer.SetViewProjectionMatrices(worldToCameraMatrix, projectionMatrix);
			posSize.x = zero.x - m_Size * 0.5f;
			posSize.y = zero.y - m_Size * 0.5f;
			posSize.z = 1f / m_Size;
			commandBuffer.SetGlobalVector(DisplacementTexPosSizePID, posSize);
			context.ExecuteCommandBuffer(commandBuffer);
			context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
			commandBuffer.Clear();
			commandBuffer.SetViewProjectionMatrices(view, proj);
		}
		CommandBufferPool.Release(commandBuffer);
	}
}
