using System.Collections.Generic;
using Doozy.Runtime.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Runtime.UIElements.Extensions;

public static class VisualElementExtensions
{
	public const float SPACING = 4f;

	public static T AddChild<T>(this T target, VisualElement child) where T : VisualElement
	{
		target.Add(child);
		return target;
	}

	public static T RemoveAllChildren<T>(this T target) where T : VisualElement
	{
		foreach (VisualElement item in new List<VisualElement>(target.Children()))
		{
			item.RemoveFromHierarchy();
		}
		return target;
	}

	public static T AddTemplateContainer<T>(this T target, string layoutPath, float flexGrow = 1f) where T : VisualElement
	{
		Debugger.LogWarning("This method works only in the Editor");
		return target;
	}

	public static T AddEndOfLineSpace<T>(this T target, int multiplier = 1) where T : VisualElement
	{
		return target.AddChild(new VisualElement().SetStyleFlexShrink(0f).SetName("EndSpace").SetStyleHeight(24f * (float)multiplier));
	}

	public static T AddSpaceBlock<T>(this T target, int multiplier = 1) where T : VisualElement
	{
		return target.AddChild(new VisualElement().SetStyleFlexShrink(0f).SetName("SpaceBlock").SetStyleHeight(4f * (float)multiplier));
	}

	public static T AddSpace<T>(this T target, float width, float height) where T : VisualElement
	{
		return target.AddChild(new VisualElement().SetStyleFlexShrink(0f).SetName("Space").SetStyleSize(width, height));
	}

	public static T AddSpace<T>(this T target, float space) where T : VisualElement
	{
		return target.AddSpace(space, space);
	}

	public static T AddHorizontalSpace<T>(this T target, float height) where T : VisualElement
	{
		return target.AddChild(new VisualElement().SetStyleFlexShrink(0f).SetName("HSpace").SetStyleHeight(height));
	}

	public static T AddVerticalSpace<T>(this T target, float width) where T : VisualElement
	{
		return target.AddChild(new VisualElement().SetStyleFlexShrink(0f).SetName("VSpace").SetStyleWidth(width));
	}

	public static T AddFlexibleSpace<T>(this T target) where T : VisualElement
	{
		return target.AddChild(new VisualElement().SetStyleFlexGrow(1f).SetName("FlexibleSpace"));
	}

	public static T AddCurrentThemeClass<T>(this T target) where T : VisualElement
	{
		Debugger.LogWarning("This method works only in the Editor");
		return target;
	}

	public static T AddClass<T>(this T target, string className) where T : VisualElement
	{
		if (!target.ClassListContains(className))
		{
			target.AddToClassList(className);
		}
		return target;
	}

	public static T RemoveClass<T>(this T target, string className) where T : VisualElement
	{
		if (target.ClassListContains(className))
		{
			target.RemoveFromClassList(className);
		}
		return target;
	}

	public static T AddStyle<T>(this T target, StyleSheet styleSheet) where T : VisualElement
	{
		if (styleSheet == null)
		{
			return target;
		}
		target.styleSheets.Add(styleSheet);
		return target;
	}

	public static T AddStyle<T>(this T target, string styleSheetPath) where T : VisualElement
	{
		Debugger.LogWarning("This method works only in the Editor");
		return target;
	}

	public static T RemoveStyle<T>(this T target, StyleSheet styleSheet) where T : VisualElement
	{
		if (target.styleSheets.Contains(styleSheet))
		{
			target.styleSheets.Remove(styleSheet);
		}
		return target;
	}

	public static T RemoveStyle<T>(this T target, string styleSheetName) where T : VisualElement
	{
		StyleSheet styleSheet = null;
		for (int i = 0; i < target.styleSheets.count; i++)
		{
			if (target.styleSheets[i].name.Equals(styleSheetName))
			{
				styleSheet = target.styleSheets[i];
				break;
			}
		}
		if (styleSheet != null)
		{
			target.styleSheets.Remove(styleSheet);
		}
		return target;
	}

	public static T SetName<T>(this T target, string value) where T : VisualElement
	{
		target.name = value;
		return target;
	}

	public static string GetName<T>(this T target) where T : VisualElement
	{
		return target.name;
	}

	public static T SetTooltip<T>(this T target, string value) where T : VisualElement
	{
		target.tooltip = value;
		return target;
	}

	public static string GetTooltip<T>(this T target) where T : VisualElement
	{
		return target.tooltip;
	}

	public static T SetPickingMode<T>(this T target, PickingMode value) where T : VisualElement
	{
		target.pickingMode = value;
		return target;
	}

	public static PickingMode GetPickingMode<T>(this T target) where T : VisualElement
	{
		return target.pickingMode;
	}

	public static T SetStylePosition<T>(this T target, Position value) where T : VisualElement
	{
		target.style.position = new StyleEnum<Position>(value);
		return target;
	}

	public static Position GetStylePosition<T>(this T target) where T : VisualElement
	{
		return target.style.position.value;
	}

	public static T SetStyleOverflow<T>(this T target, Overflow value) where T : VisualElement
	{
		target.style.overflow = new StyleEnum<Overflow>(value);
		return target;
	}

	public static Overflow GetStyleOverflow<T>(this T target) where T : VisualElement
	{
		return target.style.overflow.value;
	}

	public static T SetStyleAlignSelf<T>(this T target, Align value) where T : VisualElement
	{
		target.style.alignSelf = new StyleEnum<Align>(value);
		return target;
	}

	public static Align GetStyleAlignSelf<T>(this T target) where T : VisualElement
	{
		return target.style.alignSelf.value;
	}

	public static T SetStyleAlignItems<T>(this T target, Align value) where T : VisualElement
	{
		target.style.alignItems = new StyleEnum<Align>(value);
		return target;
	}

	public static Align GetStyleAlignItems<T>(this T target) where T : VisualElement
	{
		return target.style.alignItems.value;
	}

	public static T SetStyleAlignContent<T>(this T target, Align value) where T : VisualElement
	{
		target.style.alignContent = new StyleEnum<Align>(value);
		return target;
	}

	public static Align GetStyleAlignContent<T>(this T target) where T : VisualElement
	{
		return target.style.alignContent.value;
	}

	public static T SetStyleJustifyContent<T>(this T target, Justify value) where T : VisualElement
	{
		target.style.justifyContent = new StyleEnum<Justify>(value);
		return target;
	}

	public static Justify GetStyleJustifyContent<T>(this T target) where T : VisualElement
	{
		return target.style.justifyContent.value;
	}

	public static T SetStyleFlexDirection<T>(this T target, FlexDirection value) where T : VisualElement
	{
		target.style.flexDirection = new StyleEnum<FlexDirection>(value);
		return target;
	}

	public static FlexDirection GetStyleFlexDirection<T>(this T target) where T : VisualElement
	{
		return target.style.flexDirection.value;
	}

	public static T SetStyleFlexDirectionKeyword<T>(this T target, StyleKeyword value) where T : VisualElement
	{
		target.style.flexDirection = new StyleEnum<FlexDirection>(value);
		return target;
	}

	public static StyleKeyword GetStyleFlexDirectionKeyword<T>(this T target) where T : VisualElement
	{
		return target.style.flexDirection.keyword;
	}

	public static T SetStyleFlexGrow<T>(this T target, float value) where T : VisualElement
	{
		target.style.flexGrow = value;
		return target;
	}

	public static float GetStyleFlexGrow<T>(this T target) where T : VisualElement
	{
		return target.style.flexGrow.value;
	}

	public static T SetStyleFlexGrowKeyword<T>(this T target, StyleKeyword value) where T : VisualElement
	{
		target.style.flexGrow = new StyleFloat(value);
		return target;
	}

	public static StyleKeyword GetStyleFlexGrowKeyword<T>(this T target) where T : VisualElement
	{
		return target.style.flexGrow.keyword;
	}

	public static T SetStyleFlexShrink<T>(this T target, float value) where T : VisualElement
	{
		target.style.flexShrink = value;
		return target;
	}

	public static float GetStyleFlexShrink<T>(this T target) where T : VisualElement
	{
		return target.style.flexShrink.value;
	}

	public static T SetStyleFlexShrinkKeyword<T>(this T target, StyleKeyword value) where T : VisualElement
	{
		target.style.flexShrink = new StyleFloat(value);
		return target;
	}

	public static StyleKeyword GetStyleFlexShrinkKeyword<T>(this T target) where T : VisualElement
	{
		return target.style.flexShrink.keyword;
	}

	public static T SetStyleFlexWrap<T>(this T target, Wrap value) where T : VisualElement
	{
		target.style.flexWrap = value;
		return target;
	}

	public static Wrap GetStyleFlexWrap<T>(this T target) where T : VisualElement
	{
		return target.style.flexWrap.value;
	}

	public static T SetStyleFlexBasis<T>(this T target, float value) where T : VisualElement
	{
		target.style.flexBasis = value;
		return target;
	}

	public static float GetStyleFlexBasis<T>(this T target) where T : VisualElement
	{
		return target.style.flexBasis.value.value;
	}

	public static T SetStyleFlexBasisStyleLength<T>(this T target, StyleLength value) where T : VisualElement
	{
		target.style.flexBasis = value;
		return target;
	}

	public static StyleKeyword GetStyleFlexBasisKeyword<T>(this T target) where T : VisualElement
	{
		return target.style.flexBasis.keyword;
	}

	public static T SetStyleBorderColor<T>(this T target, Color left, Color top, Color right, Color bottom) where T : VisualElement
	{
		target.style.borderLeftColor = left;
		target.style.borderTopColor = top;
		target.style.borderRightColor = right;
		target.style.borderBottomColor = bottom;
		return target;
	}

	public static T SetStyleBorderColor<T>(this T target, Color value) where T : VisualElement
	{
		return target.SetStyleBorderColor(value, value, value, value);
	}

	public static T SetStyleBorderLeftColor<T>(this T target, Color value) where T : VisualElement
	{
		target.style.borderLeftColor = value;
		return target;
	}

	public static Color GetStyleBorderLeftColor<T>(this T target) where T : VisualElement
	{
		return target.resolvedStyle.borderLeftColor;
	}

	public static T SetStyleBorderTopColor<T>(this T target, Color value) where T : VisualElement
	{
		target.style.borderTopColor = value;
		return target;
	}

	public static Color GetStyleBorderTopColor<T>(this T target) where T : VisualElement
	{
		return target.resolvedStyle.borderTopColor;
	}

	public static T SetStyleBorderRightColor<T>(this T target, Color value) where T : VisualElement
	{
		target.style.borderRightColor = value;
		return target;
	}

	public static Color GetStyleBorderRightColor<T>(this T target) where T : VisualElement
	{
		return target.resolvedStyle.borderRightColor;
	}

	public static T SetStyleBorderBottomColor<T>(this T target, Color value) where T : VisualElement
	{
		target.style.borderBottomColor = value;
		return target;
	}

	public static Color GetStyleBorderBottomColor<T>(this T target) where T : VisualElement
	{
		return target.resolvedStyle.borderBottomColor;
	}

	public static T SetStyleBorderWidth<T>(this T target, float left, float top, float right, float bottom) where T : VisualElement
	{
		target.style.borderLeftWidth = left;
		target.style.borderTopWidth = top;
		target.style.borderRightWidth = right;
		target.style.borderBottomWidth = bottom;
		return target;
	}

	public static T SetStyleBorderWidth<T>(this T target, float value) where T : VisualElement
	{
		return target.SetStyleBorderWidth(value, value, value, value);
	}

	public static T SetStyleBorderWidth<T>(this T target, EdgeValues edge) where T : VisualElement
	{
		return target.SetStyleBorderWidth(edge.Left, edge.Top, edge.Right, edge.Bottom);
	}

	public static T SetStyleBorderLeftWidth<T>(this T target, float value) where T : VisualElement
	{
		target.style.borderLeftWidth = value;
		return target;
	}

	public static float GetStyleBorderLeftWidth<T>(this T target) where T : VisualElement
	{
		return target.style.borderLeftWidth.value;
	}

	public static T SetStyleBorderTopWidth<T>(this T target, float value) where T : VisualElement
	{
		target.style.borderTopWidth = value;
		return target;
	}

	public static float GetStyleBorderTopWidth<T>(this T target) where T : VisualElement
	{
		return target.style.borderTopWidth.value;
	}

	public static T SetStyleBorderRightWidth<T>(this T target, float value) where T : VisualElement
	{
		target.style.borderRightWidth = value;
		return target;
	}

	public static float GetStyleBorderRightWidth<T>(this T target) where T : VisualElement
	{
		return target.style.borderRightWidth.value;
	}

	public static T SetStyleBorderBottomWidth<T>(this T target, float value) where T : VisualElement
	{
		target.style.borderBottomWidth = value;
		return target;
	}

	public static float GetStyleBorderBottomWidth<T>(this T target) where T : VisualElement
	{
		return target.style.borderBottomWidth.value;
	}

	public static T SetStyleBorderRadius<T>(this T target, float topLeft, float topRight, float bottomRight, float bottomLeft) where T : VisualElement
	{
		target.style.borderTopLeftRadius = topLeft;
		target.style.borderTopRightRadius = topRight;
		target.style.borderBottomRightRadius = bottomRight;
		target.style.borderBottomLeftRadius = bottomLeft;
		return target;
	}

	public static T SetStyleBorderRadius<T>(this T target, float value) where T : VisualElement
	{
		return target.SetStyleBorderRadius(value, value, value, value);
	}

	public static T SetStyleBorderRadius<T>(this T target, EdgeValues edge) where T : VisualElement
	{
		return target.SetStyleBorderRadius(edge.Left, edge.Top, edge.Right, edge.Bottom);
	}

	public static T SetStyleBorderTopLeftRadius<T>(this T target, float value) where T : VisualElement
	{
		target.style.borderTopLeftRadius = value;
		return target;
	}

	public static float GetStyleBorderTopLeftRadius<T>(this T target) where T : VisualElement
	{
		return target.style.borderTopLeftRadius.value.value;
	}

	public static T SetStyleBorderTopRightRadius<T>(this T target, float value) where T : VisualElement
	{
		target.style.borderTopRightRadius = value;
		return target;
	}

	public static float GetStyleBorderTopRightRadius<T>(this T target) where T : VisualElement
	{
		return target.style.borderTopRightRadius.value.value;
	}

	public static T SetStyleBorderBottomRightRadius<T>(this T target, float value) where T : VisualElement
	{
		target.style.borderBottomRightRadius = value;
		return target;
	}

	public static float GetStyleBorderBottomRightRadius<T>(this T target) where T : VisualElement
	{
		return target.style.borderBottomRightRadius.value.value;
	}

	public static T SetStyleBorderBottomLeftRadius<T>(this T target, float value) where T : VisualElement
	{
		target.style.borderBottomLeftRadius = value;
		return target;
	}

	public static float GetStyleBorderBottomLeftRadius<T>(this T target) where T : VisualElement
	{
		return target.style.borderBottomLeftRadius.value.value;
	}

	public static T SetStyleBackgroundColor<T>(this T target, Color color) where T : VisualElement
	{
		target.style.backgroundColor = color;
		return target;
	}

	public static Color GetStyleBackgroundColor<T>(this T target) where T : VisualElement
	{
		return target.resolvedStyle.backgroundColor;
	}

	public static T SetStyleOpacity<T>(this T target, float value) where T : VisualElement
	{
		target.style.opacity = value;
		return target;
	}

	public static float GetStyleOpacity<T>(this T target) where T : VisualElement
	{
		return target.style.opacity.value;
	}

	public static T SetStyleHeight<T>(this T target, float height) where T : VisualElement
	{
		target.style.height = height;
		return target;
	}

	public static T SetStyleHeight<T>(this T target, StyleKeyword styleKeyword) where T : VisualElement
	{
		target.style.height = new StyleLength(styleKeyword);
		return target;
	}

	public static T ResetStyleHeight<T>(this T target) where T : VisualElement
	{
		return target.SetStyleHeight(StyleKeyword.Auto);
	}

	public static float GetStyleHeight<T>(this T target) where T : VisualElement
	{
		return target.style.height.value.value;
	}

	public static T SetStyleHeight<T>(this T target, float minHeight, float height, float maxHeight) where T : VisualElement
	{
		return target.SetStyleMinHeight(minHeight).SetStyleHeight(height).SetStyleMaxHeight(maxHeight);
	}

	public static T SetStyleMinHeight<T>(this T target, float height) where T : VisualElement
	{
		target.style.minHeight = height;
		return target;
	}

	public static T SetStyleMinHeight<T>(this T target, StyleKeyword styleKeyword) where T : VisualElement
	{
		target.style.minHeight = new StyleLength(styleKeyword);
		return target;
	}

	public static T ResetStyleMinHeight<T>(this T target) where T : VisualElement
	{
		return target.SetStyleMinHeight(StyleKeyword.Auto);
	}

	public static float GetStyleMinHeight<T>(this T target) where T : VisualElement
	{
		return target.style.minHeight.value.value;
	}

	public static T SetStyleMaxHeight<T>(this T target, float height) where T : VisualElement
	{
		target.style.maxHeight = height;
		return target;
	}

	public static T SetStyleMaxHeight<T>(this T target, StyleKeyword styleKeyword) where T : VisualElement
	{
		target.style.maxHeight = new StyleLength(styleKeyword);
		return target;
	}

	public static T ResetStyleMaxHeight<T>(this T target) where T : VisualElement
	{
		return target.SetStyleMaxHeight(StyleKeyword.Auto);
	}

	public static float GetStyleMaxHeight<T>(this T target) where T : VisualElement
	{
		return target.style.maxHeight.value.value;
	}

	public static T SetStyleWidth<T>(this T target, float width) where T : VisualElement
	{
		target.style.width = width;
		return target;
	}

	public static T SetStyleWidth<T>(this T target, StyleKeyword styleKeyword) where T : VisualElement
	{
		target.style.width = new StyleLength(styleKeyword);
		return target;
	}

	public static T ResetStyleWidth<T>(this T target) where T : VisualElement
	{
		return target.SetStyleWidth(StyleKeyword.Auto);
	}

	public static float GetStyleWidth<T>(this T target) where T : VisualElement
	{
		return target.style.width.value.value;
	}

	public static T SetStyleWidth<T>(this T target, float minWidth, float width, float maxWidth) where T : VisualElement
	{
		return target.SetStyleMinWidth(minWidth).SetStyleWidth(width).SetStyleMaxWidth(maxWidth);
	}

	public static T SetStyleMinWidth<T>(this T target, float width) where T : VisualElement
	{
		target.style.minWidth = width;
		return target;
	}

	public static T SetStyleMinWidth<T>(this T target, StyleKeyword styleKeyword) where T : VisualElement
	{
		target.style.minWidth = new StyleLength(styleKeyword);
		return target;
	}

	public static T ResetStyleMinWidth<T>(this T target) where T : VisualElement
	{
		return target.SetStyleMinWidth(StyleKeyword.Auto);
	}

	public static float GetStyleMinWidth<T>(this T target) where T : VisualElement
	{
		return target.style.minWidth.value.value;
	}

	public static T SetStyleMaxWidth<T>(this T target, float width) where T : VisualElement
	{
		target.style.maxWidth = width;
		return target;
	}

	public static T SetStyleMaxWidth<T>(this T target, StyleKeyword styleKeyword) where T : VisualElement
	{
		target.style.maxWidth = new StyleLength(styleKeyword);
		return target;
	}

	public static T ResetStyleMaxWidth<T>(this T target) where T : VisualElement
	{
		return target.SetStyleMaxWidth(StyleKeyword.Auto);
	}

	public static float GetStyleMaxWidth<T>(this T target) where T : VisualElement
	{
		return target.style.maxWidth.value.value;
	}

	public static T SetStyleSize<T>(this T target, float width, float height) where T : VisualElement
	{
		return target.SetStyleWidth(width).SetStyleHeight(height);
	}

	public static T SetStyleSize<T>(this T target, float value) where T : VisualElement
	{
		return target.SetStyleSize(value, value);
	}

	public static T SetStyleSize<T>(this T target, StyleKeyword styleKeyword) where T : VisualElement
	{
		target.SetStyleWidth(styleKeyword);
		target.SetStyleHeight(styleKeyword);
		return target;
	}

	public static T ResetStyleSize<T>(this T target) where T : VisualElement
	{
		return target.SetStyleSize(StyleKeyword.Auto);
	}

	public static T SetStyleMinSize<T>(this T target, float width, float height) where T : VisualElement
	{
		return target.SetStyleMinWidth(width).SetStyleMinHeight(height);
	}

	public static T SetStyleMinSize<T>(this T target, float value) where T : VisualElement
	{
		return target.SetStyleMinWidth(value).SetStyleMinHeight(value);
	}

	public static T SetStyleMinSize<T>(this T target, StyleKeyword styleKeyword) where T : VisualElement
	{
		target.SetStyleMinWidth(styleKeyword);
		target.SetStyleMinHeight(styleKeyword);
		return target;
	}

	public static T ResetStyleMinSize<T>(this T target) where T : VisualElement
	{
		return target.SetStyleMinSize(StyleKeyword.Auto);
	}

	public static T StyleMaxSize<T>(this T target, float width, float height) where T : VisualElement
	{
		return target.SetStyleMaxWidth(width).SetStyleMaxHeight(height);
	}

	public static T StyleMaxSize<T>(this T target, float value) where T : VisualElement
	{
		return target.SetStyleMaxWidth(value).SetStyleMaxHeight(value);
	}

	public static T SetStyleMaxSize<T>(this T target, StyleKeyword styleKeyword) where T : VisualElement
	{
		target.SetStyleMaxWidth(styleKeyword);
		target.SetStyleMaxHeight(styleKeyword);
		return target;
	}

	public static T ResetStyleMaxSize<T>(this T target) where T : VisualElement
	{
		return target.SetStyleMaxSize(StyleKeyword.Auto);
	}

	public static T SetStyleBackgroundImage<T>(this T target, Texture2D value) where T : VisualElement
	{
		target.style.backgroundImage = new StyleBackground(value);
		return target;
	}

	public static Texture2D GetStyleBackgroundImageTexture2D<T>(this T target) where T : VisualElement
	{
		return target.style.backgroundImage.value.texture;
	}

	public static T SetStyleBackgroundImage<T>(this T target, VectorImage value) where T : VisualElement
	{
		target.style.backgroundImage = new StyleBackground(value);
		return target;
	}

	public static VectorImage GetStyleBackgroundImageVectorImage<T>(this T target) where T : VisualElement
	{
		return target.style.backgroundImage.value.vectorImage;
	}

	public static T SetStyleBackgroundImage<T>(this T target, Background value) where T : VisualElement
	{
		target.style.backgroundImage = new StyleBackground(value);
		return target;
	}

	public static Background GetStyleBackgroundImageBackground<T>(this T target) where T : VisualElement
	{
		return target.style.backgroundImage.value;
	}

	public static T SetStyleBackgroundImage<T>(this T target, StyleKeyword value) where T : VisualElement
	{
		target.style.backgroundImage = new StyleBackground(value);
		return target;
	}

	public static StyleKeyword GetStyleBackgroundImageStyleKeyword<T>(this T target) where T : VisualElement
	{
		return target.style.backgroundImage.keyword;
	}

	public static T SetStyleBackgroundImage<T>(this T target, string texturePath) where T : VisualElement
	{
		Debugger.LogWarning("This method works only in the Editor");
		return target;
	}

	public static string GetStyleBackgroundImagePath<T>(this T target) where T : VisualElement
	{
		return "This method works only in the Editor";
	}

	public static T SetStyleBackgroundImageTintColor<T>(this T target, Color value) where T : VisualElement
	{
		target.style.unityBackgroundImageTintColor = new StyleColor(value);
		return target;
	}

	public static Color GetStyleBackgroundImageTintColor<T>(this T target) where T : VisualElement
	{
		return target.resolvedStyle.unityBackgroundImageTintColor;
	}

	public static T SetStyleBackgroundImageTintColor<T>(this T target, StyleKeyword value) where T : VisualElement
	{
		target.style.unityBackgroundImageTintColor = new StyleColor(value);
		return target;
	}

	public static StyleKeyword GetStyleBackgroundImageTintColorStyleKeyword<T>(this T target) where T : VisualElement
	{
		return target.style.unityBackgroundImageTintColor.keyword;
	}

	public static T SetStyleBackgroundScaleMode<T>(this T target, ScaleMode value) where T : VisualElement
	{
		target.style.unityBackgroundScaleMode = new StyleEnum<ScaleMode>(value);
		return target;
	}

	public static ScaleMode GetStyleBackgroundScaleMode<T>(this T target) where T : VisualElement
	{
		return target.style.unityBackgroundScaleMode.value;
	}

	public static T SetStyleUnitySlice<T>(this T target, int left, int top, int right, int bottom) where T : VisualElement
	{
		target.style.unitySliceLeft = left;
		target.style.unitySliceTop = top;
		target.style.unitySliceRight = right;
		target.style.unitySliceBottom = bottom;
		return target;
	}

	public static T SetStyleUnitySlice<T>(this T target, int value) where T : VisualElement
	{
		return target.SetStyleUnitySlice(value, value, value, value);
	}

	public static T SetStyleUnitySlice<T>(this T target, EdgeValues edge) where T : VisualElement
	{
		return target.SetStyleUnitySlice((int)edge.Left, (int)edge.Top, (int)edge.Right, (int)edge.Bottom);
	}

	public static T SetStyleUnitySliceLeft<T>(this T target, int value) where T : VisualElement
	{
		target.style.unitySliceLeft = value;
		return target;
	}

	public static int GetStyleUnitySliceLeft<T>(this T target) where T : VisualElement
	{
		return target.style.unitySliceLeft.value;
	}

	public static T SetStyleUnitySliceTop<T>(this T target, int value) where T : VisualElement
	{
		target.style.unitySliceTop = value;
		return target;
	}

	public static int GetStyleUnitySliceTop<T>(this T target) where T : VisualElement
	{
		return target.style.unitySliceTop.value;
	}

	public static T SetStyleUnitySliceRight<T>(this T target, int value) where T : VisualElement
	{
		target.style.unitySliceRight = value;
		return target;
	}

	public static int GetStyleUnitySliceRight<T>(this T target) where T : VisualElement
	{
		return target.style.unitySliceRight.value;
	}

	public static T SetStyleUnitySliceBottom<T>(this T target, int value) where T : VisualElement
	{
		target.style.unitySliceBottom = value;
		return target;
	}

	public static int GetStyleUnitySliceBottom<T>(this T target) where T : VisualElement
	{
		return target.style.unitySliceBottom.value;
	}

	public static T SetStyleColor<T>(this T target, Color value) where T : VisualElement
	{
		target.style.color = value;
		return target;
	}

	public static Color GetStyleColor<T>(this T target) where T : VisualElement
	{
		return target.resolvedStyle.color;
	}

	public static T SetWhiteSpace<T>(this T target, WhiteSpace value) where T : VisualElement
	{
		target.style.whiteSpace = value;
		return target;
	}

	public static WhiteSpace GetWhiteSpace<T>(this T target) where T : VisualElement
	{
		return target.style.whiteSpace.value;
	}

	public static T SetStyleUnityFont<T>(this T target, Font value) where T : VisualElement
	{
		target.style.unityFont = value;
		return target;
	}

	public static Font GetStyleUnityFont<T>(this T target) where T : VisualElement
	{
		return target.style.unityFont.value;
	}

	public static T SetStyleUnityFontStyleAndWeight<T>(this T target, FontStyle value) where T : VisualElement
	{
		target.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(value);
		return target;
	}

	public static FontStyle GetStyleUnityFontStyleAndWeight<T>(this T target) where T : VisualElement
	{
		return target.style.unityFontStyleAndWeight.value;
	}

	public static T SetStyleFontSize<T>(this T target, int value) where T : VisualElement
	{
		target.style.fontSize = value;
		return target;
	}

	public static float GetStyleFontSize<T>(this T target) where T : VisualElement
	{
		return target.style.fontSize.value.value;
	}

	public static T SetStyleTextAlign<T>(this T target, TextAnchor value) where T : VisualElement
	{
		target.style.unityTextAlign = value;
		return target;
	}

	public static TextAnchor GetStyleTextAlign<T>(this T target) where T : VisualElement
	{
		return target.style.unityTextAlign.value;
	}

	public static T SetStyleCursor<T>(this T target, Texture2D texture, Vector2 hotspot) where T : VisualElement
	{
		target.style.cursor = new UnityEngine.UIElements.Cursor
		{
			texture = texture,
			hotspot = hotspot
		};
		return target;
	}

	public static T SetStyleCursor<T>(this T target, Texture2D texture) where T : VisualElement
	{
		target.style.cursor = new UnityEngine.UIElements.Cursor
		{
			texture = texture,
			hotspot = new Vector2((float)texture.width / 2f, (float)texture.height / 2f)
		};
		return target;
	}

	public static T SetStyleCursor<T>(this T target, UnityEngine.UIElements.Cursor cursor) where T : VisualElement
	{
		target.style.cursor = new StyleCursor(cursor);
		return target;
	}

	public static T SetStyleCursor<T>(this T target, StyleKeyword keyword) where T : VisualElement
	{
		target.style.cursor = new StyleCursor(keyword);
		return target;
	}

	public static T ClearMargins<T>(this T target) where T : VisualElement
	{
		return target.SetStyleMargins(0f);
	}

	public static T SetStyleMargins<T>(this T target, float left, float top, float right, float bottom) where T : VisualElement
	{
		target.style.marginLeft = left;
		target.style.marginTop = top;
		target.style.marginRight = right;
		target.style.marginBottom = bottom;
		return target;
	}

	public static T SetStyleMargins<T>(this T target, float value) where T : VisualElement
	{
		return target.SetStyleMargins(value, value, value, value);
	}

	public static T SetStyleMargins<T>(this T target, EdgeValues edge) where T : VisualElement
	{
		return target.SetStyleMargins(edge.Left, edge.Top, edge.Right, edge.Bottom);
	}

	public static T SetStyleMarginLeft<T>(this T target, float value) where T : VisualElement
	{
		target.style.marginLeft = value;
		return target;
	}

	public static float GetStyleMarginLeft<T>(this T target) where T : VisualElement
	{
		return target.style.marginLeft.value.value;
	}

	public static T SetStyleMarginTop<T>(this T target, float value) where T : VisualElement
	{
		target.style.marginTop = value;
		return target;
	}

	public static float GetStyleMarginTop<T>(this T target) where T : VisualElement
	{
		return target.style.marginTop.value.value;
	}

	public static T SetStyleMarginRight<T>(this T target, float value) where T : VisualElement
	{
		target.style.marginRight = value;
		return target;
	}

	public static float GetStyleMarginRight<T>(this T target) where T : VisualElement
	{
		return target.style.marginRight.value.value;
	}

	public static T SetStyleMarginBottom<T>(this T target, float value) where T : VisualElement
	{
		target.style.marginBottom = value;
		return target;
	}

	public static float GetStyleMarginBottom<T>(this T target) where T : VisualElement
	{
		return target.style.marginBottom.value.value;
	}

	public static T ClearPaddings<T>(this T target) where T : VisualElement
	{
		return target.SetStylePadding(0f);
	}

	public static T SetStylePadding<T>(this T target, float left, float top, float right, float bottom) where T : VisualElement
	{
		target.style.paddingLeft = left;
		target.style.paddingTop = top;
		target.style.paddingRight = right;
		target.style.paddingBottom = bottom;
		return target;
	}

	public static T SetStylePadding<T>(this T target, float value) where T : VisualElement
	{
		return target.SetStylePadding(value, value, value, value);
	}

	public static T SetStylePadding<T>(this T target, EdgeValues edge) where T : VisualElement
	{
		return target.SetStylePadding(edge.Left, edge.Top, edge.Right, edge.Bottom);
	}

	public static T SetStylePaddingLeft<T>(this T target, float value) where T : VisualElement
	{
		target.style.paddingLeft = value;
		return target;
	}

	public static float GetStylePaddingLeft<T>(this T target) where T : VisualElement
	{
		return target.style.paddingLeft.value.value;
	}

	public static T SetStylePaddingTop<T>(this T target, float value) where T : VisualElement
	{
		target.style.paddingTop = value;
		return target;
	}

	public static float GetStylePaddingTop<T>(this T target) where T : VisualElement
	{
		return target.style.paddingTop.value.value;
	}

	public static T SetStylePaddingRight<T>(this T target, float value) where T : VisualElement
	{
		target.style.paddingRight = value;
		return target;
	}

	public static float GetStylePaddingRight<T>(this T target) where T : VisualElement
	{
		return target.style.paddingRight.value.value;
	}

	public static T SetStylePaddingBottom<T>(this T target, float value) where T : VisualElement
	{
		target.style.paddingBottom = value;
		return target;
	}

	public static float GetStylePaddingBottom<T>(this T target) where T : VisualElement
	{
		return target.style.paddingBottom.value.value;
	}

	public static T ResizeToWidth<T>(this T target, float referenceWidth) where T : VisualElement
	{
		float width = target.resolvedStyle.width;
		float height = target.resolvedStyle.height;
		float num = referenceWidth / width;
		width = referenceWidth;
		height *= num;
		return target.SetStyleSize(width, height);
	}

	public static T ResizeToHeight<T>(this T target, float referenceHeight) where T : VisualElement
	{
		float width = target.resolvedStyle.width;
		float height = target.resolvedStyle.height;
		float num = referenceHeight / height;
		height = referenceHeight;
		width *= num;
		return target.SetStyleSize(width, height);
	}

	public static T ResizeToTextureWidth<T>(this T target, Texture texture, float referenceWidth) where T : VisualElement
	{
		if (texture == null)
		{
			return target;
		}
		float num = texture.width;
		float num2 = texture.height;
		float num3 = referenceWidth / num;
		num = referenceWidth;
		num2 *= num3;
		return target.SetStyleSize(num, num2);
	}

	public static T ResizeToTextureHeight<T>(this T target, Texture texture, float referenceHeight) where T : VisualElement
	{
		if (texture == null)
		{
			return target;
		}
		float num = texture.width;
		float num2 = texture.height;
		float num3 = referenceHeight / num2;
		num2 = referenceHeight;
		num *= num3;
		return target.SetStyleSize(num, num2);
	}

	public static T ResizeToTextureWidth<T>(this T target, float referenceWidth) where T : VisualElement
	{
		return target.ResizeToTextureWidth(target.GetStyleBackgroundImageTexture2D(), referenceWidth);
	}

	public static T ResizeToTextureHeight<T>(this T target, float referenceHeight) where T : VisualElement
	{
		return target.ResizeToTextureHeight(target.GetStyleBackgroundImageTexture2D(), referenceHeight);
	}

	public static T ResizeToTextureSize<T>(this T target, Texture texture, float ratio = 1f) where T : VisualElement
	{
		bool num = texture != null;
		float num2 = (num ? ((float)texture.width) : 0f);
		float num3 = (num ? ((float)texture.height) : 0f);
		ratio = Mathf.Max(0f, ratio);
		num2 *= ratio;
		num3 *= ratio;
		return target.SetStyleSize(num2, num3);
	}

	public static T ResizeToTextureSize<T>(this T target, float ratio = 1f) where T : VisualElement
	{
		return target.ResizeToTextureSize(target.GetStyleBackgroundImageTexture2D(), ratio);
	}

	public static T ResetLayout<T>(this T target) where T : VisualElement
	{
		return target.SetStyleDisplay(DisplayStyle.Flex).ResetStyleMinSize().ResetStyleSize()
			.ResetStyleMaxSize()
			.ClearMargins()
			.ClearPaddings()
			.ClearDistances();
	}

	public static T SetStyleDisplay<T>(this T target, DisplayStyle value) where T : VisualElement
	{
		target.style.display = value;
		return target;
	}

	public static DisplayStyle GetStyleDisplay<T>(this T target) where T : VisualElement
	{
		return target.style.display.value;
	}

	public static T Show<T>(this T target) where T : VisualElement
	{
		return target.SetStyleDisplay(DisplayStyle.Flex);
	}

	public static T Hide<T>(this T target) where T : VisualElement
	{
		return target.SetStyleDisplay(DisplayStyle.None);
	}

	public static T ClearDistances<T>(this T target) where T : VisualElement
	{
		return target.SetStyleDistance(0f, 0f, 0f, 0f);
	}

	public static T SetStyleDistance<T>(this T target, float left, float top, float right, float bottom) where T : VisualElement
	{
		target.style.left = left;
		target.style.top = top;
		target.style.right = right;
		target.style.bottom = bottom;
		return target;
	}

	public static T SetStyleLeft<T>(this T target, float value) where T : VisualElement
	{
		target.style.left = value;
		return target;
	}

	public static float GetStyleLeft<T>(this T target) where T : VisualElement
	{
		return target.style.left.value.value;
	}

	public static T SetStyleTop<T>(this T target, float value) where T : VisualElement
	{
		target.style.top = value;
		return target;
	}

	public static float GetStyleTop<T>(this T target) where T : VisualElement
	{
		return target.style.top.value.value;
	}

	public static T SetStyleRight<T>(this T target, float value) where T : VisualElement
	{
		target.style.right = value;
		return target;
	}

	public static float GetStyleRight<T>(this T target) where T : VisualElement
	{
		return target.style.right.value.value;
	}

	public static T SetStyleBottom<T>(this T target, float value) where T : VisualElement
	{
		target.style.bottom = value;
		return target;
	}

	public static float GetStyleBottom<T>(this T target) where T : VisualElement
	{
		return target.style.bottom.value.value;
	}

	public static bool HasParent<T>(this T target) where T : VisualElement
	{
		return target.parent != null;
	}

	public static bool IsVisible<T>(this T target) where T : VisualElement
	{
		if (target.HasParent())
		{
			return target.style.display.value == DisplayStyle.Flex;
		}
		return false;
	}

	public static T EnableElement<T>(this T target) where T : VisualElement
	{
		target.SetEnabled(value: true);
		return target;
	}

	public static T DisableElement<T>(this T target) where T : VisualElement
	{
		target.SetEnabled(value: false);
		return target;
	}

	public static bool IsEnabled<T>(this T target) where T : VisualElement
	{
		if (target.enabledSelf)
		{
			return target.enabledInHierarchy;
		}
		return false;
	}

	public static O ReplaceWith<T, O>(this T target, O other) where T : VisualElement where O : VisualElement
	{
		if (target.childCount > 0)
		{
			foreach (VisualElement item in new List<VisualElement>(target.Children()))
			{
				other.AddChild(item);
			}
		}
		if (!target.HasParent())
		{
			return other;
		}
		VisualElement parent = target.parent;
		int index = parent.IndexOf(target);
		target.RemoveFromHierarchy();
		parent.Insert(index, other);
		return other;
	}

	public static bool IsFocused<T>(this T target) where T : VisualElement
	{
		if (target.focusable)
		{
			return target.focusController.focusedElement == target;
		}
		return false;
	}
}
