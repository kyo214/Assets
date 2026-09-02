using System;
using System.Collections.Generic;
using System.Text;

namespace BansheeGz.BGDatabase;

public class BGValidationLog
{
	private readonly List<string> errors = new List<string>();

	public bool HasError => errors.Count > 0;

	public int Errors => errors.Count;

	public void Clear()
	{
		errors.Clear();
	}

	public void Add(string error, params object[] parameters)
	{
		if (!string.IsNullOrEmpty(error))
		{
			errors.Add(BGUtil.Format(error, parameters));
		}
	}

	public override string ToString()
	{
		return ToString(errors.Count);
	}

	public string ToString(int maxLines)
	{
		if (errors.Count == 0)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		int i;
		for (i = 0; i < maxLines && i < errors.Count; i++)
		{
			string value = errors[i];
			stringBuilder.Append(i + 1).Append(") ").Append(value)
				.Append(Environment.NewLine);
		}
		if (i < errors.Count)
		{
			stringBuilder.Append(errors.Count - maxLines + " more errors...");
		}
		return stringBuilder.ToString();
	}
}
