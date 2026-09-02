using System;
using System.Text;

namespace NPOI.POIFS.FileSystem;

public class DocumentDescriptor
{
	private POIFSDocumentPath path;

	private string name;

	private int hashcode;

	public string Path => path.ToString();

	public string Name => name;

	public DocumentDescriptor(POIFSDocumentPath path, string name)
	{
		if (path == null)
		{
			throw new NullReferenceException("path must not be null");
		}
		if (name == null)
		{
			throw new NullReferenceException("name must not be null");
		}
		if (name.Length == 0)
		{
			throw new ArgumentException("name cannot be empty");
		}
		this.path = path;
		this.name = name;
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
				DocumentDescriptor documentDescriptor = (DocumentDescriptor)o;
				result = path.Equals(documentDescriptor.path) && name.Equals(documentDescriptor.name);
			}
		}
		return result;
	}

	public override int GetHashCode()
	{
		if (hashcode == 0)
		{
			hashcode = path.GetHashCode() ^ name.GetHashCode();
		}
		return hashcode;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(40 * (path.Length + 1));
		for (int i = 0; i < path.Length; i++)
		{
			stringBuilder.Append(path.GetComponent(i)).Append("/");
		}
		stringBuilder.Append(name);
		return stringBuilder.ToString();
	}
}
