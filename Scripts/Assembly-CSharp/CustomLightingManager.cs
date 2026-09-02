using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class CustomLightingManager : MonoBehaviour
{
	private readonly int LIGHT_MAP = Shader.PropertyToID("_LightMap");

	private readonly int LIGHT_DIRECTION_MAP = Shader.PropertyToID("_LightDirectionMap");

	private readonly int STATIC_LIGHT_MAP = Shader.PropertyToID("_StaticLightMap");

	private readonly int STATIC_LIGHT_DIRECTION_MAP = Shader.PropertyToID("_StaticLightDirectionMap");

	private readonly int LIGHT_MAP_SIZE = Shader.PropertyToID("_LightMapSize");

	private readonly int LIGHT_MAP_ORIGIN = Shader.PropertyToID("_LightMapOrigin");

	private readonly int LIGHT_POSITIONS = Shader.PropertyToID("_LightPositions");

	private readonly int LIGHT_COLORS = Shader.PropertyToID("_LightColors");

	private readonly int LIGHT_SPOT_DIRS = Shader.PropertyToID("_LightSpotDirs");

	private readonly int LIGHT_SPOT_ANGLES = Shader.PropertyToID("_LightSpotAngles");

	private readonly int AREA_LIGHT_POSITIONS = Shader.PropertyToID("_AreaLightPositions");

	private readonly int AREA_LIGHT_COLORS = Shader.PropertyToID("_AreaLightColors");

	private readonly int AREA_LIGHT_SHAPE = Shader.PropertyToID("_AreaLightShape");

	private readonly int AREA_LIGHT_DATA = Shader.PropertyToID("_AreaLightData");

	private readonly int LIGHT_COUNT = Shader.PropertyToID("_LightCount");

	private readonly int AREA_LIGHT_COUNT = Shader.PropertyToID("_AreaLightCount");

	private readonly int LIGHT_RESOLUTION = Shader.PropertyToID("_LightResolution");

	private readonly int GRID_SIZE = Shader.PropertyToID("_GridSize");

	private readonly int DAMPEN_ATTENUATION = Shader.PropertyToID("_DampenAttenuation");

	private readonly int GLOBAL_LIGHT_ENABLED = Shader.PropertyToID("_GlobalLightEnabled");

	private readonly int GLOBAL_LIGHT_MAP = Shader.PropertyToID("_GlobalLightMap");

	private readonly int GLOBAL_LIGHT_DIRECTION_MAP = Shader.PropertyToID("_GlobalLightDirectionMap");

	private readonly int GLOBAL_STATIC_LIGHT_MAP = Shader.PropertyToID("_GlobalStaticLightMap");

	private readonly int GLOBAL_STATIC_LIGHT_DIRECTION_MAP = Shader.PropertyToID("_GlobalStaticLightDirectionMap");

	private readonly int GLOBAL_LIGHT_MAP_SIZE = Shader.PropertyToID("_GlobalLightMapSize");

	private readonly int GLOBAL_LIGHT_MAP_ORIGIN = Shader.PropertyToID("_GlobalLightMapOrigin");

	private readonly int GLOBAL_LIGHT_RESOLUTION = Shader.PropertyToID("_GlobalLightResolution");

	private readonly int GLOBAL_GRID_SIZE = Shader.PropertyToID("_GlobalGridSize");

	[SerializeField]
	private bool _initializeOnStart;

	[SerializeField]
	[Min(1f)]
	private int _levelWidth = 10;

	[SerializeField]
	[Min(1f)]
	private int _levelLength = 10;

	[SerializeField]
	[Min(1f)]
	private int _levelHeight = 10;

	[SerializeField]
	[Min(0.001f)]
	private int _gridSize = 1;

	[SerializeField]
	[Min(1f)]
	private int _lightingResolution = 4;

	[SerializeField]
	private Vector3 _lightMapOrigin;

	[SerializeField]
	private ComputeShader _lightingCompute;

	[SerializeField]
	private ComputeShader _lightCompositeCompute;

	private RenderTexture _staticLightTexture;

	private RenderTexture _staticLightDirectionTexture;

	private int _staticLightCount;

	private List<CustomLightSource> _staticLights = new List<CustomLightSource>();

	private RenderTexture _dynamicLightTexture;

	private RenderTexture _dynamicLightDirectionTexture;

	private int _dynamicLightCount;

	private List<CustomLightSource> _dynamicLights = new List<CustomLightSource>();

	private Vector4[] _lightPositions;

	private Vector4[] _lightColors;

	private Vector4[] _lightSpotDirs;

	private Vector4[] _lightSpotAngles;

	private Vector4[] _areaLightPositions;

	private Vector4[] _areaLightColors;

	private Vector4[] _areaLightShape;

	private Vector4[] _areaLightData;

	private bool _hasInitialized;

	private bool _customLightEnabled = true;

	private bool _needStaticLightingUpdate;

	private float _staticUpdateTimer;

	private Vector3Int _lightingComputeDispatchCount;

	private const int MAX_LIGHT_COUNT = 256;

	public static CustomLightingManager Instance { get; private set; }

	private void OnEnable()
	{
		if (_initializeOnStart && !_hasInitialized)
		{
			InitializeLighting();
		}
	}

	private void Update()
	{
		CheckStaticUpdateTimer();
		if (_needStaticLightingUpdate)
		{
			UpdateLighting(_staticLights, _staticLightTexture, _staticLightDirectionTexture, dampenAttenuation: false, ref _staticLightCount);
			_needStaticLightingUpdate = false;
		}
		if (_customLightEnabled)
		{
			UpdateLighting(_dynamicLights, _dynamicLightTexture, _dynamicLightDirectionTexture, dampenAttenuation: true, ref _dynamicLightCount);
			CompositeLighting();
		}
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Shader.SetGlobalFloat(GLOBAL_LIGHT_ENABLED, 0f);
			_staticLightTexture.Release();
			_staticLightDirectionTexture.Release();
			_dynamicLightTexture.Release();
			_dynamicLightDirectionTexture.Release();
			Instance = null;
		}
	}

	public void InitializeLighting()
	{
		Vector3Int zero = Vector3Int.zero;
		zero.x = _levelWidth * _lightingResolution;
		zero.y = _levelHeight * _lightingResolution;
		zero.z = _levelLength * _lightingResolution;
		Vector4 vector = new Vector4(zero.x, zero.y, zero.z, 0f);
		RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor
		{
			width = zero.x,
			height = zero.y,
			volumeDepth = zero.z,
			dimension = TextureDimension.Tex3D,
			colorFormat = RenderTextureFormat.ARGBHalf,
			msaaSamples = 1,
			enableRandomWrite = true,
			sRGB = false
		};
		RenderTextureDescriptor desc = renderTextureDescriptor;
		desc.colorFormat = RenderTextureFormat.ARGB32;
		_staticLightTexture = new RenderTexture(renderTextureDescriptor);
		_staticLightDirectionTexture = new RenderTexture(desc);
		_dynamicLightTexture = new RenderTexture(renderTextureDescriptor);
		_dynamicLightDirectionTexture = new RenderTexture(desc);
		_lightPositions = new Vector4[256];
		_lightColors = new Vector4[256];
		_lightSpotDirs = new Vector4[256];
		_lightSpotAngles = new Vector4[256];
		_areaLightPositions = new Vector4[256];
		_areaLightColors = new Vector4[256];
		_areaLightShape = new Vector4[256];
		_areaLightData = new Vector4[256];
		_lightingCompute.GetKernelThreadGroupSizes(0, out var x, out var y, out var z);
		_lightingComputeDispatchCount.x = Mathf.CeilToInt(zero.x / x) + 1;
		_lightingComputeDispatchCount.y = Mathf.CeilToInt(zero.y / y) + 1;
		_lightingComputeDispatchCount.z = Mathf.CeilToInt(zero.z / z) + 1;
		_lightingCompute.SetVector(LIGHT_MAP_SIZE, vector);
		_lightingCompute.SetVector(LIGHT_MAP_ORIGIN, _lightMapOrigin);
		_lightingCompute.SetInt(LIGHT_RESOLUTION, _lightingResolution);
		_lightingCompute.SetInt(GRID_SIZE, _gridSize);
		_lightCompositeCompute.SetTexture(0, STATIC_LIGHT_MAP, _staticLightTexture);
		_lightCompositeCompute.SetTexture(0, STATIC_LIGHT_DIRECTION_MAP, _staticLightDirectionTexture);
		_lightCompositeCompute.SetTexture(0, LIGHT_MAP, _dynamicLightTexture);
		_lightCompositeCompute.SetTexture(0, LIGHT_DIRECTION_MAP, _dynamicLightDirectionTexture);
		_lightCompositeCompute.SetVector(LIGHT_MAP_SIZE, vector);
		Shader.SetGlobalTexture(GLOBAL_LIGHT_MAP, _dynamicLightTexture);
		Shader.SetGlobalTexture(GLOBAL_LIGHT_DIRECTION_MAP, _dynamicLightDirectionTexture);
		Shader.SetGlobalVector(GLOBAL_LIGHT_MAP_SIZE, vector);
		Shader.SetGlobalVector(GLOBAL_LIGHT_MAP_ORIGIN, _lightMapOrigin);
		Shader.SetGlobalInt(GLOBAL_LIGHT_RESOLUTION, _lightingResolution);
		Shader.SetGlobalInt(GLOBAL_GRID_SIZE, _gridSize);
		Shader.SetGlobalFloat(GLOBAL_LIGHT_ENABLED, 1f);
		_staticLights.Clear();
		_dynamicLights.Clear();
		_needStaticLightingUpdate = true;
		_hasInitialized = true;
		CustomLightSource[] array = UnityEngine.Object.FindObjectsOfType<CustomLightSource>();
		foreach (CustomLightSource customLight in array)
		{
			AddLight(customLight);
		}
		Instance = this;
	}

	public void SetStaticUpdateTimer(float time)
	{
		_staticUpdateTimer = Mathf.Max(_staticUpdateTimer, time);
	}

	public void AddLight(CustomLightSource customLight)
	{
		if (_hasInitialized)
		{
			customLight.Light.enabled = !_customLightEnabled;
			if (customLight.IsDynamic)
			{
				_dynamicLights.Add(customLight);
				return;
			}
			_staticLights.Add(customLight);
			RequestStaticLightingUpdate();
		}
	}

	public void RemoveLight(CustomLightSource light)
	{
		if (_hasInitialized)
		{
			light.enabled = true;
			_dynamicLights.Remove(light);
			if (_staticLights.Remove(light))
			{
				RequestStaticLightingUpdate();
			}
		}
	}

	private void RequestStaticLightingUpdate()
	{
		_needStaticLightingUpdate = true;
	}

	private void CheckStaticUpdateTimer()
	{
		if (_staticUpdateTimer >= 0f)
		{
			_needStaticLightingUpdate = true;
		}
		_staticUpdateTimer -= Time.deltaTime;
	}

	private void RandomizeLightColor()
	{
		if (!_hasInitialized)
		{
			return;
		}
		for (int i = 0; i < _staticLights.Count; i++)
		{
			Light light = _staticLights[i].Light;
			if (light.useColorTemperature)
			{
				float f = UnityEngine.Random.Range(0f, 1f);
				light.colorTemperature = Mathf.Lerp(2000f, 10000f, Mathf.Pow(f, 4f));
			}
			else
			{
				light.color = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 0.75f, 1f);
			}
		}
		RequestStaticLightingUpdate();
	}

	private void UpdateLighting(List<CustomLightSource> lights, RenderTexture lightTexture, RenderTexture lightDirectionTexture, bool dampenAttenuation, ref int lightCount)
	{
		if (!_hasInitialized)
		{
			return;
		}
		lightCount = lights.Count;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < lightCount; i++)
		{
			Light light = lights[i].Light;
			if (light.type != LightType.Area)
			{
				if (num < 256)
				{
					_lightPositions[num] = light.transform.position;
					_lightPositions[num].w = light.range;
					if (light.useColorTemperature)
					{
						_lightColors[num] = Mathf.CorrelatedColorTemperatureToRGB(light.colorTemperature) * light.intensity;
					}
					else
					{
						_lightColors[num] = light.color.linear * light.intensity;
					}
					_lightSpotDirs[num] = -light.transform.forward;
					_lightSpotAngles[num] = GetSpotAngle(light);
					num++;
				}
			}
			else if (num2 < 256)
			{
				_areaLightPositions[num2] = light.transform.position;
				if (light.useColorTemperature)
				{
					_areaLightColors[num2] = Mathf.CorrelatedColorTemperatureToRGB(light.colorTemperature) * light.intensity;
				}
				else
				{
					_areaLightColors[num2] = light.color.linear * light.intensity;
				}
				Vector4 zero = Vector4.zero;
				zero.z = light.transform.eulerAngles.y * (MathF.PI / 180f);
				_areaLightShape[num2] = zero;
				Vector4 zero2 = Vector4.zero;
				zero2.x = Mathf.Max(lights[i].AreaRadius, 0f);
				zero2.y = Mathf.Clamp01(lights[i].AreaShadow);
				_areaLightData[num2] = zero2;
				num2++;
			}
		}
		_lightingCompute.SetTexture(0, LIGHT_MAP, lightTexture);
		_lightingCompute.SetTexture(0, LIGHT_DIRECTION_MAP, lightDirectionTexture);
		_lightingCompute.SetVectorArray(LIGHT_POSITIONS, _lightPositions);
		_lightingCompute.SetVectorArray(LIGHT_COLORS, _lightColors);
		_lightingCompute.SetVectorArray(LIGHT_SPOT_DIRS, _lightSpotDirs);
		_lightingCompute.SetVectorArray(LIGHT_SPOT_ANGLES, _lightSpotAngles);
		_lightingCompute.SetVectorArray(AREA_LIGHT_POSITIONS, _areaLightPositions);
		_lightingCompute.SetVectorArray(AREA_LIGHT_COLORS, _areaLightColors);
		_lightingCompute.SetVectorArray(AREA_LIGHT_SHAPE, _areaLightShape);
		_lightingCompute.SetVectorArray(AREA_LIGHT_DATA, _areaLightData);
		_lightingCompute.SetInt(LIGHT_COUNT, num);
		_lightingCompute.SetInt(AREA_LIGHT_COUNT, num2);
		_lightingCompute.SetInt(DAMPEN_ATTENUATION, dampenAttenuation ? 1 : 0);
		_lightingCompute.Dispatch(0, _lightingComputeDispatchCount.x, _lightingComputeDispatchCount.y, _lightingComputeDispatchCount.z);
	}

	private Vector4 GetSpotAngle(Light light)
	{
		Vector4 zero = Vector4.zero;
		if (light.type != LightType.Spot)
		{
			return new Vector4(0f, 1f, 0f, 0f);
		}
		float num = Mathf.Cos(light.innerSpotAngle * 0.5f * (MathF.PI / 180f));
		float num2 = Mathf.Cos(light.spotAngle * 0.5f * (MathF.PI / 180f));
		zero.y = (0f - num2) * (zero.x = 1f / Mathf.Max(num - num2, 1E-05f));
		return zero;
	}

	private void CompositeLighting()
	{
		if (_hasInitialized)
		{
			_lightCompositeCompute.Dispatch(0, _lightingComputeDispatchCount.x, _lightingComputeDispatchCount.y, _lightingComputeDispatchCount.z);
		}
	}

	private void SetCustomLightEnabled(bool value)
	{
		_customLightEnabled = value;
		bool flag = !value;
		foreach (CustomLightSource staticLight in _staticLights)
		{
			staticLight.Light.enabled = flag;
		}
		foreach (CustomLightSource dynamicLight in _dynamicLights)
		{
			dynamicLight.Light.enabled = flag;
		}
		Shader.SetGlobalFloat(GLOBAL_LIGHT_ENABLED, _customLightEnabled ? 1 : 0);
	}

	private void ConvertLightsToCustomLight()
	{
		Light[] array = UnityEngine.Object.FindObjectsOfType<Light>();
		foreach (Light light in array)
		{
			if ((light.type == LightType.Point || light.type == LightType.Spot) && !light.cookie && !(light.gameObject.scene.name == "DontDestroyOnLoad"))
			{
				light.gameObject.AddComponent<CustomLightSource>();
			}
		}
	}

	private void UpdateMaterials()
	{
		MeshRenderer[] array = UnityEngine.Object.FindObjectsOfType<MeshRenderer>();
		Shader shader = Shader.Find("WMO/WMO Lit");
		Shader shader2 = Shader.Find("WMO/WMO Simple Lit");
		MeshRenderer[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Material[] sharedMaterials = array2[i].sharedMaterials;
			foreach (Material material in sharedMaterials)
			{
				if (!(material == null))
				{
					if (material.shader.name == "Universal Render Pipeline/Lit")
					{
						material.shader = shader;
					}
					if (material.shader.name == "Universal Render Pipeline/Simple Lit")
					{
						material.shader = shader2;
					}
				}
			}
		}
	}

	private void ShowCustomLights()
	{
		SetCustomLightEnabled(value: true);
	}

	private void ShowOriginalLights()
	{
		SetCustomLightEnabled(value: false);
	}

	private void OnDrawGizmosSelected()
	{
		Vector3 center = _lightMapOrigin + Vector3.up * ((float)(_levelHeight * _gridSize) * 0.5f);
		Vector3 size = new Vector3(_levelWidth, _levelHeight, _levelLength) * _gridSize;
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(center, size);
	}
}
