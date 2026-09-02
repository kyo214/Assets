using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public static class BGLocalizationUglyHacks
{
	public const string DataBinderLocale = "$locale";

	private static BGDBField.FieldValueProvider localeFieldProvider;

	private static BGDBTextBinderField.BGDBTextBinderFieldSpecial LocaleBinder;

	public static readonly HashSet<string> GoogleSheetsSpecialFieldTypeNames;

	public static bool CanEnableDelta => !HasLocalizationAddon(BGRepo.I);

	public static BGMetaTypeCodeFactory.BGMetaTypeCodeFactoryI LocalizationMetaFactory { get; }

	public static BGFieldTypeCodeFactory.BGFieldTypeCodeFactoryI LocalizationFieldFactory { get; }

	static BGLocalizationUglyHacks()
	{
		GoogleSheetsSpecialFieldTypeNames = new HashSet<string> { "BansheeGz.BGDatabase.BGFieldLocalizedAudioClip", "BansheeGz.BGDatabase.BGFieldLocalizedMaterial", "BansheeGz.BGDatabase.BGFieldLocalizedObject", "BansheeGz.BGDatabase.BGFieldLocalizedPrefab", "BansheeGz.BGDatabase.BGFieldLocalizedSprite", "BansheeGz.BGDatabase.BGFieldLocalizedString", "BansheeGz.BGDatabase.BGFieldLocalizedText", "BansheeGz.BGDatabase.BGFieldLocalizedTexture" };
		Type type = BGUtil.GetType("BansheeGz.BGDatabase.BGMetaLocalizationTypeCodeFactory");
		if (type != null)
		{
			LocalizationMetaFactory = Activator.CreateInstance(type) as BGMetaTypeCodeFactory.BGMetaTypeCodeFactoryI;
		}
		Type type2 = BGUtil.GetType("BansheeGz.BGDatabase.BGFieldLocalizationTypeCodeFactory");
		if (type2 != null)
		{
			LocalizationFieldFactory = Activator.CreateInstance(type2) as BGFieldTypeCodeFactory.BGFieldTypeCodeFactoryI;
		}
	}

	public static BGDBField.FieldValueProvider DataBindingInitValueProvider(string fieldIdString)
	{
		if (fieldIdString != null && fieldIdString.StartsWith("$locale"))
		{
			if (localeFieldProvider == null)
			{
				try
				{
					localeFieldProvider = BGUtil.Create<BGDBField.FieldValueProvider>("BansheeGz.BGDatabase.BGDBLocaleFieldValueProvider", includePrivateConstructors: false, Array.Empty<object>());
				}
				catch
				{
					return null;
				}
			}
			return localeFieldProvider.Create();
		}
		return null;
	}

	public static bool DataBindingBind(string fieldName, BGDBTextBinderRoot root, BGDBTextBinderField.Pointer pointer)
	{
		if (!"$locale".Equals(fieldName))
		{
			return false;
		}
		if (LocaleBinder == null)
		{
			try
			{
				LocaleBinder = BGUtil.Create<BGDBTextBinderField.BGDBTextBinderFieldSpecial>("BansheeGz.BGDatabase.BGDBLocaleFieldBinder", includePrivateConstructors: false, Array.Empty<object>());
			}
			catch (Exception ex)
			{
				root.Error = "Can not create locale binder:" + ex.Message;
				return true;
			}
		}
		BGDBTextBinder binder = LocaleBinder.Create(pointer);
		root.Add(binder);
		return true;
	}

	public static bool GoogleSheetsHasField(string type)
	{
		return GoogleSheetsSpecialFieldTypeNames.Contains(type);
	}

	public static bool SupportPartitioning(BGMetaEntity meta)
	{
		return !"Locale".Equals(meta.Name);
	}

	public static bool HasLocaleField(BGMetaEntity meta)
	{
		Type type = BGUtil.GetType("BansheeGz.BGDatabase.BGFieldLocalizedI");
		if (type == null)
		{
			return false;
		}
		return HasLocaleField(meta, type);
	}

	private static bool HasLocaleField(BGMetaEntity meta, Type localizedType)
	{
		if (meta.FindField(localizedType.IsInstanceOfType) != null)
		{
			return true;
		}
		List<BGField> list = meta.FindFields(null, (BGField f) => f is BGFieldNested);
		foreach (BGField item in list)
		{
			BGFieldNested bGFieldNested = (BGFieldNested)item;
			if (HasLocaleField(bGFieldNested.RelatedMeta, localizedType))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsLocaleField(BGField field)
	{
		Type type = BGUtil.GetType("BansheeGz.BGDatabase.BGFieldLocalizationI");
		if (type == null)
		{
			return false;
		}
		return type.IsInstanceOfType(field);
	}

	public static bool IsLocaleField(Type fieldType)
	{
		Type type = BGUtil.GetType("BansheeGz.BGDatabase.BGFieldLocalizationI");
		if (type == null)
		{
			return false;
		}
		return type.IsAssignableFrom(fieldType);
	}

	public static bool HasLocalizationAddon(BGRepo repo)
	{
		return repo.Addons.Has("BansheeGz.BGDatabase.BGAddonLocalization");
	}

	public static bool IsLocalizationSettings(BGMetaEntity meta)
	{
		return meta.GetType().FullName == "BansheeGz.BGDatabase.BGMetaLocalizationSettings";
	}

	public static bool IsLocalesTable(BGMetaEntity meta)
	{
		if (meta is BGMetaNested bGMetaNested)
		{
			return IsLocalizationSettings(bGMetaNested.Owner);
		}
		return false;
	}
}
