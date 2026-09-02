using System;
using NPOI.SS.Formula.PTG;

namespace NPOI.HSSF.Model;

internal class ParseNode
{
	private class TokenCollector
	{
		private Ptg[] _ptgs;

		private int _offset;

		public Ptg[] Result => _ptgs;

		public TokenCollector(int tokenCount)
		{
			_ptgs = new Ptg[tokenCount];
			_offset = 0;
		}

		public int SumTokenSizes(int fromIx, int toIx)
		{
			int num = 0;
			for (int i = fromIx; i < toIx; i++)
			{
				num += _ptgs[i].Size;
			}
			return num;
		}

		public int CreatePlaceholder()
		{
			return _offset++;
		}

		public void Add(Ptg token)
		{
			if (token == null)
			{
				throw new ArgumentException("token must not be null");
			}
			_ptgs[_offset] = token;
			_offset++;
		}

		public void SetPlaceholder(int index, Ptg token)
		{
			if (_ptgs[index] != null)
			{
				throw new InvalidOperationException("Invalid placeholder index (" + index + ")");
			}
			_ptgs[index] = token;
		}
	}

	public static ParseNode[] EMPTY_ARRAY = new ParseNode[0];

	private Ptg _token;

	private ParseNode[] _children;

	private bool _isIf;

	private int _tokenCount;

	public Ptg Token => _token;

	public ParseNode[] Children => _children;

	public ParseNode(Ptg token, ParseNode[] children)
	{
		_token = token;
		_children = children;
		_isIf = IsIf(token);
		int num = 1;
		for (int i = 0; i < children.Length; i++)
		{
			num += children[i].GetTokenCount();
		}
		if (_isIf)
		{
			num += children.Length;
		}
		_tokenCount = num;
	}

	public ParseNode(Ptg token)
		: this(token, EMPTY_ARRAY)
	{
	}

	public ParseNode(Ptg token, ParseNode child0)
		: this(token, new ParseNode[1] { child0 })
	{
	}

	public ParseNode(Ptg token, ParseNode child0, ParseNode child1)
		: this(token, new ParseNode[2] { child0, child1 })
	{
	}

	private int GetTokenCount()
	{
		return _tokenCount;
	}

	public static Ptg[] ToTokenArray(ParseNode rootNode)
	{
		TokenCollector tokenCollector = new TokenCollector(rootNode.GetTokenCount());
		rootNode.CollectPtgs(tokenCollector);
		return tokenCollector.Result;
	}

	private void CollectPtgs(TokenCollector temp)
	{
		if (IsIf(Token))
		{
			CollectIfPtgs(temp);
			return;
		}
		for (int i = 0; i < Children.Length; i++)
		{
			Children[i].CollectPtgs(temp);
		}
		temp.Add(Token);
	}

	private void CollectIfPtgs(TokenCollector temp)
	{
		Children[0].CollectPtgs(temp);
		int num = temp.CreatePlaceholder();
		Children[1].CollectPtgs(temp);
		int num2 = temp.CreatePlaceholder();
		AttrPtg token = AttrPtg.CreateIf(temp.SumTokenSizes(num + 1, num2) + 4);
		if (Children.Length > 2)
		{
			Children[2].CollectPtgs(temp);
			int num3 = temp.CreatePlaceholder();
			AttrPtg token2 = AttrPtg.CreateSkip(temp.SumTokenSizes(num2 + 1, num3) + 4 + 4 - 1);
			AttrPtg token3 = AttrPtg.CreateSkip(3);
			temp.SetPlaceholder(num, token);
			temp.SetPlaceholder(num2, token2);
			temp.SetPlaceholder(num3, token3);
		}
		else
		{
			AttrPtg token4 = AttrPtg.CreateSkip(3);
			temp.SetPlaceholder(num, token);
			temp.SetPlaceholder(num2, token4);
		}
		temp.Add(_token);
	}

	private static bool IsIf(Ptg token)
	{
		if (token is FuncVarPtg)
		{
			FuncVarPtg funcVarPtg = (FuncVarPtg)token;
			if ("IF".Equals(funcVarPtg.Name))
			{
				return true;
			}
		}
		return false;
	}
}
