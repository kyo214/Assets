using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace StylizedWater;

public class CausticsFeature : ScriptableRendererFeature
{
	[Serializable]
	public class CausticsSettings
	{
		public enum DebugMode
		{
			Disabled = 0,
			Caustics = 1,
			Mask = 2
		}

		[Header("Visuals")]
		[Range(0f, 3f)]
		public float strength = 3f;

		[Range(0f, 1f)]
		public float rgbSplit = 0.3f;

		[Range(0f, 1f)]
		public float shadowMask = 1f;

		[Header("Movement")]
		public Texture2D texture;

		[Range(0.1f, 10f)]
		public float scale = 5f;

		[Range(0f, 0.3f)]
		public float speed = 0.125f;

		[Header("Depth")]
		public float waterLevel;

		public Vector2 depth = new Vector2(0f, 4f);

		[Range(0f, 1f)]
		public float fade = 1f;

		[Header("Rendering")]
		public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingSkybox;

		public DebugMode debug;
	}

	public CausticsSettings settings = new CausticsSettings();

	private CausticsPass causticsPass;

	[SerializeField]
	[HideInInspector]
	private Shader causticsShader;

	private Material causticsMaterial;

	private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");

	private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");

	private static readonly int causticsTextureID = Shader.PropertyToID("_CausticsTexture");

	private static readonly int causticsStrengthID = Shader.PropertyToID("_CausticsStrength");

	private static readonly int causticsScaleID = Shader.PropertyToID("_CausticsScale");

	private static readonly int causticsSpeedID = Shader.PropertyToID("_CausticsSpeed");

	private static readonly int causticsSplitID = Shader.PropertyToID("_CausticsSplit");

	private static readonly int shadowMaskID = Shader.PropertyToID("_CausticsShadowMask");

	private static readonly int causticsFadeID = Shader.PropertyToID("_CausticsFade");

	private static readonly int waterLevelID = Shader.PropertyToID("_WaterLevel");

	private static readonly int causticsStartID = Shader.PropertyToID("_CausticsStart");

	private static readonly int causticsEndID = Shader.PropertyToID("_CausticsEnd");

	public override void Create()
	{
		causticsPass = new CausticsPass(settings.waterLevel);
		if ((bool)causticsMaterial)
		{
			UnityEngine.Object.DestroyImmediate(causticsMaterial);
		}
		causticsShader = Shader.Find("Hidden/Stylized Water/Caustics");
		if (causticsShader != null)
		{
			causticsMaterial = CoreUtils.CreateEngineMaterial(causticsShader);
		}
		if ((bool)causticsMaterial)
		{
			causticsMaterial.SetTexture(causticsTextureID, settings.texture);
			causticsMaterial.SetFloat(causticsStrengthID, settings.strength);
			causticsMaterial.SetFloat(causticsScaleID, settings.scale);
			causticsMaterial.SetFloat(causticsSpeedID, settings.speed);
			causticsMaterial.SetFloat(causticsSplitID, settings.rgbSplit);
			causticsMaterial.SetFloat(shadowMaskID, settings.shadowMask);
			causticsMaterial.SetFloat(causticsFadeID, settings.fade);
			causticsMaterial.SetFloat(waterLevelID, settings.waterLevel);
			causticsMaterial.SetFloat(causticsStartID, settings.depth.x);
			causticsMaterial.SetFloat(causticsEndID, settings.depth.y);
			switch (settings.debug)
			{
			case CausticsSettings.DebugMode.Disabled:
				causticsMaterial.SetFloat(SrcBlend, 2f);
				causticsMaterial.SetFloat(DstBlend, 0f);
				causticsMaterial.DisableKeyword("DEBUG_MASK");
				causticsMaterial.DisableKeyword("DEBUG_CAUSTICS");
				causticsPass.renderPassEvent = settings.renderPassEvent;
				break;
			case CausticsSettings.DebugMode.Caustics:
				causticsMaterial.SetFloat(SrcBlend, 1f);
				causticsMaterial.SetFloat(DstBlend, 0f);
				causticsMaterial.DisableKeyword("DEBUG_MASK");
				causticsMaterial.EnableKeyword("DEBUG_CAUSTICS");
				causticsPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
				break;
			case CausticsSettings.DebugMode.Mask:
				causticsMaterial.SetFloat(SrcBlend, 1f);
				causticsMaterial.SetFloat(DstBlend, 0f);
				causticsMaterial.DisableKeyword("DEBUG_CAUSTICS");
				causticsMaterial.EnableKeyword("DEBUG_MASK");
				causticsPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
				break;
			}
			causticsPass.causticsMaterial = causticsMaterial;
		}
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		renderer.EnqueuePass(causticsPass);
	}
}
