namespace NPOI.OpenXml4Net.OPC;

public class ContentTypes
{
	public static string CORE_PROPERTIES_PART = "application/vnd.openxmlformats-package.core-properties+xml";

	public static string DIGITAL_SIGNATURE_CERTIFICATE_PART = "application/vnd.openxmlformats-package.digital-signature-certificate";

	public static string DIGITAL_SIGNATURE_ORIGIN_PART = "application/vnd.openxmlformats-package.digital-signature-origin";

	public static string DIGITAL_SIGNATURE_XML_SIGNATURE_PART = "application/vnd.openxmlformats-package.digital-signature-xmlsignature+xml";

	public static string RELATIONSHIPS_PART = "application/vnd.openxmlformats-package.relationships+xml";

	public static string CUSTOM_XML_PART = "application/vnd.openxmlformats-officedocument.customXmlProperties+xml";

	public static string PLAIN_OLD_XML = "application/xml";

	public static string IMAGE_JPEG = "image/jpeg";

	public static string EXTENSION_JPG_1 = "jpg";

	public static string EXTENSION_JPG_2 = "jpeg";

	public static string IMAGE_PNG = "image/png";

	public static string EXTENSION_PNG = "png";

	public static string IMAGE_GIF = "image/gif";

	public static string EXTENSION_GIF = "gif";

	public static string IMAGE_TIFF = "image/tiff";

	public static string EXTENSION_TIFF = "tiff";

	public static string IMAGE_PICT = "image/pict";

	public static string EXTENSION_PICT = "tiff";

	public static string XML = "text/xml";

	public static string EXTENSION_XML = "xml";

	public static string GetContentTypeFromFileExtension(string filename)
	{
		string text = filename.Substring(filename.LastIndexOf(".") + 1).ToLower();
		if (text.Equals(EXTENSION_JPG_1) || text.Equals(EXTENSION_JPG_2))
		{
			return IMAGE_JPEG;
		}
		if (text.Equals(EXTENSION_GIF))
		{
			return IMAGE_GIF;
		}
		if (text.Equals(EXTENSION_PICT))
		{
			return IMAGE_PICT;
		}
		if (text.Equals(EXTENSION_PNG))
		{
			return IMAGE_PNG;
		}
		if (text.Equals(EXTENSION_TIFF))
		{
			return IMAGE_TIFF;
		}
		if (text.Equals(EXTENSION_XML))
		{
			return XML;
		}
		return null;
	}
}
