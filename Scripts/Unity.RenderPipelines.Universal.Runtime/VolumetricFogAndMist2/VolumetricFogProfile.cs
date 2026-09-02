using System;
using System.Collections.Generic;
using UnityEngine;

namespace VolumetricFogAndMist2;

[CreateAssetMenu(menuName = "Volumetric Fog \u008b& Mist/Fog Profile", fileName = "VolumetricFogProfile", order = 1001)]
public class VolumetricFogProfile : ScriptableObject
{
	[Header("Rendering")]
	[Range(1f, 16f)]
	public int raymarchQuality = 6;

	[Tooltip("Determines the minimum step size. Increase to improve performance / decrease to improve accuracy. When increasing this value, you can also increase 'Jittering' amount to improve quality.")]
	public float raymarchMinStep = 0.1f;

	public float jittering = 0.5f;

	[Range(0f, 2f)]
	public float dithering = 1f;

	[Tooltip("The render queue for this renderer. By default, all transparent objects use a render queue of 3000. Use a lower value to render before all transparent objects.")]
	public int renderQueue = 3100;

	[Tooltip("Optional sorting layer Id (number) for this renderer. By default 0. Usually used to control the order with other transparent renderers, like Sprite Renderer.")]
	public int sortingLayerID;

	[Tooltip("Optional sorting order for this renderer. Used to control the order with other transparent renderers, like Sprite Renderer.")]
	public int sortingOrder;

	[Header("Density")]
	[Tooltip("Do not use any noise at all")]
	public bool constantDensity;

	public Texture2D noiseTexture;

	[Range(0f, 3f)]
	public float noiseStrength = 1f;

	public float noiseScale = 15f;

	public float noiseFinalMultiplier = 1f;

	public bool useDetailNoise;

	public Texture3D detailTexture;

	public float detailScale = 0.35f;

	[Range(0f, 1f)]
	public float detailStrength = 0.5f;

	public float detailOffset = -0.5f;

	public float density = 1f;

	[Header("Geometry")]
	public VolumetricFogShape shape;

	[Range(0f, 1f)]
	public float border = 0.05f;

	[Tooltip("Ignores volume height and use a custom height defined by this profile")]
	public bool customHeight;

	public float height;

	public float verticalOffset;

	[Tooltip("When enabled, makes fog appear at certain distance from a camera")]
	public float distance;

	[Range(0f, 1f)]
	public float distanceFallOff = 0.93f;

	[Tooltip("Maximum distance from camera")]
	public float maxDistance = 10000f;

	[Range(0f, 1f)]
	public float maxDistanceFallOff;

	[Tooltip("Fits the fog altitude to the terrain heightmap")]
	public bool terrainFit;

	public VolumetricFog.HeightmapCaptureResolution terrainFitResolution = VolumetricFog.HeightmapCaptureResolution._128;

	[Tooltip("Which objects will be included in the heightmap capture. By default all objects are included but you may want to restrict this to just the terrain.")]
	public LayerMask terrainLayerMask = -1;

	[Tooltip("The height of fog above terrain surface.")]
	public float terrainFogHeight = 25f;

	public float terrainFogMinAltitude;

	public float terrainFogMaxAltitude = 150f;

	[Header("Colors")]
	[ColorUsage(false)]
	public Color albedo = new Color32(227, 227, 227, byte.MaxValue);

	public bool enableDepthGradient;

	[GradientUsage(true, ColorSpace.Linear)]
	public Gradient depthGradient;

	public float depthGradientMaxDistance = 1000f;

	public bool enableHeightGradient;

	[GradientUsage(true, ColorSpace.Linear)]
	public Gradient heightGradient;

	public float brightness = 1f;

	[Range(0f, 2f)]
	public float deepObscurance = 1f;

	public Color specularColor = new Color(1f, 1f, 0.8f, 1f);

	[Range(0f, 1f)]
	public float specularThreshold = 0.637f;

	[Range(0f, 1f)]
	public float specularIntensity = 0.428f;

	[Header("Animation")]
	public float turbulence = 0.73f;

	public Vector3 windDirection = new Vector3(0.02f, 0f, 0f);

	public bool useCustomDetailNoiseWindDirection;

	public Vector3 detailNoiseWindDirection = new Vector3(0.02f, 0f, 0f);

	[Header("Directional Light")]
	[Tooltip("Enable to synchronize fog light intensity and color with the Sun and the Moon (must be assigned into Volumetric Fog Manager)")]
	public bool dayNightCycle = true;

	[Tooltip("When day/night cycle option is disabled, customize the direction of the Sun light.")]
	public Vector3 sunDirection = Vector3.up;

	[Tooltip("Custom sun color when day/night cycle is disabled")]
	public Color sunColor = new Color(0f, 0.9568f, 0.8392f);

	[Tooltip("Custom sun intensity when day/night cycle is disabled")]
	public float sunIntensity = 1f;

	[Tooltip("Ambient light influence")]
	public float ambientLightMultiplier;

	[Range(0f, 64f)]
	public float lightDiffusionPower = 32f;

	[Range(0f, 1f)]
	public float lightDiffusionIntensity = 0.4f;

	public bool receiveShadows;

	[Range(0f, 1f)]
	public float shadowIntensity = 0.5f;

	[Tooltip("Removes shadowed fog")]
	[Range(0f, 1f)]
	public float shadowCancellation;

	public float shadowMaxDistance = 250f;

	[Tooltip("Uses the directional light cookie")]
	public bool cookie;

	[Header("Distant Fog")]
	[Tooltip("Enables exponential distant fog. Use this option to cover horizon/sky/far distances with optimal performance")]
	public bool distantFog;

	public float distantFogStartDistance = 1000f;

	public float distantFogDistanceDensity = 0.5f;

	public float distantFogMaxHeight = 4000f;

	public float distantFogHeightDensity = 0.5f;

	public Color distantFogColor = new Color(0.358f, 0.358f, 0.358f);

	public float distantFogDiffusionIntensity = 0.4f;

	public int distantFogRenderQueue = 2999;

	[NonSerialized]
	public Texture2D depthGradientTex;

	[NonSerialized]
	public Texture2D heightGradientTex;

	private Color[] depthGradientColors;

	private Color[] heightGradientColors;

	private static readonly List<float> colorKeysTimes = new List<float>();

	private static readonly List<float> alphaKeysTimes = new List<float>();

	public event OnSettingsChanged onSettingsChanged;

	private void OnEnable()
	{
		if (noiseTexture == null)
		{
			noiseTexture = Resources.Load<Texture2D>("Textures/NoiseTex256");
		}
		if (detailTexture == null)
		{
			detailTexture = Resources.Load<Texture3D>("Textures/NoiseTex3D");
		}
		ValidateSettings();
	}

	private void OnValidate()
	{
		ValidateSettings();
		if (onSettingsChanged != null)
		{
			onSettingsChanged();
		}
	}

	public void ValidateSettings()
	{
		distance = Mathf.Max(0f, distance);
		density = Mathf.Max(0f, density);
		noiseScale = Mathf.Max(0.1f, noiseScale);
		noiseFinalMultiplier = Mathf.Max(0f, noiseFinalMultiplier);
		detailScale = Mathf.Max(0.01f, detailScale);
		raymarchMinStep = Mathf.Max(0.1f, raymarchMinStep);
		jittering = Mathf.Max(0f, jittering);
		terrainFogHeight = Mathf.Max(0f, terrainFogHeight);
		height = Mathf.Max(0f, height);
		if (depthGradient == null)
		{
			depthGradient = new Gradient();
			depthGradient.colorKeys = new GradientColorKey[2]
			{
				new GradientColorKey(Color.white, 0f),
				new GradientColorKey(Color.white, 1f)
			};
		}
		maxDistance = Mathf.Max(0.0001f, maxDistance);
		depthGradientMaxDistance = Mathf.Max(0f, depthGradientMaxDistance);
		ambientLightMultiplier = Mathf.Max(0f, ambientLightMultiplier);
		sunIntensity = Mathf.Max(0f, sunIntensity);
		shadowMaxDistance = Mathf.Max(0f, shadowMaxDistance);
		distantFogStartDistance = Mathf.Max(0f, distantFogStartDistance);
		distantFogDistanceDensity = Mathf.Max(0f, distantFogDistanceDensity);
		distantFogMaxHeight = Mathf.Max(0f, distantFogMaxHeight);
		distantFogHeightDensity = Mathf.Max(0f, distantFogHeightDensity);
		distantFogDiffusionIntensity = Mathf.Max(0f, distantFogDiffusionIntensity);
		if (enableDepthGradient)
		{
			bool flag = false;
			if (depthGradientTex == null)
			{
				depthGradientTex = new Texture2D(32, 1, TextureFormat.RGBA32, mipChain: false, linear: true);
				depthGradientTex.wrapMode = TextureWrapMode.Clamp;
				flag = true;
			}
			if (depthGradientColors == null || depthGradientColors.Length != 32)
			{
				depthGradientColors = new Color[32];
				flag = true;
			}
			for (int i = 0; i < 32; i++)
			{
				float time = (float)i / 32f;
				Color color = depthGradient.Evaluate(time);
				if (color != depthGradientColors[i])
				{
					depthGradientColors[i] = color;
					flag = true;
				}
			}
			if (flag)
			{
				depthGradientTex.SetPixels(depthGradientColors);
				depthGradientTex.Apply();
			}
		}
		if (!enableHeightGradient)
		{
			return;
		}
		bool flag2 = false;
		if (heightGradientTex == null)
		{
			heightGradientTex = new Texture2D(32, 1, TextureFormat.RGBA32, mipChain: false, linear: true);
			heightGradientTex.wrapMode = TextureWrapMode.Clamp;
			flag2 = true;
		}
		if (heightGradientColors == null || heightGradientColors.Length != 32)
		{
			heightGradientColors = new Color[32];
			flag2 = true;
		}
		for (int j = 0; j < 32; j++)
		{
			float time2 = (float)j / 32f;
			Color color2 = heightGradient.Evaluate(time2);
			if (color2 != heightGradientColors[j])
			{
				heightGradientColors[j] = color2;
				flag2 = true;
			}
		}
		if (flag2)
		{
			heightGradientTex.SetPixels(heightGradientColors);
			heightGradientTex.Apply();
		}
	}

	public void Lerp(VolumetricFogProfile p1, VolumetricFogProfile p2, float t)
	{
		float num = 1f - t;
		raymarchQuality = (int)((float)p1.raymarchQuality * num + (float)p2.raymarchQuality * t);
		raymarchMinStep = p1.raymarchMinStep * num + p2.raymarchMinStep * t;
		jittering = p1.jittering * num + p2.jittering * t;
		dithering = p1.dithering * num + p2.dithering * t;
		renderQueue = ((t < 0.5f) ? p1.renderQueue : p2.renderQueue);
		sortingLayerID = ((t < 0.5f) ? p1.sortingLayerID : p2.sortingLayerID);
		sortingOrder = ((t < 0.5f) ? p1.sortingOrder : p2.sortingOrder);
		noiseStrength = p1.noiseStrength * num + p2.noiseStrength * t;
		noiseScale = p1.noiseScale * num + p2.noiseScale * t;
		noiseFinalMultiplier = p1.noiseFinalMultiplier * num + p2.noiseFinalMultiplier * t;
		noiseTexture = ((t < 0.5f) ? p1.noiseTexture : p2.noiseTexture);
		useDetailNoise = ((t < 0.5f) ? p1.useDetailNoise : p2.useDetailNoise);
		detailTexture = ((t < 0.5f) ? p1.detailTexture : p2.detailTexture);
		detailScale = p1.detailScale * num + p2.detailScale * t;
		detailStrength = p1.detailStrength * num + p2.detailStrength * t;
		detailOffset = p1.detailOffset * num + p2.detailOffset * t;
		density = p1.density * num + p2.density * t;
		shape = ((t < 0.5f) ? p1.shape : p2.shape);
		border = p1.border * num + p2.border * t;
		verticalOffset = p1.verticalOffset * num + p2.verticalOffset * t;
		distance = p1.distance * num + p2.distance * t;
		distanceFallOff = p1.distanceFallOff * num + p2.distanceFallOff * t;
		albedo = p1.albedo * num + p2.albedo * t;
		constantDensity = ((t < 0.5f) ? p1.constantDensity : p2.constantDensity);
		enableDepthGradient = p1.enableDepthGradient || p2.enableDepthGradient;
		LerpGradient(depthGradient, p1.depthGradient, p2.depthGradient, t);
		depthGradientMaxDistance = p1.depthGradientMaxDistance * num + p2.depthGradientMaxDistance * t;
		enableHeightGradient = p1.enableHeightGradient || p2.enableHeightGradient;
		LerpGradient(heightGradient, p1.heightGradient, p2.heightGradient, t);
		ambientLightMultiplier = p1.ambientLightMultiplier * num + p2.ambientLightMultiplier * t;
		brightness = p1.brightness * num + p2.brightness * t;
		deepObscurance = p1.deepObscurance * num + p2.deepObscurance * t;
		specularColor = p1.specularColor * num + p2.specularColor * t;
		specularThreshold = p1.specularThreshold * num + p2.specularThreshold * t;
		specularIntensity = p1.specularIntensity * num + p2.specularIntensity * t;
		turbulence = p1.turbulence * num + p2.turbulence * t;
		windDirection = p1.windDirection * num + p2.windDirection * t;
		useCustomDetailNoiseWindDirection = ((t < 0.5f) ? p1.useCustomDetailNoiseWindDirection : p2.useCustomDetailNoiseWindDirection);
		detailNoiseWindDirection = p1.detailNoiseWindDirection * num + p2.detailNoiseWindDirection * t;
		lightDiffusionPower = p1.lightDiffusionPower * num + p2.lightDiffusionPower * t;
		lightDiffusionIntensity = p1.lightDiffusionIntensity * num + p2.lightDiffusionIntensity * t;
		receiveShadows = ((t < 0.5f) ? p1.receiveShadows : p2.receiveShadows);
		shadowIntensity = p1.shadowIntensity * num + p2.shadowIntensity * t;
		shadowCancellation = ((t < 0.5f) ? p1.shadowCancellation : p2.shadowCancellation);
		shadowMaxDistance = p1.shadowMaxDistance * num + p2.shadowMaxDistance * t;
		terrainFit = ((t < 0.5f) ? p1.terrainFit : p2.terrainFit);
		terrainFitResolution = (((double)t < 0.5) ? p1.terrainFitResolution : p2.terrainFitResolution);
		terrainFogHeight = p1.terrainFogHeight * num + p2.terrainFogHeight * t;
		terrainFogMinAltitude = p1.terrainFogMinAltitude * num + p2.terrainFogMinAltitude * t;
		terrainFogMaxAltitude = p1.terrainFogMaxAltitude * num + p2.terrainFogMaxAltitude * t;
		terrainLayerMask = ((t < 0.5f) ? p1.terrainLayerMask : p2.terrainLayerMask);
		dayNightCycle = ((t < 0.5f) ? p1.dayNightCycle : p2.dayNightCycle);
		sunDirection = Vector3.Slerp(p1.sunDirection, p2.sunDirection, t);
		sunColor = p1.sunColor * num + p2.sunColor * t;
		sunIntensity = p1.sunIntensity * num + p2.sunIntensity * t;
		ambientLightMultiplier = p1.ambientLightMultiplier * num + p2.ambientLightMultiplier * t;
		cookie = ((t < 0.5f) ? p1.cookie : p2.cookie);
		distantFog = ((t < 0.5f) ? p1.distantFog : p2.distantFog);
		distantFogStartDistance = p1.distantFogStartDistance * num + p2.distantFogStartDistance * t;
		distantFogDistanceDensity = p1.distantFogDistanceDensity * num + p2.distantFogDistanceDensity * t;
		distantFogMaxHeight = p1.distantFogMaxHeight * num + p2.distantFogMaxHeight * t;
		distantFogHeightDensity = p1.distantFogHeightDensity * num + p2.distantFogHeightDensity * t;
		distantFogColor = p1.distantFogColor * num + p2.distantFogColor * t;
		distantFogDiffusionIntensity = p1.distantFogDiffusionIntensity * num + p2.distantFogDiffusionIntensity * t;
		ValidateSettings();
	}

	private void LerpGradient(Gradient g, Gradient a, Gradient b, float t)
	{
		if (a.colorKeys.Length + b.colorKeys.Length > 8 || a.alphaKeys.Length + b.alphaKeys.Length > 8)
		{
			Debug.LogError("Gradients total key count exceeding 8, can not lerp");
			return;
		}
		colorKeysTimes.Clear();
		if (a.colorKeys != null)
		{
			for (int i = 0; i < a.colorKeys.Length; i++)
			{
				float time = a.colorKeys[i].time;
				if (!colorKeysTimes.Contains(time))
				{
					colorKeysTimes.Add(time);
				}
			}
		}
		if (b.colorKeys != null)
		{
			for (int j = 0; j < b.colorKeys.Length; j++)
			{
				float time2 = b.colorKeys[j].time;
				if (!colorKeysTimes.Contains(time2))
				{
					colorKeysTimes.Add(time2);
				}
			}
		}
		alphaKeysTimes.Clear();
		if (a.alphaKeys != null)
		{
			for (int k = 0; k < a.alphaKeys.Length; k++)
			{
				float time3 = a.alphaKeys[k].time;
				if (!alphaKeysTimes.Contains(time3))
				{
					alphaKeysTimes.Add(time3);
				}
			}
		}
		if (b.alphaKeys != null)
		{
			for (int l = 0; l < b.alphaKeys.Length; l++)
			{
				float time4 = b.alphaKeys[l].time;
				if (!alphaKeysTimes.Contains(time4))
				{
					alphaKeysTimes.Add(time4);
				}
			}
		}
		int count = colorKeysTimes.Count;
		GradientColorKey[] array = g.colorKeys;
		if (array == null || array.Length != count)
		{
			array = new GradientColorKey[count];
		}
		for (int m = 0; m < count; m++)
		{
			float time5 = colorKeysTimes[m];
			Color col = Color.Lerp(a.Evaluate(time5), b.Evaluate(time5), t);
			array[m] = new GradientColorKey(col, time5);
		}
		int count2 = alphaKeysTimes.Count;
		GradientAlphaKey[] array2 = g.alphaKeys;
		if (array2 == null || array2.Length != count2)
		{
			array2 = new GradientAlphaKey[count2];
		}
		for (int n = 0; n < count2; n++)
		{
			float time6 = alphaKeysTimes[n];
			array2[n] = new GradientAlphaKey(Color.Lerp(a.Evaluate(time6), b.Evaluate(time6), t).a, time6);
		}
		g.SetKeys(array, array2);
	}
}
