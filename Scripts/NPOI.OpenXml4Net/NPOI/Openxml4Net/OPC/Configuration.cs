using System;

namespace NPOI.OpenXml4Net.OPC;

public class Configuration
{
	private static string pathForXmlSchema = Environment.CurrentDirectory + "\\src\\schemas";

	public static string PathForXmlSchema
	{
		get
		{
			return pathForXmlSchema;
		}
		set
		{
			pathForXmlSchema = value;
		}
	}
}
