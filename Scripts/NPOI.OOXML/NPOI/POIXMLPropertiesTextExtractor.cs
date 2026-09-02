using System;
using System.Text;
using NPOI.OpenXml4Net.OPC.Internal;
using NPOI.OpenXmlFormats;

namespace NPOI;

public class POIXMLPropertiesTextExtractor : POIXMLTextExtractor
{
	public override string Text => GetCorePropertiesText() + GetExtendedPropertiesText() + GetCustomPropertiesText();

	public override POITextExtractor MetadataTextExtractor
	{
		get
		{
			throw new InvalidOperationException("You already have the Metadata Text Extractor, not recursing!");
		}
	}

	public POIXMLPropertiesTextExtractor(POIXMLDocument doc)
		: base(doc)
	{
	}

	public POIXMLPropertiesTextExtractor(POIXMLTextExtractor otherExtractor)
		: base(otherExtractor.Document)
	{
	}

	private void AppendIfPresent(StringBuilder text, string thing, bool value)
	{
		AppendIfPresent(text, thing, value.ToString());
	}

	private void AppendIfPresent(StringBuilder text, string thing, int value)
	{
		AppendIfPresent(text, thing, value.ToString());
	}

	private void AppendIfPresent(StringBuilder text, string thing, DateTime? value)
	{
		if (value.HasValue)
		{
			AppendIfPresent(text, thing, value.ToString());
		}
	}

	private void AppendIfPresent(StringBuilder text, string thing, string value)
	{
		if (value != null)
		{
			text.Append(thing);
			text.Append(" = ");
			text.Append(value);
			text.Append("\n");
		}
	}

	public string GetCorePropertiesText()
	{
		if (base.Document == null)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		PackagePropertiesPart underlyingProperties = base.Document.GetProperties().CoreProperties.GetUnderlyingProperties();
		AppendIfPresent(stringBuilder, "Category", underlyingProperties.GetCategoryProperty());
		AppendIfPresent(stringBuilder, "Category", underlyingProperties.GetCategoryProperty());
		AppendIfPresent(stringBuilder, "ContentStatus", underlyingProperties.GetContentStatusProperty());
		AppendIfPresent(stringBuilder, "ContentType", underlyingProperties.GetContentTypeProperty());
		AppendIfPresent(stringBuilder, "Created", underlyingProperties.GetCreatedProperty().Value);
		AppendIfPresent(stringBuilder, "CreatedString", underlyingProperties.GetCreatedPropertyString());
		AppendIfPresent(stringBuilder, "Creator", underlyingProperties.GetCreatorProperty());
		AppendIfPresent(stringBuilder, "Description", underlyingProperties.GetDescriptionProperty());
		AppendIfPresent(stringBuilder, "Identifier", underlyingProperties.GetIdentifierProperty());
		AppendIfPresent(stringBuilder, "Keywords", underlyingProperties.GetKeywordsProperty());
		AppendIfPresent(stringBuilder, "Language", underlyingProperties.GetLanguageProperty());
		AppendIfPresent(stringBuilder, "LastModifiedBy", underlyingProperties.GetLastModifiedByProperty());
		AppendIfPresent(stringBuilder, "LastPrinted", underlyingProperties.GetLastPrintedProperty());
		AppendIfPresent(stringBuilder, "LastPrintedString", underlyingProperties.GetLastPrintedPropertyString());
		AppendIfPresent(stringBuilder, "Modified", underlyingProperties.GetModifiedProperty());
		AppendIfPresent(stringBuilder, "ModifiedString", underlyingProperties.GetModifiedPropertyString());
		AppendIfPresent(stringBuilder, "Revision", underlyingProperties.GetRevisionProperty());
		AppendIfPresent(stringBuilder, "Subject", underlyingProperties.GetSubjectProperty());
		AppendIfPresent(stringBuilder, "Title", underlyingProperties.GetTitleProperty());
		AppendIfPresent(stringBuilder, "Version", underlyingProperties.GetVersionProperty());
		return stringBuilder.ToString();
	}

	public string GetExtendedPropertiesText()
	{
		if (base.Document == null)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		CT_ExtendedProperties underlyingProperties = base.Document.GetProperties().ExtendedProperties.GetUnderlyingProperties();
		AppendIfPresent(stringBuilder, "Application", underlyingProperties.Application);
		AppendIfPresent(stringBuilder, "AppVersion", underlyingProperties.AppVersion);
		AppendIfPresent(stringBuilder, "Characters", underlyingProperties.Characters);
		AppendIfPresent(stringBuilder, "CharactersWithSpaces", underlyingProperties.CharactersWithSpaces);
		AppendIfPresent(stringBuilder, "Company", underlyingProperties.Company);
		AppendIfPresent(stringBuilder, "HyperlinkBase", underlyingProperties.HyperlinkBase);
		AppendIfPresent(stringBuilder, "HyperlinksChanged", underlyingProperties.HyperlinksChanged);
		AppendIfPresent(stringBuilder, "Lines", underlyingProperties.Lines);
		AppendIfPresent(stringBuilder, "LinksUpToDate", underlyingProperties.LinksUpToDate);
		AppendIfPresent(stringBuilder, "Manager", underlyingProperties.Manager);
		AppendIfPresent(stringBuilder, "Pages", underlyingProperties.Pages);
		AppendIfPresent(stringBuilder, "Paragraphs", underlyingProperties.Paragraphs);
		AppendIfPresent(stringBuilder, "PresentationFormat", underlyingProperties.PresentationFormat);
		AppendIfPresent(stringBuilder, "Template", underlyingProperties.Template);
		AppendIfPresent(stringBuilder, "TotalTime", underlyingProperties.TotalTime);
		return stringBuilder.ToString();
	}

	public string GetCustomPropertiesText()
	{
		if (base.Document == null)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (CT_Property property in base.Document.GetProperties().CustomProperties.GetUnderlyingProperties().GetPropertyList())
		{
			string text = "(not implemented!)";
			if (property.IsSetLpwstr())
			{
				text = property.GetLpwstr();
			}
			else if (property.IsSetLpstr())
			{
				text = property.GetLpstr();
			}
			else if (property.IsSetDate())
			{
				text = property.GetDate().ToString();
			}
			else if (property.IsSetFiletime())
			{
				text = property.GetFiletime().ToString();
			}
			else if (property.IsSetBool())
			{
				text = property.GetBool().ToString();
			}
			else if (property.IsSetI1())
			{
				text = property.GetI1().ToString();
			}
			else if (property.IsSetI2())
			{
				text = property.GetI2().ToString();
			}
			else if (property.IsSetI4())
			{
				text = property.GetI4().ToString();
			}
			else if (property.IsSetI8())
			{
				text = property.GetI8().ToString();
			}
			else if (property.IsSetInt())
			{
				text = property.GetInt().ToString();
			}
			else if (property.IsSetUi1())
			{
				text = property.GetUi1().ToString();
			}
			else if (property.IsSetUi2())
			{
				text = property.GetUi2().ToString();
			}
			else if (property.IsSetUi4())
			{
				text = property.GetUi4().ToString();
			}
			else if (property.IsSetUi8())
			{
				text = property.GetUi8().ToString();
			}
			else if (property.IsSetUint())
			{
				text = property.GetUint().ToString();
			}
			else if (property.IsSetR4())
			{
				text = property.GetR4().ToString();
			}
			else if (property.IsSetR8())
			{
				text = property.GetR8().ToString();
			}
			else if (property.IsSetDecimal())
			{
				decimal? num = property.GetDecimal();
				text = (num.HasValue ? num.ToString() : null);
			}
			stringBuilder.Append(property.name + " = " + text + "\n");
		}
		return stringBuilder.ToString();
	}
}
