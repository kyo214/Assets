using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes")]
public class CT_Vector
{
	private List<object> itemsField;

	private List<ST_VectorBaseType> itemsElementNameField;

	private ST_VectorBaseType baseTypeField;

	private uint sizeField;

	[XmlElement("bool", typeof(bool))]
	[XmlElement("bstr", typeof(string))]
	[XmlElement("cf", typeof(CT_Cf))]
	[XmlElement("clsid", typeof(string))]
	[XmlElement("cy", typeof(string))]
	[XmlElement("date", typeof(DateTime))]
	[XmlElement("error", typeof(string))]
	[XmlElement("filetime", typeof(DateTime))]
	[XmlElement("i1", typeof(sbyte))]
	[XmlElement("i2", typeof(short))]
	[XmlElement("i4", typeof(int))]
	[XmlElement("i8", typeof(long))]
	[XmlElement("lpstr", typeof(string))]
	[XmlElement("lpwstr", typeof(string))]
	[XmlElement("r4", typeof(float))]
	[XmlElement("r8", typeof(double))]
	[XmlElement("ui1", typeof(byte))]
	[XmlElement("ui2", typeof(ushort))]
	[XmlElement("ui4", typeof(uint))]
	[XmlElement("ui8", typeof(ulong))]
	[XmlElement("variant", typeof(CT_Variant))]
	[XmlChoiceIdentifier("ItemsElementName")]
	public object[] Items
	{
		get
		{
			return itemsField.ToArray();
		}
		set
		{
			if (value != null && value.Length != 0)
			{
				itemsField = new List<object>(value);
			}
			else
			{
				itemsField = new List<object>();
			}
		}
	}

	[XmlIgnore]
	public ST_VectorBaseType[] ItemsElementName
	{
		get
		{
			return itemsElementNameField.ToArray();
		}
		set
		{
			if (value != null && value.Length != 0)
			{
				itemsElementNameField = new List<ST_VectorBaseType>(value);
			}
			else
			{
				itemsElementNameField = new List<ST_VectorBaseType>();
			}
		}
	}

	[XmlAttribute]
	public ST_VectorBaseType baseType
	{
		get
		{
			return baseTypeField;
		}
		set
		{
			baseTypeField = value;
		}
	}

	[XmlAttribute]
	public uint size
	{
		get
		{
			return sizeField;
		}
		set
		{
			sizeField = value;
		}
	}

	public CT_Vector()
	{
		itemsElementNameField = new List<ST_VectorBaseType>();
		itemsField = new List<object>();
	}

	public int SizeOfVariantArray()
	{
		return SizeOfArray(ST_VectorBaseType.variant);
	}

	public CT_Variant GetVariantArray(int p)
	{
		return GetObjectArray<CT_Variant>(p, ST_VectorBaseType.variant);
	}

	public int SizeOfLpstrArray()
	{
		return SizeOfArray(ST_VectorBaseType.lpstr);
	}

	public string GetLpstrArray(int p)
	{
		return GetObjectArray<string>(p, ST_VectorBaseType.lpstr);
	}

	private List<T> GetObjectList<T>(ST_VectorBaseType type) where T : class
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

	private int SizeOfArray(ST_VectorBaseType type)
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

	private T GetObjectArray<T>(int p, ST_VectorBaseType type) where T : class
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

	private T AddNewObject<T>(ST_VectorBaseType type) where T : class, new()
	{
		T val = new T();
		lock (this)
		{
			itemsElementNameField.Add(type);
			itemsField.Add(val);
			return val;
		}
	}

	private void SetObject<T>(ST_VectorBaseType type, int p, T obj) where T : class
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

	private int GetObjectIndex(ST_VectorBaseType type, int p)
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

	private void RemoveObject(ST_VectorBaseType type, int p)
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
