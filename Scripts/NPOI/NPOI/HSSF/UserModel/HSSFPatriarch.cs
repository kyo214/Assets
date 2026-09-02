using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NPOI.DDF;
using NPOI.HSSF.Model;
using NPOI.HSSF.Record;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.HSSF.UserModel;

public class HSSFPatriarch : HSSFShapeContainer, IEnumerable<HSSFShape>, IEnumerable, IDrawing
{
	private List<HSSFShape> _shapes = new List<HSSFShape>();

	private HSSFSheet _sheet;

	private EscherSpgrRecord _spgrRecord;

	private EscherContainerRecord _mainSpgrContainer;

	private EscherAggregate _boundAggregate;

	public IList<HSSFShape> Children => _shapes;

	public int CountOfAllChildren
	{
		get
		{
			int num = _shapes.Count;
			IEnumerator enumerator = _shapes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				HSSFShape hSSFShape = (HSSFShape)enumerator.Current;
				num += hSSFShape.CountOfAllChildren;
			}
			return num;
		}
	}

	public int X1 => _spgrRecord.RectX1;

	public int Y1 => _spgrRecord.RectY1;

	public int X2 => _spgrRecord.RectX2;

	public int Y2 => _spgrRecord.RectY2;

	protected internal HSSFSheet Sheet => _sheet;

	public HSSFPatriarch(HSSFSheet sheet, EscherAggregate boundAggregate)
	{
		_boundAggregate = boundAggregate;
		_sheet = sheet;
		_mainSpgrContainer = _boundAggregate.GetEscherContainer().ChildContainers[0];
		EscherContainerRecord escherContainerRecord = (EscherContainerRecord)_boundAggregate.GetEscherContainer().ChildContainers[0].GetChild(0);
		_spgrRecord = (EscherSpgrRecord)escherContainerRecord.GetChildById(-4087);
		BuildShapeTree();
	}

	public static HSSFPatriarch CreatePatriarch(HSSFPatriarch patriarch, HSSFSheet sheet)
	{
		HSSFPatriarch hSSFPatriarch = new HSSFPatriarch(sheet, new EscherAggregate(createDefaultTree: true));
		hSSFPatriarch.AfterCreate();
		foreach (HSSFShape child in patriarch.Children)
		{
			HSSFShape shape = ((!(child is HSSFShapeGroup)) ? child.CloneShape() : ((HSSFShapeGroup)child).CloneShape(hSSFPatriarch));
			hSSFPatriarch.OnCreate(shape);
			hSSFPatriarch.AddShape(shape);
		}
		return hSSFPatriarch;
	}

	protected internal void PreSerialize()
	{
		Dictionary<int, NoteRecord> tailRecords = _boundAggregate.TailRecords;
		Hashtable hashtable = new Hashtable(tailRecords.Count);
		foreach (NoteRecord value in tailRecords.Values)
		{
			string text = new CellReference(value.Row, value.Column).FormatAsString();
			if (hashtable.Contains(text))
			{
				throw new InvalidOperationException("found multiple cell comments for cell " + text);
			}
			hashtable.Add(text, null);
		}
	}

	public bool RemoveShape(HSSFShape shape)
	{
		bool num = _mainSpgrContainer.RemoveChildRecord(shape.GetEscherContainer());
		if (num)
		{
			shape.AfterRemove(this);
			_shapes.Remove(shape);
		}
		return num;
	}

	internal void AfterCreate()
	{
		DrawingManager2 drawingManager = ((HSSFWorkbook)_sheet.Workbook).Workbook.DrawingManager;
		short dgId = drawingManager.FindNewDrawingGroupId();
		_boundAggregate.SetDgId(dgId);
		_boundAggregate.SetMainSpRecordId(NewShapeId());
		drawingManager.IncrementDrawingsSaved();
	}

	public HSSFShapeGroup CreateGroup(HSSFClientAnchor anchor)
	{
		HSSFShapeGroup hSSFShapeGroup = new HSSFShapeGroup(null, anchor);
		AddShape(hSSFShapeGroup);
		OnCreate(hSSFShapeGroup);
		return hSSFShapeGroup;
	}

	public HSSFSimpleShape CreateSimpleShape(HSSFClientAnchor anchor)
	{
		HSSFSimpleShape hSSFSimpleShape = new HSSFSimpleShape(null, anchor);
		AddShape(hSSFSimpleShape);
		OnCreate(hSSFSimpleShape);
		return hSSFSimpleShape;
	}

	public IPicture CreatePicture(HSSFClientAnchor anchor, int pictureIndex)
	{
		HSSFPicture hSSFPicture = new HSSFPicture(null, anchor);
		hSSFPicture.PictureIndex = pictureIndex;
		AddShape(hSSFPicture);
		OnCreate(hSSFPicture);
		return hSSFPicture;
	}

	public IPicture CreatePicture(IClientAnchor anchor, int pictureIndex)
	{
		return CreatePicture((HSSFClientAnchor)anchor, pictureIndex);
	}

	public HSSFObjectData CreateObjectData(HSSFClientAnchor anchor, int storageId, int pictureIndex)
	{
		ObjRecord objRecord = new ObjRecord();
		CommonObjectDataSubRecord commonObjectDataSubRecord = new CommonObjectDataSubRecord();
		commonObjectDataSubRecord.ObjectType = CommonObjectType.Picture;
		commonObjectDataSubRecord.IsLocked = true;
		commonObjectDataSubRecord.IsPrintable = true;
		commonObjectDataSubRecord.IsAutoFill = true;
		commonObjectDataSubRecord.IsAutoline = true;
		commonObjectDataSubRecord.Reserved1 = 0;
		commonObjectDataSubRecord.Reserved2 = 0;
		commonObjectDataSubRecord.Reserved3 = 0;
		objRecord.AddSubRecord(commonObjectDataSubRecord);
		FtCfSubRecord ftCfSubRecord = new FtCfSubRecord();
		HSSFPictureData hSSFPictureData = Sheet.Workbook.GetAllPictures()[pictureIndex - 1] as HSSFPictureData;
		switch ((PictureType)hSSFPictureData.Format)
		{
		case PictureType.EMF:
		case PictureType.WMF:
			ftCfSubRecord.Flags = FtCfSubRecord.METAFILE_BIT;
			break;
		case PictureType.PICT:
		case PictureType.JPEG:
		case PictureType.PNG:
		case PictureType.DIB:
			ftCfSubRecord.Flags = FtCfSubRecord.BITMAP_BIT;
			break;
		default:
			throw new InvalidOperationException("Invalid picture type: " + hSSFPictureData.Format);
		}
		objRecord.AddSubRecord(ftCfSubRecord);
		FtPioGrbitSubRecord ftPioGrbitSubRecord = new FtPioGrbitSubRecord();
		ftPioGrbitSubRecord.SetFlagByBit(FtPioGrbitSubRecord.AUTO_PICT_BIT, enabled: true);
		objRecord.AddSubRecord(ftPioGrbitSubRecord);
		EmbeddedObjectRefSubRecord embeddedObjectRefSubRecord = new EmbeddedObjectRefSubRecord();
		embeddedObjectRefSubRecord.SetUnknownFormulaData(new byte[5] { 2, 0, 0, 0, 0 });
		embeddedObjectRefSubRecord.OLEClassName = "Paket";
		embeddedObjectRefSubRecord.SetStorageId(storageId);
		objRecord.AddSubRecord(embeddedObjectRefSubRecord);
		objRecord.AddSubRecord(new EndSubRecord());
		string name = "MBD" + HexDump.ToHex(storageId);
		DirectoryEntry root;
		try
		{
			root = (DirectoryEntry)((_sheet.Workbook as HSSFWorkbook).RootDirectory ?? throw new FileNotFoundException()).GetEntry(name);
		}
		catch (FileNotFoundException innerException)
		{
			throw new InvalidOperationException("trying to add ole shape without actually Adding data first - use HSSFWorkbook.AddOlePackage first", innerException);
		}
		EscherContainerRecord escherContainer = new HSSFPicture(null, anchor)
		{
			PictureIndex = pictureIndex
		}.GetEscherContainer();
		(escherContainer.GetChildById(-4086) as EscherSpRecord).Flags |= 16;
		HSSFObjectData hSSFObjectData = new HSSFObjectData(escherContainer, objRecord, root);
		AddShape(hSSFObjectData);
		OnCreate(hSSFObjectData);
		return hSSFObjectData;
	}

	public HSSFPolygon CreatePolygon(IClientAnchor anchor)
	{
		HSSFPolygon hSSFPolygon = new HSSFPolygon(null, (HSSFAnchor)anchor);
		AddShape(hSSFPolygon);
		OnCreate(hSSFPolygon);
		return hSSFPolygon;
	}

	public HSSFSimpleShape CreateTextbox(IClientAnchor anchor)
	{
		HSSFTextbox hSSFTextbox = new HSSFTextbox(null, (HSSFAnchor)anchor);
		AddShape(hSSFTextbox);
		OnCreate(hSSFTextbox);
		return hSSFTextbox;
	}

	public HSSFComment CreateComment(HSSFAnchor anchor)
	{
		HSSFComment hSSFComment = new HSSFComment(null, anchor);
		AddShape(hSSFComment);
		OnCreate(hSSFComment);
		return hSSFComment;
	}

	public HSSFSimpleShape CreateComboBox(HSSFAnchor anchor)
	{
		HSSFCombobox hSSFCombobox = new HSSFCombobox(null, anchor);
		AddShape(hSSFCombobox);
		OnCreate(hSSFCombobox);
		return hSSFCombobox;
	}

	public IComment CreateCellComment(IClientAnchor anchor)
	{
		return CreateComment((HSSFAnchor)anchor);
	}

	private void SetFlipFlags(HSSFShape shape)
	{
		EscherSpRecord escherSpRecord = (EscherSpRecord)shape.GetEscherContainer().GetChildById(-4086);
		if (shape.Anchor.IsHorizontallyFlipped)
		{
			escherSpRecord.Flags |= 64;
		}
		if (shape.Anchor.IsVerticallyFlipped)
		{
			escherSpRecord.Flags |= 128;
		}
	}

	public void AddShape(HSSFShape shape)
	{
		shape.Patriarch = this;
		_shapes.Add(shape);
	}

	private void OnCreate(HSSFShape shape)
	{
		EscherContainerRecord escherContainerRecord = _boundAggregate.GetEscherContainer().ChildContainers[0];
		EscherContainerRecord escherContainer = shape.GetEscherContainer();
		int shapeId = NewShapeId();
		shape.ShapeId = shapeId;
		escherContainerRecord.AddChildRecord(escherContainer);
		shape.AfterInsert(this);
		SetFlipFlags(shape);
	}

	public void SetCoordinates(int x1, int y1, int x2, int y2)
	{
		_spgrRecord.RectY1 = y1;
		_spgrRecord.RectY2 = y2;
		_spgrRecord.RectX1 = x1;
		_spgrRecord.RectX2 = x2;
	}

	public void Clear()
	{
		foreach (HSSFShape item in new List<HSSFShape>(_shapes))
		{
			RemoveShape(item);
		}
	}

	internal int NewShapeId()
	{
		DrawingManager2 drawingManager = ((HSSFWorkbook)_sheet.Workbook).Workbook.DrawingManager;
		EscherDgRecord escherDgRecord = (EscherDgRecord)_boundAggregate.GetEscherContainer().GetChildById(-4088);
		short drawingGroupId = escherDgRecord.DrawingGroupId;
		return drawingManager.AllocateShapeId(drawingGroupId, escherDgRecord);
	}

	public bool ContainsChart()
	{
		EscherOptRecord escherOptRecord = (EscherOptRecord)_boundAggregate.FindFirstWithId(-4085);
		if (escherOptRecord == null)
		{
			return false;
		}
		IEnumerator enumerator = escherOptRecord.EscherProperties.GetEnumerator();
		while (enumerator.MoveNext())
		{
			EscherProperty escherProperty = (EscherProperty)enumerator.Current;
			if (escherProperty.PropertyNumber == 896 && escherProperty.IsComplex && StringUtil.GetFromUnicodeLE(((EscherComplexProperty)escherProperty).ComplexData).Equals("Chart 1\0"))
			{
				return true;
			}
		}
		return false;
	}

	internal EscherAggregate GetBoundAggregate()
	{
		return _boundAggregate;
	}

	public IClientAnchor CreateAnchor(int dx1, int dy1, int dx2, int dy2, int col1, int row1, int col2, int row2)
	{
		return new HSSFClientAnchor(dx1, dy1, dx2, dy2, (short)col1, row1, (short)col2, row2);
	}

	public IChart CreateChart(IClientAnchor anchor)
	{
		throw new RuntimeException("NotImplemented");
	}

	public void BuildShapeTree()
	{
		EscherContainerRecord escherContainer = _boundAggregate.GetEscherContainer();
		if (escherContainer == null)
		{
			return;
		}
		IList<EscherContainerRecord> childContainers = escherContainer.ChildContainers[0].ChildContainers;
		for (int i = 0; i < childContainers.Count; i++)
		{
			EscherContainerRecord container = childContainers[i];
			if (i != 0)
			{
				HSSFShapeFactory.CreateShapeTree(container, _boundAggregate, this, ((HSSFWorkbook)_sheet.Workbook).RootDirectory);
			}
		}
	}

	public List<HSSFShape> GetShapes()
	{
		return _shapes;
	}

	public IEnumerator<HSSFShape> GetEnumerator()
	{
		return _shapes.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _shapes.GetEnumerator();
	}
}
