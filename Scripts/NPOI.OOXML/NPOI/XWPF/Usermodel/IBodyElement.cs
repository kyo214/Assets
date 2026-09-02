namespace NPOI.XWPF.UserModel;

public interface IBodyElement
{
	IBody Body { get; }

	POIXMLDocumentPart Part { get; }

	BodyType PartType { get; }

	BodyElementType ElementType { get; }
}
