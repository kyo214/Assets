using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Doozy.Runtime.UIDesigner.Utils;

public static class DesignerUtils
{
	private struct TargetInfo
	{
		public RectTransform rectTransform { get; }

		public Transform savedParent { get; private set; }

		public int savedSiblingIndex { get; private set; }

		public Vector2 savedPivot { get; private set; }

		public TargetInfo(RectTransform rectTransform)
		{
			this.rectTransform = rectTransform;
			savedParent = rectTransform.parent;
			savedSiblingIndex = rectTransform.GetSiblingIndex();
			savedPivot = rectTransform.pivot;
		}

		public void Restore()
		{
			if (savedPivot != rectTransform.pivot)
			{
				rectTransform.ChangePivot(savedPivot);
			}
			if (rectTransform.parent != savedParent)
			{
				rectTransform.SetParent(savedParent);
				rectTransform.SetSiblingIndex(savedSiblingIndex);
			}
			rectTransform.ForceUpdateRectTransforms();
		}
	}

	private readonly struct RotationInfo
	{
		public RectTransform rectTransform { get; }

		public Vector3 worldRotation { get; }

		public Vector3 localRotation { get; }

		public RotationInfo(RectTransform rectTransform)
		{
			this.rectTransform = rectTransform;
			worldRotation = rectTransform.eulerAngles;
			localRotation = rectTransform.localEulerAngles;
		}

		public void SetRotation(Space space, Axis axis, float value, bool relativeChange)
		{
			Vector3 rotation = GetRotation(space);
			switch (axis)
			{
			case Axis.X:
				rotation.x = (relativeChange ? (rotation.x + value) : value);
				break;
			case Axis.Y:
				rotation.y = (relativeChange ? (rotation.y + value) : value);
				break;
			case Axis.Z:
				rotation.z = (relativeChange ? (rotation.z + value) : value);
				break;
			}
			SetRotation(space, rotation);
		}

		public void SetRotationXY(Space space, float x, float y, bool relativeChange)
		{
			Vector3 rotation = GetRotation(space);
			rotation.x = (relativeChange ? (rotation.x + x) : x);
			rotation.y = (relativeChange ? (rotation.y + y) : y);
			SetRotation(space, rotation);
		}

		public void SetRotationXZ(Space space, float x, float z, bool relativeChange)
		{
			Vector3 rotation = GetRotation(space);
			rotation.x = (relativeChange ? (rotation.x + x) : x);
			rotation.z = (relativeChange ? (rotation.z + z) : z);
			SetRotation(space, rotation);
		}

		public void SetRotationYZ(Space space, float y, float z, bool relativeChange)
		{
			Vector3 rotation = GetRotation(space);
			rotation.y = (relativeChange ? (rotation.y + y) : y);
			rotation.z = (relativeChange ? (rotation.z + z) : z);
			SetRotation(space, rotation);
		}

		public void SetRotation(Space space, float x, float y, float z, bool relativeChange)
		{
			Vector3 rotation = GetRotation(space);
			rotation.x = (relativeChange ? (rotation.x + x) : x);
			rotation.y = (relativeChange ? (rotation.y + y) : y);
			rotation.z = (relativeChange ? (rotation.z + z) : z);
			SetRotation(space, rotation);
		}

		private Vector3 GetRotation(Space space)
		{
			if (space != Space.World)
			{
				return localRotation;
			}
			return worldRotation;
		}

		private void SetRotation(Space space, Vector3 value)
		{
			switch (space)
			{
			case Space.World:
				rectTransform.eulerAngles = value;
				break;
			case Space.Local:
				rectTransform.localEulerAngles = value;
				break;
			}
		}
	}

	private readonly struct ScaleInfo
	{
		public RectTransform rectTransform { get; }

		public Vector3 localScale { get; }

		public ScaleInfo(RectTransform rectTransform)
		{
			this.rectTransform = rectTransform;
			localScale = rectTransform.localScale;
		}

		public void SetScaleX(float x, bool relativeChange)
		{
			Vector3 scale = GetScale();
			scale.x = (relativeChange ? (scale.x + x) : x);
			SetScale(scale);
		}

		public void SetScaleY(float y, bool relativeChange)
		{
			Vector3 scale = GetScale();
			scale.y = (relativeChange ? (scale.y + y) : y);
			SetScale(scale);
		}

		public void SetScaleXY(float x, float y, bool relativeChange)
		{
			Vector3 scale = GetScale();
			scale.x = (relativeChange ? (scale.x + x) : x);
			scale.y = (relativeChange ? (scale.y + y) : y);
			SetScale(scale);
		}

		private Vector3 GetScale()
		{
			return localScale;
		}

		private void SetScale(Vector3 value)
		{
			rectTransform.localScale = value;
		}
	}

	private readonly struct SizeInfo
	{
		public RectTransform rectTransform { get; }

		public Vector2 sizeDelta { get; }

		public SizeInfo(RectTransform rectTransform)
		{
			this.rectTransform = rectTransform;
			sizeDelta = rectTransform.sizeDelta;
		}

		public void SetSizeX(float x, bool relativeChange)
		{
			Vector2 size = GetSize();
			size.x = (relativeChange ? (size.x + x) : x);
			SetSize(size);
		}

		public void SetSizeY(float y, bool relativeChange)
		{
			Vector2 size = GetSize();
			size.y = (relativeChange ? (size.y + y) : y);
			SetSize(size);
		}

		public void SetSizeXY(float x, float y, bool relativeChange)
		{
			Vector2 size = GetSize();
			size.x = (relativeChange ? (size.x + x) : x);
			size.y = (relativeChange ? (size.y + y) : y);
			SetSize(size);
		}

		private Vector2 GetSize()
		{
			return sizeDelta;
		}

		private void SetSize(Vector2 value)
		{
			rectTransform.sizeDelta = value;
		}
	}

	private static List<RotationInfo> currentRotationInfos { get; } = new List<RotationInfo>();

	private static List<ScaleInfo> currentScaleInfos { get; } = new List<ScaleInfo>();

	private static List<SizeInfo> currentSizeInfos { get; } = new List<SizeInfo>();

	public static void Align(AlignTo alignTo, Align align, AlignMode alignMode, bool updateAnchors, RectTransform keyObject = null, params RectTransform[] rectTransforms)
	{
		if (rectTransforms != null && rectTransforms.Length != 0)
		{
			switch (alignTo)
			{
			case AlignTo.RootCanvas:
				AlignToRootCanvas(align, alignMode, updateAnchors, rectTransforms);
				break;
			case AlignTo.Parent:
				AlignToParent(align, alignMode, updateAnchors, rectTransforms);
				break;
			case AlignTo.Selection:
				AlignToSelection(align, alignMode, updateAnchors, rectTransforms);
				break;
			case AlignTo.KeyObject:
				AlignToKeyObject(align, alignMode, updateAnchors, keyObject, rectTransforms);
				break;
			default:
				throw new ArgumentOutOfRangeException("alignTo", alignTo, null);
			}
		}
	}

	public static void AlignToRootCanvas(Align align, AlignMode alignMode, bool updateAnchors, params RectTransform[] rectTransforms)
	{
		if (rectTransforms == null || rectTransforms.Length == 0)
		{
			return;
		}
		IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);
		RectTransform component = rectTransforms[0].root.GetComponent<RectTransform>();
		Rect rect = component.rect;
		float xMin = rect.xMin;
		float xMax = rect.xMax;
		float yMin = rect.yMin;
		float yMax = rect.yMax;
		float x = rect.center.x;
		float y = rect.center.y;
		foreach (RectTransform rectTransform in rectTransforms)
		{
			rectTransform.SetParent(component);
			rectTransform.ChangePivot(new Vector2(0.5f, 0.5f));
			rectTransform.ForceUpdateRectTransforms();
			AlignRectTransform(align, alignMode, updateAnchors, rectTransform, xMin, xMax, yMin, yMax, x, y);
		}
		foreach (TargetInfo item in targetInfos)
		{
			item.Restore();
		}
	}

	public static void AlignToParent(Align align, AlignMode alignMode, bool updateAnchors, params RectTransform[] rectTransforms)
	{
		if (rectTransforms == null || rectTransforms.Length == 0)
		{
			return;
		}
		IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);
		foreach (RectTransform rectTransform in rectTransforms)
		{
			Transform parent = rectTransform.transform.parent;
			if (!(parent == null))
			{
				RectTransform component = parent.GetComponent<RectTransform>();
				if (!(component == null))
				{
					Rect rect = component.rect;
					float xMin = rect.xMin;
					float xMax = rect.xMax;
					float yMin = rect.yMin;
					float yMax = rect.yMax;
					float x = rect.center.x;
					float y = rect.center.y;
					rectTransform.ChangePivot(new Vector2(0.5f, 0.5f));
					rectTransform.ForceUpdateRectTransforms();
					AlignRectTransform(align, alignMode, updateAnchors, rectTransform, xMin, xMax, yMin, yMax, x, y);
				}
			}
		}
		foreach (TargetInfo item in targetInfos)
		{
			item.Restore();
		}
	}

	public static void AlignToSelection(Align align, AlignMode alignMode, bool updateAnchors, params RectTransform[] rectTransforms)
	{
		if (rectTransforms == null || rectTransforms.Length == 0)
		{
			return;
		}
		IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);
		RectTransform component = rectTransforms[0].root.GetComponent<RectTransform>();
		float num = float.PositiveInfinity;
		float num2 = float.NegativeInfinity;
		float num3 = float.PositiveInfinity;
		float num4 = float.NegativeInfinity;
		RectTransform[] array = rectTransforms;
		foreach (RectTransform rectTransform in array)
		{
			rectTransform.SetParent(component);
			rectTransform.ChangePivot(new Vector2(0.5f, 0.5f));
			rectTransform.ForceUpdateRectTransforms();
			Rect rect = rectTransform.DesignerRect();
			num = Mathf.Min(num, rectTransform.anchoredPosition.x - rect.width / 2f);
			num2 = Mathf.Max(num2, rectTransform.anchoredPosition.x + rect.width / 2f);
			num3 = Mathf.Min(num3, rectTransform.anchoredPosition.y - rect.height / 2f);
			num4 = Mathf.Max(num4, rectTransform.anchoredPosition.y + rect.height / 2f);
		}
		float xCenter = (num + num2) / 2f;
		float yCenter = (num3 + num4) / 2f;
		array = rectTransforms;
		foreach (RectTransform rectTransform2 in array)
		{
			AlignRectTransform(align, alignMode, updateAnchors, rectTransform2, num, num2, num3, num4, xCenter, yCenter);
		}
		foreach (TargetInfo item in targetInfos)
		{
			item.Restore();
		}
	}

	public static void AlignToKeyObject(Align align, AlignMode alignMode, bool updateAnchors, RectTransform keyObject, params RectTransform[] rectTransforms)
	{
		if (rectTransforms == null || rectTransforms.Length == 0 || keyObject == null)
		{
			return;
		}
		IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);
		RectTransform component = keyObject.root.GetComponent<RectTransform>();
		RectTransform[] array = rectTransforms;
		foreach (RectTransform obj in array)
		{
			obj.SetParent(component);
			obj.ChangePivot(new Vector2(0.5f, 0.5f));
			obj.ForceUpdateRectTransforms();
		}
		Vector2 vector = keyObject.localPosition;
		Rect rect = keyObject.DesignerRect();
		Vector2 pivot = keyObject.pivot;
		float xMin = vector.x - rect.width * (1f - pivot.x);
		float xMax = vector.x + rect.width * pivot.x;
		float yMin = vector.y - rect.height * (1f - pivot.y);
		float yMax = vector.y + rect.height * pivot.y;
		float x = vector.x;
		float y = vector.y;
		rectTransforms = rectTransforms.Where((RectTransform r) => r != keyObject).ToArray();
		array = rectTransforms;
		foreach (RectTransform rectTransform in array)
		{
			AlignRectTransform(align, alignMode, updateAnchors, rectTransform, xMin, xMax, yMin, yMax, x, y);
		}
		foreach (TargetInfo item in targetInfos)
		{
			item.Restore();
		}
	}

	private static void AlignRectTransform(Align align, AlignMode alignMode, bool updateAnchors, RectTransform rectTransform, float xMin, float xMax, float yMin, float yMax, float xCenter, float yCenter)
	{
		Vector2 pivot = rectTransform.pivot;
		float x = pivot.x;
		float y = pivot.y;
		Vector3 localPosition = rectTransform.localPosition;
		float x2 = localPosition.x;
		float y2 = localPosition.y;
		Rect rect = rectTransform.DesignerRect();
		Vector2 vector = align switch
		{
			Doozy.Runtime.UIDesigner.Align.HorizontalLeft => alignMode switch
			{
				AlignMode.Inside => new Vector2(xMin + rect.width * x, y2), 
				AlignMode.Center => new Vector2(xMin, y2), 
				AlignMode.Outside => new Vector2(xMin - rect.width * x, y2), 
				_ => throw new ArgumentOutOfRangeException("alignMode", alignMode, null), 
			}, 
			Doozy.Runtime.UIDesigner.Align.HorizontalCenter => new Vector2(xCenter - rect.width * 0.5f + rect.width * x, y2), 
			Doozy.Runtime.UIDesigner.Align.HorizontalRight => alignMode switch
			{
				AlignMode.Inside => new Vector2(xMax - rect.width * (1f - x), y2), 
				AlignMode.Center => new Vector2(xMax, y2), 
				AlignMode.Outside => new Vector2(xMax + rect.width * (1f - x), y2), 
				_ => throw new ArgumentOutOfRangeException("alignMode", alignMode, null), 
			}, 
			Doozy.Runtime.UIDesigner.Align.VerticalTop => alignMode switch
			{
				AlignMode.Inside => new Vector2(x2, yMax - rect.height * (1f - y)), 
				AlignMode.Center => new Vector2(x2, yMax), 
				AlignMode.Outside => new Vector2(x2, yMax + rect.height * (1f - y)), 
				_ => throw new ArgumentOutOfRangeException("alignMode", alignMode, null), 
			}, 
			Doozy.Runtime.UIDesigner.Align.VerticalCenter => new Vector2(x2, yCenter - rect.height * 0.5f + rect.height * y), 
			Doozy.Runtime.UIDesigner.Align.VerticalBottom => alignMode switch
			{
				AlignMode.Inside => new Vector2(x2, yMin + rect.height * y), 
				AlignMode.Center => new Vector2(x2, yMin), 
				AlignMode.Outside => new Vector2(x2, yMin - rect.height * y), 
				_ => throw new ArgumentOutOfRangeException("alignMode", alignMode, null), 
			}, 
			_ => throw new ArgumentOutOfRangeException("align", align, null), 
		};
		if (updateAnchors)
		{
			switch (align)
			{
			case Doozy.Runtime.UIDesigner.Align.HorizontalLeft:
				rectTransform.anchorMin = new Vector2(0f, rectTransform.anchorMin.y);
				rectTransform.anchorMax = new Vector2(0f, rectTransform.anchorMax.y);
				break;
			case Doozy.Runtime.UIDesigner.Align.HorizontalCenter:
				rectTransform.anchorMin = new Vector2(0.5f, rectTransform.anchorMin.y);
				rectTransform.anchorMax = new Vector2(0.5f, rectTransform.anchorMax.y);
				break;
			case Doozy.Runtime.UIDesigner.Align.HorizontalRight:
				rectTransform.anchorMin = new Vector2(1f, rectTransform.anchorMin.y);
				rectTransform.anchorMax = new Vector2(1f, rectTransform.anchorMax.y);
				break;
			case Doozy.Runtime.UIDesigner.Align.VerticalTop:
				rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 1f);
				rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 1f);
				break;
			case Doozy.Runtime.UIDesigner.Align.VerticalCenter:
				rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 0.5f);
				rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 0.5f);
				break;
			case Doozy.Runtime.UIDesigner.Align.VerticalBottom:
				rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 0f);
				rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 0f);
				break;
			default:
				throw new ArgumentOutOfRangeException("align", align, null);
			}
		}
		rectTransform.localPosition = vector;
	}

	public static void DistributeHorizontalWithSpacing(AlignTo alignTo, RectTransform keyObject, float spacing, params RectTransform[] rectTransforms)
	{
		if (rectTransforms == null || rectTransforms.Length == 0)
		{
			return;
		}
		int num = rectTransforms.Length;
		if (num < 2)
		{
			return;
		}
		IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);
		float num2 = float.PositiveInfinity;
		float num3 = float.NegativeInfinity;
		float num4 = 0f;
		switch (alignTo)
		{
		case AlignTo.RootCanvas:
		{
			RectTransform component = rectTransforms[0].root.GetComponent<RectTransform>();
			float width = component.rect.width;
			RectTransform[] array = rectTransforms;
			foreach (RectTransform obj2 in array)
			{
				obj2.SetParent(component);
				obj2.ChangePivot(new Vector2(0.5f, 0.5f));
				obj2.ForceUpdateRectTransforms();
				num4 += obj2.DesignerRect().width;
			}
			spacing = (width - num4) / (float)(num - 1);
			float num5 = component.rect.xMin;
			rectTransforms = rectTransforms.OrderBy((RectTransform r) => r.localPosition.x).ToArray();
			array = rectTransforms;
			foreach (RectTransform rectTransform3 in array)
			{
				Rect rect4 = rectTransform3.DesignerRect();
				rectTransform3.localPosition = new Vector2(num5 + rect4.width * rectTransform3.pivot.x, rectTransform3.localPosition.y);
				num5 += rect4.width + spacing;
			}
			break;
		}
		case AlignTo.Parent:
		{
			Transform transformParent = rectTransforms[0].parent;
			if (transformParent == null)
			{
				return;
			}
			RectTransform component2 = transformParent.GetComponent<RectTransform>();
			if (component2 == null)
			{
				return;
			}
			Rect rect7 = component2.DesignerRect();
			float width2 = rect7.width;
			RectTransform[] array = rectTransforms;
			foreach (RectTransform obj3 in array)
			{
				obj3.ChangePivot(new Vector2(0.5f, 0.5f));
				obj3.ForceUpdateRectTransforms();
				num4 += obj3.DesignerRect().width;
			}
			spacing = (width2 - num4) / (float)(num - 1);
			float num5 = rect7.xMin;
			rectTransforms = (from r in rectTransforms
				where r.parent == transformParent
				orderby r.localPosition.x
				select r).ToArray();
			array = rectTransforms;
			foreach (RectTransform rectTransform6 in array)
			{
				Rect rect8 = rectTransform6.DesignerRect();
				rectTransform6.localPosition = new Vector2(num5 + rect8.width * rectTransform6.pivot.x, rectTransform6.localPosition.y);
				num5 += rect8.width + spacing;
			}
			break;
		}
		case AlignTo.Selection:
		{
			RectTransform component = rectTransforms[0].root.GetComponent<RectTransform>();
			RectTransform[] array = rectTransforms;
			foreach (RectTransform rectTransform4 in array)
			{
				rectTransform4.SetParent(component);
				rectTransform4.ChangePivot(new Vector2(0.5f, 0.5f));
				rectTransform4.ForceUpdateRectTransforms();
				Rect rect5 = rectTransform4.DesignerRect();
				Vector3 localPosition = rectTransform4.localPosition;
				num2 = Mathf.Min(num2, localPosition.x - rect5.width * rectTransform4.pivot.x);
				num3 = Mathf.Max(num3, localPosition.x + rect5.width * rectTransform4.pivot.x);
				num4 += rect5.width;
			}
			spacing = (num3 - num2 - num4) / (float)(num - 1);
			float num5 = num2;
			rectTransforms = rectTransforms.OrderBy((RectTransform r) => r.localPosition.x).ToArray();
			array = rectTransforms;
			foreach (RectTransform rectTransform5 in array)
			{
				Rect rect6 = rectTransform5.DesignerRect();
				rectTransform5.localPosition = new Vector2(num5 + rect6.width * rectTransform5.pivot.x, rectTransform5.localPosition.y);
				num5 += rect6.width + spacing;
			}
			break;
		}
		case AlignTo.KeyObject:
		{
			if (keyObject == null)
			{
				return;
			}
			RectTransform component = keyObject.root.GetComponent<RectTransform>();
			RectTransform[] array = rectTransforms;
			foreach (RectTransform obj in array)
			{
				obj.SetParent(component);
				obj.ChangePivot(new Vector2(0.5f, 0.5f));
				obj.ForceUpdateRectTransforms();
			}
			rectTransforms = rectTransforms.Where((RectTransform r) => r != keyObject).ToArray();
			RectTransform[] array2 = (from r in rectTransforms
				where r.localPosition.x <= keyObject.localPosition.x
				orderby r.localPosition.x descending
				select r).ToArray();
			RectTransform[] array3 = (from r in rectTransforms
				where r.localPosition.x > keyObject.localPosition.x
				orderby r.localPosition.x
				select r).ToArray();
			Rect rect = keyObject.DesignerRect();
			if (array2.Length != 0)
			{
				float num5 = keyObject.localPosition.x - rect.width * (1f - keyObject.pivot.x);
				num5 -= spacing;
				array = array2;
				foreach (RectTransform rectTransform in array)
				{
					Rect rect2 = rectTransform.DesignerRect();
					rectTransform.localPosition = new Vector2(num5 - rect2.width * rectTransform.pivot.x, rectTransform.localPosition.y);
					num5 -= rect2.width + spacing;
				}
			}
			if (array3.Length != 0)
			{
				float num5 = keyObject.localPosition.x + rect.width * keyObject.pivot.x;
				num5 += spacing;
				array = array3;
				foreach (RectTransform rectTransform2 in array)
				{
					Rect rect3 = rectTransform2.DesignerRect();
					rectTransform2.localPosition = new Vector2(num5 + rect3.width * rectTransform2.pivot.x, rectTransform2.localPosition.y);
					num5 += rect3.width + spacing;
				}
			}
			break;
		}
		default:
			throw new ArgumentOutOfRangeException("alignTo", alignTo, null);
		}
		foreach (TargetInfo item in targetInfos)
		{
			item.Restore();
		}
	}

	public static void DistributeVerticalWithSpacing(AlignTo alignTo, RectTransform keyObject, float spacing, params RectTransform[] rectTransforms)
	{
		if (rectTransforms == null || rectTransforms.Length == 0)
		{
			return;
		}
		int num = rectTransforms.Length;
		if (num < 2)
		{
			return;
		}
		IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);
		float num2 = float.PositiveInfinity;
		float num3 = float.NegativeInfinity;
		float num4 = 0f;
		switch (alignTo)
		{
		case AlignTo.RootCanvas:
		{
			RectTransform component = rectTransforms[0].root.GetComponent<RectTransform>();
			float height = component.rect.height;
			RectTransform[] array = rectTransforms;
			foreach (RectTransform obj2 in array)
			{
				obj2.SetParent(component);
				obj2.ChangePivot(new Vector2(0.5f, 0.5f));
				obj2.ForceUpdateRectTransforms();
				num4 += obj2.DesignerRect().height;
			}
			spacing = (height - num4) / (float)(num - 1);
			float num5 = component.rect.yMin;
			rectTransforms = rectTransforms.OrderBy((RectTransform r) => r.localPosition.y).ToArray();
			array = rectTransforms;
			foreach (RectTransform rectTransform3 in array)
			{
				Rect rect4 = rectTransform3.DesignerRect();
				rectTransform3.localPosition = new Vector2(rectTransform3.localPosition.x, num5 + rect4.height * rectTransform3.pivot.y);
				num5 += rect4.height + spacing;
			}
			break;
		}
		case AlignTo.Parent:
		{
			Transform transformParent = rectTransforms[0].parent;
			if (transformParent == null)
			{
				return;
			}
			RectTransform component2 = transformParent.GetComponent<RectTransform>();
			if (component2 == null)
			{
				return;
			}
			Rect rect7 = component2.DesignerRect();
			float height2 = rect7.height;
			RectTransform[] array = rectTransforms;
			foreach (RectTransform obj3 in array)
			{
				obj3.ChangePivot(new Vector2(0.5f, 0.5f));
				obj3.ForceUpdateRectTransforms();
				num4 += obj3.DesignerRect().height;
			}
			spacing = (height2 - num4) / (float)(num - 1);
			float num5 = rect7.yMin;
			rectTransforms = (from r in rectTransforms
				where r.parent == transformParent
				orderby r.localPosition.y
				select r).ToArray();
			array = rectTransforms;
			foreach (RectTransform rectTransform6 in array)
			{
				Rect rect8 = rectTransform6.DesignerRect();
				rectTransform6.localPosition = new Vector2(rectTransform6.localPosition.x, num5 + rect8.height * rectTransform6.pivot.y);
				num5 += rect8.height + spacing;
			}
			break;
		}
		case AlignTo.Selection:
		{
			RectTransform component = rectTransforms[0].root.GetComponent<RectTransform>();
			RectTransform[] array = rectTransforms;
			foreach (RectTransform rectTransform4 in array)
			{
				rectTransform4.SetParent(component);
				rectTransform4.ChangePivot(new Vector2(0.5f, 0.5f));
				rectTransform4.ForceUpdateRectTransforms();
				Rect rect5 = rectTransform4.DesignerRect();
				Vector3 localPosition = rectTransform4.localPosition;
				num2 = Mathf.Min(num2, localPosition.y - rect5.height * rectTransform4.pivot.y);
				num3 = Mathf.Max(num3, localPosition.y + rect5.height * rectTransform4.pivot.y);
				num4 += rect5.height;
			}
			spacing = (num3 - num2 - num4) / (float)(num - 1);
			float num5 = num2;
			rectTransforms = rectTransforms.OrderBy((RectTransform r) => r.localPosition.y).ToArray();
			array = rectTransforms;
			foreach (RectTransform rectTransform5 in array)
			{
				Rect rect6 = rectTransform5.DesignerRect();
				rectTransform5.localPosition = new Vector2(rectTransform5.localPosition.x, num5 + rect6.height * rectTransform5.pivot.y);
				num5 += rect6.height + spacing;
			}
			break;
		}
		case AlignTo.KeyObject:
		{
			if (keyObject == null)
			{
				return;
			}
			RectTransform component = keyObject.root.GetComponent<RectTransform>();
			RectTransform[] array = rectTransforms;
			foreach (RectTransform obj in array)
			{
				obj.SetParent(component);
				obj.ChangePivot(new Vector2(0.5f, 0.5f));
				obj.ForceUpdateRectTransforms();
			}
			rectTransforms = rectTransforms.Where((RectTransform r) => r != keyObject).ToArray();
			RectTransform[] array2 = (from r in rectTransforms
				where r.localPosition.x <= keyObject.localPosition.y
				orderby r.localPosition.y descending
				select r).ToArray();
			RectTransform[] array3 = (from r in rectTransforms
				where r.localPosition.x > keyObject.localPosition.y
				orderby r.localPosition.y
				select r).ToArray();
			Rect rect = keyObject.DesignerRect();
			if (array2.Length != 0)
			{
				float num5 = keyObject.localPosition.y - rect.height * (1f - keyObject.pivot.y);
				num5 -= spacing;
				array = array2;
				foreach (RectTransform rectTransform in array)
				{
					Rect rect2 = rectTransform.DesignerRect();
					rectTransform.localPosition = new Vector2(rectTransform.localPosition.x, num5 - rect2.height * rectTransform.pivot.y);
					num5 -= rect2.height + spacing;
				}
			}
			if (array3.Length != 0)
			{
				float num5 = keyObject.localPosition.y + rect.height * keyObject.pivot.y;
				num5 += spacing;
				array = array3;
				foreach (RectTransform rectTransform2 in array)
				{
					Rect rect3 = rectTransform2.DesignerRect();
					rectTransform2.localPosition = new Vector2(rectTransform2.localPosition.x, num5 + rect3.height * rectTransform2.pivot.y);
					num5 += rect3.height + spacing;
				}
			}
			break;
		}
		default:
			throw new ArgumentOutOfRangeException("alignTo", alignTo, null);
		}
		foreach (TargetInfo item in targetInfos)
		{
			item.Restore();
		}
	}

	public static void Distribute(AlignTo alignTo, Distribute distribute, RectTransform keyObject = null, float spacing = 0f, params RectTransform[] rectTransforms)
	{
		switch (alignTo)
		{
		case AlignTo.RootCanvas:
			DistributeToRootCanvas(distribute, rectTransforms);
			break;
		case AlignTo.Parent:
			DistributeToParent(distribute, rectTransforms);
			break;
		case AlignTo.Selection:
			DistributeToSelection(distribute, rectTransforms);
			break;
		case AlignTo.KeyObject:
			DistributeToKeyObject(distribute, keyObject, spacing, rectTransforms);
			break;
		default:
			throw new ArgumentOutOfRangeException("alignTo", alignTo, null);
		}
	}

	public static void DistributeToRootCanvas(Distribute distribute, params RectTransform[] rectTransforms)
	{
		if (rectTransforms == null || rectTransforms.Length == 0)
		{
			return;
		}
		IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);
		RectTransform component = rectTransforms[0].root.GetComponent<RectTransform>();
		Rect rect = component.rect;
		float xMin = rect.xMin;
		float xMax = rect.xMax;
		float yMin = rect.yMin;
		float yMax = rect.yMax;
		foreach (RectTransform obj in rectTransforms)
		{
			obj.SetParent(component);
			obj.ChangePivot(new Vector2(0.5f, 0.5f));
			obj.ForceUpdateRectTransforms();
		}
		Distribute(distribute, xMin, xMax, yMin, yMax, rectTransforms);
		foreach (TargetInfo item in targetInfos)
		{
			item.Restore();
		}
	}

	public static void DistributeToParent(Distribute distribute, params RectTransform[] rectTransforms)
	{
		if (rectTransforms == null || rectTransforms.Length == 0)
		{
			return;
		}
		IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);
		Transform parent = rectTransforms.First().transform.parent;
		if (parent == null)
		{
			return;
		}
		RectTransform component = parent.GetComponent<RectTransform>();
		if (component == null)
		{
			return;
		}
		Rect rect = component.rect;
		float xMin = rect.xMin;
		float xMax = rect.xMax;
		float yMin = rect.yMin;
		float yMax = rect.yMax;
		foreach (RectTransform obj in rectTransforms)
		{
			obj.SetParent(parent);
			obj.ChangePivot(new Vector2(0.5f, 0.5f));
			obj.ForceUpdateRectTransforms();
		}
		Distribute(distribute, xMin, xMax, yMin, yMax, rectTransforms);
		foreach (TargetInfo item in targetInfos)
		{
			item.Restore();
		}
	}

	public static void DistributeToSelection(Distribute distribute, params RectTransform[] rectTransforms)
	{
		if (rectTransforms == null || rectTransforms.Length == 0)
		{
			return;
		}
		IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);
		RectTransform component = rectTransforms[0].root.GetComponent<RectTransform>();
		float num = float.PositiveInfinity;
		float num2 = float.NegativeInfinity;
		float num3 = float.PositiveInfinity;
		float num4 = float.NegativeInfinity;
		foreach (RectTransform rectTransform in rectTransforms)
		{
			rectTransform.SetParent(component);
			rectTransform.ChangePivot(new Vector2(0.5f, 0.5f));
			rectTransform.ForceUpdateRectTransforms();
			Rect rect = rectTransform.DesignerRect();
			Vector3 localPosition = rectTransform.localPosition;
			num = Mathf.Min(num, localPosition.x - rect.width * (1f - rectTransform.pivot.x));
			num2 = Mathf.Max(num2, localPosition.x + rect.width * rectTransform.pivot.x);
			num3 = Mathf.Min(num3, localPosition.y - rect.height * (1f - rectTransform.pivot.y));
			num4 = Mathf.Max(num4, localPosition.y + rect.height * rectTransform.pivot.y);
		}
		Distribute(distribute, num, num2, num3, num4, rectTransforms);
		foreach (TargetInfo item in targetInfos)
		{
			item.Restore();
		}
	}

	public static void DistributeToKeyObject(Distribute distribute, RectTransform keyObject, float spacing, params RectTransform[] rectTransforms)
	{
		if (rectTransforms == null || rectTransforms.Length == 0)
		{
			return;
		}
		IEnumerable<TargetInfo> targetInfos = GetTargetInfos(rectTransforms);
		if ((object)keyObject == null)
		{
			keyObject = rectTransforms[0];
		}
		Vector2 anchoredPosition = keyObject.anchoredPosition;
		Rect rect = keyObject.DesignerRect();
		Vector2 pivot = keyObject.pivot;
		float xMin = anchoredPosition.x - rect.width * (1f - pivot.x);
		float xMax = anchoredPosition.x + rect.width * pivot.x;
		float yMin = anchoredPosition.y - rect.height * (1f - pivot.y);
		float yMax = anchoredPosition.y + rect.height * pivot.y;
		RectTransform component = keyObject.root.GetComponent<RectTransform>();
		foreach (RectTransform obj in rectTransforms)
		{
			obj.SetParent(component);
			obj.ChangePivot(new Vector2(0.5f, 0.5f));
			obj.ForceUpdateRectTransforms();
		}
		Distribute(distribute, xMin, xMax, yMin, yMax, spacing, rectTransforms);
		foreach (TargetInfo item in targetInfos)
		{
			item.Restore();
		}
	}

	private static void Distribute(Distribute distribute, float xMin, float xMax, float yMin, float yMax, float spacing, params RectTransform[] rectTransforms)
	{
		if (rectTransforms.Length < 2)
		{
			return;
		}
		switch (distribute)
		{
		case Doozy.Runtime.UIDesigner.Distribute.VerticalTop:
		case Doozy.Runtime.UIDesigner.Distribute.VerticalCenter:
		case Doozy.Runtime.UIDesigner.Distribute.VerticalBottom:
			rectTransforms = rectTransforms.OrderByDescending((RectTransform r) => r.localPosition.y).ToArray();
			break;
		case Doozy.Runtime.UIDesigner.Distribute.HorizontalLeft:
		case Doozy.Runtime.UIDesigner.Distribute.HorizontalCenter:
		case Doozy.Runtime.UIDesigner.Distribute.HorizontalRight:
			rectTransforms = rectTransforms.OrderBy((RectTransform r) => r.localPosition.x).ToArray();
			break;
		default:
			throw new ArgumentOutOfRangeException("distribute", distribute, null);
		}
		switch (distribute)
		{
		case Doozy.Runtime.UIDesigner.Distribute.VerticalTop:
			DistributeVerticalTop(spacing, yMax, rectTransforms);
			break;
		case Doozy.Runtime.UIDesigner.Distribute.VerticalCenter:
			DistributeVerticalCenter(spacing, yMax, rectTransforms);
			break;
		case Doozy.Runtime.UIDesigner.Distribute.VerticalBottom:
			DistributeVerticalBottom(spacing, yMin, rectTransforms);
			break;
		case Doozy.Runtime.UIDesigner.Distribute.HorizontalLeft:
			DistributeHorizontalLeft(spacing, xMin, rectTransforms);
			break;
		case Doozy.Runtime.UIDesigner.Distribute.HorizontalCenter:
			DistributeHorizontalCenter(spacing, xMin, rectTransforms);
			break;
		case Doozy.Runtime.UIDesigner.Distribute.HorizontalRight:
			DistributeHorizontalRight(spacing, xMax, rectTransforms);
			break;
		default:
			throw new ArgumentOutOfRangeException("distribute", distribute, null);
		}
	}

	private static void DistributeVerticalTop(float spacing, float yMax, params RectTransform[] rectTransforms)
	{
		float num = yMax;
		foreach (RectTransform rectTransform in rectTransforms)
		{
			rectTransform.localPosition = new Vector2(rectTransform.localPosition.x, num - rectTransform.DesignerRect().height * (1f - rectTransform.pivot.y));
			num -= spacing;
		}
	}

	private static void DistributeVerticalCenter(float spacing, float yMax, params RectTransform[] rectTransforms)
	{
		RectTransform rectTransform = rectTransforms.First();
		float num = rectTransform.DesignerRect().height * (1f - rectTransform.pivot.y);
		float num2 = yMax;
		foreach (RectTransform rectTransform2 in rectTransforms)
		{
			if (rectTransform2 == rectTransform)
			{
				rectTransform2.localPosition = new Vector2(rectTransform2.localPosition.x, num2 - num);
				num2 -= num + spacing;
			}
			else
			{
				rectTransform2.localPosition = new Vector2(rectTransform2.localPosition.x, num2);
				num2 -= spacing;
			}
		}
	}

	private static void DistributeVerticalBottom(float spacing, float yMin, params RectTransform[] rectTransforms)
	{
		float num = yMin;
		for (int num2 = rectTransforms.Length - 1; num2 >= 0; num2--)
		{
			RectTransform rectTransform = rectTransforms[num2];
			rectTransform.localPosition = new Vector2(rectTransform.localPosition.x, num + rectTransform.DesignerRect().height * rectTransform.pivot.y);
			num += spacing;
		}
	}

	private static void DistributeHorizontalLeft(float spacing, float xMin, params RectTransform[] rectTransforms)
	{
		float num = xMin;
		foreach (RectTransform rectTransform in rectTransforms)
		{
			rectTransform.localPosition = new Vector2(num + rectTransform.DesignerRect().width * rectTransform.pivot.x, rectTransform.localPosition.y);
			num += spacing;
		}
	}

	private static void DistributeHorizontalCenter(float spacing, float xMin, params RectTransform[] rectTransforms)
	{
		RectTransform rectTransform = rectTransforms.First();
		float num = rectTransform.DesignerRect().width * (1f - rectTransform.pivot.x);
		float num2 = xMin;
		foreach (RectTransform rectTransform2 in rectTransforms)
		{
			if (rectTransform2 == rectTransform)
			{
				rectTransform2.localPosition = new Vector2(num2 + num, rectTransform2.localPosition.y);
				num2 += num + spacing;
			}
			else
			{
				rectTransform2.localPosition = new Vector2(num2, rectTransform2.localPosition.y);
				num2 += spacing;
			}
		}
	}

	private static void DistributeHorizontalRight(float spacing, float xMax, params RectTransform[] rectTransforms)
	{
		float num = xMax;
		for (int num2 = rectTransforms.Length - 1; num2 >= 0; num2--)
		{
			RectTransform rectTransform = rectTransforms[num2];
			rectTransform.localPosition = new Vector2(num - rectTransform.DesignerRect().width * (1f - rectTransform.pivot.x), rectTransform.localPosition.y);
			num -= spacing;
		}
	}

	private static void Distribute(Distribute distribute, float xMin, float xMax, float yMin, float yMax, params RectTransform[] rectTransforms)
	{
		int num = rectTransforms.Length;
		if (num < 2)
		{
			return;
		}
		switch (distribute)
		{
		case Doozy.Runtime.UIDesigner.Distribute.VerticalTop:
		case Doozy.Runtime.UIDesigner.Distribute.VerticalCenter:
		case Doozy.Runtime.UIDesigner.Distribute.VerticalBottom:
			rectTransforms = rectTransforms.OrderByDescending((RectTransform r) => r.localPosition.y).ToArray();
			break;
		case Doozy.Runtime.UIDesigner.Distribute.HorizontalLeft:
		case Doozy.Runtime.UIDesigner.Distribute.HorizontalCenter:
		case Doozy.Runtime.UIDesigner.Distribute.HorizontalRight:
			rectTransforms = rectTransforms.OrderBy((RectTransform r) => r.localPosition.x).ToArray();
			break;
		default:
			throw new ArgumentOutOfRangeException("distribute", distribute, null);
		}
		float num2 = xMax - xMin;
		float num3 = yMax - yMin;
		RectTransform rectTransform = rectTransforms.First();
		RectTransform rectTransform2 = rectTransforms.Last();
		switch (distribute)
		{
		case Doozy.Runtime.UIDesigner.Distribute.VerticalTop:
		{
			float height2 = rectTransform2.DesignerRect().height;
			DistributeVerticalTop((num3 - height2) / (float)(num - 1), yMax, rectTransforms);
			break;
		}
		case Doozy.Runtime.UIDesigner.Distribute.VerticalCenter:
		{
			float num6 = rectTransform.DesignerRect().height * (1f - rectTransform.pivot.y);
			float num7 = rectTransform2.DesignerRect().height * rectTransform2.pivot.y;
			DistributeVerticalCenter((num3 - num6 - num7) / (float)(num - 1), yMax, rectTransforms);
			break;
		}
		case Doozy.Runtime.UIDesigner.Distribute.VerticalBottom:
		{
			float height = rectTransform.DesignerRect().height;
			DistributeVerticalBottom((num3 - height) / (float)(num - 1), yMin, rectTransforms);
			break;
		}
		case Doozy.Runtime.UIDesigner.Distribute.HorizontalLeft:
		{
			float width2 = rectTransform2.DesignerRect().width;
			DistributeHorizontalLeft((num2 - width2) / (float)(num - 1), xMin, rectTransforms);
			break;
		}
		case Doozy.Runtime.UIDesigner.Distribute.HorizontalCenter:
		{
			float num4 = rectTransform.DesignerRect().width * (1f - rectTransform.pivot.x);
			float num5 = rectTransform2.DesignerRect().width * rectTransform2.pivot.x;
			DistributeHorizontalCenter((num2 - num4 - num5) / (float)(num - 1), xMin, rectTransforms);
			break;
		}
		case Doozy.Runtime.UIDesigner.Distribute.HorizontalRight:
		{
			float width = rectTransform.DesignerRect().width;
			DistributeHorizontalRight((num2 - width) / (float)(num - 1), xMax, rectTransforms);
			break;
		}
		default:
			throw new ArgumentOutOfRangeException("distribute", distribute, null);
		}
	}

	private static IEnumerable<TargetInfo> GetTargetInfos(params RectTransform[] rectTransforms)
	{
		if (rectTransforms == null || rectTransforms.Length == 0)
		{
			return null;
		}
		TargetInfo[] array = new TargetInfo[rectTransforms.Length];
		for (int i = 0; i < rectTransforms.Length; i++)
		{
			array[i] = new TargetInfo(rectTransforms[i]);
		}
		return array;
	}

	public static void StartRotationChange(params RectTransform[] rectTransforms)
	{
		currentRotationInfos.Clear();
		currentRotationInfos.AddRange(GetRotationInfos(rectTransforms));
	}

	public static void StopRotationChange()
	{
		currentRotationInfos.Clear();
	}

	public static void UpdateRotationXY(Space space, float x, float y, bool relativeChange)
	{
		if (currentRotationInfos.Count == 0)
		{
			return;
		}
		foreach (RotationInfo currentRotationInfo in currentRotationInfos)
		{
			currentRotationInfo.SetRotationXY(space, x, y, relativeChange);
		}
	}

	public static void UpdateRotationXZ(Space space, float x, float z, bool relativeChange)
	{
		if (currentRotationInfos.Count == 0)
		{
			return;
		}
		foreach (RotationInfo currentRotationInfo in currentRotationInfos)
		{
			currentRotationInfo.SetRotationXZ(space, x, z, relativeChange);
		}
	}

	public static void UpdateRotationYZ(Space space, float y, float z, bool relativeChange)
	{
		if (currentRotationInfos.Count == 0)
		{
			return;
		}
		foreach (RotationInfo currentRotationInfo in currentRotationInfos)
		{
			currentRotationInfo.SetRotationYZ(space, y, z, relativeChange);
		}
	}

	public static void UpdateRotation(Space space, float x, float y, float z, bool relativeChange)
	{
		if (currentRotationInfos.Count == 0)
		{
			return;
		}
		foreach (RotationInfo currentRotationInfo in currentRotationInfos)
		{
			currentRotationInfo.SetRotation(space, x, y, z, relativeChange);
		}
	}

	public static void UpdateRotation(Space space, Axis axis, float value, bool relativeChange)
	{
		if (currentRotationInfos.Count == 0)
		{
			return;
		}
		foreach (RotationInfo currentRotationInfo in currentRotationInfos)
		{
			currentRotationInfo.SetRotation(space, axis, value, relativeChange);
		}
	}

	private static IEnumerable<RotationInfo> GetRotationInfos(params RectTransform[] rectTransforms)
	{
		if (rectTransforms == null || rectTransforms.Length == 0)
		{
			return null;
		}
		RotationInfo[] array = new RotationInfo[rectTransforms.Length];
		for (int i = 0; i < rectTransforms.Length; i++)
		{
			array[i] = new RotationInfo(rectTransforms[i]);
		}
		return array;
	}

	public static void StartScaleChange(params RectTransform[] rectTransforms)
	{
		currentScaleInfos.Clear();
		currentScaleInfos.AddRange(GetScaleInfos(rectTransforms));
	}

	public static void StopScaleChange()
	{
		currentScaleInfos.Clear();
	}

	public static void UpdateScaleX(float x, bool relativeChange)
	{
		if (currentScaleInfos.Count == 0)
		{
			return;
		}
		foreach (ScaleInfo currentScaleInfo in currentScaleInfos)
		{
			currentScaleInfo.SetScaleX(x, relativeChange);
		}
	}

	public static void UpdateScaleY(float y, bool relativeChange)
	{
		if (currentScaleInfos.Count == 0)
		{
			return;
		}
		foreach (ScaleInfo currentScaleInfo in currentScaleInfos)
		{
			currentScaleInfo.SetScaleY(y, relativeChange);
		}
	}

	public static void UpdateScaleXY(float x, float y, bool relativeChange)
	{
		if (currentScaleInfos.Count == 0)
		{
			return;
		}
		foreach (ScaleInfo currentScaleInfo in currentScaleInfos)
		{
			currentScaleInfo.SetScaleXY(x, y, relativeChange);
		}
	}

	private static IEnumerable<ScaleInfo> GetScaleInfos(params RectTransform[] rectTransforms)
	{
		if (rectTransforms == null || rectTransforms.Length == 0)
		{
			return null;
		}
		ScaleInfo[] array = new ScaleInfo[rectTransforms.Length];
		for (int i = 0; i < rectTransforms.Length; i++)
		{
			array[i] = new ScaleInfo(rectTransforms[i]);
		}
		return array;
	}

	public static void StartSizeChange(params RectTransform[] rectTransforms)
	{
		currentSizeInfos.Clear();
		currentSizeInfos.AddRange(GetSizeInfos(rectTransforms));
	}

	public static void StopSizeChange()
	{
		currentSizeInfos.Clear();
	}

	public static void UpdateSizeX(float x, bool relativeChange)
	{
		if (currentSizeInfos.Count == 0)
		{
			return;
		}
		foreach (SizeInfo currentSizeInfo in currentSizeInfos)
		{
			currentSizeInfo.SetSizeX(x, relativeChange);
		}
	}

	public static void UpdateSizeY(float y, bool relativeChange)
	{
		if (currentSizeInfos.Count == 0)
		{
			return;
		}
		foreach (SizeInfo currentSizeInfo in currentSizeInfos)
		{
			currentSizeInfo.SetSizeY(y, relativeChange);
		}
	}

	public static void UpdateSizeXY(float x, float y, bool relativeChange)
	{
		if (currentSizeInfos.Count == 0)
		{
			return;
		}
		foreach (SizeInfo currentSizeInfo in currentSizeInfos)
		{
			currentSizeInfo.SetSizeXY(x, y, relativeChange);
		}
	}

	private static IEnumerable<SizeInfo> GetSizeInfos(params RectTransform[] rectTransforms)
	{
		if (rectTransforms == null || rectTransforms.Length == 0)
		{
			return null;
		}
		SizeInfo[] array = new SizeInfo[rectTransforms.Length];
		for (int i = 0; i < rectTransforms.Length; i++)
		{
			array[i] = new SizeInfo(rectTransforms[i]);
		}
		return array;
	}
}
