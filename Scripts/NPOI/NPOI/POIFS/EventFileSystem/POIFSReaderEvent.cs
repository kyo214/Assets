using NPOI.POIFS.FileSystem;

namespace NPOI.POIFS.EventFileSystem;

public class POIFSReaderEvent
{
	private DocumentInputStream stream;

	private POIFSDocumentPath path;

	private string documentName;

	public DocumentInputStream Stream => stream;

	public POIFSDocumentPath Path => path;

	public string Name => documentName;

	public POIFSReaderEvent(DocumentInputStream stream, POIFSDocumentPath path, string documentName)
	{
		this.stream = stream;
		this.path = path;
		this.documentName = documentName;
	}
}
