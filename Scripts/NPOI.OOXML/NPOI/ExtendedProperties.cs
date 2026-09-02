using NPOI.OpenXmlFormats;

namespace NPOI;

public class ExtendedProperties
{
	public ExtendedPropertiesDocument props;

	public string Template => props.GetProperties().Template;

	public string Manager => props.GetProperties().Manager;

	public string Company => props.GetProperties().Company;

	public string PresentationFormat => props.GetProperties().PresentationFormat;

	public string Application => props.GetProperties().Application;

	public string AppVersion => props.GetProperties().AppVersion;

	public int Pages
	{
		get
		{
			if (props.GetProperties().IsSetPages())
			{
				return props.GetProperties().Pages;
			}
			return -1;
		}
	}

	public int Words
	{
		get
		{
			if (props.GetProperties().IsSetWords())
			{
				return props.GetProperties().Words;
			}
			return -1;
		}
	}

	public int Characters
	{
		get
		{
			if (props.GetProperties().IsSetCharacters())
			{
				return props.GetProperties().Characters;
			}
			return -1;
		}
	}

	public int CharactersWithSpaces
	{
		get
		{
			if (props.GetProperties().IsSetCharactersWithSpaces())
			{
				return props.GetProperties().CharactersWithSpaces;
			}
			return -1;
		}
	}

	public int Lines
	{
		get
		{
			if (props.GetProperties().IsSetLines())
			{
				return props.GetProperties().Lines;
			}
			return -1;
		}
	}

	public int Paragraphs
	{
		get
		{
			if (props.GetProperties().IsSetParagraphs())
			{
				return props.GetProperties().Paragraphs;
			}
			return -1;
		}
	}

	public int Slides
	{
		get
		{
			if (props.GetProperties().IsSetSlides())
			{
				return props.GetProperties().Slides;
			}
			return -1;
		}
	}

	public int Notes
	{
		get
		{
			if (props.GetProperties().IsSetNotes())
			{
				return props.GetProperties().Notes;
			}
			return -1;
		}
	}

	public int TotalTime
	{
		get
		{
			if (props.GetProperties().IsSetTotalTime())
			{
				return props.GetProperties().TotalTime;
			}
			return -1;
		}
	}

	public int HiddenSlides
	{
		get
		{
			if (props.GetProperties().IsSetHiddenSlides())
			{
				return props.GetProperties().HiddenSlides;
			}
			return -1;
		}
	}

	public int MMClips
	{
		get
		{
			if (props.GetProperties().IsSetMMClips())
			{
				return props.GetProperties().MMClips;
			}
			return -1;
		}
	}

	public string HyperlinkBase => props.GetProperties().HyperlinkBase;

	internal ExtendedProperties(ExtendedPropertiesDocument props)
	{
		this.props = props;
	}

	public CT_ExtendedProperties GetUnderlyingProperties()
	{
		return props.GetProperties();
	}
}
