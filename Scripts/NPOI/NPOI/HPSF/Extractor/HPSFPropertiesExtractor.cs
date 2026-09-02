using System;
using System.Collections;
using System.Globalization;
using System.Text;
using NPOI.HPSF.Wellknown;
using NPOI.POIFS.FileSystem;

namespace NPOI.HPSF.Extractor;

public class HPSFPropertiesExtractor : POIOLE2TextExtractor
{
	public abstract class HelperPropertySet : SpecialPropertySet
	{
		public HelperPropertySet()
			: base(null)
		{
		}

		public static string GetPropertyValueText(object val)
		{
			if (val == null)
			{
				return "(not set)";
			}
			return SpecialPropertySet.GetPropertyStringValue(val);
		}
	}

	public string DocumentSummaryInformationText
	{
		get
		{
			if (document == null)
			{
				return "";
			}
			DocumentSummaryInformation documentSummaryInformation = document.DocumentSummaryInformation;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GetPropertiesText(documentSummaryInformation));
			CustomProperties customProperties = documentSummaryInformation?.CustomProperties;
			if (customProperties != null)
			{
				IEnumerator enumerator = customProperties.NameSet().GetEnumerator();
				while (enumerator.MoveNext())
				{
					string text = enumerator.Current.ToString();
					string propertyValueText = HelperPropertySet.GetPropertyValueText(customProperties[text]);
					stringBuilder.Append(text + " = " + propertyValueText + "\n");
				}
			}
			return stringBuilder.ToString();
		}
	}

	public string SummaryInformationText
	{
		get
		{
			if (document == null)
			{
				return "";
			}
			return GetPropertiesText(document.SummaryInformation);
		}
	}

	public override string Text => SummaryInformationText + DocumentSummaryInformationText;

	public override POITextExtractor MetadataTextExtractor
	{
		get
		{
			throw new InvalidOperationException("You already have the Metadata Text Extractor, not recursing!");
		}
	}

	public HPSFPropertiesExtractor(POIOLE2TextExtractor mainExtractor)
		: base(mainExtractor)
	{
	}

	public HPSFPropertiesExtractor(POIDocument doc)
		: base(doc)
	{
	}

	public HPSFPropertiesExtractor(POIFSFileSystem fs)
		: base(new HPSFPropertiesOnlyDocument(fs))
	{
	}

	public HPSFPropertiesExtractor(NPOIFSFileSystem fs)
		: base(new HPSFPropertiesOnlyDocument(fs))
	{
	}

	private static string GetPropertiesText(SpecialPropertySet ps)
	{
		if (ps == null)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		PropertyIDMap propertySetIDMap = ps.PropertySetIDMap;
		Property[] properties = ps.Properties;
		for (int i = 0; i < properties.Length; i++)
		{
			string text = properties[i].ID.ToString(CultureInfo.InvariantCulture);
			object obj = propertySetIDMap.Get(properties[i].ID);
			if (obj != null)
			{
				text = obj.ToString();
			}
			string propertyValueText = HelperPropertySet.GetPropertyValueText(properties[i].Value);
			stringBuilder.Append(text + " = " + propertyValueText + "\n");
		}
		return stringBuilder.ToString();
	}
}
