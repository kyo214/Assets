using NPOI.Util;

namespace NPOI;

public abstract class POITextExtractor : ICloseable
{
	private ICloseable fsToClose;

	public abstract string Text { get; }

	public abstract POITextExtractor MetadataTextExtractor { get; }

	public void SetFilesystem(ICloseable fs)
	{
		fsToClose = fs;
	}

	public virtual void Close()
	{
		if (fsToClose != null)
		{
			fsToClose.Close();
		}
	}
}
