namespace NPOI.SS.UserModel;

public interface IPictureData
{
	byte[] Data { get; }

	string MimeType { get; }

	PictureType PictureType { get; }

	string SuggestFileExtension();
}
