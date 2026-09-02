using System.Text.RegularExpressions;

namespace NPOI.SS.UserModel;

public static class Table
{
	public static Regex IsStructuredReference = new Regex("[a-zA-Z_\\\\][a-zA-Z0-9._]*\\[.*\\]");
}
