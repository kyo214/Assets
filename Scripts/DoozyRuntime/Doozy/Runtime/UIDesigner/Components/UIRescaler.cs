using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIDesigner.Components;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UIRescaler : UIBehaviour
{
	private RectTransform m_RectTransform;

	[SerializeField]
	private Vector2 ReferenceSize;

	[SerializeField]
	private Vector2 TargetSize;

	[SerializeField]
	private bool ContinuousUpdate;

	public RectTransform rectTransform
	{
		get
		{
			if (!m_RectTransform)
			{
				return m_RectTransform = GetComponent<RectTransform>();
			}
			return m_RectTransform;
		}
	}

	public Vector2 referenceSize
	{
		get
		{
			return ReferenceSize;
		}
		set
		{
			ReferenceSize = value;
		}
	}

	public Vector2 targetSize
	{
		get
		{
			return TargetSize;
		}
		set
		{
			TargetSize = value;
		}
	}

	public bool continuousUpdate
	{
		get
		{
			return ContinuousUpdate;
		}
		set
		{
			ContinuousUpdate = value;
		}
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		UpdateScale();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		UpdateScale();
	}

	private void LateUpdate()
	{
		if (ContinuousUpdate)
		{
			UpdateScale();
		}
	}

	public void UpdateScale()
	{
		Vector2 vector = rectTransform.localScale;
		if (ReferenceSize.x <= 0f)
		{
			ReferenceSize.x = 1f;
		}
		if (ReferenceSize.y <= 0f)
		{
			ReferenceSize.y = 1f;
		}
		if (TargetSize.x < 0f)
		{
			TargetSize.x = 0f;
		}
		if (TargetSize.y < 0f)
		{
			TargetSize.y = 0f;
		}
		vector.x = TargetSize.x / ReferenceSize.x;
		vector.y = TargetSize.y / ReferenceSize.y;
		if (float.IsNaN(vector.x))
		{
			vector.x = 1f;
		}
		if (float.IsNaN(vector.y))
		{
			vector.y = 1f;
		}
		if (vector.x < 0f)
		{
			vector.x = 0f;
		}
		if (vector.y < 0f)
		{
			vector.y = 0f;
		}
		rectTransform.localScale = vector;
	}
}
