using UnityEngine;

namespace LuxURPEssentials;

[ExecuteAlways]
public class LuxURP_SetupGlobals : MonoBehaviour
{
	public Texture2D _BestFittingNormal;

	private void SetupGlobals()
	{
		if (_BestFittingNormal != null)
		{
			Shader.SetGlobalTexture("_BestFittingNormal", _BestFittingNormal);
		}
	}

	private void OnEnable()
	{
		SetupGlobals();
	}

	private void OnValidate()
	{
		SetupGlobals();
	}
}
