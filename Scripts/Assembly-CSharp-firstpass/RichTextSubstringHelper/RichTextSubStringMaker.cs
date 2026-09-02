using System.Collections.Generic;
using UnityEngine;

namespace RichTextSubstringHelper;

public class RichTextSubStringMaker
{
	private string originalText;

	private string middleText;

	private Stack<RichTextTag> tagStack;

	private int consumedLength;

	private static readonly char[] tagBrackets = new char[2] { '<', '>' };

	public RichTextSubStringMaker(string original)
	{
		originalText = original;
		middleText = "";
		tagStack = new Stack<RichTextTag>();
		consumedLength = 0;
	}

	public string GetRichText()
	{
		if (tagStack.Count == 0)
		{
			return middleText;
		}
		string text = middleText;
		Queue<RichTextTag> queue = new Queue<RichTextTag>(tagStack);
		while (queue.Count != 0)
		{
			text += queue.Dequeue().endTag;
		}
		return text;
	}

	public bool IsConsumable()
	{
		return consumedLength < originalText.Length;
	}

	public bool Consume()
	{
		char num = PeekNextOriginChar();
		bool flag = num == '<' && IsNextTagBracketClosing(consumedLength + 1);
		bool flag2 = num == '<' && PeekNextNextOriginChar() == '/';
		if (flag | flag2)
		{
			if (flag2)
			{
				ConsumeEndTag();
			}
			else if (flag)
			{
				ConsumeStartTag();
			}
			if (IsConsumable())
			{
				return Consume();
			}
			return false;
		}
		ConsumeRawChar();
		return true;
	}

	private char? PeekNextNextOriginChar()
	{
		if (originalText.Length <= consumedLength + 1)
		{
			return null;
		}
		return originalText[consumedLength + 1];
	}

	private char PeekNextOriginChar()
	{
		return originalText[consumedLength];
	}

	private bool IsNextTagBracketClosing(int charIndexToStartSearch = 0)
	{
		int num = originalText.IndexOfAny(tagBrackets, charIndexToStartSearch);
		if (num != -1)
		{
			return originalText[num] == '>';
		}
		return false;
	}

	private void ConsumeStartTag()
	{
		string text = "";
		bool flag = false;
		ConsumeRawChar();
		while (true)
		{
			char? c = ConsumeRawChar();
			if (!c.HasValue)
			{
				Debug.LogError("Cannot close start tag");
				return;
			}
			if (c == '>')
			{
				break;
			}
			if (!flag)
			{
				if (!char.IsLetterOrDigit(c.Value))
				{
					flag = true;
					continue;
				}
				string text2 = text;
				char? c2 = c;
				text = text2 + c2;
			}
		}
		if (text == "")
		{
			Debug.LogWarning("Empty tag name");
		}
		tagStack.Push(new RichTextTag
		{
			tagName = text
		});
	}

	private void ConsumeEndTag()
	{
		string text = "";
		bool flag = false;
		ConsumeRawChar();
		ConsumeRawChar();
		while (true)
		{
			char? c = ConsumeRawChar();
			if (!c.HasValue)
			{
				Debug.LogError("Cannot close start tag");
				return;
			}
			if (c == '>')
			{
				break;
			}
			if (!flag)
			{
				if (!char.IsLetterOrDigit(c.Value))
				{
					flag = true;
					continue;
				}
				string text2 = text;
				char? c2 = c;
				text = text2 + c2;
			}
		}
		if (text == "")
		{
			Debug.LogWarning("Empty tag name");
		}
		if (tagStack.Count == 0)
		{
			Debug.LogError("Could not pop tag " + text);
		}
		if (tagStack.Peek().tagName != text)
		{
			Debug.LogError("Could not pop tag " + text + " expeted " + tagStack.Peek().tagName);
		}
		tagStack.Pop();
	}

	private char? ConsumeRawChar()
	{
		if (consumedLength > originalText.Length)
		{
			return null;
		}
		char value = PeekNextOriginChar();
		middleText += value;
		consumedLength++;
		return value;
	}
}
