using System.IO;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using NPOI.Util;

namespace NPOI.OpenXml4Net.OPC.Internal;

public class ZipContentTypeManager : ContentTypeManager
{
	private static POILogger logger = POILogFactory.GetLogger(typeof(ZipContentTypeManager));

	public ZipContentTypeManager(Stream in1, OPCPackage pkg)
		: base(in1, pkg)
	{
	}

	public override bool SaveImpl(XmlDocument content, Stream out1)
	{
		ZipOutputStream zipOutputStream = null;
		zipOutputStream = ((!(out1 is ZipOutputStream)) ? new ZipOutputStream(out1) : ((ZipOutputStream)out1));
		ZipEntry entry = new ZipEntry("[Content_Types].xml");
		try
		{
			zipOutputStream.PutNextEntry(entry);
			StreamHelper.SaveXmlInStream(content, out1);
			Stream stream = new MemoryStream();
			byte[] buffer = new byte[ZipHelper.READ_WRITE_FILE_BUFFER_SIZE];
			while (true)
			{
				int num = stream.Read(buffer, 0, ZipHelper.READ_WRITE_FILE_BUFFER_SIZE);
				if (num == 0)
				{
					break;
				}
				zipOutputStream.Write(buffer, 0, num);
			}
			zipOutputStream.CloseEntry();
		}
		catch (IOException exception)
		{
			logger.Log(7, "Cannot write: [Content_Types].xml in Zip !", exception);
			return false;
		}
		return true;
	}
}
