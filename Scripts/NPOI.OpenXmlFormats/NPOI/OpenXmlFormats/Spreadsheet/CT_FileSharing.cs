using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_FileSharing
{
	private bool readOnlyRecommendedField;

	private string userNameField;

	private byte[] reservationPasswordField;

	[XmlAttribute]
	[DefaultValue(false)]
	public bool readOnlyRecommended
	{
		get
		{
			return readOnlyRecommendedField;
		}
		set
		{
			readOnlyRecommendedField = value;
		}
	}

	[XmlAttribute]
	public string userName
	{
		get
		{
			return userNameField;
		}
		set
		{
			userNameField = value;
		}
	}

	[XmlAttribute]
	public byte[] reservationPassword
	{
		get
		{
			return reservationPasswordField;
		}
		set
		{
			reservationPasswordField = value;
		}
	}

	public CT_FileSharing()
	{
		readOnlyRecommendedField = false;
	}

	public static CT_FileSharing Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_FileSharing
		{
			readOnlyRecommended = XmlHelper.ReadBool(node.Attributes["readOnlyRecommended"]),
			userName = XmlHelper.ReadString(node.Attributes["userName"]),
			reservationPassword = XmlHelper.ReadBytes(node.Attributes["reservationPassword"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "readOnlyRecommended", readOnlyRecommended);
		XmlHelper.WriteAttribute(sw, "userName", userName);
		XmlHelper.WriteAttribute(sw, "reservationPassword", reservationPassword);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}
}
