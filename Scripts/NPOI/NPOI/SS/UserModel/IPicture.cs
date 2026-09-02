using System.Drawing;

namespace NPOI.SS.UserModel;

public interface IPicture
{
	IPictureData PictureData { get; }

	IClientAnchor ClientAnchor { get; }

	ISheet Sheet { get; }

	void Resize();

	void Resize(double scale);

	void Resize(double scaleX, double scaleY);

	IClientAnchor GetPreferredSize();

	IClientAnchor GetPreferredSize(double scaleX, double scaleY);

	Size GetImageDimension();
}
