using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_IgnoredError
{
	private List<string> sqrefField;

	private bool evalErrorField;

	private bool twoDigitTextYearField;

	private bool numberStoredAsTextField;

	private bool formulaField;

	private bool formulaRangeField;

	private bool unlockedFormulaField;

	private bool emptyCellReferenceField;

	private bool listDataValidationField;

	private bool calculatedColumnField;

	[XmlAttribute]
	public List<string> sqref
	{
		get
		{
			return sqrefField;
		}
		set
		{
			sqrefField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool evalError
	{
		get
		{
			return evalErrorField;
		}
		set
		{
			evalErrorField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool twoDigitTextYear
	{
		get
		{
			return twoDigitTextYearField;
		}
		set
		{
			twoDigitTextYearField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool numberStoredAsText
	{
		get
		{
			return numberStoredAsTextField;
		}
		set
		{
			numberStoredAsTextField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool formula
	{
		get
		{
			return formulaField;
		}
		set
		{
			formulaField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool formulaRange
	{
		get
		{
			return formulaRangeField;
		}
		set
		{
			formulaRangeField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool unlockedFormula
	{
		get
		{
			return unlockedFormulaField;
		}
		set
		{
			unlockedFormulaField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool emptyCellReference
	{
		get
		{
			return emptyCellReferenceField;
		}
		set
		{
			emptyCellReferenceField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool listDataValidation
	{
		get
		{
			return listDataValidationField;
		}
		set
		{
			listDataValidationField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool calculatedColumn
	{
		get
		{
			return calculatedColumnField;
		}
		set
		{
			calculatedColumnField = value;
		}
	}

	public CT_IgnoredError()
	{
		sqrefField = new List<string>();
		evalErrorField = false;
		twoDigitTextYearField = false;
		numberStoredAsTextField = false;
		formulaField = false;
		formulaRangeField = false;
		unlockedFormulaField = false;
		emptyCellReferenceField = false;
		listDataValidationField = false;
		calculatedColumnField = false;
	}

	public static CT_IgnoredError Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_IgnoredError cT_IgnoredError = new CT_IgnoredError();
		cT_IgnoredError.evalError = XmlHelper.ReadBool(node.Attributes["evalError"]);
		cT_IgnoredError.twoDigitTextYear = XmlHelper.ReadBool(node.Attributes["twoDigitTextYear"]);
		cT_IgnoredError.numberStoredAsText = XmlHelper.ReadBool(node.Attributes["numberStoredAsText"]);
		cT_IgnoredError.formula = XmlHelper.ReadBool(node.Attributes["formula"]);
		cT_IgnoredError.formulaRange = XmlHelper.ReadBool(node.Attributes["formulaRange"]);
		cT_IgnoredError.unlockedFormula = XmlHelper.ReadBool(node.Attributes["unlockedFormula"]);
		cT_IgnoredError.emptyCellReference = XmlHelper.ReadBool(node.Attributes["emptyCellReference"]);
		cT_IgnoredError.listDataValidation = XmlHelper.ReadBool(node.Attributes["listDataValidation"]);
		cT_IgnoredError.calculatedColumn = XmlHelper.ReadBool(node.Attributes["calculatedColumn"]);
		string text = XmlHelper.ReadString(node.Attributes["sqref"]);
		if (text != null)
		{
			cT_IgnoredError.sqref.AddRange(text.Split(new char[1] { ' ' }));
		}
		return cT_IgnoredError;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "sqref", string.Join(" ", sqref));
		XmlHelper.WriteAttribute(sw, "evalError", evalError, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "twoDigitTextYear", twoDigitTextYear, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "numberStoredAsText", numberStoredAsText, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "formula", formula, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "formulaRange", formulaRange, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "unlockedFormula", unlockedFormula, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "emptyCellReference", emptyCellReference, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "listDataValidation", listDataValidation, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "calculatedColumn", calculatedColumn, writeIfBlank: false);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}
}
