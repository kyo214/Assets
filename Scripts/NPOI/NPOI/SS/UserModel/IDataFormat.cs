namespace NPOI.SS.UserModel;

public interface IDataFormat
{
	short GetFormat(string format);

	string GetFormat(short index);
}
