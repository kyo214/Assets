namespace NPOI.POIFS.EventFileSystem;

public interface POIFSReaderListener
{
	void ProcessPOIFSReaderEvent(POIFSReaderEvent evt);
}
