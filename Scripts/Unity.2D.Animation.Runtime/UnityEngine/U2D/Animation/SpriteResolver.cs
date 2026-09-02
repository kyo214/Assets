using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.U2D.Common;

namespace UnityEngine.U2D.Animation;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[AddComponentMenu("2D Animation/Sprite Resolver")]
[DefaultExecutionOrder(-2)]
[HelpURL("https://docs.unity3d.com/Packages/com.unity.2d.animation@9.0/manual/SL-Resolver.html")]
[MovedFrom("UnityEngine.Experimental.U2D.Animation")]
public class SpriteResolver : MonoBehaviour, ISerializationCallbackReceiver, IPreviewable
{
	[SerializeField]
	private float m_CategoryHash;

	[SerializeField]
	private float m_labelHash;

	[SerializeField]
	private float m_SpriteKey;

	[SerializeField]
	[DiscreteEvaluation]
	private int m_SpriteHash;

	private int m_CategoryHashInt;

	private int m_LabelHashInt;

	private int m_PreviousCategoryHash;

	private int m_PreviousLabelHash;

	private int m_PreviousSpriteKeyInt;

	private int m_PreviousSpriteHash;

	private SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();

	public SpriteLibrary spriteLibrary => base.gameObject.GetComponentInParent<SpriteLibrary>(includeInactive: true);

	private void Reset()
	{
		if ((bool)spriteRenderer)
		{
			SetSprite(spriteRenderer.sprite);
		}
	}

	private void SetSprite(Sprite sprite)
	{
		SpriteLibrary spriteLibrary = this.spriteLibrary;
		if (!(spriteLibrary != null) || !(sprite != null))
		{
			return;
		}
		foreach (string categoryName in spriteLibrary.categoryNames)
		{
			foreach (string entryName in spriteLibrary.GetEntryNames(categoryName))
			{
				if (spriteLibrary.GetSprite(categoryName, entryName) == sprite)
				{
					m_SpriteHash = SpriteLibrary.GetHashForCategoryAndEntry(categoryName, entryName);
					return;
				}
			}
		}
	}

	private void OnEnable()
	{
		InitializeSerializedData();
		ResolveSpriteToSpriteRenderer();
	}

	private void InitializeSerializedData()
	{
		m_CategoryHashInt = InternalEngineBridge.ConvertFloatToInt(m_CategoryHash);
		m_LabelHashInt = InternalEngineBridge.ConvertFloatToInt(m_labelHash);
		m_PreviousSpriteKeyInt = SpriteLibraryUtility.Convert32BitTo30BitHash(InternalEngineBridge.ConvertFloatToInt(m_SpriteKey));
		m_SpriteKey = InternalEngineBridge.ConvertIntToFloat(m_PreviousSpriteKeyInt);
		if (m_SpriteHash == 0)
		{
			if (m_SpriteKey != 0f)
			{
				m_SpriteHash = InternalEngineBridge.ConvertFloatToInt(m_SpriteKey);
			}
			else
			{
				m_SpriteHash = ConvertCategoryLabelHashToSpriteKey(spriteLibrary, SpriteLibraryUtility.Convert32BitTo30BitHash(m_CategoryHashInt), SpriteLibraryUtility.Convert32BitTo30BitHash(m_LabelHashInt));
			}
		}
		m_PreviousSpriteHash = m_SpriteHash;
		if (spriteLibrary != null && spriteLibrary.GetCategoryAndEntryNameFromHash(m_SpriteHash, out var category, out var entry))
		{
			m_CategoryHashInt = SpriteLibraryUtility.GetStringHash(category);
			m_LabelHashInt = SpriteLibraryUtility.GetStringHash(entry);
			m_CategoryHash = InternalEngineBridge.ConvertIntToFloat(m_CategoryHashInt);
			m_labelHash = InternalEngineBridge.ConvertIntToFloat(m_LabelHashInt);
		}
		m_PreviousLabelHash = m_LabelHashInt;
		m_PreviousCategoryHash = m_CategoryHashInt;
	}

	public bool SetCategoryAndLabel(string category, string label)
	{
		m_SpriteHash = SpriteLibrary.GetHashForCategoryAndEntry(category, label);
		m_PreviousSpriteHash = m_SpriteHash;
		return ResolveSpriteToSpriteRenderer();
	}

	public string GetCategory()
	{
		string category = "";
		SpriteLibrary spriteLibrary = this.spriteLibrary;
		if ((bool)spriteLibrary)
		{
			spriteLibrary.GetCategoryAndEntryNameFromHash(m_SpriteHash, out category, out var _);
		}
		return category;
	}

	public string GetLabel()
	{
		string entry = "";
		SpriteLibrary spriteLibrary = this.spriteLibrary;
		if ((bool)spriteLibrary)
		{
			spriteLibrary.GetCategoryAndEntryNameFromHash(m_SpriteHash, out var _, out entry);
		}
		return entry;
	}

	public void OnPreviewUpdate()
	{
	}

	private static bool IsInGUIUpdateLoop()
	{
		return Event.current != null;
	}

	private void LateUpdate()
	{
		ResolveUpdatedValue();
	}

	private void ResolveUpdatedValue()
	{
		if (m_SpriteHash != m_PreviousSpriteHash)
		{
			m_PreviousSpriteHash = m_SpriteHash;
			ResolveSpriteToSpriteRenderer();
			return;
		}
		int num = InternalEngineBridge.ConvertFloatToInt(m_SpriteKey);
		if (num != m_PreviousSpriteKeyInt)
		{
			m_SpriteHash = SpriteLibraryUtility.Convert32BitTo30BitHash(num);
			m_PreviousSpriteKeyInt = num;
			ResolveSpriteToSpriteRenderer();
			return;
		}
		m_CategoryHashInt = InternalEngineBridge.ConvertFloatToInt(m_CategoryHash);
		m_LabelHashInt = InternalEngineBridge.ConvertFloatToInt(m_labelHash);
		if ((m_LabelHashInt != m_PreviousLabelHash || m_CategoryHashInt != m_PreviousCategoryHash) && spriteLibrary != null)
		{
			m_PreviousCategoryHash = m_CategoryHashInt;
			m_PreviousLabelHash = m_LabelHashInt;
			m_SpriteHash = ConvertCategoryLabelHashToSpriteKey(spriteLibrary, SpriteLibraryUtility.Convert32BitTo30BitHash(m_CategoryHashInt), SpriteLibraryUtility.Convert32BitTo30BitHash(m_LabelHashInt));
			m_PreviousSpriteHash = m_SpriteHash;
			ResolveSpriteToSpriteRenderer();
		}
	}

	internal static int ConvertCategoryLabelHashToSpriteKey(SpriteLibrary library, int categoryHash, int labelHash)
	{
		if (library != null)
		{
			foreach (string categoryName in library.categoryNames)
			{
				if (categoryHash != SpriteLibraryUtility.GetStringHash(categoryName))
				{
					continue;
				}
				IEnumerable<string> entryNames = library.GetEntryNames(categoryName);
				if (entryNames == null)
				{
					continue;
				}
				foreach (string item in entryNames)
				{
					if (labelHash == SpriteLibraryUtility.GetStringHash(item))
					{
						return SpriteLibrary.GetHashForCategoryAndEntry(categoryName, item);
					}
				}
			}
		}
		return 0;
	}

	internal Sprite GetSprite(out bool validEntry)
	{
		SpriteLibrary spriteLibrary = this.spriteLibrary;
		if (spriteLibrary != null)
		{
			return spriteLibrary.GetSpriteFromCategoryAndEntryHash(m_SpriteHash, out validEntry);
		}
		validEntry = false;
		return null;
	}

	public bool ResolveSpriteToSpriteRenderer()
	{
		m_PreviousSpriteHash = m_SpriteHash;
		Sprite sprite = GetSprite(out var validEntry);
		SpriteRenderer spriteRenderer = this.spriteRenderer;
		if (spriteRenderer != null && ((sprite != null) | validEntry))
		{
			spriteRenderer.sprite = sprite;
		}
		return validEntry;
	}

	private void OnTransformParentChanged()
	{
		ResolveSpriteToSpriteRenderer();
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
	}
}
