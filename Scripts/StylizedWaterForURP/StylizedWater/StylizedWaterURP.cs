using UnityEngine;
using UnityEngine.Rendering;

namespace StylizedWater;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
[ExecuteAlways]
[AddComponentMenu("Effects/Stylized Water")]
[HelpURL("https://alexander-ameye.gitbook.io/stylized-water/")]
public class StylizedWaterURP : MonoBehaviour
{
	public enum FoamMovement
	{
		Directional = 0,
		Shore = 1
	}

	public enum Lighting
	{
		Enabled = 0,
		Disabled = 1
	}

	public enum WaterUVs
	{
		Local = 0,
		World = 1
	}

	public bool refractionExpanded;

	public bool vertexPaintingExpanded;

	public bool surfaceFoamExpanded;

	public bool foamShadowsExpanded;

	public bool intersectionEffectsExpanded;

	public bool planarReflectionsExpanded;

	private const string shaderName = "Stylized Water";

	private const string mobileShaderName = "Stylized Water Mobile";

	private const string underwaterShaderName = "Stylized Water Underwater";

	[Range(0f, 1f)]
	public float reflectionStrength;

	[Range(0f, 7f)]
	public float reflectionFresnel = 1f;

	public bool useColorGradient;

	[GradientUsage(true)]
	public Gradient colorGradient;

	public Texture2D colorGradientTexture;

	[ColorUsage(true, true)]
	public Color shallowColor;

	[ColorUsage(true, true)]
	public Color deepColor;

	[Range(0f, 4f)]
	public float colorDepth;

	[ColorUsage(true, true)]
	public Color horizonColor;

	[Range(0f, 20f)]
	public float horizonDistance;

	[Range(0f, 1f)]
	public float shoreFade;

	[ColorUsage(false, false)]
	public Color shoreColor;

	[Range(0f, 1f)]
	public float shoreStrength;

	[Range(0f, 1f)]
	public float shoreBlend;

	[Range(0f, 3f)]
	public float shoreDepth;

	[ColorUsage(false, false)]
	public Color waveColor;

	[ColorUsage(false, false)]
	public Color underwaterColor;

	[Range(0f, 1f)]
	public float underwaterColorStrength;

	[Range(0f, 0.3f)]
	public float underwaterRefractionStrength;

	public Texture surfaceFoamTexture;

	public Vector2 surfaceFoamSampling;

	[Range(0f, 1f)]
	public float surfaceFoamCutoff;

	[Range(0f, 3f)]
	public float surfaceFoamDistortion;

	[Range(0f, 1f)]
	public float surfaceFoamBlend;

	[ColorUsage(true, false)]
	public Color surfaceFoamColor1;

	[ColorUsage(true, false)]
	public Color surfaceFoamColor2;

	public Vector4 surfaceFoamMovement;

	[Range(0f, 1f)]
	public float surfaceFoamDirection1;

	[Range(0f, 1f)]
	public float surfaceFoamDirection2;

	[Range(0f, 2f)]
	public float surfaceFoamSpeed1;

	[Range(0f, 2f)]
	public float surfaceFoamSpeed2;

	public Vector4 surfaceFoamTilingAndOffset;

	[Range(0f, 1f)]
	public float surfaceFoamOffsetX;

	[Range(0f, 1f)]
	public float surfaceFoamOffsetY;

	[Range(0f, 2f)]
	public float surfaceFoamScale1;

	[Range(0f, 2f)]
	public float surfaceFoamScale2;

	public bool enableSurfaceFoam;

	[Range(0f, 1f)]
	public float surfaceFoamHeightMask;

	[Range(0f, 1f)]
	public float surfaceFoamHeightMaskSmoothness;

	public bool enableHeightMask;

	[Range(0f, 1f)]
	public float foamShadowStrength;

	[Range(0f, 10f)]
	public float foamShadowDepth;

	[Range(0f, 15f)]
	public float surfaceFoamShadowProjection;

	[Range(0f, 15f)]
	public float intersectionFoamShadowProjection;

	public FoamMovement foamMovement;

	[Range(0f, 3f)]
	public float intersectionFoamDepth;

	public Texture intersectionFoamTexture;

	[ColorUsage(true, false)]
	public Color intersectionFoamColor;

	public Vector2 intersectionFoamMovement;

	[Range(0f, 1f)]
	public float intersectionFoamDirection;

	[Range(0f, 2f)]
	public float intersectionFoamSpeed;

	[Range(0f, 2f)]
	public float intersectionFoamScale;

	public Vector2 intersectionFoamSampling;

	[Range(0f, 1f)]
	public float intersectionFoamCutoff;

	[Range(0f, 3f)]
	public float intersectionFoamDistortion;

	public bool enableIntersectionEffects;

	[Range(0f, 1f)]
	public float intersectionFoamBlend;

	[Range(0f, 1f)]
	public float intersectionWaterBlend;

	[Range(-5f, 5f)]
	public float shoreFoamSpeed;

	[Range(0f, 2f)]
	public float shoreFoamWidth;

	[Range(0f, 40f)]
	public float shoreFoamFrequency;

	[Range(0f, 5f)]
	public float shoreFoamBreakupStrength;

	[Range(0f, 50f)]
	public float shoreFoamBreakupScale;

	public Lighting lighting;

	[Range(0f, 1f)]
	public float lightingSmoothness;

	[ColorUsage(false, true)]
	public Color lightingSpecularColor;

	[ColorUsage(false, true)]
	public Color lightingDiffuseColor;

	[Range(0f, 1f)]
	public float lightingHardness;

	public Texture normalsTexture;

	public Vector2 normalsMovement;

	[Range(0f, 1f)]
	public float normalsStrength;

	[Range(0.01f, 2f)]
	public float normalsScale;

	[Range(0f, 0.3f)]
	public float normalsSpeed;

	[Range(0f, 0.3f)]
	public float refractionStrength;

	public Vector3 waveVisuals;

	[Range(0f, 0.5f)]
	public float waveSteepness;

	[Range(0f, 20f)]
	public float waveLength;

	[Range(0f, 3f)]
	public float waveSpeed;

	public Vector4 waveDirections;

	[Range(0f, 1f)]
	public float waveDirection1;

	[Range(0f, 1f)]
	public float waveDirection2;

	[Range(0f, 1f)]
	public float waveDirection3;

	[Range(0f, 1f)]
	public float waveDirection4;

	public WaterUVs waterUVs;

	public bool hideComponents;

	public bool enableFoamShadows;

	public bool enableRefraction;

	public MeshRenderer meshRenderer;

	public MeshFilter meshFilter;

	public Material material;

	private void OnEnable()
	{
		if (!meshRenderer)
		{
			meshRenderer = GetComponent<MeshRenderer>();
		}
		if (!meshFilter)
		{
			meshFilter = GetComponent<MeshFilter>();
		}
		material = meshRenderer.sharedMaterial;
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		if ((bool)material && (bool)meshRenderer && (bool)meshFilter && !Application.isPlaying)
		{
			meshRenderer.sharedMaterial.hideFlags = (hideComponents ? HideFlags.HideInInspector : HideFlags.None);
			meshRenderer.hideFlags = (hideComponents ? HideFlags.HideInInspector : HideFlags.None);
			meshFilter.hideFlags = (hideComponents ? HideFlags.HideInInspector : HideFlags.None);
		}
		base.gameObject.layer = 4;
		ReadMaterialProperties();
		WriteMaterialProperties();
	}

	private void Reset()
	{
		OnEnable();
	}

	private void ResetHideFlags()
	{
		if ((bool)material && (bool)meshRenderer && (bool)meshFilter && !Application.isPlaying)
		{
			meshRenderer.sharedMaterial.hideFlags = HideFlags.None;
			meshRenderer.hideFlags = HideFlags.None;
			meshFilter.hideFlags = HideFlags.None;
		}
	}

	private void OnDisable()
	{
		ResetHideFlags();
	}

	private void OnDestroy()
	{
		ResetHideFlags();
	}

	private void SafeDestroyObject(Object obj)
	{
		if (Application.isEditor)
		{
			Object.DestroyImmediate(obj);
		}
		else
		{
			Object.Destroy(obj);
		}
	}

	public void ReadMaterialProperties()
	{
		if ((bool)meshRenderer)
		{
			material = meshRenderer.sharedMaterial;
		}
		if (!material || (material.shader.name != "Stylized Water" && material.shader.name != "Stylized Water Mobile" && material.shader.name != "Stylized Water Underwater"))
		{
			return;
		}
		enableRefraction = (material.IsKeywordEnabled("REFRACTION_ON") ? true : false);
		waveVisuals = material.GetVector("_WaveVisuals");
		waveSteepness = waveVisuals.x;
		waveLength = waveVisuals.y;
		waveSpeed = waveVisuals.z;
		waveDirections = material.GetVector("_WaveDirections");
		waveDirection1 = waveDirections.x;
		waveDirection2 = waveDirections.y;
		waveDirection3 = waveDirections.z;
		waveDirection4 = waveDirections.w;
		refractionStrength = material.GetFloat("_RefractionStrength");
		normalsMovement = material.GetVector("_NormalsMovement");
		normalsSpeed = normalsMovement.x;
		normalsScale = normalsMovement.y;
		if (material.shader.name == "Stylized Water" || material.shader.name == "Stylized Water Underwater")
		{
			underwaterColor = material.GetColor("_WaterColorUnderwater");
		}
		if (material.shader.name == "Stylized Water" || material.shader.name == "Stylized Water Mobile")
		{
			shallowColor = material.GetColor("_WaterColorShallow");
			deepColor = material.GetColor("_WaterColorDeep");
			horizonColor = material.GetColor("_WaterColorHorizon");
			colorDepth = material.GetFloat("_WaterColorDepth");
			horizonDistance = material.GetFloat("_WaterColorHorizonDistance");
			waveColor = material.GetColor("_WaveColor");
			shoreColor = material.GetColor("_ShoreColor");
			shoreDepth = material.GetFloat("_ShoreDepth");
			shoreFade = material.GetFloat("_ShoreFade");
			shoreBlend = material.GetFloat("_ShoreBlend");
			surfaceFoamTexture = material.GetTexture("_SurfaceFoamTexture");
			surfaceFoamSampling = material.GetVector("_SurfaceFoamSampling");
			surfaceFoamCutoff = surfaceFoamSampling.x;
			surfaceFoamDistortion = surfaceFoamSampling.y;
			surfaceFoamBlend = material.GetFloat("_SurfaceFoamBlend");
			surfaceFoamColor1 = material.GetColor("_SurfaceFoamColor1");
			surfaceFoamColor2 = material.GetColor("_SurfaceFoamColor2");
			surfaceFoamMovement = material.GetVector("_SurfaceFoamMovement");
			surfaceFoamDirection1 = surfaceFoamMovement.x;
			surfaceFoamDirection2 = surfaceFoamMovement.z;
			surfaceFoamSpeed1 = surfaceFoamMovement.y;
			surfaceFoamSpeed2 = surfaceFoamMovement.w;
			surfaceFoamTilingAndOffset = material.GetVector("_SurfaceFoamTilingAndOffset");
			surfaceFoamOffsetX = surfaceFoamTilingAndOffset.x;
			surfaceFoamOffsetY = surfaceFoamTilingAndOffset.y;
			surfaceFoamScale1 = surfaceFoamTilingAndOffset.z;
			surfaceFoamScale2 = surfaceFoamTilingAndOffset.w;
			enableSurfaceFoam = (material.IsKeywordEnabled("SURFACE_FOAM_ON") ? true : false);
			if (material.IsKeywordEnabled("WORLD_SPACE_UV_ON"))
			{
				waterUVs = WaterUVs.World;
			}
			else
			{
				waterUVs = WaterUVs.Local;
			}
		}
		if (material.shader.name == "Stylized Water")
		{
			useColorGradient = (material.IsKeywordEnabled("COLOR_GRADIENT_ON") ? true : false);
			colorGradientTexture = (Texture2D)material.GetTexture("_WaterColorGradientTexture");
			underwaterRefractionStrength = material.GetFloat("_UnderwaterRefractionStrength");
			foamShadowStrength = material.GetFloat("_FoamShadowStrength");
			foamShadowDepth = material.GetFloat("_FoamShadowDepth");
			surfaceFoamShadowProjection = material.GetFloat("_SurfaceFoamShadowProjection");
			intersectionFoamShadowProjection = material.GetFloat("_IntersectionFoamShadowProjection");
			surfaceFoamHeightMaskSmoothness = material.GetFloat("_SurfaceFoamHeightMaskSmoothness");
			surfaceFoamHeightMask = material.GetFloat("_SurfaceFoamHeightMask");
			intersectionFoamBlend = material.GetFloat("_IntersectionFoamBlend");
			intersectionWaterBlend = material.GetFloat("_IntersectionWaterBlend");
			intersectionFoamDepth = material.GetFloat("_IntersectionFoamDepth");
			intersectionFoamTexture = material.GetTexture("_IntersectionFoamTexture");
			intersectionFoamColor = material.GetColor("_IntersectionFoamColor");
			intersectionFoamMovement = material.GetVector("_IntersectionFoamMovement");
			intersectionFoamDirection = intersectionFoamMovement.x;
			intersectionFoamSpeed = intersectionFoamMovement.y;
			intersectionFoamScale = material.GetFloat("_IntersectionFoamScale");
			intersectionFoamSampling = material.GetVector("_IntersectionFoamSampling");
			intersectionFoamCutoff = intersectionFoamSampling.x;
			intersectionFoamDistortion = intersectionFoamSampling.y;
			enableIntersectionEffects = (material.IsKeywordEnabled("INTERSECTION_EFFECTS_ON") ? true : false);
			shoreFoamSpeed = material.GetFloat("_ShoreFoamSpeed");
			shoreFoamWidth = material.GetFloat("_ShoreFoamWidth");
			shoreFoamFrequency = material.GetFloat("_ShoreFoamFrequency");
			shoreFoamBreakupScale = material.GetFloat("_ShoreFoamBreakupScale");
			shoreFoamBreakupStrength = material.GetFloat("_ShoreFoamBreakupStrength");
			if (material.IsKeywordEnabled("WATER_LIGHTING_ON"))
			{
				lighting = Lighting.Enabled;
			}
			else
			{
				lighting = Lighting.Disabled;
			}
			if (material.IsKeywordEnabled("SHORE_MOVEMENT_ON"))
			{
				foamMovement = FoamMovement.Shore;
			}
			else
			{
				foamMovement = FoamMovement.Directional;
			}
			lightingSmoothness = material.GetFloat("_LightingSmoothness");
			lightingSpecularColor = material.GetColor("_LightingSpecularColor");
			lightingDiffuseColor = material.GetColor("_LightingDiffuseColor");
			lightingHardness = material.GetFloat("_LightingHardness");
			normalsTexture = material.GetTexture("_NormalsTexture");
			normalsStrength = material.GetFloat("_NormalsStrength");
			reflectionStrength = material.GetFloat("_PlanarReflectionStrength");
			reflectionFresnel = material.GetFloat("_PlanarReflectionFresnel");
			enableFoamShadows = (material.IsKeywordEnabled("FOAM_SHADOWS_ON") ? true : false);
		}
	}

	public void WriteMaterialProperties()
	{
		if (!material || (material.shader.name != "Stylized Water" && material.shader.name != "Stylized Water Mobile" && material.shader.name != "Stylized Water Underwater"))
		{
			return;
		}
		if ((bool)material && (bool)meshRenderer && (bool)meshFilter && (bool)meshRenderer.sharedMaterial && !Application.isPlaying)
		{
			meshRenderer.sharedMaterial.hideFlags = (hideComponents ? HideFlags.HideInInspector : HideFlags.None);
			meshRenderer.hideFlags = (hideComponents ? HideFlags.HideInInspector : HideFlags.None);
			meshFilter.hideFlags = (hideComponents ? HideFlags.HideInInspector : HideFlags.None);
		}
		if (useColorGradient)
		{
			material.EnableKeyword("COLOR_GRADIENT_ON");
		}
		else
		{
			material.DisableKeyword("COLOR_GRADIENT_ON");
		}
		material.SetTexture("_WaterColorGradientTexture", colorGradientTexture);
		material.SetColor("_WaterColorShallow", shallowColor);
		material.SetColor("_WaterColorHorizon", horizonColor);
		material.SetFloat("_WaterColorDepth", colorDepth);
		material.SetFloat("_WaterColorHorizonDistance", horizonDistance);
		material.SetColor("_WaveColor", waveColor);
		material.SetColor("_WaterColorDeep", deepColor);
		underwaterColor.a = underwaterColorStrength;
		material.SetColor("_WaterColorUnderwater", underwaterColor);
		material.SetFloat("_UnderwaterRefractionStrength", underwaterRefractionStrength);
		shoreColor.a = shoreStrength;
		material.SetColor("_ShoreColor", shoreColor);
		material.SetFloat("_ShoreDepth", shoreDepth);
		material.SetFloat("_ShoreFade", shoreFade);
		material.SetFloat("_ShoreBlend", shoreBlend);
		material.SetFloat("_FoamShadowStrength", foamShadowStrength);
		material.SetFloat("_FoamShadowDepth", foamShadowDepth);
		material.SetFloat("_SurfaceFoamShadowProjection", surfaceFoamShadowProjection);
		material.SetFloat("_IntersectionFoamShadowProjection", intersectionFoamShadowProjection);
		material.SetTexture("_SurfaceFoamTexture", surfaceFoamTexture);
		material.SetVector("_SurfaceFoamSampling", new Vector2(surfaceFoamCutoff, surfaceFoamDistortion));
		material.SetFloat("_SurfaceFoamBlend", surfaceFoamBlend);
		material.SetColor("_SurfaceFoamColor1", surfaceFoamColor1);
		material.SetColor("_SurfaceFoamColor2", surfaceFoamColor2);
		material.SetVector("_SurfaceFoamMovement", new Vector4(surfaceFoamDirection1, surfaceFoamSpeed1, surfaceFoamDirection2, surfaceFoamSpeed2));
		material.SetVector("_SurfaceFoamTilingAndOffset", new Vector4(surfaceFoamOffsetX, surfaceFoamOffsetY, surfaceFoamScale1, surfaceFoamScale2));
		if (enableSurfaceFoam)
		{
			material.EnableKeyword("SURFACE_FOAM_ON");
		}
		else
		{
			material.DisableKeyword("SURFACE_FOAM_ON");
		}
		if (enableHeightMask)
		{
			material.SetFloat("_SurfaceFoamHeightMask", surfaceFoamHeightMask);
			material.SetFloat("_SurfaceFoamHeightMaskSmoothness", surfaceFoamHeightMaskSmoothness);
		}
		else
		{
			material.SetFloat("_SurfaceFoamHeightMask", 0f);
			material.SetFloat("_SurfaceFoamHeightMaskSmoothness", 0f);
		}
		material.SetFloat("_IntersectionFoamBlend", intersectionFoamBlend);
		material.SetFloat("_IntersectionWaterBlend", intersectionWaterBlend);
		material.SetFloat("_IntersectionFoamDepth", intersectionFoamDepth);
		material.SetTexture("_IntersectionFoamTexture", intersectionFoamTexture);
		material.SetColor("_IntersectionFoamColor", intersectionFoamColor);
		material.SetFloat("_IntersectionFoamScale", intersectionFoamScale);
		material.SetVector("_IntersectionFoamMovement", new Vector2(intersectionFoamDirection, intersectionFoamSpeed));
		material.SetVector("_IntersectionFoamSampling", new Vector2(intersectionFoamCutoff, intersectionFoamDistortion));
		if (enableIntersectionEffects)
		{
			material.EnableKeyword("INTERSECTION_EFFECTS_ON");
		}
		else
		{
			material.DisableKeyword("INTERSECTION_EFFECTS_ON");
		}
		material.SetFloat("_ShoreFoamSpeed", shoreFoamSpeed);
		material.SetFloat("_ShoreFoamWidth", shoreFoamWidth);
		material.SetFloat("_ShoreFoamFrequency", shoreFoamFrequency);
		material.SetFloat("_ShoreFoamBreakupStrength", shoreFoamBreakupStrength);
		material.SetFloat("_ShoreFoamBreakupScale", shoreFoamBreakupScale);
		material.SetVector("_WaveVisuals", new Vector3(waveSteepness, waveLength, waveSpeed));
		material.SetVector("_WaveDirections", new Vector4(waveDirection1, waveDirection2, waveDirection3, waveDirection4));
		if (waterUVs == WaterUVs.World)
		{
			material.EnableKeyword("WORLD_SPACE_UV_ON");
		}
		else
		{
			material.DisableKeyword("WORLD_SPACE_UV_ON");
		}
		if (lighting == Lighting.Enabled)
		{
			material.EnableKeyword("WATER_LIGHTING_ON");
		}
		else
		{
			material.DisableKeyword("WATER_LIGHTING_ON");
		}
		if (foamMovement == FoamMovement.Shore)
		{
			material.EnableKeyword("SHORE_MOVEMENT_ON");
		}
		else
		{
			material.DisableKeyword("SHORE_MOVEMENT_ON");
		}
		material.SetFloat("_LightingSmoothness", lightingSmoothness);
		material.SetColor("_LightingSpecularColor", lightingSpecularColor);
		material.SetColor("_LightingDiffuseColor", lightingDiffuseColor);
		material.SetFloat("_LightingHardness", lightingHardness);
		material.SetTexture("_NormalsTexture", normalsTexture);
		material.SetVector("_NormalsMovement", new Vector2(normalsSpeed, normalsScale));
		material.SetFloat("_NormalsStrength", normalsStrength);
		material.SetFloat("_PlanarReflectionStrength", reflectionStrength);
		material.SetFloat("_PlanarReflectionFresnel", reflectionFresnel);
		material.SetFloat("_RefractionStrength", refractionStrength);
		if (material.shader.name == "Stylized Water Mobile" || material.shader.name == "Stylized Water Underwater")
		{
			if (enableRefraction)
			{
				material.EnableKeyword("REFRACTION_ON");
			}
			else
			{
				material.DisableKeyword("REFRACTION_ON");
			}
		}
		if (enableFoamShadows)
		{
			material.EnableKeyword("FOAM_SHADOWS_ON");
		}
		else
		{
			material.DisableKeyword("FOAM_SHADOWS_ON");
		}
	}

	public float GetWaveSteepness()
	{
		return waveSteepness;
	}

	public float GetWaveLength()
	{
		return waveLength;
	}

	public float GetWaveSpeed()
	{
		return waveSpeed;
	}

	public float[] GetWaveDirections()
	{
		return new float[4] { waveDirection1, waveDirection2, waveDirection3, waveDirection4 };
	}
}
