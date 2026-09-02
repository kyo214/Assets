using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.HSSF.Util;

namespace NPOI.SS.Format;

public class CellFormatPart
{
	private class CaseInsensitiveComparator : IEqualityComparer<string>
	{
		public bool Equals(string x, string y)
		{
			return x.Equals(y, StringComparison.InvariantCultureIgnoreCase);
		}

		public int GetHashCode(string obj)
		{
			return obj.GetHashCode();
		}
	}

	public interface IPartHandler
	{
		string HandlePart(Match m, string part, CellFormatType type, StringBuilder desc);
	}

	private Color color;

	private CellFormatCondition condition;

	private CellFormatter format;

	private CellFormatType type;

	private static Dictionary<string, Color> NAMED_COLORS;

	public static IEqualityComparer<string> CASE_INSENSITIVE_ORDER;

	public static Regex COLOR_PAT;

	public static Regex CONDITION_PAT;

	public static Regex SPECIFICATION_PAT;

	public static Regex CURRENCY_PAT;

	public static Regex FORMAT_PAT;

	public static int COLOR_GROUP;

	public static int CONDITION_OPERATOR_GROUP;

	public static int CONDITION_VALUE_GROUP;

	public static int SPECIFICATION_GROUP;

	internal CellFormatType CellFormatType => type;

	internal bool HasCondition => condition != null;

	static CellFormatPart()
	{
		CASE_INSENSITIVE_ORDER = new CaseInsensitiveComparator();
		NAMED_COLORS = new Dictionary<string, Color>(CASE_INSENSITIVE_ORDER);
		foreach (HSSFColor value2 in HSSFColor.GetIndexHash().Values)
		{
			string name = value2.GetType().Name;
			if (name.Equals(name.ToUpper()))
			{
				byte[] rGB = value2.RGB;
				Color value = Color.FromArgb(rGB[0], rGB[1], rGB[2]);
				if (!NAMED_COLORS.ContainsKey(name))
				{
					NAMED_COLORS.Add(name, value);
				}
				if (name.IndexOf('_') > 0 && !NAMED_COLORS.ContainsKey(name.Replace('_', ' ')))
				{
					NAMED_COLORS.Add(name.Replace('_', ' '), value);
				}
				if (name.IndexOf("_PERCENT") > 0 && !NAMED_COLORS.ContainsKey(name.Replace("_PERCENT", "%").Replace('_', ' ')))
				{
					NAMED_COLORS.Add(name.Replace("_PERCENT", "%").Replace('_', ' '), value);
				}
			}
		}
		string text = "([<>=]=?|!=|<>)    # The operator\n  \\s*([0-9]+(?:\\.[0-9]*)?)\\s*  # The constant to test against\n";
		string text2 = "(\\[\\$.{0,3}-[0-9a-f]{3}\\])";
		string text3 = "\\[(black|blue|cyan|green|magenta|red|white|yellow|color [0-9]+)\\]";
		string text4 = "\\\\.                 # Quoted single character\n|\"([^\\\\\"]|\\\\.)*\"         # Quoted string of characters (handles escaped quotes like \\\") \n|" + text2 + "               # Currency symbol in a given locale\n|_.                             # Space as wide as a given character\n|\\*.                           # Repeating fill character\n|@                              # Text: cell text\n|([0?\\#](?:[0?\\#,]*))         # Number: digit + other digits and commas\n|e[-+]                          # Number: Scientific: Exponent\n|m{1,5}                         # Date: month or minute spec\n|d{1,4}                         # Date: day/date spec\n|y{2,4}                         # Date: year spec\n|h{1,2}                         # Date: hour spec\n|s{1,2}                         # Date: second spec\n|am?/pm?                        # Date: am/pm spec\n|\\[h{1,2}\\]                   # Elapsed time: hour spec\n|\\[m{1,2}\\]                   # Elapsed time: minute spec\n|\\[s{1,2}\\]                   # Elapsed time: second spec\n|[^;]                           # A character\n";
		string pattern = "(?:" + text3 + ")?                  # Text color\n(?:\\[" + text + "\\])?                # Condition\n(?:\\[\\$-[0-9a-fA-F]+\\])?                # Optional locale id, ignored currently\n((?:" + text4 + ")+)                        # Format spec\n";
		RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace;
		COLOR_PAT = new Regex(text3, options);
		CONDITION_PAT = new Regex(text, options);
		SPECIFICATION_PAT = new Regex(text4, options);
		CURRENCY_PAT = new Regex(text2, options);
		FORMAT_PAT = new Regex(pattern, options);
		COLOR_GROUP = FindGroup(FORMAT_PAT, "[Blue]@", "Blue");
		CONDITION_OPERATOR_GROUP = FindGroup(FORMAT_PAT, "[>=1]@", ">=");
		CONDITION_VALUE_GROUP = FindGroup(FORMAT_PAT, "[>=1]@", "1");
		SPECIFICATION_GROUP = FindGroup(FORMAT_PAT, "[Blue][>1]\\a ?", "\\a ?");
	}

	public CellFormatPart(string desc)
	{
		Match match = FORMAT_PAT.Match(desc);
		if (!match.Success)
		{
			throw new ArgumentException("Unrecognized format: \"" + desc + "\"");
		}
		color = GetColor(match);
		condition = GetCondition(match);
		type = GetCellFormatType(match);
		format = GetFormatter(match);
	}

	public bool Applies(object valueObject)
	{
		if (condition == null || !valueObject.GetType().IsPrimitive)
		{
			if (valueObject == null)
			{
				throw new NullReferenceException("valueObject");
			}
			return true;
		}
		double value = (double)valueObject;
		return condition.Pass(value);
	}

	private static int FindGroup(Regex pat, string str, string marker)
	{
		Match match = pat.Match(str);
		if (!match.Success)
		{
			throw new ArgumentException("Pattern \"" + pat.ToString() + "\" doesn't match \"" + str + "\"");
		}
		for (int i = 1; i <= match.Groups.Count; i++)
		{
			string value = match.Groups[i].Value;
			if (value != null && value.Equals(marker))
			{
				return i;
			}
		}
		throw new ArgumentException("\"" + marker + "\" not found in \"" + pat.ToString() + "\"");
	}

	private static Color GetColor(Match m)
	{
		string text = m.Groups[COLOR_GROUP].Value.ToUpper();
		if (text == null || text.Length == 0)
		{
			return Color.Empty;
		}
		Color result = Color.Empty;
		if (NAMED_COLORS.ContainsKey(text))
		{
			result = NAMED_COLORS[text];
		}
		return result;
	}

	private CellFormatCondition GetCondition(Match m)
	{
		string value = m.Groups[CONDITION_OPERATOR_GROUP].Value;
		if (value == null || value.Length == 0)
		{
			return null;
		}
		return CellFormatCondition.GetInstance(m.Groups[CONDITION_OPERATOR_GROUP].Value, m.Groups[CONDITION_VALUE_GROUP].Value);
	}

	private CellFormatType GetCellFormatType(Match matcher)
	{
		string value = matcher.Groups[SPECIFICATION_GROUP].Value;
		return formatType(value);
	}

	private CellFormatter GetFormatter(Match matcher)
	{
		string text = matcher.Groups[SPECIFICATION_GROUP].Value;
		Match match = CURRENCY_PAT.Match(text);
		if (match.Success)
		{
			string value = match.Groups[1].Value;
			string newValue = ((!value.StartsWith("[$-")) ? value.Substring(2, value.LastIndexOf('-') - 2) : "$");
			text = text.Replace(value, newValue);
		}
		return type.Formatter(text);
	}

	private CellFormatType formatType(string fdesc)
	{
		fdesc = fdesc.Trim();
		if (fdesc.Equals("") || fdesc.Equals("General", StringComparison.InvariantCultureIgnoreCase))
		{
			return CellFormatType.GENERAL;
		}
		MatchCollection matchCollection = SPECIFICATION_PAT.Matches(fdesc);
		bool flag = false;
		bool flag2 = false;
		foreach (Match item in matchCollection)
		{
			string value = item.Groups[0].Value;
			if (value.Length <= 0)
			{
				continue;
			}
			char c = value[0];
			char c2 = '\0';
			if (value.Length > 1)
			{
				c2 = char.ToLower(value[1]);
			}
			switch (c)
			{
			case '@':
				return CellFormatType.TEXT;
			case 'D':
			case 'Y':
			case 'd':
			case 'y':
				return CellFormatType.DATE;
			case 'H':
			case 'M':
			case 'S':
			case 'h':
			case 'm':
			case 's':
				flag = true;
				break;
			case '0':
				flag2 = true;
				break;
			case '[':
				switch (c2)
				{
				case 'h':
				case 'm':
				case 's':
					return CellFormatType.ELAPSED;
				case '$':
					return CellFormatType.NUMBER;
				default:
					throw new ArgumentException("Unsupported [] format block '" + value + "' in '" + fdesc + "' with c2: " + c2);
				}
			case '#':
			case '?':
				return CellFormatType.NUMBER;
			}
		}
		if (flag)
		{
			return CellFormatType.DATE;
		}
		if (flag2)
		{
			return CellFormatType.NUMBER;
		}
		return CellFormatType.TEXT;
	}

	private static string QuoteSpecial(string repl, CellFormatType type)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in repl)
		{
			if (c == '\'' && type.IsSpecial('\''))
			{
				stringBuilder.Append('\0');
				continue;
			}
			bool num = type.IsSpecial(c);
			if (num)
			{
				stringBuilder.Append("'");
			}
			stringBuilder.Append(c);
			if (num)
			{
				stringBuilder.Append("'");
			}
		}
		return stringBuilder.ToString();
	}

	public CellFormatResult Apply(object value)
	{
		bool num = Applies(value);
		string text;
		Color empty;
		if (num)
		{
			text = format.Format(value);
			empty = color;
		}
		else
		{
			text = format.SimpleFormat(value);
			empty = Color.Empty;
		}
		return new CellFormatResult(num, text, empty);
	}

	public static StringBuilder ParseFormat(string fdesc, CellFormatType type, IPartHandler partHandler)
	{
		MatchCollection matchCollection = SPECIFICATION_PAT.Matches(fdesc);
		StringBuilder stringBuilder = new StringBuilder();
		Match match = null;
		foreach (Match item in matchCollection)
		{
			string text = Group(item, 0);
			if (text.Length > 0)
			{
				string text2 = partHandler.HandlePart(item, text, type, stringBuilder);
				if (text2 == null)
				{
					text2 = text[0] switch
					{
						'"' => QuoteSpecial(text.Substring(1, text.Length - 2), type), 
						'\\' => QuoteSpecial(text.Substring(1), type), 
						'_' => " ", 
						'*' => ExpandChar(text), 
						_ => text, 
					};
				}
				stringBuilder.Append(text.Replace(item.Captures[0].Value, text2));
				if (item.NextMatch().Index - (item.Index + text.Length) > 0)
				{
					stringBuilder.Append(fdesc.Substring(item.Index + text.Length, item.NextMatch().Index - (item.Index + text.Length)));
				}
				match = item;
			}
		}
		if (match != null)
		{
			stringBuilder.Append(fdesc.Substring(match.Index + match.Groups[0].Value.Length));
		}
		if (type.IsSpecial('\''))
		{
			int startIndex = 0;
			while ((startIndex = stringBuilder.ToString().IndexOf("''", startIndex)) >= 0)
			{
				stringBuilder.Remove(startIndex, 2);
			}
			startIndex = 0;
			while ((startIndex = stringBuilder.ToString().IndexOf('\0', startIndex)) >= 0)
			{
				stringBuilder.Remove(startIndex, 1);
				stringBuilder.Insert(startIndex, "''");
			}
		}
		return stringBuilder;
	}

	public static string QuoteReplacement(string s)
	{
		if (s.IndexOf('\\') == -1 && s.IndexOf('$') == -1)
		{
			return s;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in s)
		{
			if (c == '\\' || c == '$')
			{
				stringBuilder.Append('\\');
			}
			stringBuilder.Append(c);
		}
		return stringBuilder.ToString();
	}

	internal static string ExpandChar(string part)
	{
		char c = part[1];
		return c.ToString() + c + c;
	}

	public static string Group(Match m, int g)
	{
		string value = m.Groups[g].Value;
		if (value != null)
		{
			return value;
		}
		return "";
	}

	public override string ToString()
	{
		return format.ToString();
	}
}
