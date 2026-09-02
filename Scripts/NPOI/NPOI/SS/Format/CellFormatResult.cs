using System;
using System.Drawing;

namespace NPOI.SS.Format;

public class CellFormatResult
{
	private bool _applies;

	private string _text;

	private Color _textcolor;

	public bool Applies
	{
		get
		{
			return _applies;
		}
		set
		{
			_applies = value;
		}
	}

	public string Text
	{
		get
		{
			return _text;
		}
		set
		{
			_text = value;
		}
	}

	public Color TextColor
	{
		get
		{
			return _textcolor;
		}
		set
		{
			_textcolor = value;
		}
	}

	public CellFormatResult(bool applies, string text, Color textColor)
	{
		if (text == null)
		{
			throw new ArgumentException("CellFormatResult text may not be null");
		}
		Applies = applies;
		Text = text;
		TextColor = (applies ? textColor : Color.Empty);
	}
}
