using System;
using System.Globalization;
using TMPro;

namespace Doozy.Runtime.Common;

[Serializable]
public struct FormattedLabel(TMP_Text label = null, string format = "")
{
	public TMP_Text Label = label;

	public string Format = format;

	public void SetText(DateTime value)
	{
		if (!(Label == null))
		{
			Label.SetText(string.IsNullOrEmpty(Format) ? value.ToString(CultureInfo.InvariantCulture) : value.ToString(Format));
		}
	}

	public void SetText(TimeSpan value)
	{
		if (!(Label == null))
		{
			Label.SetText(string.IsNullOrEmpty(Format) ? value.ToString() : value.ToString(Format));
		}
	}

	public void SetText(string value)
	{
		if (!(Label == null))
		{
			Label.SetText(string.IsNullOrEmpty(Format) ? value : string.Format(Format, value));
		}
	}

	public void SetText(int value)
	{
		if (!(Label == null))
		{
			Label.SetText(string.IsNullOrEmpty(Format) ? value.ToString(CultureInfo.InvariantCulture) : value.ToString(Format));
		}
	}

	public void SetText(float value)
	{
		if (!(Label == null))
		{
			Label.SetText(string.IsNullOrEmpty(Format) ? value.ToString(CultureInfo.InvariantCulture) : value.ToString(Format));
		}
	}

	public void SetText(double value)
	{
		if (!(Label == null))
		{
			Label.SetText(string.IsNullOrEmpty(Format) ? value.ToString(CultureInfo.InvariantCulture) : value.ToString(Format));
		}
	}
}
