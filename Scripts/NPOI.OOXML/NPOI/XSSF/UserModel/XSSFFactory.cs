using System;
using System.Reflection;
using NPOI.Util;

namespace NPOI.XSSF.UserModel;

public class XSSFFactory : POIXMLFactory
{
	private static POILogger logger = POILogFactory.GetLogger(typeof(XSSFFactory));

	private static XSSFFactory inst = new XSSFFactory();

	private XSSFFactory()
	{
	}

	public static XSSFFactory GetInstance()
	{
		return inst;
	}

	protected override POIXMLRelation GetDescriptor(string relationshipType)
	{
		return XSSFRelation.GetInstance(relationshipType);
	}

	protected override POIXMLDocumentPart CreateDocumentPart(Type cls, Type[] classes, object[] values)
	{
		if (classes == null)
		{
			classes = new Type[0];
		}
		ConstructorInfo constructor = cls.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, classes, null);
		if (constructor == null)
		{
			throw new MissingMethodException();
		}
		if (values == null)
		{
			values = new object[0];
		}
		return constructor.Invoke(values) as POIXMLDocumentPart;
	}
}
