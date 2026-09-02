using System;

namespace NPOI.SS.UserModel;

[Obsolete("deprecated POI 3.15 beta 2. Boldweight constants no longer needed due to IsBold property")]
public enum FontBoldWeight : short
{
	None = 0,
	Normal = 400,
	Bold = 700
}
