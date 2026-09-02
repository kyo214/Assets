namespace System.Data;

[Flags]
public enum DataViewRowState
{
	None = 0,
	Unchanged = 2,
	Added = 4,
	Deleted = 8,
	ModifiedCurrent = 0x10,
	ModifiedOriginal = 0x20,
	OriginalRows = Unchanged | Deleted | ModifiedOriginal,
	CurrentRows = Unchanged | Added | ModifiedCurrent
}
