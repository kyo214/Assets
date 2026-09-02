using System;
using System.Reflection;
using NPOI.Util;

namespace NPOI.XWPF.UserModel;

public class XWPFFactory : POIXMLFactory
{
	private static POILogger logger = POILogFactory.GetLogger(typeof(XWPFFactory));

	private static XWPFFactory inst = new XWPFFactory();

	private XWPFFactory()
	{
	}

	public static XWPFFactory GetInstance()
	{
		return inst;
	}

	protected override POIXMLRelation GetDescriptor(string relationshipType)
	{
		return XWPFRelation.GetInstance(relationshipType);
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
