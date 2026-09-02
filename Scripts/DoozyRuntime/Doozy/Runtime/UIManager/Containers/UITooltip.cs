using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.Global;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers.Internal;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.ScriptableObjects;
using Doozy.Runtime.UIManager.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Containers;

[AddComponentMenu("UI/Containers/UITooltip")]
[DisallowMultipleComponent]
public class UITooltip : UIContainerComponent<UITooltip>
{
	public enum Parenting
	{
		TooltipsCanvas = 0,
		TooltipTrigger = 1,
		UITag = 2
	}

	public enum Tracking
	{
		Disabled = 0,
		FollowPointer = 1,
		FollowTrigger = 2,
		FollowTarget = 3
	}

	public enum Positioning
	{
		TopLeft = 0,
		TopCenter = 1,
		TopRight = 2,
		MiddleLeft = 3,
		MiddleCenter = 4,
		MiddleRight = 5,
		BottomLeft = 6,
		BottomCenter = 7,
		BottomRight = 8
	}

	public const int k_MaxSortingOrder = 32767;

	public const string k_DefaultTooltipName = "None";

	public const string k_DefaultTooltipCanvasUITagCategory = "UITooltip";

	public const string k_DefaultTooltipCanvasUITagName = "Canvas";

	private LayoutElement m_LayoutElement;

	[ClearOnReload]
	private static Canvas s_tooltipsCanvas;

	private Canvas m_TooltipRootCanvas;

	private RectTransform m_TargetRectTransform;

	public List<TextMeshProUGUI> Labels = new List<TextMeshProUGUI>();

	public List<Image> Images = new List<Image>();

	public List<UIButton> Buttons = new List<UIButton>();

	public Parenting ParentMode;

	public Tracking TrackingMode;

	public Positioning PositionMode = Positioning.MiddleCenter;

	public UITagId ParentTag;

	public UITagId FollowTag;

	public Vector3 PositionOffset = Vector3.zero;

	public float MaximumWidth;

	public bool KeepInScreen = true;

	public bool OverrideSorting = true;

	public bool HideOnAnyButton = true;

	public bool HideOnBackButton = true;

	private static InputSystemUIInputModule s_inputSystemUIInputModule;

	private UITooltipTrigger m_Trigger;

	private GameObject m_FollowTarget;

	public static IEnumerable<UITooltip> visibleTooltips => UIContainerComponent<UITooltip>.database.Where((UITooltip item) => item.isVisible || item.isShowing);

	public LayoutElement layoutElement
	{
		get
		{
			if (!m_LayoutElement)
			{
				LayoutElement obj = GetComponent<LayoutElement>() ?? base.gameObject.AddComponent<LayoutElement>();
				LayoutElement result = obj;
				m_LayoutElement = obj;
				return result;
			}
			return m_LayoutElement;
		}
	}

	public static Canvas tooltipsCanvas
	{
		get
		{
			if (s_tooltipsCanvas != null)
			{
				return s_tooltipsCanvas;
			}
			UITag uITag = UITag.GetTags("UITooltip", "Canvas").FirstOrDefault();
			if (uITag != null)
			{
				s_tooltipsCanvas = uITag.GetComponent<Canvas>();
				if (s_tooltipsCanvas != null)
				{
					return s_tooltipsCanvas;
				}
			}
			s_tooltipsCanvas = new GameObject("Tooltips Canvas").AddComponent<Canvas>();
			s_tooltipsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
			s_tooltipsCanvas.overrideSorting = true;
			s_tooltipsCanvas.sortingOrder = 32767;
			uITag = s_tooltipsCanvas.gameObject.AddComponent<UITag>();
			uITag.Id.Category = "UITooltip";
			uITag.Id.Name = "Canvas";
			return s_tooltipsCanvas;
		}
		set
		{
			s_tooltipsCanvas = value;
		}
	}

	internal Canvas tooltipRootCanvas
	{
		get
		{
			return m_TooltipRootCanvas;
		}
		set
		{
			tooltipRootCanvasRectTransform = null;
			m_TooltipRootCanvas = value;
			if (!(value == null))
			{
				tooltipRootCanvasRectTransform = value.GetComponent<RectTransform>();
			}
		}
	}

	internal RectTransform tooltipRootCanvasRectTransform { get; private set; }

	internal RectTransform targetRectTransform
	{
		get
		{
			return m_TargetRectTransform;
		}
		set
		{
			targetRootCanvas = null;
			targetRootCanvasRectTransform = null;
			m_TargetRectTransform = value;
			if (!(value == null))
			{
				targetRootCanvas = value.GetComponentInParent<Canvas>().rootCanvas;
				targetRootCanvasRectTransform = targetRootCanvas.GetComponent<RectTransform>();
			}
		}
	}

	internal Canvas targetRootCanvas { get; private set; }

	internal RectTransform targetRootCanvasRectTransform { get; private set; }

	public bool hasLabels => Labels.RemoveNulls().Count > 0;

	public bool hasImages => Images.RemoveNulls().Count > 0;

	public bool hasButtons => Buttons.RemoveNulls().Count > 0;

	public Rect rect => base.rectTransform.rect;

	public float width => rect.width;

	public float height => rect.height;

	public float pivotX => base.rectTransform.pivot.x;

	public float pivotY => base.rectTransform.pivot.y;

	public static InputSystemUIInputModule inputModule
	{
		get
		{
			if (s_inputSystemUIInputModule != null)
			{
				return s_inputSystemUIInputModule;
			}
			if (EventSystem.current == null)
			{
				return null;
			}
			s_inputSystemUIInputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
			return s_inputSystemUIInputModule;
		}
	}

	public static Vector2 pointerPosition => inputModule.point.action.ReadValue<Vector2>();

	private bool showHasBeenCalled { get; set; }

	private bool hideHasBeenCalled { get; set; }

	private bool showHasMovement { get; set; }

	private bool hideHasMovement { get; set; }

	private UIMoveReaction showMoveReaction { get; set; }

	private UIMoveReaction hideMoveReaction { get; set; }

	private bool addedHideEventToButtons { get; set; }

	private bool addedHideOnClick { get; set; }

	public RectTransform parentRectTransform { get; internal set; }

	public UITooltipTrigger trigger
	{
		get
		{
			return m_Trigger;
		}
		set
		{
			triggerRectTransform = null;
			m_Trigger = value;
			if (!(value == null))
			{
				triggerRectTransform = value.GetComponent<RectTransform>();
			}
		}
	}

	public RectTransform triggerRectTransform { get; private set; }

	public bool hasTrigger => trigger != null;

	public bool hasTriggerRectTransform => triggerRectTransform != null;

	public GameObject followTarget
	{
		get
		{
			return m_FollowTarget;
		}
		set
		{
			followTargetRectTransform = null;
			m_FollowTarget = value;
			if (!(value == null))
			{
				followTargetRectTransform = value.GetComponent<RectTransform>();
			}
		}
	}

	public RectTransform followTargetRectTransform { get; private set; }

	public bool hasFollowTarget => followTarget != null;

	public bool hasFollowTargetRectTransform => followTargetRectTransform != null;

	public SignalReceiver backButtonReceiver { get; set; }

	internal bool updateTarget { get; set; }

	protected override void Awake()
	{
		base.Awake();
		addedHideEventToButtons = false;
		backButtonReceiver = new SignalReceiver().SetOnSignalCallback((Signal signal) =>
		{
			if (HideOnBackButton && !base.isHidden && !base.isHiding)
			{
				Hide();
			}
		});
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		BackButton.streamIgnoreDisabled.ConnectReceiver(backButtonReceiver);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		BackButton.streamIgnoreDisabled.DisconnectReceiver(backButtonReceiver);
	}

	private void LateUpdate()
	{
		CheckIfShowOrHideHaveMoveReactions();
		if ((base.isShowing && showHasMovement) || (base.isHiding && hideHasMovement))
		{
			ApplyPositioning();
			return;
		}
		ApplyTracking();
		ApplyPositioning();
		ApplyKeepInScreen();
		SetCustomStartPosition(base.rectTransform.anchoredPosition3D, jumpToPosition: false);
	}

	public virtual void Validate()
	{
		Labels.RemoveNulls();
		Images.RemoveNulls();
		Buttons.RemoveNulls();
	}

	public override void Show()
	{
		UpdateTarget();
		ApplyMaximumWidth();
		ApplyHideOnAnyButton();
		base.Show();
		Coroutiner.ExecuteLater(() =>
		{
			if (!(this == null))
			{
				UITooltipExtensions.ApplyOverrideSorting(this);
			}
		}, 3);
	}

	public override void InstantShow(bool triggerCallbacks)
	{
		UpdateTarget();
		ApplyMaximumWidth();
		ApplyHideOnAnyButton();
		base.InstantShow(triggerCallbacks);
		UITooltipExtensions.ApplyOverrideSorting(this);
	}

	public void UpdateTarget()
	{
		updateTarget = false;
		targetRectTransform = null;
		switch (TrackingMode)
		{
		case Tracking.Disabled:
		case Tracking.FollowPointer:
			break;
		case Tracking.FollowTrigger:
			targetRectTransform = triggerRectTransform;
			break;
		case Tracking.FollowTarget:
			targetRectTransform = followTargetRectTransform;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public RectTransform GetParent()
	{
		RectTransform rectTransform;
		switch (ParentMode)
		{
		case Parenting.TooltipsCanvas:
			rectTransform = tooltipsCanvas.GetComponent<RectTransform>();
			break;
		case Parenting.TooltipTrigger:
			if (trigger == null)
			{
				Debug.Log("[Tooltip] Parenting mode set to 'Tooltip Trigger' but no tooltip trigger is set.Used the TooltipCanvas as parent instead.");
				rectTransform = tooltipsCanvas.GetComponent<RectTransform>();
				break;
			}
			rectTransform = trigger.GetComponent<RectTransform>();
			if (rectTransform == null)
			{
				Debug.Log("[Tooltip] Parenting mode set to 'Tooltip Trigger' but the tooltip trigger has no RectTransform component.Used the TooltipCanvas as parent instead.");
				rectTransform = tooltipsCanvas.GetComponent<RectTransform>();
			}
			break;
		case Parenting.UITag:
		{
			if (ParentTag == null)
			{
				Debug.Log("[Tooltip] Parenting mode set to 'UITag' but no UITag is set.Used the TooltipCanvas as parent instead.");
				rectTransform = tooltipsCanvas.GetComponent<RectTransform>();
				break;
			}
			UITag firstTag = UITag.GetFirstTag(ParentTag.Category, ParentTag.Name);
			if (firstTag == null)
			{
				Debug.Log("[Tooltip] Parenting mode set to 'UITag' but the UITag is not found.Used the TooltipCanvas as parent instead.");
				rectTransform = tooltipsCanvas.GetComponent<RectTransform>();
				break;
			}
			rectTransform = firstTag.GetComponent<RectTransform>();
			if (rectTransform == null)
			{
				Debug.Log("[Tooltip] Parenting mode set to 'UITag' but the UITag has no RectTransform component.Used the TooltipCanvas as parent instead.");
				rectTransform = tooltipsCanvas.GetComponent<RectTransform>();
			}
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
		return rectTransform;
	}

	private void ApplyHideOnAnyButton()
	{
		if (addedHideEventToButtons || !hasButtons || !HideOnAnyButton)
		{
			return;
		}
		foreach (UIButton button in Buttons)
		{
			button.onClickBehaviour.Event.AddListener(Hide);
			button.onSubmitBehaviour.Event.AddListener(Hide);
		}
		addedHideEventToButtons = true;
	}

	private void CheckIfShowOrHideHaveMoveReactions()
	{
		if (base.isShowing && !showHasBeenCalled)
		{
			showHasBeenCalled = true;
			showHasMovement = base.showReactions.Any((Reaction r) => r.GetType() == typeof(UIMoveReaction) && ((UIMoveReaction)r).rectTransform == base.rectTransform && ((UIMoveReaction)r).enabled);
			if (showHasMovement)
			{
				showMoveReaction = base.showReactions.First((Reaction r) => r.GetType() == typeof(UIMoveReaction) && ((UIMoveReaction)r).rectTransform == base.rectTransform) as UIMoveReaction;
			}
		}
		else
		{
			if (!base.isHiding || hideHasBeenCalled)
			{
				return;
			}
			hideHasBeenCalled = true;
			hideHasMovement = base.hideReactions.Any((Reaction r) => r.GetType() == typeof(UIMoveReaction) && ((UIMoveReaction)r).rectTransform == base.rectTransform && ((UIMoveReaction)r).enabled);
			if (hideHasMovement)
			{
				hideMoveReaction = base.hideReactions.First((Reaction r) => r.GetType() == typeof(UIMoveReaction) && ((UIMoveReaction)r).rectTransform == base.rectTransform) as UIMoveReaction;
			}
		}
	}

	private void ApplyTracking()
	{
		Vector3 position;
		switch (TrackingMode)
		{
		case Tracking.Disabled:
			return;
		case Tracking.FollowPointer:
			position = pointerPosition;
			break;
		case Tracking.FollowTrigger:
			if (!hasTrigger)
			{
				TrackingMode = Tracking.Disabled;
				return;
			}
			position = trigger.transform.position;
			break;
		case Tracking.FollowTarget:
			if (!hasFollowTarget)
			{
				this.SetFollowTargetFromUITag(FollowTag.Category, FollowTag.Name);
				if (!hasFollowTarget)
				{
					TrackingMode = Tracking.Disabled;
					return;
				}
				UpdateTarget();
			}
			position = followTarget.transform.position;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		base.transform.position = position;
	}

	private void ApplyPositioning()
	{
		if (!(base.transform.parent == null))
		{
			Vector3 vector;
			switch (TrackingMode)
			{
			case Tracking.Disabled:
				vector = CalculatePositioningWhenTrackingIsDisabled();
				break;
			case Tracking.FollowPointer:
				vector = CalculatePositioningWhenTrackingIsFollowPointer();
				break;
			case Tracking.FollowTrigger:
			case Tracking.FollowTarget:
				vector = CalculatePositioningWhenTrackingIsEnabled();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			vector += PositionOffset;
			vector.x = (float.IsNaN(vector.x) ? 0f : vector.x);
			vector.y = (float.IsNaN(vector.y) ? 0f : vector.y);
			vector.z = (float.IsNaN(vector.z) ? 0f : vector.z);
			vector.x = (float.IsInfinity(vector.x) ? 0f : vector.x);
			vector.y = (float.IsInfinity(vector.y) ? 0f : vector.y);
			vector.z = (float.IsInfinity(vector.z) ? 0f : vector.z);
			if (base.isShowing & showHasMovement)
			{
				showMoveReaction.SetTo(vector);
			}
			else if (base.isHiding & hideHasMovement)
			{
				hideMoveReaction.SetFrom(vector);
			}
			else
			{
				base.rectTransform.anchoredPosition3D = vector;
			}
		}
	}

	private void ApplyKeepInScreen()
	{
		if (KeepInScreen)
		{
			Vector3[] array = new Vector3[4];
			tooltipRootCanvasRectTransform.GetWorldCorners(array);
			Vector3 vector = array[0];
			Vector3 vector2 = array[2];
			Vector3 vector3 = vector2 - vector;
			base.rectTransform.GetWorldCorners(array);
			Vector3 vector4 = array[0];
			Vector3 vector5 = array[2];
			Vector3 vector6 = vector5 - vector4;
			Vector3 position = base.rectTransform.position;
			Vector3 vector7 = position - vector4;
			Vector3 vector8 = vector5 - position;
			position.x = ((vector6.x < vector3.x) ? Mathf.Clamp(position.x, vector.x + vector7.x, vector2.x - vector8.x) : Mathf.Clamp(position.x, vector2.x - vector8.x, vector.x + vector7.x));
			position.y = ((vector6.y < vector3.y) ? Mathf.Clamp(position.y, vector.y + vector7.y, vector2.y - vector8.y) : Mathf.Clamp(position.y, vector2.y - vector8.y, vector.y + vector7.y));
			base.rectTransform.position = position;
		}
	}

	private void ApplyMaximumWidth()
	{
		if (MaximumWidth <= 0f)
		{
			return;
		}
		layoutElement.preferredWidth = -1f;
		layoutElement.enabled = false;
		base.rectTransform.ForceUpdateRectTransforms();
		foreach (TextMeshProUGUI label in Labels)
		{
			label.ForceMeshUpdate();
		}
		if (!(Labels.Max((TextMeshProUGUI label) => label.preferredWidth) < MaximumWidth))
		{
			layoutElement.enabled = true;
			layoutElement.preferredWidth = MaximumWidth;
			base.rectTransform.ForceUpdateRectTransforms();
		}
	}

	private Vector3 CalculatePositioningWhenTrackingIsDisabled()
	{
		if (base.transform.parent == null)
		{
			return base.rectTransform.anchoredPosition3D;
		}
		float z = base.rectTransform.anchoredPosition3D.z;
		Rect rect = parentRectTransform.rect;
		float num = rect.width;
		float num2 = rect.height;
		return PositionMode switch
		{
			Positioning.TopLeft => new Vector3((0f - num) / 2f + width * pivotX, num2 / 2f - height * pivotY, z), 
			Positioning.TopCenter => new Vector3(0f, num2 / 2f - height * pivotY, z), 
			Positioning.TopRight => new Vector3(num / 2f - width * (1f - pivotX), num2 / 2f - height * pivotY, z), 
			Positioning.MiddleLeft => new Vector3((0f - num) / 2f + width * pivotX, 0f, z), 
			Positioning.MiddleCenter => new Vector3(0f, 0f, z), 
			Positioning.MiddleRight => new Vector3(num / 2f - width * (1f - pivotX), 0f, z), 
			Positioning.BottomLeft => new Vector3((0f - num) / 2f + width * pivotX, (0f - num2) / 2f + height * (1f - pivotY), z), 
			Positioning.BottomCenter => new Vector3(0f, (0f - num2) / 2f + height * (1f - pivotY), z), 
			Positioning.BottomRight => new Vector3(num / 2f - width * (1f - pivotX), (0f - num2) / 2f + height * (1f - pivotY), z), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private Vector3 CalculatePositioningWhenTrackingIsFollowPointer()
	{
		Vector2 localPoint;
		switch (tooltipRootCanvas.renderMode)
		{
		case RenderMode.ScreenSpaceOverlay:
			localPoint = parentRectTransform.InverseTransformPoint(pointerPosition);
			break;
		case RenderMode.ScreenSpaceCamera:
		case RenderMode.WorldSpace:
			RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, pointerPosition, tooltipRootCanvas.worldCamera, out localPoint);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		localPoint -= parentRectTransform.rect.center;
		localPoint = PositionMode switch
		{
			Positioning.TopLeft => new Vector3(localPoint.x - width * pivotX, localPoint.y + height * pivotY, 0f), 
			Positioning.TopCenter => new Vector3(localPoint.x, localPoint.y + height * pivotY, 0f), 
			Positioning.TopRight => new Vector3(localPoint.x + width * (1f - pivotX), localPoint.y + height * pivotY, 0f), 
			Positioning.MiddleLeft => new Vector3(localPoint.x - width * pivotX, localPoint.y, 0f), 
			Positioning.MiddleCenter => new Vector3(localPoint.x, localPoint.y, 0f), 
			Positioning.MiddleRight => new Vector3(localPoint.x + width * (1f - pivotX), localPoint.y, 0f), 
			Positioning.BottomLeft => new Vector3(localPoint.x - width * pivotX, localPoint.y - height * (1f - pivotY), 0f), 
			Positioning.BottomCenter => new Vector3(localPoint.x, localPoint.y - height * (1f - pivotY), 0f), 
			Positioning.BottomRight => new Vector3(localPoint.x + width * (1f - pivotX), localPoint.y - height * (1f - pivotY), 0f), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
		return localPoint;
	}

	private Vector3 CalculatePositioningWhenTrackingIsEnabled()
	{
		if (updateTarget)
		{
			UpdateTarget();
		}
		Vector3 vector = parentRectTransform.InverseTransformPoint(targetRectTransform.position);
		Vector3 positionOffset = GetPositionOffset(targetRectTransform, PositionMode);
		Vector3 localScale = targetRectTransform.localScale;
		positionOffset.x *= localScale.x;
		positionOffset.y *= localScale.y;
		Vector3 lossyScale = targetRectTransform.lossyScale;
		Vector3 lossyScale2 = base.rectTransform.lossyScale;
		Vector3 vector2 = new Vector3(lossyScale.x / lossyScale2.x, lossyScale.y / lossyScale2.y, lossyScale.z / lossyScale2.z);
		positionOffset.x *= vector2.x;
		positionOffset.y *= vector2.y;
		positionOffset.z *= vector2.z;
		Vector3 vector3 = vector + positionOffset;
		Vector3 positionOffset2 = GetPositionOffset(base.rectTransform, PositionMode);
		return vector3 + positionOffset2;
	}

	private static Vector3 GetPositionOffset(RectTransform rectTransform, Positioning positionMode)
	{
		Rect rect = rectTransform.rect;
		return positionMode switch
		{
			Positioning.TopLeft => new Vector3(rect.xMin, rect.yMax, 0f), 
			Positioning.TopCenter => new Vector3(rect.center.x, rect.yMax, 0f), 
			Positioning.TopRight => new Vector3(rect.xMax, rect.yMax, 0f), 
			Positioning.MiddleLeft => new Vector3(rect.xMin, rect.center.y, 0f), 
			Positioning.MiddleCenter => new Vector3(rect.center.x, rect.center.y, 0f), 
			Positioning.MiddleRight => new Vector3(rect.xMax, rect.center.y, 0f), 
			Positioning.BottomLeft => new Vector3(rect.xMin, rect.yMin, 0f), 
			Positioning.BottomCenter => new Vector3(rect.center.x, rect.yMin, 0f), 
			Positioning.BottomRight => new Vector3(rect.xMax, rect.yMin, 0f), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	public static UITooltip Get(string tooltipName)
	{
		if (string.IsNullOrEmpty(tooltipName))
		{
			return null;
		}
		GameObject prefab = SingletonRuntimeScriptableObject<UITooltipDatabase>.instance.GetPrefab(tooltipName);
		if (prefab == null)
		{
			Debug.LogWarning("UITooltip.Get(" + tooltipName + ") - prefab not found in the database");
			return null;
		}
		UITooltip tooltip = UITooltipExtensions.Reset(UnityEngine.Object.Instantiate(prefab).GetComponent<UITooltip>());
		tooltip.OnHiddenCallback.Event.AddListener(() =>
		{
			if (!(tooltip == null))
			{
				UnityEngine.Object.Destroy(tooltip.gameObject);
				tooltip = null;
			}
		});
		return tooltip;
	}

	public static void ShowAllTooltips()
	{
		foreach (UITooltipTrigger item in UITooltipTrigger.database)
		{
			if (!item.isActiveAndEnabled)
			{
				break;
			}
			item.ShowTooltip();
		}
	}

	public void HideAllTooltips()
	{
		foreach (UITooltip item in UIContainerComponent<UITooltip>.database)
		{
			if (!item.isActiveAndEnabled)
			{
				break;
			}
			item.Hide();
		}
	}
}
