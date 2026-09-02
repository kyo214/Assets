using System.Collections;
using System.Text;

namespace NPOI.HPSF.Wellknown;

public class SectionIDMap : Hashtable
{
	public static readonly byte[] SUMMARY_INFORMATION_ID = new byte[16]
	{
		242, 159, 133, 224, 79, 249, 16, 104, 171, 145,
		8, 0, 43, 39, 179, 217
	};

	public static readonly byte[] DOCUMENT_SUMMARY_INFORMATION_ID1 = new byte[16]
	{
		213, 205, 213, 2, 46, 156, 16, 27, 147, 151,
		8, 0, 43, 44, 249, 174
	};

	public static readonly byte[] DOCUMENT_SUMMARY_INFORMATION_ID2 = new byte[16]
	{
		213, 205, 213, 5, 46, 156, 16, 27, 147, 151,
		8, 0, 43, 44, 249, 174
	};

	public const string UNDEFINED = "[undefined]";

	private static SectionIDMap defaultMap;

	public static SectionIDMap GetInstance()
	{
		if (defaultMap == null)
		{
			SectionIDMap sectionIDMap = new SectionIDMap();
			sectionIDMap.Put(SUMMARY_INFORMATION_ID, PropertyIDMap.SummaryInformationProperties);
			sectionIDMap.Put(DOCUMENT_SUMMARY_INFORMATION_ID1, PropertyIDMap.DocumentSummaryInformationProperties);
			defaultMap = sectionIDMap;
		}
		return defaultMap;
	}

	public static string GetPIDString(byte[] sectionFormatID, long pid)
	{
		PropertyIDMap propertyIDMap = GetInstance().Get(sectionFormatID);
		if (propertyIDMap == null)
		{
			return "[undefined]";
		}
		string text = (string)propertyIDMap.Get(pid);
		if (text == null)
		{
			return "[undefined]";
		}
		return text;
	}

	public PropertyIDMap Get(byte[] sectionFormatID)
	{
		return (PropertyIDMap)this[Encoding.UTF8.GetString(sectionFormatID)];
	}

	public object Put(byte[] sectionFormatID, PropertyIDMap propertyIDMap)
	{
		return this[sectionFormatID] = propertyIDMap;
	}
}
