namespace NPOI.SS.UserModel;

public class CellCopyPolicy
{
	public class Builder
	{
		internal bool copyCellValue = DEFAULT_COPY_CELL_VALUE_POLICY;

		internal bool copyCellStyle = DEFAULT_COPY_CELL_STYLE_POLICY;

		internal bool copyCellFormula = DEFAULT_COPY_CELL_FORMULA_POLICY;

		internal bool copyHyperlink = DEFAULT_COPY_HYPERLINK_POLICY;

		internal bool mergeHyperlink = DEFAULT_MERGE_HYPERLINK_POLICY;

		internal bool copyRowHeight = DEFAULT_COPY_ROW_HEIGHT_POLICY;

		internal bool condenseRows = DEFAULT_CONDENSE_ROWS_POLICY;

		internal bool copyMergedRegions = DEFAULT_COPY_MERGED_REGIONS_POLICY;

		public Builder CellValue(bool copyCellValue)
		{
			this.copyCellValue = copyCellValue;
			return this;
		}

		public Builder CellStyle(bool copyCellStyle)
		{
			this.copyCellStyle = copyCellStyle;
			return this;
		}

		public Builder CellFormula(bool copyCellFormula)
		{
			this.copyCellFormula = copyCellFormula;
			return this;
		}

		public Builder CopyHyperlink(bool copyHyperlink)
		{
			this.copyHyperlink = copyHyperlink;
			return this;
		}

		public Builder MergeHyperlink(bool mergeHyperlink)
		{
			this.mergeHyperlink = mergeHyperlink;
			return this;
		}

		public Builder RowHeight(bool copyRowHeight)
		{
			this.copyRowHeight = copyRowHeight;
			return this;
		}

		public Builder CondenseRows(bool condenseRows)
		{
			this.condenseRows = condenseRows;
			return this;
		}

		public Builder MergedRegions(bool copyMergedRegions)
		{
			this.copyMergedRegions = copyMergedRegions;
			return this;
		}

		public CellCopyPolicy Build()
		{
			return new CellCopyPolicy(this);
		}
	}

	public static bool DEFAULT_COPY_CELL_VALUE_POLICY = true;

	public static bool DEFAULT_COPY_CELL_STYLE_POLICY = true;

	public static bool DEFAULT_COPY_CELL_FORMULA_POLICY = true;

	public static bool DEFAULT_COPY_HYPERLINK_POLICY = true;

	public static bool DEFAULT_MERGE_HYPERLINK_POLICY = false;

	public static bool DEFAULT_COPY_ROW_HEIGHT_POLICY = true;

	public static bool DEFAULT_CONDENSE_ROWS_POLICY = false;

	public static bool DEFAULT_COPY_MERGED_REGIONS_POLICY = true;

	private bool copyCellValue = DEFAULT_COPY_CELL_VALUE_POLICY;

	private bool copyCellStyle = DEFAULT_COPY_CELL_STYLE_POLICY;

	private bool copyCellFormula = DEFAULT_COPY_CELL_FORMULA_POLICY;

	private bool copyHyperlink = DEFAULT_COPY_HYPERLINK_POLICY;

	private bool mergeHyperlink = DEFAULT_MERGE_HYPERLINK_POLICY;

	private bool copyRowHeight = DEFAULT_COPY_ROW_HEIGHT_POLICY;

	private bool condenseRows = DEFAULT_CONDENSE_ROWS_POLICY;

	private bool copyMergedRegions = DEFAULT_COPY_MERGED_REGIONS_POLICY;

	public bool IsCopyCellValue
	{
		get
		{
			return copyCellValue;
		}
		set
		{
			copyCellValue = value;
		}
	}

	public bool IsCopyCellStyle
	{
		get
		{
			return copyCellStyle;
		}
		set
		{
			copyCellStyle = value;
		}
	}

	public bool IsCopyCellFormula
	{
		get
		{
			return copyCellFormula;
		}
		set
		{
			copyCellFormula = value;
		}
	}

	public bool IsCopyHyperlink
	{
		get
		{
			return copyHyperlink;
		}
		set
		{
			copyHyperlink = value;
		}
	}

	public bool IsMergeHyperlink
	{
		get
		{
			return mergeHyperlink;
		}
		set
		{
			mergeHyperlink = value;
		}
	}

	public bool IsCopyRowHeight
	{
		get
		{
			return copyRowHeight;
		}
		set
		{
			copyRowHeight = value;
		}
	}

	public bool IsCondenseRows
	{
		get
		{
			return condenseRows;
		}
		set
		{
			condenseRows = value;
		}
	}

	public bool IsCopyMergedRegions
	{
		get
		{
			return copyMergedRegions;
		}
		set
		{
			copyMergedRegions = value;
		}
	}

	public CellCopyPolicy()
	{
	}

	public CellCopyPolicy(CellCopyPolicy other)
	{
		copyCellValue = other.IsCopyCellValue;
		copyCellStyle = other.IsCopyCellStyle;
		copyCellFormula = other.IsCopyCellFormula;
		copyHyperlink = other.IsCopyHyperlink;
		mergeHyperlink = other.IsMergeHyperlink;
		copyRowHeight = other.IsCopyRowHeight;
		condenseRows = other.IsCondenseRows;
		copyMergedRegions = other.IsCopyMergedRegions;
	}

	private CellCopyPolicy(Builder builder)
	{
		copyCellValue = builder.copyCellValue;
		copyCellStyle = builder.copyCellStyle;
		copyCellFormula = builder.copyCellFormula;
		copyHyperlink = builder.copyHyperlink;
		mergeHyperlink = builder.mergeHyperlink;
		copyRowHeight = builder.copyRowHeight;
		condenseRows = builder.condenseRows;
		copyMergedRegions = builder.copyMergedRegions;
	}

	public Builder CreateBuilder()
	{
		return new Builder().CellValue(copyCellValue).CellStyle(copyCellStyle).CellFormula(copyCellFormula)
			.CopyHyperlink(copyHyperlink)
			.MergeHyperlink(mergeHyperlink)
			.RowHeight(copyRowHeight)
			.CondenseRows(condenseRows)
			.MergedRegions(copyMergedRegions);
	}

	public CellCopyPolicy Clone()
	{
		return (CellCopyPolicy)MemberwiseClone();
	}
}
