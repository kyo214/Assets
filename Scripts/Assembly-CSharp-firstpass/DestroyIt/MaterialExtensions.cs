using UnityEngine;

namespace DestroyIt;

public static class MaterialExtensions
{
	public static bool IsProgressiveDamageCapable(this Material mat)
	{
		if (mat == null || mat.shader == null)
		{
			return false;
		}
		if (mat.HasProperty("_DetailMask") && mat.HasProperty("_DetailAlbedoMap") && mat.HasProperty("_DetailNormalMap") && mat.GetTexture("_DetailMask") != null && mat.GetTexture("_DetailAlbedoMap") != null)
		{
			return mat.GetTexture("_DetailNormalMap") != null;
		}
		return false;
	}
}
