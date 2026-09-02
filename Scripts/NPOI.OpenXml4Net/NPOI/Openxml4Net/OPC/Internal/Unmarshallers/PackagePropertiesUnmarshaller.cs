using System.Collections;
using System.IO;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using NPOI.OpenXml4Net.Exceptions;
using NPOI.Util;

namespace NPOI.OpenXml4Net.OPC.Internal.Unmarshallers;

public class PackagePropertiesUnmarshaller : PartUnmarshaller
{
	private static string namespaceDC = "http://purl.org/dc/elements/1.1/";

	private static string namespaceCP = "http://schemas.openxmlformats.org/package/2006/metadata/core-properties";

	private static string namespaceDcTerms = "http://purl.org/dc/terms/";

	private static string namespaceXML = "http://www.w3.org/XML/1998/namespace";

	private static string namespaceXSI = "http://www.w3.org/2001/XMLSchema-instance";

	protected static string KEYWORD_CATEGORY = "category";

	protected static string KEYWORD_CONTENT_STATUS = "contentStatus";

	protected static string KEYWORD_CONTENT_TYPE = "contentType";

	protected static string KEYWORD_CREATED = "created";

	protected static string KEYWORD_CREATOR = "creator";

	protected static string KEYWORD_DESCRIPTION = "description";

	protected static string KEYWORD_IDENTIFIER = "identifier";

	protected static string KEYWORD_KEYWORDS = "keywords";

	protected static string KEYWORD_LANGUAGE = "language";

	protected static string KEYWORD_LAST_MODIFIED_BY = "lastModifiedBy";

	protected static string KEYWORD_LAST_PRINTED = "lastPrinted";

	protected static string KEYWORD_MODIFIED = "modified";

	protected static string KEYWORD_REVISION = "revision";

	protected static string KEYWORD_SUBJECT = "subject";

	protected static string KEYWORD_TITLE = "title";

	protected static string KEYWORD_VERSION = "version";

	protected XmlNamespaceManager nsmgr;

	public PackagePart Unmarshall(UnmarshallContext context, Stream in1)
	{
		PackagePropertiesPart packagePropertiesPart = new PackagePropertiesPart(context.Package, context.PartName);
		if (in1 == null)
		{
			if (context.ZipEntry != null)
			{
				in1 = ((ZipPackage)context.Package).ZipArchive.GetInputStream(context.ZipEntry);
			}
			else
			{
				if (context.Package == null)
				{
					throw new IOException("Error while trying to get the part input stream.");
				}
				ZipEntry corePropertiesZipEntry;
				try
				{
					corePropertiesZipEntry = ZipHelper.GetCorePropertiesZipEntry((ZipPackage)context.Package);
				}
				catch (OpenXml4NetException)
				{
					throw new IOException("Error while trying to get the part input stream.");
				}
				in1 = ((ZipPackage)context.Package).ZipArchive.GetInputStream(corePropertiesZipEntry);
			}
		}
		XmlDocument xmlDocument = null;
		try
		{
			xmlDocument = DocumentHelper.LoadDocument(in1);
			nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
			nsmgr.AddNamespace("cp", namespaceCP);
			nsmgr.AddNamespace("dc", namespaceDC);
			nsmgr.AddNamespace("dcterms", namespaceDcTerms);
			nsmgr.AddNamespace("xsi", namespaceXSI);
			nsmgr.AddNamespace("cml", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			nsmgr.AddNamespace("dcmitype", "http://purl.org/dc/dcmitype/");
			CheckElementForOPCCompliance(xmlDocument.DocumentElement);
		}
		catch (XmlException ex2)
		{
			throw new IOException(ex2.Message, ex2);
		}
		if (xmlDocument != null && xmlDocument.DocumentElement != null)
		{
			packagePropertiesPart.SetCategoryProperty(LoadCategory(xmlDocument));
			packagePropertiesPart.SetContentStatusProperty(LoadContentStatus(xmlDocument));
			packagePropertiesPart.SetContentTypeProperty(LoadContentType(xmlDocument));
			packagePropertiesPart.SetCreatedProperty(LoadCreated(xmlDocument));
			packagePropertiesPart.SetCreatorProperty(LoadCreator(xmlDocument));
			packagePropertiesPart.SetDescriptionProperty(LoadDescription(xmlDocument));
			packagePropertiesPart.SetIdentifierProperty(LoadIdentifier(xmlDocument));
			packagePropertiesPart.SetKeywordsProperty(LoadKeywords(xmlDocument));
			packagePropertiesPart.SetLanguageProperty(LoadLanguage(xmlDocument));
			packagePropertiesPart.SetLastModifiedByProperty(LoadLastModifiedBy(xmlDocument));
			packagePropertiesPart.SetLastPrintedProperty(LoadLastPrinted(xmlDocument));
			packagePropertiesPart.SetModifiedProperty(LoadModified(xmlDocument));
			packagePropertiesPart.SetRevisionProperty(LoadRevision(xmlDocument));
			packagePropertiesPart.SetSubjectProperty(LoadSubject(xmlDocument));
			packagePropertiesPart.SetTitleProperty(LoadTitle(xmlDocument));
			packagePropertiesPart.SetVersionProperty(LoadVersion(xmlDocument));
		}
		return packagePropertiesPart;
	}

	private string LoadCategory(XmlDocument xmlDoc)
	{
		return xmlDoc.DocumentElement.SelectNodes("cp:" + KEYWORD_CATEGORY, nsmgr)[0]?.InnerText;
	}

	private string LoadContentStatus(XmlDocument xmlDoc)
	{
		return xmlDoc.DocumentElement.SelectNodes("cp:" + KEYWORD_CONTENT_STATUS, nsmgr)[0]?.InnerText;
	}

	private string LoadContentType(XmlDocument xmlDoc)
	{
		return xmlDoc.DocumentElement.SelectNodes("cp:" + KEYWORD_CONTENT_TYPE, nsmgr)[0]?.InnerText;
	}

	private string LoadCreated(XmlDocument xmlDoc)
	{
		return xmlDoc.DocumentElement.SelectNodes("dcterms:" + KEYWORD_CREATED, nsmgr)[0]?.InnerText;
	}

	private string LoadCreator(XmlDocument xmlDoc)
	{
		return xmlDoc.DocumentElement.SelectNodes("dc:" + KEYWORD_CREATOR, nsmgr)[0]?.InnerText;
	}

	private string LoadDescription(XmlDocument xmlDoc)
	{
		return xmlDoc.DocumentElement.SelectNodes("dc:" + KEYWORD_DESCRIPTION, nsmgr)[0]?.InnerText;
	}

	private string LoadIdentifier(XmlDocument xmlDoc)
	{
		return xmlDoc.DocumentElement.SelectNodes("dc:" + KEYWORD_IDENTIFIER, nsmgr)[0]?.InnerText;
	}

	private string LoadKeywords(XmlDocument xmlDoc)
	{
		return xmlDoc.DocumentElement.SelectNodes("cp:" + KEYWORD_KEYWORDS, nsmgr)[0]?.InnerText;
	}

	private string LoadLanguage(XmlDocument xmlDoc)
	{
		return xmlDoc.DocumentElement.SelectNodes("dc:" + KEYWORD_LANGUAGE, nsmgr)[0]?.InnerText;
	}

	private string LoadLastModifiedBy(XmlDocument xmlDoc)
	{
		return xmlDoc.DocumentElement.SelectNodes("cp:" + KEYWORD_LAST_MODIFIED_BY, nsmgr)[0]?.InnerText;
	}

	private string LoadLastPrinted(XmlDocument xmlDoc)
	{
		return xmlDoc.DocumentElement.SelectNodes("cp:" + KEYWORD_LAST_PRINTED, nsmgr)[0]?.InnerText;
	}

	private string LoadModified(XmlDocument xmlDoc)
	{
		return xmlDoc.DocumentElement.SelectNodes("dcterms:" + KEYWORD_MODIFIED, nsmgr)[0]?.InnerText;
	}

	private string LoadRevision(XmlDocument xmlDoc)
	{
		return xmlDoc.DocumentElement.SelectNodes("cp:" + KEYWORD_REVISION, nsmgr)[0]?.InnerText;
	}

	private string LoadSubject(XmlDocument xmlDoc)
	{
		return xmlDoc.DocumentElement.SelectNodes("dc:" + KEYWORD_SUBJECT, nsmgr)[0]?.InnerText;
	}

	private string LoadTitle(XmlDocument xmlDoc)
	{
		return xmlDoc.DocumentElement.SelectNodes("dc:" + KEYWORD_TITLE, nsmgr)[0]?.InnerText;
	}

	private string LoadVersion(XmlDocument xmlDoc)
	{
		return xmlDoc.DocumentElement.SelectNodes("cp:" + KEYWORD_VERSION, nsmgr)[0]?.InnerText;
	}

	public void CheckElementForOPCCompliance(XmlElement el)
	{
		XmlAttributeCollection attributes = el.Attributes;
		int count = attributes.Count;
		for (int i = 0; i < count; i++)
		{
			XmlAttribute xmlAttribute = attributes[i];
			if (xmlAttribute.NamespaceURI.Equals(XMLConstants.XMLNS_ATTRIBUTE_NS_URI) && xmlAttribute.Value.Equals("http://schemas.openxmlformats.org/markup-compatibility/2006"))
			{
				throw new InvalidFormatException("OPC Compliance error [M4.2]: A format consumer shall consider the use of the Markup Compatibility namespace to be an error.");
			}
		}
		string localName = el.LocalName;
		if (el.NamespaceURI.Equals("http://purl.org/dc/terms/") && !localName.Equals(KEYWORD_CREATED) && !localName.Equals(KEYWORD_MODIFIED))
		{
			throw new InvalidFormatException("OPC Compliance error [M4.3]: Producers shall not create a document element that contains refinements to the Dublin Core elements, except for the two specified in the schema: <dcterms:created> and <dcterms:modified> Consumers shall consider a document element that violates this constraint to be an error.");
		}
		if (el.Attributes["lang", namespaceXML] != null)
		{
			throw new InvalidFormatException("OPC Compliance error [M4.4]: Producers shall not create a document element that contains the xml:lang attribute. Consumers shall consider a document element that violates this constraint to be an error.");
		}
		if (el.NamespaceURI.Equals("http://purl.org/dc/terms/"))
		{
			if (!localName.Equals(KEYWORD_CREATED) && !localName.Equals(KEYWORD_MODIFIED))
			{
				throw new InvalidFormatException("Namespace error : " + localName + " shouldn't have the following naemspace -> http://purl.org/dc/terms/");
			}
			XmlNode xmlNode = el.Attributes["type", XMLConstants.W3C_XML_SCHEMA_INSTANCE_NS_URI];
			if (xmlNode == null)
			{
				throw new InvalidFormatException("The element '" + localName + "' must have the '" + nsmgr.LookupPrefix(namespaceXSI) + ":type' attribute present !");
			}
			if (!xmlNode.Value.Equals(el.Prefix + ":W3CDTF"))
			{
				throw new InvalidFormatException("The element '" + localName + "' must have the '" + nsmgr.LookupPrefix(namespaceXSI) + ":type' attribute with the value '" + el.Prefix + ":W3CDTF', but had '" + xmlNode.Value + "' !");
			}
		}
		IEnumerator enumerator = el.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Current is XmlElement)
			{
				CheckElementForOPCCompliance((XmlElement)enumerator.Current);
			}
		}
	}
}
