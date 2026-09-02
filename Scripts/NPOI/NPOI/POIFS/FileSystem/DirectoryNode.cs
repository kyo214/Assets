using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NPOI.POIFS.Dev;
using NPOI.POIFS.EventFileSystem;
using NPOI.POIFS.Properties;
using NPOI.Util;

namespace NPOI.POIFS.FileSystem;

[Serializable]
public class DirectoryNode : EntryNode, DirectoryEntry, Entry, IEnumerable<Entry>, IEnumerable, POIFSViewable
{
	private Dictionary<string, Entry> _byname;

	private List<Entry> _entries;

	private OPOIFSFileSystem _oFilesSystem;

	private NPOIFSFileSystem _nFilesSystem;

	private POIFSDocumentPath _path;

	public POIFSDocumentPath Path => _path;

	public NPOIFSFileSystem FileSystem => _nFilesSystem;

	public OPOIFSFileSystem OFileSystem => _oFilesSystem;

	public NPOIFSFileSystem NFileSystem => _nFilesSystem;

	public IEnumerator<Entry> Entries => _entries.GetEnumerator();

	public List<string> EntryNames => new List<string>(_byname.Keys);

	public bool IsEmpty => _entries.Count == 0;

	public int EntryCount => _entries.Count;

	public ClassID StorageClsid
	{
		get
		{
			return base.Property.StorageClsid;
		}
		set
		{
			base.Property.StorageClsid = value;
		}
	}

	public override bool IsDirectoryEntry => true;

	protected override bool IsDeleteOK => IsEmpty;

	public Array ViewableArray => new object[0];

	public IEnumerator ViewableIterator
	{
		get
		{
			ArrayList arrayList = new ArrayList();
			arrayList.Add(base.Property);
			arrayList.AddRange(_entries);
			return arrayList.GetEnumerator();
		}
	}

	public bool PreferArray => false;

	public string ShortDescription => base.Name;

	public bool CanRead
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public bool CanSeek
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public bool CanWrite
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public long Length
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public long Position
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	internal DirectoryNode(DirectoryProperty property, OPOIFSFileSystem fileSystem, DirectoryNode parent)
		: this(property, parent, fileSystem, null)
	{
	}

	internal DirectoryNode(DirectoryProperty property, NPOIFSFileSystem nFileSystem, DirectoryNode parent)
		: this(property, parent, null, nFileSystem)
	{
	}

	private DirectoryNode(DirectoryProperty property, DirectoryNode parent, OPOIFSFileSystem oFileSystem, NPOIFSFileSystem nFileSystem)
		: base(property, parent)
	{
		_oFilesSystem = oFileSystem;
		_nFilesSystem = nFileSystem;
		if (parent == null)
		{
			_path = new POIFSDocumentPath();
		}
		else
		{
			_path = new POIFSDocumentPath(parent._path, new string[1] { property.Name });
		}
		_byname = new Dictionary<string, Entry>();
		_entries = new List<Entry>();
		IEnumerator<Property> children = property.Children;
		while (children.MoveNext())
		{
			Property current = children.Current;
			Entry entry = null;
			if (current.IsDirectory)
			{
				DirectoryProperty property2 = (DirectoryProperty)current;
				entry = ((_oFilesSystem == null) ? new DirectoryNode(property2, _nFilesSystem, this) : new DirectoryNode(property2, _oFilesSystem, this));
			}
			else
			{
				entry = new DocumentNode((DocumentProperty)current, this);
			}
			_entries.Add(entry);
			_byname.Add(entry.Name, entry);
		}
	}

	public DocumentInputStream CreatePOIFSDocumentReader(string documentName)
	{
		Entry entry = GetEntry(documentName);
		if (!entry.IsDocumentEntry)
		{
			throw new IOException("Entry '" + documentName + "' Is not a DocumentEntry");
		}
		return new DocumentInputStream((DocumentEntry)entry);
	}

	public DocumentEntry CreateDocument(OPOIFSDocument document)
	{
		DocumentProperty documentProperty = document.DocumentProperty;
		DocumentNode documentNode = new DocumentNode(documentProperty, this);
		((DirectoryProperty)base.Property).AddChild(documentProperty);
		_oFilesSystem.AddDocument(document);
		_entries.Add(documentNode);
		_byname.Add(documentProperty.Name, documentNode);
		return documentNode;
	}

	public bool ChangeName(string oldName, string newName)
	{
		bool flag = false;
		EntryNode entryNode = (EntryNode)_byname[oldName];
		if (entryNode != null)
		{
			flag = ((DirectoryProperty)base.Property).ChangeName(entryNode.Property, newName);
			if (flag)
			{
				_byname.Remove(oldName);
				_byname[entryNode.Property.Name] = entryNode;
			}
		}
		return flag;
	}

	public bool DeleteEntry(EntryNode entry)
	{
		bool flag = ((DirectoryProperty)base.Property).DeleteChild(entry.Property);
		if (flag)
		{
			_entries.Remove(entry);
			_byname.Remove(entry.Name);
			if (_oFilesSystem != null)
			{
				_oFilesSystem.Remove(entry);
			}
			else
			{
				try
				{
					_nFilesSystem.Remove(entry);
				}
				catch (IOException)
				{
				}
			}
		}
		return flag;
	}

	internal Entry GetEntry(int index)
	{
		return _entries[index];
	}

	public bool HasEntry(string name)
	{
		if (name != null)
		{
			return _byname.ContainsKey(name);
		}
		return false;
	}

	public Entry GetEntry(string name)
	{
		Entry value = null;
		if (name != null)
		{
			_byname.TryGetValue(name, out value);
		}
		if (value == null)
		{
			throw new FileNotFoundException("no such entry: \"" + name + "\"");
		}
		return value;
	}

	public DocumentInputStream CreateDocumentInputStream(Entry document)
	{
		if (!document.IsDocumentEntry)
		{
			throw new IOException("Entry '" + document.Name + "' is not a DocumentEntry");
		}
		return new DocumentInputStream((DocumentEntry)document);
	}

	public DocumentInputStream CreateDocumentInputStream(string documentName)
	{
		return CreateDocumentInputStream(GetEntry(documentName));
	}

	public DocumentEntry CreateDocument(NPOIFSDocument document)
	{
		try
		{
			DocumentProperty documentProperty = document.DocumentProperty;
			DocumentNode documentNode = new DocumentNode(documentProperty, this);
			((DirectoryProperty)base.Property).AddChild(documentProperty);
			_nFilesSystem.AddDocument(document);
			_entries.Add(documentNode);
			_byname[documentProperty.Name] = documentNode;
			return documentNode;
		}
		catch (IOException ex)
		{
			throw ex;
		}
	}

	public DirectoryEntry CreateDirectory(string name)
	{
		DirectoryProperty directoryProperty = new DirectoryProperty(name);
		DirectoryNode directoryNode;
		if (_oFilesSystem != null)
		{
			directoryNode = new DirectoryNode(directoryProperty, _oFilesSystem, this);
			_oFilesSystem.AddDirectory(directoryProperty);
		}
		else
		{
			directoryNode = new DirectoryNode(directoryProperty, _nFilesSystem, this);
			_nFilesSystem.AddDirectory(directoryProperty);
		}
		((DirectoryProperty)base.Property).AddChild(directoryProperty);
		_entries.Add(directoryNode);
		_byname[name] = directoryNode;
		return directoryNode;
	}

	public DocumentEntry CreateOrUpdateDocument(string name, Stream stream)
	{
		if (!HasEntry(name))
		{
			return CreateDocument(name, stream);
		}
		DocumentNode documentNode = (DocumentNode)GetEntry(name);
		if (_nFilesSystem != null)
		{
			new NPOIFSDocument(documentNode).ReplaceContents(stream);
			return documentNode;
		}
		DeleteEntry(documentNode);
		return CreateDocument(name, stream);
	}

	public DocumentEntry CreateDocument(string name, Stream stream)
	{
		try
		{
			if (_nFilesSystem != null)
			{
				return CreateDocument(new NPOIFSDocument(name, _nFilesSystem, stream));
			}
			return CreateDocument(new OPOIFSDocument(name, stream));
		}
		catch (IOException ex)
		{
			throw ex;
		}
	}

	public DocumentEntry CreateDocument(string name, int size, POIFSWriterListener writer)
	{
		if (_nFilesSystem != null)
		{
			return CreateDocument(new NPOIFSDocument(name, size, _nFilesSystem, writer));
		}
		return CreateDocument(new OPOIFSDocument(name, size, _path, writer));
	}

	public IEnumerator<Entry> GetEnumerator()
	{
		return _entries.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _entries.GetEnumerator();
	}

	public void Flush()
	{
		throw new NotImplementedException();
	}

	public int Read(byte[] buffer, int offset, int count)
	{
		throw new NotImplementedException();
	}

	public long Seek(long offset, SeekOrigin origin)
	{
		throw new NotImplementedException();
	}

	public void SetLength(long value)
	{
		throw new NotImplementedException();
	}
}
