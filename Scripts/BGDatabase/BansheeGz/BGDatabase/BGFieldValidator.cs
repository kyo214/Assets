using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGFieldValidator : BGValidator
{
	private static readonly Dictionary<Type, Type> Type2Validator = new Dictionary<Type, Type>();

	protected virtual BGField field { get; set; }

	public static BGFieldValidator GetValidator(BGField field)
	{
		if (Type2Validator.Count == 0)
		{
			BGValidator.FillInValidators<BGFieldValidator>(Type2Validator);
		}
		Type type = BGUtil.Get(Type2Validator, field.GetType());
		BGFieldValidator bGFieldValidator = null;
		if (type != null)
		{
			bGFieldValidator = BGUtil.Create<BGFieldValidator>(type, includePrivateConstructors: false, Array.Empty<object>());
		}
		else if (field is BGFieldUnityAssetI)
		{
			Type type2 = BGUtil.GetType("BansheeGz.BGDatabase.Editor.BGFieldValidatorAsset");
			if (type2 != null)
			{
				bGFieldValidator = BGUtil.Create<BGFieldValidator>(type2, includePrivateConstructors: false, Array.Empty<object>());
			}
		}
		if (bGFieldValidator == null)
		{
			bGFieldValidator = new BGFieldValidator();
		}
		bGFieldValidator.field = field;
		return bGFieldValidator;
	}

	public virtual void Validate(BGEntity entity, Func<BGValidationLog[]> logsProvider)
	{
		if (field.Required && field.GetValue(entity.Id) == null)
		{
			BGValidator.Add(logsProvider(), "Field [$] is required, but has no value at entity #$ [$]", field.FullName, entity.Index, entity.FullName);
		}
	}
}
