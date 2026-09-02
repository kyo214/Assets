using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Vml;
using NPOI.OpenXmlFormats.Vml.Office;
using NPOI.OpenXmlFormats.Vml.Spreadsheet;

namespace NPOI.XSSF.UserModel;

public class XSSFVMLDrawing : POIXMLDocumentPart
{
	private static XmlQualifiedName QNAME_SHAPE_LAYOUT = new XmlQualifiedName("shapelayout", "urn:schemas-microsoft-com:office:office");

	private static XmlQualifiedName QNAME_SHAPE_TYPE = new XmlQualifiedName("shapetype", "urn:schemas-microsoft-com:vml");

	private static XmlQualifiedName QNAME_SHAPE = new XmlQualifiedName("shape", "urn:schemas-microsoft-com:vml");

	private static string COMMENT_SHAPE_TYPE_ID = "_x0000_t202";

	private static Regex ptrn_shapeId = new Regex("_x0000_s(\\d+)", RegexOptions.Compiled);

	private static Regex ptrn_shapeTypeId = new Regex("_x0000_[tm](\\d+)", RegexOptions.Compiled);

	private ArrayList _items = new ArrayList();

	private string _shapeTypeId;

	private int _shapeId = 1024;

	public XSSFVMLDrawing()
	{
		newDrawing();
	}

	protected XSSFVMLDrawing(PackagePart part)
		: base(part)
	{
		Read(GetPackagePart().GetInputStream());
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	protected XSSFVMLDrawing(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	internal void Read(Stream is1)
	{
		XmlDocument xmlDocument = new XmlDocument();
		string text = new StreamReader(is1).ReadToEnd();
		xmlDocument.LoadXml(text.Replace("<br>", ""));
		XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
		xmlNamespaceManager.AddNamespace("o", "urn:schemas-microsoft-com:office:office");
		xmlNamespaceManager.AddNamespace("x", "urn:schemas-microsoft-com:office:excel");
		xmlNamespaceManager.AddNamespace("v", "urn:schemas-microsoft-com:vml");
		_items = new ArrayList();
		foreach (XmlNode item in xmlDocument.SelectNodes("/xml/*", xmlNamespaceManager))
		{
			_ = item.OuterXml;
			if (item.LocalName == QNAME_SHAPE_LAYOUT.Name)
			{
				CT_ShapeLayout value = CT_ShapeLayout.Parse(item, xmlNamespaceManager);
				_items.Add(value);
			}
			else if (item.LocalName == QNAME_SHAPE_TYPE.Name)
			{
				CT_Shapetype cT_Shapetype = CT_Shapetype.Parse(item, xmlNamespaceManager);
				_items.Add(cT_Shapetype);
				_shapeTypeId = cT_Shapetype.id;
			}
			else if (item.LocalName == QNAME_SHAPE.Name)
			{
				CT_Shape cT_Shape = CT_Shape.Parse(item, xmlNamespaceManager);
				string id = cT_Shape.id;
				if (id != null)
				{
					MatchCollection matchCollection = ptrn_shapeId.Matches(id);
					if (matchCollection.Count > 0)
					{
						_shapeId = Math.Max(_shapeId, int.Parse(matchCollection[0].Groups[1].Value));
					}
				}
				_items.Add(cT_Shape);
			}
			else
			{
				_items.Add(item);
			}
		}
	}

	internal ArrayList GetItems()
	{
		return _items;
	}

	internal void Write(Stream out1)
	{
		using StreamWriter streamWriter = new StreamWriter(out1);
		streamWriter.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
		streamWriter.Write("<xml");
		streamWriter.Write(" xmlns:v=\"urn:schemas-microsoft-com:vml\"");
		streamWriter.Write(" xmlns:o=\"urn:schemas-microsoft-com:office:office\"");
		streamWriter.Write(" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"");
		streamWriter.Write(" xmlns:w=\"urn:schemas-microsoft-com:office:word\"");
		streamWriter.Write(" xmlns:p=\"urn:schemas-microsoft-com:office:powerpoint\"");
		streamWriter.Write(">");
		for (int i = 0; i < _items.Count; i++)
		{
			object obj = _items[i];
			if (obj is XmlNode)
			{
				streamWriter.Write(((XmlNode)obj).OuterXml.Replace(" xmlns:v=\"urn:schemas-microsoft-com:vml\"", "").Replace(" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"", "").Replace(" xmlns:o=\"urn:schemas-microsoft-com:office:office\"", "")
					.Replace("&#xD;&#xA;", ""));
			}
			else if (obj is CT_Shapetype)
			{
				((CT_Shapetype)obj).Write(streamWriter, "shapetype");
			}
			else if (obj is CT_ShapeLayout)
			{
				((CT_ShapeLayout)obj).Write(streamWriter, "shapelayout");
			}
			else if (obj is CT_Shape)
			{
				((CT_Shape)obj).Write(streamWriter, "shape");
			}
			else
			{
				streamWriter.Write(obj.ToString().Replace(" xmlns:v=\"urn:schemas-microsoft-com:vml\"", "").Replace(" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"", "")
					.Replace(" xmlns:o=\"urn:schemas-microsoft-com:office:office\"", "")
					.Replace("&#xD;&#xA;", ""));
			}
		}
		streamWriter.Write("</xml>");
	}

	protected internal override void Commit()
	{
		Stream outputStream = GetPackagePart().GetOutputStream();
		Write(outputStream);
		outputStream.Close();
	}

	private void newDrawing()
	{
		CT_ShapeLayout cT_ShapeLayout = new CT_ShapeLayout();
		cT_ShapeLayout.ext = ST_Ext.edit;
		CT_IdMap cT_IdMap = cT_ShapeLayout.AddNewIdmap();
		cT_IdMap.ext = ST_Ext.edit;
		cT_IdMap.data = "1";
		_items.Add(cT_ShapeLayout);
		CT_Shapetype cT_Shapetype = new CT_Shapetype();
		_shapeTypeId = COMMENT_SHAPE_TYPE_ID;
		cT_Shapetype.id = _shapeTypeId;
		cT_Shapetype.coordsize = "21600,21600";
		cT_Shapetype.spt = 202f;
		cT_Shapetype.path2 = "m,l,21600r21600,l21600,xe";
		cT_Shapetype.AddNewStroke().joinstyle = ST_StrokeJoinStyle.miter;
		CT_Path cT_Path = cT_Shapetype.AddNewPath();
		cT_Path.gradientshapeok = NPOI.OpenXmlFormats.Vml.ST_TrueFalse.t;
		cT_Path.connecttype = ST_ConnectType.rect;
		_items.Add(cT_Shapetype);
	}

	internal CT_Shape newCommentShape()
	{
		CT_Shape cT_Shape = new CT_Shape();
		int num = ++_shapeId;
		cT_Shape.id = "_x0000_s" + num;
		cT_Shape.type = "#" + _shapeTypeId;
		cT_Shape.style = "position:absolute; visibility:hidden";
		cT_Shape.fillcolor = "#ffffe1";
		cT_Shape.insetmode = ST_InsetMode.auto;
		cT_Shape.AddNewFill().color = "#ffffe1";
		CT_Shadow cT_Shadow = cT_Shape.AddNewShadow();
		cT_Shadow.on = NPOI.OpenXmlFormats.Vml.ST_TrueFalse.t;
		cT_Shadow.color = "black";
		cT_Shadow.obscured = NPOI.OpenXmlFormats.Vml.ST_TrueFalse.t;
		cT_Shape.AddNewPath().connecttype = ST_ConnectType.none;
		cT_Shape.AddNewTextbox().style = "mso-direction-alt:auto";
		CT_ClientData cT_ClientData = cT_Shape.AddNewClientData();
		cT_ClientData.ObjectType = ST_ObjectType.Note;
		cT_ClientData.AddNewMoveWithCells();
		cT_ClientData.AddNewSizeWithCells();
		cT_ClientData.AddNewAnchor("1, 15, 0, 2, 3, 15, 3, 16");
		cT_ClientData.AddNewAutoFill(ST_TrueFalseBlank.@false);
		cT_ClientData.AddNewRow(0);
		cT_ClientData.AddNewColumn(0);
		_items.Add(cT_Shape);
		return cT_Shape;
	}

	internal CT_Shape FindCommentShape(int row, int col)
	{
		foreach (object item in _items)
		{
			if (!(item is CT_Shape))
			{
				continue;
			}
			CT_Shape cT_Shape = (CT_Shape)item;
			if (cT_Shape.sizeOfClientDataArray() <= 0)
			{
				continue;
			}
			CT_ClientData clientDataArray = cT_Shape.GetClientDataArray(0);
			if (clientDataArray.ObjectType == ST_ObjectType.Note)
			{
				int rowArray = clientDataArray.GetRowArray(0);
				int columnArray = clientDataArray.GetColumnArray(0);
				if (rowArray == row && columnArray == col)
				{
					return cT_Shape;
				}
			}
		}
		return null;
	}

	internal bool RemoveCommentShape(int row, int col)
	{
		CT_Shape cT_Shape = FindCommentShape(row, col);
		if (cT_Shape == null)
		{
			return false;
		}
		_items.Remove(cT_Shape);
		return true;
	}
}
