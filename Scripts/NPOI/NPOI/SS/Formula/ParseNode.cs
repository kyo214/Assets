using System;
using NPOI.SS.Formula.PTG;

namespace NPOI.SS.Formula;

internal class ParseNode
{
	private class TokenCollector
	{
		private Ptg[] _ptgs;

		private int _offset;

		public TokenCollector(int tokenCount)
		{
			_ptgs = new Ptg[tokenCount];
			_offset = 0;
		}

		public int sumTokenSizes(int fromIx, int ToIx)
		{
			int num = 0;
			for (int i = fromIx; i < ToIx; i++)
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

		public Ptg[] GetResult()
		{
			return _ptgs;
		}
	}

	public static ParseNode[] EMPTY_ARRAY = new ParseNode[0];

	private Ptg _token;

	private ParseNode[] _children;

	private bool _isIf;

	private int _tokenCount;

	private int TokenCount => _tokenCount;

	public int EncodedSize
	{
		get
		{
			int num = ((_token is ArrayPtg) ? 8 : _token.Size);
			for (int i = 0; i < _children.Length; i++)
			{
				num += _children[i].EncodedSize;
			}
			return num;
		}
	}

	public ParseNode(Ptg token, ParseNode[] children)
	{
		_token = token;
		_children = (ParseNode[])children.Clone();
		_isIf = IsIf(token);
		int num = 1;
		for (int i = 0; i < children.Length; i++)
		{
			num += children[i].TokenCount;
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

	public static Ptg[] ToTokenArray(ParseNode rootNode)
	{
		TokenCollector tokenCollector = new TokenCollector(rootNode.TokenCount);
		rootNode.CollectPtgs(tokenCollector);
		return tokenCollector.GetResult();
	}

	private void CollectPtgs(TokenCollector temp)
	{
		if (IsIf(_token))
		{
			CollectIfPtgs(temp);
			return;
		}
		bool flag = _token is MemFuncPtg || _token is MemAreaPtg;
		if (flag)
		{
			temp.Add(_token);
		}
		for (int i = 0; i < GetChildren().Length; i++)
		{
			GetChildren()[i].CollectPtgs(temp);
		}
		if (!flag)
		{
			temp.Add(_token);
		}
	}

	private void CollectIfPtgs(TokenCollector temp)
	{
		GetChildren()[0].CollectPtgs(temp);
		int num = temp.CreatePlaceholder();
		GetChildren()[1].CollectPtgs(temp);
		int num2 = temp.CreatePlaceholder();
		AttrPtg token = AttrPtg.CreateIf(temp.sumTokenSizes(num + 1, num2) + 4);
		if (GetChildren().Length > 2)
		{
			GetChildren()[2].CollectPtgs(temp);
			int num3 = temp.CreatePlaceholder();
			AttrPtg token2 = AttrPtg.CreateSkip(temp.sumTokenSizes(num2 + 1, num3) + 4 + 4 - 1);
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
		temp.Add(GetToken());
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

	public Ptg GetToken()
	{
		return _token;
	}

	public ParseNode[] GetChildren()
	{
		return _children;
	}
}
