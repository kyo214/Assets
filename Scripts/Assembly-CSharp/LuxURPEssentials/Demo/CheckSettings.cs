using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LuxURPEssentials.Demo;

public class CheckSettings : MonoBehaviour
{
	private void Start()
	{
		UniversalRenderPipelineAsset obj = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
		if (obj.supportsCameraDepthTexture)
		{
			Debug.Log("CameraDepthTexture supported.");
		}
		else
		{
			Debug.Log("CameraDepthTexture not supported.");
		}
		if (obj.supportsCameraOpaqueTexture)
		{
			Debug.Log("CameraOpaqueTexture supported.");
		}
		else
		{
			Debug.Log("CameraOpaqueTexture not supported.");
		}
	}
}
