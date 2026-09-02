using UnityEngine;

namespace Coffee.UIExtensions;

[AddComponentMenu("UI/Unmask/UnmaskRaycastFilter", 2)]
public class UnmaskRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
{
	[Tooltip("Target unmask component. The ray passes through the unmasked rectangle.")]
	[SerializeField]
	private Unmask m_TargetUnmask;

	public Unmask targetUnmask
	{
		get
		{
			return m_TargetUnmask;
		}
		set
		{
			m_TargetUnmask = value;
		}
	}

	public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
	{
		if (!base.isActiveAndEnabled || !m_TargetUnmask || !m_TargetUnmask.isActiveAndEnabled)
		{
			return true;
		}
		if ((bool)eventCamera)
		{
			return !RectTransformUtility.RectangleContainsScreenPoint(m_TargetUnmask.transform as RectTransform, sp, eventCamera);
		}
		return !RectTransformUtility.RectangleContainsScreenPoint(m_TargetUnmask.transform as RectTransform, sp);
	}

	private void OnEnable()
	{
	}
}
