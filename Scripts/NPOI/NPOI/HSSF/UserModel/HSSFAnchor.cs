using NPOI.DDF;

namespace NPOI.HSSF.UserModel;

public abstract class HSSFAnchor
{
	protected bool _isHorizontallyFlipped;

	protected bool _isVerticallyFlipped;

	public abstract int Dx1 { get; set; }

	public abstract int Dy1 { get; set; }

	public abstract int Dy2 { get; set; }

	public abstract int Dx2 { get; set; }

	public abstract bool IsHorizontallyFlipped { get; }

	public abstract bool IsVerticallyFlipped { get; }

	public HSSFAnchor()
	{
		CreateEscherAnchor();
	}

	public HSSFAnchor(int dx1, int dy1, int dx2, int dy2)
	{
		CreateEscherAnchor();
		Dx1 = dx1;
		Dy1 = dy1;
		Dx2 = dx2;
		Dy2 = dy2;
	}

	public static HSSFAnchor CreateAnchorFromEscher(EscherContainerRecord container)
	{
		if (container.GetChildById(-4081) != null)
		{
			return new HSSFChildAnchor((EscherChildAnchorRecord)container.GetChildById(-4081));
		}
		if (container.GetChildById(-4080) != null)
		{
			return new HSSFClientAnchor((EscherClientAnchorRecord)container.GetChildById(-4080));
		}
		return null;
	}

	internal abstract EscherRecord GetEscherAnchor();

	protected abstract void CreateEscherAnchor();
}
