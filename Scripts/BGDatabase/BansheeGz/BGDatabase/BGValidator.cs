using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGValidator
{
	public class ValidatorAttribute : Attribute
	{
		public Type Target;
	}

	protected static void FillInValidators<T>(Dictionary<Type, Type> type2Validator) where T : BGValidator
	{
		List<Type> allSubTypes = BGUtil.GetAllSubTypes(typeof(T));
		foreach (Type item in allSubTypes)
		{
			ValidatorAttribute attribute = BGUtil.GetAttribute<ValidatorAttribute>(item);
			if (attribute != null && !(attribute.Target == null))
			{
				type2Validator.Add(attribute.Target, item);
			}
		}
	}

	protected static void Add(BGValidationLog[] logs, string message, params object[] parameters)
	{
		if (logs != null && logs.Length != 0 && !string.IsNullOrEmpty(message))
		{
			string error = BGUtil.Format(message, parameters);
			for (int i = 0; i < logs.Length; i++)
			{
				logs[i].Add(error);
			}
		}
	}
}
