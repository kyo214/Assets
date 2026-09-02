using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFFieldRun : XWPFRun
{
	private CT_SimpleField field;

	public string FieldInstruction
	{
		get
		{
			return this.field.instr;
		}
		set
		{
			this.field.instr = value;
		}
	}

	public XWPFFieldRun(CT_SimpleField field, CT_R run, IRunBody p)
		: base(run, p)
	{
		this.field = field;
	}

	public CT_SimpleField GetCTField()
	{
		return field;
	}
}
