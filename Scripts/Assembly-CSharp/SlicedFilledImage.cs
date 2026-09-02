using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.U2D;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
[AddComponentMenu("UI/Sliced Filled Image", 11)]
public class SlicedFilledImage : MaskableGraphic, ISerializationCallbackReceiver, ILayoutElement, ICanvasRaycastFilter
{
	private static class SetPropertyUtility
	{
		public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
		{
			if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}
	}

	public enum FillDirection
	{
		Right = 0,
		Left = 1,
		Up = 2,
		Down = 3
	}

	private static readonly Vector3[] s_Vertices = new Vector3[4];

	private static readonly Vector2[] s_UVs = new Vector2[4];

	private static readonly Vector2[] s_SlicedVertices = new Vector2[4];

	private static readonly Vector2[] s_SlicedUVs = new Vector2[4];

	[SerializeField]
	private Sprite m_Sprite;

	[SerializeField]
	private FillDirection m_FillDirection;

	[Range(0f, 1f)]
	[SerializeField]
	private float m_FillAmount = 1f;

	[SerializeField]
	private bool m_FillCenter = true;

	[SerializeField]
	private float m_PixelsPerUnitMultiplier = 1f;

	[NonSerialized]
	private Sprite m_OverrideSprite;

	private bool m_Tracked;

	private static List<SlicedFilledImage> m_TrackedTexturelessImages = new List<SlicedFilledImage>();

	private static bool s_Initialized;

	public Sprite sprite
	{
		get
		{
			return m_Sprite;
		}
		set
		{
			if (SetPropertyUtility.SetClass(ref m_Sprite, value))
			{
				SetAllDirty();
				TrackImage();
			}
		}
	}

	public FillDirection fillDirection
	{
		get
		{
			return m_FillDirection;
		}
		set
		{
			if (SetPropertyUtility.SetStruct(ref m_FillDirection, value))
			{
				SetVerticesDirty();
			}
		}
	}

	public float fillAmount
	{
		get
		{
			return m_FillAmount;
		}
		set
		{
			if (SetPropertyUtility.SetStruct(ref m_FillAmount, Mathf.Clamp01(value)))
			{
				SetVerticesDirty();
			}
		}
	}

	public bool fillCenter
	{
		get
		{
			return m_FillCenter;
		}
		set
		{
			if (SetPropertyUtility.SetStruct(ref m_FillCenter, value))
			{
				SetVerticesDirty();
			}
		}
	}

	public float pixelsPerUnitMultiplier
	{
		get
		{
			return m_PixelsPerUnitMultiplier;
		}
		set
		{
			m_PixelsPerUnitMultiplier = Mathf.Max(0.01f, value);
		}
	}

	public float pixelsPerUnit
	{
		get
		{
			float num = 100f;
			if ((bool)activeSprite)
			{
				num = activeSprite.pixelsPerUnit;
			}
			float num2 = 100f;
			if ((bool)base.canvas)
			{
				num2 = base.canvas.referencePixelsPerUnit;
			}
			return m_PixelsPerUnitMultiplier * num / num2;
		}
	}

	public Sprite overrideSprite
	{
		get
		{
			return activeSprite;
		}
		set
		{
			if (SetPropertyUtility.SetClass(ref m_OverrideSprite, value))
			{
				SetAllDirty();
				TrackImage();
			}
		}
	}

	private Sprite activeSprite
	{
		get
		{
			if (!(m_OverrideSprite != null))
			{
				return m_Sprite;
			}
			return m_OverrideSprite;
		}
	}

	public override Texture mainTexture
	{
		get
		{
			if (activeSprite != null)
			{
				return activeSprite.texture;
			}
			if (!(material != null) || !(material.mainTexture != null))
			{
				return Graphic.s_WhiteTexture;
			}
			return material.mainTexture;
		}
	}

	public bool hasBorder
	{
		get
		{
			if (activeSprite != null)
			{
				return activeSprite.border.sqrMagnitude > 0f;
			}
			return false;
		}
	}

	public override Material material
	{
		get
		{
			if (m_Material != null)
			{
				return m_Material;
			}
			if ((bool)activeSprite && activeSprite.associatedAlphaSplitTexture != null)
			{
				return Image.defaultETC1GraphicMaterial;
			}
			return defaultMaterial;
		}
		set
		{
			base.material = value;
		}
	}

	public float alphaHitTestMinimumThreshold { get; set; }

	int ILayoutElement.layoutPriority => 0;

	float ILayoutElement.minWidth => 0f;

	float ILayoutElement.minHeight => 0f;

	float ILayoutElement.flexibleWidth => -1f;

	float ILayoutElement.flexibleHeight => -1f;

	float ILayoutElement.preferredWidth
	{
		get
		{
			if (activeSprite == null)
			{
				return 0f;
			}
			return DataUtility.GetMinSize(activeSprite).x / pixelsPerUnit;
		}
	}

	float ILayoutElement.preferredHeight
	{
		get
		{
			if (activeSprite == null)
			{
				return 0f;
			}
			return DataUtility.GetMinSize(activeSprite).y / pixelsPerUnit;
		}
	}

	protected SlicedFilledImage()
	{
		base.useLegacyMeshGeneration = false;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		TrackImage();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (m_Tracked)
		{
			UnTrackImage();
		}
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		if (activeSprite == null)
		{
			base.OnPopulateMesh(vh);
		}
		else
		{
			GenerateSlicedFilledSprite(vh);
		}
	}

	protected override void UpdateMaterial()
	{
		base.UpdateMaterial();
		if (activeSprite == null)
		{
			base.canvasRenderer.SetAlphaTexture(null);
			return;
		}
		Texture2D associatedAlphaSplitTexture = activeSprite.associatedAlphaSplitTexture;
		if (associatedAlphaSplitTexture != null)
		{
			base.canvasRenderer.SetAlphaTexture(associatedAlphaSplitTexture);
		}
	}

	private void GenerateSlicedFilledSprite(VertexHelper vh)
	{
		vh.Clear();
		if (m_FillAmount < 0.001f)
		{
			return;
		}
		Rect pixelAdjustedRect = GetPixelAdjustedRect();
		Vector4 outerUV = DataUtility.GetOuterUV(activeSprite);
		Vector4 padding = DataUtility.GetPadding(activeSprite);
		if (!hasBorder)
		{
			Vector2 size = activeSprite.rect.size;
			int num = Mathf.RoundToInt(size.x);
			int num2 = Mathf.RoundToInt(size.y);
			Vector4 vertices = new Vector4(pixelAdjustedRect.x + pixelAdjustedRect.width * (padding.x / (float)num), pixelAdjustedRect.y + pixelAdjustedRect.height * (padding.y / (float)num2), pixelAdjustedRect.x + pixelAdjustedRect.width * (((float)num - padding.z) / (float)num), pixelAdjustedRect.y + pixelAdjustedRect.height * (((float)num2 - padding.w) / (float)num2));
			GenerateFilledSprite(vh, vertices, outerUV, m_FillAmount);
			return;
		}
		Vector4 innerUV = DataUtility.GetInnerUV(activeSprite);
		Vector4 adjustedBorders = GetAdjustedBorders(activeSprite.border / pixelsPerUnit, pixelAdjustedRect);
		padding /= pixelsPerUnit;
		s_SlicedVertices[0] = new Vector2(padding.x, padding.y);
		s_SlicedVertices[3] = new Vector2(pixelAdjustedRect.width - padding.z, pixelAdjustedRect.height - padding.w);
		s_SlicedVertices[1].x = adjustedBorders.x;
		s_SlicedVertices[1].y = adjustedBorders.y;
		s_SlicedVertices[2].x = pixelAdjustedRect.width - adjustedBorders.z;
		s_SlicedVertices[2].y = pixelAdjustedRect.height - adjustedBorders.w;
		for (int i = 0; i < 4; i++)
		{
			s_SlicedVertices[i].x += pixelAdjustedRect.x;
			s_SlicedVertices[i].y += pixelAdjustedRect.y;
		}
		s_SlicedUVs[0] = new Vector2(outerUV.x, outerUV.y);
		s_SlicedUVs[1] = new Vector2(innerUV.x, innerUV.y);
		s_SlicedUVs[2] = new Vector2(innerUV.z, innerUV.w);
		s_SlicedUVs[3] = new Vector2(outerUV.z, outerUV.w);
		float num3;
		float num5;
		if (m_FillDirection == FillDirection.Left || m_FillDirection == FillDirection.Right)
		{
			num3 = s_SlicedVertices[0].x;
			float num4 = s_SlicedVertices[3].x - s_SlicedVertices[0].x;
			num5 = ((num4 > 0f) ? (1f / num4) : 1f);
		}
		else
		{
			num3 = s_SlicedVertices[0].y;
			float num6 = s_SlicedVertices[3].y - s_SlicedVertices[0].y;
			num5 = ((num6 > 0f) ? (1f / num6) : 1f);
		}
		for (int j = 0; j < 3; j++)
		{
			int num7 = j + 1;
			for (int k = 0; k < 3; k++)
			{
				if (m_FillCenter || j != 1 || k != 1)
				{
					int num8 = k + 1;
					float num9;
					float num10;
					switch (m_FillDirection)
					{
					case FillDirection.Right:
						num9 = (s_SlicedVertices[j].x - num3) * num5;
						num10 = (s_SlicedVertices[num7].x - num3) * num5;
						break;
					case FillDirection.Up:
						num9 = (s_SlicedVertices[k].y - num3) * num5;
						num10 = (s_SlicedVertices[num8].y - num3) * num5;
						break;
					case FillDirection.Left:
						num9 = 1f - (s_SlicedVertices[num7].x - num3) * num5;
						num10 = 1f - (s_SlicedVertices[j].x - num3) * num5;
						break;
					case FillDirection.Down:
						num9 = 1f - (s_SlicedVertices[num8].y - num3) * num5;
						num10 = 1f - (s_SlicedVertices[k].y - num3) * num5;
						break;
					default:
						num9 = (num10 = 0f);
						break;
					}
					if (!(num9 >= m_FillAmount))
					{
						Vector4 vertices2 = new Vector4(s_SlicedVertices[j].x, s_SlicedVertices[k].y, s_SlicedVertices[num7].x, s_SlicedVertices[num8].y);
						Vector4 uvs = new Vector4(s_SlicedUVs[j].x, s_SlicedUVs[k].y, s_SlicedUVs[num7].x, s_SlicedUVs[num8].y);
						float num11 = (m_FillAmount - num9) / (num10 - num9);
						GenerateFilledSprite(vh, vertices2, uvs, num11);
					}
				}
			}
		}
	}

	private Vector4 GetAdjustedBorders(Vector4 border, Rect adjustedRect)
	{
		Rect rect = base.rectTransform.rect;
		for (int i = 0; i <= 1; i++)
		{
			if (rect.size[i] != 0f)
			{
				float num = adjustedRect.size[i] / rect.size[i];
				border[i] *= num;
				border[i + 2] *= num;
			}
			float num2 = border[i] + border[i + 2];
			if (adjustedRect.size[i] < num2 && num2 != 0f)
			{
				float num = adjustedRect.size[i] / num2;
				border[i] *= num;
				border[i + 2] *= num;
			}
		}
		return border;
	}

	private void GenerateFilledSprite(VertexHelper vh, Vector4 vertices, Vector4 uvs, float fillAmount)
	{
		if (m_FillAmount < 0.001f)
		{
			return;
		}
		float num = uvs.x;
		float num2 = uvs.y;
		float num3 = uvs.z;
		float num4 = uvs.w;
		if (fillAmount < 1f)
		{
			if (m_FillDirection == FillDirection.Left || m_FillDirection == FillDirection.Right)
			{
				if (m_FillDirection == FillDirection.Left)
				{
					vertices.x = vertices.z - (vertices.z - vertices.x) * fillAmount;
					num = num3 - (num3 - num) * fillAmount;
				}
				else
				{
					vertices.z = vertices.x + (vertices.z - vertices.x) * fillAmount;
					num3 = num + (num3 - num) * fillAmount;
				}
			}
			else if (m_FillDirection == FillDirection.Down)
			{
				vertices.y = vertices.w - (vertices.w - vertices.y) * fillAmount;
				num2 = num4 - (num4 - num2) * fillAmount;
			}
			else
			{
				vertices.w = vertices.y + (vertices.w - vertices.y) * fillAmount;
				num4 = num2 + (num4 - num2) * fillAmount;
			}
		}
		s_Vertices[0] = new Vector3(vertices.x, vertices.y);
		s_Vertices[1] = new Vector3(vertices.x, vertices.w);
		s_Vertices[2] = new Vector3(vertices.z, vertices.w);
		s_Vertices[3] = new Vector3(vertices.z, vertices.y);
		s_UVs[0] = new Vector2(num, num2);
		s_UVs[1] = new Vector2(num, num4);
		s_UVs[2] = new Vector2(num3, num4);
		s_UVs[3] = new Vector2(num3, num2);
		int currentVertCount = vh.currentVertCount;
		for (int i = 0; i < 4; i++)
		{
			vh.AddVert(s_Vertices[i], color, s_UVs[i]);
		}
		vh.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
		vh.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
	}

	void ILayoutElement.CalculateLayoutInputHorizontal()
	{
	}

	void ILayoutElement.CalculateLayoutInputVertical()
	{
	}

	bool ICanvasRaycastFilter.IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
	{
		if (alphaHitTestMinimumThreshold <= 0f)
		{
			return true;
		}
		if (alphaHitTestMinimumThreshold > 1f)
		{
			return false;
		}
		if (activeSprite == null)
		{
			return true;
		}
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, screenPoint, eventCamera, out var localPoint))
		{
			return false;
		}
		Rect pixelAdjustedRect = GetPixelAdjustedRect();
		localPoint.x += base.rectTransform.pivot.x * pixelAdjustedRect.width;
		localPoint.y += base.rectTransform.pivot.y * pixelAdjustedRect.height;
		Rect rect = activeSprite.rect;
		Vector4 border = activeSprite.border;
		Vector4 adjustedBorders = GetAdjustedBorders(border / pixelsPerUnit, pixelAdjustedRect);
		for (int i = 0; i < 2; i++)
		{
			if (!(localPoint[i] <= adjustedBorders[i]))
			{
				if (pixelAdjustedRect.size[i] - localPoint[i] <= adjustedBorders[i + 2])
				{
					localPoint[i] -= pixelAdjustedRect.size[i] - rect.size[i];
					continue;
				}
				float t = Mathf.InverseLerp(adjustedBorders[i], pixelAdjustedRect.size[i] - adjustedBorders[i + 2], localPoint[i]);
				localPoint[i] = Mathf.Lerp(border[i], rect.size[i] - border[i + 2], t);
			}
		}
		Rect textureRect = activeSprite.textureRect;
		Vector2 vector = new Vector2(localPoint.x / textureRect.width, localPoint.y / textureRect.height);
		float num = Mathf.Lerp(textureRect.x, textureRect.xMax, vector.x) / (float)activeSprite.texture.width;
		float num2 = Mathf.Lerp(textureRect.y, textureRect.yMax, vector.y) / (float)activeSprite.texture.height;
		switch (m_FillDirection)
		{
		case FillDirection.Right:
			if (num > m_FillAmount)
			{
				return false;
			}
			break;
		case FillDirection.Left:
			if (1f - num > m_FillAmount)
			{
				return false;
			}
			break;
		case FillDirection.Up:
			if (num2 > m_FillAmount)
			{
				return false;
			}
			break;
		case FillDirection.Down:
			if (1f - num2 > m_FillAmount)
			{
				return false;
			}
			break;
		}
		try
		{
			return activeSprite.texture.GetPixelBilinear(num, num2).a >= alphaHitTestMinimumThreshold;
		}
		catch (UnityException ex)
		{
			Debug.LogError("Using alphaHitTestMinimumThreshold greater than 0 on Image whose sprite texture cannot be read. " + ex.Message + " Also make sure to disable sprite packing for this sprite.", this);
			return true;
		}
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		m_FillAmount = Mathf.Clamp01(m_FillAmount);
	}

	private void TrackImage()
	{
		if (activeSprite != null && activeSprite.texture == null)
		{
			if (!s_Initialized)
			{
				SpriteAtlasManager.atlasRegistered += RebuildImage;
				s_Initialized = true;
			}
			m_TrackedTexturelessImages.Add(this);
			m_Tracked = true;
		}
	}

	private void UnTrackImage()
	{
		m_TrackedTexturelessImages.Remove(this);
		m_Tracked = false;
	}

	private static void RebuildImage(SpriteAtlas spriteAtlas)
	{
		for (int num = m_TrackedTexturelessImages.Count - 1; num >= 0; num--)
		{
			SlicedFilledImage slicedFilledImage = m_TrackedTexturelessImages[num];
			if (spriteAtlas.CanBindTo(slicedFilledImage.activeSprite))
			{
				slicedFilledImage.SetAllDirty();
				m_TrackedTexturelessImages.RemoveAt(num);
			}
		}
	}
}
