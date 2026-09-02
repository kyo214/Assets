using System;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Animations;
using UnityEngine;

namespace Doozy.Runtime.Reactor.ScriptableObjects;

[Serializable]
public class UIAnimationPreset : ScriptableObject
{
	private const string DEFAULT_ASSET_FILENAME = "AnimationPreset";

	private const string INITIAL_CATEGORY_NAME = "PresetCategory";

	private const string INITIAL_PRESET_NAME = "PresetName";

	[SerializeField]
	private string Category = "PresetCategory";

	[SerializeField]
	private string PresetName = "PresetName";

	[SerializeField]
	private UIAnimationSettings Settings;

	private static string dataFolderPath => BasePathFinder<RuntimePath>.path + "/Data/UIAnimationPresets";

	private string dataFileName => DataFileName(animationType, category, presetName);

	private string dataFilePath => dataFolderPath + "/" + dataFileName;

	public UIAnimationType animationType => Settings.animationType;

	public string category
	{
		get
		{
			return Category;
		}
		set
		{
			UpdateCategory(value);
		}
	}

	public string presetName
	{
		get
		{
			return PresetName;
		}
		set
		{
			UpdatePresetName(value);
		}
	}

	public UIAnimationSettings settings => Settings;

	public UIAnimationPreset()
		: this(UIAnimationType.Custom)
	{
	}

	public UIAnimationPreset(UIAnimationType animationType, string category = "PresetCategory", string presetName = "PresetName")
	{
		Settings = new UIAnimationSettings(animationType);
		this.category = category;
		this.presetName = presetName;
	}

	public UIAnimationPreset(UIAnimation source, string category = "PresetCategory", string presetName = "PresetName")
	{
		Settings = new UIAnimationSettings(source);
		this.category = category;
		this.presetName = presetName;
	}

	public void CleanCategory()
	{
		category = category;
	}

	public void CleanPresetName()
	{
		presetName = presetName;
	}

	public static string CleanString(string value)
	{
		return value.RemoveWhitespaces().RemoveAllSpecialCharacters();
	}

	private void UpdateCategory(string value, bool updateAssetFileName = false)
	{
		value = CleanString(value);
		if (!value.IsNullOrEmpty())
		{
			Category = value;
			if (updateAssetFileName)
			{
				UpdateAssetFileName();
			}
		}
	}

	private void UpdatePresetName(string value, bool updateAssetFileName = false)
	{
		value = CleanString(value);
		if (!value.IsNullOrEmpty())
		{
			PresetName = value;
			if (updateAssetFileName)
			{
				UpdateAssetFileName();
			}
		}
	}

	private void UpdateAssetFileName()
	{
	}

	public UIAnimationPreset RenamePreset(string newCategory, string newPresetName)
	{
		category = newCategory;
		presetName = newPresetName;
		UpdateAssetFileName();
		return this;
	}

	public UIAnimationPreset SetCategory(string value)
	{
		category = value;
		return this;
	}

	public UIAnimationPreset SetPresetName(string value)
	{
		presetName = value;
		return this;
	}

	public UIAnimationPreset GetAnimationSettings(UIAnimation source)
	{
		Settings.GetAnimationSettings(source);
		return this;
	}

	public UIAnimationPreset SetAnimationSettings(UIAnimation target)
	{
		Settings.SetAnimationSettings(target);
		return this;
	}

	public static UIAnimationPreset NewPreset(UIAnimation source, string category, string presetName, string path = "")
	{
		Debug.LogWarning("Unable to execute. A new preset can only be created in the Unity Editor");
		return null;
	}

	internal static UIAnimationPreset NewDefaultPreset(UIAnimationType animationType)
	{
		Debug.LogWarning("Unable to execute. A new preset can only be created in the Unity Editor");
		return null;
	}

	public static string DataFileName(UIAnimationType animationType, string category, string presetName, string extension = ".asset")
	{
		return $"{animationType}_{category}_{presetName}{extension}";
	}
}
