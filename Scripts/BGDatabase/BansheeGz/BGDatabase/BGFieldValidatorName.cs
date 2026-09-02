using System;

namespace BansheeGz.BGDatabase;

[Validator(Target = typeof(BGFieldEntityName))]
public class BGFieldValidatorName : BGFieldValidator
{
	public override void Validate(BGEntity entity, Func<BGValidationLog[]> logsProvider)
	{
		base.Validate(entity, logsProvider);
		if (entity.Meta.EmptyName)
		{
			string value = ((BGFieldEntityName)field)[entity.Index];
			if (!string.IsNullOrEmpty(value))
			{
				BGValidator.Add(logsProvider(), "Meta [$] is marked to have empty entity name, however entity #$ [$] has name value", entity.Meta.Name, entity.Index, entity.Name);
			}
		}
	}
}
