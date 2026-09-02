using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MoreMountains.FeedbacksForThirdParty;

[RequireComponent(typeof(Volume))]
[AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMAutoFocus_URP")]
public class MMAutoFocus_URP : MonoBehaviour
{
	[Header("Bindings")]
	[Tooltip("the position of the camera")]
	public Transform CameraTransform;

	[Tooltip("a list of all possible targets")]
	public Transform[] FocusTargets;

	[Header("Setup")]
	[Tooltip("the current target of this auto focus")]
	public float FocusTargetID;

	[Header("Desired Aperture")]
	[Tooltip("the aperture to work with")]
	[Range(0.1f, 20f)]
	public float Aperture = 0.1f;

	protected Volume _volume;

	protected VolumeProfile _profile;

	protected DepthOfField _depthOfField;

	private void Start()
	{
		_volume = GetComponent<Volume>();
		_profile = _volume.profile;
		_profile.TryGet<DepthOfField>(out _depthOfField);
	}

	private void Update()
	{
		float x = Vector3.Distance(CameraTransform.position, FocusTargets[Mathf.FloorToInt(FocusTargetID)].position);
		_depthOfField.focusDistance.Override(x);
		_depthOfField.aperture.Override(Aperture);
	}
}
