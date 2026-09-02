using System;
using System.Collections.Generic;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Animators;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Layouts.Internal;
using UnityEngine;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Layouts;

[AddComponentMenu("UI/Layouts/UI RadialLayout")]
[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
[ExecuteAlways]
public class UIRadialLayout : UILayoutGroup
{
	public const bool k_AutoRebuildDefaultValue = true;

	public const bool k_ClockwiseDefaultValue = true;

	public const bool k_ControlChildHeightDefaultValue = false;

	public const bool k_ControlChildWidthDefaultValue = false;

	public const bool k_RadiusControlsHeightDefaultValue = false;

	public const bool k_RadiusControlsWidthDefaultValue = false;

	public const bool k_RotateChildrenDefaultValue = false;

	public const float k_ChildHeightDefaultValue = 100f;

	public const float k_ChildRotationDefaultValue = 0f;

	public const float k_ChildWidthDefaultValue = 100f;

	public const float k_MAXAngle = 360f;

	public const float k_MAXAngleDefaultValue = 360f;

	public const float k_MAXRadiusDefaultValue = 1000f;

	public const float k_MINAngle = 0f;

	public const float k_MINAngleDefaultValue = 0f;

	public const float k_RadiusDefaultValue = 100f;

	public const float k_RadiusHeightFactorDefaultValue = 1f;

	public const float k_RadiusWidthFactorDefaultValue = 1f;

	public const float k_SpacingDefaultValue = 0f;

	public const float k_StartAngleDefaultValue = 180f;

	[SerializeField]
	protected bool AutoRebuild = true;

	[SerializeField]
	protected float ChildHeight = 100f;

	[SerializeField]
	protected float ChildRotation;

	[SerializeField]
	protected float ChildWidth = 100f;

	[SerializeField]
	protected bool Clockwise = true;

	[SerializeField]
	protected bool ControlChildHeight;

	[SerializeField]
	protected bool ControlChildWidth;

	[Range(0f, 360f)]
	[SerializeField]
	protected float MaxAngle = 360f;

	[SerializeField]
	protected float MaxRadius = 1000f;

	[Range(0f, 360f)]
	[SerializeField]
	protected float MinAngle;

	[SerializeField]
	protected float Radius = 100f;

	[SerializeField]
	protected bool RadiusControlsHeight;

	[SerializeField]
	protected bool RadiusControlsWidth;

	[SerializeField]
	protected float RadiusHeightFactor = 1f;

	[SerializeField]
	protected float RadiusWidthFactor = 1f;

	[SerializeField]
	protected bool RotateChildren;

	[SerializeField]
	protected float Spacing;

	[Range(0f, 360f)]
	[SerializeField]
	protected float StartAngle = 180f;

	private List<RectTransform> m_ChildList = new List<RectTransform>();

	public bool autoRebuild
	{
		get
		{
			return AutoRebuild;
		}
		set
		{
			if (AutoRebuild != value)
			{
				AutoRebuild = value;
				OnValueChanged();
			}
		}
	}

	public float childHeight
	{
		get
		{
			return ChildHeight;
		}
		set
		{
			if (!Mathf.Approximately(ChildHeight, value))
			{
				ChildHeight = value;
				OnValueChanged();
			}
		}
	}

	public float childRotation
	{
		get
		{
			return ChildRotation;
		}
		set
		{
			if (!Mathf.Approximately(ChildRotation, value))
			{
				ChildRotation = value;
				OnValueChanged();
			}
		}
	}

	public float childWidth
	{
		get
		{
			return ChildWidth;
		}
		set
		{
			if (!Mathf.Approximately(ChildWidth, value))
			{
				ChildWidth = value;
				OnValueChanged();
			}
		}
	}

	public bool clockwise
	{
		get
		{
			return Clockwise;
		}
		set
		{
			if (Clockwise != value)
			{
				Clockwise = value;
				OnValueChanged();
			}
		}
	}

	public bool controlChildHeight
	{
		get
		{
			return ControlChildHeight;
		}
		set
		{
			ControlChildHeight = value;
			OnValueChanged();
		}
	}

	public bool controlChildWidth
	{
		get
		{
			return ControlChildWidth;
		}
		set
		{
			ControlChildWidth = value;
			OnValueChanged();
		}
	}

	public float maxAngle
	{
		get
		{
			return MaxAngle;
		}
		set
		{
			if (!Mathf.Approximately(MaxAngle, value))
			{
				MaxAngle = value;
				OnValueChanged();
			}
		}
	}

	public float minAngle
	{
		get
		{
			return MinAngle;
		}
		set
		{
			if (!Mathf.Approximately(MinAngle, value))
			{
				MinAngle = value;
				OnValueChanged();
			}
		}
	}

	public float radius
	{
		get
		{
			return Radius;
		}
		set
		{
			if (!Mathf.Approximately(Radius, value))
			{
				Radius = value;
				OnValueChanged();
			}
		}
	}

	public bool radiusControlsHeight
	{
		get
		{
			return RadiusControlsHeight;
		}
		set
		{
			RadiusControlsHeight = value;
			OnValueChanged();
		}
	}

	public bool radiusControlsWidth
	{
		get
		{
			return RadiusControlsWidth;
		}
		set
		{
			RadiusControlsWidth = value;
			OnValueChanged();
		}
	}

	public float radiusHeightFactor
	{
		get
		{
			return RadiusHeightFactor;
		}
		set
		{
			if (!Mathf.Approximately(RadiusHeightFactor, value))
			{
				RadiusHeightFactor = value;
				OnValueChanged();
			}
		}
	}

	public float radiusWidthFactor
	{
		get
		{
			return RadiusWidthFactor;
		}
		set
		{
			if (!Mathf.Approximately(RadiusWidthFactor, value))
			{
				RadiusWidthFactor = value;
				OnValueChanged();
			}
		}
	}

	public bool rotateChildren
	{
		get
		{
			return RotateChildren;
		}
		set
		{
			RotateChildren = value;
			OnValueChanged();
		}
	}

	public float spacing
	{
		get
		{
			return Spacing;
		}
		set
		{
			if (!Mathf.Approximately(Spacing, value))
			{
				Spacing = value;
				OnValueChanged();
			}
		}
	}

	public float startAngle
	{
		get
		{
			return StartAngle;
		}
		set
		{
			if (!Mathf.Approximately(StartAngle, value))
			{
				StartAngle = value;
				OnValueChanged();
			}
		}
	}

	private bool runUpdateAnimatorsStartPosition { get; set; }

	protected override void OnEnable()
	{
		if (Application.isPlaying)
		{
			runUpdateAnimatorsStartPosition = false;
			CalculateRadial();
		}
	}

	public override void SetLayoutHorizontal()
	{
	}

	public override void SetLayoutVertical()
	{
	}

	public override void CalculateLayoutInputVertical()
	{
		CalculateRadial();
	}

	public override void CalculateLayoutInputHorizontal()
	{
		CalculateRadial();
	}

	public void CalculateRadial()
	{
		if (m_ChildList == null)
		{
			m_ChildList = new List<RectTransform>();
		}
		m_ChildList.Clear();
		int num = 0;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			RectTransform rectTransform = base.transform.GetChild(i) as RectTransform;
			if (!(rectTransform == null))
			{
				LayoutElement component = rectTransform.GetComponent<LayoutElement>();
				if (!(rectTransform == null) && rectTransform.gameObject.activeSelf && (!(component != null) || !component.ignoreLayout))
				{
					m_ChildList.Add(rectTransform);
					num++;
				}
			}
		}
		m_Tracker.Clear();
		if (num == 0)
		{
			return;
		}
		if (Application.isPlaying & !runUpdateAnimatorsStartPosition)
		{
			runUpdateAnimatorsStartPosition = true;
			UpdateAnimatorsStartValues();
		}
		base.rectTransform.sizeDelta = new Vector2(Radius, Radius) * 2f;
		float num2 = 360f / (float)num * ((float)num - 1f);
		float num3 = MinAngle;
		if (num3 > num2)
		{
			num3 = num2;
		}
		float num4 = 360f - MaxAngle;
		if (num4 > num2)
		{
			num4 = num2;
		}
		if (num3 > num2)
		{
			num3 = num2;
		}
		float num5 = (num2 - num3 - num4) / ((float)num - 1f) + Spacing;
		float num6 = StartAngle + num3;
		bool flag = ControlChildWidth | ControlChildHeight;
		DrivenTransformProperties drivenTransformProperties = DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.Pivot;
		if (ControlChildWidth)
		{
			drivenTransformProperties |= DrivenTransformProperties.SizeDeltaX;
		}
		if (ControlChildHeight)
		{
			drivenTransformProperties |= DrivenTransformProperties.SizeDeltaY;
		}
		if (RotateChildren)
		{
			drivenTransformProperties |= DrivenTransformProperties.Rotation;
		}
		if (Clockwise)
		{
			num5 *= -1f;
		}
		foreach (RectTransform child in m_ChildList)
		{
			if (child == null || !child.gameObject.activeSelf)
			{
				continue;
			}
			m_Tracker.Add(this, child, drivenTransformProperties);
			Vector3 vector = new Vector3(Mathf.Cos(num6 * (MathF.PI / 180f)), Mathf.Sin(num6 * (MathF.PI / 180f)), 0f);
			child.localPosition = vector * Radius;
			Vector2 vector2 = (child.pivot = new Vector2(0.5f, 0.5f));
			Vector2 anchorMin = (child.anchorMax = vector2);
			child.anchorMin = anchorMin;
			float num7 = ChildRotation;
			if (RotateChildren)
			{
				num7 += num6;
			}
			child.localEulerAngles = new Vector3(0f, 0f, num7);
			if (flag)
			{
				Vector2 sizeDelta = child.sizeDelta;
				if (controlChildWidth)
				{
					sizeDelta.x = (RadiusControlsWidth ? (ChildWidth * Radius * RadiusWidthFactor / 100f) : ChildWidth);
				}
				if (controlChildHeight)
				{
					sizeDelta.y = (RadiusControlsHeight ? (ChildHeight * Radius * RadiusHeightFactor / 100f) : ChildHeight);
				}
				child.sizeDelta = sizeDelta;
			}
			num6 += num5;
		}
	}

	private void UpdateAnimatorsStartValues()
	{
		LayoutRebuilder.MarkLayoutForRebuild(base.rectTransform);
		for (int i = 0; i < base.transform.childCount; i++)
		{
			RectTransform rectTransform = base.transform.GetChild(i) as RectTransform;
			if (rectTransform == null)
			{
				continue;
			}
			UIAnimator component = rectTransform.GetComponent<UIAnimator>();
			if (component != null)
			{
				component.animation.startPosition = component.rectTransform.anchoredPosition3D;
				component.animation.startRotation = component.rectTransform.localEulerAngles;
				if (component.animation.isPlaying)
				{
					component.UpdateValues();
				}
			}
			UIContainerUIAnimator component2 = rectTransform.GetComponent<UIContainerUIAnimator>();
			if (component2 != null && component2.isConnected && component2.controller.isVisible)
			{
				component2.showAnimation.startPosition = component2.rectTransform.anchoredPosition3D;
				component2.showAnimation.startRotation = component2.rectTransform.localEulerAngles;
			}
			UISelectableUIAnimator component3 = rectTransform.GetComponent<UISelectableUIAnimator>();
			if (!(component3 != null) || !component3.isConnected || !((component3.controller.currentUISelectionState == UISelectionState.Normal) & !component3.anyAnimationIsActive))
			{
				continue;
			}
			foreach (UISelectionState uiSelectionState in UISelectable.uiSelectionStates)
			{
				UIAnimation animation = component3.GetAnimation(uiSelectionState);
				if (animation != null)
				{
					animation.startPosition = animation.rectTransform.anchoredPosition3D;
					animation.startRotation = animation.rectTransform.localEulerAngles;
				}
			}
		}
		runUpdateAnimatorsStartPosition = false;
	}

	private void OnValueChanged()
	{
		if (AutoRebuild)
		{
			CalculateRadial();
		}
	}
}
