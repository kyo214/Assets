using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NPOI.POIFS.Properties;

public class DirectoryProperty : Property, Parent, Child, IEnumerable<Property>, IEnumerable
{
	public class PropertyComparator : IComparer<Property>
	{
		public override bool Equals(object o)
		{
			return this == o;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public int Compare(Property o1, Property o2)
		{
			string value = "_VBA_PROJECT";
			string name = o1.Name;
			string name2 = o2.Name;
			int num = name.Length - name2.Length;
			if (num == 0)
			{
				num = (name.Equals(value, StringComparison.CurrentCulture) ? 1 : (name2.Equals(value, StringComparison.CurrentCulture) ? (-1) : ((name.StartsWith("__", StringComparison.Ordinal) && name2.StartsWith("__", StringComparison.Ordinal)) ? string.Compare(name, name2, StringComparison.OrdinalIgnoreCase) : (name.StartsWith("__", StringComparison.Ordinal) ? 1 : ((!name2.StartsWith("__", StringComparison.Ordinal)) ? string.Compare(name, name2, StringComparison.OrdinalIgnoreCase) : (-1))))));
			}
			return num;
		}
	}

	private List<Property> _children;

	private List<string> _children_names;

	public override bool IsDirectory => true;

	public IEnumerator<Property> Children => _children.GetEnumerator();

	public DirectoryProperty(string name)
	{
		_children = new List<Property>();
		_children_names = new List<string>();
		base.Name = name;
		Size = 0;
		base.PropertyType = 1;
		base.StartBlock = 0;
		base.NodeColor = 1;
	}

	public DirectoryProperty(int index, byte[] array, int offset)
		: base(index, array, offset)
	{
		_children = new List<Property>();
		_children_names = new List<string>();
	}

	public bool ChangeName(Property property, string newName)
	{
		string name = property.Name;
		property.Name = newName;
		string name2 = property.Name;
		if (_children_names.Contains(name2))
		{
			property.Name = name;
			return false;
		}
		_children_names.Add(name2);
		_children_names.Remove(name);
		return true;
	}

	public bool DeleteChild(Property property)
	{
		bool num = _children.Remove(property);
		if (num)
		{
			_children_names.Remove(property.Name);
		}
		return num;
	}

	public override void PreWrite()
	{
		if (_children.Count <= 0)
		{
			return;
		}
		Property[] array = new Property[_children.Count];
		_children.CopyTo(array, 0);
		Array.Sort(array, new PropertyComparator());
		int num = array.Length / 2;
		base.ChildProperty = array[num].Index;
		array[0].PreviousChild = null;
		array[0].NextChild = null;
		for (int i = 1; i < num; i++)
		{
			array[i].PreviousChild = array[i - 1];
			array[i].NextChild = null;
		}
		if (num != 0)
		{
			array[num].PreviousChild = array[num - 1];
		}
		if (num != array.Length - 1)
		{
			array[num].NextChild = array[num + 1];
			for (int j = num + 1; j < array.Length - 1; j++)
			{
				array[j].PreviousChild = null;
				array[j].NextChild = array[j + 1];
			}
			array[^1].PreviousChild = null;
			array[^1].NextChild = null;
		}
		else
		{
			array[num].NextChild = null;
		}
	}

	public void AddChild(Property property)
	{
		string name = property.Name;
		if (_children_names.Contains(name))
		{
			throw new IOException("Duplicate name \"" + name + "\"");
		}
		_children_names.Add(name);
		_children.Add(property);
	}

	public IEnumerator<Property> GetEnumerator()
	{
		return _children.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _children.GetEnumerator();
	}
}
