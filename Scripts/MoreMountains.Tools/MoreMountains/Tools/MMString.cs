using System.Globalization;
using System.Text.RegularExpressions;

namespace MoreMountains.Tools;

public static class MMString
{
	public static string UppercaseFirst(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return string.Empty;
		}
		return char.ToUpper(s[0]) + s.Substring(1);
	}

	public static int RichTextLength(string richText)
	{
		int num = 0;
		bool flag = false;
		richText = richText.Replace("<br>", "-");
		string text = richText;
		for (int i = 0; i < text.Length; i++)
		{
			switch (text[i])
			{
			case '<':
				flag = true;
				continue;
			case '>':
				flag = false;
				continue;
			}
			if (!flag)
			{
				num++;
			}
		}
		return num;
	}

	public static string ToTitleCase(this string title)
	{
		return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
	}

	public static string RemoveExtraSpaces(this string s)
	{
		return Regex.Replace(s, "\\s+", " ");
	}
}
