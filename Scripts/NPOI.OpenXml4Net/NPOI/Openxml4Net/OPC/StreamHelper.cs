using System.IO;
using System.Text;
using System.Xml;

namespace NPOI.OpenXml4Net.OPC;

public class StreamHelper
{
	private StreamHelper()
	{
	}

	public static void SaveXmlInStream(XmlDocument xmlContent, Stream outStream)
	{
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
		xmlWriterSettings.Encoding = Encoding.UTF8;
		xmlWriterSettings.OmitXmlDeclaration = false;
		XmlWriter xmlWriter = XmlWriter.Create(outStream, xmlWriterSettings);
		xmlContent.WriteContentTo(xmlWriter);
		xmlWriter.Flush();
	}

	public static void CopyStream(Stream inStream, Stream outStream)
	{
		byte[] array = new byte[1024];
		int num = 0;
		int num2 = 0;
		while ((num = inStream.Read(array, 0, array.Length)) > 0)
		{
			outStream.Write(array, 0, num);
			num2 += num;
		}
	}
}
