using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public abstract class HeaderFooter : IHeaderFooter
{
	public class Field
	{
		public string sequence;

		[Obsolete("Use the generic list Fields.AllFields instead.")]
		public static ArrayList ALL_FIELDS => new ArrayList(Fields.AllFields);

		public Field(Fields fields, string sequence)
		{
			this.sequence = sequence;
			fields.Add(this);
		}
	}

	public class PairField : Field
	{
		public PairField(Fields fields, string sequence)
			: base(fields, sequence)
		{
		}
	}

	public class Fields
	{
		private List<Field> allFields = new List<Field>();

		private Field _sheetnamefield;

		private Field _filefield;

		private Field _fullfilefield;

		private Field _pagefield;

		private Field _datefield;

		private Field _timefield;

		private Field _numpagesfield;

		private Field _picturefield;

		private PairField _boldfield;

		private PairField _italicfield;

		private PairField _strikethroughfield;

		private PairField _subscriptfield;

		private PairField _superscriptfield;

		private PairField _underlinefield;

		private PairField _doubleunderlinefield;

		private static readonly Fields instance;

		public static ReadOnlyCollection<Field> AllFields => Instance.allFields.AsReadOnly();

		public Field SHEET_NAME_FIELD => _sheetnamefield;

		public Field DATE_FIELD => _datefield;

		public Field FILE_FIELD => _filefield;

		public Field FULL_FILE_FIELD => _fullfilefield;

		public Field PAGE_FIELD => _pagefield;

		public Field TIME_FIELD => _timefield;

		public Field NUM_PAGES_FIELD => _numpagesfield;

		public Field PICTURE_FIELD => _picturefield;

		public PairField BOLD_FIELD => _boldfield;

		public PairField ITALIC_FIELD => _italicfield;

		public PairField STRIKETHROUGH_FIELD => _strikethroughfield;

		public PairField SUBSCRIPT_FIELD => _subscriptfield;

		public PairField SUPERSCRIPT_FIELD => _superscriptfield;

		public PairField UNDERLINE_FIELD => _underlinefield;

		public PairField DOUBLE_UNDERLINE_FIELD => _doubleunderlinefield;

		public static Fields Instance => instance;

		static Fields()
		{
			instance = new Fields();
		}

		private Fields()
		{
			_sheetnamefield = new Field(this, "&A");
			_datefield = new Field(this, "&D");
			_filefield = new Field(this, "&F");
			_fullfilefield = new Field(this, "&Z");
			_pagefield = new Field(this, "&P");
			_timefield = new Field(this, "&T");
			_numpagesfield = new Field(this, "&N");
			_picturefield = new Field(this, "&G");
			_boldfield = new PairField(this, "&B");
			_italicfield = new PairField(this, "&I");
			_strikethroughfield = new PairField(this, "&S");
			_subscriptfield = new PairField(this, "&Y");
			_superscriptfield = new PairField(this, "&X");
			_underlinefield = new PairField(this, "&U");
			_doubleunderlinefield = new PairField(this, "&E");
		}

		internal void Add(Field field)
		{
			allFields.Add(field);
		}
	}

	protected bool stripFields;

	public abstract string RawText { get; }

	public string Left
	{
		get
		{
			return SplitParts()[0];
		}
		set
		{
			UpdatePart(0, value);
		}
	}

	public string Center
	{
		get
		{
			return SplitParts()[1];
		}
		set
		{
			UpdatePart(1, value);
		}
	}

	public string Right
	{
		get
		{
			return SplitParts()[2];
		}
		set
		{
			UpdatePart(2, value);
		}
	}

	public static string Page => PAGE_FIELD.sequence;

	public static string NumPages => NUM_PAGES_FIELD.sequence;

	public static string Date => DATE_FIELD.sequence;

	public static string Time => TIME_FIELD.sequence;

	public static string File => FILE_FIELD.sequence;

	public static string Tab => SHEET_NAME_FIELD.sequence;

	public static string StartBold => BOLD_FIELD.sequence;

	public static string EndBold => BOLD_FIELD.sequence;

	public static string StartUnderline => UNDERLINE_FIELD.sequence;

	public static string EndUnderline => UNDERLINE_FIELD.sequence;

	public static string StartDoubleUnderline => DOUBLE_UNDERLINE_FIELD.sequence;

	public static string EndDoubleUnderline => DOUBLE_UNDERLINE_FIELD.sequence;

	public bool AreFieldsStripped
	{
		get
		{
			return stripFields;
		}
		set
		{
			stripFields = value;
		}
	}

	internal static Field SHEET_NAME_FIELD => Fields.Instance.SHEET_NAME_FIELD;

	internal static Field DATE_FIELD => Fields.Instance.DATE_FIELD;

	internal static Field FILE_FIELD => Fields.Instance.FILE_FIELD;

	public static Field FULL_FILE_FIELD => Fields.Instance.FULL_FILE_FIELD;

	internal static Field PAGE_FIELD => Fields.Instance.PAGE_FIELD;

	internal static Field TIME_FIELD => Fields.Instance.TIME_FIELD;

	internal static Field NUM_PAGES_FIELD => Fields.Instance.NUM_PAGES_FIELD;

	public static Field PICTURE_FIELD => Fields.Instance.PICTURE_FIELD;

	internal static PairField BOLD_FIELD => Fields.Instance.BOLD_FIELD;

	public static PairField ITALIC_FIELD => Fields.Instance.ITALIC_FIELD;

	public static PairField STRIKETHROUGH_FIELD => Fields.Instance.STRIKETHROUGH_FIELD;

	public static PairField SUBSCRIPT_FIELD => Fields.Instance.SUBSCRIPT_FIELD;

	public static PairField SUPERSCRIPT_FIELD => Fields.Instance.SUPERSCRIPT_FIELD;

	internal static PairField UNDERLINE_FIELD => Fields.Instance.UNDERLINE_FIELD;

	internal static PairField DOUBLE_UNDERLINE_FIELD => Fields.Instance.DOUBLE_UNDERLINE_FIELD;

	private string[] SplitParts()
	{
		string text = RawText;
		string text2 = "";
		string text3 = "";
		string text4 = "";
		while (text.Length > 1)
		{
			if (text[0] != '&')
			{
				text3 = text;
				break;
			}
			int num = text.Length;
			switch (text[1])
			{
			case 'L':
				if (text.IndexOf("&C", StringComparison.Ordinal) >= 0)
				{
					num = Math.Min(num, text.IndexOf("&C", StringComparison.Ordinal));
				}
				if (text.IndexOf("&R", StringComparison.Ordinal) >= 0)
				{
					num = Math.Min(num, text.IndexOf("&R", StringComparison.Ordinal));
				}
				text2 = text.Substring(2, num - 2);
				text = text.Substring(num);
				break;
			case 'C':
				if (text.IndexOf("&L", StringComparison.Ordinal) >= 0)
				{
					num = Math.Min(num, text.IndexOf("&L", StringComparison.Ordinal));
				}
				if (text.IndexOf("&R", StringComparison.Ordinal) >= 0)
				{
					num = Math.Min(num, text.IndexOf("&R", StringComparison.Ordinal));
				}
				text3 = text.Substring(2, num - 2);
				text = text.Substring(num);
				break;
			case 'R':
				if (text.IndexOf("&C", StringComparison.Ordinal) >= 0)
				{
					num = Math.Min(num, text.IndexOf("&C", StringComparison.Ordinal));
				}
				if (text.IndexOf("&L", StringComparison.Ordinal) >= 0)
				{
					num = Math.Min(num, text.IndexOf("&L", StringComparison.Ordinal));
				}
				text4 = text.Substring(2, num - 2);
				text = text.Substring(num);
				break;
			default:
				text3 = text;
				break;
			}
		}
		return new string[3] { text2, text3, text4 };
	}

	private void UpdatePart(int partIndex, string newValue)
	{
		string[] array = SplitParts();
		array[partIndex] = ((newValue == null) ? "" : newValue);
		UpdateHeaderFooterText(array);
	}

	private void UpdateHeaderFooterText(string[] parts)
	{
		string text = parts[0];
		string text2 = parts[1];
		string text3 = parts[2];
		if (text2.Length < 1 && text.Length < 1 && text3.Length < 1)
		{
			SetHeaderFooterText(string.Empty);
			return;
		}
		StringBuilder stringBuilder = new StringBuilder(64);
		stringBuilder.Append("&C");
		stringBuilder.Append(text2);
		stringBuilder.Append("&L");
		stringBuilder.Append(text);
		stringBuilder.Append("&R");
		stringBuilder.Append(text3);
		string headerFooterText = stringBuilder.ToString();
		SetHeaderFooterText(headerFooterText);
	}

	protected abstract void SetHeaderFooterText(string text);

	public static string FontSize(short size)
	{
		return "&" + size;
	}

	public static string Font(string font, string style)
	{
		return "&\"" + font + "," + style + "\"";
	}

	public static string StripFields(string text)
	{
		if (text == null || text.Length == 0)
		{
			return text;
		}
		foreach (Field allField in Fields.AllFields)
		{
			string sequence = allField.sequence;
			int num;
			while ((num = text.IndexOf(sequence, StringComparison.CurrentCulture)) > -1)
			{
				text = text.Substring(0, num) + text.Substring(num + sequence.Length);
			}
		}
		text = Regex.Replace(text, "\\&\\d+", "");
		text = Regex.Replace(text, "\\&\".*?,.*?\"", "");
		return text;
	}
}
