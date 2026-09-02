using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.OpenXml4Net.Exceptions;

namespace NPOI.OpenXml4Net.OPC.Internal;

public class ContentType : IComparable
{
	private string type;

	private string subType;

	private Hashtable p;

	private Dictionary<string, string> parameters;

	private static Regex patternTypeSubType;

	private static Regex patternTypeSubTypeParams;

	private static Regex patternParams;

	public string SubType => subType;

	public string Type => type;

	static ContentType()
	{
		string text = "[\\x21\\x23-\\x27\\x2A\\x2B\\x2D\\x2E0-9A-Z\\x5E\\x5F\\x60a-z\\x7E]";
		string text2 = "(" + text + "+)=(\"?" + text + "+\"?)";
		patternTypeSubType = new Regex("^(" + text + "+)/(" + text + "+)$");
		patternTypeSubTypeParams = new Regex("^(" + text + "+)/(" + text + "+)(;" + text2 + ")*$");
		patternParams = new Regex(";" + text2);
	}

	public ContentType(string contentType)
	{
		Match match = patternTypeSubType.Match(contentType);
		if (!match.Success)
		{
			match = patternTypeSubTypeParams.Match(contentType);
		}
		if (!match.Success)
		{
			throw new InvalidFormatException("The specified content type '" + contentType + "' is not compliant with RFC 2616: malformed content type.");
		}
		if (match.Groups.Count >= 2)
		{
			type = match.Groups[1].Value;
			subType = match.Groups[2].Value;
			parameters = new Dictionary<string, string>();
			if (match.Groups.Count >= 5)
			{
				Match match2 = patternParams.Match(contentType.Substring(match.Groups[2].Index + match.Groups[2].Length));
				while (match2.Success)
				{
					parameters.Add(match2.Groups[1].Value, match2.Groups[2].Value);
					match2 = match2.NextMatch();
				}
			}
		}
		else
		{
			type = "";
			subType = "";
			parameters = new Dictionary<string, string>();
		}
	}

	public override string ToString()
	{
		return ToString(withParameters: true);
	}

	public string ToString(bool withParameters)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(Type);
		stringBuilder.Append("/");
		stringBuilder.Append(SubType);
		if (withParameters)
		{
			foreach (KeyValuePair<string, string> parameter in parameters)
			{
				stringBuilder.Append(";");
				stringBuilder.Append(parameter.Key);
				stringBuilder.Append("=");
				stringBuilder.Append(parameter.Value);
			}
		}
		return stringBuilder.ToString();
	}

	public string ToStringWithParameters()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(ToString());
		foreach (string key in parameters.Keys)
		{
			stringBuilder.Append(";");
			stringBuilder.Append(key);
			stringBuilder.Append("=");
			stringBuilder.Append(parameters[key]);
		}
		return stringBuilder.ToString();
	}

	public override bool Equals(object obj)
	{
		if (obj is ContentType)
		{
			return ToString().Equals(obj.ToString(), StringComparison.InvariantCultureIgnoreCase);
		}
		return true;
	}

	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}

	public bool HasParameters()
	{
		if (parameters != null)
		{
			return parameters.Count != 0;
		}
		return false;
	}

	public string[] GetParameterKeys()
	{
		if (parameters == null)
		{
			return new string[0];
		}
		List<string> list = new List<string>();
		list.AddRange(parameters.Keys);
		return list.ToArray();
	}

	public string GetParameter(string key)
	{
		return parameters[key];
	}

	public int CompareTo(object obj)
	{
		if (obj == null)
		{
			return -1;
		}
		if (Equals(obj))
		{
			return 0;
		}
		return 1;
	}
}
