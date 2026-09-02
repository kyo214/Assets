using System.IO;
using UnityEngine;

namespace Toked;

public static class Texture2DExtensions
{
	public static Texture2D LoadPNG(string filePath)
	{
		Texture2D texture2D = null;
		if (File.Exists(filePath))
		{
			byte[] data = File.ReadAllBytes(filePath);
			texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(data);
		}
		return texture2D;
	}

	public static Texture2D ToTexture2D(this Texture texture)
	{
		Texture2D texture2D = Texture2D.CreateExternalTexture(texture.width, texture.height, TextureFormat.RGB24, mipChain: false, linear: false, texture.GetNativeTexturePtr());
		texture2D.filterMode = FilterMode.Point;
		texture2D.name = texture.name;
		return texture2D;
	}
}
