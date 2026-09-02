using System;
using NPOI.DDF;
using NPOI.HSSF.Record;

namespace NPOI.HSSF.UserModel;

[Serializable]
public class HSSFTextbox : HSSFSimpleShape
{
	public const short OBJECT_TYPE_TEXT = 6;

	public int MarginLeft
	{
		get
		{
			return ((EscherSimpleProperty)GetOptRecord().Lookup(129))?.PropertyValue ?? 0;
		}
		set
		{
			SetPropertyValue(new EscherSimpleProperty(129, value));
		}
	}

	public int MarginRight
	{
		get
		{
			return ((EscherSimpleProperty)GetOptRecord().Lookup(131))?.PropertyValue ?? 0;
		}
		set
		{
			SetPropertyValue(new EscherSimpleProperty(131, value));
		}
	}

	public int MarginTop
	{
		get
		{
			return ((EscherSimpleProperty)GetOptRecord().Lookup(130))?.PropertyValue ?? 0;
		}
		set
		{
			SetPropertyValue(new EscherSimpleProperty(130, value));
		}
	}

	public int MarginBottom
	{
		get
		{
			return ((EscherSimpleProperty)GetOptRecord().Lookup(132))?.PropertyValue ?? 0;
		}
		set
		{
			SetPropertyValue(new EscherSimpleProperty(132, value));
		}
	}

	public HorizontalTextAlignment HorizontalAlignment
	{
		get
		{
			return GetTextObjectRecord().HorizontalTextAlignment;
		}
		set
		{
			GetTextObjectRecord().HorizontalTextAlignment = value;
		}
	}

	public VerticalTextAlignment VerticalAlignment
	{
		get
		{
			return GetTextObjectRecord().VerticalTextAlignment;
		}
		set
		{
			GetTextObjectRecord().VerticalTextAlignment = value;
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

	public HSSFTextbox(EscherContainerRecord spContainer, ObjRecord objRecord, TextObjectRecord textObjectRecord)
		: base(spContainer, objRecord, textObjectRecord)
	{
	}

	public HSSFTextbox(HSSFShape parent, HSSFAnchor anchor)
		: base(parent, anchor)
	{
		HorizontalAlignment = HorizontalTextAlignment.Left;
		VerticalAlignment = VerticalTextAlignment.Top;
		String = new HSSFRichTextString("");
	}

	protected override ObjRecord CreateObjRecord()
	{
		ObjRecord objRecord = new ObjRecord();
		CommonObjectDataSubRecord o = new CommonObjectDataSubRecord
		{
			ObjectType = CommonObjectType.Text,
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

	protected override EscherContainerRecord CreateSpContainer()
	{
		EscherContainerRecord escherContainerRecord = new EscherContainerRecord();
		EscherSpRecord escherSpRecord = new EscherSpRecord();
		EscherOptRecord escherOptRecord = new EscherOptRecord();
		EscherClientDataRecord escherClientDataRecord = new EscherClientDataRecord();
		EscherTextboxRecord escherTextboxRecord = new EscherTextboxRecord();
		escherContainerRecord.RecordId = -4092;
		escherContainerRecord.Options = 15;
		escherSpRecord.RecordId = -4086;
		escherSpRecord.Options = 3234;
		escherSpRecord.Flags = 2560;
		escherOptRecord.RecordId = -4085;
		escherOptRecord.AddEscherProperty(new EscherSimpleProperty(128, 0));
		escherOptRecord.AddEscherProperty(new EscherSimpleProperty(133, 0));
		escherOptRecord.AddEscherProperty(new EscherSimpleProperty(135, 0));
		escherOptRecord.AddEscherProperty(new EscherSimpleProperty(959, 524288));
		escherOptRecord.AddEscherProperty(new EscherSimpleProperty(129, 0));
		escherOptRecord.AddEscherProperty(new EscherSimpleProperty(131, 0));
		escherOptRecord.AddEscherProperty(new EscherSimpleProperty(130, 0));
		escherOptRecord.AddEscherProperty(new EscherSimpleProperty(132, 0));
		escherOptRecord.SetEscherProperty(new EscherSimpleProperty(462, 0));
		escherOptRecord.SetEscherProperty(new EscherBoolProperty(511, 524296));
		escherOptRecord.SetEscherProperty(new EscherSimpleProperty(459, 9525));
		escherOptRecord.SetEscherProperty(new EscherRGBProperty(385, 134217737));
		escherOptRecord.SetEscherProperty(new EscherRGBProperty(448, 134217792));
		escherOptRecord.SetEscherProperty(new EscherBoolProperty(447, 65536));
		escherOptRecord.SetEscherProperty(new EscherBoolProperty(959, 524288));
		EscherRecord escherAnchor = base.Anchor.GetEscherAnchor();
		escherClientDataRecord.RecordId = -4079;
		escherClientDataRecord.Options = 0;
		escherTextboxRecord.RecordId = -4083;
		escherTextboxRecord.Options = 0;
		escherContainerRecord.AddChildRecord(escherSpRecord);
		escherContainerRecord.AddChildRecord(escherOptRecord);
		escherContainerRecord.AddChildRecord(escherAnchor);
		escherContainerRecord.AddChildRecord(escherClientDataRecord);
		escherContainerRecord.AddChildRecord(escherTextboxRecord);
		return escherContainerRecord;
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

	internal override HSSFShape CloneShape()
	{
		TextObjectRecord textObjectRecord = ((GetTextObjectRecord() == null) ? null : ((TextObjectRecord)GetTextObjectRecord().CloneViaReserialise()));
		EscherContainerRecord escherContainerRecord = new EscherContainerRecord();
		byte[] data = GetEscherContainer().Serialize();
		escherContainerRecord.FillFields(data, 0, new DefaultEscherRecordFactory());
		ObjRecord objRecord = (ObjRecord)GetObjRecord().CloneViaReserialise();
		return new HSSFTextbox(escherContainerRecord, objRecord, textObjectRecord);
	}

	internal override void AfterRemove(HSSFPatriarch patriarch)
	{
		patriarch.GetBoundAggregate().RemoveShapeToObjRecord(GetEscherContainer().GetChildById(-4079));
		patriarch.GetBoundAggregate().RemoveShapeToObjRecord(GetEscherContainer().GetChildById(-4083));
	}
}
