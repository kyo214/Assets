using System;
using System.Text;
using NPOI.HSSF.Model;
using NPOI.HSSF.Record;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFName : IName
{
	private HSSFWorkbook book;

	private NameRecord _definedNameRec;

	private NameCommentRecord _commentRec;

	public string SheetName
	{
		get
		{
			int externSheetNumber = _definedNameRec.ExternSheetNumber;
			return book.Workbook.FindSheetFirstNameFromExternSheet(externSheetNumber);
		}
	}

	public string NameName
	{
		get
		{
			return _definedNameRec.NameText;
		}
		set
		{
			ValidateName(value);
			_definedNameRec.NameText = value;
			InternalWorkbook workbook = book.Workbook;
			int sheetNumber = _definedNameRec.SheetNumber;
			for (int num = workbook.NumNames - 1; num >= 0; num--)
			{
				NameRecord nameRecord = workbook.GetNameRecord(num);
				if (nameRecord != _definedNameRec && nameRecord.NameText.Equals(NameName, StringComparison.OrdinalIgnoreCase) && sheetNumber == nameRecord.SheetNumber)
				{
					string message = "The " + ((sheetNumber == 0) ? "workbook" : "sheet") + " already contains this name: " + value;
					_definedNameRec.NameText = value + "(2)";
					throw new ArgumentException(message);
				}
			}
			if (_commentRec != null)
			{
				_ = _commentRec.NameText;
				_commentRec.NameText = value;
				book.Workbook.UpdateNameCommentRecordCache(_commentRec);
			}
		}
	}

	public string RefersToFormula
	{
		get
		{
			if (_definedNameRec.IsFunctionName)
			{
				throw new InvalidOperationException("Only applicable to named ranges");
			}
			Ptg[] nameDefinition = _definedNameRec.NameDefinition;
			if (nameDefinition.Length < 1)
			{
				return null;
			}
			return HSSFFormulaParser.ToFormulaString(book, nameDefinition);
		}
		set
		{
			Ptg[] nameDefinition = HSSFFormulaParser.Parse(value, book, FormulaType.NamedRange, SheetIndex);
			_definedNameRec.NameDefinition = nameDefinition;
		}
	}

	public int SheetIndex
	{
		get
		{
			return _definedNameRec.SheetNumber - 1;
		}
		set
		{
			int num = book.NumberOfSheets - 1;
			if (value < -1 || value > num)
			{
				throw new ArgumentException("Sheet index (" + value + ") is out of range" + ((num == -1) ? "" : (" (0.." + num + ")")));
			}
			_definedNameRec.SheetNumber = value + 1;
		}
	}

	public string Comment
	{
		get
		{
			if (_commentRec != null && _commentRec.CommentText != null && _commentRec.CommentText.Length > 0)
			{
				return _commentRec.CommentText;
			}
			return _definedNameRec.DescriptionText;
		}
		set
		{
			_definedNameRec.DescriptionText = value;
		}
	}

	public bool IsDeleted => Ptg.DoesFormulaReferToDeletedCell(_definedNameRec.NameDefinition);

	public bool IsFunctionName => _definedNameRec.IsFunctionName;

	internal HSSFName(HSSFWorkbook book, NameRecord name)
		: this(book, name, null)
	{
	}

	internal HSSFName(HSSFWorkbook book, NameRecord name, NameCommentRecord comment)
	{
		this.book = book;
		_definedNameRec = name;
		_commentRec = comment;
	}

	private void ValidateName(string name)
	{
		if (name.Length == 0)
		{
			throw new ArgumentException("Name cannot be blank");
		}
		char c = name[0];
		string text = "_";
		if (!char.IsLetter(c) && text.IndexOf(c) == -1)
		{
			throw new ArgumentException("Invalid name: '" + name + "': first character must be underscore or a letter");
		}
		text = "_\\";
		char[] array = name.ToCharArray();
		foreach (char c2 in array)
		{
			if (!char.IsLetterOrDigit(c2) && text.IndexOf(c2) == -1)
			{
				throw new ArgumentException("Invalid name: '" + name + "'");
			}
		}
	}

	public void SetNameDefinition(Ptg[] ptgs)
	{
		_definedNameRec.NameDefinition = ptgs;
	}

	public void SetFunction(bool value)
	{
		_definedNameRec.SetFunction(value);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(64);
		stringBuilder.Append(GetType().Name).Append(" [");
		stringBuilder.Append(_definedNameRec.NameText);
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}
}
