using System;
using System.IO;
using System.Text;

namespace NPOI.POIFS.FileSystem;

public class POIFSDocumentPath
{
	private string[] components;

	private int hashcode;

	public virtual int Length => components.Length;

	public virtual POIFSDocumentPath Parent
	{
		get
		{
			int num = components.Length - 1;
			if (num < 0)
			{
				return null;
			}
			string[] destinationArray = new string[num];
			Array.Copy(components, 0, destinationArray, 0, num);
			return new POIFSDocumentPath(destinationArray);
		}
	}

	public string Name
	{
		get
		{
			if (components.Length == 0)
			{
				return "";
			}
			return components[components.Length - 1];
		}
	}

	public POIFSDocumentPath()
	{
		components = new string[0];
	}

	public POIFSDocumentPath(string[] components)
	{
		if (components == null)
		{
			this.components = new string[0];
			return;
		}
		this.components = new string[components.Length];
		for (int i = 0; i < components.Length; i++)
		{
			if (components[i] == null || components[i].Length == 0)
			{
				throw new ArgumentException("components cannot contain null or empty strings");
			}
			this.components[i] = components[i];
		}
	}

	public POIFSDocumentPath(POIFSDocumentPath path, string[] components)
	{
		if (components == null)
		{
			this.components = new string[path.components.Length];
		}
		else
		{
			this.components = new string[path.components.Length + components.Length];
		}
		for (int i = 0; i < path.components.Length; i++)
		{
			this.components[i] = path.components[i];
		}
		if (components == null)
		{
			return;
		}
		for (int j = 0; j < components.Length; j++)
		{
			if (components[j] == null)
			{
				throw new ArgumentException("components cannot contain null");
			}
			_ = components[j].Length;
			this.components[j + path.components.Length] = components[j];
		}
	}

	public override bool Equals(object o)
	{
		bool result = false;
		if (o != null && o.GetType() == GetType())
		{
			if (this == o)
			{
				result = true;
			}
			else
			{
				POIFSDocumentPath pOIFSDocumentPath = (POIFSDocumentPath)o;
				if (pOIFSDocumentPath.components.Length == components.Length)
				{
					result = true;
					for (int i = 0; i < components.Length; i++)
					{
						if (!pOIFSDocumentPath.components[i].Equals(components[i]))
						{
							result = false;
							break;
						}
					}
				}
			}
		}
		return result;
	}

	public virtual string GetComponent(int n)
	{
		return components[n];
	}

	public override int GetHashCode()
	{
		if (hashcode == 0)
		{
			hashcode = ComputeHashCode();
		}
		return hashcode;
	}

	private int ComputeHashCode()
	{
		int num = 0;
		for (int i = 0; i < components.Length; i++)
		{
			num += components[i].GetHashCode();
		}
		return num;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int length = Length;
		stringBuilder.Append(Path.DirectorySeparatorChar);
		for (int i = 0; i < length; i++)
		{
			stringBuilder.Append(GetComponent(i));
			if (i < length - 1)
			{
				stringBuilder.Append(Path.DirectorySeparatorChar);
			}
		}
		return stringBuilder.ToString();
	}
}
