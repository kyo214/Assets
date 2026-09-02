using System;
using NPOI.POIFS.FileSystem;

namespace NPOI.POIFS.EventFileSystem;

public class POIFSWriterEventArgs : EventArgs
{
	private string documentName;

	private int limit;

	private POIFSDocumentPath path;

	private DocumentOutputStream stream;

	public virtual int Limit => limit;

	public virtual string Name => documentName;

	public virtual POIFSDocumentPath Path => path;

	public virtual DocumentOutputStream Stream => stream;

	public POIFSWriterEventArgs(DocumentOutputStream stream, POIFSDocumentPath path, string documentName, int limit)
	{
		this.stream = stream;
		this.path = path;
		this.documentName = documentName;
		this.limit = limit;
	}
}
