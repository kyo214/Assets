using System;
using NPOI.POIFS.FileSystem;

namespace NPOI.POIFS.EventFileSystem;

public class POIFSReaderEventArgs : EventArgs
{
	private POIFSDocumentPath path;

	private OPOIFSDocument document;

	private string name;

	public virtual POIFSDocumentPath Path => path;

	public virtual OPOIFSDocument Document => document;

	public virtual DocumentInputStream Stream => new DocumentInputStream(document);

	public virtual string Name => name;

	public POIFSReaderEventArgs(string name, POIFSDocumentPath path, OPOIFSDocument document)
	{
		this.name = name;
		this.path = path;
		this.document = document;
	}
}
