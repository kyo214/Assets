using System;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.XSSF.UserModel;

public class XSSFHyperlink : IHyperlink
{
	private HyperlinkType _type;

	private PackageRelationship _externalRel;

	private CT_Hyperlink _ctHyperlink;

	private string _location;

	public HyperlinkType Type => _type;

	public string CellRef => _ctHyperlink.@ref;

	public string Address
	{
		get
		{
			return _location;
		}
		set
		{
			Validate(value);
			_location = value;
			if (_type == HyperlinkType.Document)
			{
				Location = value;
			}
		}
	}

	public string Label
	{
		get
		{
			return _ctHyperlink.display;
		}
		set
		{
			_ctHyperlink.display = value;
		}
	}

	public string Location
	{
		get
		{
			return _ctHyperlink.location;
		}
		set
		{
			_ctHyperlink.location = value;
		}
	}

	public int FirstColumn
	{
		get
		{
			return buildCellReference().Col;
		}
		set
		{
			SetCellReference(new CellReference(FirstRow, value));
		}
	}

	public int LastColumn
	{
		get
		{
			return buildCellReference().Col;
		}
		set
		{
			FirstColumn = value;
		}
	}

	public int FirstRow
	{
		get
		{
			return buildCellReference().Row;
		}
		set
		{
			SetCellReference(new CellReference(value, FirstColumn));
		}
	}

	public int LastRow
	{
		get
		{
			return buildCellReference().Row;
		}
		set
		{
			FirstRow = value;
		}
	}

	public string TextMark
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public string Tooltip
	{
		get
		{
			return _ctHyperlink.tooltip;
		}
		set
		{
			_ctHyperlink.tooltip = value;
		}
	}

	public XSSFHyperlink(HyperlinkType type)
	{
		_type = type;
		_ctHyperlink = new CT_Hyperlink();
		_externalRel = null;
	}

	public XSSFHyperlink(CT_Hyperlink ctHyperlink, PackageRelationship hyperlinkRel)
	{
		_ctHyperlink = ctHyperlink;
		_externalRel = hyperlinkRel;
		if (_externalRel == null)
		{
			if (ctHyperlink.location != null)
			{
				_type = HyperlinkType.Document;
				_location = ctHyperlink.location;
				return;
			}
			if (ctHyperlink.id != null)
			{
				throw new InvalidOperationException("The hyperlink for cell " + ctHyperlink.@ref + " references relation " + ctHyperlink.id + ", but that didn't exist!");
			}
			_type = HyperlinkType.Document;
		}
		else
		{
			Uri targetUri = _externalRel.TargetUri;
			_location = targetUri.ToString();
			if (ctHyperlink.location != null)
			{
				_location = _location + "#" + ctHyperlink.location;
			}
			if (_location.StartsWith("http://") || _location.StartsWith("https://") || _location.StartsWith("ftp://"))
			{
				_type = HyperlinkType.Url;
			}
			else if (_location.StartsWith("mailto:"))
			{
				_type = HyperlinkType.Email;
			}
			else
			{
				_type = HyperlinkType.File;
			}
		}
	}

	public XSSFHyperlink(IHyperlink other)
	{
		if (other is XSSFHyperlink)
		{
			XSSFHyperlink xSSFHyperlink = (XSSFHyperlink)other;
			_type = xSSFHyperlink.Type;
			_location = xSSFHyperlink._location;
			_externalRel = xSSFHyperlink._externalRel;
			_ctHyperlink = xSSFHyperlink._ctHyperlink.Copy();
		}
		else
		{
			_type = other.Type;
			_location = other.Address;
			_externalRel = null;
			_ctHyperlink = new CT_Hyperlink();
			SetCellReference(new CellReference(other.FirstRow, other.FirstColumn));
		}
	}

	public CT_Hyperlink GetCTHyperlink()
	{
		return _ctHyperlink;
	}

	public bool NeedsRelationToo()
	{
		return _type != HyperlinkType.Document;
	}

	internal void GenerateRelationIfNeeded(PackagePart sheetPart)
	{
		if (_externalRel == null && NeedsRelationToo())
		{
			PackageRelationship packageRelationship = sheetPart.AddExternalRelationship(_location, XSSFRelation.SHEET_HYPERLINKS.Relation);
			_ctHyperlink.id = packageRelationship.Id;
		}
	}

	[Obsolete("use property CellRef")]
	public string GetCellRef()
	{
		return _ctHyperlink.@ref;
	}

	private void Validate(string address)
	{
		switch (_type)
		{
		case HyperlinkType.Url:
		case HyperlinkType.Email:
		case HyperlinkType.File:
		{
			if (!Uri.TryCreate(address, UriKind.RelativeOrAbsolute, out var _))
			{
				throw new ArgumentException("Address of hyperlink must be a valid URI:" + address);
			}
			break;
		}
		default:
			throw new InvalidOperationException("Invalid Hyperlink type: " + _type);
		case HyperlinkType.Document:
			break;
		}
	}

	public void SetCellReference(string ref1)
	{
		_ctHyperlink.@ref = ref1;
	}

	protected void SetCellReference(CellReference ref1)
	{
		SetCellReference(ref1.FormatAsString());
	}

	private CellReference buildCellReference()
	{
		string text = _ctHyperlink.@ref;
		if (text == null)
		{
			text = "A1";
		}
		return new CellReference(text);
	}
}
