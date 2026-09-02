using System;
using System.IO;
using NPOI.HPSF.Wellknown;
using NPOI.POIFS.FileSystem;

namespace NPOI.HPSF;

public class PropertySetFactory
{
	public static PropertySet Create(DirectoryEntry dir, string name)
	{
		Stream stream = null;
		try
		{
			stream = new DocumentInputStream((DocumentEntry)dir.GetEntry(name));
			try
			{
				return Create(stream);
			}
			catch (MarkUnsupportedException)
			{
				return null;
			}
		}
		finally
		{
			stream?.Close();
		}
	}

	public static PropertySet Create(Stream stream)
	{
		PropertySet propertySet = new PropertySet(stream);
		try
		{
			if (propertySet.IsSummaryInformation)
			{
				return new SummaryInformation(propertySet);
			}
			if (propertySet.IsDocumentSummaryInformation)
			{
				return new DocumentSummaryInformation(propertySet);
			}
			return propertySet;
		}
		catch (UnexpectedPropertySetTypeException ex)
		{
			throw new InvalidOperationException(ex.Message, ex);
		}
	}

	public static SummaryInformation CreateSummaryInformation()
	{
		MutablePropertySet mutablePropertySet = new MutablePropertySet();
		((MutableSection)mutablePropertySet.FirstSection).SetFormatID(SectionIDMap.SUMMARY_INFORMATION_ID);
		try
		{
			return new SummaryInformation(mutablePropertySet);
		}
		catch (UnexpectedPropertySetTypeException reason)
		{
			throw new HPSFRuntimeException(reason);
		}
	}

	public static DocumentSummaryInformation CreateDocumentSummaryInformation()
	{
		MutablePropertySet mutablePropertySet = new MutablePropertySet();
		((MutableSection)mutablePropertySet.FirstSection).SetFormatID(SectionIDMap.DOCUMENT_SUMMARY_INFORMATION_ID1);
		try
		{
			return new DocumentSummaryInformation(mutablePropertySet);
		}
		catch (UnexpectedPropertySetTypeException reason)
		{
			throw new HPSFRuntimeException(reason);
		}
	}

	internal static DocumentSummaryInformation NewDocumentSummaryInformation()
	{
		MutablePropertySet mutablePropertySet = new MutablePropertySet();
		((MutableSection)mutablePropertySet.FirstSection).SetFormatID(SectionIDMap.DOCUMENT_SUMMARY_INFORMATION_ID1);
		try
		{
			return new DocumentSummaryInformation(mutablePropertySet);
		}
		catch (UnexpectedPropertySetTypeException reason)
		{
			throw new HPSFRuntimeException(reason);
		}
	}
}
