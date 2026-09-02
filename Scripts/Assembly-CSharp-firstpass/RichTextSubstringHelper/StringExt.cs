namespace RichTextSubstringHelper;

public static class StringExt
{
	public static string RichTextSubString(this string text, int length)
	{
		RichTextSubStringMaker richTextSubStringMaker = new RichTextSubStringMaker(text);
		for (int i = 0; i < length; i++)
		{
			richTextSubStringMaker.Consume();
		}
		return richTextSubStringMaker.GetRichText();
	}

	public static int RichTextLength(this string text)
	{
		RichTextSubStringMaker richTextSubStringMaker = new RichTextSubStringMaker(text);
		int num = 0;
		while (richTextSubStringMaker.IsConsumable())
		{
			if (richTextSubStringMaker.Consume())
			{
				num++;
			}
		}
		return num;
	}
}
