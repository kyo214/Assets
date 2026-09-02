using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.Mody;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Triggers;

[DisallowMultipleComponent]
public class UITooltipTrigger : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	public string UITooltipName = "None";

	public bool ShowOnPointerEnter = true;

	public bool HideOnPointerExit = true;

	public bool ShowOnPointerClick;

	public bool HideOnPointerClick;

	public bool OverrideParentMode;

	public UITooltip.Parenting ParentMode;

	public bool OverrideTrackingMode;

	public UITooltip.Tracking TrackingMode;

	public bool OverridePositionMode;

	public UITooltip.Positioning PositionMode = UITooltip.Positioning.MiddleCenter;

	public UITagId ParentTag;

	public UITagId FollowTag;

	public bool OverridePositionOffset;

	public Vector3 PositionOffset = Vector3.zero;

	public bool OverrideMaximumWidth;

	public float MaximumWidth;

	[SerializeField]
	private float ShowDelay;

	public List<string> Texts = new List<string>();

	public List<Sprite> Sprites = new List<Sprite>();

	public List<UnityEvent> Events = new List<UnityEvent>();

	public ModyEvent OnShowCallback = new ModyEvent();

	public ModyEvent OnHideCallback = new ModyEvent();

	[ClearOnReload]
	public static HashSet<UITooltipTrigger> database { get; } = new HashSet<UITooltipTrigger>();

	public float showDelay
	{
		get
		{
			return ShowDelay;
		}
		set
		{
			ShowDelay = Mathf.Max(0f, value);
		}
	}

	public UITooltip tooltip { get; private set; }

	private bool isValid { get; set; }

	private Coroutine showDelayCoroutine { get; set; }

	public bool isWaitingToShow => showDelayCoroutine != null;

	private void Validate()
	{
		isValid = SingletonRuntimeScriptableObject<UITooltipDatabase>.instance.GetPrefab(UITooltipName) != null;
	}

	private void Awake()
	{
		database.Add(this);
	}

	private void OnEnable()
	{
		database.Remove(null);
		Validate();
		if (!isValid)
		{
			Debug.LogWarning("UITooltipTrigger - " + base.name + " - The UITooltip name '" + UITooltipName + "' is not valid. Please make sure it is spelled correctly in the UITooltipDatabase.", this);
		}
	}

	private void OnDisable()
	{
		StopShowDelayCoroutine();
	}

	private void OnDestroy()
	{
		database.Remove(this);
		database.Remove(null);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		StopShowDelayCoroutine();
		if (isValid && ShowOnPointerEnter)
		{
			if (showDelay > 0f)
			{
				showDelayCoroutine = StartCoroutine(ShowDelayCoroutine());
			}
			else
			{
				ShowTooltip();
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		StopShowDelayCoroutine();
		if (isValid && HideOnPointerExit)
		{
			HideTooltip();
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		StopShowDelayCoroutine();
		if (isValid)
		{
			if (ShowOnPointerClick && tooltip == null)
			{
				ShowTooltip();
			}
			else if (HideOnPointerClick)
			{
				HideTooltip();
			}
		}
	}

	private IEnumerator ShowDelayCoroutine()
	{
		yield return new WaitForSecondsRealtime(showDelay);
		ShowTooltip();
		StopShowDelayCoroutine();
	}

	private void StopShowDelayCoroutine()
	{
		if (showDelayCoroutine != null)
		{
			StopCoroutine(showDelayCoroutine);
			showDelayCoroutine = null;
		}
	}

	public virtual void ShowTooltip()
	{
		if (tooltip != null)
		{
			if (tooltip.isVisible || tooltip.isShowing)
			{
				return;
			}
			tooltip.InstantHide();
			if (tooltip != null)
			{
				Object.Destroy(tooltip.gameObject);
				tooltip = null;
			}
		}
		tooltip = UITooltip.Get(UITooltipName);
		if (!(tooltip == null))
		{
			tooltip.SetTrigger(this);
			if (OverrideParentMode)
			{
				tooltip.ParentMode = ParentMode;
				tooltip.ParentTag = ParentTag;
			}
			UITooltipExtensions.SetParent(tooltip, tooltip.GetParent());
			if (OverrideTrackingMode)
			{
				tooltip.TrackingMode = TrackingMode;
				tooltip.FollowTag = FollowTag;
			}
			if (OverridePositionMode)
			{
				tooltip.PositionMode = PositionMode;
			}
			if (OverridePositionOffset)
			{
				tooltip.PositionOffset = PositionOffset;
			}
			if (OverrideMaximumWidth)
			{
				tooltip.MaximumWidth = MaximumWidth;
			}
			tooltip.InstantHide(triggerCallbacks: false);
			UITooltipExtensions.SetTexts(tooltip, Texts.RemoveNulls().ToArray());
			UITooltipExtensions.SetSprites(tooltip, Sprites.RemoveNulls().ToArray());
			UITooltipExtensions.SetEvents(tooltip, Events.RemoveNulls().ToArray());
			tooltip.Show();
			OnShowCallback?.Execute();
		}
	}

	public virtual void HideTooltip()
	{
		if (tooltip == null)
		{
			return;
		}
		tooltip.OnHiddenCallback.Event.AddListener(() =>
		{
			if (!(tooltip == null))
			{
				Object.Destroy(tooltip.gameObject);
				tooltip = null;
				OnHideCallback?.Execute();
			}
		});
		tooltip.Hide();
	}
}
