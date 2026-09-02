using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.UIManager.Events;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Components;

[DisallowMultipleComponent]
[AddComponentMenu("UI/Components/UISelectable")]
[SelectionBase]
public class UISelectable : Selectable, ICanvasElement, IUseMultiplayerInfo
{
	public enum SelectableType
	{
		Button = 0,
		Toggle = 1
	}

	public const string k_StreamCategory = "UISelectable";

	public const float k_DefaultAnimationDuration = 0.2f;

	[SerializeField]
	private MultiplayerInfo MultiplayerInfo;

	[SerializeField]
	internal bool IsOn;

	private static IEnumerable<UISelectionState> s_uiSelectionStates;

	private RectTransform m_RectTransform;

	[SerializeField]
	private UISelectionState CurrentUISelectionState;

	[SerializeField]
	private bool DeselectAfterPress;

	public UISelectionStateEvent OnSelectionStateChangedCallback = new UISelectionStateEvent();

	[SerializeField]
	private string CurrentStateName;

	[SerializeField]
	private UISelectableState NormalState = new UISelectableState(UISelectionState.Normal);

	[SerializeField]
	private UISelectableState HighlightedState = new UISelectableState(UISelectionState.Highlighted);

	[SerializeField]
	private UISelectableState PressedState = new UISelectableState(UISelectionState.Pressed);

	[SerializeField]
	private UISelectableState SelectedState = new UISelectableState(UISelectionState.Selected);

	[SerializeField]
	private UISelectableState DisabledState = new UISelectableState(UISelectionState.Disabled);

	[SerializeField]
	private UIBehaviours Behaviours;

	public float Cooldown;

	public bool DisableWhenInCooldown;

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

	public virtual SelectableType selectableType => SelectableType.Button;

	public bool isButton => selectableType == SelectableType.Button;

	public bool isToggle => selectableType == SelectableType.Toggle;

	public virtual bool isOn
	{
		get
		{
			return true;
		}
		set
		{
			IsOn = true;
		}
	}

	public static IEnumerable<UISelectionState> uiSelectionStates => s_uiSelectionStates ?? (s_uiSelectionStates = Enum.GetValues(typeof(UISelectionState)).Cast<UISelectionState>());

	public static UISelectable[] allUISelectablesArray => Selectable.allSelectablesArray.Where((Selectable selectable) => selectable is UISelectable).Cast<UISelectable>().ToArray();

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

	public UISelectionState currentUISelectionState => CurrentUISelectionState;

	public bool deselectAfterPress
	{
		get
		{
			return DeselectAfterPress;
		}
		set
		{
			DeselectAfterPress = value;
		}
	}

	public string currentStateName => CurrentStateName;

	public UISelectableState normalState => NormalState;

	public UISelectableState highlightedState => HighlightedState;

	public UISelectableState pressedState => PressedState;

	public UISelectableState selectedState => SelectedState;

	public UISelectableState disabledState => DisabledState;

	public UIBehaviours behaviours => Behaviours;

	public UIBehaviour onPointerEnterBehaviour => AddBehaviour(UIBehaviour.Name.PointerEnter);

	public UIBehaviour onPointerExitBehaviour => AddBehaviour(UIBehaviour.Name.PointerExit);

	public UIBehaviour onPointerDownBehaviour => AddBehaviour(UIBehaviour.Name.PointerDown);

	public UIBehaviour onPointerUpBehaviour => AddBehaviour(UIBehaviour.Name.PointerUp);

	public UIBehaviour onClickBehaviour => AddBehaviour(UIBehaviour.Name.PointerClick);

	public UIBehaviour onDoubleClickBehaviour => AddBehaviour(UIBehaviour.Name.PointerDoubleClick);

	public UIBehaviour onLongClickBehaviour => AddBehaviour(UIBehaviour.Name.PointerLongClick);

	public UIBehaviour onLeftClickBehaviour => AddBehaviour(UIBehaviour.Name.PointerLeftClick);

	public UIBehaviour onMiddleClickBehaviour => AddBehaviour(UIBehaviour.Name.PointerMiddleClick);

	public UIBehaviour onRightClickBehaviour => AddBehaviour(UIBehaviour.Name.PointerRightClick);

	public UIBehaviour onSelectedBehaviour => AddBehaviour(UIBehaviour.Name.Selected);

	public UIBehaviour onDeselectedBehaviour => AddBehaviour(UIBehaviour.Name.Deselected);

	public UIBehaviour onSubmitBehaviour => AddBehaviour(UIBehaviour.Name.Submit);

	public UnityEvent onPointerEnterEvent => onPointerEnterBehaviour.Event;

	public UnityEvent onPointerExitEvent => onPointerExitBehaviour.Event;

	public UnityEvent onPointerDownEvent => onPointerDownBehaviour.Event;

	public UnityEvent onPointerUpEvent => onPointerUpBehaviour.Event;

	public UnityEvent onClickEvent => onClickBehaviour.Event;

	public UnityEvent onDoubleClickEvent => onDoubleClickBehaviour.Event;

	public UnityEvent onLongClickEvent => onLongClickBehaviour.Event;

	public UnityEvent onLeftClickEvent => onLeftClickBehaviour.Event;

	public UnityEvent onMiddleClickEvent => onMiddleClickBehaviour.Event;

	public UnityEvent onRightClickEvent => onRightClickBehaviour.Event;

	public UnityEvent onSelectedEvent => onSelectedBehaviour.Event;

	public UnityEvent onDeselectedEvent => onDeselectedBehaviour.Event;

	public UnityEvent onSubmitEvent => onSubmitBehaviour.Event;

	private Coroutine cooldownRoutine { get; set; }

	public bool inCooldown { get; protected set; }

	private bool selectableInitialized { get; set; }

	Transform ICanvasElement.transform => base.transform;

	public void SetMultiplayerInfo(MultiplayerInfo reference)
	{
		MultiplayerInfo = reference;
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

	public UISelectable()
	{
		Behaviours = new UIBehaviours().SetSelectable(this);
	}

	protected override void Awake()
	{
		if (Application.isPlaying)
		{
			BackButton.Initialize();
		}
		base.targetGraphic = null;
		base.transition = Transition.None;
		m_RectTransform = GetComponent<RectTransform>();
		selectableInitialized = false;
		Behaviours.SetSelectable(this).SetSignalSource(base.gameObject);
		inCooldown = false;
	}

	protected override void Start()
	{
		base.Start();
		RefreshState();
	}

	protected override void OnEnable()
	{
		if (Application.isPlaying)
		{
			BackButton.Initialize();
		}
		base.OnEnable();
		if (Application.isPlaying)
		{
			if (selectableInitialized)
			{
				RefreshState();
			}
			StartCoroutine(ConnectBehaviours());
			inCooldown = false;
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		behaviours.Disconnect();
		inCooldown = false;
	}

	private IEnumerator ConnectBehaviours()
	{
		yield return null;
		if (behaviours?.behaviours != null && behaviours.behaviours.Count != 0)
		{
			behaviours.SetSelectable(this).SetSignalSource(base.gameObject).Connect();
		}
	}

	public UIBehaviour AddBehaviour(UIBehaviour.Name behaviourName)
	{
		return behaviours.AddBehaviour(behaviourName);
	}

	public void RemoveBehaviour(UIBehaviour.Name behaviourName)
	{
		behaviours.RemoveBehaviour(behaviourName);
	}

	public bool HasBehaviour(UIBehaviour.Name behaviourName)
	{
		return behaviours.HasBehaviour(behaviourName);
	}

	public UIBehaviour GetBehaviour(UIBehaviour.Name behaviourName)
	{
		return behaviours.GetBehaviour(behaviourName);
	}

	protected override void InstantClearState()
	{
		base.InstantClearState();
		if (currentUISelectionState != UISelectionState.Normal)
		{
			SetState(UISelectionState.Normal);
		}
	}

	protected override void DoStateTransition(SelectionState state, bool instant)
	{
		if (base.gameObject.activeInHierarchy && !(selectableInitialized & (currentUISelectionState == GetUISelectionState(state))))
		{
			SetState(GetUISelectionState(state));
		}
	}

	public UISelectable SetState(UISelectionState state)
	{
		selectableInitialized = true;
		if (deselectAfterPress && CurrentUISelectionState == UISelectionState.Pressed && state == UISelectionState.Selected)
		{
			EventSystem.current.SetSelectedGameObject(null);
			state = UISelectionState.Normal;
		}
		OnSelectionStateChangedCallback?.Invoke(state);
		CurrentUISelectionState = state;
		CurrentStateName = state.ToString();
		GetUISelectableState(state).stateEvent.Execute();
		return this;
	}

	public UISelectable RefreshState()
	{
		return SetState(currentUISelectionState);
	}

	public UISelectableState GetUISelectableState(UISelectionState state)
	{
		return state switch
		{
			UISelectionState.Normal => normalState, 
			UISelectionState.Highlighted => highlightedState, 
			UISelectionState.Pressed => pressedState, 
			UISelectionState.Selected => selectedState, 
			UISelectionState.Disabled => disabledState, 
			_ => throw new ArgumentOutOfRangeException("state", state, null), 
		};
	}

	public UISelectableState GetCurrentUISelectableState()
	{
		return GetUISelectableState(currentUISelectionState);
	}

	private static UISelectionState GetUISelectionState(SelectionState selectionState)
	{
		return selectionState switch
		{
			SelectionState.Normal => UISelectionState.Normal, 
			SelectionState.Highlighted => UISelectionState.Highlighted, 
			SelectionState.Pressed => UISelectionState.Pressed, 
			SelectionState.Selected => UISelectionState.Selected, 
			SelectionState.Disabled => UISelectionState.Disabled, 
			_ => throw new ArgumentOutOfRangeException("selectionState", selectionState, null), 
		};
	}

	protected void StartCooldown()
	{
		StopCooldown();
		if (!(Cooldown <= 0f))
		{
			cooldownRoutine = StartCoroutine(CooldownRoutine());
		}
	}

	protected void StopCooldown()
	{
		inCooldown = false;
		if (DisableWhenInCooldown)
		{
			base.interactable = true;
		}
		if (cooldownRoutine != null)
		{
			StopCoroutine(cooldownRoutine);
			cooldownRoutine = null;
		}
	}

	protected IEnumerator CooldownRoutine()
	{
		if (DisableWhenInCooldown)
		{
			base.interactable = false;
		}
		yield return new WaitForEndOfFrame();
		inCooldown = true;
		yield return new WaitForSecondsRealtime(Cooldown);
		if (DisableWhenInCooldown)
		{
			base.interactable = true;
		}
		inCooldown = false;
		cooldownRoutine = null;
	}
}
