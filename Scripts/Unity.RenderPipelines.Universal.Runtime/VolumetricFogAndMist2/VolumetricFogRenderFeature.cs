using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VolumetricFogAndMist2;

public class VolumetricFogRenderFeature : ScriptableRendererFeature
{
	public static class ShaderParams
	{
		public const string LightBufferName = "_LightBuffer";

		public static int LightBuffer = Shader.PropertyToID("_LightBuffer");

		public static int LightBufferSize = Shader.PropertyToID("_VFRTSize");

		public static int MainTex = Shader.PropertyToID("_MainTex");

		public static int BlurRT = Shader.PropertyToID("_BlurTex");

		public static int BlurRT2 = Shader.PropertyToID("_BlurTex2");

		public static int MiscData = Shader.PropertyToID("_MiscData");

		public static int ForcedInvisible = Shader.PropertyToID("_ForcedInvisible");

		public static int DownsampledDepth = Shader.PropertyToID("_DownsampledDepth");

		public static int BlueNoiseTexture = Shader.PropertyToID("_BlueNoise");

		public static int BlurScale = Shader.PropertyToID("_BlurScale");

		public static int Downscaling = Shader.PropertyToID("_Downscaling");

		public static int ScatteringData = Shader.PropertyToID("_ScatteringData");

		public static int ScatteringTint = Shader.PropertyToID("_ScatteringTint");

		public static int BlurredTex = Shader.PropertyToID("_BlurredTex");

		public const string SKW_DITHER = "DITHER";

		public const string SKW_EDGE_PRESERVE = "EDGE_PRESERVE";

		public const string SKW_EDGE_PRESERVE_UPSCALING = "EDGE_PRESERVE_UPSCALING";

		public const string SKW_SCATTERING_HQ = "SCATTERING_HQ";
	}

	private class VolumetricFogRenderPass : ScriptableRenderPass
	{
		private FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.transparent);

		private readonly List<ShaderTagId> shaderTagIdList = new List<ShaderTagId>();

		private const string m_ProfilerTag = "Volumetric Fog Light Buffer Rendering";

		private RTHandle m_LightBuffer;

		private VolumetricFogRenderFeature settings;

		public VolumetricFogRenderPass()
		{
			shaderTagIdList.Clear();
			shaderTagIdList.Add(new ShaderTagId("UniversalForward"));
			RenderTargetIdentifier tex = new RenderTargetIdentifier(ShaderParams.LightBuffer, 0, CubemapFace.Unknown, -1);
			m_LightBuffer = RTHandles.Alloc(tex, "_LightBuffer");
		}

		public void CleanUp()
		{
			RTHandles.Release(m_LightBuffer);
		}

		public void Setup(VolumetricFogRenderFeature settings)
		{
			this.settings = settings;
			base.renderPassEvent = settings.renderPassEvent;
		}

		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			RenderTextureDescriptor desc = cameraTextureDescriptor;
			VolumetricFogManager managerIfExists = VolumetricFogManager.GetManagerIfExists();
			if (managerIfExists != null)
			{
				if (managerIfExists.downscaling > 1f)
				{
					int height = (desc.width = GetScaledSize(cameraTextureDescriptor.width, managerIfExists.downscaling));
					desc.height = height;
				}
				desc.colorFormat = (managerIfExists.blurHDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32);
				cmd.SetGlobalVector(ShaderParams.LightBufferSize, new Vector4(desc.width, desc.height, (managerIfExists.downscaling > 1f) ? 1f : 0f, 0f));
			}
			desc.depthBufferBits = 0;
			desc.msaaSamples = 1;
			desc.useMipMap = false;
			cmd.GetTemporaryRT(ShaderParams.LightBuffer, desc, FilterMode.Bilinear);
			ConfigureTarget(m_LightBuffer);
			ConfigureClear(ClearFlag.Color, new Color(0f, 0f, 0f, 0f));
			ConfigureInput(ScriptableRenderPassInput.Depth);
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			VolumetricFogManager managerIfExists = VolumetricFogManager.GetManagerIfExists();
			CommandBuffer commandBuffer = CommandBufferPool.Get("Volumetric Fog Light Buffer Rendering");
			commandBuffer.SetGlobalInt(ShaderParams.ForcedInvisible, 0);
			context.ExecuteCommandBuffer(commandBuffer);
			if (managerIfExists == null || (managerIfExists.downscaling <= 1f && managerIfExists.blurPasses < 1 && managerIfExists.scattering <= 0f))
			{
				CommandBufferPool.Release(commandBuffer);
				return;
			}
			foreach (VolumetricFog volumetricFog in VolumetricFog.volumetricFogs)
			{
				if (volumetricFog != null)
				{
					volumetricFog.meshRenderer.renderingLayerMask = 131072u;
				}
			}
			commandBuffer.Clear();
			SortingCriteria sortingCriteria = SortingCriteria.CommonTransparent;
			DrawingSettings drawingSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, sortingCriteria);
			FilteringSettings filteringSettings = this.filteringSettings;
			filteringSettings.layerMask = settings.fogLayerMask;
			filteringSettings.renderingLayerMask = 131072u;
			context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
			CommandBufferPool.Release(commandBuffer);
		}

		public override void FrameCleanup(CommandBuffer cmd)
		{
		}
	}

	private class BlurRenderPass : ScriptableRenderPass
	{
		private enum Pass
		{
			BlurHorizontal = 0,
			BlurVertical = 1,
			BlurVerticalAndBlend = 2,
			UpscalingBlend = 3,
			DownscaleDepth = 4,
			BlurVerticalFinal = 5,
			Resample = 6,
			ResampleAndCombine = 7,
			ScatteringPrefilter = 8,
			ScatteringBlend = 9
		}

		private struct ScatteringMipData
		{
			public int rtDown;

			public int rtUp;

			public int width;

			public int height;
		}

		private ScriptableRenderer renderer;

		private Material mat;

		private RenderTextureDescriptor sourceDesc;

		private VolumetricFogManager manager;

		private ScatteringMipData[] rt;

		private const int PYRAMID_MAX_LEVELS = 5;

		public void Setup(Shader shader, ScriptableRenderer renderer, VolumetricFogRenderFeature settings)
		{
			base.renderPassEvent = settings.renderPassEvent;
			this.renderer = renderer;
			manager = VolumetricFogManager.GetManagerIfExists();
			if (mat == null)
			{
				mat = CoreUtils.CreateEngineMaterial(shader);
				Texture2D value = Resources.Load<Texture2D>("Textures/blueNoiseVF128");
				mat.SetTexture(ShaderParams.BlueNoiseTexture, value);
			}
		}

		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			sourceDesc = cameraTextureDescriptor;
			ConfigureInput(ScriptableRenderPassInput.Depth);
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			if (manager == null || (manager.downscaling <= 1f && manager.blurPasses < 1 && manager.scattering <= 0f))
			{
				Cleanup();
				return;
			}
			mat.SetVector(ShaderParams.MiscData, new Vector4(manager.ditherStrength * 0.1f, 0f, manager.blurEdgeDepthThreshold, manager.downscalingEdgeDepthThreshold * 0.001f));
			if (manager.ditherStrength > 0f)
			{
				mat.EnableKeyword("DITHER");
			}
			else
			{
				mat.DisableKeyword("DITHER");
			}
			mat.DisableKeyword("EDGE_PRESERVE");
			mat.DisableKeyword("EDGE_PRESERVE_UPSCALING");
			if (manager.blurPasses > 0 && manager.blurEdgePreserve)
			{
				mat.EnableKeyword((manager.downscaling > 1f) ? "EDGE_PRESERVE_UPSCALING" : "EDGE_PRESERVE");
			}
			RTHandle cameraColorTargetHandle = renderer.cameraColorTargetHandle;
			CommandBuffer commandBuffer = CommandBufferPool.Get("Volumetric Fog Render Feature");
			commandBuffer.SetGlobalInt(ShaderParams.ForcedInvisible, 1);
			RenderTextureDescriptor renderTextureDescriptor = sourceDesc;
			renderTextureDescriptor.width = GetScaledSize(sourceDesc.width, manager.downscaling);
			renderTextureDescriptor.height = GetScaledSize(sourceDesc.height, manager.downscaling);
			renderTextureDescriptor.useMipMap = false;
			renderTextureDescriptor.colorFormat = (manager.blurHDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32);
			renderTextureDescriptor.msaaSamples = 1;
			renderTextureDescriptor.depthBufferBits = 0;
			bool flag = manager.downscaling > 1f;
			if (flag)
			{
				RenderTextureDescriptor desc = renderTextureDescriptor;
				desc.colorFormat = RenderTextureFormat.RFloat;
				commandBuffer.GetTemporaryRT(ShaderParams.DownsampledDepth, desc, FilterMode.Bilinear);
				FullScreenBlit(commandBuffer, cameraColorTargetHandle, ShaderParams.DownsampledDepth, mat, 4);
			}
			if (manager.blurPasses < 1)
			{
				FullScreenBlit(commandBuffer, ShaderParams.LightBuffer, cameraColorTargetHandle, mat, 3);
			}
			else
			{
				renderTextureDescriptor.width = GetScaledSize(sourceDesc.width, manager.blurDownscaling);
				renderTextureDescriptor.height = GetScaledSize(sourceDesc.height, manager.blurDownscaling);
				commandBuffer.GetTemporaryRT(ShaderParams.BlurRT, renderTextureDescriptor, FilterMode.Bilinear);
				commandBuffer.GetTemporaryRT(ShaderParams.BlurRT2, renderTextureDescriptor, FilterMode.Bilinear);
				commandBuffer.SetGlobalFloat(ShaderParams.BlurScale, manager.blurSpread * manager.blurDownscaling);
				FullScreenBlit(commandBuffer, ShaderParams.LightBuffer, ShaderParams.BlurRT, mat, 0);
				commandBuffer.SetGlobalFloat(ShaderParams.BlurScale, manager.blurSpread);
				for (int i = 0; i < manager.blurPasses - 1; i++)
				{
					FullScreenBlit(commandBuffer, ShaderParams.BlurRT, ShaderParams.BlurRT2, mat, 1);
					FullScreenBlit(commandBuffer, ShaderParams.BlurRT2, ShaderParams.BlurRT, mat, 0);
				}
				if (flag)
				{
					FullScreenBlit(commandBuffer, ShaderParams.BlurRT, ShaderParams.BlurRT2, mat, 5);
					FullScreenBlit(commandBuffer, ShaderParams.BlurRT2, cameraColorTargetHandle, mat, 3);
				}
				else
				{
					FullScreenBlit(commandBuffer, ShaderParams.BlurRT, cameraColorTargetHandle, mat, 2);
				}
				commandBuffer.ReleaseTemporaryRT(ShaderParams.BlurRT2);
				commandBuffer.ReleaseTemporaryRT(ShaderParams.BlurRT);
			}
			if (manager.scattering > 0f)
			{
				ComputeScattering(commandBuffer, cameraColorTargetHandle, mat);
			}
			commandBuffer.ReleaseTemporaryRT(ShaderParams.LightBuffer);
			if (flag)
			{
				commandBuffer.ReleaseTemporaryRT(ShaderParams.DownsampledDepth);
			}
			context.ExecuteCommandBuffer(commandBuffer);
			CommandBufferPool.Release(commandBuffer);
		}

		private void ComputeScattering(CommandBuffer cmd, RTHandle source, Material mat)
		{
			mat.SetVector(ShaderParams.ScatteringData, new Vector4(manager.scatteringThreshold, manager.scatteringIntensity, 1f - manager.scatteringAbsorption, manager.scattering));
			mat.SetColor(ShaderParams.ScatteringTint, manager.scatteringTint);
			float downscaling = manager.downscaling;
			if (rt == null || rt.Length != 6)
			{
				rt = new ScatteringMipData[6];
				for (int i = 0; i < rt.Length; i++)
				{
					rt[i].rtDown = Shader.PropertyToID("_VFogDownMip" + i);
					rt[i].rtUp = Shader.PropertyToID("_VFogUpMip" + i);
				}
			}
			int num = GetScaledSize(sourceDesc.width, downscaling);
			int num2 = GetScaledSize(sourceDesc.height, downscaling);
			if (downscaling > 1f && manager.scatteringHighQuality)
			{
				mat.EnableKeyword("SCATTERING_HQ");
			}
			else
			{
				mat.DisableKeyword("SCATTERING_HQ");
			}
			if (!manager.scatteringHighQuality)
			{
				num /= 2;
				num2 /= 2;
			}
			int num3 = (manager.scatteringHighQuality ? 5 : 4);
			RenderTextureDescriptor desc = sourceDesc;
			desc.colorFormat = RenderTextureFormat.ARGBHalf;
			desc.msaaSamples = 1;
			desc.depthBufferBits = 0;
			for (int j = 0; j <= num3; j++)
			{
				if (num < 2)
				{
					num = 2;
				}
				if (num2 < 2)
				{
					num2 = 2;
				}
				desc.width = (rt[j].width = num);
				desc.height = (rt[j].height = num2);
				cmd.GetTemporaryRT(rt[j].rtDown, desc, FilterMode.Bilinear);
				cmd.GetTemporaryRT(rt[j].rtUp, desc, FilterMode.Bilinear);
				num /= 2;
				num2 /= 2;
			}
			RenderTargetIdentifier renderTargetIdentifier = rt[0].rtDown;
			FullScreenBlit(cmd, source, renderTargetIdentifier, mat, 8);
			cmd.SetGlobalFloat(ShaderParams.BlurScale, 1f);
			for (int k = 1; k <= num3; k++)
			{
				FullScreenBlit(cmd, renderTargetIdentifier, rt[k].rtDown, mat, 6);
				renderTargetIdentifier = rt[k].rtDown;
			}
			cmd.SetGlobalFloat(ShaderParams.BlurScale, 1.5f);
			for (int num4 = num3; num4 > 0; num4--)
			{
				cmd.SetGlobalTexture(ShaderParams.BlurredTex, rt[num4 - 1].rtDown);
				FullScreenBlit(cmd, renderTargetIdentifier, rt[num4 - 1].rtUp, mat, 7);
				renderTargetIdentifier = rt[num4 - 1].rtUp;
			}
			FullScreenBlit(cmd, renderTargetIdentifier, source, mat, 9);
		}

		private void FullScreenBlit(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material, int passIndex)
		{
			destination = new RenderTargetIdentifier(destination, 0, CubemapFace.Unknown, -1);
			cmd.SetRenderTarget(destination);
			cmd.SetGlobalTexture(ShaderParams.MainTex, source);
			cmd.DrawMesh(Tools.fullscreenMesh, Matrix4x4.identity, material, 0, passIndex);
		}

		public void Cleanup()
		{
			Shader.SetGlobalInt(ShaderParams.ForcedInvisible, 0);
		}
	}

	[SerializeField]
	[HideInInspector]
	private Shader blurShader;

	private VolumetricFogRenderPass fogRenderPass;

	private BlurRenderPass blurRenderPass;

	public static bool installed;

	public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;

	[Tooltip("Specify which fog volumes will be rendered by this feature.")]
	public LayerMask fogLayerMask = -1;

	[Tooltip("Specify which cameras can execute this render feature. If you have several cameras in your scene, make sure only the correct cameras use this feature in order to optimize performance.")]
	public LayerMask cameraLayerMask = -1;

	[Tooltip("Ignores reflection probes from executing this render feature")]
	public bool ignoreReflectionProbes = true;

	public static int GetScaledSize(int size, float factor)
	{
		size = (int)((float)size / factor);
		size /= 2;
		if (size < 1)
		{
			size = 1;
		}
		return size * 2;
	}

	private void OnDisable()
	{
		installed = false;
		if (blurRenderPass != null)
		{
			blurRenderPass.Cleanup();
		}
	}

	private void OnDestroy()
	{
		if (fogRenderPass != null)
		{
			fogRenderPass.CleanUp();
		}
	}

	public override void Create()
	{
		base.name = "Volumetric Fog 2";
		fogRenderPass = new VolumetricFogRenderPass();
		blurRenderPass = new BlurRenderPass();
		blurShader = Shader.Find("Hidden/VolumetricFog2/Blur");
		if (blurShader == null)
		{
			Debug.LogWarning("Could not load Volumetric Fog composition shader.");
		}
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		if (VolumetricFog.volumetricFogs.Count == 0)
		{
			return;
		}
		VolumetricFogManager managerIfExists = VolumetricFogManager.GetManagerIfExists();
		if (managerIfExists == null || (managerIfExists.downscaling <= 1f && managerIfExists.blurPasses < 1 && managerIfExists.scattering <= 0f))
		{
			Shader.SetGlobalInt(ShaderParams.ForcedInvisible, 0);
			return;
		}
		Camera camera = renderingData.cameraData.camera;
		CameraType cameraType = camera.cameraType;
		if (cameraType != CameraType.Preview && (!ignoreReflectionProbes || cameraType != CameraType.Reflection) && ((int)fogLayerMask & camera.cullingMask) != 0 && ((int)cameraLayerMask & (1 << camera.gameObject.layer)) != 0 && (!(camera.targetTexture != null) || camera.targetTexture.format != RenderTextureFormat.Depth))
		{
			fogRenderPass.Setup(this);
			blurRenderPass.Setup(blurShader, renderer, this);
			renderer.EnqueuePass(fogRenderPass);
			renderer.EnqueuePass(blurRenderPass);
			installed = true;
		}
	}
}
