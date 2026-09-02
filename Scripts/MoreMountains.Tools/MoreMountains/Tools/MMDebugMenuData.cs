using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MoreMountains.Tools;

[CreateAssetMenu(fileName = "MMDebugMenuData", menuName = "MoreMountains/MMDebugMenu/MMDebugMenuData")]
public class MMDebugMenuData : ScriptableObject
{
	[Header("Prefabs")]
	public MMDebugMenuItemTitle TitlePrefab;

	public MMDebugMenuItemButton ButtonPrefab;

	public MMDebugMenuItemButton ButtonBorderPrefab;

	public MMDebugMenuItemCheckbox CheckboxPrefab;

	public MMDebugMenuItemSlider SliderPrefab;

	public GameObject SpacerSmallPrefab;

	public GameObject SpacerBigPrefab;

	public MMDebugMenuItemText TextTinyPrefab;

	public MMDebugMenuItemText TextSmallPrefab;

	public MMDebugMenuItemText TextLongPrefab;

	public MMDebugMenuItemValue ValuePrefab;

	public MMDebugMenuItemChoices TwoChoicesPrefab;

	public MMDebugMenuItemChoices ThreeChoicesPrefab;

	public MMDebugMenuTab TabPrefab;

	public MMDebugMenuTabContents TabContentsPrefab;

	public RectTransform TabSpacerPrefab;

	public MMDebugMenuDebugTab DebugTabPrefab;

	public string DebugTabName = "Logs";

	[Header("Tabs")]
	public List<MMDebugMenuTabData> Tabs;

	public bool DisplayDebugTab = true;

	public int MaxTabs = 5;

	public int InitialActiveTabIndex;

	[Header("Toggle")]
	public MMDebugMenu.ToggleDirections ToggleDirection = MMDebugMenu.ToggleDirections.RightToLeft;

	public float ToggleDuration = 0.2f;

	public MMTween.MMTweenCurve ToggleCurve = MMTween.MMTweenCurve.EaseInCubic;

	public Key ToggleKey = Key.Backquote;

	[Header("Style")]
	public Font RegularFont;

	public Font BoldFont;

	public Color BackgroundColor = Color.black;

	public Color AccentColor = MMColors.ReunoYellow;

	public Color TextColor = Color.white;
}
