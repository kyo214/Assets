using UnityEngine;

namespace DestroyIt;

public static class ShaderExtensions
{
	public static Shader GetTransparentVersion(this Shader currentShader)
	{
		Shader shader = ((!currentShader.name.Contains("DestroyIt/")) ? Shader.Find("DestroyIt/Transparent" + currentShader.name.Replace(" ", "")) : Shader.Find(currentShader.name.Replace("DestroyIt/", "DestroyIt/Transparent")));
		if (shader != null)
		{
			return shader;
		}
		shader = Shader.Find("DestroyIt/TransparentDiffuse");
		if (shader != null)
		{
			return shader;
		}
		Debug.LogError("DestroyIt: No progressive damage transparency shader could be found. Cannot fade out material with shader \"" + currentShader.name + "\" object.");
		return currentShader;
	}
}
