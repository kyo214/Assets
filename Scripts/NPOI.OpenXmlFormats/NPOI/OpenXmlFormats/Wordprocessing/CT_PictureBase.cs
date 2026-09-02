using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXmlFormats.Vml;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlInclude(typeof(CT_Picture))]
[XmlInclude(typeof(CT_Object))]
[XmlInclude(typeof(CT_Background))]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = true)]
public class CT_PictureBase
{
	private List<object> itemsField;

	private List<ItemsChoiceType9> itemsElementNameField;

	[XmlAnyElement(Namespace = "urn:schemas-microsoft-com:office:office", Order = 0)]
	[XmlAnyElement(Namespace = "urn:schemas-microsoft-com:vml", Order = 0)]
	[XmlChoiceIdentifier("ItemsElementName")]
	public List<object> Items
	{
		get
		{
			return itemsField;
		}
		set
		{
			itemsField = value;
		}
	}

	[XmlElement("ItemsElementName", Order = 1)]
	[XmlIgnore]
	public List<ItemsChoiceType9> ItemsElementName
	{
		get
		{
			return itemsElementNameField;
		}
		set
		{
			itemsElementNameField = value;
		}
	}

	public CT_PictureBase()
	{
		itemsElementNameField = new List<ItemsChoiceType9>();
		itemsField = new List<object>();
	}

	public void Set(object obj)
	{
		if (obj is CT_Group)
		{
			object[] items = ((CT_Group)obj).Items;
			foreach (object item in items)
			{
				lock (this)
				{
					itemsField.Add(item);
					itemsElementNameField.Add(ItemsChoiceType9.vml);
				}
			}
			return;
		}
		XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
		StringBuilder stringBuilder = new StringBuilder();
		XmlWriterSettings settings = new XmlWriterSettings
		{
			Encoding = Encoding.UTF8,
			OmitXmlDeclaration = true
		};
		XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
		xmlSerializerNamespaces.Add("v", "urn:schemas-microsoft-com:vml");
		xmlSerializerNamespaces.Add("o", "urn:schemas-microsoft-com:office:office");
		XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings);
		xmlSerializer.Serialize(xmlWriter, obj, xmlSerializerNamespaces);
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(stringBuilder.ToString());
		lock (this)
		{
			itemsField.Add(xmlDocument.DocumentElement.CloneNode(deep: true));
			itemsElementNameField.Add(ItemsChoiceType9.vml);
		}
	}
}
