namespace NPOI.SS.UserModel;

public interface IHyperlink
{
	string Address { get; set; }

	string Label { get; set; }

	HyperlinkType Type { get; }

	int FirstRow { get; set; }

	int LastRow { get; set; }

	int FirstColumn { get; set; }

	int LastColumn { get; set; }

	string TextMark { get; set; }
}
