using NPOI.POIFS.FileSystem;

namespace NPOI.POIFS.EventFileSystem;

public class POIFSWriterEvent
{
	private DocumentOutputStream stream;

	private POIFSDocumentPath path;

	private string documentName;

	private int limit;

	public DocumentOutputStream Stream => stream;

	public POIFSDocumentPath Path => path;

	public string Name => documentName;

	public int Limit => limit;

	public POIFSWriterEvent(DocumentOutputStream stream, POIFSDocumentPath path, string documentName, int limit)
	{
		this.stream = stream;
		this.path = path;
		this.documentName = documentName;
		this.limit = limit;
	}
}
