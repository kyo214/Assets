using System;
using NPOI.DDF;
using NPOI.HSSF.Record;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

[Serializable]
public class HSSFSimpleShape : HSSFShape
{
	public const short OBJECT_TYPE_LINE = 20;

	public const short OBJECT_TYPE_RECTANGLE = 1;

	public const short OBJECT_TYPE_OVAL = 3;

	public const short OBJECT_TYPE_ARC = 19;

	public const short OBJECT_TYPE_PICTURE = 75;

	public const short OBJECT_TYPE_COMBO_BOX = 201;

	public const short OBJECT_TYPE_COMMENT = 202;

	public const short OBJECT_TYPE_MICROSOFT_OFFICE_DRAWING = 30;

	public const int WRAP_SQUARE = 0;

	public const int WRAP_BY_POINTS = 1;

	public const int WRAP_NONE = 2;

	private TextObjectRecord _textObjectRecord;

	public virtual int ShapeType
	{
		get
		{
			return ((EscherSpRecord)GetEscherContainer().GetChildById(-4086)).ShapeType;
		}
		set
		{
			((CommonObjectDataSubRecord)GetObjRecord().SubRecords[0]).ObjectType = CommonObjectType.MicrosoftOfficeDrawing;
			((EscherSpRecord)GetEscherContainer().GetChildById(-4086)).ShapeType = (short)value;
		}
	}

	public int WrapText
	{
		get
		{
			return ((EscherSimpleProperty)GetOptRecord().Lookup(133))?.PropertyValue ?? 0;
		}
		set
		{
			SetPropertyValue(new EscherSimpleProperty(133, isComplex: false, isBlipId: false, value));
		}
	}

	public virtual IRichTextString String
	{
		get
		{
			return _textObjectRecord.Str;
		}
		set
		{
			if (ShapeType == 0 || ShapeType == 20)
			{
				throw new InvalidOperationException("Cannot set text for shape type: " + ShapeType);
			}
			HSSFRichTextString hSSFRichTextString = (HSSFRichTextString)value;
			if (hSSFRichTextString.NumFormattingRuns == 0)
			{
				hSSFRichTextString.ApplyFont(0);
			}
			GetOrCreateTextObjRecord().Str = hSSFRichTextString;
			if (value.String != null)
			{
				SetPropertyValue(new EscherSimpleProperty(128, value.String.GetHashCode()));
			}
		}
	}

	public bool FlipVertical { get; set; }

	public bool FlipHorizontal { get; set; }

	public HSSFSimpleShape(EscherContainerRecord spContainer, ObjRecord objRecord, TextObjectRecord textObjectRecord)
		: base(spContainer, objRecord)
	{
		_textObjectRecord = textObjectRecord;
	}

	public HSSFSimpleShape(EscherContainerRecord spContainer, ObjRecord objRecord)
		: base(spContainer, objRecord)
	{
	}

	public HSSFSimpleShape(HSSFShape parent, HSSFAnchor anchor)
		: base(parent, anchor)
	{
		_textObjectRecord = CreateTextObjRecord();
	}

	protected internal TextObjectRecord GetTextObjectRecord()
	{
		return _textObjectRecord;
	}

	protected virtual TextObjectRecord CreateTextObjRecord()
	{
		return new TextObjectRecord
		{
			HorizontalTextAlignment = HorizontalTextAlignment.Center,
			VerticalTextAlignment = VerticalTextAlignment.Center,
			IsTextLocked = true,
			TextOrientation = TextOrientation.None,
			Str = new HSSFRichTextString("")
		};
	}

	internal override HSSFShape CloneShape()
	{
		TextObjectRecord textObjectRecord = null;
		EscherContainerRecord escherContainerRecord = new EscherContainerRecord();
		byte[] data = GetEscherContainer().Serialize();
		escherContainerRecord.FillFields(data, 0, new DefaultEscherRecordFactory());
		ObjRecord objRecord = (ObjRecord)GetObjRecord().CloneViaReserialise();
		if (GetTextObjectRecord() != null && String != null && String.String != null)
		{
			textObjectRecord = (TextObjectRecord)GetTextObjectRecord().CloneViaReserialise();
		}
		return new HSSFSimpleShape(escherContainerRecord, objRecord, textObjectRecord);
	}

	internal override void AfterInsert(HSSFPatriarch patriarch)
	{
		EscherAggregate boundAggregate = patriarch.GetBoundAggregate();
		boundAggregate.AssociateShapeToObjRecord(GetEscherContainer().GetChildById(-4079), GetObjRecord());
		if (GetTextObjectRecord() != null)
		{
			boundAggregate.AssociateShapeToObjRecord(GetEscherContainer().GetChildById(-4083), GetTextObjectRecord());
		}
	}

	internal override void AfterRemove(HSSFPatriarch patriarch)
	{
		patriarch.GetBoundAggregate().RemoveShapeToObjRecord(GetEscherContainer().GetChildById(-4079));
		if (GetEscherContainer().GetChildById(-4083) != null)
		{
			patriarch.GetBoundAggregate().RemoveShapeToObjRecord(GetEscherContainer().GetChildById(-4083));
		}
	}

	protected override EscherContainerRecord CreateSpContainer()
	{
		EscherContainerRecord obj = new EscherContainerRecord
		{
			RecordId = -4092,
			Options = 15
		};
		EscherSpRecord record = new EscherSpRecord
		{
			RecordId = -4086,
			Flags = 2560,
			Version = 2
		};
		EscherClientDataRecord record2 = new EscherClientDataRecord
		{
			RecordId = -4079,
			Options = 0
		};
		EscherOptRecord escherOptRecord = new EscherOptRecord();
		escherOptRecord.SetEscherProperty(new EscherSimpleProperty(462, 0));
		escherOptRecord.SetEscherProperty(new EscherBoolProperty(511, 524296));
		escherOptRecord.SetEscherProperty(new EscherRGBProperty(385, 134217737));
		escherOptRecord.SetEscherProperty(new EscherRGBProperty(448, 134217792));
		escherOptRecord.SetEscherProperty(new EscherBoolProperty(447, 65536));
		escherOptRecord.SetEscherProperty(new EscherBoolProperty(511, 524296));
		escherOptRecord.SetEscherProperty(new EscherShapePathProperty(324, 4));
		escherOptRecord.SetEscherProperty(new EscherBoolProperty(959, 524288));
		escherOptRecord.RecordId = -4085;
		EscherTextboxRecord record3 = new EscherTextboxRecord
		{
			RecordId = -4083,
			Options = 0
		};
		obj.AddChildRecord(record);
		obj.AddChildRecord(escherOptRecord);
		obj.AddChildRecord(base.Anchor.GetEscherAnchor());
		obj.AddChildRecord(record2);
		obj.AddChildRecord(record3);
		return obj;
	}

	protected override ObjRecord CreateObjRecord()
	{
		ObjRecord objRecord = new ObjRecord();
		CommonObjectDataSubRecord o = new CommonObjectDataSubRecord
		{
			IsLocked = true,
			IsPrintable = true,
			IsAutoFill = true,
			IsAutoline = true
		};
		EndSubRecord o2 = new EndSubRecord();
		objRecord.AddSubRecord(o);
		objRecord.AddSubRecord(o2);
		return objRecord;
	}

	private TextObjectRecord GetOrCreateTextObjRecord()
	{
		if (GetTextObjectRecord() == null)
		{
			_textObjectRecord = CreateTextObjRecord();
		}
		EscherTextboxRecord escherTextboxRecord = (EscherTextboxRecord)GetEscherContainer().GetChildById(-4083);
		if (escherTextboxRecord == null)
		{
			escherTextboxRecord = new EscherTextboxRecord();
			escherTextboxRecord.RecordId = -4083;
			escherTextboxRecord.Options = 0;
			GetEscherContainer().AddChildRecord(escherTextboxRecord);
			base.Patriarch.GetBoundAggregate().AssociateShapeToObjRecord(escherTextboxRecord, _textObjectRecord);
		}
		return _textObjectRecord;
	}
}
