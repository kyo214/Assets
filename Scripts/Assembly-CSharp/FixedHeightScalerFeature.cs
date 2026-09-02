using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FixedHeightScalerFeature : ScriptableRendererFeature
{
	[Serializable]
	public class Settings
	{
		public int targetHeight = 480;

		public FilterMode filterMode;
	}

	private class FixedHeightScalerPass : ScriptableRenderPass
	{
		private int targetHeight;

		private FilterMode filterMode;

		private RTHandle downscaleRT;

		public FixedHeightScalerPass(int targetHeight, FilterMode filterMode)
		{
			this.targetHeight = targetHeight;
			this.filterMode = filterMode;
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			if (!renderingData.cameraData.isSceneViewCamera)
			{
				int pixelHeight = renderingData.cameraData.camera.pixelHeight;
				if (pixelHeight > targetHeight)
				{
					CommandBuffer commandBuffer = CommandBufferPool.Get("FixedHeightScaler");
					float num = (float)renderingData.cameraData.camera.pixelWidth / (float)pixelHeight;
					int width = Mathf.RoundToInt((float)targetHeight * num);
					RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
					descriptor.width = width;
					descriptor.height = targetHeight;
					descriptor.depthBufferBits = 0;
					RenderingUtils.ReAllocateIfNeeded(ref downscaleRT, in descriptor, filterMode, TextureWrapMode.Clamp, isShadowMap: false, 1, 0f, "_FixedScalerRT");
					RTHandle cameraColorTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
					Blitter.BlitCameraTexture(commandBuffer, cameraColorTargetHandle, downscaleRT);
					Blitter.BlitCameraTexture(commandBuffer, downscaleRT, cameraColorTargetHandle);
					context.ExecuteCommandBuffer(commandBuffer);
					CommandBufferPool.Release(commandBuffer);
				}
			}
		}

		public override void OnCameraCleanup(CommandBuffer cmd)
		{
			if (downscaleRT != null)
			{
				downscaleRT.Release();
				downscaleRT = null;
			}
		}
	}

	public Settings settings = new Settings();

	private FixedHeightScalerPass scalerPass;

	public override void Create()
	{
		scalerPass = new FixedHeightScalerPass(settings.targetHeight, settings.filterMode);
		scalerPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		if (!Application.isEditor)
		{
			renderer.EnqueuePass(scalerPass);
		}
	}
}
