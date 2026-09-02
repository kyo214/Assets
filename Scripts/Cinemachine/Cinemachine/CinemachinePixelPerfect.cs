using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Cinemachine;

[AddComponentMenu("")]
[ExecuteAlways]
[DisallowMultipleComponent]
[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@2.9/manual/CinemachinePixelPerfect.html")]
public class CinemachinePixelPerfect : CinemachineExtension
{
	protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
	{
		if (stage != CinemachineCore.Stage.Body)
		{
			return;
		}
		CinemachineBrain cinemachineBrain = CinemachineCore.Instance.FindPotentialTargetBrain(vcam);
		if (!(cinemachineBrain == null) && cinemachineBrain.IsLive(vcam))
		{
			cinemachineBrain.TryGetComponent<PixelPerfectCamera>(out var component);
			if (!(component == null) && component.isActiveAndEnabled)
			{
				LensSettings lens = state.Lens;
				lens.OrthographicSize = component.CorrectCinemachineOrthoSize(lens.OrthographicSize);
				state.Lens = lens;
			}
		}
	}
}
