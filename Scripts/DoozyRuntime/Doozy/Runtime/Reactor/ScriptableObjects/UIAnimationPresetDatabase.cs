using System;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.ScriptableObjects.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.ScriptableObjects;

[Serializable]
public class UIAnimationPresetDatabase : ScriptableObject
{
	private static UIAnimationPresetDatabase s_instance;

	[SerializeField]
	private UIAnimationPresetGroup ShowPresets = new UIAnimationPresetGroup(UIAnimationType.Show);

	[SerializeField]
	private UIAnimationPresetGroup HidePresets = new UIAnimationPresetGroup(UIAnimationType.Hide);

	[SerializeField]
	private UIAnimationPresetGroup LoopPresets = new UIAnimationPresetGroup(UIAnimationType.Loop);

	[SerializeField]
	private UIAnimationPresetGroup ButtonPresets = new UIAnimationPresetGroup(UIAnimationType.Button);

	[SerializeField]
	private UIAnimationPresetGroup StatePresets = new UIAnimationPresetGroup(UIAnimationType.State);

	[SerializeField]
	private UIAnimationPresetGroup ResetPresets = new UIAnimationPresetGroup(UIAnimationType.Reset);

	[SerializeField]
	private UIAnimationPresetGroup CustomPresets = new UIAnimationPresetGroup(UIAnimationType.Custom);

	private static string fileName => "UIAnimationPresetDatabase";

	private static string assetFileName => fileName + ".asset";

	private static string assetFolderPath => BasePathFinder<RuntimePath>.path + "/Data";

	private static string assetFilePath => assetFolderPath + "/" + assetFileName;

	public static UIAnimationPresetDatabase instance
	{
		get
		{
			if (s_instance != null)
			{
				return s_instance;
			}
			s_instance = ScriptableObject.CreateInstance<UIAnimationPresetDatabase>();
			return s_instance;
		}
	}

	public static string defaultCategoryName => UIAnimationPresetGroup.defaultCategoryName;

	public static string defaultPresetName => UIAnimationPresetGroup.defaultPresetName;

	public UIAnimationPresetGroup showPresets => ShowPresets;

	public UIAnimationPresetGroup hidePresets => HidePresets;

	public UIAnimationPresetGroup loopPresets => LoopPresets;

	public UIAnimationPresetGroup buttonPresets => ButtonPresets;

	public UIAnimationPresetGroup statePresets => StatePresets;

	public UIAnimationPresetGroup resetPresets => ResetPresets;

	public UIAnimationPresetGroup customPresets => CustomPresets;

	public UIAnimationPresetGroup GetPresetGroup(UIAnimationType animationType)
	{
		return animationType switch
		{
			UIAnimationType.Show => showPresets, 
			UIAnimationType.Hide => hidePresets, 
			UIAnimationType.Loop => loopPresets, 
			UIAnimationType.Button => buttonPresets, 
			UIAnimationType.State => statePresets, 
			UIAnimationType.Reset => resetPresets, 
			UIAnimationType.Custom => customPresets, 
			_ => throw new ArgumentOutOfRangeException("animationType", animationType, null), 
		};
	}

	[RefreshData("UIAnimationPresetDatabase")]
	public static void RefreshData()
	{
		instance.RefreshDatabase();
	}

	public void RefreshDatabase(bool saveAssets = true, bool refreshAssetDatabase = false)
	{
	}

	public UIAnimationPresetDatabase Validate()
	{
		ShowPresets.Validate();
		HidePresets.Validate();
		LoopPresets.Validate();
		ButtonPresets.Validate();
		StatePresets.Validate();
		ResetPresets.Validate();
		CustomPresets.Validate();
		return this;
	}

	public UIAnimationPresetDatabase Sort()
	{
		ShowPresets.Sort();
		HidePresets.Sort();
		LoopPresets.Sort();
		ButtonPresets.Sort();
		StatePresets.Sort();
		ResetPresets.Sort();
		CustomPresets.Sort();
		return this;
	}

	public (bool, string) CanAddPreset(UIAnimationType animationType, string category, string presetName)
	{
		return GetPresetGroup(animationType).CanAddPreset(animationType, category, presetName);
	}

	public void AddPreset(UIAnimationPreset preset, bool saveAssets = true, bool allowDefaultPresets = false)
	{
		if (!(preset == null) && !ContainsPreset(preset) && ValidatePreset(preset))
		{
			GetPresetGroup(preset.animationType).AddPreset(preset, validate: false, sort: false, allowDefaultPresets);
		}
	}

	public UIAnimationPreset GetDefaultPreset(UIAnimationType animationType)
	{
		return GetPresetGroup(animationType).GetPreset(defaultCategoryName, defaultPresetName);
	}

	public UIAnimationPreset GetPreset(UIAnimationType animationType, string category, string presetName)
	{
		return GetPresetGroup(animationType).GetPreset(category, presetName);
	}

	public bool ContainsPreset(UIAnimationType animationType, string category, string presetName)
	{
		return GetPresetGroup(animationType).Contains(category, presetName);
	}

	public bool ContainsPreset(UIAnimationPreset preset)
	{
		if (preset != null)
		{
			return GetPresetGroup(preset.animationType).Contains(preset);
		}
		return false;
	}

	public bool RemovePreset(UIAnimationType animationType, string category, string presetName)
	{
		return GetPresetGroup(animationType).RemovePreset(category, presetName);
	}

	public bool RemovePreset(UIAnimationPreset preset)
	{
		if (preset != null)
		{
			return GetPresetGroup(preset.animationType).RemovePreset(preset);
		}
		return false;
	}

	private bool ValidatePreset(UIAnimationPreset preset)
	{
		if (!(preset.category.IsNullOrEmpty() | preset.presetName.IsNullOrEmpty()))
		{
			return true;
		}
		return false;
	}
}
