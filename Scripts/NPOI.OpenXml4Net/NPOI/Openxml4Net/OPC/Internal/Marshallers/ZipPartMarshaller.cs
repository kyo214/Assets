using System;
using System.IO;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using NPOI.Util;

namespace NPOI.OpenXml4Net.OPC.Internal.Marshallers;

public class ZipPartMarshaller : PartMarshaller
{
	private static POILogger logger = POILogFactory.GetLogger(typeof(ZipPartMarshaller));

	public bool Marshall(PackagePart part, Stream os)
	{
		if (!(os is ZipOutputStream))
		{
			logger.Log(7, "Unexpected class " + os.GetType().Name);
			throw new OpenXml4NetException("ZipOutputStream expected !");
		}
		if (part.Size == 0L && part.PartName.Name.Equals("/xl/sharedStrings.xml"))
		{
			return true;
		}
		ZipOutputStream zipOutputStream = (ZipOutputStream)os;
		ZipEntry entry = new ZipEntry(ZipHelper.GetZipItemNameFromOPCName(part.PartName.URI.OriginalString));
		try
		{
			zipOutputStream.PutNextEntry(entry);
			Stream inputStream = part.GetInputStream();
			byte[] array = new byte[ZipHelper.READ_WRITE_FILE_BUFFER_SIZE];
			int num = 0;
			while (true)
			{
				int num2 = inputStream.Read(array, 0, array.Length);
				if (num2 == 0)
				{
					break;
				}
				zipOutputStream.Write(array, 0, num2);
				num += num2;
			}
			zipOutputStream.CloseEntry();
		}
		catch (IOException exception)
		{
			logger.Log(7, "Cannot write: " + part.PartName?.ToString() + ": in ZIP", exception);
			return false;
		}
		if (part.HasRelationships)
		{
			PackagePartName relationshipPartName = PackagingUriHelper.GetRelationshipPartName(part.PartName);
			MarshallRelationshipPart(part.Relationships, relationshipPartName, zipOutputStream);
		}
		return true;
	}

	public static bool MarshallRelationshipPart(PackageRelationshipCollection rels, PackagePartName relPartName, ZipOutputStream zos)
	{
		XmlDocument xmlDocument = new XmlDocument();
		new XmlNamespaceManager(xmlDocument.NameTable).AddNamespace("x", "http://schemas.openxmlformats.org/package/2006/relationships");
		xmlDocument.AppendChild(xmlDocument.CreateElement(PackageRelationship.RELATIONSHIPS_TAG_NAME, "http://schemas.openxmlformats.org/package/2006/relationships"));
		Uri sourcePartUriFromRelationshipPartUri = PackagingUriHelper.GetSourcePartUriFromRelationshipPartUri(relPartName.URI);
		foreach (PackageRelationship rel in rels)
		{
			XmlElement xmlElement = xmlDocument.CreateElement(PackageRelationship.RELATIONSHIP_TAG_NAME, "http://schemas.openxmlformats.org/package/2006/relationships");
			xmlElement.SetAttribute(PackageRelationship.ID_ATTRIBUTE_NAME, rel.Id);
			xmlElement.SetAttribute(PackageRelationship.TYPE_ATTRIBUTE_NAME, rel.RelationshipType);
			Uri targetUri = rel.TargetUri;
			string value;
			if (rel.TargetMode == TargetMode.External)
			{
				value = targetUri.OriginalString;
				xmlElement.SetAttribute(PackageRelationship.TARGET_MODE_ATTRIBUTE_NAME, "External");
			}
			else
			{
				value = PackagingUriHelper.RelativizeUri(sourcePartUriFromRelationshipPartUri, rel.TargetUri, msCompatible: true).ToString();
			}
			xmlElement.SetAttribute(PackageRelationship.TARGET_ATTRIBUTE_NAME, value);
			xmlDocument.DocumentElement.AppendChild(xmlElement);
		}
		xmlDocument.Normalize();
		ZipEntry entry = new ZipEntry(ZipHelper.GetZipURIFromOPCName(relPartName.URI.ToString()).OriginalString);
		try
		{
			zos.PutNextEntry(entry);
			StreamHelper.SaveXmlInStream(xmlDocument, zos);
			zos.CloseEntry();
		}
		catch (IOException exception)
		{
			logger.Log(7, "Cannot create zip entry " + relPartName, exception);
			return false;
		}
		return true;
	}
}
