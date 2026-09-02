using System;
using NPOI.DDF;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.HSSF.UserModel;

public class HSSFPictureData : IPictureData
{
	public const short MSOBI_WMF = 8544;

	public const short MSOBI_EMF = 15680;

	public const short MSOBI_PICT = 21536;

	public const short MSOBI_PNG = 28160;

	public const short MSOBI_JPEG = 18080;

	public const short MSOBI_DIB = 31360;

	public const short FORMAT_MASK = -16;

	private EscherBlipRecord blip;

	public byte[] Data
	{
		get
		{
			byte[] array = blip.PictureData;
			if (PngUtils.MatchesPngHeader(array, 16))
			{
				byte[] array2 = new byte[array.Length - 16];
				Array.Copy(array, 16, array2, 0, array2.Length);
				array = array2;
			}
			return array;
		}
	}

	public int Format => blip.RecordId - -4072;

	public string MimeType => blip.RecordId switch
	{
		-4069 => "image/x-wmf", 
		-4070 => "image/x-emf", 
		-4068 => "image/x-pict", 
		-4066 => "image/png", 
		-4067 => "image/jpeg", 
		-4065 => "image/bmp", 
		_ => "image/unknown", 
	};

	public PictureType PictureType => blip.RecordId switch
	{
		-4069 => PictureType.WMF, 
		-4070 => PictureType.EMF, 
		-4068 => PictureType.PICT, 
		-4066 => PictureType.PNG, 
		-4067 => PictureType.JPEG, 
		-4065 => PictureType.DIB, 
		_ => PictureType.Unknown, 
	};

	public HSSFPictureData(EscherBlipRecord blip)
	{
		this.blip = blip;
	}

	public string SuggestFileExtension()
	{
		return blip.RecordId switch
		{
			-4069 => "wmf", 
			-4070 => "emf", 
			-4068 => "pict", 
			-4066 => "png", 
			-4067 => "jpeg", 
			-4065 => "dib", 
			_ => "", 
		};
	}
}
