using System;
using NPOI.DDF;
using NPOI.HSSF.Record;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.HSSF.UserModel;

[Serializable]
public class HSSFComment : HSSFTextbox, IComment
{
	private const int FILL_TYPE_SOLID = 0;

	private const int FILL_TYPE_PICTURE = 3;

	private const int GROUP_SHAPE_PROPERTY_DEFAULT_VALUE = 655362;

	private const int GROUP_SHAPE_HIDDEN_MASK = 16777218;

	private const int GROUP_SHAPE_NOT_HIDDEN_MASK = -16777219;

	private NoteRecord _note;

	public override int ShapeId
	{
		get
		{
			return base.ShapeId;
		}
		set
		{
			if (value > 65535)
			{
				throw new ArgumentException("Cannot add more than 65535 shapes");
			}
			base.ShapeId = value;
			((CommonObjectDataSubRecord)GetObjRecord().SubRecords[0]).ObjectId = value;
			_note.ShapeId = value;
		}
	}

	public bool Visible
	{
		get
		{
			return _note.Flags == 2;
		}
		set
		{
			if (_note != null)
			{
				_note.Flags = (short)(value ? 2 : 0);
			}
			SetHidden(!value);
		}
	}

	public CellAddress Address
	{
		get
		{
			return new CellAddress(Row, Column);
		}
		set
		{
			Row = value.Row;
			Column = value.Column;
		}
	}

	public int Row
	{
		get
		{
			return _note.Row;
		}
		set
		{
			if (_note != null)
			{
				_note.Row = value;
			}
		}
	}

	public int Column
	{
		get
		{
			return _note.Column;
		}
		set
		{
			if (_note != null)
			{
				_note.Column = value;
			}
		}
	}

	public string Author
	{
		get
		{
			return _note.Author;
		}
		set
		{
			if (_note != null)
			{
				_note.Author = value;
			}
		}
	}

	internal NoteRecord NoteRecord => _note;

	public bool HasPosition
	{
		get
		{
			if (_note == null)
			{
				return false;
			}
			if (Column < 0 || Row < 0)
			{
				return false;
			}
			return true;
		}
	}

	public IClientAnchor ClientAnchor
	{
		get
		{
			HSSFAnchor hSSFAnchor = base.Anchor;
			if (hSSFAnchor is IClientAnchor)
			{
				return (IClientAnchor)hSSFAnchor;
			}
			throw new InvalidCastException("Anchor can not be changed in " + typeof(IClientAnchor).Name);
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

	public HSSFComment(EscherContainerRecord spContainer, ObjRecord objRecord, TextObjectRecord textObjectRecord, NoteRecord _note)
		: base(spContainer, objRecord, textObjectRecord)
	{
		this._note = _note;
	}

	public HSSFComment(HSSFShape parent, HSSFAnchor anchor)
		: this(parent, anchor, CreateNoteRecord())
	{
	}

	private HSSFComment(HSSFShape parent, HSSFAnchor anchor, NoteRecord note)
		: base(parent, anchor)
	{
		_note = note;
		base.FillColor = 134217808;
		Visible = false;
		Author = "";
		((CommonObjectDataSubRecord)GetObjRecord().SubRecords[0]).ObjectType = CommonObjectType.Comment;
	}

	public HSSFComment(NoteRecord note, TextObjectRecord txo)
		: this(null, new HSSFClientAnchor(), note)
	{
	}

	internal override void AfterInsert(HSSFPatriarch patriarch)
	{
		base.AfterInsert(patriarch);
		patriarch.GetBoundAggregate().AddTailRecord(NoteRecord);
	}

	protected override EscherContainerRecord CreateSpContainer()
	{
		EscherContainerRecord escherContainerRecord = base.CreateSpContainer();
		EscherOptRecord obj = (EscherOptRecord)escherContainerRecord.GetChildById(-4085);
		obj.RemoveEscherProperty(129);
		obj.RemoveEscherProperty(131);
		obj.RemoveEscherProperty(130);
		obj.RemoveEscherProperty(132);
		obj.SetEscherProperty(new EscherSimpleProperty(959, isComplex: false, isBlipId: false, 655362));
		return escherContainerRecord;
	}

	protected override ObjRecord CreateObjRecord()
	{
		ObjRecord objRecord = new ObjRecord();
		CommonObjectDataSubRecord o = new CommonObjectDataSubRecord
		{
			ObjectType = (CommonObjectType)202,
			IsLocked = true,
			IsPrintable = true,
			IsAutoFill = false,
			IsAutoline = true
		};
		NoteStructureSubRecord o2 = new NoteStructureSubRecord();
		EndSubRecord o3 = new EndSubRecord();
		objRecord.AddSubRecord(o);
		objRecord.AddSubRecord(o2);
		objRecord.AddSubRecord(o3);
		return objRecord;
	}

	private static NoteRecord CreateNoteRecord()
	{
		return new NoteRecord
		{
			Flags = 0,
			Author = ""
		};
	}

	public void SetAddress(int row, int col)
	{
		Row = row;
		Column = col;
	}

	internal override void AfterRemove(HSSFPatriarch patriarch)
	{
		base.AfterRemove(patriarch);
		patriarch.GetBoundAggregate().RemoveTailRecord(NoteRecord);
	}

	internal override HSSFShape CloneShape()
	{
		TextObjectRecord textObjectRecord = (TextObjectRecord)GetTextObjectRecord().CloneViaReserialise();
		EscherContainerRecord escherContainerRecord = new EscherContainerRecord();
		byte[] data = GetEscherContainer().Serialize();
		escherContainerRecord.FillFields(data, 0, new DefaultEscherRecordFactory());
		ObjRecord objRecord = (ObjRecord)GetObjRecord().CloneViaReserialise();
		NoteRecord note = (NoteRecord)NoteRecord.CloneViaReserialise();
		return new HSSFComment(escherContainerRecord, objRecord, textObjectRecord, note);
	}

	public void SetBackgroundImage(int pictureIndex)
	{
		SetPropertyValue(new EscherSimpleProperty(390, isComplex: false, isBlipId: true, pictureIndex));
		SetPropertyValue(new EscherSimpleProperty(384, isComplex: false, isBlipId: false, 3));
		((HSSFWorkbook)base.Patriarch.Sheet.Workbook).Workbook.GetBSERecord(pictureIndex).Ref++;
	}

	public void ResetBackgroundImage()
	{
		EscherSimpleProperty escherSimpleProperty = (EscherSimpleProperty)GetOptRecord().Lookup(390);
		if (escherSimpleProperty != null)
		{
			((HSSFWorkbook)base.Patriarch.Sheet.Workbook).Workbook.GetBSERecord(escherSimpleProperty.PropertyValue).Ref--;
			GetOptRecord().RemoveEscherProperty(390);
		}
		SetPropertyValue(new EscherSimpleProperty(384, isComplex: false, isBlipId: false, 0));
	}

	public int GetBackgroundImageId()
	{
		return ((EscherSimpleProperty)GetOptRecord().Lookup(390))?.PropertyValue ?? 0;
	}

	private void SetHidden(bool value)
	{
		EscherSimpleProperty escherSimpleProperty = (EscherSimpleProperty)GetOptRecord().Lookup(959);
		if (value)
		{
			SetPropertyValue(new EscherSimpleProperty(959, isComplex: false, isBlipId: false, escherSimpleProperty.PropertyValue | 0x1000002));
		}
		else
		{
			SetPropertyValue(new EscherSimpleProperty(959, isComplex: false, isBlipId: false, escherSimpleProperty.PropertyValue & -16777219));
		}
	}

	public override bool Equals(object obj)
	{
		if (!(obj is HSSFComment))
		{
			return false;
		}
		HSSFComment hSSFComment = (HSSFComment)obj;
		return NoteRecord.Equals(hSSFComment.NoteRecord);
	}

	public override int GetHashCode()
	{
		return (Row * 17 + Column) * 31;
	}
}
