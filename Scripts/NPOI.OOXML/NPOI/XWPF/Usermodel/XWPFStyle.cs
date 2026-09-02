using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFStyle
{
	private CT_Style ctStyle;

	protected XWPFStyles styles;

	public string StyleId
	{
		get
		{
			return ctStyle.styleId;
		}
		set
		{
			ctStyle.styleId = value;
		}
	}

	public ST_StyleType StyleType
	{
		get
		{
			return ctStyle.type;
		}
		set
		{
			ctStyle.type = value;
		}
	}

	public string BasisStyleID
	{
		get
		{
			if (ctStyle.basedOn != null)
			{
				return ctStyle.basedOn.val;
			}
			return null;
		}
	}

	public string LinkStyleID
	{
		get
		{
			if (ctStyle.link != null)
			{
				return ctStyle.link.val;
			}
			return null;
		}
	}

	public string NextStyleID
	{
		get
		{
			if (ctStyle.next != null)
			{
				return ctStyle.next.val;
			}
			return null;
		}
	}

	public string Name
	{
		get
		{
			if (ctStyle.IsSetName())
			{
				return ctStyle.name.val;
			}
			return null;
		}
	}

	public XWPFStyle(CT_Style style)
		: this(style, null)
	{
	}

	public XWPFStyle(CT_Style style, XWPFStyles styles)
	{
		ctStyle = style;
		this.styles = styles;
	}

	public void SetStyle(CT_Style style)
	{
		ctStyle = style;
	}

	public CT_Style GetCTStyle()
	{
		return ctStyle;
	}

	public XWPFStyles GetStyles()
	{
		return styles;
	}

	public bool HasSameName(XWPFStyle compStyle)
	{
		return compStyle.GetCTStyle().name.val.Equals(ctStyle.name.val);
	}
}
