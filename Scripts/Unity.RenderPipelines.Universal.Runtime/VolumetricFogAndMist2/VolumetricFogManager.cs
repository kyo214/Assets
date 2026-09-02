using UnityEngine;
using UnityEngine.Rendering;

namespace VolumetricFogAndMist2;

[ExecuteInEditMode]
[HelpURL("https://kronnect.com/guides/volumetric-fog-urp-adding-volumetric-fog-mist-to-your-scene/#ftoc-heading-1")]
public class VolumetricFogManager : MonoBehaviour, IVolumetricFogManager
{
	private static PointLightManager _pointLightManager;

	private static FogVoidManager _fogVoidManager;

	private static VolumetricFogManager _instance;

	[Tooltip("Directional light used as the Sun")]
	public Light sun;

	[Tooltip("Directional light used as the Moon")]
	public Light moon;

	[Tooltip("Flip depth texture. Use only as a workaround to a bug in URP if the depth shows inverted in GameView. Alternatively you can enable MSAA or HDR instead of using this option.")]
	public bool flipDepthTexture;

	[Tooltip("Enable this option to choose this manager when others could be loaded from sub-scenes")]
	public bool mainManager;

	[Tooltip("Optionally specify which transparent layers must be included in the depth prepass. Use only to avoid fog clipping with certain transparent objects.")]
	public LayerMask includeTransparent;

	[Tooltip("Cull mode for the transparent depth prepass")]
	public CullMode transparentCullMode = CullMode.Back;

	[Tooltip("Optionally specify which semi-transparent (materials using alpha clipping or cut-off) must be included in the depth prepass. Use only to avoid fog clipping with certain transparent objects.")]
	public LayerMask includeSemiTransparent;

	[Tooltip("Optionally determines the alpha cut off for semitransparent objects")]
	[Range(0f, 1f)]
	public float alphaCutOff;

	[Tooltip("Light scattering effect through fog")]
	[Range(0f, 1f)]
	public float scattering;

	[Tooltip("Threshold applied to input brightness")]
	public float scatteringThreshold;

	[Tooltip("Brightness multiplier applied to input")]
	public float scatteringIntensity;

	[Tooltip("Absorption or brightness decay inside the fog")]
	[Range(0f, 1f)]
	public float scatteringAbsorption = 0.35f;

	public Color scatteringTint = Color.white;

	[Tooltip("Uses higher resolution intermediate buffers and edge-aware upsampling filter")]
	public bool scatteringHighQuality;

	[Range(1f, 8f)]
	public float downscaling = 1f;

	[Tooltip("Depth-based detection threshold for the upscaling reconstruction filter")]
	public float downscalingEdgeDepthThreshold = 0.05f;

	[Range(0f, 6f)]
	public int blurPasses;

	[Range(1f, 8f)]
	public float blurDownscaling = 1f;

	[Range(0.1f, 4f)]
	public float blurSpread = 1f;

	[Tooltip("Uses 16-bit RGBA floating point pixel format for rendering & blur fog volumes. If disabled, 8-bit RGBA pixel format will be used which can improve performance on certain devices. Note that if you use bloom or other HDR-based effects, you should enable this HDR option as well.")]
	public bool blurHDR = true;

	[Tooltip("Enable to use an edge-aware blur.")]
	public bool blurEdgePreserve = true;

	[Tooltip("Ignores blur when fog color intensity is below this value.")]
	public float blurEdgeDepthThreshold = 0.008f;

	[Range(0f, 0.2f)]
	public float ditherStrength;

	private const string SKW_FLIP_DEPTH_TEXTURE = "VF2_FLIP_DEPTH_TEXTURE";

	public const uint FOG_VOLUMES_RENDERING_LAYER = 131072u;

	public string managerName => "Volumetric Fog Manager";

	public static bool allowFogVoidRotation => false;

	public static VolumetricFogManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Tools.CheckMainManager();
			}
			return _instance;
		}
	}

	public static PointLightManager pointLightManager
	{
		get
		{
			Tools.CheckManager(ref _pointLightManager);
			return _pointLightManager;
		}
	}

	public static FogVoidManager fogVoidManager
	{
		get
		{
			Tools.CheckManager(ref _fogVoidManager);
			return _fogVoidManager;
		}
	}

	public static VolumetricFogManager GetManagerIfExists()
	{
		if (_instance != null && _instance.gameObject == null)
		{
			_instance = null;
		}
		return _instance;
	}

	private void OnEnable()
	{
		_pointLightManager = null;
		_fogVoidManager = null;
		VolumetricFogManager[] array = Object.FindObjectsOfType<VolumetricFogManager>(includeInactive: true);
		if (array.Length > 1)
		{
			bool flag = mainManager;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].mainManager)
				{
					Object.DestroyImmediate(array[i].gameObject);
				}
			}
			if (!flag)
			{
				return;
			}
		}
		if (_instance == null)
		{
			_instance = this;
		}
		SetupLights();
		SetupDepthPrePass();
		Tools.CheckManager(ref _pointLightManager);
		Tools.CheckManager(ref _fogVoidManager);
	}

	private void OnValidate()
	{
		downscalingEdgeDepthThreshold = Mathf.Max(0.0001f, downscalingEdgeDepthThreshold);
		blurEdgeDepthThreshold = Mathf.Max(0.0001f, blurEdgeDepthThreshold);
		scatteringThreshold = Mathf.Max(0f, scatteringThreshold);
		scatteringIntensity = Mathf.Max(0f, scatteringIntensity);
		SetupDepthPrePass();
	}

	private void SetupLights()
	{
		Light[] array = Object.FindObjectsOfType<Light>();
		foreach (Light light in array)
		{
			if (light.type == LightType.Directional)
			{
				if (sun == null)
				{
					sun = light;
				}
				break;
			}
		}
	}

	private void SetupDepthPrePass()
	{
		Shader.SetGlobalInt("VF2_FLIP_DEPTH_TEXTURE", flipDepthTexture ? 1 : 0);
		DepthRenderPrePassFeature.DepthRenderPass.SetupLayerMasks(includeTransparent, includeSemiTransparent);
	}

	public static GameObject CreateFogVolume(string name)
	{
		GameObject obj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/FogVolume2D"));
		obj.name = name;
		return obj;
	}

	public static GameObject CreateFogVoid(string name)
	{
		return new GameObject(name, typeof(FogVoid));
	}

	public static GameObject CreateFogSubVolume(string name)
	{
		GameObject obj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/FogSubVolume"));
		obj.name = name;
		return obj;
	}
}
