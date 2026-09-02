using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Signals;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Components;

[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Components/UI Button")]
[SelectionBase]
public class UIButton : UISelectable, IPointerClickHandler, IEventSystemHandler, ISubmitHandler
{
	[ClearOnReload]
	private static SignalStream s_stream;

	public UIButtonId Id;

	public static HashSet<UIButton> database { get; private set; } = new HashSet<UIButton>();

	public static SignalStream stream => s_stream ?? (s_stream = SignalsService.GetStream("UISelectable", "UIButton"));

	public static IEnumerable<UIButton> availableButtons => database.Where((UIButton item) => item.isActiveAndEnabled);

	public bool isSelected => EventSystem.current.currentSelectedGameObject == base.gameObject;

	public override SelectableType selectableType => SelectableType.Button;

	[ExecuteOnReload]
	private static void OnReload()
	{
		if (database == null)
		{
			database = new HashSet<UIButton>();
		}
	}

	protected UIButton()
	{
		Id = new UIButtonId();
	}

	protected override void Awake()
	{
		database.Add(this);
		base.Awake();
	}

	protected override void OnEnable()
	{
		if (Application.isPlaying)
		{
			StopCooldown();
			database.Remove(null);
			base.OnEnable();
		}
	}

	protected override void OnDisable()
	{
		StopCooldown();
		database.Remove(null);
		base.OnDisable();
	}

	protected override void OnDestroy()
	{
		database.Remove(null);
		database.Remove(this);
		base.OnDestroy();
	}

	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (!base.inCooldown && eventData.button == PointerEventData.InputButton.Left)
		{
			Click();
		}
	}

	public void OnSubmit(BaseEventData eventData)
	{
		if (!base.inCooldown && IsActive() && IsInteractable())
		{
			DoStateTransition(SelectionState.Pressed, instant: false);
			Click();
			if (UISelectable.inputSettings.submitTriggersPointerClick)
			{
				base.behaviours.GetBehaviour(UIBehaviour.Name.PointerClick)?.Execute();
				base.behaviours.GetBehaviour(UIBehaviour.Name.PointerLeftClick)?.Execute();
			}
		}
	}

	private IEnumerator RefreshSelectionState()
	{
		float elapsedTime = 0f;
		while (elapsedTime < 0.1f)
		{
			elapsedTime += Time.unscaledDeltaTime;
			yield return null;
		}
		DoStateTransition(base.currentSelectionState, instant: false);
	}

	public void ClickWithAnimation()
	{
		OnSubmit(null);
	}

	public void Click()
	{
		Click(forced: false);
	}

	public void Click(bool forced)
	{
		if (!base.inCooldown && (forced || (IsActive() && IsInteractable())))
		{
			if (base.isActiveAndEnabled)
			{
				StartCoroutine(RefreshSelectionState());
			}
			UISystemProfilerApi.AddMarker("UIButton.Click", this);
			stream.SendSignal(new UIButtonSignalData(Id.Category, Id.Name, ButtonTrigger.Click, base.playerIndex, this));
			if (base.isActiveAndEnabled)
			{
				StartCooldown();
			}
		}
	}

	public static IEnumerable<UIButton> GetButtons(string category, string name)
	{
		return from button in database
			where button.Id.Category.Equals(category)
			where button.Id.Name.Equals(name)
			select button;
	}

	public static IEnumerable<UIButton> GetAllButtonsInCategory(string category)
	{
		return database.Where((UIButton button) => button.Id.Category.Equals(category));
	}

	public static IEnumerable<UIButton> GetAvailableButtons()
	{
		return database.Where((UIButton button) => button.isActiveAndEnabled);
	}

	public static UIButton GetSelectedButton()
	{
		return database.FirstOrDefault((UIButton button) => button.isSelected);
	}

	public static bool SelectButton(string category, string name)
	{
		UIButton uIButton = availableButtons.FirstOrDefault((UIButton b) => b.Id.Category.Equals(category) & b.Id.Name.Equals(name));
		if (uIButton == null)
		{
			return false;
		}
		uIButton.Select();
		return true;
	}
}
