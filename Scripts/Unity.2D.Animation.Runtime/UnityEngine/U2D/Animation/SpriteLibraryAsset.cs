using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.U2D.Animation;

[HelpURL("https://docs.unity3d.com/Packages/com.unity.2d.animation@9.0/manual/AssetUpgrader.html#upgrading-sprite-libraries")]
[MovedFrom("UnityEngine.Experimental.U2D.Animation")]
public class SpriteLibraryAsset : ScriptableObject
{
	[SerializeField]
	private List<SpriteLibCategory> m_Labels = new List<SpriteLibCategory>();

	[SerializeField]
	private long m_ModificationHash;

	[SerializeField]
	private int m_Version;

	internal List<SpriteLibCategory> categories
	{
		get
		{
			return m_Labels;
		}
		set
		{
			m_Labels = value;
			ValidateCategories();
		}
	}

	internal long modificationHash
	{
		get
		{
			return m_ModificationHash;
		}
		set
		{
			m_ModificationHash = value;
		}
	}

	internal int version
	{
		set
		{
			m_Version = value;
		}
	}

	internal static SpriteLibraryAsset CreateAsset(List<SpriteLibCategory> categories, string assetName, long modificationHash)
	{
		SpriteLibraryAsset spriteLibraryAsset = ScriptableObject.CreateInstance<SpriteLibraryAsset>();
		spriteLibraryAsset.m_Labels = categories;
		spriteLibraryAsset.ValidateCategories();
		spriteLibraryAsset.name = assetName;
		spriteLibraryAsset.UpdateHashes();
		spriteLibraryAsset.m_ModificationHash = modificationHash;
		spriteLibraryAsset.version = 1;
		return spriteLibraryAsset;
	}

	private void OnEnable()
	{
		if (m_Version < 1)
		{
			UpdateToVersionOne();
		}
	}

	private void UpdateToVersionOne()
	{
		UpdateHashes();
		m_Version = 1;
	}

	internal Sprite GetSprite(int categoryHash, int labelHash)
	{
		SpriteLibCategory spriteLibCategory = m_Labels.FirstOrDefault((SpriteLibCategory x) => x.hash == categoryHash);
		if (spriteLibCategory != null)
		{
			SpriteCategoryEntry spriteCategoryEntry = spriteLibCategory.categoryList.FirstOrDefault((SpriteCategoryEntry x) => x.hash == labelHash);
			if (spriteCategoryEntry != null)
			{
				return spriteCategoryEntry.sprite;
			}
		}
		return null;
	}

	internal Sprite GetSprite(int categoryHash, int labelHash, out bool validEntry)
	{
		SpriteLibCategory spriteLibCategory = null;
		for (int i = 0; i < m_Labels.Count; i++)
		{
			if (m_Labels[i].hash == categoryHash)
			{
				spriteLibCategory = m_Labels[i];
				break;
			}
		}
		if (spriteLibCategory != null)
		{
			SpriteCategoryEntry spriteCategoryEntry = null;
			for (int j = 0; j < spriteLibCategory.categoryList.Count; j++)
			{
				if (spriteLibCategory.categoryList[j].hash == labelHash)
				{
					spriteCategoryEntry = spriteLibCategory.categoryList[j];
					break;
				}
			}
			if (spriteCategoryEntry != null)
			{
				validEntry = true;
				return spriteCategoryEntry.sprite;
			}
		}
		validEntry = false;
		return null;
	}

	public Sprite GetSprite(string category, string label)
	{
		int categoryHash = SpriteLibraryUtility.GetStringHash(category);
		int labelHash = SpriteLibraryUtility.GetStringHash(label);
		return GetSprite(categoryHash, labelHash);
	}

	public IEnumerable<string> GetCategoryNames()
	{
		return m_Labels.Select((SpriteLibCategory x) => x.name);
	}

	[Obsolete("GetCategorylabelNames has been deprecated. Please use GetCategoryLabelNames (UnityUpgradable) -> GetCategoryLabelNames(*)")]
	public IEnumerable<string> GetCategorylabelNames(string category)
	{
		return GetCategoryLabelNames(category);
	}

	public IEnumerable<string> GetCategoryLabelNames(string category)
	{
		SpriteLibCategory spriteLibCategory = m_Labels.FirstOrDefault((SpriteLibCategory x) => x.name == category);
		if (spriteLibCategory != null)
		{
			return spriteLibCategory.categoryList.Select((SpriteCategoryEntry x) => x.name);
		}
		return new string[0];
	}

	public void AddCategoryLabel(Sprite sprite, string category, string label)
	{
		category = category.Trim();
		label = label?.Trim();
		if (string.IsNullOrEmpty(category))
		{
			throw new ArgumentException("Cannot add empty or null Category string");
		}
		int catHash = SpriteLibraryUtility.GetStringHash(category);
		SpriteCategoryEntry spriteCategoryEntry = null;
		SpriteLibCategory spriteLibCategory = null;
		spriteLibCategory = m_Labels.FirstOrDefault((SpriteLibCategory x) => x.hash == catHash);
		if (spriteLibCategory != null)
		{
			if (string.IsNullOrEmpty(label))
			{
				throw new ArgumentException("Cannot add empty or null Label string");
			}
			int labelHash = SpriteLibraryUtility.GetStringHash(label);
			spriteCategoryEntry = spriteLibCategory.categoryList.FirstOrDefault((SpriteCategoryEntry y) => y.hash == labelHash);
			if (spriteCategoryEntry != null)
			{
				spriteCategoryEntry.sprite = sprite;
				return;
			}
			spriteCategoryEntry = new SpriteCategoryEntry
			{
				name = label,
				sprite = sprite
			};
			spriteLibCategory.categoryList.Add(spriteCategoryEntry);
		}
		else
		{
			SpriteLibCategory spriteLibCategory2 = new SpriteLibCategory
			{
				categoryList = new List<SpriteCategoryEntry>(),
				name = category
			};
			if (!string.IsNullOrEmpty(label))
			{
				spriteLibCategory2.categoryList.Add(new SpriteCategoryEntry
				{
					name = label,
					sprite = sprite
				});
			}
			m_Labels.Add(spriteLibCategory2);
		}
	}

	public void RemoveCategoryLabel(string category, string label, bool deleteCategory)
	{
		int catHash = SpriteLibraryUtility.GetStringHash(category);
		SpriteLibCategory libCategory = null;
		libCategory = m_Labels.FirstOrDefault((SpriteLibCategory x) => x.hash == catHash);
		if (libCategory == null)
		{
			return;
		}
		int labelHash = SpriteLibraryUtility.GetStringHash(label);
		libCategory.categoryList.RemoveAll((SpriteCategoryEntry x) => x.hash == labelHash);
		if (deleteCategory && libCategory.categoryList.Count == 0)
		{
			m_Labels.RemoveAll((SpriteLibCategory x) => x.hash == libCategory.hash);
		}
	}

	internal void UpdateHashes()
	{
		foreach (SpriteLibCategory label in m_Labels)
		{
			label.UpdateHash();
		}
	}

	internal void ValidateCategories(bool log = true)
	{
		RenameDuplicate(m_Labels, (string originalName, string newName) =>
		{
			if (log)
			{
				Debug.LogWarning("Category " + originalName + " renamed to " + newName + " due to hash clash");
			}
		});
		for (int num = 0; num < m_Labels.Count; num++)
		{
			m_Labels[num].ValidateLabels(log);
		}
	}

	internal static void RenameDuplicate(IEnumerable<INameHash> nameHashList, Action<string, string> onRename)
	{
		for (int i = 0; i < nameHashList.Count(); i++)
		{
			INameHash category = nameHashList.ElementAt(i);
			IEnumerable<INameHash> source = nameHashList.Where((INameHash x) => (x.hash == category.hash || x.name == category.name) && x != category);
			int num = 0;
			for (int num2 = 0; num2 < source.Count(); num2++)
			{
				INameHash categoryClash = source.ElementAt(num2);
				for (; num < 1000; num++)
				{
					string name = categoryClash.name;
					name = $"{name}_{num}";
					int nameHash = SpriteLibraryUtility.GetStringHash(name);
					if (nameHashList.FirstOrDefault((INameHash x) => (x.hash == nameHash || x.name == name) && x != categoryClash) == null)
					{
						onRename(categoryClash.name, name);
						categoryClash.name = name;
						break;
					}
				}
			}
		}
	}
}
