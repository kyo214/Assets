using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback allows you to destroy a target gameobject, either via Destroy, DestroyImmediate, or SetActive:False")]
[FeedbackPath("GameObject/Destroy")]
public class MMFeedbackDestroy : MMFeedback
{
	public enum Modes
	{
		Destroy = 0,
		DestroyImmediate = 1,
		Disable = 2
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Destroy")]
	[Tooltip("the gameobject we want to change the active state of")]
	public GameObject TargetGameObject;

	[Tooltip("the selected destruction mode")]
	public Modes Mode;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && !(TargetGameObject == null))
		{
			ProceedWithDestruction(TargetGameObject);
		}
	}

	protected virtual void ProceedWithDestruction(GameObject go)
	{
		switch (Mode)
		{
		case Modes.Destroy:
			Object.Destroy(go);
			break;
		case Modes.DestroyImmediate:
			Object.DestroyImmediate(go);
			break;
		case Modes.Disable:
			go.SetActive(value: false);
			break;
		}
	}
}
