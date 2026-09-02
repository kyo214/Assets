using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGMetaEntityValidator : BGValidator
{
	private static readonly Dictionary<Type, Type> MetaType2ValidatorType = new Dictionary<Type, Type>();

	private readonly HashSet<string> uniqueNames = new HashSet<string>();

	private readonly HashSet<string> duplicateNames = new HashSet<string>();

	protected BGMetaEntity meta;

	private BGFieldEntityName nameField;

	public static BGMetaEntityValidator GetValidator(BGMetaEntity meta)
	{
		if (MetaType2ValidatorType.Count == 0)
		{
			BGValidator.FillInValidators<BGMetaEntityValidator>(MetaType2ValidatorType);
		}
		Type type = BGUtil.Get(MetaType2ValidatorType, meta.GetType());
		BGMetaEntityValidator bGMetaEntityValidator = ((type != null) ? BGUtil.Create<BGMetaEntityValidator>(type, includePrivateConstructors: false, Array.Empty<object>()) : new BGMetaEntityValidator());
		bGMetaEntityValidator.meta = meta;
		return bGMetaEntityValidator;
	}

	public virtual void Start(params BGValidationLog[] logs)
	{
		nameField = meta.NameField;
		if (meta.Singleton && meta.CountEntities > 1)
		{
			BGValidator.Add(logs, "Meta [$] is a singleton. There are [$] entities exist.", meta.Name, meta.CountEntities);
		}
	}

	public virtual void Validate(BGEntity entity, params BGValidationLog[] logs)
	{
		if (meta.UniqueName)
		{
			string text = nameField[entity.Index];
			if (text != null && !string.IsNullOrEmpty(text.Trim()) && !uniqueNames.Add(text))
			{
				duplicateNames.Add(text);
			}
		}
	}

	public void Finish(params BGValidationLog[] logs)
	{
		if (duplicateNames.Count <= 0)
		{
			return;
		}
		string text = "";
		foreach (string duplicateName in duplicateNames)
		{
			if (text.Length > 0)
			{
				text += ",";
			}
			text += duplicateName;
		}
		BGValidator.Add(logs, "Entity name should be unique for meta [$], but there are following duplicate names: $", meta.Name, text);
	}
}
