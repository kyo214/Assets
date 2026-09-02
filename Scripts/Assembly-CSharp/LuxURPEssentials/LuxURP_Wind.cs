using UnityEngine;

namespace LuxURPEssentials;

[ExecuteAlways]
[RequireComponent(typeof(WindZone))]
public class LuxURP_Wind : MonoBehaviour
{
	[Space(5f)]
	[LuxURP_HelpBtn("h.wnnhm4pxp610")]
	[Space(3f)]
	public bool UpdateInEditMode;

	[Header("Render Texture Settings")]
	[Space(4f)]
	[Tooltip("Smaller resoltions will speed up rendering but may result in some quantization regarding the final bending.")]
	public RTSize Resolution = RTSize._256;

	[Tooltip("ARGB32 needs less memory and bandwidth but creates a slightly more quantized results - while ARGBHalf needs more memory and bandwith but gives you smoother results.")]
	public RTFormat Format;

	[Tooltip("Expects an RGBA texture with diffirently scaled noise patterns. If left empty the script will grab the default one.")]
	public Texture WindBaseTex;

	public Shader WindCompositeShader;

	[Header("Wind Frequency and Turbulence")]
	[Space(4f)]
	[Range(0.1f, 1f)]
	[Tooltip("Drives the frequency of the turbulence animation according to the main wind strength.")]
	public float WindToFrequency = 0.25f;

	[Tooltip("Drives the strength of turbulence according to the main wind strength.")]
	public AnimationCurve WindToTurbulence = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(5f, 1f));

	[Range(0f, 4f)]
	[Tooltip("Scales the final turbulence value used by the shaders.")]
	public float MaxTurbulence = 0.5f;

	[Header("Wind Speed and Size")]
	[Space(4f)]
	[Tooltip("Base Wind Speed in km/h at Main = 1 (WindZone).")]
	public float BaseWindSpeed = 15f;

	[Tooltip("Size of the Wind RenderTexture in World Space.")]
	public float SizeInWorldSpace = 50f;

	[Space(4f)]
	[Tooltip("Speed of Layer0 (red channel) relative to the Base Wind Speed.")]
	public float speedLayer0 = 1f;

	[Tooltip("Speed of Layer1 (green channel) relative to the Base Wind Speed.")]
	public float speedLayer1 = 1.137f;

	[Tooltip("Speed of Layer3 (blue channel) relative to the Base Wind Speed.")]
	public float speedLayer2 = 1.376f;

	[Header("Noise")]
	[Space(4f)]
	[Tooltip("Tiling of the gust layer (alpha channel) relative to Size In WorldSpace.")]
	public int GrassGustTiling = 4;

	[Tooltip("Speed of the gust layer (alpha channel) relative to the Base Wind Speed.")]
	public float GrassGustSpeed = 0.278f;

	[Tooltip("Lets you choose a Wind Layer you want the dedicated Gust sample to be combined with.")]
	public GustMixLayer LayerToMixWith = GustMixLayer.Layer_1;

	[Header("Wind Multipliers")]
	[Space(4f)]
	public float Grass = 1f;

	public float Foliage = 1f;

	public float Trees = 1f;

	private RenderTexture WindRenderTexture;

	private Material m_material;

	private Vector2 uvs = new Vector2(0f, 0f);

	private Vector2 uvs1 = new Vector2(0f, 0f);

	private Vector2 uvs2 = new Vector2(0f, 0f);

	private Vector2 uvs3 = new Vector2(0f, 0f);

	private Transform trans;

	private WindZone windZone;

	private float mainWind;

	private static readonly int WindRTPID = Shader.PropertyToID("_LuxURPWindRT");

	private static readonly int LuxURPWindDirSizePID = Shader.PropertyToID("_LuxURPWindDirSize");

	private static readonly int LuxURPWindStrengthMultipliersPID = Shader.PropertyToID("_LuxURPWindStrengthMultipliers");

	private static readonly int LuxURPSinTimePID = Shader.PropertyToID("_LuxURPSinTime");

	private static readonly int LuxURPGustPID = Shader.PropertyToID("_LuxURPGust");

	private static readonly int LuxURPGustMixLayerPID = Shader.PropertyToID("_LuxURPGustMixLayer");

	private static readonly int _LuxURPWindStrengthTurbulencePulsemagnitudePulseFrequency = Shader.PropertyToID("_LuxURPWindStrengthTurbulencePulsemagnitudePulseFrequency");

	private static readonly int LuxURPBendFrequencyPID = Shader.PropertyToID("_LuxURPBendFrequency");

	private static readonly int LuxURPWindUVsPID = Shader.PropertyToID("_LuxURPWindUVs");

	private static readonly int LuxURPWindUVs1PID = Shader.PropertyToID("_LuxURPWindUVs1");

	private static readonly int LuxURPWindUVs2PID = Shader.PropertyToID("_LuxURPWindUVs2");

	private static readonly int LuxURPWindUVs3PID = Shader.PropertyToID("_LuxURPWindUVs3");

	private int previousRTSize;

	private int previousRTFormat;

	private Vector4 WindDirectionSize = Vector4.zero;

	private float WindTurbulence;

	private static Vector3[] MixLayers = new Vector3[3]
	{
		new Vector3(1f, 0f, 0f),
		new Vector3(0f, 1f, 0f),
		new Vector3(0f, 0f, 1f)
	};

	private double currentTime;

	private double domainTime_Wind;

	private float temp_WindFrequency = 0.25f;

	private float freqSpeed = 0.0125f;

	private float currentWindPulseFrequency = -1f;

	private double domainTime_Pulse;

	private double OneOverPi = 0.31830987334251404;

	private void OnEnable()
	{
		if (WindCompositeShader == null)
		{
			WindCompositeShader = Shader.Find("Hidden/Lux URP WindComposite");
		}
		if (WindBaseTex == null)
		{
			WindBaseTex = Resources.Load("Lux URP default wind base texture") as Texture;
		}
		SetupRT();
		trans = base.transform;
		windZone = trans.GetComponent<WindZone>();
		previousRTSize = (int)Resolution;
		previousRTFormat = (int)Format;
		currentWindPulseFrequency = windZone.windPulseFrequency;
	}

	private void OnDisable()
	{
		if (WindRenderTexture != null)
		{
			WindRenderTexture.Release();
			Object.DestroyImmediate(WindRenderTexture);
			WindRenderTexture = null;
		}
		if (m_material != null)
		{
			Object.DestroyImmediate(m_material);
			m_material = null;
		}
		if (WindBaseTex != null)
		{
			WindBaseTex = null;
		}
	}

	private void SetupRT()
	{
		if (WindRenderTexture == null || m_material == null)
		{
			RenderTextureFormat format = ((Format != RTFormat.ARGB32) ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32);
			WindRenderTexture = new RenderTexture((int)Resolution, (int)Resolution, 0, format, RenderTextureReadWrite.Linear);
			WindRenderTexture.useMipMap = true;
			WindRenderTexture.wrapMode = TextureWrapMode.Repeat;
			m_material = new Material(WindCompositeShader);
		}
	}

	private void OnValidate()
	{
		if (WindCompositeShader == null)
		{
			WindCompositeShader = Shader.Find("Hidden/Lux URP WindComposite");
		}
		if (WindBaseTex == null)
		{
			WindBaseTex = Resources.Load("Default wind base texture") as Texture;
		}
		if (previousRTSize != (int)Resolution || previousRTFormat != (int)Format)
		{
			RenderTextureFormat format = ((Format != RTFormat.ARGB32) ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32);
			WindRenderTexture = new RenderTexture((int)Resolution, (int)Resolution, 0, format, RenderTextureReadWrite.Linear);
			WindRenderTexture.useMipMap = true;
			WindRenderTexture.wrapMode = TextureWrapMode.Repeat;
		}
	}

	private void Update()
	{
		mainWind = windZone.windMain;
		WindTurbulence = MaxTurbulence * WindToTurbulence.Evaluate(mainWind);
		float deltaTime = Time.deltaTime;
		currentTime += deltaTime;
		temp_WindFrequency = Mathf.MoveTowards(temp_WindFrequency, mainWind * WindToFrequency, freqSpeed);
		domainTime_Wind += deltaTime * (1f + temp_WindFrequency);
		Shader.SetGlobalFloat(LuxURPBendFrequencyPID, (float)domainTime_Wind);
		WindDirectionSize.x = trans.forward.x;
		WindDirectionSize.y = trans.forward.y;
		WindDirectionSize.z = trans.forward.z;
		WindDirectionSize.w = 1f / SizeInWorldSpace;
		Vector2 vector = new Vector2(WindDirectionSize.x, WindDirectionSize.z) * deltaTime * (BaseWindSpeed * 0.2777f * WindDirectionSize.w);
		uvs -= vector * speedLayer0;
		uvs.x -= (float)(int)uvs.x;
		uvs.y -= (float)(int)uvs.y;
		uvs1 -= vector * speedLayer1;
		uvs1.x -= (float)(int)uvs1.x;
		uvs1.y -= (float)(int)uvs1.y;
		uvs2 -= vector * speedLayer2;
		uvs2.x -= (float)(int)uvs2.x;
		uvs2.y -= (float)(int)uvs2.y;
		uvs3 -= vector * GrassGustSpeed * WindTurbulence;
		uvs3.x -= (float)(int)uvs3.x;
		uvs3.y -= (float)(int)uvs3.y;
		Shader.SetGlobalVector(LuxURPWindDirSizePID, WindDirectionSize);
		float windTurbulence = windZone.windTurbulence;
		float windPulseMagnitude = windZone.windPulseMagnitude;
		float windPulseFrequency = windZone.windPulseFrequency;
		float x = mainWind * Trees;
		currentWindPulseFrequency = Mathf.MoveTowards(currentWindPulseFrequency, windPulseFrequency, freqSpeed);
		domainTime_Pulse += deltaTime * (1f + currentWindPulseFrequency);
		float w = (float)(domainTime_Pulse * OneOverPi);
		Shader.SetGlobalVector(_LuxURPWindStrengthTurbulencePulsemagnitudePulseFrequency, new Vector4(x, windTurbulence, windPulseMagnitude, w));
		Vector2 vector2 = default;
		vector2.x = Grass * mainWind;
		vector2.y = Foliage * mainWind;
		Shader.SetGlobalVector(LuxURPWindStrengthMultipliersPID, vector2);
		Shader.SetGlobalVector(LuxURPGustPID, new Vector2(GrassGustTiling, WindTurbulence));
		Shader.SetGlobalVector(LuxURPWindUVsPID, uvs);
		Shader.SetGlobalVector(LuxURPWindUVs1PID, uvs1);
		Shader.SetGlobalVector(LuxURPWindUVs2PID, uvs2);
		Shader.SetGlobalVector(LuxURPWindUVs3PID, uvs3);
		Shader.SetGlobalVector(LuxURPGustMixLayerPID, MixLayers[(int)LayerToMixWith]);
		Graphics.Blit(WindBaseTex, WindRenderTexture, m_material);
		WindRenderTexture.SetGlobalShaderProperty("_LuxURPWindRT");
	}
}
