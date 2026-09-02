using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = true)]
public class CT_SdtPr
{
	private ArrayList itemsField;

	private List<SdtPrElementType> itemsElementNameField;

	[XmlElement("alias", typeof(CT_String), Order = 0)]
	[XmlElement("bibliography", typeof(CT_Empty), Order = 0)]
	[XmlElement("citation", typeof(CT_Empty), Order = 0)]
	[XmlElement("comboBox", typeof(CT_SdtComboBox), Order = 0)]
	[XmlElement("dataBinding", typeof(CT_DataBinding), Order = 0)]
	[XmlElement("date", typeof(CT_SdtDate), Order = 0)]
	[XmlElement("docPartList", typeof(CT_SdtDocPart), Order = 0)]
	[XmlElement("docPartObj", typeof(CT_SdtDocPart), Order = 0)]
	[XmlElement("dropDownList", typeof(CT_SdtDropDownList), Order = 0)]
	[XmlElement("equation", typeof(CT_Empty), Order = 0)]
	[XmlElement("group", typeof(CT_Empty), Order = 0)]
	[XmlElement("id", typeof(CT_DecimalNumber), Order = 0)]
	[XmlElement("lock", typeof(CT_Lock), Order = 0)]
	[XmlElement("picture", typeof(CT_Empty), Order = 0)]
	[XmlElement("placeholder", typeof(CT_Placeholder), Order = 0)]
	[XmlElement("rPr", typeof(CT_RPr), Order = 0)]
	[XmlElement("richText", typeof(CT_Empty), Order = 0)]
	[XmlElement("showingPlcHdr", typeof(CT_OnOff), Order = 0)]
	[XmlElement("tag", typeof(CT_String), Order = 0)]
	[XmlElement("temporary", typeof(CT_OnOff), Order = 0)]
	[XmlElement("text", typeof(CT_SdtText), Order = 0)]
	[XmlChoiceIdentifier("ItemsElementName")]
	public ArrayList Items
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
	public List<SdtPrElementType> ItemsElementName
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

	public CT_SdtPr()
	{
		itemsElementNameField = new List<SdtPrElementType>();
		itemsField = new ArrayList();
	}

	public static CT_SdtPr Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_SdtPr cT_SdtPr = new CT_SdtPr();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "richText")
			{
				cT_SdtPr.Items.Add(new CT_Empty());
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.richText);
			}
			else if (childNode.LocalName == "docPartList")
			{
				cT_SdtPr.Items.Add(CT_SdtDocPart.Parse(childNode, namespaceManager));
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.docPartList);
			}
			else if (childNode.LocalName == "docPartObj")
			{
				cT_SdtPr.Items.Add(CT_SdtDocPart.Parse(childNode, namespaceManager));
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.docPartObj);
			}
			else if (childNode.LocalName == "dropDownList")
			{
				cT_SdtPr.Items.Add(CT_SdtDropDownList.Parse(childNode, namespaceManager));
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.dropDownList);
			}
			else if (childNode.LocalName == "equation")
			{
				cT_SdtPr.Items.Add(new CT_Empty());
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.equation);
			}
			else if (childNode.LocalName == "group")
			{
				cT_SdtPr.Items.Add(new CT_Empty());
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.group);
			}
			else if (childNode.LocalName == "id")
			{
				cT_SdtPr.Items.Add(CT_DecimalNumber.Parse(childNode, namespaceManager));
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.id);
			}
			else if (childNode.LocalName == "lock")
			{
				cT_SdtPr.Items.Add(CT_Lock.Parse(childNode, namespaceManager));
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.@lock);
			}
			else if (childNode.LocalName == "date")
			{
				cT_SdtPr.Items.Add(CT_SdtDate.Parse(childNode, namespaceManager));
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.date);
			}
			else if (childNode.LocalName == "placeholder")
			{
				cT_SdtPr.Items.Add(CT_Placeholder.Parse(childNode, namespaceManager));
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.placeholder);
			}
			else if (childNode.LocalName == "rPr")
			{
				cT_SdtPr.Items.Add(CT_RPr.Parse(childNode, namespaceManager));
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.rPr);
			}
			else if (childNode.LocalName == "showingPlcHdr")
			{
				cT_SdtPr.Items.Add(CT_OnOff.Parse(childNode, namespaceManager));
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.showingPlcHdr);
			}
			else if (childNode.LocalName == "tag")
			{
				cT_SdtPr.Items.Add(CT_String.Parse(childNode, namespaceManager));
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.tag);
			}
			else if (childNode.LocalName == "temporary")
			{
				cT_SdtPr.Items.Add(CT_OnOff.Parse(childNode, namespaceManager));
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.temporary);
			}
			else if (childNode.LocalName == "text")
			{
				cT_SdtPr.Items.Add(CT_SdtText.Parse(childNode, namespaceManager));
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.text);
			}
			else if (childNode.LocalName == "picture")
			{
				cT_SdtPr.Items.Add(new CT_Empty());
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.picture);
			}
			else if (childNode.LocalName == "alias")
			{
				cT_SdtPr.Items.Add(CT_String.Parse(childNode, namespaceManager));
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.alias);
			}
			else if (childNode.LocalName == "bibliography")
			{
				cT_SdtPr.Items.Add(new CT_Empty());
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.bibliography);
			}
			else if (childNode.LocalName == "citation")
			{
				cT_SdtPr.Items.Add(new CT_Empty());
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.citation);
			}
			else if (childNode.LocalName == "comboBox")
			{
				cT_SdtPr.Items.Add(CT_SdtComboBox.Parse(childNode, namespaceManager));
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.comboBox);
			}
			else if (childNode.LocalName == "dataBinding")
			{
				cT_SdtPr.Items.Add(CT_DataBinding.Parse(childNode, namespaceManager));
				cT_SdtPr.ItemsElementName.Add(SdtPrElementType.dataBinding);
			}
		}
		return cT_SdtPr;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		sw.Write(">");
		for (int i = 0; i < Items.Count; i++)
		{
			object obj = Items[i];
			SdtPrElementType sdtPrElementType = itemsElementNameField[i];
			if (obj is CT_Empty && sdtPrElementType == SdtPrElementType.richText)
			{
				sw.Write("<w:richText/>");
			}
			else if (obj is CT_SdtDocPart && sdtPrElementType == SdtPrElementType.docPartList)
			{
				((CT_SdtDocPart)obj).Write(sw, "docPartList");
			}
			else if (obj is CT_SdtDocPart && sdtPrElementType == SdtPrElementType.docPartObj)
			{
				((CT_SdtDocPart)obj).Write(sw, "docPartObj");
			}
			else if (obj is CT_SdtDropDownList)
			{
				((CT_SdtDropDownList)obj).Write(sw, "dropDownList");
			}
			else if (obj is CT_Empty && sdtPrElementType == SdtPrElementType.equation)
			{
				sw.Write("<w:equation/>");
			}
			else if (obj is CT_Empty && sdtPrElementType == SdtPrElementType.group)
			{
				sw.Write("<w:group/>");
			}
			else if (obj is CT_DecimalNumber && sdtPrElementType == SdtPrElementType.id)
			{
				((CT_DecimalNumber)obj).Write(sw, "id");
			}
			else if (obj is CT_Lock)
			{
				((CT_Lock)obj).Write(sw, "lock");
			}
			else if (obj is CT_SdtDate)
			{
				((CT_SdtDate)obj).Write(sw, "date");
			}
			else if (obj is CT_Placeholder)
			{
				((CT_Placeholder)obj).Write(sw, "placeholder");
			}
			else if (obj is CT_RPr)
			{
				((CT_RPr)obj).Write(sw, "rPr");
			}
			else if (obj is CT_OnOff && sdtPrElementType == SdtPrElementType.showingPlcHdr)
			{
				((CT_OnOff)obj).Write(sw, "showingPlcHdr");
			}
			else if (obj is CT_String && sdtPrElementType == SdtPrElementType.tag)
			{
				((CT_String)obj).Write(sw, "tag");
			}
			else if (obj is CT_OnOff && sdtPrElementType == SdtPrElementType.temporary)
			{
				((CT_OnOff)obj).Write(sw, "temporary");
			}
			else if (obj is CT_SdtText)
			{
				((CT_SdtText)obj).Write(sw, "text");
			}
			else if (obj is CT_Empty && sdtPrElementType == SdtPrElementType.picture)
			{
				sw.Write("<w:picture/>");
			}
			else if (obj is CT_String && sdtPrElementType == SdtPrElementType.alias)
			{
				((CT_String)obj).Write(sw, "alias");
			}
			else if (obj is CT_Empty && sdtPrElementType == SdtPrElementType.bibliography)
			{
				sw.Write("<w:bibliography/>");
			}
			else if (obj is CT_Empty && sdtPrElementType == SdtPrElementType.citation)
			{
				sw.Write("<w:citation/>");
			}
			else if (obj is CT_SdtComboBox)
			{
				((CT_SdtComboBox)obj).Write(sw, "comboBox");
			}
			else if (obj is CT_DataBinding)
			{
				((CT_DataBinding)obj).Write(sw, "dataBinding");
			}
		}
		sw.Write($"</w:{nodeName}>");
	}

	public CT_DecimalNumber AddNewId()
	{
		return AddNewObject<CT_DecimalNumber>(SdtPrElementType.id);
	}

	public CT_SdtDocPart AddNewDocPartObj()
	{
		return AddNewObject<CT_SdtDocPart>(SdtPrElementType.docPartObj);
	}

	public CT_String[] GetAliasArray()
	{
		return GetObjectList<CT_String>(SdtPrElementType.alias).ToArray();
	}

	public List<T> GetObjectList<T>(SdtPrElementType type) where T : class
	{
		lock (this)
		{
			List<T> list = new List<T>();
			for (int i = 0; i < itemsElementNameField.Count; i++)
			{
				if (itemsElementNameField[i] == type)
				{
					list.Add(itemsField[i] as T);
				}
			}
			return list;
		}
	}

	private int SizeOfArray(SdtPrElementType type)
	{
		lock (this)
		{
			int num = 0;
			for (int i = 0; i < itemsElementNameField.Count; i++)
			{
				if (itemsElementNameField[i] == type)
				{
					num++;
				}
			}
			return num;
		}
	}

	private T GetObjectArray<T>(int p, SdtPrElementType type) where T : class
	{
		lock (this)
		{
			int objectIndex = GetObjectIndex(type, p);
			if (objectIndex < 0 || objectIndex >= itemsField.Count)
			{
				return null;
			}
			return itemsField[objectIndex] as T;
		}
	}

	private T InsertNewObject<T>(SdtPrElementType type, int p) where T : class, new()
	{
		T val = new T();
		lock (this)
		{
			int objectIndex = GetObjectIndex(type, p);
			itemsElementNameField.Insert(objectIndex, type);
			itemsField.Insert(objectIndex, val);
			return val;
		}
	}

	private T AddNewObject<T>(SdtPrElementType type) where T : class, new()
	{
		T val = new T();
		lock (this)
		{
			itemsElementNameField.Add(type);
			itemsField.Add(val);
			return val;
		}
	}

	private void SetObject<T>(SdtPrElementType type, int p, T obj) where T : class
	{
		lock (this)
		{
			int objectIndex = GetObjectIndex(type, p);
			if (objectIndex >= 0 && objectIndex < itemsField.Count)
			{
				if (!(itemsField[objectIndex] is T))
				{
					throw new Exception($"object types are difference, itemsField[{objectIndex}] is {itemsField[objectIndex].GetType().Name}, and parameter obj is {typeof(T).Name}");
				}
				itemsField[objectIndex] = obj;
			}
		}
	}

	private int GetObjectIndex(SdtPrElementType type, int p)
	{
		int result = -1;
		int num = 0;
		for (int i = 0; i < itemsElementNameField.Count; i++)
		{
			if (itemsElementNameField[i] == type)
			{
				if (num == p)
				{
					result = i;
					break;
				}
				num++;
			}
		}
		return result;
	}

	private void RemoveObject(SdtPrElementType type, int p)
	{
		lock (this)
		{
			int objectIndex = GetObjectIndex(type, p);
			if (objectIndex >= 0 && objectIndex < itemsField.Count)
			{
				itemsElementNameField.RemoveAt(objectIndex);
				itemsField.RemoveAt(objectIndex);
			}
		}
	}
}
