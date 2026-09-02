using System;
using System.Collections;
using NPOI.POIFS.Dev;
using NPOI.POIFS.Properties;

namespace NPOI.POIFS.FileSystem;

public class DocumentNode : EntryNode, POIFSViewable, DocumentEntry, Entry
{
	private OPOIFSDocument _document;

	public OPOIFSDocument Document => _document;

	public int Size => base.Property.Size;

	public override bool IsDocumentEntry => true;

	protected override bool IsDeleteOK => true;

	public Array ViewableArray => new object[0];

	public IEnumerator ViewableIterator => ((IEnumerable)new ArrayList
	{
		(object)base.Property,
		(object)_document
	}).GetEnumerator();

	public bool PreferArray => false;

	public string ShortDescription => base.Name;

	public DocumentNode(DocumentProperty property, DirectoryNode parent)
		: base(property, parent)
	{
		_document = property.Document;
	}
}
