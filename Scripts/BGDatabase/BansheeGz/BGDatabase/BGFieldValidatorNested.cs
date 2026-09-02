using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

[Validator(Target = typeof(BGFieldNested))]
public class BGFieldValidatorNested : BGFieldValidator
{
	private BGField _field;

	private BGFieldValidator[] fieldValidators;

	protected override BGField field
	{
		get
		{
			return _field;
		}
		set
		{
			_field = value;
			BGMetaNested nestedMeta = ((BGFieldNested)_field).NestedMeta;
			fieldValidators = new BGFieldValidator[nestedMeta.CountFields];
			for (int i = 0; i < fieldValidators.Length; i++)
			{
				BGFieldValidator validator = BGFieldValidator.GetValidator(nestedMeta.GetField(i));
				fieldValidators[i] = validator;
			}
		}
	}

	public override void Validate(BGEntity entity, Func<BGValidationLog[]> logsProvider)
	{
		base.Validate(entity, logsProvider);
		BGFieldNested bGFieldNested = (BGFieldNested)field;
		List<BGEntity> list = bGFieldNested[entity.Index];
		if (BGUtil.IsEmpty(list))
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			BGEntity entity2 = list[i];
			for (int j = 0; j < fieldValidators.Length; j++)
			{
				BGFieldValidator bGFieldValidator = fieldValidators[j];
				bGFieldValidator.Validate(entity2, logsProvider);
			}
		}
	}
}
