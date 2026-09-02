namespace NPOI.SS.Extractor;

public interface IExcelExtractor
{
	bool IncludeCellComments { get; set; }

	bool IncludeSheetNames { get; set; }

	bool FormulasNotResults { get; set; }

	bool IncludeHeaderFooter { get; set; }

	string Text { get; }
}
