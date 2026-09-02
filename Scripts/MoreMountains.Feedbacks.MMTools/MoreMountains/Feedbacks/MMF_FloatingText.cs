using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will request the spawn of a floating text, usually to signify damage, but not necessarily. This requires that a MMFloatingTextSpawner be correctly setup in the scene, otherwise nothing will happen. To do so, create a new empty object, add a MMFloatingTextSpawner to it. Drag (at least) one MMFloatingText prefab into its PooledSimpleMMFloatingText slot. You'll find such prefabs already made in the MMTools/Tools/MMFloatingText/Prefabs folder, but feel free to create your own. Using that feedback will always spawn the same text. While this may be what you want, if you're using the Corgi Engine or TopDown Engine, you'll find dedicated versions directly hooked to the Health component, letting you display damage taken.")]
[FeedbackPath("UI/Floating Text")]
public class MMF_FloatingText : MMF_Feedback
{
	public enum PositionModes
	{
		TargetTransform = 0,
		FeedbackPosition = 1,
		PlayPosition = 2
	}

	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Floating Text", true, 64, false, false)]
	[Tooltip("the Intensity to spawn this text with, will act as a lifetime/movement/scale multiplier based on the spawner's settings")]
	public float Intensity = 1f;

	[Tooltip("the value to display when spawning this text")]
	public string Value = "100";

	[Tooltip("if this is true, the intensity passed to this feedback will be the value displayed")]
	public bool UseIntensityAsValue;

	[MMFInspectorGroup("Color", true, 65, false, false)]
	[Tooltip("whether or not to force a color on the new text, if not, the default colors of the spawner will be used")]
	public bool ForceColor;

	[Tooltip("the gradient to apply over the lifetime of the text")]
	[GradientUsage(true)]
	public Gradient AnimateColorGradient = new Gradient();

	[MMFInspectorGroup("Lifetime", true, 66, false, false)]
	[Tooltip("whether or not to force a lifetime on the new text, if not, the default colors of the spawner will be used")]
	public bool ForceLifetime;

	[Tooltip("the forced lifetime for the spawned text")]
	[MMFCondition("ForceLifetime", true)]
	public float Lifetime = 0.5f;

	[MMFInspectorGroup("Position", true, 67, false, false)]
	[Tooltip("where to spawn the new text (at the position of the feedback, or on a specified Transform)")]
	public PositionModes PositionMode = PositionModes.FeedbackPosition;

	[Tooltip("in transform mode, the Transform on which to spawn the new floating text")]
	[MMFEnumCondition("PositionMode", new int[] { 0 })]
	public Transform TargetTransform;

	[Tooltip("the direction to apply to the new floating text (leave it to 0 to let the Spawner decide based on its settings)")]
	public Vector3 Direction = Vector3.zero;

	protected Vector3 _playPosition;

	protected string _value;

	public override float FeedbackDuration
	{
		get
		{
			return ApplyTimeMultiplier(Lifetime);
		}
		set
		{
			Lifetime = value;
		}
	}

	public override bool HasChannel => true;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			float num = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
			switch (PositionMode)
			{
			case PositionModes.FeedbackPosition:
				_playPosition = Owner.transform.position;
				break;
			case PositionModes.PlayPosition:
				_playPosition = position;
				break;
			case PositionModes.TargetTransform:
				_playPosition = TargetTransform.position;
				break;
			}
			_value = (UseIntensityAsValue ? feedbacksIntensity.ToString() : Value);
			MMFloatingTextSpawnEvent.Trigger(Channel, _playPosition, _value, Direction, Intensity * num, ForceLifetime, Lifetime, ForceColor, AnimateColorGradient, Timing.TimescaleMode == TimescaleModes.Unscaled);
		}
	}
}
