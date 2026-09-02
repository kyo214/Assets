namespace NPOI.SS.UserModel;

public interface ICreationHelper
{
	IRichTextString CreateRichTextString(string text);

	IDataFormat CreateDataFormat();

	IHyperlink CreateHyperlink(HyperlinkType type);

	IFormulaEvaluator CreateFormulaEvaluator();

	ExtendedColor CreateExtendedColor();

	IClientAnchor CreateClientAnchor();
}
