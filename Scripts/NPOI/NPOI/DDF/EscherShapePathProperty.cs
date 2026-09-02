namespace NPOI.DDF;

public class EscherShapePathProperty : EscherSimpleProperty
{
	public const int LINE_OF_STRAIGHT_SEGMENTS = 0;

	public const int CLOSED_POLYGON = 1;

	public const int CURVES = 2;

	public const int CLOSED_CURVES = 3;

	public const int COMPLEX = 4;

	public EscherShapePathProperty(short propertyNumber, int shapePath)
		: base(propertyNumber, isComplex: false, isBlipId: false, shapePath)
	{
	}
}
