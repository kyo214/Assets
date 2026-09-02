using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace MoreMountains.Tools;

public class MMDebugMenu : MonoBehaviour
{
	public enum ToggleDirections
	{
		TopToBottom = 0,
		LeftToRight = 1,
		RightToLeft = 2,
		BottomToTop = 3
	}

	[Header("Data")]
	public MMDebugMenuData Data;

	[Header("Bindings")]
	public CanvasGroup MenuContainer;

	public RectTransform Contents;

	public Image MenuBackground;

	public Image CloseIcon;

	public RectTransform TabBar;

	public RectTransform TabContainer;

	public MMDebugMenuTabManager TabManager;

	public Image MMLogo;

	[Header("Events")]
	public UnityEvent OnOpenEvent;

	public UnityEvent OnCloseEvent;

	[Header("Test")]
	[MMReadOnly]
	public bool Active;

	[MMInspectorButton("ToggleMenu")]
	public bool ToggleButton;

	protected RectTransform _containerRect;

	protected Vector3 _initialContainerPosition;

	protected Vector3 _offPosition;

	protected Vector3 _newPosition;

	protected bool _toggling;

	protected virtual void Start()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		if (Data != null)
		{
			FillMenu();
		}
		CloseIcon.color = Data.TextColor;
		_containerRect = MenuContainer.GetComponent<RectTransform>();
		_initialContainerPosition = _containerRect.localPosition;
		MenuBackground.color = Data.BackgroundColor;
		switch (Data.ToggleDirection)
		{
		case ToggleDirections.RightToLeft:
			_offPosition = _initialContainerPosition + Vector3.right * _containerRect.rect.width;
			break;
		case ToggleDirections.LeftToRight:
			_offPosition = _initialContainerPosition + Vector3.left * _containerRect.rect.width;
			break;
		case ToggleDirections.TopToBottom:
			_offPosition = _initialContainerPosition + Vector3.up * _containerRect.rect.height;
			break;
		case ToggleDirections.BottomToTop:
			_offPosition = _initialContainerPosition + Vector3.down * _containerRect.rect.height;
			break;
		}
		_containerRect.localPosition = _offPosition;
	}

	public virtual void FillMenu(bool triggerEvents = false)
	{
		int num = 0;
		if (MMLogo != null)
		{
			MMLogo.color = Data.TextColor;
		}
		foreach (Transform item in Contents.transform)
		{
			Object.Destroy(item.gameObject);
		}
		foreach (Transform item2 in TabBar.transform)
		{
			Object.Destroy(item2.gameObject);
		}
		TabManager.Tabs.Clear();
		TabManager.TabsContents.Clear();
		foreach (MMDebugMenuTabData tab in Data.Tabs)
		{
			if (tab.Active)
			{
				MMDebugMenuTab mMDebugMenuTab = Object.Instantiate(Data.TabPrefab);
				mMDebugMenuTab.SelectedBackgroundColor = Data.TextColor;
				mMDebugMenuTab.SelectedTextColor = Data.BackgroundColor;
				mMDebugMenuTab.DeselectedBackgroundColor = Data.BackgroundColor;
				mMDebugMenuTab.DeselectedTextColor = Data.TextColor;
				mMDebugMenuTab.TabText.text = tab.Name;
				mMDebugMenuTab.TabText.font = Data.RegularFont;
				mMDebugMenuTab.transform.SetParent(TabBar);
				mMDebugMenuTab.Index = num;
				mMDebugMenuTab.Manager = TabManager;
				TabManager.Tabs.Add(mMDebugMenuTab);
				MMDebugMenuTabContents mMDebugMenuTabContents = Object.Instantiate(Data.TabContentsPrefab);
				mMDebugMenuTabContents.transform.SetParent(TabContainer);
				RectTransform component = mMDebugMenuTabContents.GetComponent<RectTransform>();
				component.MMSetLeft(0f);
				component.MMSetRight(0f);
				component.MMSetTop(0f);
				component.MMSetBottom(0f);
				mMDebugMenuTabContents.Index = num;
				FillTab(mMDebugMenuTabContents, num, triggerEvents);
				if (num == Data.InitialActiveTabIndex)
				{
					mMDebugMenuTabContents.gameObject.SetActive(value: true);
					mMDebugMenuTab.Select();
				}
				else
				{
					mMDebugMenuTabContents.gameObject.SetActive(value: false);
					mMDebugMenuTab.Deselect();
				}
				TabManager.TabsContents.Add(mMDebugMenuTabContents);
				num++;
			}
		}
		if (Data.DisplayDebugTab)
		{
			MMDebugMenuTab mMDebugMenuTab2 = Object.Instantiate(Data.TabPrefab);
			mMDebugMenuTab2.SelectedBackgroundColor = Data.TextColor;
			mMDebugMenuTab2.SelectedTextColor = Data.BackgroundColor;
			mMDebugMenuTab2.DeselectedBackgroundColor = Data.BackgroundColor;
			mMDebugMenuTab2.DeselectedTextColor = Data.TextColor;
			mMDebugMenuTab2.TabText.text = Data.DebugTabName;
			mMDebugMenuTab2.TabText.font = Data.RegularFont;
			mMDebugMenuTab2.transform.SetParent(TabBar);
			mMDebugMenuTab2.Index = num;
			mMDebugMenuTab2.Manager = TabManager;
			TabManager.Tabs.Add(mMDebugMenuTab2);
			MMDebugMenuDebugTab mMDebugMenuDebugTab = Object.Instantiate(Data.DebugTabPrefab);
			mMDebugMenuDebugTab.DebugText.color = Data.TextColor;
			mMDebugMenuDebugTab.DebugText.font = Data.RegularFont;
			mMDebugMenuDebugTab.transform.SetParent(TabContainer);
			mMDebugMenuDebugTab.CommandPrompt.textComponent.font = Data.RegularFont;
			mMDebugMenuDebugTab.CommandPrompt.textComponent.color = Data.TextColor;
			mMDebugMenuDebugTab.CommandPromptCharacter.font = Data.RegularFont;
			mMDebugMenuDebugTab.CommandPromptCharacter.color = Data.TextColor;
			MMDebugMenuTabContents component2 = mMDebugMenuDebugTab.GetComponent<MMDebugMenuTabContents>();
			component2.Index = num;
			TabManager.TabsContents.Add(component2);
			RectTransform component3 = component2.GetComponent<RectTransform>();
			component3.MMSetLeft(0f);
			component3.MMSetRight(0f);
			component3.MMSetTop(0f);
			component3.MMSetBottom(0f);
			if (num == Data.InitialActiveTabIndex)
			{
				mMDebugMenuDebugTab.gameObject.SetActive(value: true);
				TabManager.Tabs[num].Select();
			}
			else
			{
				mMDebugMenuDebugTab.gameObject.SetActive(value: false);
				TabManager.Tabs[num].Deselect();
			}
			num++;
		}
		int num2 = Data.MaxTabs - num;
		for (int i = 0; i < num2; i++)
		{
			Object.Instantiate(Data.TabSpacerPrefab).transform.SetParent(TabBar);
		}
	}

	protected virtual void FillTab(MMDebugMenuTabContents tab, int index, bool triggerEvents = false)
	{
		Transform parent = tab.Parent;
		foreach (MMDebugMenuItem menuItem in Data.Tabs[index].MenuItems)
		{
			if (!menuItem.Active)
			{
				continue;
			}
			switch (menuItem.Type)
			{
			case MMDebugMenuItem.MMDebugMenuItemTypes.Button:
			{
				MMDebugMenuItemButton mMDebugMenuItemButton = ((menuItem.ButtonType == MMDebugMenuItem.MMDebugMenuItemButtonTypes.Border) ? Object.Instantiate(Data.ButtonBorderPrefab) : Object.Instantiate(Data.ButtonPrefab));
				mMDebugMenuItemButton.name = "MMDebugMenuItemButton_" + menuItem.Name;
				mMDebugMenuItemButton.ButtonText.text = menuItem.ButtonText;
				mMDebugMenuItemButton.ButtonEventName = menuItem.ButtonEventName;
				if (menuItem.ButtonType == MMDebugMenuItem.MMDebugMenuItemButtonTypes.Border)
				{
					mMDebugMenuItemButton.ButtonText.color = Data.AccentColor;
					mMDebugMenuItemButton.ButtonBg.color = Data.TextColor;
				}
				else
				{
					mMDebugMenuItemButton.ButtonText.color = Data.BackgroundColor;
					mMDebugMenuItemButton.ButtonBg.color = Data.AccentColor;
				}
				mMDebugMenuItemButton.ButtonText.font = Data.RegularFont;
				mMDebugMenuItemButton.transform.SetParent(parent);
				break;
			}
			case MMDebugMenuItem.MMDebugMenuItemTypes.Checkbox:
			{
				MMDebugMenuItemCheckbox mMDebugMenuItemCheckbox = Object.Instantiate(Data.CheckboxPrefab);
				mMDebugMenuItemCheckbox.name = "MMDebugMenuItemCheckbox_" + menuItem.Name;
				mMDebugMenuItemCheckbox.SwitchText.text = menuItem.CheckboxText;
				if (menuItem.CheckboxInitialState)
				{
					mMDebugMenuItemCheckbox.Switch.SetTrue();
				}
				else
				{
					mMDebugMenuItemCheckbox.Switch.SetFalse();
				}
				mMDebugMenuItemCheckbox.CheckboxEventName = menuItem.CheckboxEventName;
				mMDebugMenuItemCheckbox.transform.SetParent(parent);
				mMDebugMenuItemCheckbox.Switch.GetComponent<Image>().color = Data.AccentColor;
				mMDebugMenuItemCheckbox.SwitchText.color = Data.TextColor;
				mMDebugMenuItemCheckbox.SwitchText.font = Data.RegularFont;
				break;
			}
			case MMDebugMenuItem.MMDebugMenuItemTypes.Slider:
			{
				MMDebugMenuItemSlider mMDebugMenuItemSlider = Object.Instantiate(Data.SliderPrefab);
				mMDebugMenuItemSlider.name = "MMDebugMenuItemSlider_" + menuItem.Name;
				mMDebugMenuItemSlider.Mode = menuItem.SliderMode;
				mMDebugMenuItemSlider.RemapZero = menuItem.SliderRemapZero;
				mMDebugMenuItemSlider.RemapOne = menuItem.SliderRemapOne;
				mMDebugMenuItemSlider.TargetSlider.value = MMMaths.Remap(menuItem.SliderInitialValue, menuItem.SliderRemapZero, menuItem.SliderRemapOne, 0f, 1f);
				mMDebugMenuItemSlider.transform.SetParent(parent);
				mMDebugMenuItemSlider.SliderText.text = menuItem.SliderText;
				mMDebugMenuItemSlider.SliderText.color = Data.TextColor;
				mMDebugMenuItemSlider.SliderText.font = Data.RegularFont;
				mMDebugMenuItemSlider.SliderValueText.text = ((menuItem.SliderMode == MMDebugMenuItemSlider.Modes.Int) ? menuItem.SliderInitialValue.ToString() : menuItem.SliderInitialValue.ToString("F3"));
				mMDebugMenuItemSlider.SliderValueText.color = Data.AccentColor;
				mMDebugMenuItemSlider.SliderValueText.font = Data.BoldFont;
				mMDebugMenuItemSlider.SliderKnob.color = Data.AccentColor;
				mMDebugMenuItemSlider.SliderLine.color = Data.TextColor;
				mMDebugMenuItemSlider.SliderEventName = menuItem.SliderEventName;
				break;
			}
			case MMDebugMenuItem.MMDebugMenuItemTypes.Spacer:
			{
				GameObject obj = Object.Instantiate((menuItem.SpacerType == MMDebugMenuItem.MMDebugMenuItemSpacerTypes.Small) ? Data.SpacerSmallPrefab : Data.SpacerBigPrefab);
				obj.name = "MMDebugMenuItemSpacer_" + menuItem.Name;
				obj.transform.SetParent(parent);
				break;
			}
			case MMDebugMenuItem.MMDebugMenuItemTypes.Title:
			{
				MMDebugMenuItemTitle mMDebugMenuItemTitle = Object.Instantiate(Data.TitlePrefab);
				mMDebugMenuItemTitle.name = "MMDebugMenuItemSlider_" + menuItem.Name;
				mMDebugMenuItemTitle.TitleText.text = menuItem.TitleText;
				mMDebugMenuItemTitle.TitleText.color = Data.TextColor;
				mMDebugMenuItemTitle.TitleText.font = Data.BoldFont;
				mMDebugMenuItemTitle.TitleLine.color = Data.AccentColor;
				mMDebugMenuItemTitle.transform.SetParent(parent);
				break;
			}
			case MMDebugMenuItem.MMDebugMenuItemTypes.Choices:
			{
				MMDebugMenuItemChoices original = ((menuItem.ChoicesType != MMDebugMenuItem.MMDebugMenuItemChoicesTypes.TwoChoices) ? Data.ThreeChoicesPrefab : Data.TwoChoicesPrefab);
				MMDebugMenuItemChoices mMDebugMenuItemChoices = Object.Instantiate(original);
				mMDebugMenuItemChoices.name = "MMDebugMenuItemChoices_" + menuItem.Name;
				mMDebugMenuItemChoices.Choices[0].ButtonText.text = menuItem.ChoiceOneText;
				mMDebugMenuItemChoices.Choices[1].ButtonText.text = menuItem.ChoiceTwoText;
				mMDebugMenuItemChoices.Choices[0].ButtonEventName = menuItem.ChoiceOneEventName;
				mMDebugMenuItemChoices.Choices[1].ButtonEventName = menuItem.ChoiceTwoEventName;
				if (menuItem.ChoicesType == MMDebugMenuItem.MMDebugMenuItemChoicesTypes.ThreeChoices)
				{
					mMDebugMenuItemChoices.Choices[2].ButtonEventName = menuItem.ChoiceThreeEventName;
					mMDebugMenuItemChoices.Choices[2].ButtonText.text = menuItem.ChoiceThreeText;
				}
				mMDebugMenuItemChoices.OffColor = Data.BackgroundColor;
				mMDebugMenuItemChoices.OnColor = Data.TextColor;
				mMDebugMenuItemChoices.AccentColor = Data.AccentColor;
				foreach (MMDebugMenuChoiceEntry choice in mMDebugMenuItemChoices.Choices)
				{
					if (choice != null)
					{
						choice.ButtonText.font = Data.RegularFont;
					}
				}
				mMDebugMenuItemChoices.Select(menuItem.SelectedChoice);
				if (triggerEvents)
				{
					mMDebugMenuItemChoices.TriggerButtonEvent(menuItem.SelectedChoice);
				}
				mMDebugMenuItemChoices.transform.SetParent(parent);
				break;
			}
			case MMDebugMenuItem.MMDebugMenuItemTypes.Value:
			{
				MMDebugMenuItemValue mMDebugMenuItemValue = Object.Instantiate(Data.ValuePrefab);
				mMDebugMenuItemValue.name = "MMDebugMenuItemValue_" + menuItem.Name;
				mMDebugMenuItemValue.LabelText.text = menuItem.ValueLabel;
				mMDebugMenuItemValue.LabelText.color = Data.TextColor;
				mMDebugMenuItemValue.LabelText.font = Data.RegularFont;
				mMDebugMenuItemValue.ValueText.text = menuItem.ValueInitialValue;
				mMDebugMenuItemValue.ValueText.color = Data.AccentColor;
				mMDebugMenuItemValue.ValueText.font = Data.BoldFont;
				mMDebugMenuItemValue.RadioReceiver.Channel = menuItem.ValueMMRadioReceiverChannel;
				mMDebugMenuItemValue.transform.SetParent(parent);
				break;
			}
			case MMDebugMenuItem.MMDebugMenuItemTypes.Text:
			{
				MMDebugMenuItemText mMDebugMenuItemText = Object.Instantiate(menuItem.TextType switch
				{
					MMDebugMenuItem.MMDebugMenuItemTextTypes.Tiny => Data.TextTinyPrefab, 
					MMDebugMenuItem.MMDebugMenuItemTextTypes.Small => Data.TextSmallPrefab, 
					MMDebugMenuItem.MMDebugMenuItemTextTypes.Long => Data.TextLongPrefab, 
					_ => Data.TextTinyPrefab, 
				});
				mMDebugMenuItemText.name = "MMDebugMenuItemText_" + menuItem.Name;
				mMDebugMenuItemText.ContentText.text = menuItem.TextContents;
				mMDebugMenuItemText.ContentText.color = Data.TextColor;
				mMDebugMenuItemText.ContentText.font = Data.RegularFont;
				mMDebugMenuItemText.transform.SetParent(parent);
				break;
			}
			}
		}
		GameObject obj2 = Object.Instantiate(Data.SpacerBigPrefab);
		obj2.name = "MMDebugMenuItemSpacer_FinalSpacer";
		obj2.transform.SetParent(parent);
	}

	public virtual void OpenMenu()
	{
		OnOpenEvent?.Invoke();
		StartCoroutine(ToggleCo(active: false));
	}

	public virtual void CloseMenu()
	{
		StartCoroutine(ToggleCo(active: true));
	}

	public virtual void ToggleMenu()
	{
		StartCoroutine(ToggleCo(Active));
	}

	protected virtual IEnumerator ToggleCo(bool active)
	{
		if (!_toggling)
		{
			if (!active)
			{
				OnOpenEvent?.Invoke();
				_containerRect.gameObject.SetActive(value: true);
			}
			_toggling = true;
			Active = active;
			_newPosition = (active ? _offPosition : _initialContainerPosition);
			MMTween.MoveRectTransform(this, _containerRect, _containerRect.localPosition, _newPosition, null, 0f, Data.ToggleDuration, Data.ToggleCurve, ignoreTimescale: true);
			yield return MMCoroutine.WaitForUnscaled(Data.ToggleDuration);
			if (active)
			{
				OnCloseEvent?.Invoke();
				_containerRect.gameObject.SetActive(value: false);
			}
			Active = !active;
			_toggling = false;
		}
	}

	protected virtual void Update()
	{
		HandleInput();
	}

	protected virtual void HandleInput()
	{
		if (Keyboard.current[Data.ToggleKey].wasPressedThisFrame)
		{
			ToggleMenu();
		}
	}

	protected virtual void CaptureConsoleLog(string logString, string stackTrace, LogType type)
	{
		MMDebug.LogDebugToConsole(logString + " (" + type.ToString() + ")", "#00FFFF", 3, displayFrameCount: false);
	}

	protected virtual void OnEnable()
	{
		Application.logMessageReceived += CaptureConsoleLog;
	}

	protected virtual void OnDisable()
	{
		Application.logMessageReceived -= CaptureConsoleLog;
	}
}
