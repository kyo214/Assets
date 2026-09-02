using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Blit : ScriptableRendererFeature
{
	public enum BlitTarget
	{
		Camera = 0,
		Texture = 1
	}

	[Serializable]
	public class BlitSettings
	{
		public RenderPassEvent renderEvent = RenderPassEvent.AfterRenderingPostProcessing;

		public BlitTarget target;

		public string targetTextureName;

		public Material blitMaterial;

		public int blitMaterialPassIndex;
	}

	public class BlitPass : ScriptableRenderPass
	{
		private BlitSettings settings;

		private RTHandle source;

		private RTHandle target;

		private RTHandle temp;

		private string m_ProfilerTag;

		public BlitPass(RenderPassEvent renderPassEvent, BlitSettings settings, string tag)
		{
			base.renderPassEvent = renderPassEvent;
			this.settings = settings;
			m_ProfilerTag = tag;
		}

		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
			descriptor.depthBufferBits = 0;
			RenderingUtils.ReAllocateIfNeeded(ref temp, in descriptor, FilterMode.Point, TextureWrapMode.Repeat, isShadowMap: false, 1, 0f, "_TemporaryColorTexture");
			if (settings.target == BlitTarget.Texture && !string.IsNullOrEmpty(settings.targetTextureName))
			{
				RenderingUtils.ReAllocateIfNeeded(ref target, in descriptor, FilterMode.Point, TextureWrapMode.Repeat, isShadowMap: false, 1, 0f, settings.targetTextureName);
				Shader.SetGlobalTexture(settings.targetTextureName, target);
			}
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer commandBuffer = CommandBufferPool.Get(m_ProfilerTag);
			source = renderingData.cameraData.renderer.cameraColorTargetHandle;
			if (source.rt != null)
			{
				if (settings.target == BlitTarget.Camera)
				{
					Blitter.BlitCameraTexture(commandBuffer, source, temp, settings.blitMaterial, settings.blitMaterialPassIndex);
					Blitter.BlitCameraTexture(commandBuffer, temp, source, Vector2.one);
				}
				else
				{
					Blitter.BlitCameraTexture(commandBuffer, source, target, settings.blitMaterial, settings.blitMaterialPassIndex);
				}
			}
			context.ExecuteCommandBuffer(commandBuffer);
			CommandBufferPool.Release(commandBuffer);
		}

		public override void OnCameraCleanup(CommandBuffer cmd)
		{
			source = null;
		}

		public void Dispose()
		{
			temp?.Release();
			target?.Release();
		}
	}

	public BlitSettings settings = new BlitSettings();

	public BlitPass blitPass;

	public override void Create()
	{
		int max = ((!(settings.blitMaterial != null)) ? 1 : (settings.blitMaterial.passCount - 1));
		settings.blitMaterialPassIndex = Mathf.Clamp(settings.blitMaterialPassIndex, -1, max);
		blitPass = new BlitPass(settings.renderEvent, settings, base.name);
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		if (renderingData.cameraData.cameraType == CameraType.Game)
		{
			if (settings.renderEvent == RenderPassEvent.AfterRendering)
			{
				Debug.LogWarning("Using \"After Rendering\" event for Blit may be buggy. Prefer using \"After Rendering Post Processing\".");
			}
			if (settings.blitMaterial == null)
			{
				Debug.LogWarningFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
			}
			else if (settings.target == BlitTarget.Texture && string.IsNullOrEmpty(settings.targetTextureName))
			{
				Debug.LogWarning("Target texture name is empty.");
			}
			else
			{
				renderer.EnqueuePass(blitPass);
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		blitPass.Dispose();
	}
}
