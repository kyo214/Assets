using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Procedural Texture 2D", menuName = "", order = 1)]
public class ProceduralTexture2D : ScriptableObject
{
	public enum TextureType
	{
		Color = 0,
		Normal = 1,
		Other = 2
	}

	public enum CompressionLevel
	{
		None = -1,
		LowQuality = 0,
		NormalQuality = 50,
		HighQuality = 100
	}

	public Texture2D input;

	public TextureType type;

	public bool includeAlpha;

	public bool generateMipMaps = true;

	public FilterMode filterMode = FilterMode.Trilinear;

	public int anisoLevel = 16;

	public CompressionLevel compressionQuality = CompressionLevel.HighQuality;

	public Texture2D Tinput;

	public Texture2D invT;

	public Vector3 colorSpaceOrigin = Vector3.zero;

	public Vector3 colorSpaceVector1 = Vector3.zero;

	public Vector3 colorSpaceVector2 = Vector3.zero;

	public Vector3 colorSpaceVector3 = Vector3.zero;

	public Vector4 compressionScalers = Vector4.zero;

	public long memoryUsageBytes;

	public Texture2D currentInput;

	public TextureType currentType;

	public bool currentIncludeAlpha;

	public bool currentGenerateMipMaps;

	public FilterMode currentFilterMode;

	public int currentAnisoLevel;

	public CompressionLevel currentCompressionQuality;
}
