using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.Global;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers.Internal;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.ScriptableObjects;
using Doozy.Runtime.UIManager.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Containers;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))]
[AddComponentMenu("UI/Containers/UIPopup")]
[DisallowMultipleComponent]
public class UIPopup : UIContainerComponent<UIPopup>
{
	public enum Parenting
	{
		PopupsCanvas = 0,
		UITag = 1
	}

	public const int k_MaxSortingOrder = 32766;

	public const string k_DefaultPopupName = "None";

	public const string k_DefaultPopupCanvasUITagCategory = "UIPopup";

	public const string k_DefaultPopupCanvasUITagName = "Canvas";

	public const string k_DefaultQueueName = "Default";

	[ClearOnReload]
	private static Canvas s_popupsCanvas;

	[ClearOnReload]
	private static Dictionary<string, List<UIPopup>> s_queues;

	public Parenting ParentMode;

	public UITagId ParentTag;

	public bool OverrideSorting = true;

	public bool BlockBackButton = true;

	public bool RestoreSelectedAfterHide = true;

	public bool HideOnAnyButton = true;

	public bool HideOnBackButton;

	public bool HideOnClickContainer = true;

	public bool HideOnClickOverlay = true;

	public RectTransform Overlay;

	public RectTransform Container;

	public List<TextMeshProUGUI> Labels = new List<TextMeshProUGUI>();

	public List<Image> Images = new List<Image>();

	public List<UIButton> Buttons = new List<UIButton>();

	public static IEnumerable<UIPopup> visiblePopups => UIContainerComponent<UIPopup>.database.Where((UIPopup item) => item.isVisible || item.isShowing);

	public static Canvas popupsCanvas
	{
		get
		{
			if (s_popupsCanvas != null)
			{
				return s_popupsCanvas;
			}
			UITag uITag = UITag.GetTags("UIPopup", "Canvas").FirstOrDefault();
			if (uITag != null)
			{
				s_popupsCanvas = uITag.GetComponent<Canvas>();
				if (s_popupsCanvas != null)
				{
					return s_popupsCanvas;
				}
			}
			s_popupsCanvas = new GameObject("Popups Canvas").AddComponent<Canvas>();
			s_popupsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
			s_popupsCanvas.overrideSorting = true;
			s_popupsCanvas.sortingOrder = 32766;
			uITag = s_popupsCanvas.gameObject.AddComponent<UITag>();
			uITag.Id.Category = "UIPopup";
			uITag.Id.Name = "Canvas";
			return s_popupsCanvas;
		}
		set
		{
			s_popupsCanvas = value;
		}
	}

	public static Dictionary<string, List<UIPopup>> queues => s_queues ?? (s_queues = new Dictionary<string, List<UIPopup>>());

	public bool hasOverlay => Overlay != null;

	public bool hasContainer => Container != null;

	public bool hasLabels => Labels.RemoveNulls().Count > 0;

	public bool hasImages => Images.RemoveNulls().Count > 0;

	public bool hasButtons => Buttons.RemoveNulls().Count > 0;

	public RectTransform parentRectTransform { get; internal set; }

	public SignalReceiver backButtonReceiver { get; set; }

	private bool unblockBackButton { get; set; }

	private bool addedHideOnClickOverlay { get; set; }

	private bool addedHideOnClickContainer { get; set; }

	private bool addedHideEventToButtons { get; set; }

	private GameObject previouslySelectedGameObject { get; set; }

	private Coroutine sortingCoroutine { get; set; }

	public static List<UIPopup> GetQueue(string queueName = "Default")
	{
		if (string.IsNullOrEmpty(queueName))
		{
			return null;
		}
		if (!queues.TryGetValue(queueName, out var value))
		{
			return null;
		}
		return value.RemoveNulls();
	}

	public static UIPopup GetFirstPopupInQueue(string queueName = "Default")
	{
		return GetQueue(queueName)?.FirstOrDefault();
	}

	public static UIPopup ShowNextPopupInQueue(string queueName = "Default")
	{
		List<UIPopup> queue = GetQueue(queueName);
		if (queue == null)
		{
			return null;
		}
		if (queue.Count == 0)
		{
			queues.Remove(queueName);
			return null;
		}
		UIPopup popup = queue.FirstOrDefault();
		if (popup == null)
		{
			return null;
		}
		popup.OnHiddenCallback.Event.AddListener(() =>
		{
			RemovePopupFromQueue(popup);
			ShowNextPopupInQueue(queueName);
		});
		popup.Show();
		return popup;
	}

	public static void AddPopupToQueue(UIPopup popup, string queueName = "Default")
	{
		if (popup == null)
		{
			return;
		}
		List<UIPopup> list = GetQueue(queueName);
		bool flag = list == null || list.Count == 0;
		if (list == null)
		{
			list = new List<UIPopup>();
			queues.Add(queueName, list);
		}
		if (flag || !list.Contains(popup))
		{
			list.Add(popup);
			if (!popup.isHidden)
			{
				popup.InstantHide(triggerCallbacks: false);
			}
			if (flag)
			{
				ShowNextPopupInQueue(queueName);
			}
		}
	}

	public static void RemovePopupFromQueue(UIPopup popup, string queueName = "Default")
	{
		if (!(popup == null))
		{
			GetQueue(queueName)?.Remove(popup);
			if (popup.isVisible || popup.isShowing)
			{
				popup.Hide();
			}
		}
	}

	public static void ClearQueue(string queueName = "Default")
	{
		List<UIPopup> queue = GetQueue(queueName);
		if (queue == null)
		{
			return;
		}
		foreach (UIPopup item in queue)
		{
			item.Hide();
		}
		queue.Clear();
		queues.Remove(queueName);
	}

	public virtual void Validate()
	{
		Labels.RemoveNulls();
		Images.RemoveNulls();
		Buttons.RemoveNulls();
	}

	protected override void Awake()
	{
		base.Awake();
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
		StopSortingCoroutine();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		EnableBackButton();
	}

	public override void Show()
	{
		SavePreviouslySelectedGameObject();
		DisableBackButton();
		AddOnClickToOverlay();
		AddOnClickToContainer();
		StartSortingCoroutine();
		base.Show();
	}

	public override void InstantShow()
	{
		SavePreviouslySelectedGameObject();
		DisableBackButton();
		AddOnClickToOverlay();
		AddOnClickToContainer();
		StartSortingCoroutine();
		base.InstantShow();
	}

	public override void Hide()
	{
		StopSortingCoroutine();
		EnableBackButton();
		base.Hide();
	}

	public override void InstantHide()
	{
		StopSortingCoroutine();
		EnableBackButton();
		base.InstantHide();
	}

	public RectTransform GetParent()
	{
		RectTransform rectTransform;
		switch (ParentMode)
		{
		case Parenting.PopupsCanvas:
			rectTransform = popupsCanvas.GetComponent<RectTransform>();
			break;
		case Parenting.UITag:
		{
			if (ParentTag == null)
			{
				Debug.Log("[Popup] Parenting mode set to 'UITag' but no UITag is set.Used the PopupsCanvas as parent instead.");
				rectTransform = popupsCanvas.GetComponent<RectTransform>();
				break;
			}
			UITag firstTag = UITag.GetFirstTag(ParentTag.Category, ParentTag.Name);
			if (firstTag == null)
			{
				Debug.Log("[Popup] Parenting mode set to 'UITag' but the UITag is not found.Used the PopupsCanvas as parent instead.");
				rectTransform = popupsCanvas.GetComponent<RectTransform>();
				break;
			}
			rectTransform = firstTag.GetComponent<RectTransform>();
			if (rectTransform == null)
			{
				Debug.Log("[Popup] Parenting mode set to 'UITag' but the UITag has no RectTransform component.Used the PopupsCanvas as parent instead.");
				rectTransform = popupsCanvas.GetComponent<RectTransform>();
			}
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
		return rectTransform;
	}

	private void StartSortingCoroutine()
	{
		StopSortingCoroutine();
		sortingCoroutine = StartCoroutine(Coroutiner.DelayExecution(() =>
		{
			if (!(this == null))
			{
				UIPopupExtensions.ApplyOverrideSorting(this);
			}
		}, 3));
	}

	private void StopSortingCoroutine()
	{
		if (sortingCoroutine != null)
		{
			StopCoroutine(sortingCoroutine);
			sortingCoroutine = null;
		}
	}

	private void DisableBackButton()
	{
		if (!unblockBackButton && BlockBackButton)
		{
			BackButton.Disable();
			unblockBackButton = true;
		}
	}

	private void EnableBackButton()
	{
		if (unblockBackButton && BlockBackButton)
		{
			BackButton.Enable();
			unblockBackButton = false;
		}
	}

	private void AddOnClickToOverlay()
	{
		if (hasOverlay && HideOnClickOverlay && !addedHideOnClickOverlay)
		{
			(Overlay.GetComponent<PointerClickTrigger>() ?? Overlay.gameObject.AddComponent<PointerClickTrigger>()).OnTrigger.AddListener((PointerEventData evt) =>
			{
				Hide();
			});
			addedHideOnClickOverlay = true;
		}
	}

	private void AddOnClickToContainer()
	{
		if (hasContainer && HideOnClickContainer && !addedHideOnClickContainer)
		{
			(Container.GetComponent<PointerClickTrigger>() ?? Container.gameObject.AddComponent<PointerClickTrigger>()).OnTrigger.AddListener((PointerEventData evt) =>
			{
				Hide();
			});
			addedHideOnClickContainer = true;
		}
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

	private void SavePreviouslySelectedGameObject()
	{
		if (!(EventSystem.current == null))
		{
			previouslySelectedGameObject = EventSystem.current.currentSelectedGameObject;
		}
	}

	private void RestorePreviouslySelectedGameObject()
	{
		if (RestoreSelectedAfterHide && !(EventSystem.current == null) && !(previouslySelectedGameObject == null))
		{
			EventSystem.current.SetSelectedGameObject(previouslySelectedGameObject);
		}
	}

	public static UIPopup Get(string popupName)
	{
		if (string.IsNullOrEmpty(popupName))
		{
			return null;
		}
		GameObject prefab = SingletonRuntimeScriptableObject<UIPopupDatabase>.instance.GetPrefab(popupName);
		if (prefab == null)
		{
			Debug.LogWarning("UIPopup.Get(" + popupName + ") - prefab not found in the database");
			return null;
		}
		UIPopup popup = UIPopupExtensions.Reset(UnityEngine.Object.Instantiate(prefab).GetComponent<UIPopup>());
		popup.Validate();
		popup.ApplyHideOnAnyButton();
		UIPopupExtensions.SetParent(popup, popup.GetParent());
		popup.InstantHide(triggerCallbacks: false);
		popup.OnHiddenCallback.Event.AddListener(() =>
		{
			if (!(popup == null))
			{
				popup.RestorePreviouslySelectedGameObject();
				UnityEngine.Object.Destroy(popup.gameObject);
				popup = null;
			}
		});
		return popup;
	}
}
