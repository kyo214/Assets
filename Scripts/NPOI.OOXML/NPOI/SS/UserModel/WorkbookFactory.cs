using System;
using System.IO;
using NPOI.HSSF.Record.Crypto;
using NPOI.HSSF.UserModel;
using NPOI.OpenXml4Net.Exceptions;
using NPOI.OpenXml4Net.OPC;
using NPOI.POIFS.Crypt;
using NPOI.POIFS.FileSystem;
using NPOI.Util;
using NPOI.XSSF.UserModel;

namespace NPOI.SS.UserModel;

public class WorkbookFactory
{
	public static IWorkbook Create(POIFSFileSystem fs)
	{
		return new HSSFWorkbook(fs);
	}

	public static IWorkbook Create(NPOIFSFileSystem fs)
	{
		return new HSSFWorkbook(fs.Root, preserveNodes: true);
	}

	private static IWorkbook Create(NPOIFSFileSystem fs, string password)
	{
		DirectoryNode root = fs.Root;
		if (root.HasEntry(Decryptor.DEFAULT_POIFS_ENTRY))
		{
			return Create(OPCPackage.Open(DocumentFactoryHelper.GetDecryptedStream(fs, password)));
		}
		if (password != null)
		{
			Biff8EncryptionKey.CurrentUserPassword = password;
		}
		try
		{
			return new HSSFWorkbook(root, preserveNodes: true);
		}
		finally
		{
			Biff8EncryptionKey.CurrentUserPassword = null;
		}
	}

	public static IWorkbook Create(OPCPackage pkg)
	{
		return new XSSFWorkbook(pkg);
	}

	public static IWorkbook Create(Stream inputStream, bool bReadonly)
	{
		if (inputStream.Length == 0L)
		{
			throw new EmptyFileException();
		}
		inputStream = new PushbackStream(inputStream);
		if (POIFSFileSystem.HasPOIFSHeader(inputStream))
		{
			return new HSSFWorkbook(inputStream);
		}
		inputStream.Position = 0L;
		if (DocumentFactoryHelper.HasOOXMLHeader(inputStream))
		{
			return new XSSFWorkbook(OPCPackage.Open(inputStream, bReadonly));
		}
		throw new InvalidFormatException("Your stream was neither an OLE2 stream, nor an OOXML stream.");
	}

	public static IWorkbook Create(Stream inputStream)
	{
		return Create(inputStream, bReadonly: false);
	}

	public static IWorkbook Create(string file)
	{
		if (!File.Exists(file))
		{
			throw new FileNotFoundException(file);
		}
		using FileStream inputStream = File.OpenRead(file);
		return Create(inputStream);
	}

	public static IWorkbook Create(string file, string password)
	{
		return Create(file, password, readOnly: false);
	}

	public static IWorkbook Create(string file, string password, bool readOnly)
	{
		if (!File.Exists(file))
		{
			throw new FileNotFoundException(file);
		}
		FileInfo file2 = new FileInfo(file);
		try
		{
			NPOIFSFileSystem nPOIFSFileSystem = new NPOIFSFileSystem(file2, readOnly);
			try
			{
				return Create(nPOIFSFileSystem, password);
			}
			finally
			{
				nPOIFSFileSystem?.Close();
			}
		}
		catch (OfficeXmlFileException)
		{
			OPCPackage oPCPackage = OPCPackage.Open(file, (!readOnly) ? PackageAccess.READ_WRITE : PackageAccess.READ);
			try
			{
				return new XSSFWorkbook(oPCPackage);
			}
			catch (IOException ex2)
			{
				oPCPackage.Revert();
				throw ex2;
			}
			catch (Exception ex3)
			{
				oPCPackage.Revert();
				throw ex3;
			}
		}
	}

	public static IWorkbook Create(Stream inputStream, ImportOption importOption)
	{
		SetImportOption(importOption);
		return Create(inputStream, bReadonly: true);
	}

	public static IFormulaEvaluator CreateFormulaEvaluator(IWorkbook workbook)
	{
		if (typeof(HSSFWorkbook) == workbook.GetType())
		{
			return new HSSFFormulaEvaluator(workbook as HSSFWorkbook);
		}
		return new XSSFFormulaEvaluator(workbook as XSSFWorkbook);
	}

	public static void SetImportOption(ImportOption importOption)
	{
		if (ImportOption.SheetContentOnly == importOption)
		{
			XSSFRelation.AddRelation(XSSFRelation.WORKSHEET);
			XSSFRelation.AddRelation(XSSFRelation.SHARED_STRINGS);
			XSSFRelation.RemoveRelation(XSSFRelation.WORKBOOK);
			XSSFRelation.RemoveRelation(XSSFRelation.MACROS_WORKBOOK);
			XSSFRelation.RemoveRelation(XSSFRelation.TEMPLATE_WORKBOOK);
			XSSFRelation.RemoveRelation(XSSFRelation.MACRO_TEMPLATE_WORKBOOK);
			XSSFRelation.RemoveRelation(XSSFRelation.MACRO_ADDIN_WORKBOOK);
			XSSFRelation.RemoveRelation(XSSFRelation.CHARTSHEET);
			XSSFRelation.RemoveRelation(XSSFRelation.STYLES);
			XSSFRelation.RemoveRelation(XSSFRelation.DRAWINGS);
			XSSFRelation.RemoveRelation(XSSFRelation.CHART);
			XSSFRelation.RemoveRelation(XSSFRelation.VML_DRAWINGS);
			XSSFRelation.RemoveRelation(XSSFRelation.CUSTOM_XML_MAPPINGS);
			XSSFRelation.RemoveRelation(XSSFRelation.TABLE);
			XSSFRelation.RemoveRelation(XSSFRelation.IMAGES);
			XSSFRelation.RemoveRelation(XSSFRelation.IMAGE_EMF);
			XSSFRelation.RemoveRelation(XSSFRelation.IMAGE_WMF);
			XSSFRelation.RemoveRelation(XSSFRelation.IMAGE_PICT);
			XSSFRelation.RemoveRelation(XSSFRelation.IMAGE_JPEG);
			XSSFRelation.RemoveRelation(XSSFRelation.IMAGE_PNG);
			XSSFRelation.RemoveRelation(XSSFRelation.IMAGE_DIB);
			XSSFRelation.RemoveRelation(XSSFRelation.SHEET_COMMENTS);
			XSSFRelation.RemoveRelation(XSSFRelation.SHEET_HYPERLINKS);
			XSSFRelation.RemoveRelation(XSSFRelation.OLEEMBEDDINGS);
			XSSFRelation.RemoveRelation(XSSFRelation.PACKEMBEDDINGS);
			XSSFRelation.RemoveRelation(XSSFRelation.VBA_MACROS);
			XSSFRelation.RemoveRelation(XSSFRelation.ACTIVEX_CONTROLS);
			XSSFRelation.RemoveRelation(XSSFRelation.ACTIVEX_BINS);
			XSSFRelation.RemoveRelation(XSSFRelation.THEME);
			XSSFRelation.RemoveRelation(XSSFRelation.CALC_CHAIN);
			XSSFRelation.RemoveRelation(XSSFRelation.PRINTER_SETTINGS);
		}
		else if (ImportOption.TextOnly == importOption)
		{
			XSSFRelation.AddRelation(XSSFRelation.WORKSHEET);
			XSSFRelation.AddRelation(XSSFRelation.SHARED_STRINGS);
			XSSFRelation.AddRelation(XSSFRelation.SHEET_COMMENTS);
			XSSFRelation.RemoveRelation(XSSFRelation.WORKBOOK);
			XSSFRelation.RemoveRelation(XSSFRelation.MACROS_WORKBOOK);
			XSSFRelation.RemoveRelation(XSSFRelation.TEMPLATE_WORKBOOK);
			XSSFRelation.RemoveRelation(XSSFRelation.MACRO_TEMPLATE_WORKBOOK);
			XSSFRelation.RemoveRelation(XSSFRelation.MACRO_ADDIN_WORKBOOK);
			XSSFRelation.RemoveRelation(XSSFRelation.CHARTSHEET);
			XSSFRelation.RemoveRelation(XSSFRelation.STYLES);
			XSSFRelation.RemoveRelation(XSSFRelation.DRAWINGS);
			XSSFRelation.RemoveRelation(XSSFRelation.CHART);
			XSSFRelation.RemoveRelation(XSSFRelation.VML_DRAWINGS);
			XSSFRelation.RemoveRelation(XSSFRelation.CUSTOM_XML_MAPPINGS);
			XSSFRelation.RemoveRelation(XSSFRelation.TABLE);
			XSSFRelation.RemoveRelation(XSSFRelation.IMAGES);
			XSSFRelation.RemoveRelation(XSSFRelation.IMAGE_EMF);
			XSSFRelation.RemoveRelation(XSSFRelation.IMAGE_WMF);
			XSSFRelation.RemoveRelation(XSSFRelation.IMAGE_PICT);
			XSSFRelation.RemoveRelation(XSSFRelation.IMAGE_JPEG);
			XSSFRelation.RemoveRelation(XSSFRelation.IMAGE_PNG);
			XSSFRelation.RemoveRelation(XSSFRelation.IMAGE_DIB);
			XSSFRelation.RemoveRelation(XSSFRelation.SHEET_HYPERLINKS);
			XSSFRelation.RemoveRelation(XSSFRelation.OLEEMBEDDINGS);
			XSSFRelation.RemoveRelation(XSSFRelation.PACKEMBEDDINGS);
			XSSFRelation.RemoveRelation(XSSFRelation.VBA_MACROS);
			XSSFRelation.RemoveRelation(XSSFRelation.ACTIVEX_CONTROLS);
			XSSFRelation.RemoveRelation(XSSFRelation.ACTIVEX_BINS);
			XSSFRelation.RemoveRelation(XSSFRelation.THEME);
			XSSFRelation.RemoveRelation(XSSFRelation.CALC_CHAIN);
			XSSFRelation.RemoveRelation(XSSFRelation.PRINTER_SETTINGS);
		}
		else
		{
			XSSFRelation.AddRelation(XSSFRelation.WORKBOOK);
			XSSFRelation.AddRelation(XSSFRelation.MACROS_WORKBOOK);
			XSSFRelation.AddRelation(XSSFRelation.TEMPLATE_WORKBOOK);
			XSSFRelation.AddRelation(XSSFRelation.MACRO_TEMPLATE_WORKBOOK);
			XSSFRelation.AddRelation(XSSFRelation.MACRO_ADDIN_WORKBOOK);
			XSSFRelation.AddRelation(XSSFRelation.WORKSHEET);
			XSSFRelation.AddRelation(XSSFRelation.CHARTSHEET);
			XSSFRelation.AddRelation(XSSFRelation.SHARED_STRINGS);
			XSSFRelation.AddRelation(XSSFRelation.STYLES);
			XSSFRelation.AddRelation(XSSFRelation.DRAWINGS);
			XSSFRelation.AddRelation(XSSFRelation.CHART);
			XSSFRelation.AddRelation(XSSFRelation.VML_DRAWINGS);
			XSSFRelation.AddRelation(XSSFRelation.CUSTOM_XML_MAPPINGS);
			XSSFRelation.AddRelation(XSSFRelation.TABLE);
			XSSFRelation.AddRelation(XSSFRelation.IMAGES);
			XSSFRelation.AddRelation(XSSFRelation.IMAGE_EMF);
			XSSFRelation.AddRelation(XSSFRelation.IMAGE_WMF);
			XSSFRelation.AddRelation(XSSFRelation.IMAGE_PICT);
			XSSFRelation.AddRelation(XSSFRelation.IMAGE_JPEG);
			XSSFRelation.AddRelation(XSSFRelation.IMAGE_PNG);
			XSSFRelation.AddRelation(XSSFRelation.IMAGE_DIB);
			XSSFRelation.AddRelation(XSSFRelation.SHEET_COMMENTS);
			XSSFRelation.AddRelation(XSSFRelation.SHEET_HYPERLINKS);
			XSSFRelation.AddRelation(XSSFRelation.OLEEMBEDDINGS);
			XSSFRelation.AddRelation(XSSFRelation.PACKEMBEDDINGS);
			XSSFRelation.AddRelation(XSSFRelation.VBA_MACROS);
			XSSFRelation.AddRelation(XSSFRelation.ACTIVEX_CONTROLS);
			XSSFRelation.AddRelation(XSSFRelation.ACTIVEX_BINS);
			XSSFRelation.AddRelation(XSSFRelation.THEME);
			XSSFRelation.AddRelation(XSSFRelation.CALC_CHAIN);
			XSSFRelation.AddRelation(XSSFRelation.PRINTER_SETTINGS);
		}
	}
}
