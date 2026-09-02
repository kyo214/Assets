using System;
using System.Collections;
using System.Configuration;
using System.Drawing;
using System.IO;
using NPOI.Util.Collections;

namespace NPOI.HSSF.UserModel;

internal class StaticFontMetrics
{
	private const string FONT_METRICS_PROPERTIES_FILE_NAME = "font_metrics.properties";

	private static Properties fontMetricsProps;

	private static Hashtable fontDetailsMap = new Hashtable();

	public static FontDetails GetFontDetails(Font font)
	{
		if (fontMetricsProps == null)
		{
			Stream stream = null;
			try
			{
				fontMetricsProps = new Properties();
				string text = null;
				try
				{
					text = ConfigurationManager.AppSettings["font.metrics.filename"];
				}
				catch (Exception)
				{
				}
				if (text != null)
				{
					if (!File.Exists(text))
					{
						throw new FileNotFoundException("font_metrics.properties not found at path " + Path.GetFullPath(text));
					}
					stream = typeof(StaticFontMetrics).Assembly.GetManifestResourceStream("font_metrics.properties");
				}
				else
				{
					stream = typeof(StaticFontMetrics).Assembly.GetManifestResourceStream("font_metrics.properties");
					if (stream == null)
					{
						throw new FileNotFoundException("font_metrics.properties not found in classpath");
					}
				}
				fontMetricsProps.Load(stream);
			}
			catch (IOException ex2)
			{
				throw new Exception("Could not Load font metrics: " + ex2.Message);
			}
			finally
			{
				if (stream != null)
				{
					try
					{
						stream.Close();
					}
					catch (IOException)
					{
					}
				}
			}
		}
		string text2 = font.FontFamily.Name;
		string text3 = "";
		if (font.Bold)
		{
			text3 += "bold";
		}
		if (font.Italic)
		{
			text3 += "italic";
		}
		if (fontMetricsProps[FontDetails.BuildFontHeightProperty(text2)] == null && fontMetricsProps[FontDetails.BuildFontHeightProperty(text2 + "." + text3)] != null)
		{
			text2 = text2 + "." + text3;
		}
		if (fontDetailsMap[text2] == null)
		{
			FontDetails fontDetails = FontDetails.Create(text2, fontMetricsProps);
			fontDetailsMap[text2] = fontDetails;
			return fontDetails;
		}
		return (FontDetails)fontDetailsMap[text2];
	}
}
