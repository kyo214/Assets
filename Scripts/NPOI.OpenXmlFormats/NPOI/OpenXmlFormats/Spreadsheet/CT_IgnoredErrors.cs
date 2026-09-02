using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_IgnoredErrors
{
	private List<CT_IgnoredError> ignoredErrorField;

	private CT_ExtensionList extLstField;

	public List<CT_IgnoredError> ignoredError
	{
		get
		{
			return ignoredErrorField;
		}
		set
		{
			ignoredErrorField = value;
		}
	}

	public CT_ExtensionList extLst
	{
		get
		{
			return extLstField;
		}
		set
		{
			extLstField = value;
		}
	}

	public static CT_IgnoredErrors Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_IgnoredErrors cT_IgnoredErrors = new CT_IgnoredErrors();
		cT_IgnoredErrors.ignoredError = new List<CT_IgnoredError>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "extLst")
			{
				cT_IgnoredErrors.extLst = CT_ExtensionList.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "ignoredError")
			{
				cT_IgnoredErrors.ignoredError.Add(CT_IgnoredError.Parse(childNode, namespaceManager));
			}
		}
		return cT_IgnoredErrors;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		sw.Write(">");
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		if (ignoredError != null)
		{
			foreach (CT_IgnoredError item in ignoredError)
			{
				item.Write(sw, "ignoredError");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_IgnoredError AddNewIgnoredError()
	{
		if (ignoredError == null)
		{
			ignoredError = new List<CT_IgnoredError>();
		}
		CT_IgnoredError cT_IgnoredError = new CT_IgnoredError();
		ignoredError.Add(cT_IgnoredError);
		return cT_IgnoredError;
	}

	internal CT_IgnoredError GetIgnoredErrorArray(int v)
	{
		if (ignoredError == null)
		{
			return null;
		}
		return ignoredErrorField[v];
	}
}
