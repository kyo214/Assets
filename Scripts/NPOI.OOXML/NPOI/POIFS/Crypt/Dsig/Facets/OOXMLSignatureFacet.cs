using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Xml;
using NPOI.OpenXml4Net.OPC;

namespace NPOI.POIFS.Crypt.Dsig.Facets;

public class OOXMLSignatureFacet : SignatureFacet
{
	public static string[] contentTypes = new string[19]
	{
		"application/vnd.Openxmlformats-officedocument.wordProcessingml.document.main+xml", "application/vnd.Openxmlformats-officedocument.wordProcessingml.fontTable+xml", "application/vnd.Openxmlformats-officedocument.wordProcessingml.Settings+xml", "application/vnd.Openxmlformats-officedocument.wordProcessingml.styles+xml", "application/vnd.Openxmlformats-officedocument.theme+xml", "application/vnd.Openxmlformats-officedocument.wordProcessingml.webSettings+xml", "application/vnd.Openxmlformats-officedocument.wordProcessingml.numbering+xml", "application/vnd.ms-word.stylesWithEffects+xml", "application/vnd.Openxmlformats-officedocument.spreadsheetml.sharedStrings+xml", "application/vnd.Openxmlformats-officedocument.spreadsheetml.worksheet+xml",
		"application/vnd.Openxmlformats-officedocument.spreadsheetml.styles+xml", "application/vnd.Openxmlformats-officedocument.spreadsheetml.sheet.main+xml", "application/vnd.Openxmlformats-officedocument.presentationml.presentation.main+xml", "application/vnd.Openxmlformats-officedocument.presentationml.slideLayout+xml", "application/vnd.Openxmlformats-officedocument.presentationml.slideMaster+xml", "application/vnd.Openxmlformats-officedocument.presentationml.slide+xml", "application/vnd.Openxmlformats-officedocument.presentationml.tableStyles+xml", "application/vnd.Openxmlformats-officedocument.presentationml.viewProps+xml", "application/vnd.Openxmlformats-officedocument.presentationml.presProps+xml"
	};

	public static string[] signed = new string[146]
	{
		"powerPivotData", "activeXControlBinary", "attachedToolbars", "connectorXml", "downRev", "functionPrototypes", "graphicFrameDoc", "groupShapeXml", "ink", "keyMapCustomizations",
		"legacyDiagramText", "legacyDocTextInfo", "officeDocument", "pictureXml", "shapeXml", "smartTags", "ui/altText", "ui/buttonSize", "ui/controlID", "ui/description",
		"ui/enabled", "ui/extensibility", "ui/helperText", "ui/imageID", "ui/imageMso", "ui/keyTip", "ui/label", "ui/lcid", "ui/loud", "ui/pressed",
		"ui/progID", "ui/ribbonID", "ui/ShowImage", "ui/ShowLabel", "ui/supertip", "ui/target", "ui/text", "ui/title", "ui/tooltip", "ui/userCustomization",
		"ui/visible", "userXmlData", "vbaProject", "wordVbaData", "wsSortMap", "xlBinaryIndex", "xlExternalLinkPath/xlAlternateStartup", "xlExternalLinkPath/xlLibrary", "xlExternalLinkPath/xlPathMissing", "xlExternalLinkPath/xlStartup",
		"xlIntlMacrosheet", "xlMacrosheet", "customData", "diagramDrawing", "hdphoto", "inkXml", "media", "slicer", "slicerCache", "stylesWithEffects",
		"ui/extensibility", "chartColorStyle", "chartLayout", "chartStyle", "dictionary", "timeline", "timelineCache", "aFChunk", "attachedTemplate", "audio",
		"calcChain", "chart", "chartsheet", "chartUserShapes", "commentAuthors", "comments", "connections", "control", "customProperty", "customXml",
		"diagramColors", "diagramData", "diagramLayout", "diagramQuickStyle", "dialogsheet", "drawing", "endnotes", "externalLink", "externalLinkPath", "font",
		"fontTable", "footer", "footnotes", "glossaryDocument", "handoutMaster", "header", "hyperlink", "image", "mailMergeHeaderSource", "mailMergeRecipientData",
		"mailMergeSource", "notesMaster", "notesSlide", "numbering", "officeDocument", "oleObject", "package", "pivotCacheDefInition", "pivotCacheRecords", "pivotTable",
		"presProps", "printerSettings", "queryTable", "recipientData", "settings", "sharedStrings", "sheetMetadata", "slide", "slideLayout", "slideMaster",
		"slideUpdateInfo", "slideUpdateUrl", "styles", "table", "tableSingleCells", "tableStyles", "tags", "theme", "themeOverride", "transform",
		"video", "viewProps", "volatileDependencies", "webSettings", "worksheet", "xmlMaps", "ctrlProp", "customData", "diagram", "diagramColorsHeader",
		"diagramLayoutHeader", "diagramQuickStyleHeader", "documentParts", "slicer", "slicerCache", "vmlDrawing"
	};

	public override void preSign(XmlDocument document, List<Reference> references, List<XmlNode> objects)
	{
		AddManifestObject(document, references, objects);
		AddSignatureInfo(document, references, objects);
	}

	protected void AddManifestObject(XmlDocument document, List<Reference> references, List<XmlNode> objects)
	{
		List<Reference> manifestReferences = new List<Reference>();
		AddManifestReferences(manifestReferences);
		throw new NotImplementedException();
	}

	protected void AddManifestReferences(List<Reference> manifestReferences)
	{
		signatureConfig.GetOpcPackage().GetPartsByContentType(ContentTypes.RELATIONSHIPS_PART);
		new HashSet<string>();
		throw new NotImplementedException();
	}

	protected void AddSignatureTime(XmlDocument document, List<XmlNode> objectContent)
	{
		throw new NotImplementedException();
	}

	protected void AddSignatureInfo(XmlDocument document, List<Reference> references, List<XmlNode> objects)
	{
		throw new NotImplementedException();
	}

	protected static string GetRelationshipReferenceURI(string zipEntryName)
	{
		return "/" + zipEntryName + "?ContentType=application/vnd.Openxmlformats-package.relationships+xml";
	}

	protected static string GetResourceReferenceURI(string resourceName, string contentType)
	{
		return "/" + resourceName + "?ContentType=" + contentType;
	}

	protected static bool IsSignedRelationship(string relationshipType)
	{
		string[] array = signed;
		foreach (string value in array)
		{
			if (relationshipType.EndsWith(value))
			{
				return true;
			}
		}
		if (relationshipType.EndsWith("customXml"))
		{
			return true;
		}
		return false;
	}
}
