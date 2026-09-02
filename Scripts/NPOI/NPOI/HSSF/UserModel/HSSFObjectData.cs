using System;
using System.Collections;
using System.IO;
using NPOI.DDF;
using NPOI.HSSF.Record;
using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI.HSSF.UserModel;

public class HSSFObjectData : HSSFPicture
{
	private DirectoryEntry _root;

	public string OLE2ClassName => FindObjectRecord().OLEClassName;

	public HSSFObjectData(EscherContainerRecord spContainer, ObjRecord objRecord, DirectoryEntry _root)
		: base(spContainer, objRecord)
	{
		this._root = _root;
	}

	public DirectoryEntry GetDirectory()
	{
		string text = "MBD" + HexDump.ToHex(FindObjectRecord().StreamId.Value);
		Entry entry = _root.GetEntry(text);
		if (entry is DirectoryEntry)
		{
			return (DirectoryEntry)entry;
		}
		throw new IOException("Stream " + text + " was not an OLE2 directory");
	}

	public byte[] GetObjectData()
	{
		return FindObjectRecord().ObjectData;
	}

	public bool HasDirectoryEntry()
	{
		int? streamId = FindObjectRecord().StreamId;
		if (streamId.HasValue)
		{
			return streamId != 0;
		}
		return false;
	}

	public EmbeddedObjectRefSubRecord FindObjectRecord()
	{
		IEnumerator enumerator = GetObjRecord().SubRecords.GetEnumerator();
		while (enumerator.MoveNext())
		{
			object current = enumerator.Current;
			if (current is EmbeddedObjectRefSubRecord)
			{
				return (EmbeddedObjectRefSubRecord)current;
			}
		}
		throw new InvalidOperationException("Object data does not contain a reference to an embedded object OLE2 directory");
	}

	protected override EscherContainerRecord CreateSpContainer()
	{
		throw new InvalidOperationException("HSSFObjectData cannot be created from scratch");
	}

	protected override ObjRecord CreateObjRecord()
	{
		throw new InvalidOperationException("HSSFObjectData cannot be created from scratch");
	}

	internal override void AfterRemove(HSSFPatriarch patriarch)
	{
		throw new InvalidOperationException("HSSFObjectData cannot be created from scratch");
	}

	internal override void AfterInsert(HSSFPatriarch patriarch)
	{
		patriarch.GetBoundAggregate().AssociateShapeToObjRecord(GetEscherContainer().GetChildById(-4079), GetObjRecord());
		((HSSFWorkbook)patriarch.Sheet.Workbook).Workbook.GetBSERecord(base.PictureIndex).Ref++;
	}

	internal override HSSFShape CloneShape()
	{
		EscherContainerRecord escherContainerRecord = new EscherContainerRecord();
		byte[] data = GetEscherContainer().Serialize();
		escherContainerRecord.FillFields(data, 0, new DefaultEscherRecordFactory());
		ObjRecord objRecord = (ObjRecord)GetObjRecord().CloneViaReserialise();
		return new HSSFObjectData(escherContainerRecord, objRecord, _root);
	}
}
