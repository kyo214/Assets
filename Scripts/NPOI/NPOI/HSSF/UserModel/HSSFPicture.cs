using System;
using System.Drawing;
using System.IO;
using NPOI.DDF;
using NPOI.HSSF.Record;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.HSSF.UserModel;

public class HSSFPicture : HSSFSimpleShape, IPicture
{
	private static POILogger logger = POILogFactory.GetLogger(typeof(HSSFPicture));

	public int PictureIndex
	{
		get
		{
			return ((EscherSimpleProperty)GetOptRecord().Lookup(260))?.PropertyValue ?? (-1);
		}
		set
		{
			SetPropertyValue(new EscherSimpleProperty(260, isComplex: false, isBlipId: true, value));
		}
	}

	public IPictureData PictureData => new HSSFPictureData((_patriarch.Sheet.Workbook as HSSFWorkbook).Workbook.GetBSERecord(PictureIndex).BlipRecord);

	public string FileName
	{
		get
		{
			EscherComplexProperty escherComplexProperty = (EscherComplexProperty)GetOptRecord().Lookup(261);
			if (escherComplexProperty != null)
			{
				return Trim(StringUtil.GetFromUnicodeLE(escherComplexProperty.ComplexData));
			}
			return "";
		}
		set
		{
			byte[] toUnicodeLE = StringUtil.GetToUnicodeLE(value);
			EscherComplexProperty propertyValue = new EscherComplexProperty(261, isBlipId: true, toUnicodeLE);
			SetPropertyValue(propertyValue);
		}
	}

	public override int ShapeType
	{
		get
		{
			return base.ShapeType;
		}
		set
		{
			throw new InvalidOperationException("Shape type can not be changed in " + GetType().Name);
		}
	}

	public IClientAnchor ClientAnchor
	{
		get
		{
			HSSFAnchor hSSFAnchor = base.Anchor;
			if (!(hSSFAnchor is HSSFClientAnchor))
			{
				return null;
			}
			return (HSSFClientAnchor)hSSFAnchor;
		}
	}

	public ISheet Sheet => base.Patriarch.Sheet;

	public HSSFPicture(EscherContainerRecord spContainer, ObjRecord objRecord)
		: base(spContainer, objRecord)
	{
	}

	public HSSFPicture(HSSFShape parent, HSSFAnchor anchor)
		: base(parent, anchor)
	{
		base.ShapeType = 75;
		((CommonObjectDataSubRecord)GetObjRecord().SubRecords[0]).ObjectType = CommonObjectType.Picture;
	}

	protected override EscherContainerRecord CreateSpContainer()
	{
		EscherContainerRecord escherContainerRecord = base.CreateSpContainer();
		EscherOptRecord obj = (EscherOptRecord)escherContainerRecord.GetChildById(-4085);
		obj.RemoveEscherProperty(462);
		obj.RemoveEscherProperty(511);
		escherContainerRecord.RemoveChildRecord(escherContainerRecord.GetChildById(-4083));
		return escherContainerRecord;
	}

	public void Resize()
	{
		Resize(double.MaxValue);
	}

	public void Resize(double scale)
	{
		Resize(scale, scale);
	}

	public void Resize(double scaleX, double scaleY)
	{
		HSSFClientAnchor obj = (HSSFClientAnchor)ClientAnchor;
		obj.AnchorType = AnchorType.MoveDontResize;
		HSSFClientAnchor hSSFClientAnchor = GetPreferredSize(scaleX, scaleY) as HSSFClientAnchor;
		int row = obj.Row1 + (hSSFClientAnchor.Row2 - hSSFClientAnchor.Row1);
		int num = obj.Col1 + (hSSFClientAnchor.Col2 - hSSFClientAnchor.Col1);
		obj.Col2 = (short)num;
		obj.Dx2 = hSSFClientAnchor.Dx2;
		obj.Row2 = row;
		obj.Dy2 = hSSFClientAnchor.Dy2;
	}

	public IClientAnchor GetPreferredSize(double scale)
	{
		return GetPreferredSize(scale, scale);
	}

	public IClientAnchor GetPreferredSize(double scaleX, double scaleY)
	{
		ImageUtils.SetPreferredSize(this, scaleX, scaleY);
		return ClientAnchor;
	}

	public IClientAnchor GetPreferredSize()
	{
		return GetPreferredSize(1.0);
	}

	protected Size GetResolution(Image r)
	{
		return new Size((int)r.HorizontalResolution, (int)r.VerticalResolution);
	}

	public Size GetImageDimension()
	{
		using MemoryStream stream = new MemoryStream((_patriarch.Sheet.Workbook as HSSFWorkbook).Workbook.GetBSERecord(PictureIndex).BlipRecord.PictureData);
		using Image image = Image.FromStream(stream);
		return image.Size;
	}

	internal override void AfterInsert(HSSFPatriarch patriarch)
	{
		patriarch.GetBoundAggregate().AssociateShapeToObjRecord(GetEscherContainer().GetChildById(-4079), GetObjRecord());
		if (PictureIndex != -1)
		{
			(patriarch.Sheet.Workbook as HSSFWorkbook).Workbook.GetBSERecord(PictureIndex).Ref++;
		}
	}

	private string Trim(string value)
	{
		int num = value.Length;
		int i = 0;
		char[] array;
		for (array = value.ToCharArray(); i < num && array[i] <= ' '; i++)
		{
		}
		while (i < num && array[num - 1] <= ' ')
		{
			num--;
		}
		if (i <= 0 && num >= value.Length)
		{
			return value;
		}
		return value.Substring(i, num - i);
	}

	internal override HSSFShape CloneShape()
	{
		EscherContainerRecord escherContainerRecord = new EscherContainerRecord();
		byte[] data = GetEscherContainer().Serialize();
		escherContainerRecord.FillFields(data, 0, new DefaultEscherRecordFactory());
		ObjRecord objRecord = (ObjRecord)GetObjRecord().CloneViaReserialise();
		return new HSSFPicture(escherContainerRecord, objRecord);
	}
}
