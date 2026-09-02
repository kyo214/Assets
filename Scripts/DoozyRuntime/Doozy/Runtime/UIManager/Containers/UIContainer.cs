using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.Global;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIManager.Events;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Containers;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Containers/UIContainer")]
[SelectionBase]
public class UIContainer : MonoBehaviour, ICanvasElement, IUseMultiplayerInfo
{
	public const string k_StreamCategory = "UIContainer";

	public const float k_DefaultAnimationDuration = 0.3f;

	[SerializeField]
	private MultiplayerInfo MultiplayerInfo;

	private Canvas m_Canvas;

	private CanvasGroup m_CanvasGroup;

	private GraphicRaycaster m_GraphicRaycaster;

	private RectTransform m_RectTransform;

	public ContainerBehaviour OnStartBehaviour;

	private int m_LastFrameVisibilityStateChanged;

	protected VisibilityState VisibilityState;

	public ModyEvent OnShowCallback;

	public ModyEvent OnVisibleCallback;

	public ModyEvent OnHideCallback;

	public ModyEvent OnHiddenCallback;

	public VisibilityStateEvent OnVisibilityChangedCallback;

	[SerializeField]
	private List<Progressor> ShowProgressors;

	[SerializeField]
	private List<Progressor> HideProgressors;

	[SerializeField]
	private List<Progressor> ShowHideProgressors;

	public Vector3 CustomStartPosition;

	public bool UseCustomStartPosition;

	public bool AutoHideAfterShow;

	public float AutoHideAfterShowDelay;

	public bool DisableGameObjectWhenHidden;

	public bool DisableCanvasWhenHidden = true;

	public bool DisableGraphicRaycasterWhenHidden = true;

	public bool HandleCanvasGroupBlockRaycasts = true;

	public bool ClearSelectedOnShow;

	public bool ClearSelectedOnHide;

	public bool AutoSelectAfterShow;

	public GameObject AutoSelectTarget;

	private HashSet<Reaction> m_ShowReactions;

	private HashSet<Reaction> m_HideReactions;

	private Coroutine m_AutoHideCoroutine;

	private Coroutine m_CoroutineIsShowing;

	private Coroutine m_CoroutineIsHiding;

	private Coroutine m_DisableGameObjectWithDelayCoroutine;

	private Coroutine m_DelayedShowCoroutine;

	private Coroutine m_DelayedHideCoroutine;

	protected MultiplayerEventSystem MultiplayerEventSystem;

	public MultiplayerInfo multiplayerInfo => MultiplayerInfo;

	public bool hasMultiplayerInfo => multiplayerInfo != null;

	public int playerIndex
	{
		get
		{
			if (!(multiplayerMode & hasMultiplayerInfo))
			{
				return inputSettings.defaultPlayerIndex;
			}
			return multiplayerInfo.playerIndex;
		}
	}

	public static UIManagerInputSettings inputSettings => SingletonRuntimeScriptableObject<UIManagerInputSettings>.instance;

	public static bool multiplayerMode => inputSettings.multiplayerMode;

	public Canvas canvas
	{
		get
		{
			if (!m_Canvas)
			{
				return m_Canvas = GetComponent<Canvas>();
			}
			return m_Canvas;
		}
	}

	public CanvasGroup canvasGroup
	{
		get
		{
			if (!m_CanvasGroup)
			{
				return m_CanvasGroup = GetComponent<CanvasGroup>();
			}
			return m_CanvasGroup;
		}
	}

	public GraphicRaycaster graphicRaycaster
	{
		get
		{
			if (!m_GraphicRaycaster)
			{
				return m_GraphicRaycaster = GetComponent<GraphicRaycaster>();
			}
			return m_GraphicRaycaster;
		}
	}

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

	public bool hasCanvas { get; private set; }

	public bool hasGraphicRaycaster { get; private set; }

	public bool hasCanvasGroup { get; private set; }

	public VisibilityState visibilityState
	{
		get
		{
			return VisibilityState;
		}
		private set
		{
			SetVisibility(value, triggerCallbacks: true);
		}
	}

	public bool isVisible => visibilityState == VisibilityState.Visible;

	public bool isHidden => visibilityState == VisibilityState.Hidden;

	public bool isShowing => visibilityState == VisibilityState.IsShowing;

	public bool isHiding => visibilityState == VisibilityState.IsHiding;

	public bool inTransition
	{
		get
		{
			if (!isShowing)
			{
				return isHiding;
			}
			return true;
		}
	}

	public bool hasOnShowCallbacks
	{
		get
		{
			if (OnShowCallback != null)
			{
				return OnShowCallback.hasCallbacks;
			}
			return false;
		}
	}

	public bool hasOnVisibleCallbacks
	{
		get
		{
			if (OnVisibleCallback != null)
			{
				return OnVisibleCallback.hasCallbacks;
			}
			return false;
		}
	}

	public bool hasOnHideCallbacks
	{
		get
		{
			if (OnHideCallback != null)
			{
				return OnHideCallback.hasCallbacks;
			}
			return false;
		}
	}

	public bool hasOnHiddenCallbacks
	{
		get
		{
			if (OnHiddenCallback != null)
			{
				return OnHiddenCallback.hasCallbacks;
			}
			return false;
		}
	}

	public bool hasOnVisibilityChangedCallbacks
	{
		get
		{
			if (OnVisibilityChangedCallback != null)
			{
				return OnVisibilityChangedCallback.GetPersistentEventCount() > 0;
			}
			return false;
		}
	}

	public bool hasCallbacks => hasOnShowCallbacks | hasOnVisibleCallbacks | hasOnHideCallbacks | hasOnHiddenCallbacks | hasOnVisibilityChangedCallbacks;

	public List<Progressor> showProgressors => ShowProgressors ?? (ShowProgressors = new List<Progressor>());

	public List<Progressor> hideProgressors => HideProgressors ?? (HideProgressors = new List<Progressor>());

	public List<Progressor> showHideProgressors => ShowHideProgressors ?? (ShowHideProgressors = new List<Progressor>());

	public UnityAction<ShowHideExecute> showHideExecute { get; set; }

	public bool executedFirstCommand { get; protected set; }

	public ShowHideExecute previouslyExecutedCommand { get; protected set; }

	public bool hasShowReactions
	{
		get
		{
			if (showReactions != null)
			{
				return showReactions.Count > 0;
			}
			return false;
		}
	}

	public bool hasHideReactions
	{
		get
		{
			if (hideReactions != null)
			{
				return hideReactions.Count > 0;
			}
			return false;
		}
	}

	public bool anyShowAnimationIsActive => showReactions.Any((Reaction show) => show.isActive);

	public bool anyHideAnimationIsActive => hideReactions.Any((Reaction hide) => hide.isActive);

	public bool anyAnimationIsActive => anyShowAnimationIsActive | anyHideAnimationIsActive;

	public bool hasShowProgressors => showProgressors.Count > 0;

	public bool hasHideProgressors => hideProgressors.Count > 0;

	public bool hasShowHideProgressors => showHideProgressors.Count > 0;

	public bool hasProgressors
	{
		get
		{
			if (!hasShowProgressors && !hasHideProgressors)
			{
				return hasShowHideProgressors;
			}
			return true;
		}
	}

	public bool anyShowProgressorIsActive => showProgressors.Where((Progressor p) => p != null).Any((Progressor p) => p.reaction.isActive);

	public bool anyHideProgressorIsActive => hideProgressors.Where((Progressor p) => p != null).Any((Progressor p) => p.reaction.isActive);

	public bool anyShowHideProgressorIsActive => showHideProgressors.Where((Progressor p) => p != null).Any((Progressor p) => p.reaction.isActive);

	public bool anyProgressorIsActive => anyShowProgressorIsActive | anyHideProgressorIsActive | anyShowHideProgressorIsActive;

	internal HashSet<Reaction> showReactions => m_ShowReactions ?? (m_ShowReactions = new HashSet<Reaction>());

	public float totalDurationForShow => CalculateTotalShowDuration();

	internal HashSet<Reaction> hideReactions => m_HideReactions ?? (m_HideReactions = new HashSet<Reaction>());

	public float totalDurationForHide => CalculateTotalHideDuration();

	Transform ICanvasElement.transform => base.transform;

	public void SetMultiplayerInfo(MultiplayerInfo reference)
	{
		MultiplayerInfo = reference;
	}

	public UIContainer()
	{
		UseCustomStartPosition = true;
		OnShowCallback = new ModyEvent("OnShowCallback");
		OnVisibleCallback = new ModyEvent("OnVisibleCallback");
		OnHideCallback = new ModyEvent("OnHideCallback");
		OnHiddenCallback = new ModyEvent("OnHiddenCallback");
		OnVisibilityChangedCallback = new VisibilityStateEvent();
	}

	public virtual void Rebuild(CanvasUpdate executing)
	{
	}

	public virtual void LayoutComplete()
	{
	}

	public virtual void GraphicUpdateComplete()
	{
	}

	public bool IsDestroyed()
	{
		return this == null;
	}

	protected virtual void Awake()
	{
		if (Application.isPlaying)
		{
			BackButton.Initialize();
			hasCanvas = GetComponent<Canvas>() != null;
			hasGraphicRaycaster = GetComponent<GraphicRaycaster>() != null;
			hasCanvasGroup = GetComponent<CanvasGroup>() != null;
			showReactions.Remove(null);
			hideReactions.Remove(null);
			executedFirstCommand = false;
			if (UseCustomStartPosition)
			{
				SetCustomStartPosition(CustomStartPosition);
			}
		}
	}

	protected virtual void OnEnable()
	{
		if (Application.isPlaying)
		{
			BackButton.Initialize();
			hasCanvas = GetComponent<Canvas>() != null;
			hasGraphicRaycaster = GetComponent<GraphicRaycaster>() != null;
			hasCanvasGroup = GetComponent<CanvasGroup>() != null;
			showReactions.Remove(null);
			hideReactions.Remove(null);
		}
	}

	protected virtual void Start()
	{
		if (Application.isPlaying)
		{
			RunBehaviour(OnStartBehaviour);
		}
	}

	protected virtual void OnDisable()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		StopIsShowingCoroutine();
		StopIsHidingCoroutine();
		showReactions.Remove(null);
		foreach (Reaction showReaction in showReactions)
		{
			showReaction.Stop();
		}
		hideReactions.Remove(null);
		foreach (Reaction hideReaction in hideReactions)
		{
			hideReaction.Stop();
		}
		StopAllCoroutines();
	}

	protected virtual void OnDestroy()
	{
	}

	public virtual void SetCustomStartPosition(Vector3 startPosition, bool jumpToPosition = true)
	{
		CustomStartPosition = startPosition;
		showReactions.Remove(null);
		foreach (Reaction showReaction in showReactions)
		{
			if (showReaction is UIMoveReaction uIMoveReaction)
			{
				uIMoveReaction.startPosition = startPosition;
			}
		}
		hideReactions.Remove(null);
		foreach (Reaction hideReaction in hideReactions)
		{
			if (hideReaction is UIMoveReaction uIMoveReaction2)
			{
				uIMoveReaction2.startPosition = startPosition;
			}
		}
		if (jumpToPosition)
		{
			rectTransform.anchoredPosition3D = startPosition;
		}
	}

	protected virtual void SetSelected(GameObject selectable)
	{
		if (multiplayerMode && (MultiplayerEventSystem != null || (hasMultiplayerInfo && multiplayerInfo.gameObject.TryGetComponent<MultiplayerEventSystem>(out MultiplayerEventSystem))))
		{
			MultiplayerEventSystem.SetSelectedGameObject(selectable);
		}
		else
		{
			EventSystem.current.SetSelectedGameObject(selectable);
		}
	}

	private void ExecutedCommand(ShowHideExecute command)
	{
		showHideExecute?.Invoke(command);
		executedFirstCommand = true;
		if (!hasProgressors)
		{
			previouslyExecutedCommand = command;
			return;
		}
		showProgressors.RemoveNulls();
		hideProgressors.RemoveNulls();
		showHideProgressors.RemoveNulls();
		switch (command)
		{
		case ShowHideExecute.Show:
			hideProgressors.ForEach((Progressor p) =>
			{
				p.Stop();
			});
			showProgressors.ForEach((Progressor p) =>
			{
				p.SetProgressAtZero();
				p.Play(PlayDirection.Forward);
			});
			showHideProgressors.ForEach((Progressor p) =>
			{
				p.SetProgressAtZero();
				p.Play(PlayDirection.Forward);
			});
			break;
		case ShowHideExecute.Hide:
			showProgressors.ForEach((Progressor p) =>
			{
				p.Stop();
			});
			hideProgressors.ForEach((Progressor p) =>
			{
				p.SetProgressAtZero();
				p.Play(PlayDirection.Forward);
			});
			showHideProgressors.ForEach((Progressor p) =>
			{
				p.SetProgressAtOne();
				p.Play(PlayDirection.Reverse);
			});
			break;
		case ShowHideExecute.InstantShow:
			hideProgressors.ForEach((Progressor p) =>
			{
				p.Stop();
			});
			showProgressors.ForEach((Progressor p) =>
			{
				p.SetProgressAtOne();
			});
			showHideProgressors.ForEach((Progressor p) =>
			{
				p.SetProgressAtOne();
			});
			break;
		case ShowHideExecute.InstantHide:
			showProgressors.ForEach((Progressor p) =>
			{
				p.Stop();
			});
			hideProgressors.ForEach((Progressor p) =>
			{
				p.SetProgressAtOne();
			});
			showHideProgressors.ForEach((Progressor p) =>
			{
				p.SetProgressAtZero();
			});
			break;
		case ShowHideExecute.ReverseShow:
			hideProgressors.ForEach((Progressor p) =>
			{
				p.Stop();
			});
			if (previouslyExecutedCommand == ShowHideExecute.ReverseShow)
			{
				showProgressors.ForEach((Progressor p) =>
				{
					if (p.reaction.isActive)
					{
						p.Reverse();
					}
					else
					{
						p.Play(PlayDirection.Forward);
					}
				});
				showHideProgressors.ForEach((Progressor p) =>
				{
					if (p.reaction.isActive && p.reaction.direction == PlayDirection.Reverse)
					{
						p.Reverse();
					}
					else
					{
						p.Play(PlayDirection.Forward);
					}
				});
				break;
			}
			showProgressors.ForEach((Progressor p) =>
			{
				if (p.reaction.isActive)
				{
					p.Reverse();
				}
				else
				{
					p.Play(PlayDirection.Reverse);
				}
			});
			showHideProgressors.ForEach((Progressor p) =>
			{
				if (p.reaction.isActive && p.reaction.direction == PlayDirection.Forward)
				{
					p.Reverse();
				}
				else
				{
					p.Play(PlayDirection.Reverse);
				}
			});
			break;
		case ShowHideExecute.ReverseHide:
			showProgressors.ForEach((Progressor p) =>
			{
				p.Stop();
			});
			if (previouslyExecutedCommand == ShowHideExecute.ReverseHide)
			{
				hideProgressors.ForEach((Progressor p) =>
				{
					if (p.reaction.isActive)
					{
						p.Reverse();
					}
					else
					{
						p.Play(PlayDirection.Forward);
					}
				});
				showHideProgressors.ForEach((Progressor p) =>
				{
					if (p.reaction.isActive && p.reaction.direction == PlayDirection.Forward)
					{
						p.Reverse();
					}
					else
					{
						p.Play(PlayDirection.Reverse);
					}
				});
				break;
			}
			hideProgressors.ForEach((Progressor p) =>
			{
				if (p.reaction.isActive)
				{
					p.Reverse();
				}
				else
				{
					p.Play(PlayDirection.Reverse);
				}
			});
			showHideProgressors.ForEach((Progressor p) =>
			{
				if (p.reaction.isActive && p.reaction.direction == PlayDirection.Reverse)
				{
					p.Reverse();
				}
				else
				{
					p.Play(PlayDirection.Forward);
				}
			});
			break;
		default:
			throw new ArgumentOutOfRangeException("command", command, null);
		}
		previouslyExecutedCommand = command;
	}

	public virtual void InstantShow()
	{
		InstantShow(triggerCallbacks: true);
	}

	public virtual void InstantShow(bool triggerCallbacks)
	{
		StopDelayedShowCoroutine();
		StopDelayedHideCoroutine();
		if (!isVisible)
		{
			StopIsShowingCoroutine();
			StopIsHidingCoroutine();
			if (hasCanvas)
			{
				canvas.enabled = true;
			}
			if (hasGraphicRaycaster & DisableGraphicRaycasterWhenHidden)
			{
				graphicRaycaster.enabled = true;
			}
			if (hasCanvasGroup & HandleCanvasGroupBlockRaycasts)
			{
				canvasGroup.blocksRaycasts = true;
			}
			base.gameObject.SetActive(value: true);
			ExecutedCommand(ShowHideExecute.InstantShow);
			if (ClearSelectedOnShow)
			{
				SetSelected(null);
			}
			if (AutoSelectAfterShow && AutoSelectTarget != null)
			{
				SetSelected(AutoSelectTarget);
			}
			SetVisibility(VisibilityState.IsShowing, triggerCallbacks);
			SetVisibility(VisibilityState.Visible, triggerCallbacks);
		}
	}

	public virtual void InstantHide()
	{
		InstantHide(triggerCallbacks: true);
	}

	public virtual void InstantHide(bool triggerCallbacks)
	{
		StopDelayedShowCoroutine();
		StopDelayedHideCoroutine();
		if (!isHidden)
		{
			StopIsShowingCoroutine();
			StopIsHidingCoroutine();
			ExecutedCommand(ShowHideExecute.InstantHide);
			if (ClearSelectedOnHide)
			{
				SetSelected(null);
			}
			SetVisibility(VisibilityState.IsHiding, triggerCallbacks);
			SetVisibility(VisibilityState.Hidden, triggerCallbacks);
		}
	}

	public virtual void InstantToggle()
	{
		InstantToggle(triggerCallbacks: true);
	}

	public virtual void InstantToggle(bool triggerCallbacks)
	{
		switch (visibilityState)
		{
		case VisibilityState.Visible:
		case VisibilityState.IsShowing:
			InstantHide();
			break;
		case VisibilityState.Hidden:
		case VisibilityState.IsHiding:
			InstantShow();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public virtual void Show()
	{
		Show(triggerCallbacks: true);
	}

	public virtual void Show(bool triggerCallbacks)
	{
		StopDelayedShowCoroutine();
		StopDelayedHideCoroutine();
		if (isShowing || isVisible)
		{
			return;
		}
		base.gameObject.SetActive(value: true);
		if (m_LastFrameVisibilityStateChanged == Time.frameCount)
		{
			StartDelayedShowCoroutine(triggerCallbacks);
			return;
		}
		if (ClearSelectedOnShow)
		{
			SetSelected(null);
		}
		if (hasCanvas)
		{
			canvas.enabled = true;
		}
		if (hasGraphicRaycaster & DisableGraphicRaycasterWhenHidden)
		{
			graphicRaycaster.enabled = true;
		}
		if (hasCanvasGroup & HandleCanvasGroupBlockRaycasts)
		{
			canvasGroup.blocksRaycasts = true;
		}
		if (isHiding)
		{
			StopIsHidingCoroutine();
			ExecutedCommand(ShowHideExecute.ReverseHide);
			m_CoroutineIsShowing = StartCoroutine(IsShowing(triggerCallbacks));
		}
		else
		{
			ExecutedCommand(ShowHideExecute.Show);
			m_CoroutineIsShowing = StartCoroutine(IsShowing(triggerCallbacks));
		}
	}

	private void StartDelayedShowCoroutine(bool triggerCallbacks)
	{
		StopDelayedShowCoroutine();
		m_DelayedShowCoroutine = StartCoroutine(Coroutiner.DelayExecution(() =>
		{
			Show(triggerCallbacks);
		}, 2));
	}

	private void StopDelayedShowCoroutine()
	{
		if (m_DelayedShowCoroutine != null)
		{
			StopCoroutine(m_DelayedShowCoroutine);
			m_DelayedShowCoroutine = null;
		}
	}

	private void StopIsShowingCoroutine()
	{
		if (m_CoroutineIsShowing != null)
		{
			StopCoroutine(m_CoroutineIsShowing);
			m_CoroutineIsShowing = null;
		}
	}

	private IEnumerator IsShowing(bool triggerCallbacks)
	{
		StopIsHidingCoroutine();
		SetVisibility(VisibilityState.IsShowing, triggerCallbacks);
		yield return new WaitForEndOfFrame();
		while (anyAnimationIsActive)
		{
			yield return null;
		}
		if (hasProgressors)
		{
			while (anyProgressorIsActive)
			{
				yield return null;
			}
		}
		if (AutoSelectAfterShow && AutoSelectTarget != null)
		{
			SetSelected(AutoSelectTarget);
		}
		SetVisibility(VisibilityState.Visible, triggerCallbacks);
		m_CoroutineIsShowing = null;
	}

	public virtual void Hide()
	{
		Hide(triggerCallbacks: true);
	}

	public virtual void Hide(bool triggerCallbacks)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		StopDelayedShowCoroutine();
		StopDelayedHideCoroutine();
		if (isHiding || isHidden)
		{
			return;
		}
		if (m_LastFrameVisibilityStateChanged == Time.frameCount)
		{
			StartDelayedHideCoroutine(triggerCallbacks);
			return;
		}
		if (ClearSelectedOnHide)
		{
			SetSelected(null);
		}
		if (isShowing)
		{
			StopIsShowingCoroutine();
			ExecutedCommand(ShowHideExecute.ReverseShow);
			m_CoroutineIsHiding = StartCoroutine(IsHiding(triggerCallbacks));
		}
		else
		{
			ExecutedCommand(ShowHideExecute.Hide);
			m_CoroutineIsHiding = StartCoroutine(IsHiding(triggerCallbacks));
		}
	}

	private void StartDelayedHideCoroutine(bool triggerCallbacks)
	{
		StopDelayedHideCoroutine();
		m_DelayedHideCoroutine = StartCoroutine(Coroutiner.DelayExecution(() =>
		{
			m_DelayedHideCoroutine = null;
			Hide(triggerCallbacks);
		}, 2));
	}

	private void StopDelayedHideCoroutine()
	{
		if (m_DelayedHideCoroutine != null)
		{
			StopCoroutine(m_DelayedHideCoroutine);
			m_DelayedHideCoroutine = null;
		}
	}

	private void StopIsHidingCoroutine()
	{
		StopDisableGameObject();
		if (m_CoroutineIsHiding != null)
		{
			StopCoroutine(m_CoroutineIsHiding);
			m_CoroutineIsHiding = null;
		}
	}

	private IEnumerator IsHiding(bool triggerCallbacks)
	{
		StopDisableGameObject();
		StopIsShowingCoroutine();
		SetVisibility(VisibilityState.IsHiding, triggerCallbacks);
		yield return new WaitForEndOfFrame();
		while (anyAnimationIsActive)
		{
			yield return null;
		}
		if (hasProgressors)
		{
			while (anyProgressorIsActive)
			{
				yield return null;
			}
		}
		SetVisibility(VisibilityState.Hidden, triggerCallbacks);
		m_CoroutineIsHiding = null;
	}

	public virtual void Toggle()
	{
		Toggle(triggerCallbacks: true);
	}

	public virtual void Toggle(bool triggerCallbacks)
	{
		switch (visibilityState)
		{
		case VisibilityState.Visible:
		case VisibilityState.IsShowing:
			Hide(triggerCallbacks);
			break;
		case VisibilityState.Hidden:
		case VisibilityState.IsHiding:
			Show(triggerCallbacks);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	internal void SetVisibility(VisibilityState state, bool triggerCallbacks)
	{
		m_LastFrameVisibilityStateChanged = Time.frameCount;
		VisibilityState = state;
		if (triggerCallbacks)
		{
			OnVisibilityChangedCallback?.Invoke(VisibilityState);
		}
		switch (state)
		{
		case VisibilityState.Visible:
			ExecuteOnVisible(triggerCallbacks);
			break;
		case VisibilityState.Hidden:
			ExecuteOnHidden(triggerCallbacks);
			break;
		case VisibilityState.IsShowing:
			ExecuteOnShow(triggerCallbacks);
			break;
		case VisibilityState.IsHiding:
			ExecuteOnHide(triggerCallbacks);
			break;
		default:
			throw new ArgumentOutOfRangeException("state", state, null);
		}
	}

	private void ExecuteOnShow(bool triggerCallbacks)
	{
		if (triggerCallbacks)
		{
			OnShowCallback.Execute();
		}
	}

	private void ExecuteOnHide(bool triggerCallbacks)
	{
		if (triggerCallbacks)
		{
			OnHideCallback.Execute();
		}
		if (hasGraphicRaycaster & DisableGraphicRaycasterWhenHidden)
		{
			graphicRaycaster.enabled = false;
		}
		if (hasCanvasGroup & HandleCanvasGroupBlockRaycasts)
		{
			canvasGroup.blocksRaycasts = false;
		}
		StopAutoHide();
	}

	private void ExecuteOnVisible(bool triggerCallbacks)
	{
		if (triggerCallbacks)
		{
			OnVisibleCallback.Execute();
		}
		StartAutoHide();
	}

	private void ExecuteOnHidden(bool triggerCallbacks)
	{
		if (triggerCallbacks)
		{
			OnHiddenCallback.Execute();
		}
		if (hasCanvas & DisableCanvasWhenHidden)
		{
			canvas.enabled = false;
		}
		if (hasGraphicRaycaster & DisableGraphicRaycasterWhenHidden)
		{
			graphicRaycaster.enabled = false;
		}
		if (hasCanvasGroup & HandleCanvasGroupBlockRaycasts)
		{
			canvasGroup.blocksRaycasts = false;
		}
		StartDisableGameObject();
	}

	private void StartDisableGameObject()
	{
		StopDisableGameObject();
		m_DisableGameObjectWithDelayCoroutine = StartCoroutine(DisableGameObjectWithDelay());
	}

	private void StopDisableGameObject()
	{
		if (m_DisableGameObjectWithDelayCoroutine != null)
		{
			StopCoroutine(m_DisableGameObjectWithDelayCoroutine);
			m_DisableGameObjectWithDelayCoroutine = null;
		}
	}

	private IEnumerator DisableGameObjectWithDelay()
	{
		yield return null;
		yield return null;
		yield return null;
		base.gameObject.SetActive(!DisableGameObjectWhenHidden);
	}

	private void StartAutoHide()
	{
		StopAutoHide();
		if (AutoHideAfterShow)
		{
			m_AutoHideCoroutine = StartCoroutine(AutoHideEnumerator());
		}
	}

	private void StopAutoHide()
	{
		if (m_AutoHideCoroutine != null)
		{
			StopCoroutine(m_AutoHideCoroutine);
			m_AutoHideCoroutine = null;
		}
	}

	private IEnumerator AutoHideEnumerator()
	{
		yield return new WaitForSecondsRealtime(AutoHideAfterShowDelay);
		Hide();
		m_AutoHideCoroutine = null;
	}

	protected virtual void RunBehaviour(ContainerBehaviour behaviour)
	{
		switch (behaviour)
		{
		case ContainerBehaviour.Disabled:
			break;
		case ContainerBehaviour.InstantHide:
			VisibilityState = VisibilityState.Visible;
			InstantHide();
			break;
		case ContainerBehaviour.InstantShow:
			VisibilityState = VisibilityState.Hidden;
			InstantShow();
			break;
		case ContainerBehaviour.Hide:
			VisibilityState = VisibilityState.Visible;
			Hide();
			break;
		case ContainerBehaviour.Show:
			InstantHide(triggerCallbacks: false);
			StartCoroutine(Coroutiner.DelayExecution(Show, 2));
			break;
		default:
			throw new ArgumentOutOfRangeException("behaviour", behaviour, null);
		}
	}

	private float CalculateTotalShowDuration()
	{
		float a = CalculateTotalDurationForReactions(showReactions);
		float num = 0f;
		float num2 = 0f;
		showProgressors.RemoveNulls();
		foreach (FloatReaction item in showProgressors.Select((Progressor p) => p.reaction))
		{
			num = Mathf.Max(num, item.settings.useRandomStartDelay ? item.settings.randomStartDelay.max : item.settings.startDelay);
			num2 = Mathf.Max(num2, item.settings.useRandomDuration ? item.settings.randomDuration.max : item.settings.duration);
		}
		showHideProgressors.RemoveNulls();
		foreach (FloatReaction item2 in showHideProgressors.Select((Progressor p) => p.reaction))
		{
			num = Mathf.Max(num, item2.settings.useRandomStartDelay ? item2.settings.randomStartDelay.max : item2.settings.startDelay);
			num2 = Mathf.Max(num2, item2.settings.useRandomDuration ? item2.settings.randomDuration.max : item2.settings.duration);
		}
		return Mathf.Max(a, num + num2);
	}

	private float CalculateTotalHideDuration()
	{
		float a = CalculateTotalDurationForReactions(hideReactions);
		float num = 0f;
		float num2 = 0f;
		hideProgressors.RemoveNulls();
		foreach (FloatReaction item in hideProgressors.Select((Progressor p) => p.reaction))
		{
			num = Mathf.Max(num, item.settings.useRandomStartDelay ? item.settings.randomStartDelay.max : item.settings.startDelay);
			num2 = Mathf.Max(num2, item.settings.useRandomDuration ? item.settings.randomDuration.max : item.settings.duration);
		}
		showHideProgressors.RemoveNulls();
		foreach (FloatReaction item2 in showHideProgressors.Select((Progressor p) => p.reaction))
		{
			num2 = Mathf.Max(num2, item2.settings.useRandomDuration ? item2.settings.randomDuration.max : item2.settings.duration);
		}
		return Mathf.Max(a, num + num2);
	}

	private static float CalculateTotalDurationForReactions(IEnumerable<Reaction> reactions, params Reaction[] others)
	{
		if (reactions == null)
		{
			return 0f;
		}
		float num = 0f;
		float num2 = 0f;
		foreach (Reaction reaction2 in reactions)
		{
			if (reaction2 != null)
			{
				num = Mathf.Max(num, reaction2.settings.useRandomStartDelay ? reaction2.settings.randomStartDelay.max : reaction2.settings.startDelay);
				num2 = Mathf.Max(num2, reaction2.settings.useRandomDuration ? reaction2.settings.randomDuration.max : reaction2.settings.duration);
			}
		}
		if (others == null)
		{
			return num + num2;
		}
		foreach (Reaction reaction in others)
		{
			if (reaction != null)
			{
				num = Mathf.Max(num, reaction.settings.useRandomStartDelay ? reaction.settings.randomStartDelay.max : reaction.settings.startDelay);
				num2 = Mathf.Max(num2, reaction.settings.useRandomDuration ? reaction.settings.randomDuration.max : reaction.settings.duration);
			}
		}
		return num + num2;
	}
}
