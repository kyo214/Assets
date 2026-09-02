namespace System.Net.Http.Headers;

internal struct Token
{
	public enum Type
	{
		Error = 0,
		End = 1,
		Token = 2,
		QuotedString = 3,
		SeparatorEqual = 4,
		SeparatorSemicolon = 5,
		SeparatorSlash = 6,
		SeparatorDash = 7,
		SeparatorComma = 8,
		OpenParens = 9
	}

	public static readonly Token Empty = new Token(Type.Token, 0, 0);

	private readonly Type type;

	public int StartPosition { get; private set; }

	public int EndPosition { get; private set; }

	public Type Kind => type;

	public Token(Type type, int startPosition, int endPosition)
	{
		this = default;
		this.type = type;
		StartPosition = startPosition;
		EndPosition = endPosition;
	}

	public static implicit operator Type(Token token)
	{
		return token.type;
	}

	public override string ToString()
	{
		return type.ToString();
	}
}
