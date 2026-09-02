using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using NPOI.OpenXml4Net.Exceptions;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS;
using NPOI.SS.Formula;
using NPOI.SS.Formula.UDF;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;
using NPOI.XSSF.Model;
using NPOI.XSSF.UserModel.Helpers;

namespace NPOI.XSSF.UserModel;

public class XSSFWorkbook : POIXMLDocument, IWorkbook, ICloseable
{
	private static Regex COMMA_PATTERN = new Regex(",", RegexOptions.Compiled);

	public static float DEFAULT_CHARACTER_WIDTH = 7.0017f;

	private static int Max_SENSITIVE_SHEET_NAME_LEN = 31;

	public static int PICTURE_TYPE_EMF = 2;

	public static int PICTURE_TYPE_WMF = 3;

	public static int PICTURE_TYPE_PICT = 4;

	public static int PICTURE_TYPE_JPEG = 5;

	public static int PICTURE_TYPE_PNG = 6;

	public static int PICTURE_TYPE_DIB = 7;

	public static int PICTURE_TYPE_GIF = 8;

	public static int PICTURE_TYPE_TIFF = 9;

	public static int PICTURE_TYPE_EPS = 10;

	public static int PICTURE_TYPE_BMP = 11;

	public static int PICTURE_TYPE_WPG = 12;

	public static int PICTURE_TYPE_JPG = 13;

	private CT_Workbook workbook;

	private List<XSSFSheet> sheets;

	private Dictionary<string, List<XSSFName>> namedRangesByName;

	private List<XSSFName> namedRanges;

	private SharedStringsTable sharedStringSource;

	private StylesTable stylesSource;

	private IndexedUDFFinder _udfFinder = new IndexedUDFFinder(UDFFinder.GetDefault());

	private CalculationChain calcChain;

	private List<ExternalLinksTable> externalLinks;

	private MapInfo mapInfo;

	private XSSFDataFormat formatter;

	private MissingCellPolicy _missingCellPolicy = MissingCellPolicy.RETURN_NULL_AND_BLANK;

	private List<XSSFPictureData> pictures;

	private static POILogger logger = POILogFactory.GetLogger(typeof(XSSFWorkbook));

	private XSSFCreationHelper _creationHelper;

	private List<XSSFPivotTable> pivotTables;

	private List<CT_PivotCache> pivotCaches;

	private WorkbookDocument doc;

	public int ActiveSheetIndex => (int)workbook.bookViews.GetWorkbookViewArray(0).activeTab;

	public int NumCellStyles => stylesSource.NumCellStyles;

	public short NumberOfFonts => (short)stylesSource.GetFonts().Count;

	public int NumberOfNames => namedRanges.Count;

	public int NumberOfSheets => sheets.Count;

	public MissingCellPolicy MissingCellPolicy
	{
		get
		{
			return _missingCellPolicy;
		}
		set
		{
			_missingCellPolicy = value;
		}
	}

	public int FirstVisibleTab
	{
		get
		{
			return (int)workbook.bookViews.GetWorkbookViewArray(0).firstSheet;
		}
		set
		{
			workbook.bookViews.GetWorkbookViewArray(0).firstSheet = (uint)value;
		}
	}

	public bool IsHidden
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

	public List<ExternalLinksTable> ExternalLinksTable => externalLinks;

	public SpreadsheetVersion SpreadsheetVersion => SpreadsheetVersion.EXCEL2007;

	public List<XSSFPivotTable> PivotTables
	{
		get
		{
			return pivotTables;
		}
		set
		{
			pivotTables = value;
		}
	}

	public XSSFWorkbookType WorkbookType
	{
		get
		{
			if (!IsMacroEnabled())
			{
				return XSSFWorkbookType.XLSX;
			}
			return XSSFWorkbookType.XLSM;
		}
		set
		{
			try
			{
				GetPackagePart().ContentType = value.ContentType;
			}
			catch (InvalidFormatException ex)
			{
				throw new POIXMLException(ex);
			}
		}
	}

	public ISheet this[int index]
	{
		get
		{
			return GetSheetAt(index);
		}
		set
		{
			if (sheets[index] != null)
			{
				sheets[index] = (XSSFSheet)value;
			}
			else
			{
				sheets.Insert(index, (XSSFSheet)value);
			}
		}
	}

	public int Count => NumberOfSheets;

	public bool IsReadOnly => false;

	public XSSFWorkbook()
		: this(XSSFWorkbookType.XLSX)
	{
	}

	public XSSFWorkbook(XSSFWorkbookType workbookType)
		: base(newPackage(workbookType))
	{
		OnWorkbookCreate();
	}

	public XSSFWorkbook(OPCPackage pkg)
		: base(pkg)
	{
		BeforeDocumentRead();
		Load(XSSFFactory.GetInstance());
		if (!workbook.IsSetBookViews())
		{
			workbook.AddNewBookViews().AddNewWorkbookView().activeTab = 0u;
		}
	}

	public XSSFWorkbook(Stream is1)
		: base(PackageHelper.Open(is1))
	{
		BeforeDocumentRead();
		Load(XSSFFactory.GetInstance());
		if (!workbook.IsSetBookViews())
		{
			workbook.AddNewBookViews().AddNewWorkbookView().activeTab = 0u;
		}
	}

	public XSSFWorkbook(FileInfo file)
		: this(OPCPackage.Open(file))
	{
	}

	public XSSFWorkbook(string path)
		: this(POIXMLDocument.OpenPackage(path))
	{
	}

	protected void BeforeDocumentRead()
	{
		if (base.CorePart.ContentType.Equals(XSSFRelation.XLSB_BINARY_WORKBOOK.ContentType))
		{
			throw new XLSBUnsupportedException();
		}
		pivotTables = new List<XSSFPivotTable>();
		pivotCaches = new List<CT_PivotCache>();
	}

	internal override void OnDocumentRead()
	{
		try
		{
			XmlDocument xmlDoc = POIXMLDocumentPart.ConvertStreamToXml(GetPackagePart().GetInputStream());
			doc = WorkbookDocument.Parse(xmlDoc, POIXMLDocumentPart.NamespaceManager);
			workbook = doc.Workbook;
			ThemesTable theme = null;
			Dictionary<string, XSSFSheet> dictionary = new Dictionary<string, XSSFSheet>();
			Dictionary<string, ExternalLinksTable> dictionary2 = new Dictionary<string, ExternalLinksTable>();
			foreach (RelationPart relationPart in base.RelationParts)
			{
				POIXMLDocumentPart documentPart = relationPart.DocumentPart;
				if (documentPart is SharedStringsTable)
				{
					sharedStringSource = (SharedStringsTable)documentPart;
				}
				else if (documentPart is StylesTable)
				{
					stylesSource = (StylesTable)documentPart;
				}
				else if (documentPart is ThemesTable)
				{
					theme = (ThemesTable)documentPart;
				}
				else if (documentPart is CalculationChain)
				{
					calcChain = (CalculationChain)documentPart;
				}
				else if (documentPart is MapInfo)
				{
					mapInfo = (MapInfo)documentPart;
				}
				else if (documentPart is XSSFSheet)
				{
					dictionary[relationPart.Relationship.Id] = (XSSFSheet)documentPart;
				}
				else if (documentPart is ExternalLinksTable)
				{
					dictionary2[relationPart.Relationship.Id] = (ExternalLinksTable)documentPart;
				}
			}
			bool flag = base.Package.GetPackageAccess() == PackageAccess.READ;
			if (stylesSource == null)
			{
				if (flag)
				{
					stylesSource = new StylesTable();
				}
				else
				{
					stylesSource = (StylesTable)CreateRelationship(XSSFRelation.STYLES, XSSFFactory.GetInstance());
				}
			}
			stylesSource.SetWorkbook(this);
			stylesSource.SetTheme(theme);
			if (sharedStringSource == null)
			{
				if (flag)
				{
					sharedStringSource = new SharedStringsTable();
				}
				else
				{
					sharedStringSource = (SharedStringsTable)CreateRelationship(XSSFRelation.SHARED_STRINGS, XSSFFactory.GetInstance());
				}
			}
			sheets = new List<XSSFSheet>(dictionary.Count);
			foreach (CT_Sheet item in workbook.sheets.sheet)
			{
				ParseSheet(dictionary, item);
			}
			externalLinks = new List<ExternalLinksTable>(dictionary2.Count);
			if (workbook.IsSetExternalReferences())
			{
				foreach (CT_ExternalReference item2 in workbook.externalReferences.externalReference)
				{
					ExternalLinksTable externalLinksTable = null;
					if (dictionary2.ContainsKey(item2.id))
					{
						externalLinksTable = dictionary2[item2.id];
					}
					if (externalLinksTable == null)
					{
						logger.Log(5, "ExternalLinksTable with r:id " + item2.id + " was defined, but didn't exist in package, skipping");
					}
					else
					{
						externalLinks.Add(externalLinksTable);
					}
				}
			}
			ReprocessNamedRanges();
		}
		catch (XmlException ex)
		{
			throw new POIXMLException(ex);
		}
	}

	private void ParseSheet(Dictionary<string, XSSFSheet> shIdMap, CT_Sheet ctSheet)
	{
		XSSFSheet xSSFSheet = null;
		if (shIdMap.ContainsKey(ctSheet.id))
		{
			xSSFSheet = shIdMap[ctSheet.id];
		}
		if (xSSFSheet == null)
		{
			logger.Log(5, "Sheet with name " + ctSheet.name + " and r:id " + ctSheet.id + " was defined, but didn't exist in package, skipping");
		}
		else
		{
			xSSFSheet.sheet = ctSheet;
			xSSFSheet.OnDocumentRead();
			sheets.Add(xSSFSheet);
		}
	}

	private void OnWorkbookCreate()
	{
		doc = new WorkbookDocument();
		workbook = doc.Workbook;
		workbook.AddNewWorkbookPr().date1904 = false;
		workbook.AddNewBookViews().AddNewWorkbookView().activeTab = 0u;
		workbook.AddNewSheets();
		CT_ExtendedProperties underlyingProperties = GetProperties().ExtendedProperties.GetUnderlyingProperties();
		underlyingProperties.Application = POIXMLDocument.DOCUMENT_CREATOR;
		underlyingProperties.DocSecurity = 0;
		underlyingProperties.DocSecuritySpecified = true;
		underlyingProperties.ScaleCrop = false;
		underlyingProperties.ScaleCropSpecified = true;
		underlyingProperties.LinksUpToDate = false;
		underlyingProperties.LinksUpToDateSpecified = true;
		underlyingProperties.HyperlinksChanged = false;
		underlyingProperties.HyperlinksChangedSpecified = true;
		underlyingProperties.SharedDoc = false;
		underlyingProperties.SharedDocSpecified = true;
		sharedStringSource = (SharedStringsTable)CreateRelationship(XSSFRelation.SHARED_STRINGS, XSSFFactory.GetInstance());
		stylesSource = (StylesTable)CreateRelationship(XSSFRelation.STYLES, XSSFFactory.GetInstance());
		stylesSource.SetWorkbook(this);
		namedRanges = new List<XSSFName>();
		namedRangesByName = new Dictionary<string, List<XSSFName>>();
		sheets = new List<XSSFSheet>();
		pivotTables = new List<XSSFPivotTable>();
	}

	protected static OPCPackage newPackage(XSSFWorkbookType workbookType)
	{
		try
		{
			OPCPackage oPCPackage = OPCPackage.Create(new MemoryStream());
			PackagePartName packagePartName = PackagingUriHelper.CreatePartName(XSSFRelation.WORKBOOK.DefaultFileName);
			oPCPackage.AddRelationship(packagePartName, TargetMode.Internal, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument");
			oPCPackage.CreatePart(packagePartName, workbookType.ContentType);
			oPCPackage.GetPackageProperties().SetCreatorProperty(POIXMLDocument.DOCUMENT_CREATOR);
			return oPCPackage;
		}
		catch (Exception ex)
		{
			throw new POIXMLException(ex);
		}
	}

	public CT_Workbook GetCTWorkbook()
	{
		return workbook;
	}

	public int AddPicture(byte[] pictureData, int format)
	{
		return AddPicture(pictureData, (PictureType)format);
	}

	public int AddPicture(Stream picStream, int format)
	{
		int num = GetAllPictures().Count + 1;
		XSSFPictureData xSSFPictureData = (XSSFPictureData)CreateRelationship(XSSFPictureData.RELATIONS[format], XSSFFactory.GetInstance(), num, noRelation: true).DocumentPart;
		Stream outputStream = xSSFPictureData.GetPackagePart().GetOutputStream();
		IOUtils.Copy(picStream, outputStream);
		outputStream.Close();
		pictures.Add(xSSFPictureData);
		return num - 1;
	}

	public ISheet CloneSheet(int sheetNum)
	{
		return CloneSheet(sheetNum, null);
	}

	public ISheet CloneSheet(int sheetNum, string newName)
	{
		ValidateSheetIndex(sheetNum);
		XSSFSheet xSSFSheet = sheets[sheetNum];
		if (newName == null)
		{
			string sheetName = xSSFSheet.SheetName;
			newName = GetUniqueSheetName(sheetName);
		}
		else
		{
			ValidateSheetName(newName);
			WorkbookUtil.ValidateSheetName(newName);
		}
		XSSFSheet xSSFSheet2 = CreateSheet(newName) as XSSFSheet;
		List<RelationPart> relationParts = xSSFSheet.RelationParts;
		XSSFDrawing xSSFDrawing = null;
		foreach (RelationPart item in relationParts)
		{
			POIXMLDocumentPart documentPart = item.DocumentPart;
			if (documentPart is XSSFDrawing)
			{
				xSSFDrawing = (XSSFDrawing)documentPart;
			}
			else
			{
				AddRelation(item, xSSFSheet2);
			}
		}
		try
		{
			foreach (PackageRelationship relationship in xSSFSheet.GetPackagePart().Relationships)
			{
				if (relationship.TargetMode == TargetMode.External)
				{
					xSSFSheet2.GetPackagePart().AddExternalRelationship(relationship.TargetUri.ToString(), relationship.RelationshipType, null);
				}
			}
		}
		catch (InvalidFormatException ex)
		{
			throw new POIXMLException("Failed to clone sheet", ex);
		}
		try
		{
			using MemoryStream memoryStream = new MemoryStream();
			xSSFSheet.Write(memoryStream);
			xSSFSheet2.Read(new MemoryStream(memoryStream.ToArray()));
		}
		catch (IOException ex2)
		{
			throw new POIXMLException("Failed to clone sheet", ex2);
		}
		CT_Worksheet cTWorksheet = xSSFSheet2.GetCTWorksheet();
		if (cTWorksheet.IsSetLegacyDrawing())
		{
			cTWorksheet.UnsetLegacyDrawing();
		}
		if (cTWorksheet.IsSetPageSetup())
		{
			cTWorksheet.UnsetPageSetup();
		}
		xSSFSheet2.IsSelected = false;
		if (xSSFDrawing != null)
		{
			if (cTWorksheet.IsSetDrawing())
			{
				cTWorksheet.UnsetDrawing();
			}
			XSSFDrawing xSSFDrawing2 = xSSFSheet2.CreateDrawingPatriarch() as XSSFDrawing;
			xSSFDrawing2.GetCTDrawing().Set(xSSFDrawing.GetCTDrawing());
			xSSFDrawing2 = xSSFSheet2.CreateDrawingPatriarch() as XSSFDrawing;
			foreach (RelationPart relationPart in (xSSFSheet.CreateDrawingPatriarch() as XSSFDrawing).RelationParts)
			{
				AddRelation(relationPart, xSSFDrawing2);
			}
		}
		return xSSFSheet2;
	}

	private static void AddRelation(RelationPart rp, POIXMLDocumentPart target)
	{
		PackageRelationship relationship = rp.Relationship;
		if (relationship.TargetMode == TargetMode.External)
		{
			target.GetPackagePart().AddRelationship(relationship.TargetUri, relationship.TargetMode.Value, relationship.RelationshipType, relationship.Id);
			return;
		}
		XSSFRelation instance = XSSFRelation.GetInstance(relationship.RelationshipType);
		if (instance == null)
		{
			throw new POIXMLException("Can't clone sheet - unknown relation type found: " + relationship.RelationshipType);
		}
		target.AddRelation(relationship.Id, instance, rp.DocumentPart);
	}

	private string GetUniqueSheetName(string srcName)
	{
		int num = 2;
		string text = srcName;
		int num2 = srcName.LastIndexOf('(');
		if (num2 > 0 && srcName.EndsWith(")"))
		{
			string text2 = srcName.Substring(num2 + 1, srcName.Length - ")".Length - num2 - 1);
			try
			{
				num = int.Parse(text2.Trim());
				num++;
				text = srcName.Substring(0, num2).Trim();
			}
			catch (FormatException)
			{
			}
		}
		string text4;
		do
		{
			string text3 = num++.ToString();
			text4 = ((text.Length + text3.Length + 2 >= 31) ? (text.Substring(0, 31 - text3.Length - 2) + "(" + text3 + ")") : (text + " (" + text3 + ")"));
		}
		while (GetSheetIndex(text4) != -1);
		return text4;
	}

	public ICellStyle CreateCellStyle()
	{
		return stylesSource.CreateCellStyle();
	}

	public IDataFormat CreateDataFormat()
	{
		if (formatter == null)
		{
			formatter = new XSSFDataFormat(stylesSource);
		}
		return formatter;
	}

	public IFont CreateFont()
	{
		XSSFFont xSSFFont = new XSSFFont();
		xSSFFont.RegisterTo(stylesSource);
		return xSSFFont;
	}

	public IName CreateName()
	{
		CT_DefinedName cT_DefinedName = new CT_DefinedName();
		cT_DefinedName.name = "";
		return CreateAndStoreName(cT_DefinedName);
	}

	private void PutValuesMapping(string key, XSSFName name)
	{
		if (namedRangesByName.ContainsKey(key))
		{
			namedRangesByName[key].Add(name);
			return;
		}
		namedRangesByName.Add(key, new List<XSSFName> { name });
	}

	private XSSFName CreateAndStoreName(CT_DefinedName ctName)
	{
		XSSFName xSSFName = new XSSFName(ctName, this);
		namedRanges.Add(xSSFName);
		PutValuesMapping(ctName.name.ToLower(), xSSFName);
		return xSSFName;
	}

	public ISheet CreateSheet()
	{
		string text = "Sheet" + sheets.Count;
		int num = 0;
		while (GetSheet(text) != null)
		{
			text = "Sheet" + num;
			num++;
		}
		return CreateSheet(text);
	}

	public ISheet CreateSheet(string sheetname)
	{
		if (sheetname == null)
		{
			throw new ArgumentException("sheetName must not be null");
		}
		ValidateSheetName(sheetname);
		WorkbookUtil.ValidateSheetName(sheetname);
		CT_Sheet cT_Sheet = AddSheet(sheetname);
		int num = 1;
		foreach (XSSFSheet sheet in sheets)
		{
			num = (int)Math.Max(sheet.sheet.sheetId + 1, num);
		}
		while (true)
		{
			IL_006a:
			foreach (XSSFSheet sheet2 in sheets)
			{
				num = (int)Math.Max(sheet2.sheet.sheetId + 1, num);
			}
			string fileName = XSSFRelation.WORKSHEET.GetFileName(num);
			foreach (POIXMLDocumentPart relation in GetRelations())
			{
				if (relation.GetPackagePart() != null && fileName.Equals(relation.GetPackagePart().PartName.Name))
				{
					num++;
					goto IL_006a;
				}
			}
			break;
		}
		RelationPart relationPart = CreateRelationship(XSSFRelation.WORKSHEET, XSSFFactory.GetInstance(), num, noRelation: false);
		XSSFSheet xSSFSheet = relationPart.DocumentPart as XSSFSheet;
		xSSFSheet.sheet = cT_Sheet;
		cT_Sheet.id = relationPart.Relationship.Id;
		cT_Sheet.sheetId = (uint)num;
		if (sheets.Count == 0)
		{
			xSSFSheet.IsSelected = true;
		}
		sheets.Add(xSSFSheet);
		return xSSFSheet;
	}

	private void ValidateSheetName(string sheetName)
	{
		if (ContainsSheet(sheetName, sheets.Count))
		{
			throw new ArgumentException($"The workbook already contains a sheet named '{sheetName}'");
		}
	}

	protected XSSFDialogsheet CreateDialogsheet(string sheetname, CT_Dialogsheet dialogsheet)
	{
		XSSFSheet xSSFSheet = CreateSheet(sheetname) as XSSFSheet;
		string relationId = GetRelationId(xSSFSheet);
		PackageRelationship relationship = GetPackagePart().GetRelationship(relationId);
		return new XSSFDialogsheet(xSSFSheet, relationship);
	}

	private CT_Sheet AddSheet(string sheetname)
	{
		CT_Sheet cT_Sheet = workbook.sheets.AddNewSheet();
		cT_Sheet.name = sheetname;
		return cT_Sheet;
	}

	[Obsolete("deprecated POI 3.15. Use {@link #findFont(boolean, short, short, String, boolean, boolean, short, byte)} instead.")]
	public IFont FindFont(short boldWeight, short color, short fontHeight, string name, bool italic, bool strikeout, FontSuperScript typeOffset, FontUnderlineType underline)
	{
		return stylesSource.FindFont(boldWeight, color, fontHeight, name, italic, strikeout, typeOffset, underline);
	}

	public IFont FindFont(bool bold, short color, short fontHeight, string name, bool italic, bool strikeout, FontSuperScript typeOffset, FontUnderlineType underline)
	{
		return stylesSource.FindFont(bold, color, fontHeight, name, italic, strikeout, typeOffset, underline);
	}

	public IList GetAllPictures()
	{
		if (pictures == null)
		{
			List<PackagePart> partsByName = base.Package.GetPartsByName(new Regex("/xl/media/.*?"));
			pictures = new List<XSSFPictureData>(partsByName.Count);
			foreach (PackagePart item in partsByName)
			{
				pictures.Add(new XSSFPictureData(item));
			}
		}
		return pictures;
	}

	public ICellStyle GetCellStyleAt(int idx)
	{
		return stylesSource.GetStyleAt(idx);
	}

	public IFont GetFontAt(short idx)
	{
		return stylesSource.GetFontAt(idx);
	}

	public IName GetName(string name)
	{
		IList<IName> names = GetNames(name);
		if (names.Count == 0)
		{
			return null;
		}
		return names[0];
	}

	public IList<IName> GetNames(string name)
	{
		List<IName> list = new List<IName>();
		if (namedRangesByName.ContainsKey(name.ToLower()))
		{
			list.AddRange(namedRangesByName[name.ToLower()]);
		}
		return list.AsReadOnly();
	}

	[Obsolete("deprecated 3.16. New projects should avoid accessing named ranges by index.")]
	public IName GetNameAt(int nameIndex)
	{
		int count = namedRanges.Count;
		if (count < 1)
		{
			throw new InvalidOperationException("There are no defined names in this workbook");
		}
		if (nameIndex < 0 || nameIndex > count)
		{
			throw new ArgumentException("Specified name index " + nameIndex + " is outside the allowable range (0.." + (count - 1) + ").");
		}
		return namedRanges[nameIndex];
	}

	public IList<IName> GetAllNames()
	{
		List<IName> list = new List<IName>();
		list.AddRange(namedRanges);
		return list.AsReadOnly();
	}

	[Obsolete("deprecated 3.16. New projects should avoid accessing named ranges by index. Use {@link #getName(String)} instead.")]
	public int GetNameIndex(string name)
	{
		if (GetName(name) is XSSFName item)
		{
			return namedRanges.IndexOf(item);
		}
		return -1;
	}

	public string GetPrintArea(int sheetIndex)
	{
		return GetBuiltInName(XSSFName.BUILTIN_PRINT_AREA, sheetIndex)?.RefersToFormula;
	}

	public ISheet GetSheet(string name)
	{
		foreach (XSSFSheet sheet in sheets)
		{
			if (name.Equals(sheet.SheetName, StringComparison.InvariantCultureIgnoreCase))
			{
				return sheet;
			}
		}
		return null;
	}

	public ISheet GetSheetAt(int index)
	{
		ValidateSheetIndex(index);
		return sheets[index];
	}

	public int GetSheetIndex(string name)
	{
		int num = 0;
		foreach (XSSFSheet sheet in sheets)
		{
			if (name.Equals(sheet.SheetName, StringComparison.InvariantCultureIgnoreCase))
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public int GetSheetIndex(ISheet sheet)
	{
		int num = 0;
		foreach (XSSFSheet sheet2 in sheets)
		{
			if (sheet2 == sheet)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public string GetSheetName(int sheetIx)
	{
		ValidateSheetIndex(sheetIx);
		return sheets[sheetIx].SheetName;
	}

	public IEnumerator<ISheet> GetEnumerator()
	{
		return sheets.GetEnumerator();
	}

	public bool IsMacroEnabled()
	{
		return GetPackagePart().ContentType.Equals(XSSFRelation.MACROS_WORKBOOK.ContentType);
	}

	[Obsolete("deprecated 3.16. New projects should use {@link #removeName(Name)}.")]
	public void RemoveName(int nameIndex)
	{
		RemoveName(GetNameAt(nameIndex));
	}

	public void RemoveName(string name)
	{
		List<XSSFName> list = namedRangesByName[name.ToLower()];
		if (list.Count == 0)
		{
			throw new ArgumentException("Named range was not found: " + name);
		}
		RemoveName(list[0]);
	}

	private bool RemoveMapping(string key, XSSFName item)
	{
		if (namedRangesByName.ContainsKey(key))
		{
			return namedRangesByName[key].Remove(item);
		}
		return false;
	}

	public void RemoveName(IName name)
	{
		if (!RemoveMapping(name.NameName.ToLower(), name as XSSFName))
		{
			throw new ArgumentException("Name was not found: " + name);
		}
		if (!namedRanges.Remove((XSSFName)name))
		{
			throw new ArgumentException("Name was not found: " + name);
		}
	}

	internal void UpdateName(XSSFName name, string oldName)
	{
		if (!RemoveMapping(oldName.ToLower(), name))
		{
			throw new ArgumentException("Name was not found: " + name);
		}
		PutValuesMapping(name.NameName.ToLower(), name);
	}

	public void RemovePrintArea(int sheetIndex)
	{
		XSSFName builtInName = GetBuiltInName(XSSFName.BUILTIN_PRINT_AREA, sheetIndex);
		if (builtInName != null)
		{
			RemoveName(builtInName);
		}
	}

	public void RemoveSheetAt(int index)
	{
		ValidateSheetIndex(index);
		OnSheetDelete(index);
		XSSFSheet part = (XSSFSheet)GetSheetAt(index);
		RemoveRelation(part);
		sheets.RemoveAt(index);
		if (sheets.Count != 0)
		{
			int num = index;
			if (num >= sheets.Count)
			{
				num = sheets.Count - 1;
			}
			int activeSheetIndex = ActiveSheetIndex;
			if (activeSheetIndex == index)
			{
				SetActiveSheet(num);
			}
			else if (activeSheetIndex > index)
			{
				SetActiveSheet(activeSheetIndex - 1);
			}
		}
	}

	private void OnSheetDelete(int index)
	{
		workbook.sheets.RemoveSheet(index);
		if (calcChain != null)
		{
			RemoveRelation(calcChain);
			calcChain = null;
		}
		List<XSSFName> list = new List<XSSFName>();
		foreach (XSSFName namedRange in namedRanges)
		{
			CT_DefinedName cTName = namedRange.GetCTName();
			if (cTName.IsSetLocalSheetId())
			{
				if (cTName.localSheetId == index)
				{
					list.Add(namedRange);
				}
				else if (cTName.localSheetId > index)
				{
					cTName.localSheetId--;
					cTName.localSheetIdSpecified = true;
				}
			}
		}
		foreach (XSSFName item in list)
		{
			RemoveName(item);
		}
	}

	private void ValidateSheetIndex(int index)
	{
		int num = sheets.Count - 1;
		if (index < 0 || index > num)
		{
			string text = "(0.." + num + ")";
			if (num == -1)
			{
				text = "(no sheets)";
			}
			throw new ArgumentException("Sheet index (" + index + ") is out of range " + text);
		}
	}

	public void SetPrintArea(int sheetIndex, string reference)
	{
		XSSFName xSSFName = GetBuiltInName(XSSFName.BUILTIN_PRINT_AREA, sheetIndex);
		if (xSSFName == null)
		{
			xSSFName = CreateBuiltInName(XSSFName.BUILTIN_PRINT_AREA, sheetIndex);
		}
		string[] array = COMMA_PATTERN.Split(reference);
		StringBuilder stringBuilder = new StringBuilder(32);
		for (int i = 0; i < array.Length; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append(",");
			}
			SheetNameFormatter.AppendFormat(stringBuilder, GetSheetName(sheetIndex));
			stringBuilder.Append("!");
			stringBuilder.Append(array[i]);
		}
		xSSFName.RefersToFormula = stringBuilder.ToString();
	}

	public void SetPrintArea(int sheetIndex, int startColumn, int endColumn, int startRow, int endRow)
	{
		string referencePrintArea = GetReferencePrintArea(GetSheetName(sheetIndex), startColumn, endColumn, startRow, endRow);
		SetPrintArea(sheetIndex, referencePrintArea);
	}

	private static string GetReferenceBuiltInRecord(string sheetName, int startC, int endC, int startR, int endR)
	{
		CellReference cellReference = new CellReference(sheetName, 0, startC, pAbsRow: true, pAbsCol: true);
		CellReference cellReference2 = new CellReference(sheetName, 0, endC, pAbsRow: true, pAbsCol: true);
		string text = SheetNameFormatter.Format(sheetName);
		string value = ((startC != -1 || endC != -1) ? (text + "!$" + cellReference.CellRefParts[2] + ":$" + cellReference2.CellRefParts[2]) : "");
		CellReference cellReference3 = new CellReference(sheetName, startR, 0, pAbsRow: true, pAbsCol: true);
		CellReference cellReference4 = new CellReference(sheetName, endR, 0, pAbsRow: true, pAbsCol: true);
		string text2 = "";
		if (startR == -1 && endR == -1)
		{
			text2 = "";
		}
		else if (!cellReference3.CellRefParts[1].Equals("0") && !cellReference4.CellRefParts[1].Equals("0"))
		{
			text2 = text + "!$" + cellReference3.CellRefParts[1] + ":$" + cellReference4.CellRefParts[1];
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(value);
		if (stringBuilder.Length > 0 && text2.Length > 0)
		{
			stringBuilder.Append(',');
		}
		stringBuilder.Append(text2);
		return stringBuilder.ToString();
	}

	private static string GetReferencePrintArea(string sheetName, int startC, int endC, int startR, int endR)
	{
		CellReference cellReference = new CellReference(sheetName, startR, startC, pAbsRow: true, pAbsCol: true);
		CellReference cellReference2 = new CellReference(sheetName, endR, endC, pAbsRow: true, pAbsCol: true);
		return "$" + cellReference.CellRefParts[2] + "$" + cellReference.CellRefParts[1] + ":$" + cellReference2.CellRefParts[2] + "$" + cellReference2.CellRefParts[1];
	}

	public XSSFName GetBuiltInName(string builtInCode, int sheetNumber)
	{
		if (!namedRangesByName.ContainsKey(builtInCode.ToLower()))
		{
			return null;
		}
		foreach (XSSFName item in namedRangesByName[builtInCode.ToLower()])
		{
			if (item.SheetIndex == sheetNumber)
			{
				return item;
			}
		}
		return null;
	}

	internal XSSFName CreateBuiltInName(string builtInName, int sheetNumber)
	{
		ValidateSheetIndex(sheetNumber);
		CT_DefinedName cT_DefinedName = ((workbook.definedNames == null) ? workbook.AddNewDefinedNames() : workbook.definedNames).AddNewDefinedName();
		cT_DefinedName.name = builtInName;
		cT_DefinedName.localSheetId = (uint)sheetNumber;
		cT_DefinedName.localSheetIdSpecified = true;
		if (GetBuiltInName(builtInName, sheetNumber) != null)
		{
			throw new POIXMLException("Builtin (" + builtInName + ") already exists for sheet (" + sheetNumber + ")");
		}
		return CreateAndStoreName(cT_DefinedName);
	}

	public void SetSelectedTab(int index)
	{
		int num = 0;
		foreach (XSSFSheet sheet in sheets)
		{
			sheet.IsSelected = num == index;
			num++;
		}
	}

	public void SetSheetName(int sheetIndex, string sheetname)
	{
		ValidateSheetIndex(sheetIndex);
		string sheetName = GetSheetName(sheetIndex);
		if (sheetname != null && sheetname.Length > 31)
		{
			sheetname = sheetname.Substring(0, 31);
		}
		WorkbookUtil.ValidateSheetName(sheetname);
		if (!sheetname.Equals(sheetName))
		{
			if (ContainsSheet(sheetname, sheetIndex))
			{
				throw new ArgumentException($"The workbook already contains a sheet named '{sheetname}'");
			}
			new XSSFFormulaUtils(this).UpdateSheetName(sheetIndex, sheetName, sheetname);
			workbook.sheets.GetSheetArray(sheetIndex).name = sheetname;
		}
	}

	public void SetSheetOrder(string sheetname, int pos)
	{
		int sheetIndex = GetSheetIndex(sheetname);
		XSSFSheet item = sheets[sheetIndex];
		sheets.RemoveAt(sheetIndex);
		sheets.Insert(pos, item);
		CT_Sheets cT_Sheets = workbook.sheets;
		CT_Sheet sheet = cT_Sheets.GetSheetArray(sheetIndex).Copy();
		workbook.sheets.RemoveSheet(sheetIndex);
		cT_Sheets.InsertNewSheet(pos).Set(sheet);
		List<CT_Sheet> sheet2 = cT_Sheets.sheet;
		for (int i = 0; i < sheets.Count; i++)
		{
			sheets[i].sheet = sheet2[i];
		}
		int activeSheetIndex = ActiveSheetIndex;
		if (activeSheetIndex == sheetIndex)
		{
			SetActiveSheet(pos);
		}
		else if ((activeSheetIndex >= sheetIndex || activeSheetIndex >= pos) && (activeSheetIndex <= sheetIndex || activeSheetIndex <= pos))
		{
			if (pos > sheetIndex)
			{
				SetActiveSheet(activeSheetIndex - 1);
			}
			else
			{
				SetActiveSheet(activeSheetIndex + 1);
			}
		}
	}

	private void SaveNamedRanges()
	{
		if (namedRanges.Count > 0)
		{
			CT_DefinedNames cT_DefinedNames = new CT_DefinedNames();
			List<CT_DefinedName> list = new List<CT_DefinedName>(namedRanges.Count);
			foreach (XSSFName namedRange in namedRanges)
			{
				list.Add(namedRange.GetCTName());
			}
			cT_DefinedNames.SetDefinedNameArray(list);
			if (workbook.IsSetDefinedNames())
			{
				workbook.unsetDefinedNames();
			}
			workbook.SetDefinedNames(cT_DefinedNames);
			ReprocessNamedRanges();
		}
		else if (workbook.IsSetDefinedNames())
		{
			workbook.unsetDefinedNames();
		}
	}

	private void ReprocessNamedRanges()
	{
		namedRangesByName = new Dictionary<string, List<XSSFName>>();
		namedRanges = new List<XSSFName>();
		if (!workbook.IsSetDefinedNames())
		{
			return;
		}
		foreach (CT_DefinedName item in workbook.definedNames.definedName)
		{
			CreateAndStoreName(item);
		}
	}

	private void SaveCalculationChain()
	{
		if (calcChain != null && calcChain.GetCTCalcChain().SizeOfCArray() == 0)
		{
			RemoveRelation(calcChain);
			calcChain = null;
		}
	}

	protected internal override void Commit()
	{
		SaveNamedRanges();
		SaveCalculationChain();
		PackagePart packagePart = GetPackagePart();
		doc.Save(packagePart.GetOutputStream());
	}

	public void Write(Stream stream, bool leaveOpen)
	{
		bool? flag = null;
		if (base.Package is ZipPackage)
		{
			flag = ((ZipPackage)base.Package).IsExternalStream;
			((ZipPackage)base.Package).IsExternalStream = leaveOpen;
		}
		Write(stream);
		if (flag.HasValue && base.Package is ZipPackage)
		{
			((ZipPackage)base.Package).IsExternalStream = flag.Value;
		}
	}

	public SharedStringsTable GetSharedStringSource()
	{
		return sharedStringSource;
	}

	public StylesTable GetStylesSource()
	{
		return stylesSource;
	}

	public ThemesTable GetTheme()
	{
		if (stylesSource == null)
		{
			return null;
		}
		return stylesSource.GetTheme();
	}

	public ICreationHelper GetCreationHelper()
	{
		if (_creationHelper == null)
		{
			_creationHelper = new XSSFCreationHelper(this);
		}
		return _creationHelper;
	}

	private bool ContainsSheet(string name, int excludeSheetIdx)
	{
		List<CT_Sheet> sheet = workbook.sheets.sheet;
		if (name.Length > Max_SENSITIVE_SHEET_NAME_LEN)
		{
			name = name.Substring(0, Max_SENSITIVE_SHEET_NAME_LEN);
		}
		for (int i = 0; i < sheet.Count; i++)
		{
			string text = sheet[i].name;
			if (text.Length > Max_SENSITIVE_SHEET_NAME_LEN)
			{
				text = text.Substring(0, Max_SENSITIVE_SHEET_NAME_LEN);
			}
			if (excludeSheetIdx != i && name.Equals(text, StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsDate1904()
	{
		CT_WorkbookPr workbookPr = workbook.workbookPr;
		if (workbookPr == null)
		{
			return false;
		}
		if (workbookPr.date1904Specified)
		{
			return workbookPr.date1904;
		}
		return false;
	}

	public override List<PackagePart> GetAllEmbedds()
	{
		List<PackagePart> list = new List<PackagePart>();
		foreach (XSSFSheet sheet in sheets)
		{
			foreach (PackageRelationship item in sheet.GetPackagePart().GetRelationshipsByType(XSSFRelation.OLEEMBEDDINGS.Relation))
			{
				list.Add(sheet.GetPackagePart().GetRelatedPart(item));
			}
			foreach (PackageRelationship item2 in sheet.GetPackagePart().GetRelationshipsByType(XSSFRelation.PACKEMBEDDINGS.Relation))
			{
				list.Add(sheet.GetPackagePart().GetRelatedPart(item2));
			}
		}
		return list;
	}

	public bool IsSheetHidden(int sheetIx)
	{
		ValidateSheetIndex(sheetIx);
		return sheets[sheetIx].sheet.state == ST_SheetState.hidden;
	}

	public bool IsSheetVeryHidden(int sheetIx)
	{
		ValidateSheetIndex(sheetIx);
		return sheets[sheetIx].sheet.state == ST_SheetState.veryHidden;
	}

	public void SetSheetHidden(int sheetIx, bool hidden)
	{
		SetSheetHidden(sheetIx, hidden ? SheetState.Hidden : SheetState.Visible);
	}

	public void SetSheetHidden(int sheetIx, SheetState state)
	{
		ValidateSheetIndex(sheetIx);
		WorkbookUtil.ValidateSheetState(state);
		sheets[sheetIx].sheet.state = (ST_SheetState)state;
	}

	public void SetSheetHidden(int sheetIx, int hidden)
	{
		ValidateSheetIndex(sheetIx);
		SetSheetHidden(sheetIx, (SheetState)hidden);
	}

	internal void OnDeleteFormula(XSSFCell cell)
	{
		if (calcChain != null)
		{
			int sheetId = (int)((XSSFSheet)cell.Sheet).sheet.sheetId;
			calcChain.RemoveItem(sheetId, cell.GetReference());
		}
	}

	public CalculationChain GetCalculationChain()
	{
		return calcChain;
	}

	public List<XSSFMap> GetCustomXMLMappings()
	{
		if (mapInfo != null)
		{
			return mapInfo.GetAllXSSFMaps();
		}
		return new List<XSSFMap>();
	}

	public MapInfo GetMapInfo()
	{
		return mapInfo;
	}

	public int LinkExternalWorkbook(string name, IWorkbook workbook)
	{
		throw new RuntimeException("Not Implemented - see bug #57184");
	}

	public bool IsStructureLocked()
	{
		if (WorkbookProtectionPresent())
		{
			return workbook.workbookProtection.lockStructure;
		}
		return false;
	}

	public bool IsWindowsLocked()
	{
		if (WorkbookProtectionPresent())
		{
			return workbook.workbookProtection.lockWindows;
		}
		return false;
	}

	public bool IsRevisionLocked()
	{
		if (WorkbookProtectionPresent())
		{
			return workbook.workbookProtection.lockRevision;
		}
		return false;
	}

	public void LockStructure()
	{
		CreateProtectionFieldIfNotPresent();
		workbook.workbookProtection.lockStructure = true;
	}

	public void UnlockStructure()
	{
		CreateProtectionFieldIfNotPresent();
		workbook.workbookProtection.lockStructure = false;
	}

	public void LockWindows()
	{
		CreateProtectionFieldIfNotPresent();
		workbook.workbookProtection.lockWindows = true;
	}

	public void UnlockWindows()
	{
		CreateProtectionFieldIfNotPresent();
		workbook.workbookProtection.lockWindows = false;
	}

	public void LockRevision()
	{
		CreateProtectionFieldIfNotPresent();
		workbook.workbookProtection.lockRevision = true;
	}

	public void UnlockRevision()
	{
		CreateProtectionFieldIfNotPresent();
		workbook.workbookProtection.lockRevision = false;
	}

	public void RemovePivotTables()
	{
		foreach (XSSFPivotTable pivotTable in pivotTables)
		{
			POIXMLDocumentPart pOIXMLDocumentPart = pivotTable.GetParent();
			if (pOIXMLDocumentPart is XSSFSheet)
			{
				pOIXMLDocumentPart.RemoveRelation(pivotTable);
			}
		}
		foreach (POIXMLDocumentPart relation in GetRelations())
		{
			if (relation is XSSFPivotCacheDefinition)
			{
				XSSFPivotCacheDefinition part = (XSSFPivotCacheDefinition)relation;
				RemoveRelation(part);
			}
		}
	}

	private bool WorkbookProtectionPresent()
	{
		return workbook.workbookProtection != null;
	}

	private void CreateProtectionFieldIfNotPresent()
	{
		if (workbook.workbookProtection == null)
		{
			workbook.workbookProtection = new CT_WorkbookProtection();
		}
	}

	internal UDFFinder GetUDFFinder()
	{
		return _udfFinder;
	}

	public void AddToolPack(UDFFinder toopack)
	{
		_udfFinder.Add(toopack);
	}

	public void SetForceFormulaRecalculation(bool value)
	{
		CT_Workbook cTWorkbook = GetCTWorkbook();
		CT_CalcPr cT_CalcPr = (cTWorkbook.IsSetCalcPr() ? cTWorkbook.calcPr : cTWorkbook.AddNewCalcPr());
		cT_CalcPr.calcId = 0u;
		if (value && cT_CalcPr.calcMode == ST_CalcMode.manual)
		{
			cT_CalcPr.calcMode = ST_CalcMode.auto;
		}
	}

	public bool GetForceFormulaRecalculation()
	{
		CT_CalcPr calcPr = GetCTWorkbook().calcPr;
		if (calcPr != null)
		{
			return calcPr.calcId != 0;
		}
		return false;
	}

	public XSSFTable GetTable(string name)
	{
		if (name != null && sheets != null)
		{
			foreach (XSSFSheet sheet in sheets)
			{
				foreach (XSSFTable table in sheet.GetTables())
				{
					if (name.Equals(table.Name, StringComparison.OrdinalIgnoreCase))
					{
						return table;
					}
				}
			}
		}
		return null;
	}

	public void SetActiveSheet(int sheetIndex)
	{
		ValidateSheetIndex(sheetIndex);
		foreach (CT_BookView item in workbook.bookViews.workbookView)
		{
			item.activeTab = (uint)sheetIndex;
		}
	}

	protected internal CT_PivotCache AddPivotCache(string rId)
	{
		CT_Workbook cTWorkbook = GetCTWorkbook();
		CT_PivotCaches cT_PivotCaches = ((!cTWorkbook.IsSetPivotCaches()) ? cTWorkbook.AddNewPivotCaches() : cTWorkbook.pivotCaches);
		CT_PivotCache cT_PivotCache = cT_PivotCaches.AddNewPivotCache();
		int cacheId = PivotTables.Count + 1;
		cT_PivotCache.cacheId = (uint)cacheId;
		cT_PivotCache.id = rId;
		if (pivotCaches == null)
		{
			pivotCaches = new List<CT_PivotCache>();
		}
		pivotCaches.Add(cT_PivotCache);
		return cT_PivotCache;
	}

	public int AddPicture(byte[] pictureData, PictureType format)
	{
		int num = GetAllPictures().Count + 1;
		XSSFPictureData xSSFPictureData = (XSSFPictureData)CreateRelationship(XSSFPictureData.RELATIONS[(int)format], XSSFFactory.GetInstance(), num, noRelation: true).DocumentPart;
		try
		{
			Stream outputStream = xSSFPictureData.GetPackagePart().GetOutputStream();
			outputStream.Write(pictureData, 0, pictureData.Length);
			outputStream.Close();
		}
		catch (IOException ex)
		{
			throw new POIXMLException(ex);
		}
		pictures.Add(xSSFPictureData);
		return num - 1;
	}

	public void SetVBAProject(Stream vbaProjectStream)
	{
		if (!IsMacroEnabled())
		{
			WorkbookType = XSSFWorkbookType.XLSM;
		}
		PackagePartName partName;
		try
		{
			partName = PackagingUriHelper.CreatePartName(XSSFRelation.VBA_MACROS.DefaultFileName);
		}
		catch (InvalidFormatException ex)
		{
			throw new POIXMLException(ex);
		}
		OPCPackage package = base.Package;
		Stream stream = (package.ContainPart(partName) ? package.GetPart(partName).GetOutputStream() : CreateRelationship(XSSFRelation.VBA_MACROS, XSSFFactory.GetInstance()).GetPackagePart().GetOutputStream());
		try
		{
			IOUtils.Copy(vbaProjectStream, stream);
		}
		finally
		{
			IOUtils.CloseQuietly(stream);
		}
	}

	public void SetVBAProject(XSSFWorkbook macroWorkbook)
	{
		if (macroWorkbook.IsMacroEnabled())
		{
			Stream contents = XSSFRelation.VBA_MACROS.GetContents(macroWorkbook.CorePart);
			if (contents != null)
			{
				SetVBAProject(contents);
			}
		}
	}

	public int IndexOf(ISheet item)
	{
		throw new NotImplementedException();
	}

	public void Insert(int index, ISheet item)
	{
		sheets.Insert(index, (XSSFSheet)item);
	}

	public void RemoveAt(int index)
	{
		RemoveSheetAt(index);
	}

	public void Add(ISheet item)
	{
		sheets.Add((XSSFSheet)item);
	}

	public void Clear()
	{
		sheets.Clear();
	}

	public bool Contains(ISheet item)
	{
		return sheets.Contains(item as XSSFSheet);
	}

	public void CopyTo(ISheet[] array, int arrayIndex)
	{
		throw new NotImplementedException();
	}

	public bool Remove(ISheet item)
	{
		return sheets.Remove((XSSFSheet)item);
	}
}
