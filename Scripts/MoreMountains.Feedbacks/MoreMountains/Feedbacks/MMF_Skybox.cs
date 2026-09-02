using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you change the scene's skybox on play, replacing it with another one, either a specific one, or one picked at random among multiple skyboxes.")]
[FeedbackPath("Renderer/Skybox")]
public class MMF_Skybox : MMF_Feedback
{
	public enum Modes
	{
		Single = 0,
		Random = 1
	}

	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Skybox", true, 65, false, false)]
	public Modes Mode;

	public Material SingleSkybox;

	public Material[] RandomSkyboxes;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			if (Mode == Modes.Single)
			{
				RenderSettings.skybox = SingleSkybox;
			}
			else if (Mode == Modes.Random)
			{
				RenderSettings.skybox = RandomSkyboxes[Random.Range(0, RandomSkyboxes.Length)];
			}
		}
	}
}
