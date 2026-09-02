using System;
using System.Collections;
using NPOI.HPSF.Wellknown;

namespace NPOI.HPSF;

[Serializable]
public class DocumentSummaryInformation : SpecialPropertySet
{
	public const string DEFAULT_STREAM_NAME = "\u0005DocumentSummaryInformation";

	public override PropertyIDMap PropertySetIDMap => PropertyIDMap.DocumentSummaryInformationProperties;

	public string Category
	{
		get
		{
			return GetPropertyStringValue(2);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(2, value);
		}
	}

	public string PresentationFormat
	{
		get
		{
			return GetPropertyStringValue(3);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(3, value);
		}
	}

	public int ByteCount
	{
		get
		{
			return GetPropertyIntValue(4);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(4, value);
		}
	}

	public int LineCount
	{
		get
		{
			return GetPropertyIntValue(5);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(5, value);
		}
	}

	public int ParCount
	{
		get
		{
			return GetPropertyIntValue(6);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(6, value);
		}
	}

	public int SlideCount
	{
		get
		{
			return GetPropertyIntValue(7);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(7, value);
		}
	}

	public int NoteCount
	{
		get
		{
			return GetPropertyIntValue(8);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(8, value);
		}
	}

	public int HiddenCount
	{
		get
		{
			return GetPropertyIntValue(9);
		}
		set
		{
			((MutableSection)Sections[0]).SetProperty(9, value);
		}
	}

	public int MMClipCount
	{
		get
		{
			return GetPropertyIntValue(10);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(10, value);
		}
	}

	public bool Scale
	{
		get
		{
			return GetPropertyBooleanValue(11);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(11, value);
		}
	}

	public byte[] HeadingPair
	{
		get
		{
			return (byte[])GetProperty(12);
		}
		set
		{
			throw new NotImplementedException("Writing byte arrays ");
		}
	}

	public byte[] Docparts
	{
		get
		{
			return (byte[])GetProperty(13);
		}
		set
		{
			throw new NotImplementedException("Writing byte arrays");
		}
	}

	public string Manager
	{
		get
		{
			return GetPropertyStringValue(14);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(14, value);
		}
	}

	public string Company
	{
		get
		{
			return GetPropertyStringValue(15);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(15, value);
		}
	}

	public bool LinksDirty
	{
		get
		{
			return GetPropertyBooleanValue(16);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(16, value);
		}
	}

	public int CharCountWithSpaces
	{
		get
		{
			return GetPropertyIntValue(PropertyIDMap.PID_CCHWITHSPACES);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(PropertyIDMap.PID_CCHWITHSPACES, value);
		}
	}

	public bool HyperlinksChanged
	{
		get
		{
			return GetPropertyBooleanValue(PropertyIDMap.PID_HYPERLINKSCHANGED);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(PropertyIDMap.PID_HYPERLINKSCHANGED, value);
		}
	}

	public int ApplicationVersion
	{
		get
		{
			return GetPropertyIntValue(PropertyIDMap.PID_VERSION);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(PropertyIDMap.PID_VERSION, value);
		}
	}

	public byte[] VBADigitalSignature
	{
		get
		{
			object property = GetProperty(PropertyIDMap.PID_DIGSIG);
			if (property != null && property is byte[])
			{
				return (byte[])property;
			}
			return null;
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(PropertyIDMap.PID_DIGSIG, value);
		}
	}

	public string ContentType
	{
		get
		{
			return GetPropertyStringValue(PropertyIDMap.PID_CONTENTTYPE);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(PropertyIDMap.PID_CONTENTTYPE, value);
		}
	}

	public string ContentStatus
	{
		get
		{
			return GetPropertyStringValue(PropertyIDMap.PID_CONTENTSTATUS);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(PropertyIDMap.PID_CONTENTSTATUS, value);
		}
	}

	public string Language
	{
		get
		{
			return GetPropertyStringValue(PropertyIDMap.PID_LANGUAGE);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(PropertyIDMap.PID_LANGUAGE, value);
		}
	}

	public string DocumentVersion
	{
		get
		{
			return GetPropertyStringValue(PropertyIDMap.PID_DOCVERSION);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(PropertyIDMap.PID_DOCVERSION, value);
		}
	}

	public CustomProperties CustomProperties
	{
		get
		{
			CustomProperties customProperties = null;
			if (SectionCount >= 2)
			{
				customProperties = new CustomProperties();
				Section section = Sections[1];
				IDictionary dictionary = section.Dictionary;
				Property[] properties = section.Properties;
				int num = 0;
				foreach (Property property in properties)
				{
					long iD = property.ID;
					if (iD != 0L && iD != 1)
					{
						num++;
						CustomProperty customProperty = new CustomProperty(property, (string)dictionary[iD]);
						customProperties.Put(customProperty.Name, customProperty);
					}
				}
				if (customProperties.Count != num)
				{
					customProperties.IsPure = false;
				}
			}
			return customProperties;
		}
		set
		{
			EnsureSection2();
			MutableSection mutableSection = (MutableSection)Sections[1];
			IDictionary dictionary = value.Dictionary;
			mutableSection.Clear();
			int num = value.Codepage;
			if (num < 0)
			{
				num = mutableSection.Codepage;
			}
			if (num < 0)
			{
				num = 1200;
			}
			value.Codepage = num;
			mutableSection.Codepage = num;
			mutableSection.Dictionary = dictionary;
			IEnumerator enumerator = value.Values.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Property property = (Property)enumerator.Current;
				mutableSection.SetProperty(property);
			}
		}
	}

	public DocumentSummaryInformation(PropertySet ps)
		: base(ps)
	{
		if (!IsDocumentSummaryInformation)
		{
			throw new UnexpectedPropertySetTypeException("Not a " + GetType().Name);
		}
	}

	public void RemoveCategory()
	{
		((MutableSection)FirstSection).RemoveProperty(2L);
	}

	public void RemovePresentationFormat()
	{
		((MutableSection)FirstSection).RemoveProperty(3L);
	}

	public void RemoveByteCount()
	{
		((MutableSection)FirstSection).RemoveProperty(4L);
	}

	public void RemoveLineCount()
	{
		((MutableSection)FirstSection).RemoveProperty(5L);
	}

	public void RemoveParCount()
	{
		((MutableSection)FirstSection).RemoveProperty(6L);
	}

	public void RemoveSlideCount()
	{
		((MutableSection)FirstSection).RemoveProperty(7L);
	}

	public void RemoveNoteCount()
	{
		((MutableSection)FirstSection).RemoveProperty(8L);
	}

	public void RemoveHiddenCount()
	{
		((MutableSection)FirstSection).RemoveProperty(9L);
	}

	public void RemoveMMClipCount()
	{
		((MutableSection)FirstSection).RemoveProperty(10L);
	}

	public void RemoveScale()
	{
		((MutableSection)FirstSection).RemoveProperty(11L);
	}

	public void RemoveHeadingPair()
	{
		((MutableSection)FirstSection).RemoveProperty(12L);
	}

	public void RemoveDocparts()
	{
		((MutableSection)FirstSection).RemoveProperty(13L);
	}

	public void RemoveManager()
	{
		((MutableSection)FirstSection).RemoveProperty(14L);
	}

	public void RemoveCompany()
	{
		((MutableSection)FirstSection).RemoveProperty(15L);
	}

	public void RemoveLinksDirty()
	{
		((MutableSection)FirstSection).RemoveProperty(16L);
	}

	public void RemoveCharCountWithSpaces()
	{
		((MutableSection)FirstSection).RemoveProperty(PropertyIDMap.PID_CCHWITHSPACES);
	}

	public void RemoveHyperlinksChanged()
	{
		((MutableSection)FirstSection).RemoveProperty(PropertyIDMap.PID_HYPERLINKSCHANGED);
	}

	public void RemoveApplicationVersion()
	{
		((MutableSection)FirstSection).RemoveProperty(PropertyIDMap.PID_VERSION);
	}

	public void RemoveVBADigitalSignature()
	{
		((MutableSection)FirstSection).RemoveProperty(PropertyIDMap.PID_DIGSIG);
	}

	public void RemoveContentType()
	{
		((MutableSection)FirstSection).RemoveProperty(PropertyIDMap.PID_CONTENTTYPE);
	}

	public void RemoveContentStatus()
	{
		((MutableSection)FirstSection).RemoveProperty(PropertyIDMap.PID_CONTENTSTATUS);
	}

	public void RemoveLanguage()
	{
		((MutableSection)FirstSection).RemoveProperty(PropertyIDMap.PID_LANGUAGE);
	}

	public void RemoveDocumentVersion()
	{
		((MutableSection)FirstSection).RemoveProperty(PropertyIDMap.PID_DOCVERSION);
	}

	private void EnsureSection2()
	{
		if (SectionCount < 2)
		{
			MutableSection mutableSection = new MutableSection();
			mutableSection.SetFormatID(SectionIDMap.DOCUMENT_SUMMARY_INFORMATION_ID2);
			AddSection(mutableSection);
		}
	}

	public void RemoveCustomProperties()
	{
		if (SectionCount >= 2)
		{
			Sections.RemoveAt(1);
			return;
		}
		throw new HPSFRuntimeException("Illegal internal format of Document SummaryInformation stream: second section is missing.");
	}
}
