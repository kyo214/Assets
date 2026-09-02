using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;

namespace Doozy.Runtime.Reactor.ScriptableObjects.Internal;

[Serializable]
public class UIAnimationPresetGroup
{
	[SerializeField]
	private UIAnimationType GroupAnimationType;

	[SerializeField]
	private List<UIAnimationPreset> Presets = new List<UIAnimationPreset>();

	[SerializeField]
	private List<string> CategoryNames = new List<string>();

	[SerializeField]
	private List<PresetCategory> PresetCategories = new List<PresetCategory>();

	public static string defaultCategoryName => "Default";

	public static string defaultPresetName => "Default";

	public List<UIAnimationPreset> presets => Presets;

	public List<string> categoryNames => CategoryNames;

	public List<PresetCategory> presetCategories => PresetCategories;

	public UIAnimationPresetGroup(UIAnimationType animationType)
	{
		GroupAnimationType = animationType;
	}

	public UIAnimationPresetGroup AddPreset(UIAnimationPreset preset, bool validate = false, bool sort = false, bool allowDefaultPresets = false)
	{
		var (flag, message) = CanAddPreset(preset, allowDefaultPresets);
		if (!flag)
		{
			Debug.Log(message);
			return this;
		}
		Presets.Add(preset);
		PresetCategory category = GetCategory(preset.category);
		if (category != null)
		{
			category.AddName(preset.presetName);
		}
		else
		{
			PresetCategories.Add(new PresetCategory(preset.category).AddName(preset.presetName));
			CategoryNames.Add(preset.category);
		}
		if (validate)
		{
			Validate();
		}
		if (sort)
		{
			Sort();
		}
		return this;
	}

	private bool ContainsCategory(string categoryName)
	{
		return PresetCategories.Any((PresetCategory c) => c.Category.Equals(categoryName.RemoveAllSpecialCharacters().RemoveWhitespaces()));
	}

	public PresetCategory GetCategory(string categoryName)
	{
		return PresetCategories.FirstOrDefault((PresetCategory presetCategory) => presetCategory.Category.Equals(categoryName));
	}

	public List<string> GetPresetNames(string categoryName)
	{
		foreach (PresetCategory presetCategory in PresetCategories)
		{
			if (presetCategory.Category.Equals(categoryName))
			{
				return presetCategory.Names;
			}
		}
		return null;
	}

	internal void AddDefaultPreset()
	{
		if (!Contains(defaultCategoryName, defaultPresetName))
		{
			Presets.Add(UIAnimationPreset.NewDefaultPreset(GroupAnimationType));
			PresetCategories.Add(new PresetCategory(defaultCategoryName).AddName(defaultPresetName));
			CategoryNames.Add(defaultCategoryName);
			Validate(addDefaultPreset: false);
			Sort();
		}
	}

	public void RemoveCategory(string category)
	{
		PresetCategories = PresetCategories.Where((PresetCategory pc) => pc.Category != category).ToList();
		CategoryNames.Remove(category);
	}

	public bool RemovePreset(UIAnimationPreset preset)
	{
		if (preset == null)
		{
			return false;
		}
		if (!Contains(preset))
		{
			return false;
		}
		Presets.Remove(preset);
		if (!ContainsCategory(preset.category))
		{
			return true;
		}
		PresetCategory category = GetCategory(preset.category);
		category.Names.Remove(preset.presetName);
		if (category.Names.Count == 0)
		{
			RemoveCategory(category.Category);
		}
		return true;
	}

	public bool RemovePreset(string category, string presetName)
	{
		return RemovePreset(GetPreset(category, presetName));
	}

	public UIAnimationPreset GetPreset(string category, string presetName)
	{
		return Presets.Where((UIAnimationPreset p) => p.category.Equals(category)).FirstOrDefault((UIAnimationPreset p) => p.presetName.Equals(presetName));
	}

	public UIAnimationPresetGroup Clear()
	{
		Presets.Clear();
		PresetCategories.Clear();
		CategoryNames.Clear();
		return this;
	}

	public bool Contains(UIAnimationPreset preset)
	{
		return Presets.Contains(preset);
	}

	public bool Contains(string category, string presetName)
	{
		return GetPreset(category, presetName) != null;
	}

	public (bool, string) CanAddPreset(UIAnimationPreset preset, bool allowDefaultPresets = false)
	{
		if (preset == null)
		{
			return (false, "preset is null");
		}
		preset.CleanCategory();
		preset.CleanPresetName();
		if (Presets.Contains(preset))
		{
			return (false, "Preset already exists in the database");
		}
		return CanAddPreset(preset.animationType, preset.category, preset.presetName, allowDefaultPresets);
	}

	public (bool, string) CanAddPreset(UIAnimationType animationType, string category, string presetName, bool allowDefaultPresets = false)
	{
		category = UIAnimationPreset.CleanString(category);
		presetName = UIAnimationPreset.CleanString(presetName);
		if (animationType != GroupAnimationType)
		{
			return (false, $"Preset AnimationType: '{animationType}' is different than the Preset Group AnimationType: '{GroupAnimationType}'");
		}
		if (category.IsNullOrEmpty())
		{
			return (false, "Category cannot be null or empty");
		}
		if (presetName.IsNullOrEmpty())
		{
			return (false, "Preset name cannot be null or empty");
		}
		if (!allowDefaultPresets)
		{
			if (category.Equals(defaultCategoryName))
			{
				return (false, "Cannot add any presets to the '" + defaultCategoryName + "' category");
			}
			if (presetName.Equals(defaultPresetName))
			{
				return (false, "Cannot use '" + defaultPresetName + "' as a preset name");
			}
		}
		foreach (UIAnimationPreset preset in Presets)
		{
			if (preset.category.Equals(category) && preset.presetName.Equals(presetName))
			{
				return (false, "Another preset with the '" + presetName + "' name already exists in the '" + category + "' category. Change the preset name and/or the category and try again.");
			}
		}
		return (true, "Preset can be added to this group");
	}

	public UIAnimationPresetGroup Validate(bool addDefaultPreset = true)
	{
		if (addDefaultPreset)
		{
			AddDefaultPreset();
		}
		Presets = (from p in Presets.Distinct()
			where p != null
			select p).ToList();
		PresetCategories = PresetCategories.Where((PresetCategory pc) => pc.Names.Count > 0).ToList();
		CategoryNames.Clear();
		foreach (PresetCategory presetCategory in PresetCategories)
		{
			CategoryNames.Add(presetCategory.Category);
		}
		return this;
	}

	public UIAnimationPresetGroup Sort()
	{
		Presets = (from p in Presets
			orderby p.category, p.presetName
			select p).ToList();
		foreach (PresetCategory presetCategory in presetCategories)
		{
			presetCategory.Names.Sort();
		}
		CategoryNames.Sort();
		return this;
	}
}
