using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.Formula.UDF;
using NPOI.Util;

namespace NPOI.SS.UserModel;

public interface IWorkbook : ICloseable
{
	int ActiveSheetIndex { get; }

	int FirstVisibleTab { get; set; }

	int NumberOfSheets { get; }

	short NumberOfFonts { get; }

	int NumCellStyles { get; }

	int NumberOfNames { get; }

	MissingCellPolicy MissingCellPolicy { get; set; }

	bool IsHidden { get; set; }

	SpreadsheetVersion SpreadsheetVersion { get; }

	void SetSheetOrder(string sheetname, int pos);

	void SetSelectedTab(int index);

	void SetActiveSheet(int sheetIndex);

	string GetSheetName(int sheet);

	void SetSheetName(int sheet, string name);

	int GetSheetIndex(string name);

	int GetSheetIndex(ISheet sheet);

	ISheet CreateSheet();

	ISheet CreateSheet(string sheetname);

	ISheet CloneSheet(int sheetNum);

	ISheet GetSheetAt(int index);

	ISheet GetSheet(string name);

	void RemoveSheetAt(int index);

	IEnumerator<ISheet> GetEnumerator();

	IFont CreateFont();

	[Obsolete("deprecated 3.15 beta 2. Use {@link #findFont(boolean, short, short, String, boolean, boolean, short, byte)} instead.")]
	IFont FindFont(short boldWeight, short color, short fontHeight, string name, bool italic, bool strikeout, FontSuperScript typeOffset, FontUnderlineType underline);

	IFont FindFont(bool bold, short color, short fontHeight, string name, bool italic, bool strikeout, FontSuperScript typeOffset, FontUnderlineType underline);

	IFont GetFontAt(short idx);

	ICellStyle CreateCellStyle();

	ICellStyle GetCellStyleAt(int idx);

	void Write(Stream stream);

	IName GetName(string name);

	IList<IName> GetNames(string name);

	IList<IName> GetAllNames();

	IName GetNameAt(int nameIndex);

	IName CreateName();

	int GetNameIndex(string name);

	void RemoveName(int index);

	void RemoveName(string name);

	void RemoveName(IName name);

	int LinkExternalWorkbook(string name, IWorkbook workbook);

	void SetPrintArea(int sheetIndex, string reference);

	void SetPrintArea(int sheetIndex, int startColumn, int endColumn, int startRow, int endRow);

	string GetPrintArea(int sheetIndex);

	void RemovePrintArea(int sheetIndex);

	IDataFormat CreateDataFormat();

	int AddPicture(byte[] pictureData, PictureType format);

	IList GetAllPictures();

	ICreationHelper GetCreationHelper();

	bool IsSheetHidden(int sheetIx);

	bool IsSheetVeryHidden(int sheetIx);

	void SetSheetHidden(int sheetIx, SheetState hidden);

	void SetSheetHidden(int sheetIx, int hidden);

	void AddToolPack(UDFFinder toopack);

	bool IsDate1904();

	new void Close();
}
