using System;
using System.Collections;
using System.Collections.Generic;
using NPOI.DDF;
using NPOI.HSSF.Record;

namespace NPOI.HSSF.UserModel;

public class HSSFShapeGroup : HSSFShape, HSSFShapeContainer, IEnumerable<HSSFShape>, IEnumerable
{
	private List<HSSFShape> shapes = new List<HSSFShape>();

	private EscherSpgrRecord _spgrRecord;

	public IList<HSSFShape> Children => shapes;

	public int X1 => _spgrRecord.RectX1;

	public int Y1 => _spgrRecord.RectY1;

	public int X2 => _spgrRecord.RectX2;

	public int Y2 => _spgrRecord.RectY2;

	public override int CountOfAllChildren
	{
		get
		{
			int num = shapes.Count;
			IEnumerator enumerator = shapes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				HSSFShape hSSFShape = (HSSFShape)enumerator.Current;
				num += hSSFShape.CountOfAllChildren;
			}
			return num;
		}
	}

	public override int ShapeId
	{
		get
		{
			return ((EscherSpRecord)((EscherContainerRecord)GetEscherContainer().GetChildById(-4092)).GetChildById(-4086)).ShapeId;
		}
		set
		{
			((EscherSpRecord)((EscherContainerRecord)GetEscherContainer().GetChildById(-4092)).GetChildById(-4086)).ShapeId = value;
			((CommonObjectDataSubRecord)GetObjRecord().SubRecords[0]).ObjectId = (short)(value % 1024);
		}
	}

	public HSSFShapeGroup(EscherContainerRecord spgrContainer, ObjRecord objRecord)
		: base(spgrContainer, objRecord)
	{
		EscherContainerRecord escherContainerRecord = spgrContainer.ChildContainers[0];
		_spgrRecord = (EscherSpgrRecord)escherContainerRecord.GetChild(0);
		foreach (EscherRecord childRecord in escherContainerRecord.ChildRecords)
		{
			switch (childRecord.RecordId)
			{
			case -4080:
				anchor = new HSSFClientAnchor((EscherClientAnchorRecord)childRecord);
				break;
			case -4081:
				anchor = new HSSFChildAnchor((EscherChildAnchorRecord)childRecord);
				break;
			}
		}
	}

	public HSSFShapeGroup(HSSFShape parent, HSSFAnchor anchor)
		: base(parent, anchor)
	{
		_spgrRecord = (EscherSpgrRecord)((EscherContainerRecord)GetEscherContainer().GetChild(0)).GetChildById(-4087);
	}

	protected override EscherContainerRecord CreateSpContainer()
	{
		EscherContainerRecord escherContainerRecord = new EscherContainerRecord();
		EscherContainerRecord escherContainerRecord2 = new EscherContainerRecord();
		EscherSpgrRecord escherSpgrRecord = new EscherSpgrRecord();
		EscherSpRecord escherSpRecord = new EscherSpRecord();
		EscherOptRecord escherOptRecord = new EscherOptRecord();
		EscherClientDataRecord escherClientDataRecord = new EscherClientDataRecord();
		escherContainerRecord.RecordId = -4093;
		escherContainerRecord.Options = 15;
		escherContainerRecord2.RecordId = -4092;
		escherContainerRecord2.Options = 15;
		escherSpgrRecord.RecordId = -4087;
		escherSpgrRecord.Options = 1;
		escherSpgrRecord.RectX1 = 0;
		escherSpgrRecord.RectY1 = 0;
		escherSpgrRecord.RectX2 = 1023;
		escherSpgrRecord.RectY2 = 255;
		escherSpRecord.RecordId = -4086;
		escherSpRecord.Options = 2;
		if (base.Anchor is HSSFClientAnchor)
		{
			escherSpRecord.Flags = 513;
		}
		else
		{
			escherSpRecord.Flags = 515;
		}
		escherOptRecord.RecordId = -4085;
		escherOptRecord.Options = 35;
		escherOptRecord.AddEscherProperty(new EscherBoolProperty(127, 262148));
		escherOptRecord.AddEscherProperty(new EscherBoolProperty(959, 524288));
		EscherRecord escherAnchor = base.Anchor.GetEscherAnchor();
		escherClientDataRecord.RecordId = -4079;
		escherClientDataRecord.Options = 0;
		escherContainerRecord.AddChildRecord(escherContainerRecord2);
		escherContainerRecord2.AddChildRecord(escherSpgrRecord);
		escherContainerRecord2.AddChildRecord(escherSpRecord);
		escherContainerRecord2.AddChildRecord(escherOptRecord);
		escherContainerRecord2.AddChildRecord(escherAnchor);
		escherContainerRecord2.AddChildRecord(escherClientDataRecord);
		return escherContainerRecord;
	}

	protected override ObjRecord CreateObjRecord()
	{
		ObjRecord objRecord = new ObjRecord();
		CommonObjectDataSubRecord o = new CommonObjectDataSubRecord
		{
			ObjectType = CommonObjectType.Group,
			IsLocked = true,
			IsPrintable = true,
			IsAutoFill = true,
			IsAutoline = true
		};
		GroupMarkerSubRecord o2 = new GroupMarkerSubRecord();
		EndSubRecord o3 = new EndSubRecord();
		objRecord.AddSubRecord(o);
		objRecord.AddSubRecord(o2);
		objRecord.AddSubRecord(o3);
		return objRecord;
	}

	internal override void AfterRemove(HSSFPatriarch patriarch)
	{
		patriarch.GetBoundAggregate().RemoveShapeToObjRecord(GetEscherContainer().ChildContainers[0].GetChildById(-4079));
		for (int i = 0; i < shapes.Count; i++)
		{
			HSSFShape hSSFShape = shapes[i];
			RemoveShape(hSSFShape);
			hSSFShape.AfterRemove(base.Patriarch);
		}
		shapes.Clear();
	}

	private void OnCreate(HSSFShape shape)
	{
		if (base.Patriarch != null)
		{
			EscherContainerRecord escherContainer = shape.GetEscherContainer();
			int shapeId = base.Patriarch.NewShapeId();
			shape.ShapeId = shapeId;
			GetEscherContainer().AddChildRecord(escherContainer);
			shape.AfterInsert(base.Patriarch);
			((!(shape is HSSFShapeGroup)) ? ((EscherSpRecord)shape.GetEscherContainer().GetChildById(-4086)) : ((EscherSpRecord)shape.GetEscherContainer().ChildContainers[0].GetChildById(-4086))).Flags |= 2;
		}
	}

	public HSSFShapeGroup CreateGroup(HSSFChildAnchor anchor)
	{
		HSSFShapeGroup hSSFShapeGroup = new HSSFShapeGroup(this, anchor);
		hSSFShapeGroup.Parent = this;
		hSSFShapeGroup.Anchor = anchor;
		shapes.Add(hSSFShapeGroup);
		OnCreate(hSSFShapeGroup);
		return hSSFShapeGroup;
	}

	public void AddShape(HSSFShape shape)
	{
		shape.Patriarch = base.Patriarch;
		shape.Parent = this;
		shapes.Add(shape);
	}

	public HSSFSimpleShape CreateShape(HSSFChildAnchor anchor)
	{
		HSSFSimpleShape hSSFSimpleShape = new HSSFSimpleShape(this, anchor);
		hSSFSimpleShape.Parent = this;
		hSSFSimpleShape.Anchor = anchor;
		shapes.Add(hSSFSimpleShape);
		OnCreate(hSSFSimpleShape);
		EscherSpRecord escherSpRecord = (EscherSpRecord)hSSFSimpleShape.GetEscherContainer().GetChildById(-4086);
		if (hSSFSimpleShape.Anchor.IsHorizontallyFlipped)
		{
			escherSpRecord.Flags |= 64;
		}
		if (hSSFSimpleShape.Anchor.IsVerticallyFlipped)
		{
			escherSpRecord.Flags |= 128;
		}
		return hSSFSimpleShape;
	}

	public HSSFTextbox CreateTextbox(HSSFChildAnchor anchor)
	{
		HSSFTextbox hSSFTextbox = new HSSFTextbox(this, anchor);
		hSSFTextbox.Parent = this;
		hSSFTextbox.Anchor = anchor;
		shapes.Add(hSSFTextbox);
		OnCreate(hSSFTextbox);
		return hSSFTextbox;
	}

	public HSSFPolygon CreatePolygon(HSSFChildAnchor anchor)
	{
		HSSFPolygon hSSFPolygon = new HSSFPolygon(this, anchor);
		hSSFPolygon.Parent = this;
		hSSFPolygon.Anchor = anchor;
		shapes.Add(hSSFPolygon);
		OnCreate(hSSFPolygon);
		return hSSFPolygon;
	}

	public HSSFPicture CreatePicture(HSSFChildAnchor anchor, int pictureIndex)
	{
		HSSFPicture hSSFPicture = new HSSFPicture(this, anchor);
		hSSFPicture.Parent = this;
		hSSFPicture.Anchor = anchor;
		hSSFPicture.PictureIndex = pictureIndex;
		shapes.Add(hSSFPicture);
		OnCreate(hSSFPicture);
		EscherSpRecord escherSpRecord = (EscherSpRecord)hSSFPicture.GetEscherContainer().GetChildById(-4086);
		if (hSSFPicture.Anchor.IsHorizontallyFlipped)
		{
			escherSpRecord.Flags |= 64;
		}
		if (hSSFPicture.Anchor.IsVerticallyFlipped)
		{
			escherSpRecord.Flags |= 128;
		}
		return hSSFPicture;
	}

	public void SetCoordinates(int x1, int y1, int x2, int y2)
	{
		_spgrRecord.RectX1 = x1;
		_spgrRecord.RectX2 = x2;
		_spgrRecord.RectY1 = y1;
		_spgrRecord.RectY2 = y2;
	}

	public void Clear()
	{
		foreach (HSSFShape item in new List<HSSFShape>(shapes))
		{
			RemoveShape(item);
		}
	}

	internal override void AfterInsert(HSSFPatriarch patriarch)
	{
		EscherAggregate boundAggregate = patriarch.GetBoundAggregate();
		EscherContainerRecord escherContainerRecord = (EscherContainerRecord)GetEscherContainer().GetChildById(-4092);
		boundAggregate.AssociateShapeToObjRecord(escherContainerRecord.GetChildById(-4079), GetObjRecord());
	}

	internal override HSSFShape CloneShape()
	{
		throw new NotImplementedException("Use method cloneShape(HSSFPatriarch patriarch)");
	}

	internal HSSFShape CloneShape(HSSFPatriarch patriarch)
	{
		EscherContainerRecord obj = new EscherContainerRecord
		{
			RecordId = -4093,
			Options = 15
		};
		EscherContainerRecord escherContainerRecord = new EscherContainerRecord();
		byte[] data = ((EscherContainerRecord)GetEscherContainer().GetChildById(-4092)).Serialize();
		escherContainerRecord.FillFields(data, 0, new DefaultEscherRecordFactory());
		obj.AddChildRecord(escherContainerRecord);
		ObjRecord objRecord = null;
		if (GetObjRecord() != null)
		{
			objRecord = (ObjRecord)GetObjRecord().CloneViaReserialise();
		}
		HSSFShapeGroup hSSFShapeGroup = new HSSFShapeGroup(obj, objRecord);
		hSSFShapeGroup.Patriarch = patriarch;
		foreach (HSSFShape child in Children)
		{
			HSSFShape shape = ((!(child is HSSFShapeGroup)) ? child.CloneShape() : ((HSSFShapeGroup)child).CloneShape(patriarch));
			hSSFShapeGroup.AddShape(shape);
			hSSFShapeGroup.OnCreate(shape);
		}
		return hSSFShapeGroup;
	}

	public bool RemoveShape(HSSFShape shape)
	{
		bool num = GetEscherContainer().RemoveChildRecord(shape.GetEscherContainer());
		if (num)
		{
			shape.AfterRemove(base.Patriarch);
			shapes.Remove(shape);
		}
		return num;
	}

	public IEnumerator<HSSFShape> GetEnumerator()
	{
		return shapes.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return shapes.GetEnumerator();
	}
}
