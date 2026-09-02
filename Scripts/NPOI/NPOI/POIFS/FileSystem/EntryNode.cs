using System;
using NPOI.POIFS.Properties;

namespace NPOI.POIFS.FileSystem;

[Serializable]
public abstract class EntryNode : Entry
{
	protected Property _property;

	protected DirectoryNode _parent;

	public Property Property => _property;

	protected bool IsRoot => _parent == null;

	protected abstract bool IsDeleteOK { get; }

	public string Name => _property.Name;

	public virtual bool IsDirectoryEntry => false;

	public virtual bool IsDocumentEntry => false;

	public DirectoryEntry Parent => _parent;

	protected EntryNode()
		: this(null, null)
	{
	}

	protected EntryNode(Property property, DirectoryNode parent)
	{
		_property = property;
		_parent = parent;
	}

	public bool Delete()
	{
		bool result = false;
		if (!IsRoot && IsDeleteOK)
		{
			result = _parent.DeleteEntry(this);
		}
		return result;
	}

	public bool RenameTo(string newName)
	{
		bool result = false;
		if (!IsRoot)
		{
			result = _parent.ChangeName(Name, newName);
		}
		return result;
	}

	public override string ToString()
	{
		return Name;
	}
}
