using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = true)]
public class CT_FFData
{
	private List<object> itemsField;

	private List<FFDataItemsType> itemsElementNameField;

	[XmlElement("calcOnExit", typeof(CT_OnOff), Order = 0)]
	[XmlElement("checkBox", typeof(CT_FFCheckBox), Order = 0)]
	[XmlElement("ddList", typeof(CT_FFDDList), Order = 0)]
	[XmlElement("enabled", typeof(CT_OnOff), Order = 0)]
	[XmlElement("entryMacro", typeof(CT_MacroName), Order = 0)]
	[XmlElement("exitMacro", typeof(CT_MacroName), Order = 0)]
	[XmlElement("helpText", typeof(CT_FFHelpText), Order = 0)]
	[XmlElement("name", typeof(CT_FFName), Order = 0)]
	[XmlElement("statusText", typeof(CT_FFStatusText), Order = 0)]
	[XmlElement("textInput", typeof(CT_FFTextInput), Order = 0)]
	[XmlChoiceIdentifier("ItemsElementName")]
	public object[] Items
	{
		get
		{
			return itemsField.ToArray();
		}
		set
		{
			itemsField.Clear();
			itemsField.AddRange(value);
		}
	}

	[XmlElement("ItemsElementName", Order = 1)]
	[XmlIgnore]
	public FFDataItemsType[] ItemsElementName
	{
		get
		{
			return itemsElementNameField.ToArray();
		}
		set
		{
			itemsElementNameField.Clear();
			itemsElementNameField.AddRange(value);
		}
	}

	public CT_FFData()
	{
		itemsElementNameField = new List<FFDataItemsType>();
		itemsField = new List<object>();
	}

	internal static CT_FFData Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_FFData cT_FFData = new CT_FFData();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "name")
			{
				cT_FFData.AddNewObject(CT_FFName.Parse(childNode, namespaceManager), FFDataItemsType.name);
			}
			else if (childNode.LocalName == "enabled")
			{
				cT_FFData.AddNewObject(CT_OnOff.Parse(childNode, namespaceManager), FFDataItemsType.enabled);
			}
			else if (childNode.LocalName == "calcOnExit")
			{
				cT_FFData.AddNewObject(CT_OnOff.Parse(childNode, namespaceManager), FFDataItemsType.calcOnExit);
			}
			else if (childNode.LocalName == "checkBox")
			{
				cT_FFData.AddNewObject(CT_FFCheckBox.Parse(childNode, namespaceManager), FFDataItemsType.checkBox);
			}
			else if (childNode.LocalName == "ddList")
			{
				cT_FFData.AddNewObject(CT_FFDDList.Parse(childNode, namespaceManager), FFDataItemsType.ddList);
			}
			else if (childNode.LocalName == "entryMacro")
			{
				cT_FFData.AddNewObject(CT_MacroName.Parse(childNode, namespaceManager), FFDataItemsType.entryMacro);
			}
			else if (childNode.LocalName == "exitMacro")
			{
				cT_FFData.AddNewObject(CT_MacroName.Parse(childNode, namespaceManager), FFDataItemsType.exitMacro);
			}
			else if (childNode.LocalName == "helpText")
			{
				cT_FFData.AddNewObject(CT_FFHelpText.Parse(childNode, namespaceManager), FFDataItemsType.helpText);
			}
			else if (childNode.LocalName == "statusText")
			{
				cT_FFData.AddNewObject(CT_FFStatusText.Parse(childNode, namespaceManager), FFDataItemsType.statusText);
			}
			else if (childNode.LocalName == "textInput")
			{
				cT_FFData.AddNewObject(CT_FFTextInput.Parse(childNode, namespaceManager), FFDataItemsType.textInput);
			}
		}
		return cT_FFData;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}>");
		for (int i = 0; i < itemsElementNameField.Count; i++)
		{
			if (itemsElementNameField[i] == FFDataItemsType.name)
			{
				(itemsField[i] as CT_FFName).Write(sw, "name");
			}
			else if (itemsElementNameField[i] == FFDataItemsType.enabled)
			{
				(itemsField[i] as CT_OnOff).Write(sw, "enabled");
			}
			else if (itemsElementNameField[i] == FFDataItemsType.calcOnExit)
			{
				(itemsField[i] as CT_OnOff).Write(sw, "calcOnExit");
			}
			else if (itemsElementNameField[i] == FFDataItemsType.ddList)
			{
				(itemsField[i] as CT_FFDDList).Write(sw, "ddList");
			}
			else if (itemsElementNameField[i] == FFDataItemsType.checkBox)
			{
				(itemsField[i] as CT_FFCheckBox).Write(sw, "checkBox");
			}
			else if (itemsElementNameField[i] == FFDataItemsType.entryMacro)
			{
				(itemsField[i] as CT_MacroName).Write(sw, "entryMacro");
			}
			else if (itemsElementNameField[i] == FFDataItemsType.exitMacro)
			{
				(itemsField[i] as CT_MacroName).Write(sw, "exitMacro");
			}
			else if (itemsElementNameField[i] == FFDataItemsType.helpText)
			{
				(itemsField[i] as CT_FFHelpText).Write(sw, "helpText");
			}
			else if (itemsElementNameField[i] == FFDataItemsType.statusText)
			{
				(itemsField[i] as CT_FFStatusText).Write(sw, "statusText");
			}
			else if (itemsElementNameField[i] == FFDataItemsType.textInput)
			{
				(itemsField[i] as CT_FFTextInput).Write(sw, "textInput");
			}
		}
		sw.Write($"</w:{nodeName}>");
	}

	private void AddNewObject(object obj, FFDataItemsType type)
	{
		lock (this)
		{
			itemsElementNameField.Add(type);
			itemsField.Add(obj);
		}
	}

	private List<T> GetObjectList<T>(FFDataItemsType type) where T : class
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

	private int SizeOfObjectArray(FFDataItemsType type)
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

	private T GetObjectArray<T>(int p, FFDataItemsType type) where T : class
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

	private T InsertNewObject<T>(FFDataItemsType type, int p) where T : class, new()
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

	private T AddNewObject<T>(FFDataItemsType type) where T : class, new()
	{
		T val = new T();
		lock (this)
		{
			itemsElementNameField.Add(type);
			itemsField.Add(val);
			return val;
		}
	}

	private void SetObjectArray<T>(FFDataItemsType type, int p, T obj) where T : class
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

	private int GetObjectIndex(FFDataItemsType type, int p)
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

	private void RemoveObject(FFDataItemsType type, int p)
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

	public List<CT_FFCheckBox> GetCheckBoxList()
	{
		return GetObjectList<CT_FFCheckBox>(FFDataItemsType.checkBox);
	}
}
