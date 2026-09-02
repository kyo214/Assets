using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Doozy.Runtime.Common.Layouts;

[AddComponentMenu("UI/Layouts/UIBehaviour Handler")]
[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UIBehaviourHandler : UIBehaviour
{
	private RectTransform m_RectTransform;

	private LayoutGroup m_LayoutGroup;

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

	public LayoutGroup layoutGroup
	{
		get
		{
			if (!m_LayoutGroup)
			{
				return m_LayoutGroup = GetComponent<LayoutGroup>();
			}
			return m_LayoutGroup;
		}
	}

	public UnityAction onRectTransformDimensionsChanged { get; set; }

	private int lastDirty { get; set; } = -1;

	private Coroutine setDirtyCoroutine { get; set; }

	private int lastRefreshLayout { get; set; } = -1;

	private int activeChildCount { get; set; } = -1;

	private bool hasLayoutGroup { get; set; }

	protected override void Awake()
	{
		lastDirty = -1;
		lastRefreshLayout = -1;
		activeChildCount = -1;
		base.Awake();
		m_RectTransform = GetComponent<RectTransform>();
		m_LayoutGroup = GetComponent<LayoutGroup>();
		hasLayoutGroup = layoutGroup != null;
		RefreshLayout();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		m_LayoutGroup = GetComponent<LayoutGroup>();
		hasLayoutGroup = layoutGroup != null;
		SetDirty();
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		SetDirty();
		onRectTransformDimensionsChanged?.Invoke();
	}

	private void LateUpdate()
	{
		if (!hasLayoutGroup || rectTransform.childCount == 0)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < rectTransform.childCount; i++)
		{
			if (rectTransform.GetChild(i).gameObject.activeInHierarchy)
			{
				num++;
			}
		}
		if (activeChildCount != num)
		{
			activeChildCount = num;
			SetDirty();
			ForceRebuildLayoutImmediate();
		}
	}

	public void RefreshLayout()
	{
		if (Application.isPlaying)
		{
			if (lastRefreshLayout == Time.frameCount)
			{
				return;
			}
			lastRefreshLayout = Time.frameCount;
		}
		if (hasLayoutGroup)
		{
			layoutGroup.CalculateLayoutInputHorizontal();
			layoutGroup.CalculateLayoutInputVertical();
			layoutGroup.SetLayoutHorizontal();
			layoutGroup.SetLayoutVertical();
		}
	}

	public void ForceRebuildLayoutImmediate()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
	}

	public void MarkLayoutForRebuild()
	{
		LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
	}

	public void SetDirty()
	{
		if (Application.isPlaying)
		{
			if (lastDirty == Time.frameCount)
			{
				return;
			}
			lastDirty = Time.frameCount;
		}
		if (!IsActive())
		{
			return;
		}
		RefreshLayout();
		if (!CanvasUpdateRegistry.IsRebuildingLayout())
		{
			MarkLayoutForRebuild();
			return;
		}
		if (setDirtyCoroutine != null)
		{
			StopCoroutine(setDirtyCoroutine);
			setDirtyCoroutine = null;
		}
		setDirtyCoroutine = StartCoroutine(DelayedSetDirty());
	}

	private IEnumerator DelayedSetDirty()
	{
		yield return null;
		MarkLayoutForRebuild();
	}
}
