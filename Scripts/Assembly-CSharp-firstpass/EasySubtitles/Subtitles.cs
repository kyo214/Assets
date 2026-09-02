using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace EasySubtitles;

public class Subtitles : List<Subtitle>
{
	public float Duration => base[base.Count - 1].End;

	public Subtitles(TextAsset textAsset)
	{
		if (textAsset == null)
		{
			throw new ArgumentNullException("textAsset");
		}
		Parse(textAsset.text);
	}

	public Subtitles(string subfile)
	{
		Parse(subfile);
	}

	private void Parse(string subfile)
	{
		string[] array = subfile.Split(new string[1] { Environment.NewLine + Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
		foreach (string text in array)
		{
			RegexOptions options = RegexOptions.Multiline;
			Match match = Regex.Match(text, "^(\\d+)\\r?\\n(\\d{2}:\\d{2}:\\d{2},\\d{3})\\s-->\\s(\\d{2}:\\d{2}:\\d{2},\\d{3})(?:\\sX1:(-?\\d+)\\sX2:(-?\\d+)\\sY1:(-?\\d+)\\sY2:(-?\\d+))?\\r?\\n([\\S\\s]+)$", options);
			if (!match.Success)
			{
				Debug.LogError("Invalid subtitle block: " + text);
				continue;
			}
			int index = int.Parse(match.Groups[1].Value);
			float start = ParseTime(match.Groups[2].Value);
			float end = ParseTime(match.Groups[3].Value);
			string text2 = FixFormatting(match.Groups[8].Value.Trim());
			int x = ((!string.IsNullOrEmpty(match.Groups[4].Value)) ? int.Parse(match.Groups[4].Value) : 0);
			int x2 = ((!string.IsNullOrEmpty(match.Groups[5].Value)) ? int.Parse(match.Groups[5].Value) : 0);
			int y = ((!string.IsNullOrEmpty(match.Groups[6].Value)) ? int.Parse(match.Groups[6].Value) : 0);
			int y2 = ((!string.IsNullOrEmpty(match.Groups[7].Value)) ? int.Parse(match.Groups[7].Value) : 0);
			Add(new Subtitle(index, start, end, x, x2, y, y2, text2));
		}
	}

	private float ParseTime(string time)
	{
		string[] array = time.Split(new char[2] { ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
		int num = int.Parse(array[0]);
		int num2 = int.Parse(array[1]);
		int num3 = int.Parse(array[2]);
		int num4 = int.Parse(array[3]);
		return (float)(num * 3600 + num2 * 60 + num3) + (float)num4 / 1000f;
	}

	private string FixFormatting(string text)
	{
		text = Regex.Replace(text, "{(\\w)}", "<$1>");
		text = Regex.Replace(text, "{(/\\w)}", "<$1>");
		text = Regex.Replace(text, "<font color=\\x22(.*)\\x22>([\\S\\s]+)</font>", "<color=$1>$2</color>");
		return text;
	}

	public Subtitle GetSubtitleAt(float time)
	{
		return Find((Subtitle subtitle) => subtitle.Start <= time && subtitle.End >= time) ?? Subtitle.Empty;
	}
}
