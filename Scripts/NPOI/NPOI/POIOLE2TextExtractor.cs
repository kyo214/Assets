using NPOI.HPSF;
using NPOI.HPSF.Extractor;
using NPOI.POIFS.FileSystem;

namespace NPOI;

public abstract class POIOLE2TextExtractor : POITextExtractor
{
	protected POIDocument document;

	public virtual DocumentSummaryInformation DocSummaryInformation => document.DocumentSummaryInformation;

	public virtual SummaryInformation SummaryInformation => document.SummaryInformation;

	public override POITextExtractor MetadataTextExtractor => new HPSFPropertiesExtractor(this);

	public DirectoryEntry Root => document.Directory;

	public POIOLE2TextExtractor(POIDocument document)
	{
		this.document = document;
		SetFilesystem(document);
	}

	protected POIOLE2TextExtractor(POIOLE2TextExtractor otherExtractor)
	{
		document = otherExtractor.document;
	}
}
