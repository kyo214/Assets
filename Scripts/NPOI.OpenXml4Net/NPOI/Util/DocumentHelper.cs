using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace NPOI.Util;

public class DocumentHelper
{
	private DocumentHelper()
	{
	}

	public static XPathDocument ReadDocument(Stream stream)
	{
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		xmlReaderSettings.ValidationFlags = XmlSchemaValidationFlags.None;
		xmlReaderSettings.ValidationType = ValidationType.None;
		xmlReaderSettings.XmlResolver = null;
		xmlReaderSettings.DtdProcessing = DtdProcessing.Ignore;
		return new XPathDocument(XmlReader.Create(stream, xmlReaderSettings));
	}

	public static XmlDocument LoadDocument(Stream stream)
	{
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		xmlReaderSettings.ValidationFlags = XmlSchemaValidationFlags.None;
		xmlReaderSettings.ValidationType = ValidationType.Schema;
		xmlReaderSettings.XmlResolver = null;
		xmlReaderSettings.DtdProcessing = DtdProcessing.Ignore;
		xmlReaderSettings.ConformanceLevel = ConformanceLevel.Auto;
		xmlReaderSettings.IgnoreProcessingInstructions = true;
		try
		{
			XmlReader reader = XmlReader.Create(stream, xmlReaderSettings);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.XmlResolver = null;
			xmlDocument.PreserveWhitespace = true;
			xmlDocument.Load(reader);
			return xmlDocument;
		}
		catch (XmlException)
		{
			stream.Position = 0L;
			using StreamReader streamReader = new StreamReader(stream);
			XmlDocument xmlDocument2 = new XmlDocument();
			xmlDocument2.XmlResolver = null;
			xmlDocument2.PreserveWhitespace = true;
			xmlDocument2.LoadXml(streamReader.ReadToEnd());
			return xmlDocument2;
		}
	}

	public static XPathDocument CreateDocument()
	{
		throw new NotImplementedException();
	}
}
